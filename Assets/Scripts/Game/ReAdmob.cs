using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using GoogleMobileAds.Api;
using BackEnd;

public class ReAdmob : MonoBehaviour
{
    private InterstitialAd interstitial;
    private bool isAD = false;

    // Start is called before the first frame update
    void Start()
    {
        ConfirmPurchase();

        if (!isAD)
            RequestInterestitial();
    }

    private void RequestInterestitial()
    {
        string adUnitId = "ca-app-pub-9037965244963922/6602036672";

        this.interstitial = new InterstitialAd(adUnitId);
        this.interstitial.OnAdClosed += HandleOnAdClosed;

        AdRequest request = new AdRequest.Builder().Build();

        this.interstitial.LoadAd(request);
    }

    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        Time.timeScale = 0;
    }

    public void GameOver()
    {
        if (this.interstitial.IsLoaded())
        {
            this.interstitial.Show();
        }
    }

    private void ConfirmPurchase()
    {
        var bro = Backend.GameData.GetMyData("Purchase", new Where(), 10);
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
            string tempAD = bro.Rows()[i]["ADPurchase"]["BOOL"].ToString();
            if (tempAD == "True")
                isAD = true;
            else
                isAD = false;
        }
    }
}
