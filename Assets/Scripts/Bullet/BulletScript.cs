using UnityEngine;
using System.Collections;

public class BulletScript : MonoBehaviour {

	[Range(5, 100)]
	[Tooltip("После какого времени пуля должна уничтожиться?")]
	public float destroyAfter;
	[Tooltip("Если включено, то уничтожает пулю после столкновения")]
	public bool destroyOnImpact = false;
	[Tooltip("Минимальное время после столковения когда пропадет пуля")]
	public float minDestroyTime;
	[Tooltip("Максимальное время после столковения когда пропадет пуля")]
	public float maxDestroyTime;

	[HideInInspector]
	public int bulletDamage;

	[Header("Префабы эффекта столкновения")]
	public Transform metalImpactPrefab;
	public Transform bloodImpactPrefab;

	private void Start () 
	{
		//Создаем таймер самоуничтожения
		StartCoroutine (DestroyAfter ());
	}

	//Если пуля столкнулась с чем-нибудь
	private void OnCollisionEnter (Collision collision) 
	{
		//Если мы не уничтожаем пулю после столкновения, то запускаем таймер уничтожения
		if (!destroyOnImpact) 
		{
			StartCoroutine (DestroyTimer ());
		}
		//Иначе, уничтожаем пулю после столкновения
		else 
		{
			Destroy (gameObject);
		}

		//Если пуля столкнулась с объектом с тегом "Enviroment"
		if (collision.transform.tag == "Enviroment") 
		{
			//Создаем эффект попадания по объекту
			Instantiate (metalImpactPrefab, transform.position, 
				Quaternion.LookRotation (collision.contacts [0].normal));

			//Уничтожаем пулю
			Destroy(gameObject);
		}

		//If bullet collides with "Target" tag
		//Если пуля столкнулась с объектом с тегом "Enemy"
		if (collision.transform.tag == "Enemy") 
		{
			//Говорим врагу, что в него попали
			collision.transform.gameObject.GetComponent
				<ZombieController>().isHit = true;

			//Говорим врагу, что он получил урон
			collision.transform.gameObject.GetComponent
				<ZombieController>().recivedDamage = bulletDamage;

			//Создаем эффект крови
			Instantiate(bloodImpactPrefab, transform.position,
				Quaternion.LookRotation(collision.contacts[0].normal));

			//Уничтожаем пулю
			Destroy(gameObject);
		}
	}

	private IEnumerator DestroyTimer () 
	{
		//Ждем случайное время (от минимума до максимума)
		yield return new WaitForSeconds
			(Random.Range(minDestroyTime, maxDestroyTime));

		//Уничтожаем пулю
		Destroy(gameObject);
	}

	private IEnumerator DestroyAfter () 
	{
		//Ждем фиксированное к-во времени
		yield return new WaitForSeconds (destroyAfter);

		//Уничтожаем пулю
		Destroy(gameObject);
	}
}