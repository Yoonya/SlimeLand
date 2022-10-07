using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    public Text MaxHPText;
    public Text atkText;
    public Text defText;
    public Text atkSpdText;
    public Text spdText;
    public Text critText;
    public Text hpsText;

    public Text jobName;
    public Text equipName;
    public Text jobText;
    public Text equipText;

    //인벤토리 정보
    public ItemBtn weapon1;
    public ItemBtn weapon2;
    public Image weaponCombo;
    public ItemBtn equip1;
    public ItemBtn equip2;
    public Image equipCombo;
    public Image sideWeapon1;
    public Image sideWeapon2;

    //help관련
    public GameObject helpBtn;
    public GameObject helpImg;
    public GameObject helpImg2;

    private CharacterStatus characterStatus;
    private Inventory inventory;
    private ItemDatabase itemDatabase;

    //켜질 때마다
    private void OnEnable()
    {
        //비활성화 상태에서 시작하기에 여기있다.
        characterStatus = FindObjectOfType<CharacterStatus>();
        inventory = FindObjectOfType<Inventory>();
        itemDatabase = FindObjectOfType<ItemDatabase>();

        MaxHPText.text = characterStatus.maxHP.ToString();
        atkText.text = characterStatus.atk .ToString();
        defText.text = characterStatus.def.ToString();
        atkSpdText.text = characterStatus.atkSpd.ToString();
        spdText.text = characterStatus.spd.ToString();
        critText.text = characterStatus.crit.ToString() + "%";
        hpsText.text = characterStatus.hps.ToString("F1");

        weapon1.img.sprite = inventory.weapon1.img.sprite;
        weapon1.name.text = inventory.weapon1.name.text;

        weapon2.img.sprite = inventory.weapon2.img.sprite;
        weapon2.name.text = inventory.weapon2.name.text;

        sideWeapon1.sprite = inventory.sideWeapon1.img.sprite;
        sideWeapon2.sprite = inventory.sideWeapon2.img.sprite;

        Color color;
        if (inventory.weaponCombo.sprite == null)
        {
            weaponCombo.sprite = inventory.weaponCombo.sprite;
            color = weaponCombo.color;
            color.a = 0f;
            weaponCombo.color = color;
        }
        else
            weaponCombo.sprite = inventory.weaponCombo.sprite;


        equip1.img.sprite = inventory.equip1.img.sprite;
        equip1.name.text = inventory.equip1.name.text;

        equip2.img.sprite = inventory.equip2.img.sprite;
        equip2.name.text = inventory.equip2.name.text;

        equipCombo.sprite = inventory.equipCombo.sprite;

        if (inventory.weapon1.img.sprite != null)
        {
            color = weapon1.img.color; //알파 변경
            color.a = 1.0f;
            weapon1.img.color = color;
        }
        if (inventory.weapon2.img.sprite != null)
        {
            color = weapon2.img.color; //알파 변경
            color.a = 1.0f;
            weapon2.img.color = color;
        }
        if (inventory.weaponCombo.sprite != null)
        {
            color = weaponCombo.color; //알파 변경
            color.a = 1.0f;
            weaponCombo.color = color;

            if (characterStatus.hiddenCombo != -1)
                weaponCombo.color = new Color(255, 255, 0);
            else
                weaponCombo.color = new Color(255, 255, 255);
        }
        if (inventory.equip1.img.sprite != null)
        {
            color = equip1.img.color; //알파 변경
            color.a = 1.0f;
            equip1.img.color = color;
        }
        if (inventory.equip2.img.sprite != null)
        {
            color = equip2.img.color; //알파 변경
            color.a = 1.0f;
            equip2.img.color = color;
        }
        if (inventory.equipCombo.sprite != null)
        {
            color = equipCombo.color; //알파 변경
            color.a = 1.0f;
            equipCombo.color = color;
        }
        if (inventory.sideWeapon1.img.sprite != null)
        {
            color = sideWeapon1.color; //알파 변경
            color.a = 1.0f;
            sideWeapon1.color = color;
        }
        if (inventory.sideWeapon2.img.sprite != null)
        {
            color = sideWeapon2.color; //알파 변경
            color.a = 1.0f;
            sideWeapon2.color = color;
        }

        if (characterStatus.weaponCombo != -1)
        {
            if (characterStatus.hiddenCombo == -1)
            {
                jobName.text = itemDatabase.wcCombos[characterStatus.weaponCombo].wcName;
                jobText.text = itemDatabase.wcCombos[characterStatus.weaponCombo].wcDesc;
            }
            else
            {
                jobName.text = itemDatabase.wcCombos[characterStatus.weaponCombo + itemDatabase.wcCombos.Count / 2].wcName;
                jobText.text = itemDatabase.wcCombos[characterStatus.weaponCombo + itemDatabase.wcCombos.Count / 2].wcDesc;
            }

        }
        if (characterStatus.equipCombo != -1)
        {
            equipName.text = itemDatabase.ecCombos[characterStatus.equipCombo].ecName;
            equipText.text = itemDatabase.ecCombos[characterStatus.equipCombo].ecDesc;
        }          
    }

    //다시 게임으로
    public void BackButton()
    {
        AudioManager.instance.PlaySFX("Button");
        Time.timeScale = 1;
        GameManager.instance.isPause = false;
        helpImg.SetActive(false);
        helpImg2.SetActive(false);
        gameObject.SetActive(false);
    }

    //메인메뉴로
    public void MainButton()
    {
        AudioManager.instance.PlaySFX("Button");
        StartCoroutine(LoadScene("Lobby"));
    }

    //재시작
    public void RestartButton()
    {
        AudioManager.instance.PlaySFX("Button");
        StartCoroutine(LoadScene("Game"));
    }

    //도움말
    public void HelpButton()
    {
        AudioManager.instance.PlaySFX("Button");
        helpImg.SetActive(true);
    }
    //도움말 다음
    public void HelpNextButton()
    {
        AudioManager.instance.PlaySFX("Button");
        helpImg.SetActive(false);
        helpImg2.SetActive(true);
    }
    //도움말 뒤로
    public void HelpBackButton()
    {
        AudioManager.instance.PlaySFX("Button");
        helpImg2.SetActive(false);
    }

    IEnumerator LoadScene(string nextScene)
    {
        Time.timeScale = 1;
        yield return new WaitForSeconds(1);
        LoadingScene.LoadScene(nextScene);
    }
}
