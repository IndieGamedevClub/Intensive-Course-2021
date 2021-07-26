using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;
public class ZombieController : MonoBehaviour
{
    private NavMeshAgent enemy; //Агент Искусственного Интеллекта
    private Transform player; //Игрок
    private bool attacking = false; //Зомби атакует?

    [HideInInspector]
    public bool dead = false; //Зомби умер?

    public Animator animator;

    public bool isHit = false; //Зомби получил урон?

    public int recivedDamage; //К-во полученного урона

    public int zombieHealth = 100; //Здоровье зомби
    public int zombieDamage = 10; //Урон зомби за удар

    public GameObject[] hurtBoxes; //Вспомогательные "руки" зомби

    void Start()
    {
        //Нахоидм на сцене игрока и получаем Агента
        player = GameObject.FindGameObjectWithTag("Player").transform;
        enemy = gameObject.GetComponent<NavMeshAgent>();
    }

    
    void Update()
    {
        if(enemy.enabled) //Если зомби активен, то ищем игрока на карте
            enemy.SetDestination(player.position);

        //Получил урон
        if (isHit && !dead)
        {
            if (zombieHealth > 0)
            {
                StartCoroutine(GetHit(recivedDamage));
            }
            else//если здоровье кончилось, то зомби умер
            {
                dead = true;
                StartCoroutine(Death());
            }
        }

        if (zombieHealth <= 0 && enemy.enabled)//если здоровье кончилось, то зомби умер
        {
            dead = true;
            StartCoroutine(Death());
        }

        if (dead)
        {
            //Отключаем некоторые компоненты зомби
            gameObject.GetComponent<CapsuleCollider>().enabled = false;
            enemy.enabled = false;
            gameObject.GetComponent<Rigidbody>().useGravity = false;
            gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }

        //Атакуем игрока, если зомби подошел достаточно близко
        if (enemy.enabled && Vector3.Distance(player.position, transform.position) - 0.7 <= enemy.stoppingDistance && !dead)
        {
            if (!attacking)
                StartCoroutine(Attack());
        }
    }
    public IEnumerator GetHit(int damage)
    {
        enemy.enabled = false; //отключаем врага, чтобы он не ходил, при получении урона некоторое время
        
        //Проигрываем анимацию получения урона
        animator.SetTrigger("Hit");
        zombieHealth -= damage;

        isHit = false;
        foreach (var box in hurtBoxes) //отключаем "руки" зомби
        {
            box.SetActive(false);
        }

        
        yield return new WaitForSeconds(0.5f); //ждем конца анимации
        if(!isHit && !dead)
            enemy.enabled = true; 
    }

    public IEnumerator Attack()
    {
        //Включаем анимацию удара и включаем "руки"
        attacking = true;
        animator.SetTrigger("Attack");
        foreach (var box in hurtBoxes)
        {
            box.SetActive(true);
            box.GetComponent<HurtBox>().damage = zombieDamage;
        }

        enemy.enabled = false; //отключаем передвижение врага

        yield return new WaitForSeconds(2f);

        //отключаем "руки"
        foreach (var box in hurtBoxes)
        {
            box.SetActive(false);
        }
        attacking = false;
        enemy.enabled = true;
    }

    public IEnumerator Death()
    {
        //Отключаем передвижение и включаем анимацию смерти
        enemy.enabled = false;
        animator.SetTrigger("Death");

        //добавляем в счетчик убитых врагов
        player.gameObject.GetComponent<ScoreController>().score++;

        yield return new WaitForSeconds(5f);
        StartCoroutine(Hide()); //прячем тело врага, чтобы оно не нагружало ПК
    }

    public IEnumerator Hide()
    {
        //медленно прячем тело под "пол"
        transform.position = new Vector3(transform.position.x, transform.position.y-0.005f, transform.position.z); 
        
        yield return new WaitForSeconds(0.01f);

        if (transform.position.y > -3f) //если тело еще не спряталось, то снова прячем его
        {
            StartCoroutine(Hide());
        }
        else
        {
            Destroy(gameObject); //иначе - уничтожаем объект
        }
    }
}
