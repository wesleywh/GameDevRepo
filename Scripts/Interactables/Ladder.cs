using UnityEngine;
using System.Collections;

public class Ladder : MonoBehaviour
{


    private Vector3 climbDir = Vector3.zero;
    public BoxCollider col;

    void Start()
    {
        climbDir = (transform.position + new Vector3(0, col.size.y / 2, 0)) - (transform.position - new Vector3(0, col.size.y / 2, 0));
    }

    public Vector3 ClimbDirection()
    {
        return climbDir;
    }


}