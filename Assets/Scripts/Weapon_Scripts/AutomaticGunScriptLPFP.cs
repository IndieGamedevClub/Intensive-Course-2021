using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AutomaticGunScriptLPFP : MonoBehaviour {

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
	//Check if running
	private bool isRunning;
	//Check if walking
	private bool isWalking;

	//How much ammo is currently left
	private int currentAmmo;
	//Totalt amount of ammo
	[Tooltip("How much ammo the weapon should have.")]
	public int ammo;
	//Check if out of ammo
	private bool outOfAmmo;

	[Header("Bullet Settings")]
	//Bullet
	[Tooltip("How much force is applied to the bullet when shooting.")]
	public float bulletForce = 400.0f;

	[Header("Muzzleflash Settings")]
	public bool enableMuzzleflash = true;
	public ParticleSystem muzzleParticles;
	public bool enableSparks = true;
	public ParticleSystem sparkParticles;
	public int minSparkEmission = 1;
	public int maxSparkEmission = 7;

	[Header("Muzzleflash Light Settings")]
	public Light muzzleflashLight;
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
	
	[Header("Prefabs & Spawn Points")]
	public Transform bulletPrefab;
	public Transform casingPrefab;
	//Casing spawn point array
	public Transform casingSpawnPoint;
	//Bullet prefab spawn from this point
	public Transform bulletSpawnPoint;
	
	[Header("Audio")]
	public AudioClip shootSound;
	public AudioClip reloadSoundOutOfAmmo;

	private void Start () {
		
		//Set the animator component
		anim = GetComponent<Animator>();
		//Set current ammo to total ammo value
		currentAmmo = ammo;

		muzzleflashLight.enabled = false;
		
		//Save the weapon name
		storedWeaponName = weaponName;
		currentWeaponText.text = weaponName;
		
		//Set total ammo text from total ammo int
		totalAmmoText.text = ammo.ToString();

		//Set the shoot sound to audio source
		shootAudioSource.clip = shootSound;
	}

	private void Update () {
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
			StartCoroutine(AutoReload());
		} 
		else 
		{
			//When ammo is full, show weapon name again
			currentWeaponText.text = storedWeaponName.ToString ();
			//Toggle bool
			outOfAmmo = false;
		}
			
		//AUtomatic fire
		//Left click hold 
		if (Input.GetMouseButton (0) && !outOfAmmo && !isReloading && !isRunning) 
		{
			//Shoot automatic
			if (Time.time - lastFired > 1 / fireRate) 
			{
				lastFired = Time.time;

				//Remove 1 bullet from ammo
				currentAmmo -= 1;

				shootAudioSource.clip = shootSound;
				shootAudioSource.Play ();
				
				anim.Play ("Fire", 0, 0f);
				//If random muzzle is false
				//Only emit if random value is 1
				
				if (enableSparks == true) 
				{
					//Emit random amount of spark particles
					sparkParticles.Emit (Random.Range (minSparkEmission, maxSparkEmission));
				}
				if (enableMuzzleflash == true) 
				{
					muzzleParticles.Emit (1);
					//Light flash start
					StartCoroutine (MuzzleFlashLight ());
				}

				//Spawn bullet from bullet spawnpoint
				var bullet = (Transform)Instantiate (
					bulletPrefab,
					bulletSpawnPoint.transform.position,
					bulletSpawnPoint.transform.rotation);

				//Add damage
				bullet.GetComponent<BulletScript>().bulletDamage = damage;

				//Add velocity to the bullet
				bullet.GetComponent<Rigidbody>().velocity = 
					bullet.transform.forward * bulletForce;
				
				//Spawn casing prefab at spawnpoint
				Instantiate (casingPrefab, 
					casingSpawnPoint.transform.position, 
					casingSpawnPoint.transform.rotation);
			}
		}

		//Walking when pressing down WASD keys
		if (Input.GetKey (KeyCode.W) && !isRunning || 
			Input.GetKey (KeyCode.A) && !isRunning || 
			Input.GetKey (KeyCode.S) && !isRunning || 
			Input.GetKey (KeyCode.D) && !isRunning) 
		{
			anim.SetBool ("Walk", true);
		} else {
			anim.SetBool ("Walk", false);
		}

		//Running when pressing down W and Left Shift key
		if ((Input.GetKey (KeyCode.W) && Input.GetKey (KeyCode.LeftShift))) 
		{
			isRunning = true;
		} else {
			isRunning = false;
		}
		
		//Run anim toggle
		if (isRunning == true) 
		{
			anim.SetBool ("Run", true);
		} 
		else 
		{
			anim.SetBool ("Run", false);
		}
	}

	private IEnumerator AutoReload ()
	{
		yield return new WaitForSeconds(0.35f);
		if (outOfAmmo == true) 
		{
			//Play diff anim if out of ammo
			anim.Play ("Reload Out Of Ammo", 0, 0f);

			mainAudioSource.clip = reloadSoundOutOfAmmo;
			mainAudioSource.Play ();
		} 
		//Restore ammo when reloading
		currentAmmo = ammo;
		outOfAmmo = false;
	}

	//Show light when shooting, then disable after set amount of time
	private IEnumerator MuzzleFlashLight () {
		
		muzzleflashLight.enabled = true;
		yield return new WaitForSeconds (lightDuration);
		muzzleflashLight.enabled = false;
	}

	//Check current animation playing
	private void AnimationCheck () {
		//Check if reloading
		//Check both animations
		if (anim.GetCurrentAnimatorStateInfo (0).IsName ("Reload Out Of Ammo") || 
			anim.GetCurrentAnimatorStateInfo (0).IsName ("Reload Ammo Left")) 
		{
			isReloading = true;
		} 
		else 
		{
			isReloading = false;
		}
	}
}
