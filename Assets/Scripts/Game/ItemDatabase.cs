using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Item
{
    public string itemName;//아이템 이름
    public string itemDesc;//아이템 설명
    public ItemType itemType; //아이템 타입
    public int itemNum; // 아이템 고유번호
    public Image itemImage; //아이템 이미지

    public enum ItemType
    {
        Weapon,
        Equipment
    }
}

[System.Serializable]
public class WeaponCombo
{
    public string wcName;//콤보 이름
    public string wcDesc;//콤보 설명
    public int wcNum; // 콤보 고유번호
    public int[] recipe = new int[2]; //조합법
    public Image wcImage; //콤보 이미지
}

[System.Serializable]
public class EquipCombo
{
    public string ecName;//콤보 이름
    public string ecDesc;//콤보 설명
    public int ecNum; // 콤보 고유번호
    public int[] recipe = new int[2]; //조합법
    public Image ecImage; //콤보 이미지
}

public class ItemDatabase : MonoBehaviour
{
    public List<Item> items = new List<Item>();
    public List<WeaponCombo> wcCombos = new List<WeaponCombo>();
    public List<EquipCombo> ecCombos = new List<EquipCombo>();
}
