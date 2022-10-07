using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bullet : MonoBehaviour
{
    //0 = 총, 1 = 지팡이, 2 = 대포, 3 = 강화총, 4 = 강화지팡이, 5 = 강화대포, 10 = 히든강화총, 11 = 히든강화대포
    public int bulletType;
    public float atk;
    public float speed; // 이동 속도는 캐릭터 이동속도의 2배
    public float crit;
    public float weaponCombo;
    public int equipCombo;

    public bool istriggered = false; //단일 객체만 공격하기 위해

    private void OnEnable()
    {
        Invoke("DestroyObject", 2f); //적당한 시간 후에 삭제
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.up * speed * 2f * Time.deltaTime;
    }

    private void DestroyObject()
    {
        istriggered = false; //초기화
        ObjectPool.instance.queue[bulletType].Enqueue(gameObject);
        gameObject.SetActive(false);
    }

    private void DamageText(GameObject collision, float damage)
    {
        GameObject damageText = ObjectPool.instance.queue[19].Dequeue();
        damageText.gameObject.SetActive(true);
        //damageText.GetComponent<RectTransform>().anchoredPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, collision.transform.position);
        damageText.GetComponent<Text>().text = ((int)damage).ToString();
        damageText.transform.localScale = new Vector3(1f, 1f, 1f);
        damageText.transform.position = Camera.main.WorldToScreenPoint(collision.transform.position + new Vector3(0, 2.5f, 0));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Block")) //맵 밖으로 벗어나면 삭제
            DestroyObject();

        if (collision.CompareTag("Enemy"))
        {
            switch (bulletType)
            {
                case 0://총은 단일 타격
                case 3:
                case 10:
                    float critRand = Random.Range(0f, 100f); //크리티컬
                    if (!istriggered)
                    {                      
                        if (weaponCombo == 2) //직업효과 Gunner일때 2배 데미지
                        {
                            if (critRand <= crit)
                            {
                                collision.GetComponent<Enemy>().EnemyHitted(atk * 2 * 2, equipCombo);
                                DamageText(collision.gameObject, atk * 2 * 2);
                            }
                            else
                            {
                                collision.GetComponent<Enemy>().EnemyHitted(atk * 2, equipCombo);
                                DamageText(collision.gameObject, atk * 2);
                            }
                        }
                        else
                        {
                            if (critRand <= crit)
                            {
                                collision.GetComponent<Enemy>().EnemyHitted(atk * 2, equipCombo);
                                DamageText(collision.gameObject, atk * 2);
                            }
                            else
                            {
                                collision.GetComponent<Enemy>().EnemyHitted(atk, equipCombo);
                                DamageText(collision.gameObject, atk);
                            }
                        }

                        DestroyObject();
                        istriggered = true;
                    }
                    break;
                case 1://지팡이는 관통 타격, 시간이 지난 후 삭제, 공격력의 1/3
                case 4:
                    critRand = Random.Range(0f, 100f); //크리티컬
                    if (critRand <= crit)
                    {
                        collision.GetComponent<Enemy>().EnemyHitted(atk * 2 / 4, equipCombo);
                        DamageText(collision.gameObject, atk * 2 / 4);
                    }
                    else
                    {
                        collision.GetComponent<Enemy>().EnemyHitted(atk / 4, equipCombo);
                        DamageText(collision.gameObject, atk / 4);
                    }
                    break;
                case 2://대포는 범위 타격, 공격력의 1/3
                case 5:
                    critRand = Random.Range(0f, 100f); //크리티컬
                    Collider2D[] rangeCol = Physics2D.OverlapCircleAll(gameObject.transform.position, 1.5f);
                    GameObject effect = ObjectPool.instance.queue[9].Dequeue();
                    if (!istriggered)
                    {
                       
                        for (int i = 0; i < rangeCol.Length; i++)
                        {
                            if (rangeCol[i].CompareTag("Enemy")) //적이라면
                            {
                                if (weaponCombo == 3) //직업효과 Launcher일때 1/2배 데미지
                                {
                                    if (critRand <= crit)
                                    {
                                        rangeCol[i].GetComponent<Enemy>().EnemyHitted(atk, equipCombo);
                                        DamageText(rangeCol[i].gameObject, atk);
                                    }
                                    else
                                    {
                                        rangeCol[i].GetComponent<Enemy>().EnemyHitted(atk / 2, equipCombo);
                                        DamageText(rangeCol[i].gameObject, atk / 2);
                                    }
                                }
                                else
                                {
                                    if (critRand <= crit)
                                    {
                                        rangeCol[i].GetComponent<Enemy>().EnemyHitted(atk * 2 / 3, equipCombo);
                                        DamageText(rangeCol[i].gameObject, atk * 2 / 3);
                                    }
                                    else
                                    {
                                        rangeCol[i].GetComponent<Enemy>().EnemyHitted(atk / 3, equipCombo);
                                        DamageText(rangeCol[i].gameObject, atk / 3);
                                    }
                                }
                            }
                        }                       
                        effect.transform.localScale = new Vector3(1f, 1f, 1f);
                        effect.SetActive(true);
                        effect.transform.position = gameObject.transform.position;
                        DestroyObject();
                        istriggered = true;
                    }

                    break;
               case 11:
                    if (!istriggered)
                    {
                        critRand = Random.Range(0f, 100f); //크리티컬
                        rangeCol = Physics2D.OverlapCircleAll(gameObject.transform.position, 3f);
                        for (int i = 0; i < rangeCol.Length; i++)
                        {
                            if (rangeCol[i].CompareTag("Enemy")) //적이라면
                            {
                                if (critRand <= crit)
                                {
                                    rangeCol[i].GetComponent<Enemy>().EnemyHitted(atk * 1.5f * 2, equipCombo);
                                    DamageText(collision.gameObject, atk * 1.5f * 2);
                                }
                                else
                                {
                                    rangeCol[i].GetComponent<Enemy>().EnemyHitted(atk * 1.5f, equipCombo);
                                    DamageText(collision.gameObject, atk * 1.5f);
                                }

                            }
                        }
                        effect = ObjectPool.instance.queue[9].Dequeue();
                        effect.transform.localScale = new Vector3(2f, 2f, 2f);
                        effect.SetActive(true);
                        effect.transform.position = gameObject.transform.position;
                        DestroyObject();
                        istriggered = true;
                    }
                    break;

            }
        }
    }
}
