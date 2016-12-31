/// MouseLook rotates the transform based on the mouse delta.
/// Minimum and Maximum values can be used to constrain the possible rotation


/// To make an FPS style character:
/// - Create a capsule.
/// - Add a rigid body to the capsule
/// - Add the MouseLook script to the capsule.
///   -> Set the mouse look to use LookX. (You want to only turn character but not tilt it)
/// - Add FPSWalker script to the capsule


/// - Create a camera. Make the camera a child of the capsule. Reset it's transform.
/// - Add a MouseLook script to the camera.
///   -> Set the mouse look to use LookY. (You want the camera to tilt up and down like a head. The character already turns.)
using UnityEngine;
using System.Collections;
using TeamUtility.IO;					//Custom InputManager Manager

[AddComponentMenu("Camera-Control/Mouse Look")]
public class MouseLook : MonoBehaviour {


	public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
	public RotationAxes axes = RotationAxes.MouseXAndY;
	public float sensitivityX = 15F;
	public float sensitivityY = 15F;
	public float minimumX = -360F;
	public float maximumX = 360F;
	public float minimumY = -60F;
	public float maximumY = 60F;
	public bool enable = true;
	float rotationX = 0F;
	float rotationY = 0F;
	Quaternion originalRotation;
	void Update ()
	{
		if (enable == true) {
			if (axes == RotationAxes.MouseXAndY) {
				// Read the mouse input axis
				rotationX += InputManager.GetAxis ("Mouse X") * GameObject.FindGameObjectWithTag ("GameManager").GetComponent<MouseSettings> ().mouseSpeedX;
				rotationY += InputManager.GetAxis ("Mouse Y") * GameObject.FindGameObjectWithTag ("GameManager").GetComponent<MouseSettings> ().mouseSpeedY;
				rotationX = ClampAngle (rotationX, minimumX, maximumX);
				rotationY = ClampAngle (rotationY, minimumY, maximumY);
				Quaternion xQuaternion = Quaternion.AngleAxis (rotationX, Vector3.up);
				Quaternion yQuaternion = Quaternion.AngleAxis (rotationY, -Vector3.right);
				transform.localRotation = originalRotation * xQuaternion * yQuaternion;
			} else if (axes == RotationAxes.MouseX) {
				rotationX += InputManager.GetAxis ("Mouse X") * GameObject.FindGameObjectWithTag ("GameManager").GetComponent<MouseSettings> ().mouseSpeedX;
				rotationX = ClampAngle (rotationX, minimumX, maximumX);
				Quaternion xQuaternion = Quaternion.AngleAxis (rotationX, Vector3.up);
				transform.localRotation = originalRotation * xQuaternion;
			} else {
				rotationY += InputManager.GetAxis ("Mouse Y") * GameObject.FindGameObjectWithTag ("GameManager").GetComponent<MouseSettings> ().mouseSpeedY;
				rotationY = ClampAngle (rotationY, minimumY, maximumY);
				Quaternion yQuaternion = Quaternion.AngleAxis (-rotationY, Vector3.right);
				transform.localRotation = originalRotation * yQuaternion;
			}
		}
	}
	void Start ()
	{
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
}