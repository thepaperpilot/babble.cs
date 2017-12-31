using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace Babble.Assets {
    [Serializable]
    public class Asset {

        // Commented fields are fields present in the JSON, but which
        // are ignored in this implementation of babble

        [Serializable]
        public struct Position {
            int x;
            int y;
        }

        public string name;
        public string location;
        //public string tab;
        public string type;
        //public int version;
        //public Position[] panning;

        public virtual GameObject CreateGameObject(Puppet puppet, Puppet.Asset asset) {
            GameObject gameObject = CreateGenericGameObject(asset);

            string path = Path.ChangeExtension(puppet.stage.assetsPath + location, null);
            Sprite sprite = Resources.Load<Sprite>(path);
            Image image = gameObject.AddComponent<Image>();
            image.sprite = sprite;
            image.rectTransform.sizeDelta = new Vector2(sprite.rect.width, sprite.rect.height);

            return gameObject;
        }

        protected GameObject CreateGenericGameObject(Puppet.Asset asset) {
            GameObject gameObject = new GameObject();
            gameObject.name = name;
            gameObject.transform.position = new Vector3(asset.x, -asset.y, 0);
            gameObject.transform.localScale = new Vector3(asset.scaleX, asset.scaleY, 1);
            gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, asset.rotation));

            return gameObject;
        }
    }
}
