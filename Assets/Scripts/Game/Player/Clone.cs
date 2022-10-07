using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Clone : MonoBehaviour
{
    public float atk;
    public float atkSpd;
    public float crit;
    public int weaponTypeL = -1; //-1�� ���� ����
    public int weaponTypeR = -1;
    public int sideWeaponTypeL = -1;
    public int sideWeaponTypeR = -1;

    private bool isSWC = false; //��ȿ��
    private bool isEWC = false; //����ȿ��
    private bool isFWC = false; //��ȿ��

    public float time;

    //�˺������ 5�ʸ���
    public GameObject slashEffect;
    public float slashTime = 0f;
    private Collider2D[] slashRangeCol; //���� ������ ����

    //�������� ���� 3�ʸ���
    public GameObject magicMine;
    public float mmTime = 0f;

    public Transform characterLT; //ĳ������ ��ġ
    public Transform characterDirect; //ĳ������ ����
    public Transform weaponLTL; //���� ����, 45�� �����̼� ó��, �� ������ �θ�
    public Transform weaponLTR; //������ ����
    public Transform sideWeaponLTL; //���� ���̵� ����
    public Transform sideWeaponLTR; //������ ���̵� ����
    public Transform floorWeapon1; //���ǹ����� �θ�
    public Transform floorWeapon2; //���ǹ����� �θ�
    public Transform sideFloorWeapon1; //���ǹ����� �θ�
    public Transform sideFloorWeapon2; //���ǹ����� �θ�

    public CharacterStatus characterStatus;
    public CharacterWeapon characterWeapon;

    private void OnEnable()
    {
        time = characterWeapon.time; //����ȭ
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

        //floorweapon�� rotation�� ������ ���� �ʱ� ���� �ڽĿ��� �����ϰ� ��������� ����
        floorWeapon1.transform.position = gameObject.transform.position;
        floorWeapon2.transform.position = gameObject.transform.position;
        sideFloorWeapon1.transform.position = gameObject.transform.position;
        sideFloorWeapon2.transform.position = gameObject.transform.position;

        if (!GameManager.instance.isEnd) //������ �ʾҰ�
        {
            //�߻�������
            if (time >= atkSpd)
            {
                if (weaponTypeL >= 0 && weaponTypeL <= 2) //�߻��� ������
                {
                    //���� �ִٸ� �����ְ�
                    if (weaponLTL.childCount != 0)
                        ClearWeapon(0);

                    //������ �ִٸ� �����ְ�
                    if (floorWeapon1.childCount != 0)
                        ClearWeapon(2);

                    CreateBullet(weaponTypeL, 0);
                }


                if (weaponTypeR >= 0 && weaponTypeR <= 2) //�߻��� ������
                {
                    //���� �ִٸ� �����ְ�
                    if (weaponLTR.childCount != 0)
                        ClearWeapon(1);

                    //������ �ִٸ� �����ְ�
                    if (floorWeapon2.childCount != 0)
                        ClearWeapon(3);

                    CreateBullet(weaponTypeR, 1);
                }

                if (sideWeaponTypeL >= 0 && sideWeaponTypeL <= 2) //���̵� �߻��� ������
                {
                    //���� �ִٸ� �����ְ�
                    if (sideWeaponLTL.childCount != 0)
                        ClearSideWeapon(0);

                    //������ �ִٸ� �����ְ�
                    if (sideFloorWeapon1.childCount != 0)
                        ClearSideWeapon(2);
                    CreateSideBullet(sideWeaponTypeL, 0);
                }
                if (sideWeaponTypeR >= 0 && sideWeaponTypeR <= 2) //���̵� �߻��� ������
                {
                    //���� �ִٸ� �����ְ�
                    if (sideWeaponLTR.childCount != 0)
                        ClearSideWeapon(1);

                    //������ �ִٸ� �����ְ�
                    if (sideFloorWeapon2.childCount != 0)
                        ClearSideWeapon(3);
                    CreateSideBullet(sideWeaponTypeR, 1);
                }

                //�⺻ źȯ 
                CreateBasicBullet();
            }

            //����
            if (!(weaponTypeL == -1 && weaponTypeR == -1)) //���Ⱑ ���� ���°� �ƴ϶��
            {
                if (weaponTypeL == weaponTypeR) //���� ���⸦ ������ ���¶�� ���������� �˻��Ѵ�.
                {
                    EquipWeaponCombo();

                    //���� ����
                    if (floorWeapon1.childCount != 0 && isEWC)
                        if (characterStatus.hiddenCombo != 14 && floorWeapon1.GetChild(0).name == "BulletElementPP(Clone)")
                            ClearWeaoponCombo();

                    //�� ����
                    if (floorWeapon1.childCount != 0 && isFWC)
                        if (characterStatus.hiddenCombo != 15 && floorWeapon1.GetChild(0).name == "BulletFirePP(Clone)")
                            ClearWeaoponCombo();
                }
                else
                {
                    //�޺� ȿ���� ��� �Ǿ��� ���
                    if (isSWC || isEWC || isFWC)
                        ClearWeaoponCombo();

                    //���� ����
                    if (isEWC && !(weaponTypeL == 4 && sideWeaponTypeL == 4 && sideWeaponTypeR == 4))
                        ClearWeaoponCombo();

                    //�� ����
                    if (isFWC && !(weaponTypeL == 5 && sideWeaponTypeL == 5 && sideWeaponTypeR == 5))
                        ClearWeaoponCombo();

                }

                if (weaponTypeL != -1) //���� ���Ⱑ �ִٸ�
                {
                    if (weaponTypeL == 3 && weaponLTL.childCount == 0) //�� ������, ����������� �ʴٸ�
                    {
                        //������ �ִٸ� �����ְ�
                        if (floorWeapon1.childCount != 0)
                            ClearWeapon(2);

                        CreateSword(0); //�˹��� ����
                    }

                    if (weaponTypeL >= 4 && weaponTypeL <= 5 && floorWeapon1.childCount == 0) //�ٴ� ������, ����������� �ʴٸ�
                    {
                        //���� �ִٸ� �����ְ�
                        if (weaponLTL.childCount != 0)
                            ClearWeapon(0);

                        CreateFloorBullet(weaponTypeL - 4, 0);
                    }
                    else if (weaponTypeL >= 4 && weaponTypeL <= 5)
                    {
                        if (!isEWC && !isFWC)
                        {
                            //�̹� ������� �ִ� ���¶��, �ٵ� ����� �ٸ��ٸ�
                            if (weaponTypeL == 4 && (floorWeapon1.GetChild(0).name != "BulletElement(Clone)" && floorWeapon1.GetChild(0).name != "BulletElementPP(Clone)"))
                            {
                                ClearWeapon(2);
                                CreateFloorBullet(weaponTypeL - 4, 0);
                            }
                            //�̹� ������� �ִ� ���¶��, �ٵ� ����� �ٸ��ٸ�
                            if (weaponTypeL == 5 && (floorWeapon1.GetChild(0).name != "BulletFire(Clone)" && floorWeapon1.GetChild(0).name != "BulletFirePP(Clone)"))
                            {
                                ClearWeapon(2);
                                CreateFloorBullet(weaponTypeL - 4, 0);
                            }
                        }
                    }
                }
                if (weaponTypeR != -1) //������ ���Ⱑ �ִٸ�
                {

                    if (weaponTypeR == 3 && weaponLTR.childCount == 0) //�� ������
                    {
                        //������ �ִٸ� �����ְ�
                        if (floorWeapon2.childCount != 0)
                            ClearWeapon(3);

                        CreateSword(1); //�˹��� ������
                    }

                    if (weaponTypeR >= 4 && weaponTypeR <= 5 && floorWeapon2.childCount == 0) //�ٴ� ������, ����������� �ʴٸ�
                    {
                        //���� �޺�ȿ���� ���ʿ� ���ܳ��Ƿ� ȿ�� ������ �� ���⸦ ������� �Ѵ�.
                        if (!isEWC && !isFWC)
                        {
                            //���� �ִٸ� �����ְ�
                            if (weaponLTR.childCount != 0)
                                ClearWeapon(1);

                            CreateFloorBullet(weaponTypeR - 4, 1);
                        }
                    }
                    else if (weaponTypeR >= 4 && weaponTypeR <= 5)
                    {
                        if (!isEWC && !isFWC)
                        {
                            //�̹� ������� �ִ� ���¶��, �ٵ� ����� �ٸ��ٸ�
                            if (weaponTypeR == 4 && (floorWeapon2.GetChild(0).name != "BulletElement(Clone)") && floorWeapon2.GetChild(0).name != "BulletElementPP(Clone)")
                            {
                                ClearWeapon(3);
                                CreateFloorBullet(weaponTypeR - 4, 0);
                            }
                            //�̹� ������� �ִ� ���¶��, �ٵ� ����� �ٸ��ٸ�
                            if (weaponTypeR == 5 && (floorWeapon2.GetChild(0).name != "BulletFire(Clone)" && floorWeapon2.GetChild(0).name != "BulletFirePP(Clone)"))
                            {
                                ClearWeapon(3);
                                CreateFloorBullet(weaponTypeR - 4, 0);
                            }
                        }
                    }
                }
            }

            //���̵� ����
            if (!(sideWeaponTypeL == -1 && sideWeaponTypeR == -1))
            {
                if (sideWeaponTypeL != -1) //���� ���̵� ���Ⱑ �ִٸ�
                {
                    if (sideWeaponTypeL == 3 && weaponTypeL >= -1 && weaponTypeL <= 2 && sideWeaponLTL.childCount == 0) //�߻���źȯ�϶� ���� ����
                    {
                        //������ �ִٸ� �����ְ�
                        if (sideFloorWeapon1.childCount != 0)
                            ClearSideWeapon(2);

                        CreateSideSword(0); //�˹��� ����
                    }
                    else if (sideWeaponTypeL == 3 && weaponTypeL > 2 && sideWeaponLTL.childCount != 0) //�ƴϸ� ����
                        ClearSideWeapon(0);

                    if ((sideWeaponTypeL == 4 || sideWeaponTypeL == 5) && weaponTypeL >= -1 && weaponTypeL <= 2) //�߻���źȯ�϶� ������ ����
                    {
                        //���� �ִٸ� �����ְ�
                        if (sideWeaponLTL.childCount != 0)
                            ClearSideWeapon(0);

                        if (sideFloorWeapon1.childCount != 0)
                        {
                            //�̹� ������� �ִ� ���¶��, �ٵ� ����� �ٸ��ٸ�
                            if (sideWeaponTypeL == 4 && sideFloorWeapon1.GetChild(0).name != "BulletElement(Clone)")
                            {
                                ClearSideWeapon(2);
                                CreateSideFloorBullet(sideWeaponTypeL - 4, 0);
                            }
                            //�̹� ������� �ִ� ���¶��, �ٵ� ����� �ٸ��ٸ�
                            if (sideWeaponTypeL == 5 && sideFloorWeapon1.GetChild(0).name != "BulletFire(Clone)")
                            {
                                ClearSideWeapon(2);
                                CreateSideFloorBullet(sideWeaponTypeL - 4, 0);
                            }
                        }
                        else
                            CreateSideFloorBullet(sideWeaponTypeL - 4, 0); //���� ����
                    }
                    else if ((sideWeaponTypeL == 4 || sideWeaponTypeL == 5) && weaponTypeL > 2 && sideFloorWeapon1.childCount != 0) //�ƴϸ� ����
                        ClearSideWeapon(2);


                }
                if (sideWeaponTypeR != -1) //������ ���Ⱑ �ִٸ�
                {
                    if (sideWeaponTypeR == 3 && weaponTypeR >= -1 && weaponTypeR <= 2 && sideWeaponLTR.childCount == 0) //�߻���źȯ�϶� ���� ����
                    {
                        //������ �ִٸ� �����ְ�
                        if (sideFloorWeapon2.childCount != 0)
                            ClearSideWeapon(3);

                        CreateSideSword(1); //�˹��� ������
                    }
                    else if (sideWeaponTypeR == 3 && weaponTypeR > 2 && sideWeaponLTR.childCount != 0) //�ƴϸ� ����
                        ClearSideWeapon(1);

                    if ((sideWeaponTypeR == 4 || sideWeaponTypeR == 5) && weaponTypeR >= -1 && weaponTypeR <= 2) //�߻���źȯ�϶� ������ ����
                    {
                        //���� �ִٸ� �����ְ�
                        if (sideWeaponLTR.childCount != 0)
                            ClearSideWeapon(1);

                        if (sideFloorWeapon2.childCount != 0)
                        {
                            //�̹� ������� �ִ� ���¶��, �ٵ� ����� �ٸ��ٸ�
                            if (sideWeaponTypeR == 4 && sideFloorWeapon2.GetChild(0).name != "BulletElement(Clone)")
                            {
                                ClearSideWeapon(3);
                                CreateSideFloorBullet(sideWeaponTypeR - 4, 0);
                            }
                            //�̹� ������� �ִ� ���¶��, �ٵ� ����� �ٸ��ٸ�
                            if (sideWeaponTypeR == 5 && sideFloorWeapon2.GetChild(0).name != "BulletFire(Clone)")
                            {
                                ClearSideWeapon(3);
                                CreateSideFloorBullet(sideWeaponTypeR - 4, 0);
                            }
                        }
                        else
                            CreateSideFloorBullet(sideWeaponTypeR - 4, 1); //���� ����
                    }
                    else if ((sideWeaponTypeR == 4 || sideWeaponTypeR == 5) && weaponTypeR > 2 && sideFloorWeapon2.childCount != 0) //�ƴϸ� ����
                        ClearSideWeapon(3);
                }
            }

            if (time >= atkSpd) //time�ʱ�ȭ
                time = 0f;
        }

        if (characterStatus.hiddenCombo == 10) //���� ���̴�
            slashTime += Time.deltaTime;

        if (slashTime >= 5.0f)
        {
            if (!GameManager.instance.isEnd)
            {
                if (!GameManager.instance.isPause)
                {
                    slashRangeCol = Physics2D.OverlapCircleAll(characterLT.transform.position, 3.0f); //���� �ȿ� ������ ����  
                    slashEffect.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f);
                    slashEffect.SetActive(true);

                    AudioManager.instance.PlaySFX("Slash");

                    List<Collider2D> slashAttack = new List<Collider2D>(); //������ ��������
                    for (int i = 0; i < slashRangeCol.Length; i++)
                    {
                        if (slashRangeCol[i].CompareTag("Enemy")) //���̶��, �׸��� ��ܿ� �ִ� ����
                        {
                           slashAttack.Add(slashRangeCol[i]);
                        }
                    }
                    StartCoroutine(SlashAttack(slashAttack)); //1�ʵڿ� ����
                }
            }
            slashTime = 0f;
        }

        if (characterStatus.hiddenCombo == 11) //���� ������
            mmTime += Time.deltaTime;

        if (mmTime >= 3.0f)
        {
            if (!GameManager.instance.isEnd)
            {
                if (!GameManager.instance.isPause)
                {

                    GameObject mm = Instantiate(magicMine, transform.position, Quaternion.identity); //��ü����

                    mm.GetComponent<WeaponManager>().weaponType = 3;
                    mm.GetComponent<WeaponManager>().isClone = true; //Ŭ�м���
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

        float critRand = Random.Range(0f, 100f); //ũ��Ƽ��
        for (int i = 0; i < slashAttack.Count; i++)
        {
            if (slashAttack[i].gameObject.activeSelf == true) //����ִ� ����
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


    //ĳ���Ͱ� �⺻���� �߻��ϴ� źȯ
    private void CreateBasicBullet()
    {
        if (characterStatus.hiddenCombo != 13) //���緱ó�� �ƴ϶��
        {
            GameObject tempBullet = null;
            if (characterStatus.weaponCombo == 2) //Gunner��� Ư�� źȯ����
            {
                if (characterStatus.hiddenCombo == 12) //���� �ųʶ�� �� Ư��
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
                tempBullet = ObjectPool.instance.queue[0].Dequeue(); //������Ʈ Ǯ�� ���
                tempBullet.GetComponent<Bullet>().bulletType = 0;
            }

            tempBullet.transform.localEulerAngles = characterDirect.localEulerAngles; //ĳ���Ͱ� �ٶ󺸴� �������� �߻� ����
            tempBullet.transform.position = new Vector2(characterLT.position.x + 0.6f, characterLT.position.y + 0.5f);//�ð������� ĳ���ͺ��� �տ��� �߻�
            tempBullet.GetComponent<Bullet>().speed = characterStatus.spd * 2f;
            tempBullet.GetComponent<Bullet>().atk = atk;
            tempBullet.GetComponent<Bullet>().crit = crit;
            tempBullet.GetComponent<Bullet>().istriggered = false;
            tempBullet.GetComponent<Bullet>().weaponCombo = characterStatus.weaponCombo;
            tempBullet.GetComponent<Bullet>().equipCombo = -1;  //�н��� ���ȿ�� ����x
            tempBullet.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f, 0.5f); //�н� źȯ��
            tempBullet.SetActive(true);
        }
        else
        {
            GameObject tempBullet = null;
            tempBullet = ObjectPool.instance.queue[11].Dequeue(); //������Ʈ Ǯ�� ���
            tempBullet.GetComponent<Bullet>().bulletType = 11;
            tempBullet.transform.localEulerAngles = characterDirect.localEulerAngles; //ĳ���Ͱ� �ٶ󺸴� �������� �߻� ����
            tempBullet.transform.position = new Vector2(characterLT.position.x + 0.6f, characterLT.position.y + 0.5f);//�ð������� ĳ���ͺ��� �տ��� �߻�
            tempBullet.GetComponent<Bullet>().speed = characterStatus.spd * 2f;
            tempBullet.GetComponent<Bullet>().atk = atk;
            tempBullet.GetComponent<Bullet>().crit = crit;
            tempBullet.GetComponent<Bullet>().weaponCombo = characterStatus.weaponCombo;
            tempBullet.GetComponent<Bullet>().equipCombo = characterStatus.equipCombo;
            tempBullet.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f, 0.5f); //�н� źȯ��
            tempBullet.SetActive(true);
        }
    }

    //�߻��� ����(0, 1, 2)
    public void CreateBullet(int weaponType, int dir) //���� Ÿ�԰� �������� ���������� ������ �޾ƿ´�.
    {
        if (characterStatus.hiddenCombo != 13) //���緱ó�� �ƴ϶��
        {
            GameObject tempBullet = null;
            if (characterStatus.weaponCombo == 1) //Magicion ��� Ư�� źȯ����
            {
                tempBullet = ObjectPool.instance.queue[4].Dequeue();
                tempBullet.GetComponent<Bullet>().bulletType = 4;
               
            }
            else if (characterStatus.weaponCombo == 2) //Gunner��� Ư�� źȯ����
            {
                if (characterStatus.hiddenCombo == 12) //���� �ųʶ�� �� Ư��
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
            else if (characterStatus.weaponCombo == 3) //Launcher��� Ư�� źȯ����
            {
                tempBullet = ObjectPool.instance.queue[5].Dequeue(); //������Ʈ Ǯ�� ���
                tempBullet.GetComponent<Bullet>().bulletType = 5;
            }
            else
            {
                tempBullet = ObjectPool.instance.queue[weaponType].Dequeue(); //������Ʈ Ǯ�� ���
                tempBullet.GetComponent<Bullet>().bulletType = weaponType;
            }


            if (dir == 0) //���⿡ ���� ��ġ ����
            {
                tempBullet.transform.localEulerAngles = characterDirect.localEulerAngles + weaponLTL.localEulerAngles; //ĳ���Ͱ� �ٶ󺸴� �������� �߻� ����
                tempBullet.transform.position = weaponLTL.position + transform.up;
            }
            else if (dir == 1)
            {
                tempBullet.transform.localEulerAngles = characterDirect.localEulerAngles + weaponLTR.localEulerAngles; //ĳ���Ͱ� �ٶ󺸴� �������� �߻� ����
                tempBullet.transform.position = weaponLTR.position + transform.up;
            }

            tempBullet.GetComponent<Bullet>().speed = characterStatus.spd * 2f;
            tempBullet.GetComponent<Bullet>().atk = atk;
            tempBullet.GetComponent<Bullet>().crit = crit;
            tempBullet.GetComponent<Bullet>().istriggered = false;
            tempBullet.GetComponent<Bullet>().weaponCombo = characterStatus.weaponCombo;
            tempBullet.GetComponent<Bullet>().equipCombo = -1; //�н��� ���ȿ�� ����x
            tempBullet.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f, 0.5f);//�н�źȯ��
            tempBullet.SetActive(true);
        }
    }


    //ĳ���� ���̵�� �߻��ϴ� źȯ
    private void CreateSideBullet(int weaponType, int dir)
    {
        if (characterStatus.hiddenCombo != 13) //���緱ó�� �ƴ϶��
        {
            GameObject tempBullet = null;

            if (characterStatus.hiddenCombo == 12) //���� Gunner��� Ư�� źȯ����
            {
                tempBullet = ObjectPool.instance.queue[10].Dequeue();
                tempBullet.GetComponent<Bullet>().bulletType = 10;
            }
            else
            {
                tempBullet = ObjectPool.instance.queue[weaponType].Dequeue(); //������Ʈ Ǯ�� ���
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

            tempBullet.transform.localEulerAngles = characterDirect.localEulerAngles; //ĳ���Ͱ� �ٶ󺸴� �������� �߻� ����
            tempBullet.GetComponent<Bullet>().speed = characterStatus.spd * 2f;
            tempBullet.GetComponent<Bullet>().crit = characterStatus.crit;
            tempBullet.GetComponent<Bullet>().istriggered = false;
            tempBullet.GetComponent<Bullet>().weaponCombo = characterStatus.weaponCombo;
            tempBullet.GetComponent<Bullet>().equipCombo = -1;
            tempBullet.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            tempBullet.SetActive(true);

            if (characterStatus.hiddenCombo == 12) //���� Gunner��� Ư�� źȯ����
                tempBullet.GetComponent<Bullet>().atk = characterStatus.atk;
            else
                tempBullet.GetComponent<Bullet>().atk = characterStatus.atk / 2;
        }
    }

    //�� ����(3), type 0 = L, 1 = R, 2 = Combo
    public void CreateSword(int type)
    {
        if (type == 2) //�޺�ȿ�����
        {
            GameObject sword1 = Instantiate(characterWeapon.weapons[type + 2], transform.position, Quaternion.identity); //��ü����
            GameObject sword2 = Instantiate(characterWeapon.weapons[type + 3], transform.position, Quaternion.identity); //��ü����

            sword1.transform.localScale = new Vector3(15f, 15f, 1f); //���� ũ�⸦ 2���
            sword2.transform.localScale = new Vector3(15f, 15f, 1f); //���� ũ�⸦ 2���

            sword1.transform.SetParent(weaponLTL, false); //�θ� ��ü ����
            sword2.transform.SetParent(weaponLTR, false); //�θ� ��ü ����

            sword1.GetComponent<WeaponManager>().weaponType = 0; //type�� ���� ���� 0
            sword1.GetComponent<WeaponManager>().isClone = true; //Ŭ�м���
            sword1.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f);

            sword2.GetComponent<WeaponManager>().weaponType = 0; //type�� ���� ���� 0
            sword2.GetComponent<WeaponManager>().isClone = true; //Ŭ�м���
            sword2.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f);

            return; //��
        }

        GameObject sword = Instantiate(characterWeapon.weapons[type + 4], transform.position, Quaternion.identity); //��ü����

        sword.GetComponent<WeaponManager>().weaponType = 0; //type�� ���� ���� 0
        sword.GetComponent<WeaponManager>().isClone = true; //Ŭ�м���
        sword.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f);

        //�θ�ü����
        if (type == 0)
            sword.transform.SetParent(weaponLTL, false);
        else if (type == 1)
            sword.transform.SetParent(weaponLTR, false);
    }

    public void CreateSideSword(int type)
    {
        GameObject sword = Instantiate(characterWeapon.weapons[type + 4], transform.position, Quaternion.identity); //��ü����

        sword.GetComponent<WeaponManager>().weaponType = 0; //type�� ���� ���� 0
        sword.GetComponent<WeaponManager>().isSide = true;
        sword.GetComponent<WeaponManager>().isClone = true; //Ŭ�м���
        sword.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f);

        //�θ�ü����
        if (type == 0)
            sword.transform.SetParent(sideWeaponLTL, false);
        else if (type == 1)
            sword.transform.SetParent(sideWeaponLTR, false);
    }

    //�ٴ��� ����(4, 5) type 0 = element, 1 = fire, 2 = elements, 3 = fires , 6 = elementPP, 7 = firePP lot  0 = L, 1 = R
    private void CreateFloorBullet(int type, int lot)
    {
        GameObject floor = Instantiate(characterWeapon.weapons[type]); //��ü����

        if (type == 0 || type == 2 || type == 6)
            floor.GetComponent<WeaponManager>().weaponType = 1;
        else if (type == 1 || type == 3 || type == 7)
            floor.GetComponent<WeaponManager>().weaponType = 2;

        floor.GetComponent<WeaponManager>().isClone = true; //Ŭ�м���
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

    //�ٴ��� ����(4, 5) type 0 = element, 1 = fire, 2 = elements, 3 = fires , lot  0 = L, 1 = R
    private void CreateSideFloorBullet(int type, int lot)
    {
        GameObject floor = Instantiate(characterWeapon.weapons[type]); //��ü����

        if (type == 0 || type == 2)
            floor.GetComponent<WeaponManager>().weaponType = 1;
        else if (type == 1 || type == 3)
            floor.GetComponent<WeaponManager>().weaponType = 2;

        floor.GetComponent<WeaponManager>().isClone = true; //Ŭ�м���
        floor.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f);
        floor.GetComponent<WeaponManager>().isSide = true;

        if (lot == 0)
            floor.transform.SetParent(sideFloorWeapon1, false);
        else if (lot == 1)
            floor.transform.SetParent(sideFloorWeapon2, false);

    }

    //������ ����
    private void EquipWeaponCombo()
    {
        //�� �����϶�, �޺� ���� �ƴҶ�
        if (weaponTypeL == 3 && !isSWC)
        {
            for (int i = 0; i < 4; i++)
                ClearWeapon(i);

            //���⸦ �����ϰ� �޺�ȿ�� ���⸦ �����Ѵ�.
            CreateSword(2);
            isSWC = true;
        }

        //element�����϶�, ���� ������ �ϴ� 1����
        if (weaponTypeL == 4 && !isEWC)
        {
            for (int i = 0; i < 4; i++)
                ClearWeapon(i);

            if (characterStatus.hiddenCombo == 14) //���� ������Ʈ
                CreateFloorBullet(6, 0);
            else
                CreateFloorBullet(2, 0);
            isEWC = true;
        }

        //�� �����϶�
        if (weaponTypeL == 5 && !isFWC)
        {
            for (int i = 0; i < 4; i++)
                ClearWeapon(i);

            if (characterStatus.hiddenCombo == 15) //���� ������Ʈ
                CreateFloorBullet(7, 0);
            else
                CreateFloorBullet(3, 0);
            isFWC = true;
        }
    }

    //���⸦ ������Ų��. 0 = ���ʰ�, 1 = �����ʰ�, 2 = ����1, 3 = ����2
    private void ClearWeapon(int type)
    {
        Transform[] childs = null;

        switch (type)
        {
            case 0:
                childs = weaponLTL.GetComponentsInChildren<Transform>();//�ڽĵ��� ���� ��Ƽ� ����
                break;
            case 1:
                childs = weaponLTR.GetComponentsInChildren<Transform>();//�ڽĵ��� ���� ��Ƽ� ����
                break;
            case 2:
                childs = floorWeapon1.GetComponentsInChildren<Transform>();//�ڽĵ��� ���� ��Ƽ� ����
                break;
            case 3:
                childs = floorWeapon2.GetComponentsInChildren<Transform>();//�ڽĵ��� ���� ��Ƽ� ����
                break;
        }

        if (childs != null)
            for (int i = 1; i < childs.Length; i++)
            {
                if (childs[i] != transform)
                    Destroy(childs[i].gameObject);
            }
    }

    //���⸦ ������Ų��. 0 = ���ʰ�, 1 = �����ʰ�, 2 = ����1, 3 = ����2
    private void ClearSideWeapon(int type)
    {
        Transform[] childs = null;

        switch (type)
        {
            case 0:
                childs = sideWeaponLTL.GetComponentsInChildren<Transform>();//�ڽĵ��� ���� ��Ƽ� ����
                break;
            case 1:
                childs = sideWeaponLTR.GetComponentsInChildren<Transform>();//�ڽĵ��� ���� ��Ƽ� ����
                break;
            case 2:
                childs = sideFloorWeapon1.GetComponentsInChildren<Transform>();//�ڽĵ��� ���� ��Ƽ� ����
                break;
            case 3:
                childs = sideFloorWeapon2.GetComponentsInChildren<Transform>();//�ڽĵ��� ���� ��Ƽ� ����
                break;
        }

        if (childs != null)
            for (int i = 1; i < childs.Length; i++)
            {
                if (childs[i] != transform)
                    Destroy(childs[i].gameObject);
            }
    }

    //���� ȿ���� ������Ų��.
    private void ClearWeaoponCombo()
    {
        if (isSWC)
        {
            //����� �ٽ� ������ֱ�
            ClearWeapon(0);
            ClearWeapon(1);

            if (weaponTypeL == 3) //�� ������
                CreateSword(0); //�˹��� ����

            if (weaponTypeR == 3) //�� ������
                CreateSword(1); //�˹��� ������

            isSWC = false;
        }

        if (isEWC)
        {
            //����� �ٽ� ������ֱ�
            ClearWeapon(2);
            ClearWeapon(3);

            if (weaponTypeL == 4) //���� ������             
                CreateFloorBullet(weaponTypeL - 4, 0);

            if (weaponTypeR == 4) //���� ������
                CreateFloorBullet(weaponTypeR - 4, 1);

            isEWC = false;
        }

        if (isFWC)
        {
            //����� �ٽ� ������ֱ�
            ClearWeapon(2);
            ClearWeapon(3);

            if (weaponTypeL == 5) //���� ������
                CreateFloorBullet(weaponTypeL - 4, 0);

            if (weaponTypeR == 5) //���� ������
                CreateFloorBullet(weaponTypeR - 4, 1);

            isFWC = false;
        }
    }

    public void CheckHiddenCombo()
    {
        //element�����϶�
        if (characterStatus.hiddenCombo == 14)
        {
            for (int i = 0; i < 4; i++)
                ClearWeapon(i);

            CreateFloorBullet(6, 0);

            isEWC = true;
        }

        //element�����϶�
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
