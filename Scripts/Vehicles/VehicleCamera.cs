using UnityEngine;
using System.Collections;

public class VehicleCamera : MonoBehaviour
{


    public Transform target;
    private Transform myTransform;

    public float targetHeight = 2.0f;
    public float targetRight = 0.0f;
    private float dis = 10.0f;

    public bool prevButtonRight = false;

    public float maxDis = 20;
    public float minDis = 5;

    public float xSpeed = 250.0f;
    public float ySpeed = 120.0f;

    public float yMinLimit = -20;
    public float yMaxLimit = 80;

    public float zoomRate = 1;
    public float rotationDampening = 3.0f;

    private float x = 0.0f;
    private float y = 0.0f;
    private float distmod = 0.0f;

    void Start()
    {
        myTransform = transform;
        Vector3 angles = myTransform.eulerAngles;
        x = angles.y;
        y = angles.x;
    }

    void LateUpdate()
    {
        if (!target) return;

        if (Input.GetMouseButtonUp(0)) prevButtonRight = false;
        if (Input.GetMouseButtonUp(1)) prevButtonRight = true;


        if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
        {
            x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
            y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

        }
        else if (prevButtonRight)
        {
            float targetRotationAngle = target.eulerAngles.y;
            float currentRotationAngle = myTransform.eulerAngles.y;
            x = Mathf.LerpAngle(currentRotationAngle, targetRotationAngle, rotationDampening * Time.deltaTime);
        }

        dis -= Input.GetAxis("Mouse ScrollWheel") * zoomRate * Mathf.Abs(dis);
        dis = Mathf.Clamp(dis, minDis, maxDis);

        y = ClampAngle(y, yMinLimit, yMaxLimit);

        Quaternion rotation = Quaternion.Euler(y, x, 0);
        Vector3 targetMod = new Vector3(0, -targetHeight, 0) - (rotation * Vector3.right * targetRight);
        LayerMask layerMask = 1 << 8;
        layerMask = ~layerMask;
        Vector3 pos = target.position - (rotation * Vector3.forward * (dis - distmod) + targetMod);

        myTransform.rotation = rotation;
        myTransform.position = pos;
    }

    float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }

}