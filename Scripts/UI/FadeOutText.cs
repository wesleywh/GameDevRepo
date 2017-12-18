using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeOutText : MonoBehaviour {

    public float fadeOutSpeed = 0.5f;
    public float waitBeforeFade = 6.0f;

    private bool startFade = false;
    private string text = "";
    private float timer = 0;
    private float guiAlpha = 0;
    private Text textComp;
    private Color org;
    void Start() {
        textComp = GetComponent<Text>();
    }
    void Update() {
        if (startFade == false)
        {
            if (textComp.text != "")
            {
                if (textComp.text != text)
                {
                    Reset();
                }
            }
        }
        if (startFade == true) {
            if (textComp.text != text)
            {
                startFade = false;
            }
            else
            {
                timer += Time.deltaTime;
                if (timer >= waitBeforeFade)
                {
                    guiAlpha -= Time.deltaTime * fadeOutSpeed;
                    org = textComp.color;
                    org.a = guiAlpha;
                    textComp.color = org;
                    if (guiAlpha <= 0)
                    {
                        startFade = false;
                        textComp.text = "";
                        org.a = 1;
                        textComp.color = org;
                        guiAlpha = 1;
                        timer = 0.0f;
                    }
                }
                else
                {
                    org = textComp.color;
                    org.a = guiAlpha;
                    textComp.color = org;
                }
            }
        }
    }

    void Reset()
    {
        text = textComp.text;
        startFade = true;
        guiAlpha = 1;
        timer = 0.0f;
    }
}
