using BackEnd;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackEndFederationAuth : MonoBehaviour
{
    public Text loginText;
    public Text ADPurchase;
    public Text PTPurchase;

    public GameObject admob;

    // GPGS 로그인 
    void Awake()
    {
        // GPGS 플러그인 설정
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration
            .Builder()
            .RequestServerAuthCode(false)
            .RequestEmail() // 이메일 권한을 얻고 싶지 않다면 해당 줄(RequestEmail)을 지워주세요.
            .RequestIdToken()
            .Build();
        //커스텀 된 정보로 GPGS 초기화
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true; // 디버그 로그를 보고 싶지 않다면 false로 바꿔주세요.
                                                  //GPGS 시작.
        PlayGamesPlatform.Activate();
        GPGSLogin();
    }

    public void GPGSLogin()
    {
        // 이미 로그인 된 경우
        if (Social.localUser.authenticated == true)
        {
            BackendReturnObject BRO = Backend.BMember.AuthorizeFederation(GetTokens(), FederationType.Google, "gpgs");
            loginText.text = "Google Play ON";
            ConfirmPurchase(); 
        }
        else
        {
            Social.localUser.Authenticate((bool success, string error) => {
                if (success)
                {
                    if (OnClickCheckUserAuthenticate())
                    {
                        // 로그인 성공 -> 뒤끝 서버에 획득한 구글 토큰으로 가입 요청
                        BackendReturnObject BRO = Backend.BMember.AuthorizeFederation(GetTokens(), FederationType.Google, "gpgs");
                    }
                    else
                    {
                        // 회원가입 성공 -> 뒤끝 서버에 획득한 구글 토큰으로 가입 요청 -> 초기 회원정보 삽입
                        BackendReturnObject BRO = Backend.BMember.AuthorizeFederation(GetTokens(), FederationType.Google, "gpgs");
                        SignUpInsert(GetEmail());
                    }
                    ConfirmPurchase();
                    loginText.text = "Google Play ON";
                }
                else
                {
                    // 로그인 실패
                    //Debug.Log("Login failed for some reason");
                    loginText.text = "Google Play OFF";
                }
            });
        }
    }

    // 이미 가입된 상태인지 확인
    public bool OnClickCheckUserAuthenticate()
    {
        BackendReturnObject BRO = Backend.BMember.CheckUserInBackend(GetTokens(), FederationType.Google);
        if (BRO.GetStatusCode() == "200")
        {
            //Debug.Log("가입 중인 계정입니다.");

            // 해당 계정 정보
            //Debug.Log(BRO.GetReturnValue());
            return true;
        }

        else
        {
            //Debug.Log("가입된 계정이 아닙니다.");
            return false;
        }
    }

    // 구글 토큰 받아옴
    public string GetTokens()
    {
        if (PlayGamesPlatform.Instance.localUser.authenticated)
        {
            // 유저 토큰 받기 첫 번째 방법
            string _IDtoken = PlayGamesPlatform.Instance.GetIdToken();
            // 두 번째 방법
            //string _IDtoken2 = ((PlayGamesLocalUser)Social.localUser).GetIdToken();
            return _IDtoken;
        }
        else
        {
            //Debug.Log("접속되어 있지 않습니다. PlayGamesPlatform.Instance.localUser.authenticated :  fail");
            return null;
        }
    }

    //구글 이메일 받아옴
    public string GetEmail()
    {
        if (PlayGamesPlatform.Instance.localUser.authenticated)
        {
            // 유저 토큰 받기 첫 번째 방법
            string email = PlayGamesPlatform.Instance.GetUserEmail();
            // 두 번째 방법
            //string email2 = ((PlayGamesLocalUser)Social.localUser).Email);
            return email;
        }
        else
        {
            Debug.Log("접속되어 있지 않습니다. PlayGamesPlatform.Instance.localUser.authenticated :  fail");
            return null;
        }
    }

    // 구글 유저닉네임 받아옴
    public string GetUsername()
    {
        if (PlayGamesPlatform.Instance.localUser.authenticated)
        {
            // 유저 토큰 받기 첫 번째 방법
            string userName = PlayGamesPlatform.Instance.GetUserDisplayName();
            // 두 번째 방법
            //string userName = ((PlayGamesLocalUser)Social.localUser).userName;
            return userName;
        }
        else
        {
            //Debug.Log("접속되어 있지 않습니다. PlayGamesPlatform.Instance.localUser.authenticated :  fail");
            return null;
        }
    }

    private void SignUpInsert(string id) //최초 회원가입
    {
        //초기 정보 넣어주기     
        Param param = new Param();
        param.Add("ID", id);
        param.Add("HP", 100f);
        param.Add("Atk", 100f);
        param.Add("Def", 0f);
        param.Add("AtkSpd", 1f);
        param.Add("Spd", 5f);
        param.Add("Crit", 0f);
        param.Add("Hps", 0f);
        param.Add("PlusPt", 0f);
        param.Add("TotalPt", 0f);
        param.Add("RemainPt", 0f);
        param.Add("LV", 1);
        param.Add("FirstGame", true);

        Param param2 = new Param();
        param2.Add("ID", id);
        param2.Add("ADPurchase", false);
        param2.Add("PTPurchase", false);

        Param param3 = new Param();
        param3.Add("ID", id);
        param3.Add("LV", 1);
        param3.Add("HighScore", 0f);
        param3.Add("WeekScore", 0f);

        //유저의 정보 생성
        Backend.GameData.Insert("Status", param);
        //유저의 정보 생성
        Backend.GameData.Insert("Purchase", param2);
        //유저의 정보 생성
        Backend.GameData.Insert("HighScore", param3);
        //유저의 닉네임 생성, 초기 닉네임은 구글 이메일에서 아이디
        //string[] splitID = id.Split('@');
        Backend.BMember.CreateNickname(GetUsername());
    }

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
            string tempPT = bro.Rows()[i]["PTPurchase"]["BOOL"].ToString();

            if (tempAD == "True")
            {
                admob.GetComponent<Admob>().isAD = true;
                ADPurchase.text = "NO ADS ON";
            }
            else
            {
                admob.GetComponent<Admob>().isAD = false;
                ADPurchase.text = "NO ADS OFF";
            }

            if (tempPT == "True")
                PTPurchase.text = "PT BOOST ON";
            else
                PTPurchase.text = "PT BOOST OFF";


        }
    }

}