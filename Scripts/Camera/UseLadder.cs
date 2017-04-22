using UnityEngine;

public class UseLadder : MonoBehaviour
{


    public float climbSpeed = 6.0f;
    private float climbDownThreshold = -0.4f;
    private Vector3 climbDirection = Vector3.zero;
    private Vector3 lateralMove = Vector3.zero;
    private Vector3 ladderMovement = Vector3.zero;
    [HideInInspector]
    public Ladder currentLadder = null;
    public GameObject mainCamera;
    public CharacterController controller;
	public MovementController moveController;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ladder"))
        {
            LatchLadder(other.gameObject);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ladder"))
        {
            UnlatchLadder();
        }
    }

    void LatchLadder(GameObject latchedLadder)
    {
        currentLadder = latchedLadder.GetComponent<Ladder>();
        climbDirection = currentLadder.ClimbDirection();
		moveController.OnLadder();
    }

    void UnlatchLadder()
    {
        currentLadder = null;
		moveController.OffLadder(ladderMovement);
    }

    public void LadderUpdate()
    {
        Vector3 verticalMove;
        verticalMove = climbDirection.normalized;
        verticalMove *= Input.GetAxis("Vertical");
        verticalMove *= (mainCamera.transform.forward.y > climbDownThreshold) ? 1 : -1;
        lateralMove = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        lateralMove = transform.TransformDirection(lateralMove);
        ladderMovement = verticalMove + lateralMove;
        controller.Move(ladderMovement * climbSpeed * Time.deltaTime);
    }


}