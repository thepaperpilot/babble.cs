using System;
using System.Collections;
using UnityEngine;

namespace Babble.Commands {
    public class DelayCommandFactory : CommandFactory {

        public override Command GetCommand() {
            return new DelayCommand();
        }

        public class DelayCommand : Command {

            public float delay; // in ms

            public override void Act(Cutscene cutscene, Action callback) {
                cutscene.stage.StartCoroutine(Delay(delay / 1000f, callback));
            }

            IEnumerator Delay(float seconds, Action callback) {
                yield return new WaitForSeconds(seconds);
                callback();
            }
        }
    }
}
