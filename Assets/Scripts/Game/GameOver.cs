using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using BackEnd;

public class GameOver : MonoBehaviour
{
    public Text scoreNum;
    public int totalScore= 0; //총 점수

    public string id; //임시로 아이디 얻어오기
    public int point = 0; //얻게되는 포인트
    public bool isPt = false; //pt부스트 상태인지
    public float weekScore; //주간 최고점수
    public float highScore; //최고점수
    public float plusPT; //게임 끝났을때 추가점수

    //상세 점수
    public float survivalScore = 0f;
    public float roundScore = 0f;
    public float killScore = 0f;
    public float dpsScore = 0f;
    public float hpScore = 0f;
    public float enemyScore = 0f;
    public float statScore = 0f;
    public float equipScore = 0f;

    public Text survivalScoreText;
    public Text roundScoreText;
    public Text killScoreText;
    public Text dpsScoreText;
    public Text hpScoreText;
    public Text enemyScoreText;
    public Text statScoreText;
    public Text equipScoreText;
    public Text getPtText;

    public Image weekScoreImg;
    public Image highScoreImg;

    private Inventory inventory;

    private void OnEnable()
    {
        inventory = FindObjectOfType<Inventory>();

        survivalScore = GameManager.instance.survivalScore;
        roundScore = GameManager.instance.roundScore;
        killScore = GameManager.instance.killScore;
        dpsScore = GameManager.instance.dpsScore;
        hpScore = GameManager.instance.hpScore;
        enemyScore = GameManager.instance.enemyScore;
        statScore = GameManager.instance.statScore;
        equipScore = GameManager.instance.equipScore;

        survivalScoreText.text = ((int)survivalScore).ToString();
        roundScoreText.text = ((int)roundScore).ToString();
        killScoreText.text = ((int)killScore).ToString();
        dpsScoreText.text = ((int)dpsScore).ToString();
        hpScoreText.text = ((int)hpScore).ToString();
        enemyScoreText.text = ((int)enemyScore).ToString();
        statScoreText.text = ((int)statScore).ToString();
        equipScoreText.text = ((int)equipScore).ToString();

        ConfirmPurchase(); //포인트부스트 구매했는지

        //포인트 보상
        point = GameManager.instance.round;
        if (isPt)
            point = point * 2;
        getPtText.text = point.ToString();

        ConfirmStatus(); //남은 포인트 합산
        ConfirmScore(); //점수 불러오기

        //최종점수
        totalScore = (int)survivalScore + (int)roundScore + (int)killScore + (int)dpsScore + (int)hpScore + (int)enemyScore + (int)statScore + (int)equipScore;
        totalScore += (int)(totalScore * plusPT / 100); //pluspt계산
        scoreNum.text = totalScore.ToString();




        if (totalScore > weekScore) //점수 갱신
        {
            weekScore = totalScore;
            weekScoreImg.gameObject.SetActive(true);
            GetUpdateUserWeekRankTest();//랭킹갱신
        }
        if (totalScore > highScore)
        {
            highScore = totalScore;
            highScoreImg.gameObject.SetActive(true);
            GetUpdateUserWeekRankTest();//랭킹갱신
            GetUpdateUserHighRankTest();
        }

        UpdateServer(); //서버 갱신
    }

    //메인메뉴로
    public void MainBack()
    {
        Time.timeScale = 1;
        AudioManager.instance.PlaySFX("Button");
        LoadingScene.LoadScene("Lobby");
    }

    //재시작
    public void GameRestart()
    {
        Time.timeScale = 1;
        AudioManager.instance.PlaySFX("Button");
        LoadingScene.LoadScene("Game");
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
            float tempPlusPt = float.Parse(bro.Rows()[i]["PlusPt"]["N"].ToString());
            float tempRemainPt = float.Parse(bro.Rows()[i]["RemainPt"]["N"].ToString());

            id = tempID;
            plusPT = tempPlusPt;
            point += (int)tempRemainPt; //업데이트를 위하여 포인트 합치기
        }
    }

    private void ConfirmScore()
    {
        var bro = Backend.GameData.GetMyData("HighScore", new Where(), 10);
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
            float tempWeek = float.Parse(bro.Rows()[i]["WeekScore"]["N"].ToString());
            float tempHigh = float.Parse(bro.Rows()[i]["HighScore"]["N"].ToString());

            weekScore = tempWeek;
            highScore = tempHigh;
        }
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
            string tempPT = bro.Rows()[i]["PTPurchase"]["BOOL"].ToString();
            if (tempPT == "True")
                isPt = true;
            else
                isPt = false;
        }
    }

    //주간 랭킹 갱신
    public void GetUpdateUserWeekRankTest()
    {
        string tableName = "HighScore";
        string rowIndate = string.Empty;
        string rankingUUID = "3d332a10-3581-11ed-8d90-5db245fad319";

        Param param = new Param();
        param.Add("WeekScore", weekScore);

        var bro = Backend.GameData.Get("HighScore", new Where());

        if (bro.IsSuccess())
        {
            if (bro.FlattenRows().Count > 0)
            {
                rowIndate = bro.FlattenRows()[0]["inDate"].ToString();
            }
            else
            {
                var bro2 = Backend.GameData.Insert(tableName, param);

                if (bro2.IsSuccess())
                {
                    rowIndate = bro2.GetInDate();
                }
                else
                {
                    return;
                }

            }
        }
        else
        {
            return;
        }

        if (rowIndate == string.Empty)
        {
            return;
        }

        var rankBro = Backend.URank.User.UpdateUserScore(rankingUUID, tableName, rowIndate, param);
        if (rankBro.IsSuccess())
        {
            //Debug.Log("랭킹 등록 성공");
        }
        else
        {
            //Debug.Log("랭킹 등록 실패 : " + rankBro);
        }
    }

    //하이 랭킹 갱신
    public void GetUpdateUserHighRankTest()
    {
        string tableName = "HighScore";
        string rowIndate = string.Empty;
        string rankingUUID = "14729530-452a-11ed-a6f5-9118577b4fb9";

        Param param = new Param();
        param.Add("HighScore", highScore);

        var bro = Backend.GameData.Get("HighScore", new Where());

        if (bro.IsSuccess())
        {
            if (bro.FlattenRows().Count > 0)
            {
                rowIndate = bro.FlattenRows()[0]["inDate"].ToString();
            }
            else
            {
                var bro2 = Backend.GameData.Insert(tableName, param);

                if (bro2.IsSuccess())
                {
                    rowIndate = bro2.GetInDate();
                }
                else
                {
                    return;
                }

            }
        }
        else
        {
            return;
        }

        if (rowIndate == string.Empty)
        {
            return;
        }

        var rankBro = Backend.URank.User.UpdateUserScore(rankingUUID, tableName, rowIndate, param);
        if (rankBro.IsSuccess())
        {
            //Debug.Log("랭킹 등록 성공");
        }
        else
        {
            //Debug.Log("랭킹 등록 실패 : " + rankBro);
        }
    }


    private void UpdateServer() //설정 데이터 서버에 저장
    {
        //정보 넣어주기     
        Param param = new Param();
        param.Add("RemainPt", point);

        Param param2 = new Param();
        param2.Add("WeekScore", weekScore);
        param2.Add("HighScore", highScore);

        Where where = new Where(); //id가 같은 곳에
        where.Equal("ID", id);

        Backend.GameData.Update("Status", where, param); //update
        Backend.GameData.Update("HighScore", where, param2); //update
    }
}
