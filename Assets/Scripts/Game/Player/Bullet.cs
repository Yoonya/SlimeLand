using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bullet : MonoBehaviour
{
    //0 = ��, 1 = ������, 2 = ����, 3 = ��ȭ��, 4 = ��ȭ������, 5 = ��ȭ����, 10 = ���簭ȭ��, 11 = ���簭ȭ����
    public int bulletType;
    public float atk;
    public float speed; // �̵� �ӵ��� ĳ���� �̵��ӵ��� 2��
    public float crit;
    public float weaponCombo;
    public int equipCombo;

    public bool istriggered = false; //���� ��ü�� �����ϱ� ����

    private void OnEnable()
    {
        Invoke("DestroyObject", 2f); //������ �ð� �Ŀ� ����
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.up * speed * 2f * Time.deltaTime;
    }

    private void DestroyObject()
    {
        istriggered = false; //�ʱ�ȭ
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
        if (collision.CompareTag("Block")) //�� ������ ����� ����
            DestroyObject();

        if (collision.CompareTag("Enemy"))
        {
            switch (bulletType)
            {
                case 0://���� ���� Ÿ��
                case 3:
                case 10:
                    float critRand = Random.Range(0f, 100f); //ũ��Ƽ��
                    if (!istriggered)
                    {                      
                        if (weaponCombo == 2) //����ȿ�� Gunner�϶� 2�� ������
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
                case 1://�����̴� ���� Ÿ��, �ð��� ���� �� ����, ���ݷ��� 1/3
                case 4:
                    critRand = Random.Range(0f, 100f); //ũ��Ƽ��
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
                case 2://������ ���� Ÿ��, ���ݷ��� 1/3
                case 5:
                    critRand = Random.Range(0f, 100f); //ũ��Ƽ��
                    Collider2D[] rangeCol = Physics2D.OverlapCircleAll(gameObject.transform.position, 1.5f);
                    GameObject effect = ObjectPool.instance.queue[9].Dequeue();
                    if (!istriggered)
                    {
                       
                        for (int i = 0; i < rangeCol.Length; i++)
                        {
                            if (rangeCol[i].CompareTag("Enemy")) //���̶��
                            {
                                if (weaponCombo == 3) //����ȿ�� Launcher�϶� 1/2�� ������
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
                        critRand = Random.Range(0f, 100f); //ũ��Ƽ��
                        rangeCol = Physics2D.OverlapCircleAll(gameObject.transform.position, 3f);
                        for (int i = 0; i < rangeCol.Length; i++)
                        {
                            if (rangeCol[i].CompareTag("Enemy")) //���̶��
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
