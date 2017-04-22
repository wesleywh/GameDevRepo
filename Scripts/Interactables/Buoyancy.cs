// Buoyancy.cs
// by Alex Zhdankin
// Version 2.1
//
// http://forum.unity3d.com/threads/72974-Buoyancy-script
//
// Terms of use: do whatever you like

using System.Collections.Generic;
using UnityEngine;

public class Buoyancy : MonoBehaviour
{
//	public Ocean ocean;

	public float density = 500;
	public int slicesPerAxis = 2;
	public bool isConcave = false;
	public int voxelsLimit = 16;

	private const float DAMPFER = 0.1f;
	private const float WATER_DENSITY = 500;

	private float voxelHalfHeight;
	private Vector3 localArchimedesForce;
	private List<Vector3> voxels;
	private bool isMeshCollider;
	private List<Vector3[]> forces; // For drawing force gizmos

	/// <summary>
	/// Provides initialization.
	/// </summary>
	private void Start()
	{
		forces = new List<Vector3[]>(); // For drawing force gizmos

		// Store original rotation and position
		var originalRotation = transform.rotation;
		var originalPosition = transform.position;
		transform.rotation = Quaternion.identity;
		transform.position = Vector3.zero;

		// The object must have a collider
		if (GetComponent<Collider>() == null)
		{
			gameObject.AddComponent<MeshCollider>();
			Debug.LogWarning(string.Format("[Buoyancy.cs] Object \"{0}\" had no collider. MeshCollider has been added.", name));
		}
		isMeshCollider = GetComponent<MeshCollider>() != null;

		var bounds = GetComponent<Collider>().bounds;
		if (bounds.size.x < bounds.size.y)
		{
			voxelHalfHeight = bounds.size.x;
		}
		else
		{
			voxelHalfHeight = bounds.size.y;
		}
		if (bounds.size.z < voxelHalfHeight)
		{
			voxelHalfHeight = bounds.size.z;
		}
		voxelHalfHeight /= 2 * slicesPerAxis;

		// The object must have a RidigBody
		if (GetComponent<Rigidbody>() == null)
		{
			gameObject.AddComponent<Rigidbody>();
			Debug.LogWarning(string.Format("[Buoyancy.cs] Object \"{0}\" had no Rigidbody. Rigidbody has been added.", name));
		}
		GetComponent<Rigidbody>().centerOfMass = new Vector3(0, -bounds.extents.y * 0f, 0) + transform.InverseTransformPoint(bounds.center);

		voxels = SliceIntoVoxels(isMeshCollider && isConcave);

		// Restore original rotation and position
		transform.rotation = originalRotation;
		transform.position = originalPosition;

		float volume = GetComponent<Rigidbody>().mass / density;

		WeldPoints(voxels, voxelsLimit);

		float archimedesForceMagnitude = WATER_DENSITY * Mathf.Abs(Physics.gravity.y) * volume;
		localArchimedesForce = new Vector3(0, archimedesForceMagnitude, 0) / voxels.Count;

//		Debug.Log(string.Format("[Buoyancy.cs] Name=\"{0}\" volume={1:0.0}, mass={2:0.0}, density={3:0.0}", name, volume, GetComponent<Rigidbody>().mass, density));
	}

	/// <summary>
	/// Slices the object into number of voxels represented by their center points.
	/// <param name="concave">Whether the object have a concave shape.</param>
	/// <returns>List of voxels represented by their center points.</returns>
	/// </summary>
	private List<Vector3> SliceIntoVoxels(bool concave)
	{
		var points = new List<Vector3>(slicesPerAxis * slicesPerAxis * slicesPerAxis);

		if (concave)
		{
			var meshCol = GetComponent<MeshCollider>();

			var convexValue = meshCol.convex;
			meshCol.convex = false;

			// Concave slicing
			var bounds = GetComponent<Collider>().bounds;
			for (int ix = 0; ix < slicesPerAxis; ix++)
			{
				for (int iy = 0; iy < slicesPerAxis; iy++)
				{
					for (int iz = 0; iz < slicesPerAxis; iz++)
					{
						float x = bounds.min.x + bounds.size.x / slicesPerAxis * (0.5f + ix);
						float y = bounds.min.y + bounds.size.y / slicesPerAxis * (0.5f + iy);
						float z = bounds.min.z + bounds.size.z / slicesPerAxis * (0.5f + iz);

						var p = transform.InverseTransformPoint(new Vector3(x, y, z));

						if (PointIsInsideMeshCollider(meshCol, p))
						{
							points.Add(p);
						}
					}
				}
			}
			if (points.Count == 0)
			{
				points.Add(bounds.center);
			}

			meshCol.convex = convexValue;
		}
		else
		{
			// Convex slicing
			var bounds = GetComponent<Collider>().bounds;
			for (int ix = 0; ix < slicesPerAxis; ix++)
			{
				for (int iy = 0; iy < slicesPerAxis; iy++)
				{
					for (int iz = 0; iz < slicesPerAxis; iz++)
					{
						float x = bounds.min.x + bounds.size.x / slicesPerAxis * (0.5f + ix);
						float y = bounds.min.y + bounds.size.y / slicesPerAxis * (0.5f + iy);
						float z = bounds.min.z + bounds.size.z / slicesPerAxis * (0.5f + iz);

						var p = transform.InverseTransformPoint(new Vector3(x, y, z));

						points.Add(p);
					}
				}
			}
		}

		return points;
	}

	/// <summary>
	/// Returns whether the point is inside the mesh collider.
	/// </summary>
	/// <param name="c">Mesh collider.</param>
	/// <param name="p">Point.</param>
	/// <returns>True - the point is inside the mesh collider. False - the point is outside of the mesh collider. </returns>
	private static bool PointIsInsideMeshCollider(Collider c, Vector3 p)
	{
		Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

		foreach (var ray in directions)
		{
			RaycastHit hit;
			if (c.Raycast(new Ray(p - ray * 1000, ray), out hit, 1000f) == false)
			{
				return false;
			}
		}

		return true;
	}

	/// <summary>
	/// Returns two closest points in the list.
	/// </summary>
	/// <param name="list">List of points.</param>
	/// <param name="firstIndex">Index of the first point in the list. It's always less than the second index.</param>
	/// <param name="secondIndex">Index of the second point in the list. It's always greater than the first index.</param>
	private static void FindClosestPoints(IList<Vector3> list, out int firstIndex, out int secondIndex)
	{
		float minDistance = float.MaxValue, maxDistance = float.MinValue;
		firstIndex = 0;
		secondIndex = 1;

		for (int i = 0; i < list.Count - 1; i++)
		{
			for (int j = i + 1; j < list.Count; j++)
			{
				float distance = Vector3.Distance(list[i], list[j]);
				if (distance < minDistance)
				{
					minDistance = distance;
					firstIndex = i;
					secondIndex = j;
				}
				if (distance > maxDistance)
				{
					maxDistance = distance;
				}
			}
		}
	}

	/// <summary>
	/// Welds closest points.
	/// </summary>
	/// <param name="list">List of points.</param>
	/// <param name="targetCount">Target number of points in the list.</param>
	private static void WeldPoints(IList<Vector3> list, int targetCount)
	{
		if (list.Count <= 2 || targetCount < 2)
		{
			return;
		}

		while (list.Count > targetCount)
		{
			int first, second;
			FindClosestPoints(list, out first, out second);

			var mixed = (list[first] + list[second]) * 0.5f;
			list.RemoveAt(second); // the second index is always greater that the first => removing the second item first
			list.RemoveAt(first);
			list.Add(mixed);
		}
	}

	/// <summary>
	/// Returns the water level at given location.
	/// </summary>
	/// <param name="x">x-coordinate</param>
	/// <param name="z">z-coordinate</param>
	/// <returns>Water level</returns>
	private float GetWaterLevel(float x, float z)
	{
//		return ocean == null ? 0.0f : ocean.GetWaterHeightAtLocation(x, z);
		return -2.0f;
	}

	/// <summary>
	/// Calculates physics.
	/// </summary>
	private void FixedUpdate()
	{
		forces.Clear(); // For drawing force gizmos

		foreach (var point in voxels)
		{
			var wp = transform.TransformPoint(point);
			float waterLevel = GetWaterLevel(wp.x, wp.z);

			if (wp.y - voxelHalfHeight < waterLevel)
			{
				float k = (waterLevel - wp.y) / (2 * voxelHalfHeight) + 0.5f;
				if (k > 1)
				{
					k = 1f;
				}
				else if (k < 0)
				{
					k = 0f;
				}

				var velocity = GetComponent<Rigidbody>().GetPointVelocity(wp);
				var localDampingForce = -velocity * DAMPFER * GetComponent<Rigidbody>().mass;
				var force = localDampingForce + Mathf.Sqrt(k) * localArchimedesForce;
				GetComponent<Rigidbody>().AddForceAtPosition(force, wp);

				forces.Add(new[] { wp, force }); // For drawing force gizmos
			}
		}
	}

	/// <summary>
	/// Draws gizmos.
	/// </summary>
	private void OnDrawGizmos()
	{
		if (voxels == null || forces == null)
		{
			return;
		}

		const float gizmoSize = 0.05f;
		Gizmos.color = Color.yellow;

		foreach (var p in voxels)
		{
			Gizmos.DrawCube(transform.TransformPoint(p), new Vector3(gizmoSize, gizmoSize, gizmoSize));
		}

		Gizmos.color = Color.cyan;

		foreach (var force in forces)
		{
			Gizmos.DrawCube(force[0], new Vector3(gizmoSize, gizmoSize, gizmoSize));
			Gizmos.DrawLine(force[0], force[0] + force[1] / GetComponent<Rigidbody>().mass);
		}
	}
}