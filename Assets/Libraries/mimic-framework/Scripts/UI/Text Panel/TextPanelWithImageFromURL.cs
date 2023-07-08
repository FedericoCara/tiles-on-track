using System.Collections;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

using Mimic;

namespace Mimic.UI {

    public class TextPanelWithImageFromURL : TextPanel {

        [SerializeField]
        protected Image image;
        public virtual Image Image {
            get => image == null ? image = graphicContainer.GetComponentInChildren<Image>() : image;
        }
        
        public virtual string ImageURL {
            set{
                Image.sprite = null;
                StartCoroutine(GetImageFromURL(value));
            }
        }
        
        public virtual void Set(string title, string content, string imageURL) {
            Set(title, content, true);
            ImageURL = imageURL;
        }

        protected virtual IEnumerator GetImageFromURL(string url) {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
            yield return www.SendWebRequest();

            if(www.isNetworkError || www.isHttpError) {
                Debug.LogWarning("Error downloading image from " + url);
                graphicContainer.SetActive(false);
            } else {
                Texture2D texture = ((DownloadHandlerTexture) www.downloadHandler).texture;
                bodyTxt.TextComponent.alignment = TextAnchor.MiddleRight;
                Image.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                Image.sprite = sprite;
                Image.enabled = true;
            }
        }

    }

}