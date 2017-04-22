using UnityEngine;
using System.Collections;

public class PlatformActivate : MonoBehaviour
{

    // FPS KIT [www.armedunity.com]

    public Animation gateAnims;
    public AudioSource aSource;
    public AudioClip[] sounds;
    public string[] anims = new string[] { "PlatformUp", "PlatformDown" };
    public float waitTime = 4.0f;
    private int state = 0;
    private bool inTransition = false;

    void ApplyDamage()
    {
       StartCoroutine(Action());
    }

    IEnumerator Action()
    {
        if (inTransition) yield break;
        inTransition = true;

        aSource.clip = sounds[state];
        aSource.Play();
        gateAnims.CrossFade(anims[state]);
        state = System.Convert.ToBoolean(state) ? 0 : 1;
        yield return new WaitForSeconds(waitTime);

        inTransition = false;
    }

}