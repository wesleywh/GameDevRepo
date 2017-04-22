using UnityEngine;
using System.Collections;

public class PlatformParent : MonoBehaviour
{

    // FPS KIT [www.armedunity.com]

    public Transform platform;
    private Transform player;
    private CharacterController cc;

    void OnTriggerEnter(Collider other)
    {
        if (player) return;

        if (other.CompareTag("Player"))
        {
            player = other.transform;
            player.transform.parent = platform.transform;
            cc = player.GetComponent<CharacterController>();
            StartCoroutine(CheckStatus());
        }
    }

    IEnumerator CheckStatus()
    {

        while (player)
        {
            if (!cc.isGrounded)
            {
                player.parent = null;
                player = null;
                cc = null;
            }
            yield return null;
        }
    }

    //void OnTriggerExit ( Collider hit  ){	
    //	if(player) player.parent = null;
    //}
}