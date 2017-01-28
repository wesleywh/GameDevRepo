using UnityEngine;

public class SwitchWeapons : MonoBehaviour
{
	public Gun gunScript = null;

	void Update()
	{
		if(Input.GetKey(KeyCode.Alpha1))
		{
			setMachineGun();
		}

		if (Input.GetKey(KeyCode.Alpha2))
		{
			setBurst();
		}

		if (Input.GetKey(KeyCode.Alpha3))
		{
			setShotgun();
		}

		if (Input.GetKey(KeyCode.Alpha4))
		{
			setNoob();
		}

		if (Input.GetKey(KeyCode.Alpha5))
		{
			setRocket();
		}
	}

	void setMachineGun()
	{
		Debug.Log("Machinegun");
		gunScript.typeOfGun = Gun.weaponType.Machinegun;
		gunScript.bulletSpeed = 200.0f;
		gunScript.baseSpread = 0.2f;
		gunScript.maxSpread = 1.5f;
		gunScript.maxPenetration = 3.0f;
		gunScript.fireRate = 0.1f;
		gunScript.kickBackAmount = 0.2f;
		gunScript.kickBackRecovery = 0.2f;
		gunScript.numberOfClips = gunScript.maxNumberOfClips;
	}

	void setBurst()
	{
		Debug.Log("Burst");
		gunScript.typeOfGun = Gun.weaponType.Burst;
		gunScript.bulletSpeed = 200.0f;
		gunScript.baseSpread = 0.1f;
		gunScript.maxSpread = 0.8f;
		gunScript.maxPenetration = 3.0f;
		gunScript.fireRate = 0.6f;
		gunScript.lagBetweenShots = 0.1f;
		gunScript.kickBackAmount = 0.1f;
		gunScript.kickBackRecovery = 0.1f;
		gunScript.roundsPerBurst = 3;
		gunScript.numberOfClips = gunScript.maxNumberOfClips;
	}

	void setShotgun()
	{
		Debug.Log("ShotGun");
		gunScript.typeOfGun = Gun.weaponType.Shotgun;
		gunScript.bulletSpeed = 200.0f;
		gunScript.baseSpread = 3.1f;
		gunScript.maxSpread = 8.8f;
		gunScript.maxPenetration = 3.0f;
		gunScript.fireRate = 0.6f;
		gunScript.kickBackAmount = 2.6f;
		gunScript.kickBackRecovery = 2.6f;
		gunScript.numberOfClips = gunScript.maxNumberOfClips;
	}

	void setNoob()
	{
		Debug.Log("Noob Tube");
		gunScript.typeOfGun = Gun.weaponType.Launcher;
		gunScript.typeOfLauncher = Gun.LauncherType.Grenade;
		gunScript.bulletSpeed = 45.0f;
		gunScript.baseSpread = 0.5f;
		gunScript.maxSpread = 1.0f;
		gunScript.fireRate = 1.6f;
		gunScript.kickBackAmount = 4.6f;
		gunScript.kickBackRecovery = 4.6f;
		gunScript.numberOfClips = gunScript.maxNumberOfClips;
	}

	void setRocket()
	{
		Debug.Log("Rocket Launcher");
		gunScript.typeOfGun = Gun.weaponType.Launcher;
		gunScript.typeOfLauncher = Gun.LauncherType.Rocket;
		gunScript.bulletSpeed = 40.0f;
		gunScript.baseSpread = 2.1f;
		gunScript.maxSpread = 4.5f;
		gunScript.fireRate = 1.6f;
		gunScript.kickBackAmount = 0.3f;
		gunScript.kickBackRecovery = 0.3f;
		gunScript.numberOfClips = gunScript.maxNumberOfClips;
	}
}