using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;

//���� ���� â�� CharacterStatus, Inventory, CharacterJob�� ���¸� ��� ���ؾ��Ѵ�.
public class CharacterStatus : MonoBehaviour
{
    //����â
    public float maxHP = 100;
    public float hp = 100;
    public float atk = 100;
    public float def = 0;
    public float atkSpd = 1f;
    public float MaxAtkSpd = 0.1f;
    public float spd = 5;
    public float MaxSpd = 15;
    public float crit = 0;
    public float hps = 0; //�ʴ� ü�� ȸ��
    //���� �� ���
    public int weaponL = -1; //-1�� ����
    public int weaponR = -1;
    public int sideWeaponL = -1; //-1�� ����
    public int sideWeaponR = -1;
    public int equipL = -1; 
    public int equipR = -1;
    public int weaponCombo = -1;
    public int equipCombo = -1;
    public int sideWeaponCombo = -1;
    public int hiddenCombo = -1;

    //�н�ȿ��
    public GameObject clone;
    //HPS�� ���� �� time
    private float time = 0f;
    //���� ������ ������� time
    private float blackTime = 0f;

    private Inventory inventory;
    private CharacterMove characterMove;
    private CharacterWeapon characterWeapon;

    //equipComboAura ���� ��ƼŬ�ý���
    public GameObject equipComboAura;
    private ParticleSystem equipComboPartical;
    public Color equipComboColor
    {
        set
        {
            var main = equipComboPartical.main;
            main.startColor = value;
        }
    }

    private void Start()
    {
        ConfirmStatus(); //�������� status��������

        inventory = FindObjectOfType<Inventory>();
        characterMove = FindObjectOfType<CharacterMove>();
        characterWeapon = GetComponent<CharacterWeapon>();

        equipComboPartical = equipComboAura.GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (hps != 0) //HPS�� �����Ѵٸ�
        {
            time += Time.deltaTime;
            if (time >= 1.0f) //1�ʸ���
            {
                time = 0f;
                hp += hps;

                if (hp > maxHP) //�Ѵ´ٸ� �ִ�ü������
                    hp = maxHP;
            }
        }

        CheckAura(); //����޺� ���� ȿ�� Ȯ��
    }

    private void CheckAura()
    {
        if (equipCombo == -1 && equipComboAura.activeSelf == true) //aura ���ֱ�
            equipComboAura.SetActive(false);
        if (equipCombo != -1 && equipComboAura.activeSelf == false) //aura ���ֱ�
        {
            equipComboAura.SetActive(true);
            switch (equipCombo)
            {
                case 0:
                    equipComboColor = new Color(0f, 1f, 0f, 0.5f);
                    break;
                case 1:
                    equipComboColor = new Color(0.8f, 0.8f, 0.8f, 0.5f);
                    break;
                case 2:
                    equipComboColor = new Color(0f, 1f, 1f, 0.5f);
                    break;
                case 3:
                    equipComboColor = new Color(1f, 1f, 0f, 0.5f);
                    break;
                case 4:
                    equipComboColor = new Color(1f, 0f, 0f, 0.5f);
                    break;
                case 5:
                    equipComboColor = new Color(1f, 0f, 1f, 0.5f);
                    break;
                default:
                    break;
            }
        }
    }

    public void CharacterHitted(float atk)
    {
        float damage = atk - def;
        if (damage <= 0)
            damage = 1f;

        hp -= damage;

        if (hp < 0)
        {
            hp = 0;
            GameManager.instance.isEnd = true;
        }

        AudioManager.instance.PlaySFX("PlayerHit");
        GameObject effect = ObjectPool.instance.queue[7].Dequeue();
        effect.SetActive(true);
        effect.transform.position = gameObject.transform.position;
    }

    //���� �޺� ȿ�� ����
    public void SetWeaponCombo(int wc)
    {
        int lastWeaponCombo = weaponCombo; //�����ؾ� �� ȿ���� �Ѱ��ش�.
        weaponCombo = wc;
        //�������� ���� ȿ��
        if (lastWeaponCombo == 4)
            hps -= 3;
        if (weaponCombo == 4)
            hps += 3;
    }
    //��� �޺� ȿ�� ����
    public void SetEquipCombo(int ec)
    {
        int lastEquipCombo = equipCombo;//�����ؾ� �� ȿ���� �Ѱ��ش�.
        equipCombo = ec;
        //�н�ȿ��
        if (lastEquipCombo == 5)
            clone.SetActive(false);
        if (equipCombo == 5)
        {
            clone.SetActive(true);
            characterWeapon.RestartWeapon();
        }

    }
    //���� �޺� ȿ�� ����
    public void SetSideWeaponCombo(int wc)
    {
        int lastWeaponCombo = sideWeaponCombo; //�����ؾ� �� ȿ���� �Ѱ��ش�.
        sideWeaponCombo = wc;
    }
    //���� �޺� ȿ�� ����
    public void SetHiddenCombo(int wc)
    {
        int lastHiddenCombo = hiddenCombo; //�����ؾ� �� ȿ���� �Ѱ��ش�.
        hiddenCombo = wc;
    }
    public void SetDef(float defP)
    {
        def += defP;
    }

    //���� ���� ����
    public void SetAtkSpd(float atkspdP)
    {
        atkSpd -= atkspdP;
        if (atkSpd < 0.1f)
            atkSpd = 0.1f;
    }
    public void SetSpeed(float spdP)
    {
        spd += spdP;
        if (spd > 15f)
            spd = 15f;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Black"))
        {
            if (!GameManager.instance.isEnd && !GameManager.instance.isPause)
            {
                hp -= 10 / 60f;
                blackTime += 1 / 60f;
                if (blackTime >= 1f)
                {
                    AudioManager.instance.PlaySFX("PlayerHit");
                    GameObject effect = ObjectPool.instance.queue[7].Dequeue();
                    effect.SetActive(true);
                    effect.transform.position = gameObject.transform.position;
                    blackTime = 0f;
                }
            }
        }
    }

    private void ConfirmStatus()
    {
        var bro = Backend.GameData.GetMyData("Status", new Where(), 10);
        if (bro.IsSuccess() == false)
        {
            // ��û ���� ó��
            //Debug.Log(bro);
            return;
        }
        if (bro.GetReturnValuetoJSON()["rows"].Count <= 0)
        {
            // ��û�� �����ص� where ���ǿ� �����ϴ� �����Ͱ� ���� �� �ֱ� ������
            // �����Ͱ� �����ϴ��� Ȯ��
            // ���� ���� new Where() ������ ��� ���̺� row�� �ϳ��� ������ Count�� 0 ���� �� �� �ִ�.
            //Debug.Log(bro);
            return;
        }
        // �˻��� �������� ��� row�� inDate �� Ȯ��
        for (int i = 0; i < bro.Rows().Count; ++i)
        {
            string tempID = bro.Rows()[i]["ID"]["S"].ToString();
            float tempHP = float.Parse(bro.Rows()[i]["HP"]["N"].ToString());
            float tempAtk = float.Parse(bro.Rows()[i]["Atk"]["N"].ToString());
            float tempDef = float.Parse(bro.Rows()[i]["Def"]["N"].ToString());
            float tempAtkSpd = float.Parse(bro.Rows()[i]["AtkSpd"]["N"].ToString());
            float tempSpd = float.Parse(bro.Rows()[i]["Spd"]["N"].ToString());
            float tempCrit = float.Parse(bro.Rows()[i]["Crit"]["N"].ToString());
            float tempHps = float.Parse(bro.Rows()[i]["Hps"]["N"].ToString());
            float tempPlusPt = float.Parse(bro.Rows()[i]["PlusPt"]["N"].ToString());
            float tempRemainPt = float.Parse(bro.Rows()[i]["RemainPt"]["N"].ToString());
            int tempLV = int.Parse(bro.Rows()[i]["LV"]["N"].ToString());

            hp = tempHP;
            atk = tempAtk;
            def = tempDef;
            atkSpd = tempAtkSpd;
            spd = tempSpd;
            crit = tempCrit;
            hps = tempHps; //�ʴ� ü�� ȸ��
        }
    }
}
