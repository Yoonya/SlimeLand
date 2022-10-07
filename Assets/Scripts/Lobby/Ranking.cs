using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BackEnd;

public class RankUser
{
    public string inDate = "";
    public string nickName = "";
    public int lv = 1;
    public int rank = 0;
    public float weekScore = 0f;
    public float highScore = 0f;
    public RankUser(string inDate, string nickName, int lv, int rank, float weekScore, float highScore)
    {
        this.inDate = inDate;
        this.nickName = nickName;
        this.lv = lv;
        this.rank = rank;
        this.weekScore = weekScore;
        this.highScore = highScore;
    }
}

public class Ranking : MonoBehaviour
{
    public GameObject Btns;
    public GameObject weekFrame;
    public GameObject highFrame;

    RankUser[] rankUsers = new RankUser[10];
    RankUser myRank = new RankUser("", "", 1, 0 , 0f,0f);

    public Text myID;
    public Text myLV;
    public Text myRanking;
    public Text myScore;

    public GameObject[] rankUsersTxT = new GameObject[10];

    private void Awake() //onEnable보다 먼저
    {
        for (int i = 0; i < 10; i++)
            rankUsers[i] = new RankUser("", "", 1, 0, 0f, 0f);
    }

    private void OnEnable()
    {
        Btns.SetActive(false);
        SetHighRanking(); //초기화면은 하이랭킹
    }

    public void CloseBtn()
    {
        Btns.SetActive(true);
        gameObject.SetActive(false);
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
            int tempLV = int.Parse(bro.Rows()[i]["LV"]["N"].ToString());
            myRank.lv = tempLV;
        }

        var bro2 = Backend.GameData.GetMyData("HighScore", new Where(), 10);
        if (bro2.IsSuccess() == false)
        {
            //Debug.Log(bro2);
            return;
        }
        if (bro2.GetReturnValuetoJSON()["rows"].Count <= 0)
        {
            //Debug.Log(bro2);
            return;
        }
        // 검색한 데이터의 모든 row의 inDate 값 확인
        for (int i = 0; i < bro2.Rows().Count; ++i)
        {
            float tempWeekScore = int.Parse(bro2.Rows()[i]["WeekScore"]["N"].ToString());
            float tempHighScore = int.Parse(bro2.Rows()[i]["HighScore"]["N"].ToString());
            string tempDate = bro2.Rows()[i]["inDate"]["S"].ToString();

            myRank.weekScore = tempWeekScore;
            myRank.highScore = tempHighScore;
            myRank.inDate = tempDate;
        }
        BackendReturnObject bro3 = Backend.BMember.GetUserInfo();
        myRank.nickName = bro3.GetReturnValuetoJSON()["row"]["nickname"].ToString();
    }

    private void ConfirmMyWeekRank()
    {
        Param param = new Param();
        param.Add("WeekScore", myRank.weekScore);
        param.Add("LV", myRank.lv);

        Backend.URank.User.UpdateUserScore("3d332a10-3581-11ed-8d90-5db245fad319", "HighScore", myRank.inDate, param);

        var bro = Backend.URank.User.GetMyRank("3d332a10-3581-11ed-8d90-5db245fad319");
        if (bro.IsSuccess() == false)
        {
            //Debug.Log(bro);
            return;
        }
        if (bro.GetReturnValuetoJSON()["rows"].Count <= 0)
        {
            //Debug.Log(bro);
            return;
        }
        for (int i = 0; i < bro.Rows().Count; ++i)
        {
            int tempRank = int.Parse(bro.Rows()[i]["rank"]["N"].ToString());

            myRank.rank = tempRank;
        }
    }

    private void ConfirmUserWeekRank()
    {
        var bro = Backend.URank.User.GetRankList("3d332a10-3581-11ed-8d90-5db245fad319");
        if (bro.IsSuccess() == false)
        {
            //Debug.Log(bro);
            return;
        }
        if (bro.GetReturnValuetoJSON()["rows"].Count <= 0)
        {
            //Debug.Log(bro);
            return;
        }
        for (int i = 0; i < bro.Rows().Count; ++i)
        {
            string tempName = bro.Rows()[i]["nickname"]["S"].ToString();
            string tempDate = bro.Rows()[i]["gamerInDate"]["S"].ToString();
            int tempRank = int.Parse(bro.Rows()[i]["rank"]["N"].ToString());
            int tempLV = int.Parse(bro.Rows()[i]["LV"]["N"].ToString());
            float tempScore = float.Parse(bro.Rows()[i]["score"]["N"].ToString());

            rankUsers[i].nickName = tempName;
            rankUsers[i].inDate = tempDate;
            rankUsers[i].rank = tempRank;
            rankUsers[i].lv = tempLV;
            rankUsers[i].weekScore = tempScore;
        }

        for (int i = 0; i < bro.Rows().Count; ++i)
        {
            rankUsersTxT[i].transform.GetChild(0).GetComponent<Text>().text = rankUsers[i].nickName;
            rankUsersTxT[i].transform.GetChild(1).GetComponent<Text>().text = rankUsers[i].lv.ToString();
            rankUsersTxT[i].transform.GetChild(2).GetComponent<Text>().text = rankUsers[i].weekScore.ToString();
        }
    }


    private void ConfirmMyHighRank()
    {
        Param param = new Param();
        param.Add("HighScore", myRank.highScore);
        param.Add("LV", myRank.lv);

        Backend.URank.User.UpdateUserScore("14729530-452a-11ed-a6f5-9118577b4fb9", "HighScore", myRank.inDate, param);

        var bro = Backend.URank.User.GetMyRank("14729530-452a-11ed-a6f5-9118577b4fb9");
        if (bro.IsSuccess() == false)
        {
            //Debug.Log(bro);
            return;
        }
        if (bro.GetReturnValuetoJSON()["rows"].Count <= 0)
        {
            //Debug.Log(bro);
            return;
        }
        for (int i = 0; i < bro.Rows().Count; ++i)
        {
            int tempRank = int.Parse(bro.Rows()[i]["rank"]["N"].ToString());

            myRank.rank = tempRank;
        }
    }

    private void ConfirmUserHighRank()
    {
        var bro = Backend.URank.User.GetRankList("14729530-452a-11ed-a6f5-9118577b4fb9");
        if (bro.IsSuccess() == false)
        {
            //Debug.Log(bro);
            return;
        }
        if (bro.GetReturnValuetoJSON()["rows"].Count <= 0)
        {
            //Debug.Log(bro);
            return;
        }
        for (int i = 0; i < bro.Rows().Count; ++i)
        {
            string tempName = bro.Rows()[i]["nickname"]["S"].ToString();
            string tempDate = bro.Rows()[i]["gamerInDate"]["S"].ToString();
            int tempRank = int.Parse(bro.Rows()[i]["rank"]["N"].ToString());
            int tempLV = int.Parse(bro.Rows()[i]["LV"]["N"].ToString());
            float tempScore = float.Parse(bro.Rows()[i]["score"]["N"].ToString());

            rankUsers[i].nickName = tempName;
            rankUsers[i].inDate = tempDate;
            rankUsers[i].rank = tempRank;
            rankUsers[i].lv = tempLV;
            rankUsers[i].highScore = tempScore;
        }

        for (int i = 0; i < bro.Rows().Count; ++i)
        {
            rankUsersTxT[i].transform.GetChild(0).GetComponent<Text>().text = rankUsers[i].nickName;
            rankUsersTxT[i].transform.GetChild(1).GetComponent<Text>().text = rankUsers[i].lv.ToString();
            rankUsersTxT[i].transform.GetChild(2).GetComponent<Text>().text = rankUsers[i].highScore.ToString();
        }
    }

    public void SetWeekRanking()
    {
        weekFrame.SetActive(true);
        highFrame.SetActive(false);

        ClearFrame();

        ConfirmStatus();
        ConfirmMyWeekRank();
        ConfirmUserWeekRank();

        myID.text = myRank.nickName;
        myLV.text = myRank.lv.ToString();
        myRanking.text = myRank.rank.ToString();
        myScore.text = myRank.highScore.ToString();
    }

    public void SetHighRanking()
    {
        weekFrame.SetActive(false);
        highFrame.SetActive(true);

        ClearFrame();

        ConfirmStatus();
        ConfirmMyHighRank();
        ConfirmUserHighRank();

        myID.text = myRank.nickName;
        myLV.text = myRank.lv.ToString();
        myRanking.text = myRank.rank.ToString();
        myScore.text = myRank.highScore.ToString();
    }

    private void ClearFrame()
    {
        for (int i = 0; i < rankUsers.Length; i++)
            rankUsers[i] = new RankUser("", "", 1, 0, 0f, 0f);

        for (int i = 0; i < rankUsersTxT.Length; ++i)
        {
            rankUsersTxT[i].transform.GetChild(0).GetComponent<Text>().text = rankUsers[i].nickName;
            rankUsersTxT[i].transform.GetChild(1).GetComponent<Text>().text = rankUsers[i].lv.ToString();
            rankUsersTxT[i].transform.GetChild(2).GetComponent<Text>().text = rankUsers[i].highScore.ToString();
        }
    }
}
