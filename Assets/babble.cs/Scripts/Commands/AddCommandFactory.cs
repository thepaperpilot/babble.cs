using System;

namespace Babble.Commands {
    public class AddCommandFactory : CommandFactory {

        public override Command GetCommand() {
            return new AddCommand();
        }

        public class AddCommand : Command {

            public string name;
            public int id;

            public int position = 0;
            public bool facingLeft = false;
            public int emote;

            public override void Act(Cutscene cutscene, Action callback) {
                Puppet puppet = cutscene.stage.AddPuppet(cutscene.actors[name], id);
                puppet.position = position;
                puppet.SetTarget(position);
                puppet.UpdatePosition();
                puppet.facingLeft = facingLeft;
                puppet.emote = emote;

                callback();
            }
        }
    }
}
