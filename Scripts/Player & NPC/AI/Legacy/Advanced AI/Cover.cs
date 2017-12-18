using UnityEngine;
using System.Collections;
using System.Collections.Generic;			//for adding to arrays
using System.Linq;

[RequireComponent(typeof(SphereCollider))]
public class Cover : MonoBehaviour {
    public string[] checkTags = null;
    public string unavailableTag = "Untagged";
    public string availableTag = "Cover";

    void OnTriggerEnter(Collider col)
    {
        if (checkTags.Contains(col.transform.root.tag))
        {
            this.tag = unavailableTag;
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (checkTags.Contains(col.transform.root.tag))
        {
            this.tag = availableTag;
        }
    }
}
