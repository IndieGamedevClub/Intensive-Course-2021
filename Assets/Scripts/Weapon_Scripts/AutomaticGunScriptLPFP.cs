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

	private void Start () {

		anim = GetComponent<Animator>(); //получаем компонент Animator
		currentAmmo = ammo; //ставим к-во начальной обоймы

		
		//Сохраняем название оружия
		storedWeaponName = weaponName;
		currentWeaponText.text = weaponName;
		
		//Переводим в текст к-во патрон
		totalAmmoText.text = ammo.ToString();
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
				
				anim.Play ("Fire", 0, 0f);
			

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
		} 
		//Восстанавливаем к-во патрон в обойме
		currentAmmo = ammo;
		outOfAmmo = false;
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
