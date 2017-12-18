using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtTarget : MonoBehaviour {
    public string targetTag;
    private GameObject target;

    public bool lockY = true;
    public bool lockX = false;
    public bool lockZ = false;
    private float orgY;
    private float orgX;
    private float orgZ;
    private Quaternion modify;

    void Start()
    {
        orgY = transform.rotation.eulerAngles.y;
        target = GameObject.FindGameObjectWithTag(targetTag);
    }
	// Update is called once per frame
	void Update () {
        if (target)
        {
            this.transform.LookAt(target.transform);
            if (lockY)
            {
                modify = Quaternion.Euler(transform.rotation.x,orgY, transform.rotation.z);
            }
            if (lockX)
            {
                modify = Quaternion.Euler(orgX,modify.y, modify.z);
            }
            if (lockZ)
            {
                modify = Quaternion.Euler(modify.x,modify.y, orgZ);
            }
            transform.rotation = modify;
        }
        else
        {
            target = GameObject.FindGameObjectWithTag(targetTag);
        }
	}
}
