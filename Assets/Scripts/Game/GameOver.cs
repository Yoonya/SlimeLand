using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using BackEnd;

public class GameOver : MonoBehaviour
{
    public Text scoreNum;
    public int totalScore= 0; //�� ����

    public string id; //�ӽ÷� ���̵� ������
    public int point = 0; //��ԵǴ� ����Ʈ
    public bool isPt = false; //pt�ν�Ʈ ��������
    public float weekScore; //�ְ� �ְ�����
    public float highScore; //�ְ�����
    public float plusPT; //���� �������� �߰�����

    //�� ����
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

        ConfirmPurchase(); //����Ʈ�ν�Ʈ �����ߴ���

        //����Ʈ ����
        point = GameManager.instance.round;
        if (isPt)
            point = point * 2;
        getPtText.text = point.ToString();

        ConfirmStatus(); //���� ����Ʈ �ջ�
        ConfirmScore(); //���� �ҷ�����

        //��������
        totalScore = (int)survivalScore + (int)roundScore + (int)killScore + (int)dpsScore + (int)hpScore + (int)enemyScore + (int)statScore + (int)equipScore;
        totalScore += (int)(totalScore * plusPT / 100); //pluspt���
        scoreNum.text = totalScore.ToString();




        if (totalScore > weekScore) //���� ����
        {
            weekScore = totalScore;
            weekScoreImg.gameObject.SetActive(true);
            GetUpdateUserWeekRankTest();//��ŷ����
        }
        if (totalScore > highScore)
        {
            highScore = totalScore;
            highScoreImg.gameObject.SetActive(true);
            GetUpdateUserWeekRankTest();//��ŷ����
            GetUpdateUserHighRankTest();
        }

        UpdateServer(); //���� ����
    }

    //���θ޴���
    public void MainBack()
    {
        Time.timeScale = 1;
        AudioManager.instance.PlaySFX("Button");
        LoadingScene.LoadScene("Lobby");
    }

    //�����
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
            float tempPlusPt = float.Parse(bro.Rows()[i]["PlusPt"]["N"].ToString());
            float tempRemainPt = float.Parse(bro.Rows()[i]["RemainPt"]["N"].ToString());

            id = tempID;
            plusPT = tempPlusPt;
            point += (int)tempRemainPt; //������Ʈ�� ���Ͽ� ����Ʈ ��ġ��
        }
    }

    private void ConfirmScore()
    {
        var bro = Backend.GameData.GetMyData("HighScore", new Where(), 10);
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
            string tempPT = bro.Rows()[i]["PTPurchase"]["BOOL"].ToString();
            if (tempPT == "True")
                isPt = true;
            else
                isPt = false;
        }
    }

    //�ְ� ��ŷ ����
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
            //Debug.Log("��ŷ ��� ����");
        }
        else
        {
            //Debug.Log("��ŷ ��� ���� : " + rankBro);
        }
    }

    //���� ��ŷ ����
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
            //Debug.Log("��ŷ ��� ����");
        }
        else
        {
            //Debug.Log("��ŷ ��� ���� : " + rankBro);
        }
    }


    private void UpdateServer() //���� ������ ������ ����
    {
        //���� �־��ֱ�     
        Param param = new Param();
        param.Add("RemainPt", point);

        Param param2 = new Param();
        param2.Add("WeekScore", weekScore);
        param2.Add("HighScore", highScore);

        Where where = new Where(); //id�� ���� ����
        where.Equal("ID", id);

        Backend.GameData.Update("Status", where, param); //update
        Backend.GameData.Update("HighScore", where, param2); //update
    }
}
