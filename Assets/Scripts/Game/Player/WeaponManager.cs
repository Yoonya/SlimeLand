using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//투사체인 0,1,2는 bullet에서 관리하고 3,4,5는 여기서 관리
public class WeaponManager : MonoBehaviour
{
    //0 = 검, 1 = 원소, 2 = 불
    public bool isClone = false; //클론 무기인가
    public bool isSide = false; //사이드 무기인가
    public bool isSideEnforceL = false; //사이드 무기가 같은 장판으로서 강화되었는데
    public bool isSideEnforceR = false; //사이드 무기가 같은 장판으로서 강화되었는데
    public int weaponType = -1; //0검 1원소 2불 3마법지뢰
    public float atk = 0f;
    public float atkSpd = 0f;
    public float time = 0f;
    public float crit = 0f;
    public float critRand = 0f;

    public int hitTime = 0; //1프레임당 불 데미지와 마인 이펙트는 히트이펙트가 너무 과부화되서 10프레임으로

    private Collider2D[] rangeCol; //불 공격의 범위

    //bullet의 atk는 characterWeapon에서 지정해주지만 여기 weapon들은 한번 공격하고 사라지는게 아니기 때문에 atk가 업데이트 되어야 한다.
    private CharacterStatus characterStatus;

    // Start is called before the first frame update
    void Start()
    {
        characterStatus = FindObjectOfType<CharacterStatus>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isClone && isSide)  //클론의 사이드 무기 데미지
            atk = characterStatus.atk * 0.1f;
        else if (isSide) //사이드 무기 데미지
            atk = characterStatus.atk * 0.5f;
        else if (isClone)//클론 데미지
            atk = characterStatus.atk * 0.2f;
        else
            atk = characterStatus.atk;

        //강화상태 유무
        if ((characterStatus.weaponL >= 3 && characterStatus.weaponL <= 5) && (characterStatus.sideWeaponL >= 3 && characterStatus.sideWeaponL <= 5))
            isSideEnforceL = true;
        else
            isSideEnforceL = false;

        if ((characterStatus.weaponR >= 3 && characterStatus.weaponR <= 5) && (characterStatus.sideWeaponR >= 3 && characterStatus.sideWeaponR <= 5))
            isSideEnforceR = true;
        else
            isSideEnforceR = false;

        if (characterStatus.hiddenCombo != 13 && characterStatus.hiddenCombo != 14 && characterStatus.hiddenCombo != 15) //장판형 히든이 아니라면
        {
            if (isSideEnforceL && isSideEnforceR) //강화상태라면 강화
                atk += atk * 1f;
            else if (isSideEnforceL)
                atk += atk * 0.5f;
            else if (isSideEnforceR)
                atk += atk * 0.5f;
        }



        atkSpd = characterStatus.atkSpd;
        crit = characterStatus.crit;

        time += Time.deltaTime;

        if (time >= atkSpd) //불공격이지만 fireman제외
        {
            if (!GameManager.instance.isEnd)
            {
                if (weaponType == 2 && characterStatus.weaponCombo != 5 && !GameManager.instance.isPause)
                {
                    rangeCol = Physics2D.OverlapCircleAll(gameObject.transform.position, 4.0f); //범위 안에 있으면 공격
                    for (int i = 0; i < rangeCol.Length; i++)
                    {
                        critRand = Random.Range(0f, 100f); //크리티컬
                        if (rangeCol[i].CompareTag("Enemy")) //적이라면
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

        //Fireman은 적에게 도트 데미지
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
                        critRand = Random.Range(0f, 100f); //크리티컬
                        if (rangeCol[i].CompareTag("Enemy")) //적이라면 
                        {
                            if (critRand <= crit)
                            {
                                if (characterStatus.hiddenCombo == 15) //히든 불
                                {
                                    rangeCol[i].GetComponent<Enemy>().EnemyHitted(atk * 2 / 8f, characterStatus.equipCombo);
                                    DamageText(rangeCol[i].gameObject, atk * 2 / 8f);
                                    if (rangeCol[i].GetComponent<Enemy>().debuff != 1) //더 높은 디버프
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
                                if (characterStatus.hiddenCombo == 15) //히든 불
                                {
                                    rangeCol[i].GetComponent<Enemy>().EnemyHitted(atk / 8f, characterStatus.equipCombo);
                                    DamageText(rangeCol[i].gameObject, atk / 8f);
                                    if (rangeCol[i].GetComponent<Enemy>().debuff != 1) //더 높은 디버프
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

        //검또는 원소라면 공격속도 적용
        if (weaponType == 1 || weaponType == 0)
        {
            gameObject.GetComponent<Animator>().SetFloat("AtkSpd", 1f / atkSpd); //공격속도가 -되는 형식이기 때문에 애니메이션 속도는 비율만큼 계산
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
        critRand = Random.Range(0f, 100f); //크리티컬
        if (weaponType == 0) //Sword
        {
            if (collision.CompareTag("Enemy"))
            {
                if (collision.GetComponent<Enemy>().swordTime >= atkSpd) //다이아몬드스텝 데미지방지
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
        critRand = Random.Range(0f, 100f); //크리티컬
        if (weaponType == 3) //마법지뢰
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
