using UnityEngine;
using System.Collections;

public class VehicleScript : MonoBehaviour
{

    // FPS KIT [www.armedunity.com]

    private GameObject weaponCamera;
    public GameObject vehicleCam;
    public Transform vehicleCameraTarget;
    public GameObject vehicle;
    private GameObject Player;
    public Transform GetOutPosition;
    private float waitTime = 0.5f;

    private GameObject mainCamera;
    public bool inVehicle = false;
    void Start()
    {
        vehicleCam.GetComponent<Camera>().enabled = false;
        vehicle.SendMessage("Status", inVehicle);
        vehicleCam.GetComponent<AudioListener>().enabled = false;
    }

    void Update()
    {
        if (!inVehicle) return;
        if (Input.GetKeyDown("e")) GetOut();
    }

    void Action()
    {
        if (!inVehicle) StartCoroutine(GetIn());
    }

    IEnumerator GetIn()
    {
        Player = GameObject.FindWithTag("Player");
        mainCamera = GameObject.FindWithTag("MainCamera");
        weaponCamera = GameObject.FindWithTag("WeaponCamera");
        Player.SetActive(false);

        VehicleCamera changeTarget = vehicleCam.transform.GetComponent<VehicleCamera>();
        changeTarget.target = vehicleCameraTarget;
        Player.transform.parent = vehicle.transform;
        Player.transform.position = vehicleCameraTarget.transform.position;

        weaponCamera.GetComponent<Camera>().enabled = false;
        mainCamera.GetComponent<AudioListener>().enabled = false;
        mainCamera.GetComponent<Camera>().enabled = false;

        vehicleCam.GetComponent<Camera>().enabled = true;
        vehicle.SendMessage("Status", true);

        vehicleCam.GetComponent<AudioListener>().enabled = true;
        yield return new WaitForSeconds(waitTime);
        inVehicle = true;
    }

    public void GetOut()
    {

        Player.transform.parent = null;
        Player.transform.position = GetOutPosition.position;
        Player.SetActive(true);
        Player.SendMessage("SetRotation", GetOutPosition.transform.rotation.eulerAngles.y);

        weaponCamera.GetComponent<Camera>().enabled = true;
        mainCamera.GetComponent<AudioListener>().enabled = true;
        mainCamera.GetComponent<Camera>().enabled = true;
        vehicleCam.GetComponent<Camera>().enabled = false;
        vehicleCam.GetComponent<AudioListener>().enabled = false;
        vehicle.SendMessage("Status", false);
        inVehicle = false;
    }

}