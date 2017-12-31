using UnityEngine;

namespace Babble.Commands {
    public abstract class CommandFactory {
        public abstract Command GetCommand();

        public virtual void ApplyCommand(Command command, string json) {
            JsonUtility.FromJsonOverwrite(json, command);
        }
    }
}
