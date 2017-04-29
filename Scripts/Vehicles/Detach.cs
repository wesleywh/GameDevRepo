using UnityEngine;

public class Detach : MonoBehaviour
{

    // FPS KIT [www.armedunity.com]

    public GameObject[] unparentWheels;
    public float hitPoints = 100;
    public Transform explosion;
    public GameObject body;
    public GameObject trigger;
    public VehicleScript vechicleScript;

    public void ApplyDamage(float damage)
    {
        if (hitPoints <= 0.0f)
            return;

        // Apply damage
        hitPoints -= damage;

        if (hitPoints <= 0.0f) Detonate();
    }
    void Detonate()
    {
	
		AudioSource[] aSources = GetComponentsInChildren<AudioSource>();
        foreach (AudioSource source in aSources) {
            source.enabled = false;
        }

        Component[] coms = GetComponentsInChildren<MonoBehaviour>();
        foreach (var b in coms)
        {
            MonoBehaviour p = b as MonoBehaviour;
            if (p)
                p.enabled = false;
        }
        trigger.SetActive(false);
        for (int i = 0; i < unparentWheels.Length; i++)
        {
            unparentWheels[i].transform.parent = null;
            unparentWheels[i].AddComponent<MeshCollider>();
            unparentWheels[i].GetComponent<MeshCollider>().convex = true;
            unparentWheels[i].AddComponent<Rigidbody>();
            unparentWheels[i].GetComponent<Rigidbody>().mass = 12;
            unparentWheels[i].transform.position = new Vector3(unparentWheels[i].transform.position.x, unparentWheels[i].transform.position.y + 1, unparentWheels[i].transform.position.z);

        }
        Instantiate(explosion, body.transform.position, body.transform.rotation);
        transform.DetachChildren();
        if(vechicleScript.inVehicle) vechicleScript.GetOut();
    }

}