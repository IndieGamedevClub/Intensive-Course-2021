using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AutomaticGunScriptLPFP : MonoBehaviour {

	//Animator прицепленный к оружию
	Animator anim;

	[Header("UI Имя оружия")]
	public string weaponName;
	private string storedWeaponName;

	private float lastFired;
	[Header("Настройки оружия")]
	public float fireRate;
	public int damage;

	//Мы перезаряжаемся?
	private bool isReloading;
	//Мы бежим?
	private bool isRunning;

	//Сколько патронов осталось
	private int currentAmmo;
	//Размер обоймы
	[Tooltip("Как много пуль есть в обойме")]
	public int ammo;
	//Мы без пуль?
	private bool outOfAmmo;

	[Header("Настройки пуль")]
	public float bulletForce = 400.0f;

	[Header("Настройки частиц выстрела")]
	public bool enableMuzzleflash = true;
	public ParticleSystem muzzleParticles;
	public bool enableSparks = true;
	public ParticleSystem sparkParticles;
	public int minSparkEmission = 1;
	public int maxSparkEmission = 7;

	[Header("Настройка света частиц выстрела")]
	public Light muzzleflashLight;
	public float lightDuration = 0.02f;

	[Header("Audio Source")]
	//Главный Audio Source
	public AudioSource mainAudioSource;
	//Audio source для выстрела
	public AudioSource shootAudioSource;

	[Header("UI Компоненты")]
	public Text currentWeaponText;
	public Text currentAmmoText;
	public Text totalAmmoText;
	
	[Header("Префабы и  Вспомогательные точки спавна")]
	public Transform bulletPrefab;
	public Transform casingPrefab;
	//Точка спавна гильз
	public Transform casingSpawnPoint;
	//Точка спавна пуль
	public Transform bulletSpawnPoint;
	
	[Header("Аудио")]
	public AudioClip shootSound;
	public AudioClip reloadSoundOutOfAmmo;

	private void Start () {

		anim = GetComponent<Animator>(); //получаем компонент Animator
		currentAmmo = ammo; //ставим к-во начальной обоймы

		muzzleflashLight.enabled = false;
		
		//Сохраняем название оружия
		storedWeaponName = weaponName;
		currentWeaponText.text = weaponName;
		
		//Переводим в текст к-во патрон
		totalAmmoText.text = ammo.ToString();

		//Задаем звук стрельбы
		shootAudioSource.clip = shootSound;
	}

	private void Update () {
		//Переводим в текст к-во патрон
		currentAmmoText.text = currentAmmo.ToString ();

		AnimationCheck (); //Проверка анимаций, чтобы они проигрывались верно

		//Если закончились патроны
		if (currentAmmo == 0) 
		{
			//Показываем текст
			currentWeaponText.text = "ПАТРОНЫ КОНЧИЛИСЬ";

			outOfAmmo = true;
			//Делаем авто-перезарядку
			StartCoroutine(AutoReload());
		} 
		else 
		{
			//Если патроны есть, то отображаем название оружия
			currentWeaponText.text = storedWeaponName.ToString ();

			outOfAmmo = false;
		}
			
		//Автоматическая стрельба на левую кнопку мыши
		if (Input.GetMouseButton (0) && !outOfAmmo && !isReloading && !isRunning) 
		{
			if (Time.time - lastFired > 1 / fireRate) 
			{
				lastFired = Time.time;

				//Убираем 1 патрон из обоймы
				currentAmmo -= 1;

				shootAudioSource.clip = shootSound;
				shootAudioSource.Play ();
				
				anim.Play ("Fire", 0, 0f);
				
				if (enableSparks == true) 
				{
					//Создаем случайную частицу выстрела
					sparkParticles.Emit (Random.Range (minSparkEmission, maxSparkEmission));
				}
				if (enableMuzzleflash == true) 
				{
					muzzleParticles.Emit (1);

					//Запускаем свет от выстрела
					StartCoroutine (MuzzleFlashLight ());
				}

				//Создаем пулю из её спавн-поинта
				var bullet = (Transform)Instantiate (
					bulletPrefab,
					bulletSpawnPoint.transform.position,
					bulletSpawnPoint.transform.rotation);

				//Добавляем урон пуле
				bullet.GetComponent<BulletScript>().bulletDamage = damage;

				//Добавляем скорость и направление для пули
				bullet.GetComponent<Rigidbody>().velocity = 
					bullet.transform.forward * bulletForce;
				
				//Создаем гильзу в её спавн-поинте
				Instantiate (casingPrefab, 
					casingSpawnPoint.transform.position, 
					casingSpawnPoint.transform.rotation);
			}
		}

		//Ходим на WASD
		if (Input.GetKey (KeyCode.W) && !isRunning || 
			Input.GetKey (KeyCode.A) && !isRunning || 
			Input.GetKey (KeyCode.S) && !isRunning || 
			Input.GetKey (KeyCode.D) && !isRunning) 
		{
			anim.SetBool ("Walk", true);
		} else {
			anim.SetBool ("Walk", false);
		}

		//Беим, когда нажат W и Левый Shift
		if ((Input.GetKey (KeyCode.W) && Input.GetKey (KeyCode.LeftShift))) 
		{
			isRunning = true;
		} else {
			isRunning = false;
		}
		
		//Включаем анимацию бега
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
		//Немного ждем, чтобы анимация выстрела проигралась
		yield return new WaitForSeconds(0.35f);

		if (outOfAmmo == true) 
		{
			//Проигрываем анимацию перезарядки
			anim.Play ("Reload Out Of Ammo", 0, 0f);

			mainAudioSource.clip = reloadSoundOutOfAmmo;
			mainAudioSource.Play ();
		} 
		//Восстанавливаем к-во патрон в обойме
		currentAmmo = ammo;
		outOfAmmo = false;
	}

	//Показываем свет, когда стреляем, затем убираем его через некоторое время
	private IEnumerator MuzzleFlashLight () {
		
		muzzleflashLight.enabled = true;
		yield return new WaitForSeconds (lightDuration);
		muzzleflashLight.enabled = false;
	}

	//Проверяем анимации
	private void AnimationCheck () {
		//Проверяем если перезаряжаемся
		if (anim.GetCurrentAnimatorStateInfo (0).IsName ("Reload Out Of Ammo")) 
		{
			isReloading = true;
		} 
		else 
		{
			isReloading = false;
		}
	}
}
