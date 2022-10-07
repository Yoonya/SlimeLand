using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//����ü�� 0,1,2�� bullet���� �����ϰ� 3,4,5�� ���⼭ ����
public class WeaponManager : MonoBehaviour
{
    //0 = ��, 1 = ����, 2 = ��
    public bool isClone = false; //Ŭ�� �����ΰ�
    public bool isSide = false; //���̵� �����ΰ�
    public bool isSideEnforceL = false; //���̵� ���Ⱑ ���� �������μ� ��ȭ�Ǿ��µ�
    public bool isSideEnforceR = false; //���̵� ���Ⱑ ���� �������μ� ��ȭ�Ǿ��µ�
    public int weaponType = -1; //0�� 1���� 2�� 3��������
    public float atk = 0f;
    public float atkSpd = 0f;
    public float time = 0f;
    public float crit = 0f;
    public float critRand = 0f;

    public int hitTime = 0; //1�����Ӵ� �� �������� ���� ����Ʈ�� ��Ʈ����Ʈ�� �ʹ� ����ȭ�Ǽ� 10����������

    private Collider2D[] rangeCol; //�� ������ ����

    //bullet�� atk�� characterWeapon���� ������������ ���� weapon���� �ѹ� �����ϰ� ������°� �ƴϱ� ������ atk�� ������Ʈ �Ǿ�� �Ѵ�.
    private CharacterStatus characterStatus;

    // Start is called before the first frame update
    void Start()
    {
        characterStatus = FindObjectOfType<CharacterStatus>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isClone && isSide)  //Ŭ���� ���̵� ���� ������
            atk = characterStatus.atk * 0.1f;
        else if (isSide) //���̵� ���� ������
            atk = characterStatus.atk * 0.5f;
        else if (isClone)//Ŭ�� ������
            atk = characterStatus.atk * 0.2f;
        else
            atk = characterStatus.atk;

        //��ȭ���� ����
        if ((characterStatus.weaponL >= 3 && characterStatus.weaponL <= 5) && (characterStatus.sideWeaponL >= 3 && characterStatus.sideWeaponL <= 5))
            isSideEnforceL = true;
        else
            isSideEnforceL = false;

        if ((characterStatus.weaponR >= 3 && characterStatus.weaponR <= 5) && (characterStatus.sideWeaponR >= 3 && characterStatus.sideWeaponR <= 5))
            isSideEnforceR = true;
        else
            isSideEnforceR = false;

        if (characterStatus.hiddenCombo != 13 && characterStatus.hiddenCombo != 14 && characterStatus.hiddenCombo != 15) //������ ������ �ƴ϶��
        {
            if (isSideEnforceL && isSideEnforceR) //��ȭ���¶�� ��ȭ
                atk += atk * 1f;
            else if (isSideEnforceL)
                atk += atk * 0.5f;
            else if (isSideEnforceR)
                atk += atk * 0.5f;
        }



        atkSpd = characterStatus.atkSpd;
        crit = characterStatus.crit;

        time += Time.deltaTime;

        if (time >= atkSpd) //�Ұ��������� fireman����
        {
            if (!GameManager.instance.isEnd)
            {
                if (weaponType == 2 && characterStatus.weaponCombo != 5 && !GameManager.instance.isPause)
                {
                    rangeCol = Physics2D.OverlapCircleAll(gameObject.transform.position, 4.0f); //���� �ȿ� ������ ����
                    for (int i = 0; i < rangeCol.Length; i++)
                    {
                        critRand = Random.Range(0f, 100f); //ũ��Ƽ��
                        if (rangeCol[i].CompareTag("Enemy")) //���̶��
                        {
                            if (critRand <= crit)
                            {
                                rangeCol[i].GetComponent<Enemy>().EnemyHitted(atk * 2f / 6f, characterStatus.equipCombo);
                                DamageText(rangeCol[i].gameObject, atk * 2f / 6f);
                            }
                            else
                            {
                                rangeCol[i].GetComponent<Enemy>().EnemyHitted(atk / 6f, characterStatus.equipCombo);
                                DamageText(rangeCol[i].gameObject, atk / 6f);
                            }
                        }

                    }
                }

            }

            time = 0;
        }

        //Fireman�� ������ ��Ʈ ������
        if (characterStatus.weaponCombo == 5 && !GameManager.instance.isPause)
        {
            if (!GameManager.instance.isEnd)
            {
                if (hitTime % 6 == 0)
                {
                    if(characterStatus.hiddenCombo == 15)
                        rangeCol = Physics2D.OverlapCircleAll(gameObject.transform.position, 5.0f);
                    else
                        rangeCol = Physics2D.OverlapCircleAll(gameObject.transform.position, 4.0f);

                    for (int i = 0; i < rangeCol.Length; i++)
                    {
                        critRand = Random.Range(0f, 100f); //ũ��Ƽ��
                        if (rangeCol[i].CompareTag("Enemy")) //���̶�� 
                        {
                            if (critRand <= crit)
                            {
                                if (characterStatus.hiddenCombo == 15) //���� ��
                                {
                                    rangeCol[i].GetComponent<Enemy>().EnemyHitted(atk * 2 / 8f, characterStatus.equipCombo);
                                    DamageText(rangeCol[i].gameObject, atk * 2 / 8f);
                                    if (rangeCol[i].GetComponent<Enemy>().debuff != 1) //�� ���� �����
                                        rangeCol[i].GetComponent<Enemy>().debuff = 2;
                                }
                                else
                                {
                                    rangeCol[i].GetComponent<Enemy>().EnemyHitted(atk * 2 / 18f, characterStatus.equipCombo);
                                    DamageText(rangeCol[i].gameObject, atk * 2 / 18f);
                                }
                            }
                            else
                            {
                                if (characterStatus.hiddenCombo == 15) //���� ��
                                {
                                    rangeCol[i].GetComponent<Enemy>().EnemyHitted(atk / 8f, characterStatus.equipCombo);
                                    DamageText(rangeCol[i].gameObject, atk / 8f);
                                    if (rangeCol[i].GetComponent<Enemy>().debuff != 1) //�� ���� �����
                                        rangeCol[i].GetComponent<Enemy>().debuff = 2;
                                }
                                else
                                {
                                    rangeCol[i].GetComponent<Enemy>().EnemyHitted(atk / 18f, characterStatus.equipCombo);
                                    DamageText(rangeCol[i].gameObject, atk / 18f);
                                }
                            }
                        }

                    }
                }

                hitTime++;
                if (hitTime == 7)
                    hitTime = 1;
            }
        }

        //�˶Ǵ� ���Ҷ�� ���ݼӵ� ����
        if (weaponType == 1 || weaponType == 0)
        {
            gameObject.GetComponent<Animator>().SetFloat("AtkSpd", 1f / atkSpd); //���ݼӵ��� -�Ǵ� �����̱� ������ �ִϸ��̼� �ӵ��� ������ŭ ���
        }
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
        critRand = Random.Range(0f, 100f); //ũ��Ƽ��
        if (weaponType == 0) //Sword
        {
            if (collision.CompareTag("Enemy"))
            {
                if (collision.GetComponent<Enemy>().swordTime >= atkSpd) //���̾Ƹ�彺�� ����������
                {
                    if (critRand <= crit)
                    {
                        collision.GetComponent<Enemy>().EnemyHitted(atk * 3f, characterStatus.equipCombo);
                        DamageText(collision.gameObject, atk * 3f);
                    }
                    else
                    {
                        collision.GetComponent<Enemy>().EnemyHitted(atk* 1.5f, characterStatus.equipCombo);
                        DamageText(collision.gameObject, atk * 1.5f);
                    }

                    collision.GetComponent<Enemy>().swordTime = 0f;
                }
            }
        }
        else if (weaponType == 1) //element
        {
            if (collision.CompareTag("Enemy"))
            {
                if (critRand <= crit)
                {
                    collision.GetComponent<Enemy>().EnemyHitted(atk * 2 / 8, characterStatus.equipCombo);
                    DamageText(collision.gameObject, atk * 2 / 8);
                }
                else
                {
                    collision.GetComponent<Enemy>().EnemyHitted(atk / 8, characterStatus.equipCombo);
                    DamageText(collision.gameObject, atk / 8);
                }

            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        critRand = Random.Range(0f, 100f); //ũ��Ƽ��
        if (weaponType == 3) //��������
        {
            if (collision.CompareTag("Enemy"))
            {
                if (!GameManager.instance.isEnd && !GameManager.instance.isPause)
                {
                    if (hitTime % 6 == 0)
                    {
                        if (critRand <= crit)
                        {
                            collision.GetComponent<Enemy>().EnemyHitted(atk * 2 / 10, characterStatus.equipCombo);
                            DamageText(collision.gameObject, atk * 2 / 10);
                        }
                        else
                        {
                            collision.GetComponent<Enemy>().EnemyHitted(atk / 10, characterStatus.equipCombo);
                            DamageText(collision.gameObject, atk / 10);
                        }

                    }
                    hitTime++;
                    if (hitTime == 7)
                        hitTime = 1;
                }

            }

        }
    }
}
