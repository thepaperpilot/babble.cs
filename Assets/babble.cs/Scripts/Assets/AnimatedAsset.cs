using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace Babble.Assets {
    [Serializable]
    public class AnimatedAsset : Asset {

        public int rows;
        public int cols;
        public int numFrames;
        public float delay;

        public override GameObject CreateGameObject(Puppet puppet, Puppet.Asset asset) {
            GameObject gameObject = CreateGenericGameObject(asset);

            string path = Path.ChangeExtension(puppet.stage.assetsPath + location, null);
            Texture2D texture = Resources.Load<Texture2D>(path);
            List<Sprite> sprites = new List<Sprite>();
            int row = 0;
            int col = 0;
            float height = texture.height / rows;
            float width = texture.width / cols;
            for (int i = 0; i < numFrames; i++) {
                Sprite frame = Sprite.Create(texture, new Rect(col * width, row * height, width, height), new Vector2(.5f, .5f));
                sprites.Add(frame);
                col++;
                if (col >= cols) {
                    col = 0;
                    row++;
                }
            }
            Image image = gameObject.AddComponent<Image>();
            image.sprite = sprites[0];
            image.rectTransform.sizeDelta = new Vector2(sprites[0].rect.width, sprites[0].rect.height);
            Animator animator = gameObject.AddComponent<Animator>();
            animator.sprites = sprites;
            animator.frame = 0;
            animator.delay = delay / 1000f;

            return gameObject;
        }

        [RequireComponent(typeof(Image))]
        class Animator : MonoBehaviour {

            public List<Sprite> sprites;
            public int frame;
            public float delay;

            private float time = 0;
            private Image image;

            private void Awake() {
                image = GetComponent<Image>();
            }

            void Update() {
                time += Time.deltaTime;
                if (time >= delay) {
                    time -= delay;
                    frame++;
                    if (frame == sprites.Count) frame = 0;
                    image.sprite = sprites[frame];
                }
            }

        }
    }
}
