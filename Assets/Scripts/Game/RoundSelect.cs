using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//여기서 라운드 선택 창을 관리
[System.Serializable]
public class ItemBtn
{
    public Image img; //아이템 이미지
    public Text name; //아이템 이름
}

public class RoundSelect : MonoBehaviour
{
    public ItemBtn weaponBtn1;
    public ItemBtn weaponBtn2;
    public ItemBtn weaponBtn3;
    public ItemBtn equipBtn1;
    public ItemBtn equipBtn2;
    public ItemBtn equipBtn3;
    public Text statBtn1;
    public Text statBtn2;
    public Text statBtn3;

    int rand1 = 0;
    int rand2 = 0;
    int rand3 = 0;

    private ItemDatabase itemDatabase;
    private Inventory inventory;

    // Start is called before the first frame update
    void Start()
    {
        itemDatabase = FindObjectOfType<ItemDatabase>();
        inventory = FindObjectOfType<Inventory>();
    }
    //스탯 선택창
    public void StatSelect()
    {
        Randoming(0, 6);

        switch (rand1)
        {
            case 0 :
                statBtn1.text = "MaxHP + 30";
                break;
            case 1:
                statBtn1.text = "ATK + 20";
                break;
            case 2:
                statBtn1.text = "DEF + 10";
                break;
            case 3:
                statBtn1.text = "SPD + 0.5";
                break;
            case 4:
                statBtn1.text = "ASPD-0.05";
                break;
            case 5:
                statBtn1.text = "CRI + 5";
                break;
        }

        switch (rand2)
        {
            case 0:
                statBtn2.text = "MaxHP + 30";
                break;
            case 1:
                statBtn2.text = "ATK + 20";
                break;
            case 2:
                statBtn2.text = "DEF + 10";
                break;
            case 3:
                statBtn2.text = "SPD + 0.5";
                break;
            case 4:
                statBtn2.text = "ASPD-0.05";
                break;
            case 5:
                statBtn2.text = "CRI + 5";
                break;
        }

        switch (rand3)
        {
            case 0:
                statBtn3.text = "MaxHP + 30";
                break;
            case 1:
                statBtn3.text = "ATK + 20";
                break;
            case 2:
                statBtn3.text = "DEF + 10";
                break;
            case 3:
                statBtn3.text = "SPD + 0.5";
                break;
            case 4:
                statBtn3.text = "ASPD-0.05";
                break;
            case 5:
                statBtn3.text = "CRI + 5";
                break;
        }
    }
    //무기 선택창
    public void WeaponSelect()
    {
        //랜덤 숫자지정
        Randoming(0, 6);

        //1번 버튼에 넣어주기
        PutWeaponBtn(weaponBtn1, rand1);

        //2번 버튼에 넣어주기
        PutWeaponBtn(weaponBtn2, rand2);

        //3번 버튼에 넣어주기
        PutWeaponBtn(weaponBtn3, rand3);
    }

    public void PutWeaponBtn(ItemBtn itemBtn, int rand)
    {
        itemBtn.img.sprite = itemDatabase.items[rand].itemImage.sprite;
        itemBtn.name.text = itemDatabase.items[rand].itemName;
    }
    //장비 선택창
    public void EquipSelect()
    {
        //랜덤 숫자지정
        Randoming(6, 12);

        //1번 버튼에 넣어주기
        PutEquipBtn(equipBtn1, rand1);

        //2번 버튼에 넣어주기
        PutEquipBtn(equipBtn2, rand2);

        //3번 버튼에 넣어주기
        PutEquipBtn(equipBtn3, rand3);
    }

    public void PutEquipBtn(ItemBtn itemBtn, int rand)
    {
        itemBtn.img.sprite = itemDatabase.items[rand].itemImage.sprite;
        itemBtn.name.text = itemDatabase.items[rand].itemName;
    }
             
    public void Randoming(int num1, int num2)
    {
        //랜덤 숫자지정
        rand1 = Random.Range(num1, num2);
        rand2 = Random.Range(num1, num2);
        rand3 = Random.Range(num1, num2);

        if (rand1 == rand2)
        {
            while (true)
            {
                rand2 = Random.Range(num1, num2);
                if (rand1 != rand2)
                    break;
            }
        }
        if (rand1 == rand3 || rand2 == rand3)
        {
            while (true)
            {
                rand3 = Random.Range(num1, num2);
                if (rand1 != rand3 && rand2 != rand3)
                    break;
            }
        }
    }
}
