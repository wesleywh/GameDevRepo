using UnityEngine;
using System.Collections;

public class Activate : MonoBehaviour
{

    // FPS KIT [www.armedunity.com]

    public GameObject GO;

    void ApplyDamage(float s)
    {
        Action();
    }

    void Action()
    {
        GO.SendMessage("Action", SendMessageOptions.DontRequireReceiver);
    }
}