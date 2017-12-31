using SimpleJSON;
using System;

namespace Babble.Commands {
    public class RunCommandFactory : CommandFactory {

        public override Command GetCommand() {
            return new RunCommand();
        }

        public override void ApplyCommand(Command command, string json) {
            JSONObject jsonObject = JSONNode.Parse(json) as JSONObject;
            ((RunCommand) command).script = jsonObject["script"].ToString();
        }

        public class RunCommand : Command {

            public string script;

            public override void Act(Cutscene cutscene, Action callback) {
                if (wait) {
                    cutscene.ParseNextCommand(cutscene.ReadScript(script), callback);
                } else {
                    cutscene.ParseNextCommand(cutscene.ReadScript(script), cutscene.NOOP);
                    // TODO stack overflow concerns?
                    callback();
                }
            }
        }
    }
}
