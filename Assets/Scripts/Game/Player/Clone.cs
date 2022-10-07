using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Clone : MonoBehaviour
{
    public float atk;
    public float atkSpd;
    public float crit;
    public int weaponTypeL = -1; //-1은 무기 없음
    public int weaponTypeR = -1;
    public int sideWeaponTypeL = -1;
    public int sideWeaponTypeR = -1;

    private bool isSWC = false; //검효과
    private bool isEWC = false; //원소효과
    private bool isFWC = false; //불효과

    public float time;

    //검베기공격 5초마다
    public GameObject slashEffect;
    public float slashTime = 0f;
    private Collider2D[] slashRangeCol; //폭발 공격의 범위

    //마법지뢰 공격 3초마다
    public GameObject magicMine;
    public float mmTime = 0f;

    public Transform characterLT; //캐릭터의 위치
    public Transform characterDirect; //캐릭터의 방향
    public Transform weaponLTL; //왼쪽 무기, 45도 로테이션 처리, 검 무기의 부모
    public Transform weaponLTR; //오른쪽 무기
    public Transform sideWeaponLTL; //왼쪽 사이드 무기
    public Transform sideWeaponLTR; //오른쪽 사이드 무기
    public Transform floorWeapon1; //장판무기의 부모
    public Transform floorWeapon2; //장판무기의 부모
    public Transform sideFloorWeapon1; //장판무기의 부모
    public Transform sideFloorWeapon2; //장판무기의 부모

    public CharacterStatus characterStatus;
    public CharacterWeapon characterWeapon;

    private void OnEnable()
    {
        time = characterWeapon.time; //동기화
        slashTime = characterWeapon.slashTime;
        mmTime = characterWeapon.mmTime;

        magicMine = characterWeapon.magicMine;
    }

    // Update is called once per frame
    void Update()
    {
        atk = characterStatus.atk * 0.2f;
        atkSpd = characterStatus.atkSpd;
        crit = characterStatus.crit;
        weaponTypeL = characterWeapon.weaponTypeL;
        weaponTypeR = characterWeapon.weaponTypeR;
        sideWeaponTypeL = characterWeapon.sideWeaponTypeL;
        sideWeaponTypeR = characterWeapon.sideWeaponTypeR;

        characterDirect.localEulerAngles = new Vector3(characterWeapon.characterDirect.localEulerAngles.x, characterWeapon.characterDirect.localEulerAngles.y, 
            characterWeapon.characterDirect.localEulerAngles.z);

        time += Time.deltaTime;

        //floorweapon이 rotation의 영향을 받지 않기 위해 자식에서 제외하고 따라오도록 수정
        floorWeapon1.transform.position = gameObject.transform.position;
        floorWeapon2.transform.position = gameObject.transform.position;
        sideFloorWeapon1.transform.position = gameObject.transform.position;
        sideFloorWeapon2.transform.position = gameObject.transform.position;

        if (!GameManager.instance.isEnd) //끝나지 않았고
        {
            //발사형무기
            if (time >= atkSpd)
            {
                if (weaponTypeL >= 0 && weaponTypeL <= 2) //발사형 무기라면
                {
                    //검이 있다면 지워주고
                    if (weaponLTL.childCount != 0)
                        ClearWeapon(0);

                    //장판이 있다면 지워주고
                    if (floorWeapon1.childCount != 0)
                        ClearWeapon(2);

                    CreateBullet(weaponTypeL, 0);
                }


                if (weaponTypeR >= 0 && weaponTypeR <= 2) //발사형 무기라면
                {
                    //검이 있다면 지워주고
                    if (weaponLTR.childCount != 0)
                        ClearWeapon(1);

                    //장판이 있다면 지워주고
                    if (floorWeapon2.childCount != 0)
                        ClearWeapon(3);

                    CreateBullet(weaponTypeR, 1);
                }

                if (sideWeaponTypeL >= 0 && sideWeaponTypeL <= 2) //사이드 발사형 무기라면
                {
                    //검이 있다면 지워주고
                    if (sideWeaponLTL.childCount != 0)
                        ClearSideWeapon(0);

                    //장판이 있다면 지워주고
                    if (sideFloorWeapon1.childCount != 0)
                        ClearSideWeapon(2);
                    CreateSideBullet(sideWeaponTypeL, 0);
                }
                if (sideWeaponTypeR >= 0 && sideWeaponTypeR <= 2) //사이드 발사형 무기라면
                {
                    //검이 있다면 지워주고
                    if (sideWeaponLTR.childCount != 0)
                        ClearSideWeapon(1);

                    //장판이 있다면 지워주고
                    if (sideFloorWeapon2.childCount != 0)
                        ClearSideWeapon(3);
                    CreateSideBullet(sideWeaponTypeR, 1);
                }

                //기본 탄환 
                CreateBasicBullet();
            }

            //무기
            if (!(weaponTypeL == -1 && weaponTypeR == -1)) //무기가 없는 상태가 아니라면
            {
                if (weaponTypeL == weaponTypeR) //같은 무기를 착용한 상태라면 무기조합을 검사한다.
                {
                    EquipWeaponCombo();

                    //원소 히든
                    if (floorWeapon1.childCount != 0 && isEWC)
                        if (characterStatus.hiddenCombo != 14 && floorWeapon1.GetChild(0).name == "BulletElementPP(Clone)")
                            ClearWeaoponCombo();

                    //불 히든
                    if (floorWeapon1.childCount != 0 && isFWC)
                        if (characterStatus.hiddenCombo != 15 && floorWeapon1.GetChild(0).name == "BulletFirePP(Clone)")
                            ClearWeaoponCombo();
                }
                else
                {
                    //콤보 효과가 취소 되었을 경우
                    if (isSWC || isEWC || isFWC)
                        ClearWeaoponCombo();

                    //원소 히든
                    if (isEWC && !(weaponTypeL == 4 && sideWeaponTypeL == 4 && sideWeaponTypeR == 4))
                        ClearWeaoponCombo();

                    //불 히든
                    if (isFWC && !(weaponTypeL == 5 && sideWeaponTypeL == 5 && sideWeaponTypeR == 5))
                        ClearWeaoponCombo();

                }

                if (weaponTypeL != -1) //왼쪽 무기가 있다면
                {
                    if (weaponTypeL == 3 && weaponLTL.childCount == 0) //검 무기라면, 만들어져있지 않다면
                    {
                        //장판이 있다면 지워주고
                        if (floorWeapon1.childCount != 0)
                            ClearWeapon(2);

                        CreateSword(0); //검무기 왼쪽
                    }

                    if (weaponTypeL >= 4 && weaponTypeL <= 5 && floorWeapon1.childCount == 0) //바닥 무기라면, 만들어져있지 않다면
                    {
                        //검이 있다면 지워주고
                        if (weaponLTL.childCount != 0)
                            ClearWeapon(0);

                        CreateFloorBullet(weaponTypeL - 4, 0);
                    }
                    else if (weaponTypeL >= 4 && weaponTypeL <= 5)
                    {
                        if (!isEWC && !isFWC)
                        {
                            //이미 만들어져 있는 상태라면, 근데 무기와 다르다면
                            if (weaponTypeL == 4 && (floorWeapon1.GetChild(0).name != "BulletElement(Clone)" && floorWeapon1.GetChild(0).name != "BulletElementPP(Clone)"))
                            {
                                ClearWeapon(2);
                                CreateFloorBullet(weaponTypeL - 4, 0);
                            }
                            //이미 만들어져 있는 상태라면, 근데 무기와 다르다면
                            if (weaponTypeL == 5 && (floorWeapon1.GetChild(0).name != "BulletFire(Clone)" && floorWeapon1.GetChild(0).name != "BulletFirePP(Clone)"))
                            {
                                ClearWeapon(2);
                                CreateFloorBullet(weaponTypeL - 4, 0);
                            }
                        }
                    }
                }
                if (weaponTypeR != -1) //오른쪽 무기가 있다면
                {

                    if (weaponTypeR == 3 && weaponLTR.childCount == 0) //검 무기라면
                    {
                        //장판이 있다면 지워주고
                        if (floorWeapon2.childCount != 0)
                            ClearWeapon(3);

                        CreateSword(1); //검무기 오른쪽
                    }

                    if (weaponTypeR >= 4 && weaponTypeR <= 5 && floorWeapon2.childCount == 0) //바닥 무기라면, 만들어져있지 않다면
                    {
                        //장판 콤보효과는 왼쪽에 생겨나므로 효과 상태일 때 여기를 막아줘야 한다.
                        if (!isEWC && !isFWC)
                        {
                            //검이 있다면 지워주고
                            if (weaponLTR.childCount != 0)
                                ClearWeapon(1);

                            CreateFloorBullet(weaponTypeR - 4, 1);
                        }
                    }
                    else if (weaponTypeR >= 4 && weaponTypeR <= 5)
                    {
                        if (!isEWC && !isFWC)
                        {
                            //이미 만들어져 있는 상태라면, 근데 무기와 다르다면
                            if (weaponTypeR == 4 && (floorWeapon2.GetChild(0).name != "BulletElement(Clone)") && floorWeapon2.GetChild(0).name != "BulletElementPP(Clone)")
                            {
                                ClearWeapon(3);
                                CreateFloorBullet(weaponTypeR - 4, 0);
                            }
                            //이미 만들어져 있는 상태라면, 근데 무기와 다르다면
                            if (weaponTypeR == 5 && (floorWeapon2.GetChild(0).name != "BulletFire(Clone)" && floorWeapon2.GetChild(0).name != "BulletFirePP(Clone)"))
                            {
                                ClearWeapon(3);
                                CreateFloorBullet(weaponTypeR - 4, 0);
                            }
                        }
                    }
                }
            }

            //사이드 무기
            if (!(sideWeaponTypeL == -1 && sideWeaponTypeR == -1))
            {
                if (sideWeaponTypeL != -1) //왼쪽 사이드 무기가 있다면
                {
                    if (sideWeaponTypeL == 3 && weaponTypeL >= -1 && weaponTypeL <= 2 && sideWeaponLTL.childCount == 0) //발사형탄환일때 검을 생성
                    {
                        //장판이 있다면 지워주고
                        if (sideFloorWeapon1.childCount != 0)
                            ClearSideWeapon(2);

                        CreateSideSword(0); //검무기 왼쪽
                    }
                    else if (sideWeaponTypeL == 3 && weaponTypeL > 2 && sideWeaponLTL.childCount != 0) //아니면 삭제
                        ClearSideWeapon(0);

                    if ((sideWeaponTypeL == 4 || sideWeaponTypeL == 5) && weaponTypeL >= -1 && weaponTypeL <= 2) //발사형탄환일때 장판을 생성
                    {
                        //검이 있다면 지워주고
                        if (sideWeaponLTL.childCount != 0)
                            ClearSideWeapon(0);

                        if (sideFloorWeapon1.childCount != 0)
                        {
                            //이미 만들어져 있는 상태라면, 근데 무기와 다르다면
                            if (sideWeaponTypeL == 4 && sideFloorWeapon1.GetChild(0).name != "BulletElement(Clone)")
                            {
                                ClearSideWeapon(2);
                                CreateSideFloorBullet(sideWeaponTypeL - 4, 0);
                            }
                            //이미 만들어져 있는 상태라면, 근데 무기와 다르다면
                            if (sideWeaponTypeL == 5 && sideFloorWeapon1.GetChild(0).name != "BulletFire(Clone)")
                            {
                                ClearSideWeapon(2);
                                CreateSideFloorBullet(sideWeaponTypeL - 4, 0);
                            }
                        }
                        else
                            CreateSideFloorBullet(sideWeaponTypeL - 4, 0); //장판 왼쪽
                    }
                    else if ((sideWeaponTypeL == 4 || sideWeaponTypeL == 5) && weaponTypeL > 2 && sideFloorWeapon1.childCount != 0) //아니면 삭제
                        ClearSideWeapon(2);


                }
                if (sideWeaponTypeR != -1) //오른쪽 무기가 있다면
                {
                    if (sideWeaponTypeR == 3 && weaponTypeR >= -1 && weaponTypeR <= 2 && sideWeaponLTR.childCount == 0) //발사형탄환일때 검을 생성
                    {
                        //장판이 있다면 지워주고
                        if (sideFloorWeapon2.childCount != 0)
                            ClearSideWeapon(3);

                        CreateSideSword(1); //검무기 오른쪽
                    }
                    else if (sideWeaponTypeR == 3 && weaponTypeR > 2 && sideWeaponLTR.childCount != 0) //아니면 삭제
                        ClearSideWeapon(1);

                    if ((sideWeaponTypeR == 4 || sideWeaponTypeR == 5) && weaponTypeR >= -1 && weaponTypeR <= 2) //발사형탄환일때 장판을 생성
                    {
                        //검이 있다면 지워주고
                        if (sideWeaponLTR.childCount != 0)
                            ClearSideWeapon(1);

                        if (sideFloorWeapon2.childCount != 0)
                        {
                            //이미 만들어져 있는 상태라면, 근데 무기와 다르다면
                            if (sideWeaponTypeR == 4 && sideFloorWeapon2.GetChild(0).name != "BulletElement(Clone)")
                            {
                                ClearSideWeapon(3);
                                CreateSideFloorBullet(sideWeaponTypeR - 4, 0);
                            }
                            //이미 만들어져 있는 상태라면, 근데 무기와 다르다면
                            if (sideWeaponTypeR == 5 && sideFloorWeapon2.GetChild(0).name != "BulletFire(Clone)")
                            {
                                ClearSideWeapon(3);
                                CreateSideFloorBullet(sideWeaponTypeR - 4, 0);
                            }
                        }
                        else
                            CreateSideFloorBullet(sideWeaponTypeR - 4, 1); //장판 왼쪽
                    }
                    else if ((sideWeaponTypeR == 4 || sideWeaponTypeR == 5) && weaponTypeR > 2 && sideFloorWeapon2.childCount != 0) //아니면 삭제
                        ClearSideWeapon(3);
                }
            }

            if (time >= atkSpd) //time초기화
                time = 0f;
        }

        if (characterStatus.hiddenCombo == 10) //히든 블레이더
            slashTime += Time.deltaTime;

        if (slashTime >= 5.0f)
        {
            if (!GameManager.instance.isEnd)
            {
                if (!GameManager.instance.isPause)
                {
                    slashRangeCol = Physics2D.OverlapCircleAll(characterLT.transform.position, 3.0f); //범위 안에 있으면 공격  
                    slashEffect.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f);
                    slashEffect.SetActive(true);

                    AudioManager.instance.PlaySFX("Slash");

                    List<Collider2D> slashAttack = new List<Collider2D>(); //실제로 공격할적
                    for (int i = 0; i < slashRangeCol.Length; i++)
                    {
                        if (slashRangeCol[i].CompareTag("Enemy")) //적이라면, 그리고 상단에 있는 적만
                        {
                           slashAttack.Add(slashRangeCol[i]);
                        }
                    }
                    StartCoroutine(SlashAttack(slashAttack)); //1초뒤에 공격
                }
            }
            slashTime = 0f;
        }

        if (characterStatus.hiddenCombo == 11) //히든 마법사
            mmTime += Time.deltaTime;

        if (mmTime >= 3.0f)
        {
            if (!GameManager.instance.isEnd)
            {
                if (!GameManager.instance.isPause)
                {

                    GameObject mm = Instantiate(magicMine, transform.position, Quaternion.identity); //객체생성

                    mm.GetComponent<WeaponManager>().weaponType = 3;
                    mm.GetComponent<WeaponManager>().isClone = true; //클론설정
                    mm.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f);

                    AudioManager.instance.PlaySFX("MagicMine");
                }
            }
            mmTime = 0f;
        }
    }

    IEnumerator SlashAttack(List<Collider2D> slashAttack)
    {
        yield return new WaitForSeconds(1.0f);

        float critRand = Random.Range(0f, 100f); //크리티컬
        for (int i = 0; i < slashAttack.Count; i++)
        {
            if (slashAttack[i].gameObject.activeSelf == true) //살아있는 적만
            {
                if (critRand <= characterStatus.crit)
                {
                    slashAttack[i].GetComponent<Enemy>().EnemyHitted(characterStatus.atk * 5 * 2 * 0.2f, characterStatus.equipCombo);
                    DamageText(slashAttack[i].gameObject, characterStatus.atk * 5 * 2 * 0.2f);
                }
                else
                {
                    slashAttack[i].GetComponent<Enemy>().EnemyHitted(characterStatus.atk * 5 * 0.2f, characterStatus.equipCombo);
                    DamageText(slashAttack[i].gameObject, characterStatus.atk * 5 * 0.2f);
                }
            }
        }

    }


    //캐릭터가 기본으로 발사하는 탄환
    private void CreateBasicBullet()
    {
        if (characterStatus.hiddenCombo != 13) //히든런처가 아니라면
        {
            GameObject tempBullet = null;
            if (characterStatus.weaponCombo == 2) //Gunner라면 특수 탄환으로
            {
                if (characterStatus.hiddenCombo == 12) //히든 거너라면 더 특수
                {
                    tempBullet = ObjectPool.instance.queue[10].Dequeue();
                    tempBullet.GetComponent<Bullet>().bulletType = 10;
                }
                else
                {
                    tempBullet = ObjectPool.instance.queue[3].Dequeue();
                    tempBullet.GetComponent<Bullet>().bulletType = 3;
                }
            }
            else
            {
                tempBullet = ObjectPool.instance.queue[0].Dequeue(); //오브젝트 풀링 사용
                tempBullet.GetComponent<Bullet>().bulletType = 0;
            }

            tempBullet.transform.localEulerAngles = characterDirect.localEulerAngles; //캐릭터가 바라보는 방향으로 발사 조정
            tempBullet.transform.position = new Vector2(characterLT.position.x + 0.6f, characterLT.position.y + 0.5f);//시각적으로 캐릭터보다 앞에서 발사
            tempBullet.GetComponent<Bullet>().speed = characterStatus.spd * 2f;
            tempBullet.GetComponent<Bullet>().atk = atk;
            tempBullet.GetComponent<Bullet>().crit = crit;
            tempBullet.GetComponent<Bullet>().istriggered = false;
            tempBullet.GetComponent<Bullet>().weaponCombo = characterStatus.weaponCombo;
            tempBullet.GetComponent<Bullet>().equipCombo = -1;  //분신은 장비효과 적용x
            tempBullet.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f, 0.5f); //분신 탄환색
            tempBullet.SetActive(true);
        }
        else
        {
            GameObject tempBullet = null;
            tempBullet = ObjectPool.instance.queue[11].Dequeue(); //오브젝트 풀링 사용
            tempBullet.GetComponent<Bullet>().bulletType = 11;
            tempBullet.transform.localEulerAngles = characterDirect.localEulerAngles; //캐릭터가 바라보는 방향으로 발사 조정
            tempBullet.transform.position = new Vector2(characterLT.position.x + 0.6f, characterLT.position.y + 0.5f);//시각적으로 캐릭터보다 앞에서 발사
            tempBullet.GetComponent<Bullet>().speed = characterStatus.spd * 2f;
            tempBullet.GetComponent<Bullet>().atk = atk;
            tempBullet.GetComponent<Bullet>().crit = crit;
            tempBullet.GetComponent<Bullet>().weaponCombo = characterStatus.weaponCombo;
            tempBullet.GetComponent<Bullet>().equipCombo = characterStatus.equipCombo;
            tempBullet.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f, 0.5f); //분신 탄환색
            tempBullet.SetActive(true);
        }
    }

    //발사형 무기(0, 1, 2)
    public void CreateBullet(int weaponType, int dir) //무기 타입과 왼쪽인지 오른쪽인지 방향을 받아온다.
    {
        if (characterStatus.hiddenCombo != 13) //히든런처가 아니라면
        {
            GameObject tempBullet = null;
            if (characterStatus.weaponCombo == 1) //Magicion 라면 특수 탄환으로
            {
                tempBullet = ObjectPool.instance.queue[4].Dequeue();
                tempBullet.GetComponent<Bullet>().bulletType = 4;
               
            }
            else if (characterStatus.weaponCombo == 2) //Gunner라면 특수 탄환으로
            {
                if (characterStatus.hiddenCombo == 12) //히든 거너라면 더 특수
                {
                    tempBullet = ObjectPool.instance.queue[10].Dequeue();
                    tempBullet.GetComponent<Bullet>().bulletType = 10;
                }
                else
                {
                    tempBullet = ObjectPool.instance.queue[3].Dequeue();
                    tempBullet.GetComponent<Bullet>().bulletType = 3;                 
                }
            }
            else if (characterStatus.weaponCombo == 3) //Launcher라면 특수 탄환으로
            {
                tempBullet = ObjectPool.instance.queue[5].Dequeue(); //오브젝트 풀링 사용
                tempBullet.GetComponent<Bullet>().bulletType = 5;
            }
            else
            {
                tempBullet = ObjectPool.instance.queue[weaponType].Dequeue(); //오브젝트 풀링 사용
                tempBullet.GetComponent<Bullet>().bulletType = weaponType;
            }


            if (dir == 0) //방향에 따른 위치 지정
            {
                tempBullet.transform.localEulerAngles = characterDirect.localEulerAngles + weaponLTL.localEulerAngles; //캐릭터가 바라보는 방향으로 발사 조정
                tempBullet.transform.position = weaponLTL.position + transform.up;
            }
            else if (dir == 1)
            {
                tempBullet.transform.localEulerAngles = characterDirect.localEulerAngles + weaponLTR.localEulerAngles; //캐릭터가 바라보는 방향으로 발사 조정
                tempBullet.transform.position = weaponLTR.position + transform.up;
            }

            tempBullet.GetComponent<Bullet>().speed = characterStatus.spd * 2f;
            tempBullet.GetComponent<Bullet>().atk = atk;
            tempBullet.GetComponent<Bullet>().crit = crit;
            tempBullet.GetComponent<Bullet>().istriggered = false;
            tempBullet.GetComponent<Bullet>().weaponCombo = characterStatus.weaponCombo;
            tempBullet.GetComponent<Bullet>().equipCombo = -1; //분신은 장비효과 적용x
            tempBullet.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f, 0.5f);//분신탄환색
            tempBullet.SetActive(true);
        }
    }


    //캐릭터 사이드로 발사하는 탄환
    private void CreateSideBullet(int weaponType, int dir)
    {
        if (characterStatus.hiddenCombo != 13) //히든런처가 아니라면
        {
            GameObject tempBullet = null;

            if (characterStatus.hiddenCombo == 12) //히든 Gunner라면 특수 탄환으로
            {
                tempBullet = ObjectPool.instance.queue[10].Dequeue();
                tempBullet.GetComponent<Bullet>().bulletType = 10;
            }
            else
            {
                tempBullet = ObjectPool.instance.queue[weaponType].Dequeue(); //오브젝트 풀링 사용
                tempBullet.GetComponent<Bullet>().bulletType = weaponType;
            }
            if (dir == 0)
            {
                tempBullet.transform.position = sideWeaponLTL.transform.position;
            }
            else if (dir == 1)
            {
                tempBullet.transform.position = sideWeaponLTR.transform.position;
            }

            tempBullet.transform.localEulerAngles = characterDirect.localEulerAngles; //캐릭터가 바라보는 방향으로 발사 조정
            tempBullet.GetComponent<Bullet>().speed = characterStatus.spd * 2f;
            tempBullet.GetComponent<Bullet>().crit = characterStatus.crit;
            tempBullet.GetComponent<Bullet>().istriggered = false;
            tempBullet.GetComponent<Bullet>().weaponCombo = characterStatus.weaponCombo;
            tempBullet.GetComponent<Bullet>().equipCombo = -1;
            tempBullet.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            tempBullet.SetActive(true);

            if (characterStatus.hiddenCombo == 12) //히든 Gunner라면 특수 탄환으로
                tempBullet.GetComponent<Bullet>().atk = characterStatus.atk;
            else
                tempBullet.GetComponent<Bullet>().atk = characterStatus.atk / 2;
        }
    }

    //검 무기(3), type 0 = L, 1 = R, 2 = Combo
    public void CreateSword(int type)
    {
        if (type == 2) //콤보효과라면
        {
            GameObject sword1 = Instantiate(characterWeapon.weapons[type + 2], transform.position, Quaternion.identity); //객체생성
            GameObject sword2 = Instantiate(characterWeapon.weapons[type + 3], transform.position, Quaternion.identity); //객체생성

            sword1.transform.localScale = new Vector3(15f, 15f, 1f); //검의 크기를 2배로
            sword2.transform.localScale = new Vector3(15f, 15f, 1f); //검의 크기를 2배로

            sword1.transform.SetParent(weaponLTL, false); //부모 객체 설정
            sword2.transform.SetParent(weaponLTR, false); //부모 객체 설정

            sword1.GetComponent<WeaponManager>().weaponType = 0; //type이 뭐가 오든 0
            sword1.GetComponent<WeaponManager>().isClone = true; //클론설정
            sword1.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f);

            sword2.GetComponent<WeaponManager>().weaponType = 0; //type이 뭐가 오든 0
            sword2.GetComponent<WeaponManager>().isClone = true; //클론설정
            sword2.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f);

            return; //끝
        }

        GameObject sword = Instantiate(characterWeapon.weapons[type + 4], transform.position, Quaternion.identity); //객체생성

        sword.GetComponent<WeaponManager>().weaponType = 0; //type이 뭐가 오든 0
        sword.GetComponent<WeaponManager>().isClone = true; //클론설정
        sword.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f);

        //부모객체설정
        if (type == 0)
            sword.transform.SetParent(weaponLTL, false);
        else if (type == 1)
            sword.transform.SetParent(weaponLTR, false);
    }

    public void CreateSideSword(int type)
    {
        GameObject sword = Instantiate(characterWeapon.weapons[type + 4], transform.position, Quaternion.identity); //객체생성

        sword.GetComponent<WeaponManager>().weaponType = 0; //type이 뭐가 오든 0
        sword.GetComponent<WeaponManager>().isSide = true;
        sword.GetComponent<WeaponManager>().isClone = true; //클론설정
        sword.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f);

        //부모객체설정
        if (type == 0)
            sword.transform.SetParent(sideWeaponLTL, false);
        else if (type == 1)
            sword.transform.SetParent(sideWeaponLTR, false);
    }

    //바닥형 무기(4, 5) type 0 = element, 1 = fire, 2 = elements, 3 = fires , 6 = elementPP, 7 = firePP lot  0 = L, 1 = R
    private void CreateFloorBullet(int type, int lot)
    {
        GameObject floor = Instantiate(characterWeapon.weapons[type]); //객체생성

        if (type == 0 || type == 2 || type == 6)
            floor.GetComponent<WeaponManager>().weaponType = 1;
        else if (type == 1 || type == 3 || type == 7)
            floor.GetComponent<WeaponManager>().weaponType = 2;

        floor.GetComponent<WeaponManager>().isClone = true; //클론설정
        if(type != 7 && type != 6)
            floor.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f);
        if (type == 6)
        {
            floor.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f);
            floor.transform.GetChild(1).GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f);
        }

        if (lot == 0)
            floor.transform.SetParent(floorWeapon1, false);
        else if (lot == 1)
            floor.transform.SetParent(floorWeapon2, false);

    }

    //바닥형 무기(4, 5) type 0 = element, 1 = fire, 2 = elements, 3 = fires , lot  0 = L, 1 = R
    private void CreateSideFloorBullet(int type, int lot)
    {
        GameObject floor = Instantiate(characterWeapon.weapons[type]); //객체생성

        if (type == 0 || type == 2)
            floor.GetComponent<WeaponManager>().weaponType = 1;
        else if (type == 1 || type == 3)
            floor.GetComponent<WeaponManager>().weaponType = 2;

        floor.GetComponent<WeaponManager>().isClone = true; //클론설정
        floor.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f);
        floor.GetComponent<WeaponManager>().isSide = true;

        if (lot == 0)
            floor.transform.SetParent(sideFloorWeapon1, false);
        else if (lot == 1)
            floor.transform.SetParent(sideFloorWeapon2, false);

    }

    //조합을 적용
    private void EquipWeaponCombo()
    {
        //검 조합일때, 콤보 중이 아닐때
        if (weaponTypeL == 3 && !isSWC)
        {
            for (int i = 0; i < 4; i++)
                ClearWeapon(i);

            //무기를 해제하고 콤보효과 무기를 착용한다.
            CreateSword(2);
            isSWC = true;
        }

        //element조합일때, 장판 조합은 일단 1번에
        if (weaponTypeL == 4 && !isEWC)
        {
            for (int i = 0; i < 4; i++)
                ClearWeapon(i);

            if (characterStatus.hiddenCombo == 14) //히든 엘리멘트
                CreateFloorBullet(6, 0);
            else
                CreateFloorBullet(2, 0);
            isEWC = true;
        }

        //불 조합일때
        if (weaponTypeL == 5 && !isFWC)
        {
            for (int i = 0; i < 4; i++)
                ClearWeapon(i);

            if (characterStatus.hiddenCombo == 15) //히든 엘리멘트
                CreateFloorBullet(7, 0);
            else
                CreateFloorBullet(3, 0);
            isFWC = true;
        }
    }

    //무기를 해제시킨다. 0 = 왼쪽검, 1 = 오른쪽검, 2 = 장판1, 3 = 장판2
    private void ClearWeapon(int type)
    {
        Transform[] childs = null;

        switch (type)
        {
            case 0:
                childs = weaponLTL.GetComponentsInChildren<Transform>();//자식들을 전부 모아서 삭제
                break;
            case 1:
                childs = weaponLTR.GetComponentsInChildren<Transform>();//자식들을 전부 모아서 삭제
                break;
            case 2:
                childs = floorWeapon1.GetComponentsInChildren<Transform>();//자식들을 전부 모아서 삭제
                break;
            case 3:
                childs = floorWeapon2.GetComponentsInChildren<Transform>();//자식들을 전부 모아서 삭제
                break;
        }

        if (childs != null)
            for (int i = 1; i < childs.Length; i++)
            {
                if (childs[i] != transform)
                    Destroy(childs[i].gameObject);
            }
    }

    //무기를 해제시킨다. 0 = 왼쪽검, 1 = 오른쪽검, 2 = 장판1, 3 = 장판2
    private void ClearSideWeapon(int type)
    {
        Transform[] childs = null;

        switch (type)
        {
            case 0:
                childs = sideWeaponLTL.GetComponentsInChildren<Transform>();//자식들을 전부 모아서 삭제
                break;
            case 1:
                childs = sideWeaponLTR.GetComponentsInChildren<Transform>();//자식들을 전부 모아서 삭제
                break;
            case 2:
                childs = sideFloorWeapon1.GetComponentsInChildren<Transform>();//자식들을 전부 모아서 삭제
                break;
            case 3:
                childs = sideFloorWeapon2.GetComponentsInChildren<Transform>();//자식들을 전부 모아서 삭제
                break;
        }

        if (childs != null)
            for (int i = 1; i < childs.Length; i++)
            {
                if (childs[i] != transform)
                    Destroy(childs[i].gameObject);
            }
    }

    //무기 효과를 해제시킨다.
    private void ClearWeaoponCombo()
    {
        if (isSWC)
        {
            //지우고 다시 만들어주기
            ClearWeapon(0);
            ClearWeapon(1);

            if (weaponTypeL == 3) //검 무기라면
                CreateSword(0); //검무기 왼쪽

            if (weaponTypeR == 3) //검 무기라면
                CreateSword(1); //검무기 오른쪽

            isSWC = false;
        }

        if (isEWC)
        {
            //지우고 다시 만들어주기
            ClearWeapon(2);
            ClearWeapon(3);

            if (weaponTypeL == 4) //장판 무기라면             
                CreateFloorBullet(weaponTypeL - 4, 0);

            if (weaponTypeR == 4) //장판 무기라면
                CreateFloorBullet(weaponTypeR - 4, 1);

            isEWC = false;
        }

        if (isFWC)
        {
            //지우고 다시 만들어주기
            ClearWeapon(2);
            ClearWeapon(3);

            if (weaponTypeL == 5) //장판 무기라면
                CreateFloorBullet(weaponTypeL - 4, 0);

            if (weaponTypeR == 5) //장판 무기라면
                CreateFloorBullet(weaponTypeR - 4, 1);

            isFWC = false;
        }
    }

    public void CheckHiddenCombo()
    {
        //element조합일때
        if (characterStatus.hiddenCombo == 14)
        {
            for (int i = 0; i < 4; i++)
                ClearWeapon(i);

            CreateFloorBullet(6, 0);

            isEWC = true;
        }

        //element조합일때
        if (characterStatus.hiddenCombo == 15)
        {
            for (int i = 0; i < 4; i++)
                ClearWeapon(i);

            CreateFloorBullet(7, 0);

            isFWC = true;
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
}
