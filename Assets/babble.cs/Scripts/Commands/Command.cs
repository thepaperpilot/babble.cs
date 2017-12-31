using System;

namespace Babble.Commands {
    public class Command {

        public bool wait;

        // Perform act, and call callback (think of it as a promise)
        public virtual void Act(Cutscene cutscene, Action callback) {
            callback();
        }
    }
}
