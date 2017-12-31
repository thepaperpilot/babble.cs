using System;

namespace Babble.Commands {
    public class BabbleCommandFactory : CommandFactory {

        public override Command GetCommand() {
            return new BabbleCommand();
        }

        public class BabbleCommand : Command {

            public int target;

            public string action = "toggle";

            public override void Act(Cutscene cutscene, Action callback) {
                Puppet puppet = cutscene.stage.GetPuppet(target);

                bool babble = action == "toggle" ? !puppet.babbling : action == "start";
                puppet.babbling = babble;

                callback();
            }
        }
    }
}
