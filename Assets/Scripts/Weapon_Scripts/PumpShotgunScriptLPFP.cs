using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PumpShotgunScriptLPFP : MonoBehaviour {

	//Animator component attached to weapon
	Animator anim;

	[Header("UI Weapon Name")]
	[Tooltip("Name of the current weapon, shown in the game UI.")]
	public string weaponName;
	private string storedWeaponName;

	//Used for fire rate
	private float lastFired;
	[Header("Weapon Settings")]
	[Tooltip("How fast the weapon fires, higher value means faster rate of fire.")]
	public float fireRate;
	public int damage;

	//Check if reloading
	private bool isReloading;
	//Chek if shooting
	private bool isShooting;
	//Check if running
	private bool isRunning;
	//Check if walking
	private bool isWalking;

	//How much ammo is currently left
	private int currentAmmo;
	//Totalt amount of ammo
	[Tooltip("How much ammo the weapon should have.")]
	private int ammo = 5;
	//Check if out of ammo
	private bool outOfAmmo;

	[Header("Bullet Settings")]
	//Bullet
	[Tooltip("How much force is applied to the bullet when shooting.")]
	public float bulletForce = 400;

	[Header("Muzzleflash Settings")]
	public bool enableMuzzleFlash;
	public ParticleSystem muzzleParticles;
	public bool enableSparks;
	public ParticleSystem sparkParticles;
	public int minSparkEmission = 1;
	public int maxSparkEmission = 7;

	[Header("Muzzleflash Light Settings")]
	public Light muzzleFlashLight;
	public float lightDuration = 0.02f;

	[Header("Audio Source")]
	//Main audio source
	public AudioSource mainAudioSource;
	//Audio source used for shoot sound
	public AudioSource shootAudioSource;

	[Header("UI Components")]
	public Text currentWeaponText;
	public Text currentAmmoText;
	public Text totalAmmoText;

	[Header("Prefabs")]
	public Transform bulletPrefab;
	public Transform casingPrefab;

	[Header("Spawnpoints")]
	//Array holding casing spawn points 
	//(some weapons use more than one casing spawn)
	public float casingDelayTimer;
	//Casing spawn point array
	public Transform casingSpawnPoint;
	//Bullet prefab spawn from this point
	public Transform[] bulletSpawnPoint;

	public AudioClip shootSound;
	public AudioClip takeOutSound;

	private void Awake () 
	{
		//Set the animator component
		anim = GetComponent<Animator>();
		//Set current ammo to total ammo value
		currentAmmo = ammo;

		muzzleFlashLight.enabled = false;
	}

	private void Start () 
	{
		//Save the weapon name
		storedWeaponName = weaponName;
		//Get weapon name from string to text
		currentWeaponText.text = weaponName;
		//Set total ammo text from total ammo int
		totalAmmoText.text = ammo.ToString();

		//Set the shoot sound to audio source
		shootAudioSource.clip = shootSound;
	}
	
	private void Update () 
	{
		//Set current ammo text from ammo int
		currentAmmoText.text = currentAmmo.ToString ();

		//Continosuly check which animation 
		//is currently playing
		AnimationCheck ();

		//If out of ammo
		if (currentAmmo == 0) 
		{
			//Show out of ammo text
			currentWeaponText.text = "OUT OF AMMO";
			//Toggle bool
			outOfAmmo = true;
			//Auto reload if true
			StartCoroutine (AutoReload ());
		} 
		else 
		{
			//When ammo is full, show weapon name again
			currentWeaponText.text = storedWeaponName.ToString ();
			//Toggle bool
			outOfAmmo = false;
		}
			
		//Fire
		if (Input.GetMouseButton (0) && !outOfAmmo && !isReloading && !isRunning) 
		{
			if (Time.time - lastFired > 1 / fireRate) 
			{
				lastFired = Time.time;

				//Remove 1 bullet from ammo
				currentAmmo -= 1;

				shootAudioSource.clip = shootSound;
				shootAudioSource.Play ();
				
				anim.Play ("Fire", 0, 0f);
				//Only emit if random value is 1

				if (enableSparks == true) 
				{
					//Emit random amount of spark particles
					sparkParticles.Emit (Random.Range (minSparkEmission, maxSparkEmission));
				}
				if (enableMuzzleFlash == true) 
				{
					muzzleParticles.Emit (1);
					//Light flash start
					StartCoroutine (MuzzleFlashLight ());
				}

				//Bullet spawnpoints array
				for (int i = 0; i < bulletSpawnPoint.Length; i++) 
				{
					//If random bullet spawn point is enabled
					// (Used for shotgun bullet spread)

					//Spawn bullets from bullet spawnpoint positions using array
					var bullet = (Transform)Instantiate (
						bulletPrefab,
						bulletSpawnPoint [i].transform.position,
						bulletSpawnPoint [i].transform.rotation);

					//Add damage
					bullet.GetComponent<BulletScript>().bulletDamage = damage;

					//Add velocity to the bullets
					bullet.GetComponent<Rigidbody>().velocity = 
						bullet.transform.forward * bulletForce;
				}

				StartCoroutine (CasingDelay ());
			}
		}

		//Walking when pressing down WASD keys
		if (Input.GetKey (KeyCode.W) && !isRunning && !isShooting || 
			Input.GetKey (KeyCode.A) && !isRunning && !isShooting || 
			Input.GetKey (KeyCode.S) && !isRunning && !isShooting || 
			Input.GetKey (KeyCode.D) && !isRunning && !isShooting) 
		{
			anim.SetBool ("Walk", true);
		} else {
			anim.SetBool ("Walk", false);
		}

		//Running when pressing down W and Left Shift key
		if ((Input.GetKey (KeyCode.W) && Input.GetKey (KeyCode.LeftShift))) 
		{
			isRunning = true;
		} 
		else 
		{
			isRunning = false;
		}
		
		//Run anim toggle
		if (isRunning == true) {
			anim.SetBool ("Run", true);
		} else {
			anim.SetBool ("Run", false);
		}
	}

	private IEnumerator CasingDelay () {
		//Wait before spawning casing
		yield return new WaitForSeconds (casingDelayTimer);
		//Instantiate casing prefab at spawnpoint
		Instantiate (casingPrefab, 
			casingSpawnPoint.transform.position, 
			casingSpawnPoint.transform.rotation);
	}

	private IEnumerator AutoReload () {
		//Wait for set amount of time
		yield return new WaitForSeconds (0.35f);

		if (outOfAmmo == true) 
		{
			//Play diff anim if out of ammo
			anim.Play ("Reload Open", 0, 0f);
		} 
		//Restore ammo when reloading
		currentAmmo = ammo;
		outOfAmmo = false;
	}

	//Reload
	private void Reload () {
		if (outOfAmmo == true) 
		{
			//Play diff anim if out of ammo
			anim.Play ("Reload Open", 0, 0f);
		} 
		else 
		{
			//Play diff anim if ammo left
			anim.Play ("Reload Open", 0, 0f);
		}
		//Restore ammo when reloading
		currentAmmo = ammo;
		outOfAmmo = false;
	}

	//Show light when shooting, then disable after set amount of time
	private IEnumerator MuzzleFlashLight () 
	{
		muzzleFlashLight.enabled = true;
		yield return new WaitForSeconds (lightDuration);
		muzzleFlashLight.enabled = false;
	}

	//Check current animation playing
	private void AnimationCheck () 
	{
		//Check if reloading
		//Check both animations
		if (anim.GetCurrentAnimatorStateInfo (0).IsName ("Reload Open") ||
			anim.GetCurrentAnimatorStateInfo(0).IsName("Insert Shell") ||
			anim.GetCurrentAnimatorStateInfo(0).IsName("Insert Shell 1") ||
			anim.GetCurrentAnimatorStateInfo(0).IsName("Insert Shell 2") ||
			anim.GetCurrentAnimatorStateInfo(0).IsName("Insert Shell 3") ||
			anim.GetCurrentAnimatorStateInfo(0).IsName("Insert Shell 4"))
		{
			isReloading = true;
		} 
		else 
		{
			isReloading = false;
		}

		//Check is shooting
		if (anim.GetCurrentAnimatorStateInfo (0).IsName ("Fire")) 
		{
			isShooting = true;
		} 
		else 
		{
			isShooting = false;
		}
	}
}