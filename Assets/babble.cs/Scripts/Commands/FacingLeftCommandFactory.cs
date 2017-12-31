using System;

namespace Babble.Commands {
    // I'm not a fan of object oriented programming
    // If you're reading this, be aware in babble.cs 
    // it only has stage, puppet, and cutscene files
    // All the assets are handled in stage, and all 
    // the commands are handled in cutscene
    // Sorry, I was just reminded about this dislike
    // because of this long class name
    public class FacingLeftCommandFactory : CommandFactory {

        public override Command GetCommand() {
            return new FacingLeftCommand();
        }

        public class FacingLeftCommand : Command {

            public int target;

            public bool facingLeft = true;

            public override void Act(Cutscene cutscene, Action callback) {
                Puppet puppet = cutscene.stage.GetPuppet(target);
                puppet.facingLeft = facingLeft;

                callback();
            }
        }
    }
}
