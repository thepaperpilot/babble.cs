using System;

namespace Babble.Commands {
    public class SetCommandFactory : CommandFactory {

        public override Command GetCommand() {
            return new SetCommand();
        }

        public class SetCommand : Command {

            public string name;
            public int target;

            public override void Act(Cutscene cutscene, Action callback) {
                cutscene.stage.SetPuppet(target, cutscene.actors[name]);

                callback();
            }
        }
    }
}
