using UnityEngine;
using System.Collections;



public class MovePlatforms : MonoBehaviour
{

    // FPS KIT [www.armedunity.com]

    public enum dir { Horizontal, Vertical }

    public dir movement = dir.Horizontal;

    public float dist = 10;
    public float movementSpeed = 3.0f;
    private float startPos;

    void Start()
    {
        if (movement == dir.Horizontal)
            startPos = transform.position.x;
        else
            startPos = transform.position.y;
    }

    void Update()
    {
        if (movement == dir.Horizontal)
            transform.position = new Vector3(startPos + Mathf.PingPong(Time.time * movementSpeed, dist), transform.position.y, transform.position.z);
        else
            transform.position = new Vector3(transform.position.x, startPos + Mathf.PingPong(Time.time * movementSpeed, dist), transform.position.z);
    }
}