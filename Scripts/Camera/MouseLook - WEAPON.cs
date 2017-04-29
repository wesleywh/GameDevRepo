using UnityEngine;
using System.Collections;

[AddComponentMenu("Camera-Control/Mouse Look")]
public class MouseLook : MonoBehaviour {

	public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
	public RotationAxes axes = RotationAxes.MouseXAndY;
	public float sensitivityX = 15F;
	public float sensitivityY = 15F;

	public float minimumX = -60F;
	public float maximumX = 60F;

	public float minimumY = -60F;
	public float maximumY = 60F;

	public float offsetY = 0F;
	public float offsetX = 0F;

	public float rotationX = 0F;
	GameObject cmra = null;

	public float rotationY = 0F;
	
	Quaternion originalRotation;

	void Update (){
	
		if(Cursor.lockState == CursorLockMode.None) return;

		if (axes == RotationAxes.MouseXAndY)
		{
			// Read the mouse input axis
			rotationX += (Input.GetAxis("Mouse X") * sensitivityX /30*cmra.GetComponent<Camera>().fieldOfView + offsetX);
			rotationY += (Input.GetAxis("Mouse Y") * sensitivityY /30*cmra.GetComponent<Camera>().fieldOfView + offsetY);

			rotationX = ClampAngle (rotationX, minimumX, maximumX);
			rotationY = ClampAngle (rotationY, minimumY, maximumY);
			
			Quaternion xQuaternion = Quaternion.AngleAxis (rotationX, Vector3.up);
			Quaternion yQuaternion = Quaternion.AngleAxis (rotationY, Vector3.left);
			
			transform.localRotation = originalRotation * xQuaternion * yQuaternion;
		}
		else if (axes == RotationAxes.MouseX)
		{
			rotationX += (Input.GetAxis("Mouse X") * sensitivityX /60*cmra.GetComponent<Camera>().fieldOfView + offsetX);
			rotationX = ClampAngle (rotationX, minimumX, maximumX);

			Quaternion xQuaternion = Quaternion.AngleAxis (rotationX, Vector3.up);
			transform.localRotation = originalRotation * xQuaternion;
		}
		else
		{
			rotationY += (Input.GetAxis("Mouse Y") * sensitivityY /60*cmra.GetComponent<Camera>().fieldOfView + offsetY);
			rotationY = ClampAngle (rotationY, minimumY, maximumY);

			Quaternion yQuaternion = Quaternion.AngleAxis (rotationY, Vector3.left);
			transform.localRotation = originalRotation * yQuaternion;
		}
		offsetY = 0F;
		offsetX = 0F;
	}
	
	void Start ()
	{
		cmra = GameObject.FindWithTag("MainCamera");
		// Make the rigid body not change rotation
		if (GetComponent<Rigidbody>())
			GetComponent<Rigidbody>().freezeRotation = true;
		originalRotation = transform.localRotation;
	}
	
	public static float ClampAngle (float angle, float min, float max)
	{
		if (angle < -360F)
			angle += 360F;
		if (angle > 360F)
			angle -= 360F;
		return Mathf.Clamp (angle, min, max);
	}
	
	public void SetRotation(float r){
		rotationX = r;
	}
}