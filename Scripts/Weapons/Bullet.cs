using UnityEngine;
using System.Collections;
//
[AddComponentMenu("Weapons/Bullet")]
public class Bullet : MonoBehaviour
{
	public float lifetime = 2.0f;
	public GameObject bulletHole = null; // default bullet hole
	public GameObject impactEffect = null; // default impact effect
	// for more variety add new impact effects for different material types, also bullet holes for different materials
	//public GameObject metalImpactEffect = null;
	//public GameObject woodImpactEffect = null;
	//public GameObject stoneImpactEffect = null;
	private int hitCount = 0; // hit counter for counting bullet impacts for bullet penetration
	private float damage; // damage bullet applies to a target
	private float maxHits; // number of collisions before bullet gets destroyed
	private float impactForce; // force applied to a rigid body object
	private float maxInaccuracy; // maximum amount of inaccuracy
	private float variableInaccuracy; // used in machineguns to decrease accuracy if maintaining fire
	private float speed; // bullet speed
	private Vector3 velocity = Vector3.zero; // bullet velocity
	private Vector3 newPos = Vector3.zero; // bullet's new position
	private Vector3 oldPos = Vector3.zero; // bullet's previous location
	private bool hasHit = false; // has the bullet hit something?
	private Collider ignore = null; // ignore this collider
	private Vector3 direction; // direction bullet is travelling
	private GameObject owner; // owner of bullet
	private string ownersName; // owner's name
	private bool isTracer = false; // used in raycast bullet system... sets the bullet to just act like a tracer
	private string[] bulletInfo = new string[2]; // damage and owner name sent to hit object
	public GameObject Owner // sets and returns the bullet owners gameObject
	{
		get { return owner; }
		set { owner = value; }
	}
	public Collider ColliderToIgnore // not really used at the moment, can send a collider to the bullet if you want it to ignore it
	{
		get { return ignore; }
		set { ignore = value; }
	}
	public void SetUp(float[] info) // information sent from gun to bullet to change bullet properties
	{
		damage = info[0]; // bullet damage
		impactForce = info[1]; // force applied to rigid bodies
		maxHits = info[2]; // max number of bullet impacts before bullet is destroyed
		maxInaccuracy = info[3]; // max inaccuracy of the bullet
		variableInaccuracy = info[4]; // current inaccuracy... mostly for machine guns that lose accuracy over time
		speed = info[5]; // bullet speed
		// drection bullet is traveling
		direction = transform.TransformDirection(Random.Range(-maxInaccuracy, maxInaccuracy) * variableInaccuracy, Random.Range(-maxInaccuracy, maxInaccuracy) * variableInaccuracy, 1);
		newPos = transform.position; // bullet's new position
		oldPos = newPos; // bullet's old position
		velocity = speed * transform.forward; // bullet's velocity determined by direction and bullet speed
		// schedule for destruction if bullet never hits anything
		Destroy(gameObject, lifetime);
	}

	// Update is called once per frame
	void Update()
	{
		if (hasHit)
			return; // if bullet has already hit its max hits... exit

		// assume we move all the way
		newPos += (velocity + direction) * Time.deltaTime;

		// Check if we hit anything on the way
		Vector3 dir = newPos - oldPos;
		float dist = dir.magnitude;
		if (dist > 0)
		{
			// normalize
			dir /= dist;
			RaycastHit[] hits = Physics.RaycastAll(oldPos, dir, dist);

			// Find the first valid hit
			for (int i = 0; i < hits.Length; i++)
			{
				RaycastHit hit = hits[i];
				//Debug.Log( "Bullet hit " + hit.collider.gameObject.name + " at " + hit.point.ToString() );

				if (ShouldIgnoreHit(hit))
					continue;

				// adjust new position
				newPos = hit.point;

				// notify hit
				OnHit(hit);
				// Debug.Log("if " + hitCount + " > " + maxHits + " then destroy bullet...");
				if (hitCount >= maxHits)
				{
					hasHit = true;
					Destroy(gameObject);
					break;
				}
			}
		}

		//=========================================================================================
		// test for hit against a back of a wall
		// want to try to have bullet holes on both sides of a penetrated wall
		//=========================================================================================
		RaycastHit hit2;
		if (Physics.Raycast(newPos, -dir, out hit2, dist)) // send a ray behind the bullet to check for exit impact
		{ // testing to see if the bullet passed through something
			//RaycastHit hit2 = hits2[j];
			if ((!hasHit))// && (hit2.transform != owner.transform))
			{
				OnBackHit(hit2); // send rear impact and check what to do with it
			}
		}
		//============================================================================================

		oldPos = transform.position; // set old position to current position
		transform.position = newPos; // set current position to the new position
	}

	protected virtual bool ShouldIgnoreHit(RaycastHit hit)
	{
		if (hit.collider == this.GetComponent<Collider>())
			return true; // if I hit myself... ignore it
		return false;
	}
		
	protected virtual void OnHit(RaycastHit hit)
	{
		hitCount++; // add another hit to counter
		Vector3 contact = hit.point; // point where bullet hit
		Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hit.normal); // rotation of bullet impact
		switch (hit.transform.tag) // decide what the bullet collided with and what to do with it
		{
		case "Projectile":
			// do nothing if 2 bullets collide
			break;
		case "Player":
			// add blood effect
			break;
		case "wood":
			// add wood impact effects
			break;
		case "stone":
			// add stone impact effect
			break;
		case "ground":
			// add dirt or ground impact effect
			break;
		case "wall":
			// create an impact effect
			Instantiate(impactEffect, hit.point + 0.1f * hit.normal, Quaternion.FromToRotation(Vector3.up, hit.normal));

			// create a bullet hole
			GameObject newBulletHole = Instantiate(bulletHole, contact, rotation) as GameObject;

			// attach the bullet hole to the hit object, this keeps holes attached to rigid bodies as they move about
			newBulletHole.transform.parent = hit.transform;
			break;
		default: // default impact effect and bullet hole
			// create an impact effect
			Instantiate(impactEffect, hit.point + 0.1f * hit.normal, Quaternion.FromToRotation(Vector3.up, hit.normal));
			break;
		}
		if (!isTracer) // if this is a bullet and not a tracer, then apply damage to the hit object
		{
			// send a message to the hit object... let it know it was hit
			bulletInfo[0] = ownersName; // tell hit object who hit them
			bulletInfo[1] = damage.ToString(); // tell them how much damage they recieved
			// send the message
			hit.collider.SendMessageUpwards("ImHit", bulletInfo, SendMessageOptions.DontRequireReceiver);

			if (hit.rigidbody) // if we hit a rigi body... apply a force
			{
				hit.rigidbody.AddForce(transform.forward * impactForce, ForceMode.Impulse);
			}
		}
	}

	protected virtual void OnBackHit(RaycastHit hit)
	{
		Vector3 contact = hit.point;
		Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
		switch (hit.transform.tag)
		{
		case "bullet":
			// do nothing if 2 bullets collide
			break;
		case "Player":
			// add blood effect
			break;
		case "wood":
			// add wood impact effects
			break;
		case "stone":
			// add
			break;
		case "ground":
			// add
			break;
		case "wall":
			// create an impact effect
			Instantiate(impactEffect, hit.point + 0.1f * hit.normal, Quaternion.FromToRotation(Vector3.up, hit.normal));

			// create a bullet hole
			GameObject newBulletHole = Instantiate(bulletHole, contact, rotation) as GameObject;
		
			// attach the bullet hole to the hit object, this keeps holes attached to rigid bodies as they move about
			newBulletHole.transform.parent = hit.transform;
			break;
		default: // default impact effect and bullet hole

			// create an impact effect
			Instantiate(impactEffect, hit.point + 0.1f * hit.normal, Quaternion.FromToRotation(Vector3.up, hit.normal));
			break;
		}
	}

	public void setPlayer(string pName)
	{
		ownersName = pName; // player that shot fired this bullet... used for multiplayer (not fully working yet)
	}

	public void SetTracer()
	{
		isTracer = true; // tell this bullet it is only a tracer... keeps this object from applying damage
	}
}