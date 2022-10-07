using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using BackEnd;

public class GameManager : MonoBehaviour
{
    public float roundTime = 0f; // 라운드는 총 60초 업데이트에서 시작하기 위해 0으로 시작
    public int round = 0; //라운드 수
    public int enemy = 0; //적 수
    public float totalDamage = 0f;
    public float totalTime = 0f;
    public float totalScore = 0; //총 점수

    public bool isFirst = true; //첫판인지
    public string id;//회원 아이디

    //난이도 업시키는 변수
    private int roundRand = -1;
    public int roundStack1 = 0;
    public int roundStack2 = 0;
    public int roundStack3 = 0;
    public GameObject[] wall = new GameObject[3]; 

    //상세 점수
    public float survivalScore = 0f;
    public float roundScore = 0f;
    public float killScore = 0f;
    public float dpsScore = 0f;
    public float hpScore = 0f;
    public float enemyScore = 0f;
    public float statScore = 0f;
    public float equipScore = 0f;

    public Text roundTimeText;
    public Text roundText;
    public Text enemyText;
    public Text scoreText;
    public Text dpsText;
    public Text difficultyText;

    public GameObject go_roundSelect; //라운드 보상 선택 창
    public GameObject weaponSelect; //무기 선택 창
    public GameObject equipSelect; //방어구 선택 창
    public GameObject statSelect; //스탯 선택 창 
    public GameObject lrSelect; //왼오 선택 창
    public Image leftImg; //무엇을 장착했는지 보여주기 위하여
    public Image rightImg;
    public Image leftSideImg; //무엇을 장착했는지 보여주기 위하여
    public Image rightSideImg;

    public GameObject leftBtn;
    public GameObject rightBtn;
    public GameObject leftSideBtn;
    public GameObject rightSideBtn;

    private GameObject clickObject; //방금 눌린 버튼
    private int type = 0; //무기 = 0, 장비 = 1 

    public GameObject pause;
    public GameObject gameOver;
    public bool isPause = false; //일시정지 상태인지
    public bool isEnd = false; //게임이 끝났는지
    int ClickCount = 0; //종료버튼 더블클릭

    private CharacterStatus characterStatus;
    private CharacterWeapon characterWeapon;
    private EnemyManager enemyManager;
    private ItemDatabase itemDatabase;
    private Inventory inventory;
    private RoundSelect roundSelect;
    private ReAdmob reAdmob;   

    public static GameManager instance; //공유자원으로 설정

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        characterStatus = FindObjectOfType<CharacterStatus>();
        characterWeapon = FindObjectOfType<CharacterWeapon>();
        enemyManager = FindObjectOfType<EnemyManager>();
        itemDatabase = FindObjectOfType<ItemDatabase>();
        inventory = FindObjectOfType<Inventory>();
        roundSelect = FindObjectOfType<RoundSelect>();
        reAdmob = FindObjectOfType<ReAdmob>();

        Time.timeScale = 1;

        SetResolution();
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;
        AudioManager.instance.PlayBGM("BGM0");

        ConfirmFirst();
        if (isFirst) //첫판이라면 실행
        {
            isFirst = false;
            UpdateFirst();
            PauseBtn();
            pause.GetComponent<Pause>().HelpButton();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClickCount++;
            if (!IsInvoking("DoubleClick"))
                Invoke("DoubleClick", 0.5f);

        }
        else if (ClickCount == 2)
        {
            CancelInvoke("DoubleClick");
            Application.Quit();
        }

        if (!isEnd)
        {
            roundTime -= Time.deltaTime; //라운드 남은 시간
            totalTime += Time.deltaTime; //총 생존시간
            survivalScore += Time.deltaTime; //초당 1점씩 생존점수 증가

            totalScore = (int)survivalScore + (int)roundScore + (int)killScore + (int)dpsScore + (int)hpScore + (int)enemyScore + (int)statScore + (int)equipScore;
            scoreText.text = ((int)totalScore).ToString();
            roundTimeText.text = ((int)roundTime).ToString();
            enemyText.text = enemy.ToString();

            if (totalTime >= 1f)
                dpsText.text = (totalDamage / Mathf.Floor(totalTime)).ToString("F2");
        }

        if (roundTime <= 0f) //라운드 시간이 끝나면
        {
            if(round != 0) //0부터 시작하기 때문에
            {
                roundScore += round * 100; //라운드가 끝날 때마다 추가 점수
                dpsScore += totalDamage / totalTime; //라운드가 끝날 때마다 dps 추가 점수
                enemyScore += 300 - enemy; //남은 적 추가점수
                hpScore += characterStatus.hp;//남은 체력 추가점수
                go_roundSelect.SetActive(true); //라운드 끝나고 보상 선택
                Time.timeScale = 0; //일시정지
                isPause = true;
            }

            roundTime = 30f; //라운드 시간 재설정
            NextRound(); //라운드 넘어가기 후 적 생성  
        }

        if (characterStatus.hp <= 0 || enemy > 300 || round > 30) //체력이 0이 되거나 적의 수가 300을 넘으면 게임 종료
        {
            isEnd = true;
        }

        if (isEnd && gameOver.activeSelf == false)
            GameEnd();

        if (isEnd && Time.timeScale > 0) //광고가 일시정지를 푼다
            Time.timeScale = 0;

        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        if (enemy == 0)
            RoundSkip();

    }

    private void GameEnd()
    {
        //올린 스탯만큼 추가점수
        statScore += (characterStatus.maxHP - 100);
        statScore += (characterStatus.atk - 100) * 2 / 3;
        statScore += (characterStatus.def * 3);
        statScore += (1f - characterStatus.atkSpd) * 600;
        statScore += (characterStatus.spd - 5) * 60;
        statScore += (characterStatus.crit * 6);

        gameOver.SetActive(true);

        Time.timeScale = 0;

        reAdmob.GameOver();
    }

    private void DifficultyUp()
    {
        while (true) //각 난이도효과마다 최대 3스택을 넘지 못하게
        {
            roundRand = Random.Range(0, 3);

            if (roundRand == 0)
            {
                if (roundStack1 < 3)
                {
                    roundStack1++;
                    break;
                }
            }
            else if (roundRand == 1)
            {
                if(roundStack2 < 3)
                {
                    roundStack2++;
                    break;
                }
            }
            else if (roundRand == 2)
            {
                if (roundStack3 < 3)
                {
                    roundStack3++;
                    break;
                }
            }
        }

        switch (roundRand)
        {
            case 0:
                wall[roundStack1 - 1].SetActive(true); //벽생성
                difficultyText.text = "Map is Reduced!!";
                difficultyText.gameObject.SetActive(true);
                break;
            case 1:
                characterStatus.hps += -1; //hp가 점점 감소
                difficultyText.text = "Release HP!!";
                difficultyText.gameObject.SetActive(true);
                break;
            case 2:
                difficultyText.text = "Enemy SPD UP!!";
                difficultyText.gameObject.SetActive(true);
                //enemyManager에서 실행중
                break;
        }
    }

    private void NextRound()
    {
        round++; //다음 라운드 시작
        roundText.text = round.ToString();
        enemyManager.round++;

        if (round % 5 == 0) //보스 만들기
        {
            enemyManager.enemyNum = 4;
            enemyManager.CreateEnemy(6);
            DifficultyUp();
        }

        enemyManager.enemyNum = Random.Range(0, 4);
        for (int i = 0; i < enemyManager.createLT.Length; i++)
        {
            enemyManager.CreateEnemy(i);
        }
    }

    public void EnemyPlusBtn()
    {
        enemyManager.enemyNum = Random.Range(0, 4);
        for (int i = 0; i < enemyManager.createLT.Length; i++)
        {
            enemyManager.CreateEnemy(i);
        }
    }

    //일시정지 버튼
    public void PauseBtn()
    {
        AudioManager.instance.PlaySFX("Button");
        if (!isPause)
        {
            pause.SetActive(true);
            Time.timeScale = 0;
            isPause = true;
        }
    }

    //라운드 스킵 적의 수가 0에 도달했을경우에만
    public void RoundSkip()
    {
        if (!isPause)
            roundTime = 0f; //라운드 시간을 0으로 만든다.
    }

    public void WeaponBtn() // 라운드 선택 창에서 무기 선택을 고를 때
    {
        AudioManager.instance.PlaySFX("Button");
        go_roundSelect.SetActive(false);
        WeaponSelect();
    }
    public void EquipBtn() // 라운드 선택 창에서 방어구 선택을 고를 때
    {
        AudioManager.instance.PlaySFX("Button");
        go_roundSelect.SetActive(false);
        EquipSelect();
    }
    public void StatBtn() // 라운드 선택 창에서 스탯 선택을 고를 때
    {
        AudioManager.instance.PlaySFX("Button");
        go_roundSelect.SetActive(false);
        StatSelect();
    }
    public void HPBtn() // 라운드 선택 창에서 체력 선택을 고를 때
    {
        AudioManager.instance.PlaySFX("Button");
        go_roundSelect.SetActive(false);
        characterStatus.hp += 50f; //체력을 회복시켜준다.
        if (characterStatus.hp >= characterStatus.maxHP)
            characterStatus.hp = characterStatus.maxHP;
        Time.timeScale = 1; //다시 시작
        isPause = false;
    }

    public void WeaponSelect() //무기 선택 창 표시
    {
        weaponSelect.SetActive(true);
        roundSelect.WeaponSelect();
    }
    public void EquipSelect() //방어구 선택 창 표시
    {
        equipSelect.SetActive(true);
        roundSelect.EquipSelect();
    }
    public void StatSelect() // 스탯 선택 창 표시
    {
        statSelect.SetActive(true);
        roundSelect.StatSelect();
    }

    public void LRSelect(int type) //왼오 선택 창 표시
    {
        //type = 0 weapon, type = 1 equip
        lrSelect.SetActive(true);
        if (type == 0)
        {
            leftImg.sprite = null;
            rightImg.sprite = null;
            leftBtn.transform.localPosition = new Vector2(leftBtn.transform.localPosition.x, 150);
            rightBtn.transform.localPosition = new Vector2(rightBtn.transform.localPosition.x, 150);
            if (inventory.weaponL != -1)
            {
                leftImg.gameObject.SetActive(true);
                leftImg.sprite = inventory.weapon1.img.sprite;
            }
            if (inventory.weaponR != -1)
            {
                rightImg.gameObject.SetActive(true);
                rightImg.sprite = inventory.weapon2.img.sprite;
            }
            leftSideBtn.SetActive(true);
            rightSideBtn.SetActive(true);
            if (inventory.sideWeaponL != -1)
            {
                leftSideImg.gameObject.SetActive(true);
                leftSideImg.sprite = inventory.sideWeapon1.img.sprite;
            }
            if (inventory.sideWeaponR != -1)
            {
                rightSideImg.gameObject.SetActive(true);
                rightSideImg.sprite = inventory.sideWeapon2.img.sprite;
            }
        }
        else if (type == 1)
        {
            leftImg.sprite = null;
            rightImg.sprite = null;
            leftBtn.transform.localPosition = new Vector2(leftBtn.transform.localPosition.x, 0);
            rightBtn.transform.localPosition = new Vector2(rightBtn.transform.localPosition.x, 0);
            if (inventory.equipL != -1)
            {
                leftImg.gameObject.SetActive(true);
                leftImg.sprite = inventory.equip1.img.sprite;
            }
            if (inventory.equipR != -1)
            {
                rightImg.gameObject.SetActive(true);
                rightImg.sprite = inventory.equip2.img.sprite;
            }
            leftSideBtn.SetActive(false);
            rightSideBtn.SetActive(false);
        }
    }

    public void WeaponSelectBtn() //무기 선택 버튼, 세 버튼 모두 하나로 관리
    {
        //방금 누른 버튼 오브젝트를 가져와서 정보 추출하고 적용하기
        AudioManager.instance.PlaySFX("Button");
        clickObject = EventSystem.current.currentSelectedGameObject;
        type = 0;
        weaponSelect.SetActive(false);
        LRSelect(0);
    }

    public void EquipSelectBtn() //무기 선택 버튼, 세 버튼 모두 하나로 관리
    {
        //방금 누른 버튼 오브젝트를 가져와서 정보 추출하고 적용하기
        AudioManager.instance.PlaySFX("Button");
        clickObject = EventSystem.current.currentSelectedGameObject;
        type = 1;
        equipSelect.SetActive(false);
        LRSelect(1);
    }

    public void StatSelectBtn() //무기 선택 버튼, 세 버튼 모두 하나로 관리
    {
        //방금 누른 버튼 오브젝트를 가져와서 정보 추출하고 적용하기
        AudioManager.instance.PlaySFX("Button");
        clickObject = EventSystem.current.currentSelectedGameObject.transform.parent.gameObject;
        inventory.PutStat(clickObject);
        statSelect.SetActive(false);
        Time.timeScale = 1;
        isPause = false;
    }
    //왼쪽 선택 버튼
    public void LeftSelectBtn()
    {
        AudioManager.instance.PlaySFX("Button");
        if (type == 0)
            //인벤토리에서 적용
            inventory.PutWepaonInventory(clickObject, 0);
        else if(type == 1)
            //인벤토리에서 적용
            inventory.PutEquipInventory(clickObject, 0);

        lrSelect.SetActive(false);
        Time.timeScale = 1;
        isPause = false;

        equipScore += 100;

        if (type == 0)
        {
            characterWeapon.CheckHiddenCombo(); //다 끝났을 때 검사
            inventory.CheckHiddenCombo();
            if (characterStatus.clone.activeSelf == true)
                characterStatus.clone.GetComponent<Clone>().CheckHiddenCombo();
        }
    }
    //오른쪽 선택 버튼
    public void RightSelectBtn()
    {
        AudioManager.instance.PlaySFX("Button");
        if (type == 0)
            //인벤토리에서 적용
            inventory.PutWepaonInventory(clickObject, 1);
        else if (type == 1)
            //인벤토리에서 적용
            inventory.PutEquipInventory(clickObject, 1);

        lrSelect.SetActive(false);
        Time.timeScale = 1;
        isPause = false;

        equipScore += 100;

        if (type == 0)
        {
            characterWeapon.CheckHiddenCombo(); //다 끝났을 때 검사
            inventory.CheckHiddenCombo();
            if (characterStatus.clone.activeSelf == true)
                characterStatus.clone.GetComponent<Clone>().CheckHiddenCombo();
        }
    }
    //왼쪽 선택 버튼
    public void LeftSideSelectBtn()
    {
        AudioManager.instance.PlaySFX("Button");
        //인벤토리에서 적용
        inventory.PutSideWepaonInventory(clickObject, 0);
 
        lrSelect.SetActive(false);
        Time.timeScale = 1;
        isPause = false;

        equipScore += 100;

        inventory.CheckHiddenCombo();
        characterWeapon.CheckHiddenCombo(); //다 끝났을 때 검사
        if (characterStatus.clone.activeSelf == true)
            characterStatus.clone.GetComponent<Clone>().CheckHiddenCombo();
    }

    //왼쪽 선택 버튼
    public void RightSideSelectBtn()
    {
        AudioManager.instance.PlaySFX("Button");
        //인벤토리에서 적용
        inventory.PutSideWepaonInventory(clickObject,1);

        lrSelect.SetActive(false);
        Time.timeScale = 1;
        isPause = false;

        equipScore += 100;

        inventory.CheckHiddenCombo();
        characterWeapon.CheckHiddenCombo(); //다 끝났을 때 검사
        if (characterStatus.clone.activeSelf == true)
            characterStatus.clone.GetComponent<Clone>().CheckHiddenCombo();
    }

    // 해상도 설정하는 함수
    public void SetResolution()
    {
        int setWidth = 1920; // 사용자 설정 너비
        int setHeight = 1080; // 사용자 설정 높이

        int deviceWidth = Screen.width; // 기기 너비 저장
        int deviceHeight = Screen.height; // 기기 높이 저장

        Screen.SetResolution(setWidth, (int)(((float)deviceHeight / deviceWidth) * setWidth), true); // SetResolution 함수 제대로 사용하기

        if ((float)setWidth / setHeight < (float)deviceWidth / deviceHeight) // 기기의 해상도 비가 더 큰 경우
        {
            float newWidth = ((float)setWidth / setHeight) / ((float)deviceWidth / deviceHeight); // 새로운 너비
            Camera.main.rect = new Rect((1f - newWidth) / 2f, 0f, newWidth, 1f); // 새로운 Rect 적용
        }
        else // 게임의 해상도 비가 더 큰 경우
        {
            float newHeight = ((float)deviceWidth / deviceHeight) / ((float)setWidth / setHeight); // 새로운 높이
            Camera.main.rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight); // 새로운 Rect 적용
        }
    }

    void DoubleClick()
    {
        ClickCount = 0;
    }

    private void ConfirmFirst()
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
            string tempFirst = bro.Rows()[i]["FirstGame"]["BOOL"].ToString();
            string tempID = bro.Rows()[i]["ID"]["S"].ToString();

            id = tempID;
            if (tempFirst == "True")
                isFirst = true;
            else
                isFirst = false;
        }
    }
    private void UpdateFirst() //설정 데이터 서버에 저장
    {
        //초기 정보 넣어주기     
        Param param = new Param();
        param.Add("FirstGame", false);

        Where where = new Where(); //id가 같은 곳에
        where.Equal("ID", id);

        Backend.GameData.Update("Status", where, param); //update
    }
}
