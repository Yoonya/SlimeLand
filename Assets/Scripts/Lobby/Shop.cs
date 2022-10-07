using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BackEnd;
using UnityEngine.Purchasing;
using UnityEngine.Events;

public class Shop : MonoBehaviour
{
    public GameObject Btns;
    public IAPButton btnPT;
    public IAPButton btnAD;
    public IAPButton btn100;

    private string id;
    private int point;

    private void OnEnable()
    {
        Btns.SetActive(false);

        ConfirmStatus(); //���� Ȯ��

        /*
        this.btnPT.onPurchaseComplete.AddListener(new UnityAction<Product>((product) =>
        {
            //Debug.LogFormat("����Ʈ�ν�Ʈ ���� ���� : ", product.transactionID);
            BoostBtn();
        }));

        this.btnPT.onPurchaseFailed.AddListener(new UnityAction<Product, PurchaseFailureReason>((product, reason) =>
        {
            //Debug.LogFormat("����Ʈ�ν�Ʈ ���� ���� : {0}, {1} ", product.transactionID, reason);
        }));

        this.btnAD.onPurchaseComplete.AddListener(new UnityAction<Product>((product) =>
        {
            //Debug.LogFormat("�������� ���� ���� : ", product.transactionID);
            ADBtn();
        }));

        this.btnAD.onPurchaseFailed.AddListener(new UnityAction<Product, PurchaseFailureReason>((product, reason) =>
        {
            //Debug.LogFormat("�������� ���� ���� : {0}, {1} ", product.transactionID, reason);
        }));

        this.btn100.onPurchaseComplete.AddListener(new UnityAction<Product>((product) =>
        {
            //Debug.LogFormat("����Ʈ100 ���� ���� : ", product.transactionID);
            PTBtn();
        }));

        this.btn100.onPurchaseFailed.AddListener(new UnityAction<Product, PurchaseFailureReason>((product, reason) =>
        {
            //Debug.LogFormat("����Ʈ100 ���� ���� : {0}, {1} ", product.transactionID, reason);
        }));
        */
    }

    public void CloseBtn()
    {
        Btns.SetActive(true);
        gameObject.SetActive(false);
    }

    public void PTBtn() //100����Ʈ�� ������ ������Ʈ
    {
        point += 100;
        UpdatePoint();
    }

    public void BoostBtn()
    {
        UpdatePtBoost();
    }

    public void ADBtn()
    {
        UpdateAD();
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
            //���� ����Ʈ �ҷ�����
            string tempID = bro.Rows()[i]["ID"]["S"].ToString();
            float tempRemainPt = float.Parse(bro.Rows()[i]["RemainPt"]["N"].ToString());

            id = tempID;
            point = (int)tempRemainPt; //������Ʈ�� ���Ͽ� ����Ʈ ��ġ��
        }
    }

    private void UpdatePoint() //���� ������ ������ ����
    {
        //���� �־��ֱ�     
        Param param = new Param();
        param.Add("RemainPt", point);

        Where where = new Where(); //id�� ���� ����
        where.Equal("ID", id);

        BackendReturnObject bro = Backend.GameData.Update("Status", where, param); //update

        if (bro.IsSuccess() == false)
        {
            // ������ ������ ���ϵǾ��ٸ�
            return;
        }

    }

    private void UpdatePtBoost() //���� ������ ������ ����
    {
        //���� �־��ֱ�     
        Param param = new Param();
        param.Add("PTPurchase", true);

        Where where = new Where(); //id�� ���� ����
        where.Equal("ID", id);

        BackendReturnObject bro = Backend.GameData.Update("Purchase", where, param); //update
        if (bro.IsSuccess() == false)
        {
            // ������ ������ ���ϵǾ��ٸ�
            return;
        }
    }

    private void UpdateAD() //���� ������ ������ ����
    {
        //���� �־��ֱ�     
        Param param = new Param();
        param.Add("ADPurchase", true);

        Where where = new Where(); //id�� ���� ����
        where.Equal("ID", id);

        BackendReturnObject bro = Backend.GameData.Update("Purchase", where, param); //update
        if (bro.IsSuccess() == false)
        {
            // ������ ������ ���ϵǾ��ٸ�
            return;
        }
    }
}
