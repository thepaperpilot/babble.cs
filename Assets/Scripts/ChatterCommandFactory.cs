using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Babble.Commands {
    public class ChatterCommandFactory : CommandFactory {

        public GameObject chatboxPrefab;

        public override Command GetCommand() {
            return new ChatterCommand {
                chatboxPrefab = this.chatboxPrefab
            };
        }

        public class ChatterCommand : Command {

            public GameObject chatboxPrefab;

            private bool chatClicked = false;
            private int textPos = 0;
            private GameObject chatboxInstance;
            private Text text;

            public string chat;
            public bool babbleQuotes;
            public int target;

            public float characterInterval = 0.04f;

            public override void Act(Cutscene cutscene, Action callback) {
                chatboxInstance = GameObject.Instantiate(chatboxPrefab, cutscene.stage.GetPuppet(target).gameObject.transform);
                text = chatboxInstance.GetComponentInChildren<Text>();
                chatboxInstance.GetComponent<Button>().onClick.AddListener(OnClick);
                cutscene.stage.StartCoroutine(Chatter(cutscene, callback));
            }

            IEnumerator Chatter(Cutscene cutscene, Action callback) {
                Puppet puppet = cutscene.stage.GetPuppet(target);
                while (true) {
                    if (chatClicked && textPos < chat.Length) {
                        puppet.SetBabbling(false);
                        textPos = chat.Length;
                        chatClicked = false;
                    }

                    if (++textPos > chat.Length) {
                        if (chatClicked) {
                            GameObject.Destroy(chatboxInstance);
                            
                            callback();
                            break;
                        }
                    } else {
                        char currentCharacter = chat.ToCharArray()[textPos - 1];
                        if (currentCharacter == '“' || (currentCharacter == '"' && !puppet.babbling))
                            puppet.SetBabbling(true);
                        else if (currentCharacter == '”' || (currentCharacter == '"' && puppet.babbling))
                            puppet.SetBabbling(false);

                        text.text = chat.Substring(0, textPos) + "_";
                    }
                    
                    yield return new WaitForSeconds(characterInterval);
                }
            }

            void OnClick() {
                chatClicked = true;
            }
        }
    }
}
