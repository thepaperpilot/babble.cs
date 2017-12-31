using Babble.Commands;
using SimpleJSON;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Babble {

    public class Cutscene {

        public Dictionary<string, CommandFactory> commandFactories = new Dictionary<string, CommandFactory>();

        public Stage stage;
        public Dictionary<string, string> actors;

        public Cutscene(Stage stage, Dictionary<string, string> actors) {
            this.stage = stage;
            this.actors = actors;

            // Add default commands
            commandFactories.Add("run", new RunCommandFactory());
            commandFactories.Add("add", new AddCommandFactory());
            commandFactories.Add("set", new SetCommandFactory());
            commandFactories.Add("remove", new RemoveCommandFactory());
            commandFactories.Add("delay", new DelayCommandFactory());
            commandFactories.Add("move", new MoveCommandFactory());
            commandFactories.Add("facingLeft", new FacingLeftCommandFactory());
            commandFactories.Add("babble", new BabbleCommandFactory());
            commandFactories.Add("emote", new EmoteCommandFactory());
            commandFactories.Add("jiggle", new JiggleCommandFactory());
        }

        public List<Command> ReadScript(string script) {
            List<Command> list = new List<Command>();
            JSONArray lines = JSONNode.Parse(script) as JSONArray;

            if (lines == null) {
                Debug.LogError("Script invalid. Expected array of objects\nScript: " + script);
                return list;
            }

            foreach (JSONNode line in lines) {
                if (!commandFactories.ContainsKey(line["command"].Value)) {
                    Debug.LogError("Command " + line["command"].Value + " in script not found. Skipping...");
                    continue;
                }
                CommandFactory factory = commandFactories[line["command"].Value];
                Command command = factory.GetCommand();
                factory.ApplyCommand(command, line.ToString());
                list.Add(command);
            }

            return list;
        }

        public void Start(List<Command> script, Action callback = null) {
            callback = callback ?? NOOP;
            ParseNextCommand(script, callback);
        }

        public void ParseNextCommand(List<Command> script, Action callback) {
            // If the cutscene is over
            if (script.Count == 0) {
                callback();
                return;
            }

            // If the current command is invalid
            if (script[0] == null) {
                callback();
                return;
            }

            // Handle command
            if (script[0].wait) {
                // Act, then parse next command when complete
                script[0].Act(this, delegate { ParseNextCommand(script.GetRange(1, script.Count - 1), callback); });
            } else {
                // Act, and do nothing when complete
                script[0].Act(this, NOOP);
                // Immediately parse next complete
                // TODO stack overflow concerns
                ParseNextCommand(script.GetRange(1, script.Count - 1), callback);
            }
        }

        public void NOOP() {
            // No Operation
            // This should be the tail of every callback chain
        }
    }
}
