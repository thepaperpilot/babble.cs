using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Babble {

    [Serializable]
    public class Puppet {

        public static float MOVE_DURATION = 0.75f;

        [Serializable]
        public struct Emote {
            public bool enabled;
            public Asset[] mouth;
            public Asset[] eyes;
            public string name;
        }

        // Note this is a different type of asset than that on the stage
        // this is the id of the stage asset to use, and data on how to
        // position, rotate, and scale it on the puppet
        [Serializable]
        public struct Asset {
            public float x;
            public float y;
            public float rotation;
            public float scaleX;
            public float scaleY;
            public string id;
        }

        // Serialized values
        public bool deadbonesStyle;
        public Asset[] body;
        public Asset[] head;
        public Asset[] hat;
        public Asset[] props;
        public int[] mouths;
        public int[] eyes;
        public Emote[] emotes;
        public string name;
        public int id;
        public int position;

        // serialized, with custom setters for private values
        // so that we can update things when the values change
        // also they're not capitalized because that's unity's
        // standard, yes I'm aware its against MS's standard
        public bool facingLeft {
            get { return _facingLeft; }
            set {
                _facingLeft = value;
                if (gameObject != null && _facingLeft != gameObject.transform.localScale.x < 0) {
                    Vector3 scale = gameObject.transform.localScale;
                    scale.x *= -1;
                    gameObject.transform.localScale = scale;
                }
            }
        }
        public int emote {
            get { return _emote; }
            set {
                _emote = value;
                if (gameObject != null) {
                    for (int i = 0; i < emotes.Length; i++) {
                        eyesContainers[i].SetActive(false);
                        mouthContainers[i].SetActive(false);
                    }
                    eyesContainers[emote].SetActive(true);
                    mouthContainers[emote].SetActive(true);
                }
            }
        }
        public int target {
            get { return _target; }
            set {
                _target = value;
                movingAnim = 0;
            }
        }
        public bool babbling {
            get { return _babbling; }
            set {
                _babbling = value;
                if (!babbling) {
                    ChangeEmote(emote);

                    if (deadbonesStyle) {
                        deadbonesAnim = 0;
                        deadbonesDuration = .1f;
                        deadbonesTargetY = -GetMaxBounds(headBaseContainer).size.y / 2f;
                        deadbonesTargetRotation = Quaternion.identity;
                        deadbonesStartY = headContainer.transform.localPosition.y;
                        deadbonesStartRotation = headContainer.transform.localRotation;
                    }
                }
            }
        }

        [NonSerialized]
        public GameObject gameObject;
        [NonSerialized]
        public Stage stage;

        // private fields with public getters/setters
        private bool _facingLeft;
        private int _emote;
        private int _target;
        private bool _babbling;
        
        private float movingAnim = 0;
        private float eyesAnim = 0;
        private float mouthAnim = 0;
        private float deadbonesAnim = 0;
        private float eyesDuration = 0;
        private float mouthDuration = 0;
        private float deadbonesDuration = 0;
        private float deadbonesTargetY = 0;
        private float deadbonesStartY = 0;
        private Quaternion deadbonesTargetRotation = Quaternion.identity;
        private Quaternion deadbonesStartRotation = Quaternion.identity;
        [SerializeField]
        private float eyesBabbleDuration = 2;
        [SerializeField]
        private float mouthBabbleDuration = .27f;

        private GameObject bodyContainer;
        private GameObject headContainer;
        private GameObject headBaseContainer;
        private GameObject mouthsContainer;
        private GameObject eyesContainer;
        private GameObject hatContainer;
        private GameObject propsContainer;
        private List<GameObject> mouthContainers = new List<GameObject>();
        private List<GameObject> eyesContainers = new List<GameObject>();

        public void Setup() {
            bodyContainer = new GameObject();
            headContainer = new GameObject();
            headBaseContainer = new GameObject();
            mouthsContainer = new GameObject();
            eyesContainer = new GameObject();
            hatContainer = new GameObject();
            propsContainer = new GameObject();

            gameObject.name = name;
            bodyContainer.transform.SetParent(gameObject.transform);
            bodyContainer.name = "Body";
            headContainer.transform.SetParent(gameObject.transform);
            headContainer.name = "Head";
            propsContainer.transform.SetParent(gameObject.transform);
            propsContainer.name = "Props";

            headBaseContainer.transform.SetParent(headContainer.transform);
            headBaseContainer.name = "Base";
            mouthsContainer.transform.SetParent(headContainer.transform);
            mouthsContainer.name = "Mouths";
            eyesContainer.transform.SetParent(headContainer.transform);
            eyesContainer.name = "Eyes";
            hatContainer.transform.SetParent(headContainer.transform);
            hatContainer.name = "Hat";

            for (int i = 0; i < emotes.Length; i++) {
                GameObject eyes = new GameObject();
                eyes.name = emotes[i].name;
                eyesContainers.Add(eyes);
                eyes.transform.SetParent(eyesContainer.transform);
                GameObject mouth = new GameObject();
                mouth.name = emotes[i].name;
                mouthContainers.Add(mouth);
                mouth.transform.SetParent(mouthsContainer.transform);
            }

            for (int i = 0; i < body.Length; i++) {
                AddAsset(body[i], bodyContainer);
            }

            for (int i = 0; i < head.Length; i++) {
                AddAsset(head[i], headBaseContainer);
            }
            for (int i = 0; i < emotes.Length; i++) {
                for (int j = 0; j < emotes[i].mouth.Length; j++) {
                    AddAsset(emotes[i].mouth[j], mouthContainers[i]);
                }
                for (int j = 0; j < emotes[i].eyes.Length; j++) {
                    AddAsset(emotes[i].eyes[j], eyesContainers[i]);
                }
            }
            for (int i = 0; i < hat.Length; i++) {
                AddAsset(hat[i], hatContainer);
            }

            for (int i = 0; i < props.Length; i++) {
                AddAsset(props[i], propsContainer);
            }

            target = position;
            ChangeEmote(emote);
            gameObject.transform.localScale = new Vector3(stage.puppetScale, stage.puppetScale, 0);
            // Calls the setter
            facingLeft = _facingLeft;
        }

        // This isn't a monobehavior, so we need to call it from stage
        public void Update() {
            // Movement animations
            // I've tried to emulate what puppet pals does as closely as possible
            // But frankly it's difficult to tell
            if (target != position || movingAnim != 0) {
                // Whether its going left or right
                int direction = target > position ? 1 : -1;
                // Update how far into the animation we are
                movingAnim += Time.deltaTime / MOVE_DURATION;

                // We want to do a bit of animation when they arrive at the target slot. 
                //  in order to do that we have part of the animation (0 - .6) be for each slot
                //  and the rest (.6 - 1) only plays at the destination slot
                if (movingAnim >= 0.6f && movingAnim - Time.deltaTime / MOVE_DURATION < 0.6f) {
                    // Once we pass .6, update our new slot position
                    position += direction;
                    // If we're not at the final slot yet, reset the animation
                    if (position != target) movingAnim = 0;
                } else if (movingAnim >= 1) movingAnim = 0;
                // Scale in a sin formation such that it does 3 half circles per slot, plus 2 more at the end
                float scaleY = (1 + Mathf.Sin((1 + movingAnim * 5f) * Mathf.PI) / 40f) * stage.puppetScale;
                gameObject.transform.localScale = new Vector2(gameObject.transform.localScale.x, scaleY);
                // Update y value so it doesn't leave the bottom of the screen while bouncing
                Vector3 bounds = GetMaxBounds(gameObject).size;
                // Linearly move across the slot, unless we're in the (.6 - 1) part of the animation, and ensure we're off screen even when the puppets are large
                float interpolation = Mathf.Min(1, movingAnim / 0.6f);
                float start = GetSlotPosition(position);
                float end = GetSlotPosition(position + direction);
                float posX = interpolation == 1 ? start : start + (end - start) * interpolation;

                // Wrap Edges
                if (target > stage.numSlots + 1 && position >= stage.numSlots + 1 && movingAnim > 0) {
                    position = 0;
                    posX = GetSlotPosition(position);
                    target -= stage.numSlots + 1;
                }
                if (target < 0 && position <= 0 && movingAnim > 0) {
                    position = stage.numSlots + 1;
                    posX = GetSlotPosition(position);
                    target += stage.numSlots + 1;
                }

                // Apply new position
                gameObject.transform.position = new Vector3(posX, 0, 0);
            }

            if (babbling) {
                // Update how long each face part has been on display
                eyesAnim += Time.deltaTime;
                mouthAnim += Time.deltaTime;

                // Update eyes
                if (eyesAnim >= eyesDuration && eyes.Length > 0 && (emote == 0 || !emotes[emote].enabled)) {
                    mouthContainers[0].SetActive(false);
                    for (int i = 0; i < eyes.Length; i++) {
                        eyesContainers[eyes[i]].SetActive(false);
                    }
                    int random = eyes[(int)Mathf.Floor(UnityEngine.Random.value * eyes.Length)];
                    eyesContainers[random].SetActive(true);
                    eyesAnim = 0;
                    eyesDuration = (0.1f + UnityEngine.Random.value) * eyesBabbleDuration;
                }

                // Update mouth
                if (mouthAnim >= mouthDuration && mouths.Length > 0) {
                    mouthContainers[emote].SetActive(false);
                    mouthContainers[0].SetActive(false);
                    for (int i = 0; i < mouths.Length; i++) {
                        mouthContainers[mouths[i]].SetActive(false);
                    }
                    int random = mouths[(int)Mathf.Floor(UnityEngine.Random.value * mouths.Length)];
                    mouthContainers[random].SetActive(true);
                    mouthAnim = 0;
                    mouthDuration = (0.1f + UnityEngine.Random.value) * mouthBabbleDuration;
                }
            }
            // Update DeadbonesStyle Babbling
            // I'm not sure what Puppet Pals does, but I'm pretty sure this isn't it
            // But I think this looks "close enough", and probably the best I'm going
            // to get without Rob actually telling people how Puppet Pals does it
            if (deadbonesStyle && (babbling || deadbonesDuration != 0)) {
                deadbonesAnim += Time.deltaTime;
                if (deadbonesAnim >= deadbonesDuration) {
                    deadbonesAnim = 0;
                    Bounds bounds = GetMaxBounds(headBaseContainer);
                    deadbonesStartY = deadbonesTargetY;
                    deadbonesStartRotation = deadbonesTargetRotation;
                    headContainer.transform.localPosition = new Vector3(headContainer.transform.localPosition.x, deadbonesTargetY, headContainer.transform.localPosition.z);
                    headContainer.transform.localRotation = deadbonesTargetRotation;
                    if (babbling) {
                        deadbonesDuration = .1f + UnityEngine.Random.value * .2f;
                        deadbonesTargetY = UnityEngine.Random.value * -20f - bounds.size.y / 2f;
                        deadbonesTargetRotation = Quaternion.Euler(new Vector3(0, 0, 1f - UnityEngine.Random.value * 2f));
                    } else {
                        deadbonesDuration = 0;
                    }
                } else {
                    float percent = (deadbonesAnim / deadbonesDuration) * (deadbonesAnim / deadbonesDuration);
                    headContainer.transform.localPosition = new Vector3(headContainer.transform.localPosition.x, deadbonesStartY + (deadbonesTargetY - deadbonesStartY) * percent, headContainer.transform.localPosition.z);
                    headContainer.transform.localRotation = Quaternion.Lerp(deadbonesStartRotation, deadbonesTargetRotation, percent);
                }
            }
        }

        public void WrapPosition() {
            if (position > stage.numSlots + 1 || target > stage.numSlots + 1) {
                position = target = stage.numSlots + 1;
                movingAnim = 0;
            }
        }

        public void UpdatePosition() {
            float x = GetSlotPosition(position);
            gameObject.transform.position = new Vector3(x, 0, 0);
        }

        Bounds GetMaxBounds(GameObject gameObject) {
            Bounds bounds = new Bounds(gameObject.transform.position, Vector3.zero);
            bool foundChild = false;
            foreach (Image image in gameObject.GetComponentsInChildren<Image>()) {
                if (!foundChild) {
                    bounds = image.sprite.bounds;
                    foundChild = true;
                } else {
                    bounds.Encapsulate(image.sprite.bounds);
                }
            }
            return bounds;
        }

        float GetSlotPosition(int position) {
            Vector3 bounds = GetMaxBounds(gameObject).size;
            float width = bounds.x * Mathf.Abs(gameObject.transform.lossyScale.x) * 50;
            return position <= 0 ? -width :                 // Starting left of screen
                position >= stage.numSlots + 1 ?
                stage.numSlots * stage.slotWidth + width :   // Starting right of screen
                (position - 0.5f) * stage.slotWidth;         // Starting on screen
        }

        void AddAsset(Asset asset, GameObject container) {
            GameObject gameObject = stage.assets[asset.id].CreateGameObject(this, asset);
            gameObject.transform.SetParent(container.transform);
        }

        public void ChangeEmote(int emote = 0) {
            if (!emotes[emote].enabled)
                emote = 0;

            this.emote = emote;
        }

        public void MoveLeft() {
            if (target > position) return;
            if (facingLeft || position == 0 || position == stage.numSlots + 1) {
                target--;
            }
            facingLeft = true;
        }

        public void MoveRight() {
            if (target < position) return;
            if (!facingLeft || position == 0 || position == stage.numSlots + 1) {
                target++;
            }
            facingLeft = false;
        }

        public void SetBabbling(bool babbling) {
            if (this.babbling == babbling) return;
            this.babbling = babbling;
        }

        public void Jiggle() {
            // In JS I didn't have any issues just using 0.6. The only conclusion
            // I can draw is that Unity and/or c# has less precision in floating point numbers
            if (movingAnim == 0) movingAnim = 0.6001f;
        }
    }
}
