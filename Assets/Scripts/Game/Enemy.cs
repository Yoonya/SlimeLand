using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public int enemyNum = 0; // �� ����
    public float MaxHP = 100;
    public float HP = 100;
    public float atk = 10;
    public float atkSpd = 0.5f;
    public float spd = 2;

    public float time = 0f;
    public float swordTime = 0f; //���̾Ƹ�彺�� ����������

    private float itemRand = -1f;

    public int debuff = 0; //0 = �⺻ 1 = �̼Ӱ��� 2 =  �� �̼Ӱ���

    public Transform characterLT; //ĳ���Ϳ��� �����ǵ��� ��ġ �ʿ�

    private CharacterStatus characterStatus;
    private Animator enemyAnimator;
    private Rigidbody2D rigidbody;

    Vector3 direction;

    private void Start()
    {
        characterStatus = FindObjectOfType<CharacterStatus>();
        enemyAnimator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody2D>();
    }
    // Update is called once per frame
    void Update()
    {        
        swordTime += Time.deltaTime;
        
        /* rigidbody������ �̵��� �̰� ������ �ٸ� ĳ���Ϳ� ��ĥ�� �̵����� ���Ͽ���.
        direction = (characterLT.position - transform.position).normalized; //rigidbody �̵����� ĳ���Ϳ��Է� ���� ����
        rigidbody.MovePosition(transform.position + direction * Time.deltaTime * spd);*/

        if (debuff == 1) //�̼� ����� ���̶��
            transform.position = Vector3.MoveTowards(transform.position, characterLT.position, spd * 5 / 10 * Time.deltaTime);
        else if (debuff == 2) //���� �� �̼� �����
            transform.position = Vector3.MoveTowards(transform.position, characterLT.position, spd * 8 / 10 * Time.deltaTime);
        else
            transform.position = Vector3.MoveTowards(transform.position, characterLT.position, spd * Time.deltaTime);

        if (transform.position.x > characterLT.position.x)
            transform.localEulerAngles = new Vector3(0f, 180f, 0f);
        else
            transform.localEulerAngles = new Vector3(0f, 0f, 0f);


        if (HP <= 0) //ü���� 0 ���ϰ� �Ǹ� �Ҹ�
        {
            EnemyDie();
        }

    }

    public void EnemyDie()
    {
        ObjectPool.instance.queue[14+enemyNum].Enqueue(gameObject);
        GameManager.instance.enemy--; //�� �� ���� ���̰�
        if(enemyNum != 4)
            GameManager.instance.killScore += 10; //������ �߰��ϰ�
        else if(enemyNum == 4)
            GameManager.instance.killScore += 500; //������ �߰��ϰ�

        GameObject effect = ObjectPool.instance.queue[8].Dequeue();
        effect.SetActive(true);
        effect.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 1.5f, gameObject.transform.position.z);

        //���� ������ ����
        itemRand = Random.Range(0f, 100f);
        if (itemRand <= 25f) //����
        {
            GameObject item = ObjectPool.instance.queue[12].Dequeue();
            item.transform.position = gameObject.transform.position;
            item.SetActive(true);
        }
        else if (itemRand <= 50f) //����
        {
            GameObject item = ObjectPool.instance.queue[13].Dequeue();
            item.transform.position = gameObject.transform.position;
            item.SetActive(true);
        }

        AudioManager.instance.PlaySFX("EnemyDie");
        gameObject.SetActive(false);
    }

    public void EnemyHitted(float damage, int slimeType)
    {
        GameObject effect = ObjectPool.instance.queue[7].Dequeue();
        effect.SetActive(true);
        effect.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 1.5f, gameObject.transform.position.z);
        if (slimeType == 3) //���� ������
        {
            damage += damage * 0.2f;
        }
        HP -= damage;
        GameManager.instance.totalDamage += damage;

        if (slimeType == 0) //���� ������
        {
            if (characterStatus.hp < characterStatus.maxHP)
                characterStatus.hp += damage / 1000f;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ice"))
            if (enemyNum != 4) //������ �ƴ϶��
                debuff = 1;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ice"))
            if (enemyNum != 4) //������ �ƴ϶��
                debuff = 0;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Fire"))
        {
            if(enemyNum != 4) //������ �ƴ϶��
                HP -= HP / 20f / 60f; //�ʴ� 5% ü�°���
        }
        if (collision.CompareTag("Player")) //�÷��̾� ����
        {
            if (enemyNum == 3) //������
            {
                collision.GetComponent<CharacterStatus>().CharacterHitted(atk);
                if (collision.GetComponent<CharacterStatus>().equipCombo == 1) //��ö ������
                {
                    EnemyHitted(collision.GetComponent<CharacterStatus>().atk * 2, 1);                   
                }

                EnemyDie();
            }

            if (!GameManager.instance.isEnd && !GameManager.instance.isPause)
                if (!enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack")) //�������� �ƴҶ���
                    time += atkSpd / 60f; //time.deltatime�Ұ�

            if (time >= atkSpd)
            {
                enemyAnimator.SetTrigger("Attack");
                collision.GetComponent<CharacterStatus>().CharacterHitted(atk);

                if (collision.GetComponent<CharacterStatus>().equipCombo == 1) //��ö ������
                {
                    EnemyHitted(collision.GetComponent<CharacterStatus>().atk * 2, 1);
                }

                time = 0f;
            }
        }
    }

}
