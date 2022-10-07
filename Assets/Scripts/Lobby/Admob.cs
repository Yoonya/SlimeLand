using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using BackEnd;
using UnityEngine.UI;

public class Admob : MonoBehaviour
{
    private BannerView bannerView;
    public bool isAD = false; //광고 구매 상태인지
    private bool isONAD = false; //광고가 켜진상태인지

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
            string tempAD = bro.Rows()[i]["ADPurchase"]["BOOL"].ToString();
            if (tempAD == "True")
                isAD = true;
            else
                isAD = false;
        }
    }
    */
}
