using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using BackEnd;
using UnityEngine.UI;

public class Admob : MonoBehaviour
{
    private BannerView bannerView;
    public bool isAD = false; //���� ���� ��������
    private bool isONAD = false; //���� ������������

    // Start is called before the first frame update
    void Start()
    {
        MobileAds.Initialize(initStatus => { });
    }

    private void Update()
    {
        if (!isAD && !isONAD)
            this.RequestBanner();

        if (isAD && isONAD)
        {
            bannerView.Hide();
            isONAD = false;
        }
    }

    void OnDisable()
    {
        bannerView.Hide();
    }

    private void RequestBanner()
    {
        string adUnitID = "ca-app-pub-9037965244963922/4213329444";

        if (this.bannerView != null)
            this.bannerView.Destroy();

        AdSize adaptiveSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);

        this.bannerView = new BannerView(adUnitID, adaptiveSize, AdPosition.Bottom);

        AdRequest request = new AdRequest.Builder().Build();

        this.bannerView.LoadAd(request);
        isONAD = true;
    }

    /*
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
    */
}
