using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using BackEnd;

public class GameManager : MonoBehaviour
{
    public float roundTime = 0f; // ����� �� 60�� ������Ʈ���� �����ϱ� ���� 0���� ����
    public int round = 0; //���� ��
    public int enemy = 0; //�� ��
    public float totalDamage = 0f;
    public float totalTime = 0f;
    public float totalScore = 0; //�� ����

    public bool isFirst = true; //ù������
    public string id;//ȸ�� ���̵�

    //���̵� ����Ű�� ����
    private int roundRand = -1;
    public int roundStack1 = 0;
    public int roundStack2 = 0;
    public int roundStack3 = 0;
    public GameObject[] wall = new GameObject[3]; 

    //�� ����
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

    public GameObject go_roundSelect; //���� ���� ���� â
    public GameObject weaponSelect; //���� ���� â
    public GameObject equipSelect; //�� ���� â
    public GameObject statSelect; //���� ���� â 
    public GameObject lrSelect; //�޿� ���� â
    public Image leftImg; //������ �����ߴ��� �����ֱ� ���Ͽ�
    public Image rightImg;
    public Image leftSideImg; //������ �����ߴ��� �����ֱ� ���Ͽ�
    public Image rightSideImg;

    public GameObject leftBtn;
    public GameObject rightBtn;
    public GameObject leftSideBtn;
    public GameObject rightSideBtn;

    private GameObject clickObject; //��� ���� ��ư
    private int type = 0; //���� = 0, ��� = 1 

    public GameObject pause;
    public GameObject gameOver;
    public bool isPause = false; //�Ͻ����� ��������
    public bool isEnd = false; //������ ��������
    int ClickCount = 0; //�����ư ����Ŭ��

    private CharacterStatus characterStatus;
    private CharacterWeapon characterWeapon;
    private EnemyManager enemyManager;
    private ItemDatabase itemDatabase;
    private Inventory inventory;
    private RoundSelect roundSelect;
    private ReAdmob reAdmob;   

    public static GameManager instance; //�����ڿ����� ����

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
        if (isFirst) //ù���̶�� ����
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
            roundTime -= Time.deltaTime; //���� ���� �ð�
            totalTime += Time.deltaTime; //�� �����ð�
            survivalScore += Time.deltaTime; //�ʴ� 1���� �������� ����

            totalScore = (int)survivalScore + (int)roundScore + (int)killScore + (int)dpsScore + (int)hpScore + (int)enemyScore + (int)statScore + (int)equipScore;
            scoreText.text = ((int)totalScore).ToString();
            roundTimeText.text = ((int)roundTime).ToString();
            enemyText.text = enemy.ToString();

            if (totalTime >= 1f)
                dpsText.text = (totalDamage / Mathf.Floor(totalTime)).ToString("F2");
        }

        if (roundTime <= 0f) //���� �ð��� ������
        {
            if(round != 0) //0���� �����ϱ� ������
            {
                roundScore += round * 100; //���尡 ���� ������ �߰� ����
                dpsScore += totalDamage / totalTime; //���尡 ���� ������ dps �߰� ����
                enemyScore += 300 - enemy; //���� �� �߰�����
                hpScore += characterStatus.hp;//���� ü�� �߰�����
                go_roundSelect.SetActive(true); //���� ������ ���� ����
                Time.timeScale = 0; //�Ͻ�����
                isPause = true;
            }

            roundTime = 30f; //���� �ð� �缳��
            NextRound(); //���� �Ѿ�� �� �� ����  
        }

        if (characterStatus.hp <= 0 || enemy > 300 || round > 30) //ü���� 0�� �ǰų� ���� ���� 300�� ������ ���� ����
        {
            isEnd = true;
        }

        if (isEnd && gameOver.activeSelf == false)
            GameEnd();

        if (isEnd && Time.timeScale > 0) //���� �Ͻ������� Ǭ��
            Time.timeScale = 0;

        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        if (enemy == 0)
            RoundSkip();

    }

    private void GameEnd()
    {
        //�ø� ���ȸ�ŭ �߰�����
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
        while (true) //�� ���̵�ȿ������ �ִ� 3������ ���� ���ϰ�
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
                wall[roundStack1 - 1].SetActive(true); //������
                difficultyText.text = "Map is Reduced!!";
                difficultyText.gameObject.SetActive(true);
                break;
            case 1:
                characterStatus.hps += -1; //hp�� ���� ����
                difficultyText.text = "Release HP!!";
                difficultyText.gameObject.SetActive(true);
                break;
            case 2:
                difficultyText.text = "Enemy SPD UP!!";
                difficultyText.gameObject.SetActive(true);
                //enemyManager���� ������
                break;
        }
    }

    private void NextRound()
    {
        round++; //���� ���� ����
        roundText.text = round.ToString();
        enemyManager.round++;

        if (round % 5 == 0) //���� �����
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

    //�Ͻ����� ��ư
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

    //���� ��ŵ ���� ���� 0�� ����������쿡��
    public void RoundSkip()
    {
        if (!isPause)
            roundTime = 0f; //���� �ð��� 0���� �����.
    }

    public void WeaponBtn() // ���� ���� â���� ���� ������ �� ��
    {
        AudioManager.instance.PlaySFX("Button");
        go_roundSelect.SetActive(false);
        WeaponSelect();
    }
    public void EquipBtn() // ���� ���� â���� �� ������ �� ��
    {
        AudioManager.instance.PlaySFX("Button");
        go_roundSelect.SetActive(false);
        EquipSelect();
    }
    public void StatBtn() // ���� ���� â���� ���� ������ �� ��
    {
        AudioManager.instance.PlaySFX("Button");
        go_roundSelect.SetActive(false);
        StatSelect();
    }
    public void HPBtn() // ���� ���� â���� ü�� ������ �� ��
    {
        AudioManager.instance.PlaySFX("Button");
        go_roundSelect.SetActive(false);
        characterStatus.hp += 50f; //ü���� ȸ�������ش�.
        if (characterStatus.hp >= characterStatus.maxHP)
            characterStatus.hp = characterStatus.maxHP;
        Time.timeScale = 1; //�ٽ� ����
        isPause = false;
    }

    public void WeaponSelect() //���� ���� â ǥ��
    {
        weaponSelect.SetActive(true);
        roundSelect.WeaponSelect();
    }
    public void EquipSelect() //�� ���� â ǥ��
    {
        equipSelect.SetActive(true);
        roundSelect.EquipSelect();
    }
    public void StatSelect() // ���� ���� â ǥ��
    {
        statSelect.SetActive(true);
        roundSelect.StatSelect();
    }

    public void LRSelect(int type) //�޿� ���� â ǥ��
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

    public void WeaponSelectBtn() //���� ���� ��ư, �� ��ư ��� �ϳ��� ����
    {
        //��� ���� ��ư ������Ʈ�� �����ͼ� ���� �����ϰ� �����ϱ�
        AudioManager.instance.PlaySFX("Button");
        clickObject = EventSystem.current.currentSelectedGameObject;
        type = 0;
        weaponSelect.SetActive(false);
        LRSelect(0);
    }

    public void EquipSelectBtn() //���� ���� ��ư, �� ��ư ��� �ϳ��� ����
    {
        //��� ���� ��ư ������Ʈ�� �����ͼ� ���� �����ϰ� �����ϱ�
        AudioManager.instance.PlaySFX("Button");
        clickObject = EventSystem.current.currentSelectedGameObject;
        type = 1;
        equipSelect.SetActive(false);
        LRSelect(1);
    }

    public void StatSelectBtn() //���� ���� ��ư, �� ��ư ��� �ϳ��� ����
    {
        //��� ���� ��ư ������Ʈ�� �����ͼ� ���� �����ϰ� �����ϱ�
        AudioManager.instance.PlaySFX("Button");
        clickObject = EventSystem.current.currentSelectedGameObject.transform.parent.gameObject;
        inventory.PutStat(clickObject);
        statSelect.SetActive(false);
        Time.timeScale = 1;
        isPause = false;
    }
    //���� ���� ��ư
    public void LeftSelectBtn()
    {
        AudioManager.instance.PlaySFX("Button");
        if (type == 0)
            //�κ��丮���� ����
            inventory.PutWepaonInventory(clickObject, 0);
        else if(type == 1)
            //�κ��丮���� ����
            inventory.PutEquipInventory(clickObject, 0);

        lrSelect.SetActive(false);
        Time.timeScale = 1;
        isPause = false;

        equipScore += 100;

        if (type == 0)
        {
            characterWeapon.CheckHiddenCombo(); //�� ������ �� �˻�
            inventory.CheckHiddenCombo();
            if (characterStatus.clone.activeSelf == true)
                characterStatus.clone.GetComponent<Clone>().CheckHiddenCombo();
        }
    }
    //������ ���� ��ư
    public void RightSelectBtn()
    {
        AudioManager.instance.PlaySFX("Button");
        if (type == 0)
            //�κ��丮���� ����
            inventory.PutWepaonInventory(clickObject, 1);
        else if (type == 1)
            //�κ��丮���� ����
            inventory.PutEquipInventory(clickObject, 1);

        lrSelect.SetActive(false);
        Time.timeScale = 1;
        isPause = false;

        equipScore += 100;

        if (type == 0)
        {
            characterWeapon.CheckHiddenCombo(); //�� ������ �� �˻�
            inventory.CheckHiddenCombo();
            if (characterStatus.clone.activeSelf == true)
                characterStatus.clone.GetComponent<Clone>().CheckHiddenCombo();
        }
    }
    //���� ���� ��ư
    public void LeftSideSelectBtn()
    {
        AudioManager.instance.PlaySFX("Button");
        //�κ��丮���� ����
        inventory.PutSideWepaonInventory(clickObject, 0);
 
        lrSelect.SetActive(false);
        Time.timeScale = 1;
        isPause = false;

        equipScore += 100;

        inventory.CheckHiddenCombo();
        characterWeapon.CheckHiddenCombo(); //�� ������ �� �˻�
        if (characterStatus.clone.activeSelf == true)
            characterStatus.clone.GetComponent<Clone>().CheckHiddenCombo();
    }

    //���� ���� ��ư
    public void RightSideSelectBtn()
    {
        AudioManager.instance.PlaySFX("Button");
        //�κ��丮���� ����
        inventory.PutSideWepaonInventory(clickObject,1);

        lrSelect.SetActive(false);
        Time.timeScale = 1;
        isPause = false;

        equipScore += 100;

        inventory.CheckHiddenCombo();
        characterWeapon.CheckHiddenCombo(); //�� ������ �� �˻�
        if (characterStatus.clone.activeSelf == true)
            characterStatus.clone.GetComponent<Clone>().CheckHiddenCombo();
    }

    // �ػ� �����ϴ� �Լ�
    public void SetResolution()
    {
        int setWidth = 1920; // ����� ���� �ʺ�
        int setHeight = 1080; // ����� ���� ����

        int deviceWidth = Screen.width; // ��� �ʺ� ����
        int deviceHeight = Screen.height; // ��� ���� ����

        Screen.SetResolution(setWidth, (int)(((float)deviceHeight / deviceWidth) * setWidth), true); // SetResolution �Լ� ����� ����ϱ�

        if ((float)setWidth / setHeight < (float)deviceWidth / deviceHeight) // ����� �ػ� �� �� ū ���
        {
            float newWidth = ((float)setWidth / setHeight) / ((float)deviceWidth / deviceHeight); // ���ο� �ʺ�
            Camera.main.rect = new Rect((1f - newWidth) / 2f, 0f, newWidth, 1f); // ���ο� Rect ����
        }
        else // ������ �ػ� �� �� ū ���
        {
            float newHeight = ((float)deviceWidth / deviceHeight) / ((float)setWidth / setHeight); // ���ο� ����
            Camera.main.rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight); // ���ο� Rect ����
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
            string tempFirst = bro.Rows()[i]["FirstGame"]["BOOL"].ToString();
            string tempID = bro.Rows()[i]["ID"]["S"].ToString();

            id = tempID;
            if (tempFirst == "True")
                isFirst = true;
            else
                isFirst = false;
        }
    }
    private void UpdateFirst() //���� ������ ������ ����
    {
        //�ʱ� ���� �־��ֱ�     
        Param param = new Param();
        param.Add("FirstGame", false);

        Where where = new Where(); //id�� ���� ����
        where.Equal("ID", id);

        Backend.GameData.Update("Status", where, param); //update
    }
}
