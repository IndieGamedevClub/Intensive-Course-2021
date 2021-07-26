using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;
public class ZombieController : MonoBehaviour
{
    private NavMeshAgent enemy; //����� �������������� ����������
    private Transform player; //�����
    private bool attacking = false; //����� �������?

    [HideInInspector]
    public bool dead = false; //����� ����?

    public Animator animator;

    public bool isHit = false; //����� ������� ����?

    public int recivedDamage; //�-�� ����������� �����

    public int zombieHealth = 100; //�������� �����
    public int zombieDamage = 10; //���� ����� �� ����

    public GameObject[] hurtBoxes; //��������������� "����" �����

    void Start()
    {
        //������� �� ����� ������ � �������� ������
        player = GameObject.FindGameObjectWithTag("Player").transform;
        enemy = gameObject.GetComponent<NavMeshAgent>();
    }

    
    void Update()
    {
        if(enemy.enabled) //���� ����� �������, �� ���� ������ �� �����
            enemy.SetDestination(player.position);

        //������� ����
        if (isHit && !dead)
        {
            if (zombieHealth > 0)
            {
                StartCoroutine(GetHit(recivedDamage));
            }
            else//���� �������� ���������, �� ����� ����
            {
                dead = true;
                StartCoroutine(Death());
            }
        }

        if (zombieHealth <= 0 && enemy.enabled)//���� �������� ���������, �� ����� ����
        {
            dead = true;
            StartCoroutine(Death());
        }

        if (dead)
        {
            //��������� ��������� ���������� �����
            gameObject.GetComponent<CapsuleCollider>().enabled = false;
            enemy.enabled = false;
            gameObject.GetComponent<Rigidbody>().useGravity = false;
            gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }

        //������� ������, ���� ����� ������� ���������� ������
        if (enemy.enabled && Vector3.Distance(player.position, transform.position) - 0.7 <= enemy.stoppingDistance && !dead)
        {
            if (!attacking)
                StartCoroutine(Attack());
        }
    }
    public IEnumerator GetHit(int damage)
    {
        enemy.enabled = false; //��������� �����, ����� �� �� �����, ��� ��������� ����� ��������� �����
        
        //����������� �������� ��������� �����
        animator.SetTrigger("Hit");
        zombieHealth -= damage;

        isHit = false;
        foreach (var box in hurtBoxes) //��������� "����" �����
        {
            box.SetActive(false);
        }

        
        yield return new WaitForSeconds(0.5f); //���� ����� ��������
        if(!isHit && !dead)
            enemy.enabled = true; 
    }

    public IEnumerator Attack()
    {
        //�������� �������� ����� � �������� "����"
        attacking = true;
        animator.SetTrigger("Attack");
        foreach (var box in hurtBoxes)
        {
            box.SetActive(true);
            box.GetComponent<HurtBox>().damage = zombieDamage;
        }

        enemy.enabled = false; //��������� ������������ �����

        yield return new WaitForSeconds(2f);

        //��������� "����"
        foreach (var box in hurtBoxes)
        {
            box.SetActive(false);
        }
        attacking = false;
        enemy.enabled = true;
    }

    public IEnumerator Death()
    {
        //��������� ������������ � �������� �������� ������
        enemy.enabled = false;
        animator.SetTrigger("Death");

        //��������� � ������� ������ ������
        player.gameObject.GetComponent<ScoreController>().score++;

        yield return new WaitForSeconds(5f);
        StartCoroutine(Hide()); //������ ���� �����, ����� ��� �� ��������� ��
    }

    public IEnumerator Hide()
    {
        //�������� ������ ���� ��� "���"
        transform.position = new Vector3(transform.position.x, transform.position.y-0.005f, transform.position.z); 
        
        yield return new WaitForSeconds(0.01f);

        if (transform.position.y > -3f) //���� ���� ��� �� ����������, �� ����� ������ ���
        {
            StartCoroutine(Hide());
        }
        else
        {
            Destroy(gameObject); //����� - ���������� ������
        }
    }
}
