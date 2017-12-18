using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Pandora.GameManager {
    public class ExamineWindow : MonoBehaviour {

        [SerializeField] private Image icon;
        [SerializeField] private Text description;

        public void SetIcon(Sprite newIcon)
        {
            icon.sprite = newIcon;
        }

        public void SetDescription(string newDesc)
        {
            description.text = newDesc.Replace("\\n","\n");
        }
    }
}