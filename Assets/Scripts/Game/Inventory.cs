using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//���� ĵ������ ī�޶�� �����Ǹ鼭 �������� �ſ� �پ��� Image�� ���
//ĵ���� ���� sprite�� ���

public class Inventory : MonoBehaviour
{
    //�κ��丮 �̹�����
    public ItemBtn weapon1;
    public ItemBtn weapon2;
    public ItemBtn sideWeapon1;
    public ItemBtn sideWeapon2;
    public Image weaponCombo;
    public ItemBtn equip1;
    public ItemBtn equip2;
    public Image equipCombo;

    //���� �� ��� ����
    public int weaponL = -1;
    public int weaponR = -1;
    public int sideWeaponL = -1;
    public int sideWeaponR = -1;
    public int equipL = -1;
    public int equipR = -1;

    private ItemDatabase itemDatabase;
    private CharacterStatus characterStatus;
    private CharacterWeapon characterWeapon;

    private void Start()
    {
        itemDatabase = FindObjectOfType<ItemDatabase>();
        characterStatus = FindObjectOfType<CharacterStatus>();
        characterWeapon = FindObjectOfType<CharacterWeapon>();
    }

    //�κ��丮�� �ִ´�, Ÿ���� ����0���� ������1����
    public void PutWepaonInventory(GameObject weaponBtn, int type)
    {
        Image weapon = null;

        if (type == 0) //Ÿ�Կ� ���� ���� ����
        {
            //�ð��� �����ֱ�
            weapon = weapon1.img;
            weapon1.name.text = weaponBtn.transform.GetChild(0).GetComponent<Text>().text;
        }
        else
        {
            //�ð��� �����ֱ�
            weapon = weapon2.img;
            weapon2.name.text = weaponBtn.transform.GetChild(0).GetComponent<Text>().text;
        }

        weapon.sprite = weaponBtn.GetComponent<Image>().sprite;

        //ĳ���������� ����ִ� ���� �߰�
        for (int i = 0; i < itemDatabase.items.Count; i++)
        {
            if (weaponBtn.transform.GetChild(0).GetComponent<Text>().text == itemDatabase.items[i].itemName)
            {
                if (type == 0)
                {
                    characterStatus.weaponL = i;
                    characterWeapon.weaponTypeL = i;
                    weaponL = i;
                }
                else
                {
                    characterStatus.weaponR = i;
                    characterWeapon.weaponTypeR = i;
                    weaponR = i;
                }
            }
        }

        //�� �ʿ� ���⸦ ���� ����� ��
        if (characterStatus.weaponL != -1 && characterStatus.weaponR != -1)
        {
            PutWeaponCombo();
        }
    }

    //�κ��丮�� �ִ´�, Ÿ���� ����0���� ������1����
    public void PutEquipInventory(GameObject equipBtn, int type)
    {
        Image equip = null;

        if (type == 0) //Ÿ�Կ� ���� ���� ����
        {
            equip = equip1.img;
            equip1.name.text = equipBtn.transform.GetChild(0).GetComponent<Text>().text;
        }
        else
        {
            equip = equip2.img;
            equip2.name.text = equipBtn.transform.GetChild(0).GetComponent<Text>().text;
        }

        equip.sprite = equipBtn.GetComponent<Image>().sprite;
       
        //ĳ���������� ����ִ� ��� �߰�
        for (int i = 0; i < itemDatabase.items.Count; i++)
        {
            if (equipBtn.transform.GetChild(0).GetComponent<Text>().text == itemDatabase.items[i].itemName)
                if (type == 0)
                {
                    characterStatus.equipL = i;
                    equipL = i;
                }
                else
                { 
                    characterStatus.equipR = i;
                    equipR = i;
                }
        }

        //�� �ʿ� ��� ���� ����� ��
        if (characterStatus.equipL != -1 && characterStatus.equipR != -1)
            PutEquipCombo();
    }

    //���̵� �κ��丮�� �ִ´�, Ÿ���� ����0���� ������1����
    public void PutSideWepaonInventory(GameObject weaponBtn, int type)
    {
        Image weapon = null;

        if (type == 0) //Ÿ�Կ� ���� ���� ����
        {
            //�ð��� �����ֱ�
            weapon = sideWeapon1.img;
            sideWeapon1.name.text = weaponBtn.transform.GetChild(0).GetComponent<Text>().text;
        }
        else
        {
            //�ð��� �����ֱ�
            weapon = sideWeapon2.img;
            sideWeapon2.name.text = weaponBtn.transform.GetChild(0).GetComponent<Text>().text;
        }

        weapon.sprite = weaponBtn.GetComponent<Image>().sprite;

        //ĳ���������� ����ִ� ���� �߰�
        for (int i = 0; i < itemDatabase.items.Count; i++)
        {
            if (weaponBtn.transform.GetChild(0).GetComponent<Text>().text == itemDatabase.items[i].itemName)
            {
                if (type == 0)
                {
                    characterStatus.sideWeaponL = i;
                    characterWeapon.sideWeaponTypeL = i;
                    sideWeaponL = i;
                }
                else
                {
                    characterStatus.sideWeaponR = i;
                    characterWeapon.sideWeaponTypeR = i;
                    sideWeaponR = i;
                }
            }
        }

        //�� �ʿ� ���⸦ ���� ����� ��
        if (characterStatus.sideWeaponL != -1 && characterStatus.sideWeaponR != -1)
        {
            PutSideWeaponCombo();
        }
    }

    //������ ĳ���Ϳ� �߰��Ѵ�. ������ ������ ��ü ����â�� ����.
    public void PutStat(GameObject statBtn)
    {
        switch (statBtn.GetComponent<Text>().text)
        {
            case "MaxHP + 30":
                characterStatus.maxHP +=  30;
                characterStatus.hp += 30;
                break;
            case "ATK + 20":
                characterStatus.atk += 20;
                break;
            case "DEF + 10":
                characterStatus.SetDef(10);
                break;
            case "SPD + 0.5":
                characterStatus.SetSpeed(0.5f); //���� �ӵ� ����
                break;
            case "ASPD-0.05":
                characterStatus.SetAtkSpd(0.05f); //���� ���ݼӵ� ����
                break;
            case "CRI + 5":
                characterStatus.crit += 5;
                break;
        }
    }

    public void PutWeaponCombo() 
    {      
        for (int i = 0; i < itemDatabase.wcCombos.Count / 2; i++) //��������
        {
            //recipe�� ��, ������ ���� �ٲ�������������� 2���˻�
            if (characterStatus.weaponL == itemDatabase.wcCombos[i].recipe[0] &&
                characterStatus.weaponR == itemDatabase.wcCombos[i].recipe[1])
            {
                characterStatus.SetWeaponCombo(itemDatabase.wcCombos[i].wcNum);//ĳ���Ϳ� ���� �߰�
                weaponCombo.sprite = itemDatabase.wcCombos[i].wcImage.sprite; //�κ��丮�� �̹��� �Է�   
                return;
            }
            else if (characterStatus.weaponL == itemDatabase.wcCombos[i].recipe[1] &&
                characterStatus.weaponR == itemDatabase.wcCombos[i].recipe[0])
            {
                characterStatus.SetWeaponCombo(itemDatabase.wcCombos[i].wcNum);
                weaponCombo.sprite = itemDatabase.wcCombos[i].wcImage.sprite; //�κ��丮�� �̹��� �Է�
                return;
            }
            else //������ �ƴҶ�
            {
                characterStatus.SetWeaponCombo(-1);
                weaponCombo.sprite = null; //�κ��丮�� �̹��� �Է�
            }
        }
    }

    public void PutEquipCombo()
    {
        for (int i = 0; i < itemDatabase.ecCombos.Count; i++) 
        {
            //recipe�� ��, ������ ���� �ٲ�������������� 2���˻�
            if (characterStatus.equipL == itemDatabase.ecCombos[i].recipe[0] &&
                characterStatus.equipR == itemDatabase.ecCombos[i].recipe[1])
            {
                characterStatus.SetEquipCombo(itemDatabase.ecCombos[i].ecNum);//ĳ���Ϳ� ���� �߰�
                equipCombo.sprite = itemDatabase.ecCombos[i].ecImage.sprite; //�κ��丮�� �̹��� �Է�   
                return;
            }
            else if (characterStatus.equipL == itemDatabase.ecCombos[i].recipe[1] &&
                characterStatus.equipR == itemDatabase.ecCombos[i].recipe[0])
            {
                characterStatus.SetEquipCombo(itemDatabase.ecCombos[i].ecNum);
                equipCombo.sprite = itemDatabase.ecCombos[i].ecImage.sprite; //�κ��丮�� �̹��� �Է�
                return;
            }
            else
            {
                characterStatus.SetEquipCombo(-1);
                equipCombo.sprite = null; //�κ��丮�� �̹��� �Է�
            }
        }
    }

    public void PutSideWeaponCombo()
    {
        for (int i = 0; i < itemDatabase.wcCombos.Count / 2; i++) //��������
        {
            //recipe�� ��, ������ ���� �ٲ�������������� 2���˻�
            if (characterStatus.sideWeaponL == itemDatabase.wcCombos[i].recipe[0] &&
                characterStatus.sideWeaponR == itemDatabase.wcCombos[i].recipe[1])
            {
                characterStatus.SetSideWeaponCombo(itemDatabase.wcCombos[i].wcNum);//ĳ���Ϳ� ���� �߰�                 
                return;
            }
            else if (characterStatus.sideWeaponL == itemDatabase.wcCombos[i].recipe[1] &&
                characterStatus.sideWeaponR == itemDatabase.wcCombos[i].recipe[0])
            {
                characterStatus.SetSideWeaponCombo(itemDatabase.wcCombos[i].wcNum);
                return;
            }
            else //������ �ƴҶ�
            {
                characterStatus.SetSideWeaponCombo(-1);
            }
        }
    }

    public void CheckHiddenCombo()
    {
        for (int i = itemDatabase.wcCombos.Count / 2; i < itemDatabase.wcCombos.Count; i++) //������ ���ݺ���, �Ϲ��޺� ����
        {
            //recipe�� ��, ������ ���� �ٲ�������������� 2���˻�, ������ ����� ���̵��� ���պ�
            if (characterStatus.weaponCombo == itemDatabase.wcCombos[i].recipe[0] &&
                characterStatus.sideWeaponCombo == itemDatabase.wcCombos[i].recipe[1])
            {
                characterStatus.SetHiddenCombo(itemDatabase.wcCombos[i].wcNum);//ĳ���Ϳ� ���� �߰�
                weaponCombo.sprite = itemDatabase.wcCombos[i].wcImage.sprite; //�κ��丮�� �̹��� �Է�   
                return;
            }
            else if (characterStatus.weaponCombo == itemDatabase.wcCombos[i].recipe[1] &&
                characterStatus.sideWeaponCombo == itemDatabase.wcCombos[i].recipe[0])
            {
                characterStatus.SetHiddenCombo(itemDatabase.wcCombos[i].wcNum);
                weaponCombo.sprite = itemDatabase.wcCombos[i].wcImage.sprite; //�κ��丮�� �̹��� �Է�
                return;
            }
            else //������ �ƴҶ�
            {
                characterStatus.SetHiddenCombo(-1);
                PutWeaponCombo();
            }
        }
    }
}
