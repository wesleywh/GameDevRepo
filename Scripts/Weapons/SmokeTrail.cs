using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class SmokeTrail : MonoBehaviour {

	private LineRenderer line;
	private Transform tr;
	private Vector3[] positions;
	private Vector3[] directions;
	private int i;
	private float timeSinceUpdate = 0.0f;
	private Material lineMaterial;
	private float lineSegment = 0.0f;
	private int currentNumberOfPoints = 2;
	private bool  allPointsAdded = false;
	int numberOfPoints = 10;
	float updateSpeed = 0.25f;
	float riseSpeed = 0.25f;
	float spread = 0.2f;

	private Vector3 tempVec;

	void  Start (){
		tr = this.transform;
		line = this.GetComponent<LineRenderer>();
		lineMaterial = line.material;

		lineSegment = 1.0f / numberOfPoints;

		positions = new Vector3[numberOfPoints];
		directions = new Vector3[numberOfPoints];

//		line.SetVertexCount ( currentNumberOfPoints ); //deprecated
        line.positionCount = currentNumberOfPoints;

		for ( i = 0; i < currentNumberOfPoints; i++ ) {
			tempVec = getSmokeVec ();
			directions[i] = tempVec;
			positions[i] = tr.position;
			line.SetPosition ( i, positions[i] );
		}
	}

	void  Update (){

		timeSinceUpdate += Time.deltaTime; // Update time.

		// If it's time to update the line...
		if ( timeSinceUpdate > updateSpeed ) {
			timeSinceUpdate -= updateSpeed;

			// Add points until the target number is reached.
			if ( !allPointsAdded ) {
				currentNumberOfPoints++;
//				line.SetVertexCount ( currentNumberOfPoints );//deprecated
                line.positionCount = currentNumberOfPoints;
				tempVec = getSmokeVec ();
				directions[0] = tempVec;
				positions[0] = tr.position;
				line.SetPosition ( 0, positions[0] );
			}

			if ( !allPointsAdded && ( currentNumberOfPoints == numberOfPoints ) ) {
				allPointsAdded = true;
			}

			// Make each point in the line take the position and direction of the one before it (effectively removing the last point from the line and adding a new one at transform position).
			for ( i = currentNumberOfPoints - 1; i > 0; i-- ) {
				tempVec = positions[i-1];
				positions[i] = tempVec;
				tempVec = directions[i-1];
				directions[i] = tempVec;
			}
			tempVec = getSmokeVec ();
			directions[0] = tempVec; // Remember and give 0th point a direction for when it gets pulled up the chain in the next line update.
		}

		// Update the line...
		for ( i = 1; i < currentNumberOfPoints; i++ ) {
			tempVec = positions[i];
			tempVec += directions[i] * Time.deltaTime;
			positions[i] = tempVec;

			line.SetPosition ( i, positions[i] );
		}
		positions[0] = tr.position; // 0th point is a special case, always follows the transform directly.
		line.SetPosition ( 0, tr.position );

		// If we're at the maximum number of points, tweak the offset so that the last line segment is "invisible" (i.e. off the top of the texture) when it disappears.
		// Makes the change less jarring and ensures the texture doesn't jump.
		if ( allPointsAdded ) {
			Vector3 offsetOrg = lineMaterial.mainTextureOffset;
			offsetOrg.x = lineSegment * ( timeSinceUpdate / updateSpeed );
			lineMaterial.mainTextureOffset = offsetOrg;
		}
	}

	// Give a random upwards vector.
	Vector3 getSmokeVec (){
		Vector3 smokeVec;
		smokeVec.x = Random.Range ( -1.0f, 1.0f );
		smokeVec.y = Random.Range ( 0.0f, 1.0f );
		smokeVec.z = Random.Range ( -1.0f, 1.0f );
		smokeVec.Normalize ();
		smokeVec *= spread;
		smokeVec.y += riseSpeed;
		return smokeVec;
	}
}