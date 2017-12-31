using System;
using UnityEngine;

namespace Babble.Commands {
    public class MoveCommandFactory : CommandFactory {

        public override Command GetCommand() {
            return new MoveCommand();
        }

        public class MoveCommand : Command {
            
            public int target;
            public int position;

            public override void Act(Cutscene cutscene, Action callback) {
                Puppet puppet = cutscene.stage.GetPuppet(target);
                puppet.SetTarget(position, true);

                if (position > puppet.position) {
                    puppet.facingLeft = false;
                } else if (position != puppet.position) {
                    puppet.facingLeft = true;
                }

                DelayCommandFactory.DelayCommand command = (DelayCommandFactory.DelayCommand) cutscene.commandFactories["delay"].GetCommand();
                command.delay = (Mathf.Abs(puppet.GetTarget() - puppet.position) * Puppet.MOVE_DURATION * 0.6f + Puppet.MOVE_DURATION * 0.4f) * 1000;
                command.Act(cutscene, delegate { callback(); });
            }
        }
    }
}
