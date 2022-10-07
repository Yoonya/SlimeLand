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

    // GPGS �α��� 
    void Awake()
    {
        // GPGS �÷����� ����
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration
            .Builder()
            .RequestServerAuthCode(false)
            .RequestEmail() // �̸��� ������ ��� ���� �ʴٸ� �ش� ��(RequestEmail)�� �����ּ���.
            .RequestIdToken()
            .Build();
        //Ŀ���� �� ������ GPGS �ʱ�ȭ
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true; // ����� �α׸� ���� ���� �ʴٸ� false�� �ٲ��ּ���.
                                                  //GPGS ����.
        PlayGamesPlatform.Activate();
        GPGSLogin();
    }

    public void GPGSLogin()
    {
        // �̹� �α��� �� ���
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
                        // �α��� ���� -> �ڳ� ������ ȹ���� ���� ��ū���� ���� ��û
                        BackendReturnObject BRO = Backend.BMember.AuthorizeFederation(GetTokens(), FederationType.Google, "gpgs");
                    }
                    else
                    {
                        // ȸ������ ���� -> �ڳ� ������ ȹ���� ���� ��ū���� ���� ��û -> �ʱ� ȸ������ ����
                        BackendReturnObject BRO = Backend.BMember.AuthorizeFederation(GetTokens(), FederationType.Google, "gpgs");
                        SignUpInsert(GetEmail());
                    }
                    ConfirmPurchase();
                    loginText.text = "Google Play ON";
                }
                else
                {
                    // �α��� ����
                    //Debug.Log("Login failed for some reason");
                    loginText.text = "Google Play OFF";
                }
            });
        }
    }

    // �̹� ���Ե� �������� Ȯ��
    public bool OnClickCheckUserAuthenticate()
    {
        BackendReturnObject BRO = Backend.BMember.CheckUserInBackend(GetTokens(), FederationType.Google);
        if (BRO.GetStatusCode() == "200")
        {
            //Debug.Log("���� ���� �����Դϴ�.");

            // �ش� ���� ����
            //Debug.Log(BRO.GetReturnValue());
            return true;
        }

        else
        {
            //Debug.Log("���Ե� ������ �ƴմϴ�.");
            return false;
        }
    }

    // ���� ��ū �޾ƿ�
    public string GetTokens()
    {
        if (PlayGamesPlatform.Instance.localUser.authenticated)
        {
            // ���� ��ū �ޱ� ù ��° ���
            string _IDtoken = PlayGamesPlatform.Instance.GetIdToken();
            // �� ��° ���
            //string _IDtoken2 = ((PlayGamesLocalUser)Social.localUser).GetIdToken();
            return _IDtoken;
        }
        else
        {
            //Debug.Log("���ӵǾ� ���� �ʽ��ϴ�. PlayGamesPlatform.Instance.localUser.authenticated :  fail");
            return null;
        }
    }

    //���� �̸��� �޾ƿ�
    public string GetEmail()
    {
        if (PlayGamesPlatform.Instance.localUser.authenticated)
        {
            // ���� ��ū �ޱ� ù ��° ���
            string email = PlayGamesPlatform.Instance.GetUserEmail();
            // �� ��° ���
            //string email2 = ((PlayGamesLocalUser)Social.localUser).Email);
            return email;
        }
        else
        {
            Debug.Log("���ӵǾ� ���� �ʽ��ϴ�. PlayGamesPlatform.Instance.localUser.authenticated :  fail");
            return null;
        }
    }

    // ���� �����г��� �޾ƿ�
    public string GetUsername()
    {
        if (PlayGamesPlatform.Instance.localUser.authenticated)
        {
            // ���� ��ū �ޱ� ù ��° ���
            string userName = PlayGamesPlatform.Instance.GetUserDisplayName();
            // �� ��° ���
            //string userName = ((PlayGamesLocalUser)Social.localUser).userName;
            return userName;
        }
        else
        {
            //Debug.Log("���ӵǾ� ���� �ʽ��ϴ�. PlayGamesPlatform.Instance.localUser.authenticated :  fail");
            return null;
        }
    }

    private void SignUpInsert(string id) //���� ȸ������
    {
        //�ʱ� ���� �־��ֱ�     
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

        //������ ���� ����
        Backend.GameData.Insert("Status", param);
        //������ ���� ����
        Backend.GameData.Insert("Purchase", param2);
        //������ ���� ����
        Backend.GameData.Insert("HighScore", param3);
        //������ �г��� ����, �ʱ� �г����� ���� �̸��Ͽ��� ���̵�
        //string[] splitID = id.Split('@');
        Backend.BMember.CreateNickname(GetUsername());
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