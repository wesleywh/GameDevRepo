using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Events;

[Serializable]
class MovePointList 
{
	public string movePointTag = null;
	public string movePointName = null;
	public Transform movePoint = null;
	public float speed = 1.0F;
	public float closeEnough = 0.1f;
    public float waitTime = 0.0f;
    public bool lookAtTarget = true;
    public Transform lookPoint = null;
    public bool matchRotation = false;
    public float rotationSpeed = 1.0f;
    [Space(10)]
    [Tooltip("This takes precedence over rotation")]
    public bool completeBasedOnMovement = true;
    [Tooltip("Movement takes precedence, can't have both")]
    public bool completeBasedOnRotation = false;
}

public class MoveToPoints : MonoBehaviour 
{
    public Transform target = null;
    [SerializeField] private bool callToStart = false;
    [SerializeField] private bool resetOnCompleted = false;
	[SerializeField] private MovePointList[] movePoints = null;
    [SerializeField] private UnityEvent OnCompleted;
	private Transform startMarker;
	private Transform endMarker;
	private float startTime;
	private float journeyLength;
	[SerializeField] private bool pingpong = true;
	private int index = 0;
	private bool forward = true;
    private bool running = false;
    private bool check = false;

	void Start() 
    {
        target = (target == null) ? this.transform : target;
        if (callToStart == false)
		    MoveStart ();
	}
	public void MoveStart() 
    {
		startTime = Time.time;
		startMarker = target;
		if (string.IsNullOrEmpty (movePoints [index].movePointTag) == false) 
        {
			endMarker = GameObject.FindGameObjectWithTag (movePoints [index].movePointTag).transform;
		} 
        else if (string.IsNullOrEmpty (movePoints [index].movePointName) == false) 
        {
			endMarker = GameObject.Find (movePoints [index].movePointName).transform;
		} 
        else 
        {
			endMarker = movePoints [index].movePoint;
		}
		journeyLength = Vector3.Distance(startMarker.position, endMarker.position);
        running = true;
	}

	void Update () 
    {
        if (running == false)
            return;
		float distCovered = (Time.time - startTime) * movePoints[index].speed;
		float fracJourney = distCovered / journeyLength;
        target.position = Vector3.Lerp(startMarker.position, endMarker.position, fracJourney);
        if (movePoints[index].lookAtTarget == true)
        {
            movePoints[index].lookPoint = (movePoints[index].lookPoint == null) ? endMarker.transform : movePoints[index].lookPoint;
            Quaternion targetRotation = Quaternion.LookRotation(movePoints[index].lookPoint.position - target.position);
            target.rotation = Quaternion.Slerp(target.rotation, targetRotation, (Time.time - startTime) / movePoints[index].rotationSpeed);
        }
        if (movePoints[index].matchRotation == true)
        {
            target.rotation = Quaternion.Slerp(target.rotation, movePoints [index].movePoint.rotation, (Time.time - startTime) / movePoints[index].rotationSpeed);
        }
        if (movePoints[index].completeBasedOnMovement == true)
        {
            check = Vector3.Distance(target.position, endMarker.position) <= movePoints[index].closeEnough;
        }
        else if (movePoints[index].completeBasedOnRotation == true)
        {
            check = Quaternion.Angle(target.rotation, movePoints[index].movePoint.rotation) <= movePoints[index].closeEnough;
        }
        else
        {
            Debug.LogError("Point: " + index + " doesn't have a completion senerio!");
        }
        if (check) 
        {
			index = (forward == true) ? index + 1 : index - 1;
			if (index <= 0 || index >= movePoints.Length) 
            {
                if (pingpong == true)
                {
                    forward = !forward;
                    index = (forward == true) ? index + 1 : index - 1;
                    index = (forward == true) ? index + 1 : index - 1;
                    StartCoroutine(PerformWait(movePoints[index].waitTime));
                }
                else if (resetOnCompleted == true)
                {
                    index = 0;
                    running = false;
                    OnCompleted.Invoke();
                }
                else 
                {
                    OnCompleted.Invoke();
                    GetComponent<MoveToPoints>().enabled = false;
                }
			} 
            else 
            {
                StartCoroutine(PerformWait(movePoints[index].waitTime));
			}
		}
	}
    IEnumerator PerformWait(float waitTime)
    {
        running = false;
        yield return new WaitForSeconds(waitTime);
        MoveStart();
    }
}
