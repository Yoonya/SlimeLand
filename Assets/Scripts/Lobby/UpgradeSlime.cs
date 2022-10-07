using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BackEnd;

public class UpgradeSlime : MonoBehaviour
{
    //상태창
    public float hp = 100;
    public float atk = 100;
    public float def = 0;
    public float atkSpd = 1f;
    public float MaxAtkSpd = 0.1f;
    public float spd = 5;
    public float MaxSpd = 15;
    public float crit = 0;
    public float hps = 0; //초당 체력 회복
    public float plusPt = 0;

    public Text nameText;
    public Text idText;
    public Text hpText;
    public Text atkText;
    public Text defText;
    public Text atkSpdText;
    public Text spdText;
    public Text critText;
    public Text hpsText;
    public Text plusPtText;
    public Text highScoreText;

    //포인트
    public int point = 0;
    public int lv = 1; //포인트 누른 횟수
    public int needPoint = 0;
    public Text pointText;
    public Text lvText;
    public Text needPointText;

    //버튼
    public Button[] upBtns;

    //스탯창키면 비활성화할것들
    public GameObject logo; 
    public GameObject btns;

    //도움말
    public GameObject help;
    public bool isHelp = false;

    private void OnEnable()
    {
        logo.SetActive(false);
        btns.SetActive(false);

        ConfirmStatus();
        ConfirmHighScore();
    }

    private void Update()
    {
        if (point >= needPoint) //버튼활성화 비활성화
        {
            for (int i = 0; i < upBtns.Length; i++)
            {
                upBtns[i].interactable = true;
            }
        }
        else
        {
            for (int i = 0; i < upBtns.Length; i++)
            {
                upBtns[i].interactable = false;
            }
        }
    }

    public void ExitBtn() //종료버튼
    {
        AudioManager.instance.PlaySFX("Button");
        UpdateStatus();
        logo.SetActive(true);
        btns.SetActive(true);
        help.SetActive(false);
        gameObject.SetActive(false);
    }

    public void HelpBtn()
    {
        AudioManager.instance.PlaySFX("Button");
        isHelp = true;
        help.SetActive(true);
    }

    public void HelpCloseBtn()
    {
        AudioManager.instance.PlaySFX("Button");
        isHelp = false;
        help.SetActive(false);
    }

    public void HPBtn()
    {
        if (!isHelp)
        {
            point -= needPoint;
            hp += 2;
            lv += 1;
            needPoint = lv * 10;

            hpText.text = hp.ToString();
            lvText.text = lv.ToString();
            pointText.text = point.ToString();
            needPointText.text = needPoint.ToString();
        }
    }

    public void AtkBtn()
    {
        if (!isHelp)
        {
            point -= needPoint;
            atk += 2;
            lv += 1;
            needPoint = lv * 10;

            atkText.text = atk.ToString();
            lvText.text = lv.ToString();
            pointText.text = point.ToString();
            needPointText.text = needPoint.ToString();
        }
    }

    public void AtkSpdBtn()
    {
        if (!isHelp)
        {
            point -= needPoint;
            atkSpd -= 0.02f;
            lv += 1;
            needPoint = lv * 10;

            atkSpdText.text = atkSpd.ToString("F2");
            lvText.text = lv.ToString();
            pointText.text = point.ToString();
            needPointText.text = needPoint.ToString();
        }
    }

    public void CritBtn()
    {
        if (!isHelp)
        {
            point -= needPoint;
            crit += 1;
            lv += 1;
            needPoint = lv * 10;

            critText.text = crit.ToString();
            lvText.text = lv.ToString();
            pointText.text = point.ToString();
            needPointText.text = needPoint.ToString();
        }
    }

    public void DefBtn()
    {
        if (!isHelp)
        {
            point -= needPoint;
            def += 1;
            lv += 1;
            needPoint = lv * 10;

            defText.text = def.ToString();
            lvText.text = lv.ToString();
            pointText.text = point.ToString();
            needPointText.text = needPoint.ToString();
        }
    }

    public void SpdBtn()
    {
        if (!isHelp)
        {
            point -= needPoint;
            spd += 0.1f;
            lv += 1;
            needPoint = lv * 10;

            spdText.text = spd.ToString("F1");
            lvText.text = lv.ToString();
            pointText.text = point.ToString();
            needPointText.text = needPoint.ToString();
        }
    }

    public void PlusPtBtn()
    {
        if (!isHelp)
        {
            point -= needPoint;
            plusPt += 1;
            lv += 1;
            needPoint = lv * 10;

            plusPtText.text = plusPt.ToString();
            lvText.text = lv.ToString();
            pointText.text = point.ToString();
            needPointText.text = needPoint.ToString();
        }
    }

    public void HpsBtn()
    {
        if (!isHelp)
        {
            point -= needPoint;
            hps += 0.1f;
            lv += 1;
            needPoint = lv * 10;

            hpsText.text = hps.ToString("F1");
            lvText.text = lv.ToString();
            pointText.text = point.ToString();
            needPointText.text = needPoint.ToString();
        }
    }

    public void ResetBtn()
    {
        AudioManager.instance.PlaySFX("Button");
        for (int i = 0; i < lv; i++)
        {
            point += i * 10;
        }

        lv = 1;
        needPoint = lv * 10;

        pointText.text = point.ToString();
        lvText.text = lv.ToString();
        needPointText.text = needPoint.ToString();

        hp = 100;
        atk = 100;
        def = 0;
        atkSpd = 1f;
        spd = 5;
        crit = 0;
        hps = 0; 
        plusPt = 0;

        hpText.text = hp.ToString();
        atkText.text = atk.ToString();
        defText.text = def.ToString();
        atkSpdText.text = atkSpd.ToString("F2");
        spdText.text = spd.ToString("F1");
        critText.text = crit.ToString();
        hpsText.text = hps.ToString("F1");
        plusPtText.text = plusPt.ToString();
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
            string tempID = bro.Rows()[i]["ID"]["S"].ToString();
            float tempHP = float.Parse(bro.Rows()[i]["HP"]["N"].ToString());
            float tempAtk = float.Parse(bro.Rows()[i]["Atk"]["N"].ToString());
            float tempDef = float.Parse(bro.Rows()[i]["Def"]["N"].ToString());
            float tempAtkSpd = float.Parse(bro.Rows()[i]["AtkSpd"]["N"].ToString());
            float tempSpd = float.Parse(bro.Rows()[i]["Spd"]["N"].ToString());
            float tempCrit = float.Parse(bro.Rows()[i]["Crit"]["N"].ToString());
            float tempHps = float.Parse(bro.Rows()[i]["Hps"]["N"].ToString());
            float tempPlusPt = float.Parse(bro.Rows()[i]["PlusPt"]["N"].ToString());
            float tempRemainPt = float.Parse(bro.Rows()[i]["RemainPt"]["N"].ToString());
            int tempLV = int.Parse(bro.Rows()[i]["LV"]["N"].ToString());

            hp = tempHP;
            atk = tempAtk;
            def = tempDef;
            atkSpd = tempAtkSpd;
            MaxAtkSpd = 0.1f;
            spd = tempSpd;
            MaxSpd = 15f;
            crit = tempCrit;
            hps = tempHps; //초당 체력 회복
            plusPt = tempPlusPt;
            lv = tempLV;
            point = (int)tempRemainPt;

            idText.text = tempID;
            hpText.text = hp.ToString();
            atkText.text = atk.ToString();
            defText.text = def.ToString();
            atkSpdText.text = atkSpd.ToString("F2");
            spdText.text = spd.ToString("F1");
            critText.text = crit.ToString();
            hpsText.text = hps.ToString("F1");
            plusPtText.text = plusPt.ToString();

            needPoint = lv * 10;
            needPointText.text = needPoint.ToString();
            pointText.text = point.ToString();
            lvText.text = lv.ToString();
        }
        BackendReturnObject bro2 = Backend.BMember.GetUserInfo();
        nameText.text = bro2.GetReturnValuetoJSON()["row"]["nickname"].ToString();
    }

    private void ConfirmHighScore()
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
            float tempScore = float.Parse(bro.Rows()[i]["HighScore"]["N"].ToString());

            highScoreText.text = ((int)tempScore).ToString();
        }
    }

    private void UpdateStatus() //설정 데이터 서버에 저장
    {
        //초기 정보 넣어주기     
        Param param = new Param();
        param.Add("ID", idText.text);
        param.Add("HP", hp);
        param.Add("Atk", atk);
        param.Add("Def", def);
        param.Add("AtkSpd", atkSpd);
        param.Add("Spd", spd);
        param.Add("Crit", crit);
        param.Add("Hps", hps);
        param.Add("PlusPt", plusPt);
        param.Add("RemainPt", point);
        param.Add("LV", lv);

        Where where = new Where(); //id가 같은 곳에
        where.Equal("ID", idText.text);

        Backend.GameData.Update("Status", where, param); //update
    }
}
