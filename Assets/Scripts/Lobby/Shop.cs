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

        ConfirmStatus(); //서버 확인

        /*
        this.btnPT.onPurchaseComplete.AddListener(new UnityAction<Product>((product) =>
        {
            //Debug.LogFormat("포인트부스트 구매 성공 : ", product.transactionID);
            BoostBtn();
        }));

        this.btnPT.onPurchaseFailed.AddListener(new UnityAction<Product, PurchaseFailureReason>((product, reason) =>
        {
            //Debug.LogFormat("포인트부스트 구매 실패 : {0}, {1} ", product.transactionID, reason);
        }));

        this.btnAD.onPurchaseComplete.AddListener(new UnityAction<Product>((product) =>
        {
            //Debug.LogFormat("광고제거 구매 성공 : ", product.transactionID);
            ADBtn();
        }));

        this.btnAD.onPurchaseFailed.AddListener(new UnityAction<Product, PurchaseFailureReason>((product, reason) =>
        {
            //Debug.LogFormat("광고제거 구매 실패 : {0}, {1} ", product.transactionID, reason);
        }));

        this.btn100.onPurchaseComplete.AddListener(new UnityAction<Product>((product) =>
        {
            //Debug.LogFormat("포인트100 구매 성공 : ", product.transactionID);
            PTBtn();
        }));

        this.btn100.onPurchaseFailed.AddListener(new UnityAction<Product, PurchaseFailureReason>((product, reason) =>
        {
            //Debug.LogFormat("포인트100 구매 실패 : {0}, {1} ", product.transactionID, reason);
        }));
        */
    }

    public void CloseBtn()
    {
        Btns.SetActive(true);
        gameObject.SetActive(false);
    }

    public void PTBtn() //100포인트를 붙혀서 업데이트
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
            // 요청 실패 처리
            //Debug.Log(bro);
            return;
        }
        if (bro.GetReturnValuetoJSON()["rows"].Count <= 0)
        {
            // 요청이 성공해도 where 조건에 부합하는 데이터가 없을 수 있기 때문에
            // 데이터가 존재하는지 확인
            // 위와 같은 new Where() 조건의 경우 테이블에 row가 하나도 없으면 Count가 0 이하 일 수 있다.
            //Debug.Log(bro);
            return;
        }
        // 검색한 데이터의 모든 row의 inDate 값 확인
        for (int i = 0; i < bro.Rows().Count; ++i)
        {
            //남은 포인트 불러오기
            string tempID = bro.Rows()[i]["ID"]["S"].ToString();
            float tempRemainPt = float.Parse(bro.Rows()[i]["RemainPt"]["N"].ToString());

            id = tempID;
            point = (int)tempRemainPt; //업데이트를 위하여 포인트 합치기
        }
    }

    private void UpdatePoint() //설정 데이터 서버에 저장
    {
        //정보 넣어주기     
        Param param = new Param();
        param.Add("RemainPt", point);

        Where where = new Where(); //id가 같은 곳에
        where.Equal("ID", id);

        BackendReturnObject bro = Backend.GameData.Update("Status", where, param); //update

        if (bro.IsSuccess() == false)
        {
            // 서버에 에러가 리턴되었다면
            return;
        }

    }

    private void UpdatePtBoost() //설정 데이터 서버에 저장
    {
        //정보 넣어주기     
        Param param = new Param();
        param.Add("PTPurchase", true);

        Where where = new Where(); //id가 같은 곳에
        where.Equal("ID", id);

        BackendReturnObject bro = Backend.GameData.Update("Purchase", where, param); //update
        if (bro.IsSuccess() == false)
        {
            // 서버에 에러가 리턴되었다면
            return;
        }
    }

    private void UpdateAD() //설정 데이터 서버에 저장
    {
        //정보 넣어주기     
        Param param = new Param();
        param.Add("ADPurchase", true);

        Where where = new Where(); //id가 같은 곳에
        where.Equal("ID", id);

        BackendReturnObject bro = Backend.GameData.Update("Purchase", where, param); //update
        if (bro.IsSuccess() == false)
        {
            // 서버에 에러가 리턴되었다면
            return;
        }
    }
}
