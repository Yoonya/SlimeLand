using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Item
{
    public string itemName;//������ �̸�
    public string itemDesc;//������ ����
    public ItemType itemType; //������ Ÿ��
    public int itemNum; // ������ ������ȣ
    public Image itemImage; //������ �̹���

    public enum ItemType
    {
        Weapon,
        Equipment
    }
}

[System.Serializable]
public class WeaponCombo
{
    public string wcName;//�޺� �̸�
    public string wcDesc;//�޺� ����
    public int wcNum; // �޺� ������ȣ
    public int[] recipe = new int[2]; //���չ�
    public Image wcImage; //�޺� �̹���
}

[System.Serializable]
public class EquipCombo
{
    public string ecName;//�޺� �̸�
    public string ecDesc;//�޺� ����
    public int ecNum; // �޺� ������ȣ
    public int[] recipe = new int[2]; //���չ�
    public Image ecImage; //�޺� �̹���
}

public class ItemDatabase : MonoBehaviour
{
    public List<Item> items = new List<Item>();
    public List<WeaponCombo> wcCombos = new List<WeaponCombo>();
    public List<EquipCombo> ecCombos = new List<EquipCombo>();
}
