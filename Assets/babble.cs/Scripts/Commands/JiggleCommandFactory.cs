using System;

namespace Babble.Commands {
    public class JiggleCommandFactory : CommandFactory {

        public override Command GetCommand() {
            return new JiggleCommand();
        }

        public class JiggleCommand : Command {

            public int target;

            public override void Act(Cutscene cutscene, Action callback) {
                Puppet puppet = cutscene.stage.GetPuppet(target);

                puppet.Jiggle();

                callback();
            }
        }
    }
}
