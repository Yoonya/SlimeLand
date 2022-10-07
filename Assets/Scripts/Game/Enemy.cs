using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public int enemyNum = 0; // 적 종류
    public float MaxHP = 100;
    public float HP = 100;
    public float atk = 10;
    public float atkSpd = 0.5f;
    public float spd = 2;

    public float time = 0f;
    public float swordTime = 0f; //다이아몬드스텝 데미지방지

    private float itemRand = -1f;

    public int debuff = 0; //0 = 기본 1 = 이속감소 2 =  불 이속감소

    public Transform characterLT; //캐릭터에게 유도되도록 위치 필요

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
        
        /* rigidbody에서의 이동은 이게 맞으나 다른 캐릭터와 겹칠때 이동하지 못하였다.
        direction = (characterLT.position - transform.position).normalized; //rigidbody 이동으로 캐릭터에게로 방향 설정
        rigidbody.MovePosition(transform.position + direction * Time.deltaTime * spd);*/

        if (debuff == 1) //이속 디버프 중이라면
            transform.position = Vector3.MoveTowards(transform.position, characterLT.position, spd * 5 / 10 * Time.deltaTime);
        else if (debuff == 2) //히든 불 이속 디버프
            transform.position = Vector3.MoveTowards(transform.position, characterLT.position, spd * 8 / 10 * Time.deltaTime);
        else
            transform.position = Vector3.MoveTowards(transform.position, characterLT.position, spd * Time.deltaTime);

        if (transform.position.x > characterLT.position.x)
            transform.localEulerAngles = new Vector3(0f, 180f, 0f);
        else
            transform.localEulerAngles = new Vector3(0f, 0f, 0f);


        if (HP <= 0) //체력이 0 이하가 되면 소멸
        {
            EnemyDie();
        }

    }

    public void EnemyDie()
    {
        ObjectPool.instance.queue[14+enemyNum].Enqueue(gameObject);
        GameManager.instance.enemy--; //총 적 수를 줄이고
        if(enemyNum != 4)
            GameManager.instance.killScore += 10; //점수를 추가하고
        else if(enemyNum == 4)
            GameManager.instance.killScore += 500; //점수를 추가하고

        GameObject effect = ObjectPool.instance.queue[8].Dequeue();
        effect.SetActive(true);
        effect.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 1.5f, gameObject.transform.position.z);

        //랜덤 아이템 생성
        itemRand = Random.Range(0f, 100f);
        if (itemRand <= 25f) //포션
        {
            GameObject item = ObjectPool.instance.queue[12].Dequeue();
            item.transform.position = gameObject.transform.position;
            item.SetActive(true);
        }
        else if (itemRand <= 50f) //점수
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
        if (slimeType == 3) //복싱 슬라임
        {
            damage += damage * 0.2f;
        }
        HP -= damage;
        GameManager.instance.totalDamage += damage;

        if (slimeType == 0) //흡혈 슬라임
        {
            if (characterStatus.hp < characterStatus.maxHP)
                characterStatus.hp += damage / 1000f;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ice"))
            if (enemyNum != 4) //보스가 아니라면
                debuff = 1;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ice"))
            if (enemyNum != 4) //보스가 아니라면
                debuff = 0;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Fire"))
        {
            if(enemyNum != 4) //보스가 아니라면
                HP -= HP / 20f / 60f; //초당 5% 체력감소
        }
        if (collision.CompareTag("Player")) //플레이어 공격
        {
            if (enemyNum == 3) //자폭병
            {
                collision.GetComponent<CharacterStatus>().CharacterHitted(atk);
                if (collision.GetComponent<CharacterStatus>().equipCombo == 1) //강철 슬라임
                {
                    EnemyHitted(collision.GetComponent<CharacterStatus>().atk * 2, 1);                   
                }

                EnemyDie();
            }

            if (!GameManager.instance.isEnd && !GameManager.instance.isPause)
                if (!enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack")) //공격중이 아닐때만
                    time += atkSpd / 60f; //time.deltatime불가

            if (time >= atkSpd)
            {
                enemyAnimator.SetTrigger("Attack");
                collision.GetComponent<CharacterStatus>().CharacterHitted(atk);

                if (collision.GetComponent<CharacterStatus>().equipCombo == 1) //강철 슬라임
                {
                    EnemyHitted(collision.GetComponent<CharacterStatus>().atk * 2, 1);
                }

                time = 0f;
            }
        }
    }

}
