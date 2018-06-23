using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TeamUtility.IO;

namespace CyberBullet.UI {
    public class FollowMouse : MonoBehaviour {
        public Image icon;
        public GameObject window;

        private Vector3 mouse;
        [SerializeField] private Vector3 offset = Vector3.zero;
        void OnEnable()
        {
            if (icon.sprite != null)
            {
                window.SetActive(true);
                Color cur = icon.color;
                cur.a = 1;
                icon.color = cur;
            }
            else if (icon.sprite == null)
            {
                window.SetActive(false);
            }
        }
        void Update () {
            mouse = Input.mousePosition + offset;
            this.GetComponent<RectTransform>().position = mouse;
            if (InputManager.GetMouseButtonUp(0))
            {
                Color cur = icon.color;
                cur.a = 0;
                icon.color = cur;
                icon.sprite = null;
                this.gameObject.SetActive(false);
            }
    	}
        public void SetIcon(Sprite newIcon)
        {
            icon.sprite = newIcon;
            Color cur = icon.color;
            cur.a = 1;
            icon.color = cur;
        }
    }
}