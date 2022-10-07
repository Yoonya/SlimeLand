using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//현재 캔버스는 카메라와 연동되면서 스케일이 매우 줄었다 Image를 사용
//캔버스 밖은 sprite를 사용

public class Inventory : MonoBehaviour
{
    //인벤토리 이미지들
    public ItemBtn weapon1;
    public ItemBtn weapon2;
    public ItemBtn sideWeapon1;
    public ItemBtn sideWeapon2;
    public Image weaponCombo;
    public ItemBtn equip1;
    public ItemBtn equip2;
    public Image equipCombo;

    //무기 및 장비 정보
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

    //인벤토리에 넣는다, 타입은 왼쪽0인지 오른쪽1인지
    public void PutWepaonInventory(GameObject weaponBtn, int type)
    {
        Image weapon = null;

        if (type == 0) //타입에 따라서 값을 지정
        {
            //시각적 정보넣기
            weapon = weapon1.img;
            weapon1.name.text = weaponBtn.transform.GetChild(0).GetComponent<Text>().text;
        }
        else
        {
            //시각적 정보넣기
            weapon = weapon2.img;
            weapon2.name.text = weaponBtn.transform.GetChild(0).GetComponent<Text>().text;
        }

        weapon.sprite = weaponBtn.GetComponent<Image>().sprite;

        //캐릭터정보에 들고있는 무기 추가
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

        //양 쪽에 무기를 전부 들었을 때
        if (characterStatus.weaponL != -1 && characterStatus.weaponR != -1)
        {
            PutWeaponCombo();
        }
    }

    //인벤토리에 넣는다, 타입은 왼쪽0인지 오른쪽1인지
    public void PutEquipInventory(GameObject equipBtn, int type)
    {
        Image equip = null;

        if (type == 0) //타입에 따라서 값을 지정
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
       
        //캐릭터정보에 들고있는 장비 추가
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

        //양 쪽에 장비를 전부 들었을 때
        if (characterStatus.equipL != -1 && characterStatus.equipR != -1)
            PutEquipCombo();
    }

    //사이드 인벤토리에 넣는다, 타입은 왼쪽0인지 오른쪽1인지
    public void PutSideWepaonInventory(GameObject weaponBtn, int type)
    {
        Image weapon = null;

        if (type == 0) //타입에 따라서 값을 지정
        {
            //시각적 정보넣기
            weapon = sideWeapon1.img;
            sideWeapon1.name.text = weaponBtn.transform.GetChild(0).GetComponent<Text>().text;
        }
        else
        {
            //시각적 정보넣기
            weapon = sideWeapon2.img;
            sideWeapon2.name.text = weaponBtn.transform.GetChild(0).GetComponent<Text>().text;
        }

        weapon.sprite = weaponBtn.GetComponent<Image>().sprite;

        //캐릭터정보에 들고있는 무기 추가
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

        //양 쪽에 무기를 전부 들었을 때
        if (characterStatus.sideWeaponL != -1 && characterStatus.sideWeaponR != -1)
        {
            PutSideWeaponCombo();
        }
    }

    //스탯을 캐릭터에 추가한다. 스탯은 슬라임 자체 상태창에 들어간다.
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
                characterStatus.SetSpeed(0.5f); //최종 속도 적용
                break;
            case "ASPD-0.05":
                characterStatus.SetAtkSpd(0.05f); //최종 공격속도 적용
                break;
            case "CRI + 5":
                characterStatus.crit += 5;
                break;
        }
    }

    public void PutWeaponCombo() 
    {      
        for (int i = 0; i < itemDatabase.wcCombos.Count / 2; i++) //히든포함
        {
            //recipe와 비교, 오른쪽 왼쪽 바뀌어있을수있으니 2번검사
            if (characterStatus.weaponL == itemDatabase.wcCombos[i].recipe[0] &&
                characterStatus.weaponR == itemDatabase.wcCombos[i].recipe[1])
            {
                characterStatus.SetWeaponCombo(itemDatabase.wcCombos[i].wcNum);//캐릭터에 정보 추가
                weaponCombo.sprite = itemDatabase.wcCombos[i].wcImage.sprite; //인벤토리에 이미지 입력   
                return;
            }
            else if (characterStatus.weaponL == itemDatabase.wcCombos[i].recipe[1] &&
                characterStatus.weaponR == itemDatabase.wcCombos[i].recipe[0])
            {
                characterStatus.SetWeaponCombo(itemDatabase.wcCombos[i].wcNum);
                weaponCombo.sprite = itemDatabase.wcCombos[i].wcImage.sprite; //인벤토리에 이미지 입력
                return;
            }
            else //조합이 아닐때
            {
                characterStatus.SetWeaponCombo(-1);
                weaponCombo.sprite = null; //인벤토리에 이미지 입력
            }
        }
    }

    public void PutEquipCombo()
    {
        for (int i = 0; i < itemDatabase.ecCombos.Count; i++) 
        {
            //recipe와 비교, 오른쪽 왼쪽 바뀌어있을수있으니 2번검사
            if (characterStatus.equipL == itemDatabase.ecCombos[i].recipe[0] &&
                characterStatus.equipR == itemDatabase.ecCombos[i].recipe[1])
            {
                characterStatus.SetEquipCombo(itemDatabase.ecCombos[i].ecNum);//캐릭터에 정보 추가
                equipCombo.sprite = itemDatabase.ecCombos[i].ecImage.sprite; //인벤토리에 이미지 입력   
                return;
            }
            else if (characterStatus.equipL == itemDatabase.ecCombos[i].recipe[1] &&
                characterStatus.equipR == itemDatabase.ecCombos[i].recipe[0])
            {
                characterStatus.SetEquipCombo(itemDatabase.ecCombos[i].ecNum);
                equipCombo.sprite = itemDatabase.ecCombos[i].ecImage.sprite; //인벤토리에 이미지 입력
                return;
            }
            else
            {
                characterStatus.SetEquipCombo(-1);
                equipCombo.sprite = null; //인벤토리에 이미지 입력
            }
        }
    }

    public void PutSideWeaponCombo()
    {
        for (int i = 0; i < itemDatabase.wcCombos.Count / 2; i++) //히든포함
        {
            //recipe와 비교, 오른쪽 왼쪽 바뀌어있을수있으니 2번검사
            if (characterStatus.sideWeaponL == itemDatabase.wcCombos[i].recipe[0] &&
                characterStatus.sideWeaponR == itemDatabase.wcCombos[i].recipe[1])
            {
                characterStatus.SetSideWeaponCombo(itemDatabase.wcCombos[i].wcNum);//캐릭터에 정보 추가                 
                return;
            }
            else if (characterStatus.sideWeaponL == itemDatabase.wcCombos[i].recipe[1] &&
                characterStatus.sideWeaponR == itemDatabase.wcCombos[i].recipe[0])
            {
                characterStatus.SetSideWeaponCombo(itemDatabase.wcCombos[i].wcNum);
                return;
            }
            else //조합이 아닐때
            {
                characterStatus.SetSideWeaponCombo(-1);
            }
        }
    }

    public void CheckHiddenCombo()
    {
        for (int i = itemDatabase.wcCombos.Count / 2; i < itemDatabase.wcCombos.Count; i++) //히든은 절반부터, 일반콤보 포함
        {
            //recipe와 비교, 오른쪽 왼쪽 바뀌어있을수있으니 2번검사, 히든은 무기와 사이드의 조합비교
            if (characterStatus.weaponCombo == itemDatabase.wcCombos[i].recipe[0] &&
                characterStatus.sideWeaponCombo == itemDatabase.wcCombos[i].recipe[1])
            {
                characterStatus.SetHiddenCombo(itemDatabase.wcCombos[i].wcNum);//캐릭터에 정보 추가
                weaponCombo.sprite = itemDatabase.wcCombos[i].wcImage.sprite; //인벤토리에 이미지 입력   
                return;
            }
            else if (characterStatus.weaponCombo == itemDatabase.wcCombos[i].recipe[1] &&
                characterStatus.sideWeaponCombo == itemDatabase.wcCombos[i].recipe[0])
            {
                characterStatus.SetHiddenCombo(itemDatabase.wcCombos[i].wcNum);
                weaponCombo.sprite = itemDatabase.wcCombos[i].wcImage.sprite; //인벤토리에 이미지 입력
                return;
            }
            else //조합이 아닐때
            {
                characterStatus.SetHiddenCombo(-1);
                PutWeaponCombo();
            }
        }
    }
}
