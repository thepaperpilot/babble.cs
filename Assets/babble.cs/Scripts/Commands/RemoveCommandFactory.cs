using System;

namespace Babble.Commands {
    public class RemoveCommandFactory : CommandFactory {

        public override Command GetCommand() {
            return new RemoveCommand();
        }

        public class RemoveCommand : Command {

            public int target;

            public override void Act(Cutscene cutscene, Action callback) {
                cutscene.stage.RemovePuppet(target);

                callback();
            }
        }
    }
}
