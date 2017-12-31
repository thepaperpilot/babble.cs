using System;

namespace Babble.Commands {
    public class EmoteCommandFactory : CommandFactory {

        public override Command GetCommand() {
            return new EmoteCommand();
        }

        public class EmoteCommand : Command {

            public int target;

            public int emote = 0;

            public override void Act(Cutscene cutscene, Action callback) {
                Puppet puppet = cutscene.stage.GetPuppet(target);
                puppet.ChangeEmote(emote);

                callback();
            }
        }
    }
}
