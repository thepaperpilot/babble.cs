using Babble.Assets;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Babble {

    [RequireComponent(typeof(Canvas))]
    public class Stage : MonoBehaviour {

        public TextAsset assetsJSON;    // JSON file with information on the various assets
        public string assetsPath;       // Path, relative to resources folder, where puppet assets are stored
        public int numSlots = 5;        // How many slots for puppets there are. Does not limit number of puppets, just how many distinct positions there are for puppets on the screen
        public float puppetScale = 1;

        [HideInInspector]
        public Dictionary<string, Asset> assets = new Dictionary<string, Asset>();
        [HideInInspector]
        public float slotWidth = 1;

        private Dictionary<int, Puppet> puppets = new Dictionary<int, Puppet>();
        private float lastScreenWidth = 0f;

        void Awake() {
            // Using a third party library to parse a dictionary
            // because that's what babble buds exports assets as,
            // and Unity's built in JSON utility doesn't support them
            JSONObject assetsDict = JSONNode.Parse(assetsJSON.text) as JSONObject;
            foreach (KeyValuePair<string, JSONNode> kvp in assetsDict) {
                switch (kvp.Value["type"].Value) {
                    default:
                    case "sprite":
                        // Pretty sure JsonUtility is still faster at deserialization
                        assets.Add(kvp.Key, JsonUtility.FromJson<Asset>(kvp.Value.ToString()));
                        break;
                    case "animated":
                        assets.Add(kvp.Key, JsonUtility.FromJson<AnimatedAsset>(kvp.Value.ToString()));
                        break;
                }
            }
        }

        void Start() {
            Resize();
        }

        void Update() {
            foreach (Puppet puppet in puppets.Values) {
                puppet.Update();
            }
        }

        void LastUpdate() {
            if (lastScreenWidth != Screen.width) {
                Resize();
            }
        }

        public void Resize() {
            RectTransform rectTransform = GetComponent<RectTransform>();
            Rect rect = rectTransform.rect;
            float scale = rectTransform.localScale.x;
            Resize(rect.width * scale, rect.height * scale);
        }

        public void Resize(float width, float height) {
            lastScreenWidth = Screen.width;
            slotWidth = width / numSlots;
            foreach (Puppet puppet in puppets.Values) {
                // Wrap puppet's position
                puppet.WrapPosition();
                // Face correct direction
                float x, y;
                x = y = puppetScale;
                if (puppet.facingLeft) x *= -1;
                puppet.gameObject.transform.localScale = new Vector2(x, y);
                // Position puppet
                puppet.UpdatePosition();
            }
        }

        public Puppet AddPuppet(string json, int id) {
            Puppet puppet = JsonUtility.FromJson<Puppet>(json);
            puppet.gameObject = new GameObject();
            puppet.stage = this;
            puppet.Setup();
            puppets.Add(id, puppet);
            puppet.gameObject.transform.parent = gameObject.transform;
            puppet.UpdatePosition();
            return puppet;
        }

        public Puppet GetPuppet(int id) {
            return puppets[id];
        }

        public void SetPuppet(int id, string json) {
            Puppet puppet = puppets[id];
            puppets.Remove(id);
            Puppet newPuppet = AddPuppet(json, id);

            newPuppet.ChangeEmote(puppet.emote);
            newPuppet.position = puppet.position;
            newPuppet.target = puppet.target;
            newPuppet.facingLeft = puppet.facingLeft;
            newPuppet.gameObject.transform.localPosition = puppet.gameObject.transform.localPosition;
            newPuppet.gameObject.transform.localScale = puppet.gameObject.transform.localScale;

            Destroy(puppet.gameObject);
        }

        public void RemovePuppet(int id) {
            Destroy(puppets[id].gameObject);
            puppets.Remove(id);
        }
    }
}
