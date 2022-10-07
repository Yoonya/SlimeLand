using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;

//최종 상태 창은 CharacterStatus, Inventory, CharacterJob의 상태를 모두 더해야한다.
public class CharacterStatus : MonoBehaviour
{
    //상태창
    public float maxHP = 100;
    public float hp = 100;
    public float atk = 100;
    public float def = 0;
    public float atkSpd = 1f;
    public float MaxAtkSpd = 0.1f;
    public float spd = 5;
    public float MaxSpd = 15;
    public float crit = 0;
    public float hps = 0; //초당 체력 회복
    //무기 및 장비
    public int weaponL = -1; //-1은 없음
    public int weaponR = -1;
    public int sideWeaponL = -1; //-1은 없음
    public int sideWeaponR = -1;
    public int equipL = -1; 
    public int equipR = -1;
    public int weaponCombo = -1;
    public int equipCombo = -1;
    public int sideWeaponCombo = -1;
    public int hiddenCombo = -1;

    //분신효과
    public GameObject clone;
    //HPS에 도움 줄 time
    private float time = 0f;
    //범위 밖으로 나갈경우 time
    private float blackTime = 0f;

    private Inventory inventory;
    private CharacterMove characterMove;
    private CharacterWeapon characterWeapon;

    //equipComboAura 관련 파티클시스템
    public GameObject equipComboAura;
    private ParticleSystem equipComboPartical;
    public Color equipComboColor
    {
        set
        {
            var main = equipComboPartical.main;
            main.startColor = value;
        }
    }

    private void Start()
    {
        ConfirmStatus(); //서버에서 status가져오기

        inventory = FindObjectOfType<Inventory>();
        characterMove = FindObjectOfType<CharacterMove>();
        characterWeapon = GetComponent<CharacterWeapon>();

        equipComboPartical = equipComboAura.GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (hps != 0) //HPS가 존재한다면
        {
            time += Time.deltaTime;
            if (time >= 1.0f) //1초마다
            {
                time = 0f;
                hp += hps;

                if (hp > maxHP) //넘는다면 최대체력으로
                    hp = maxHP;
            }
        }

        CheckAura(); //장비콤보 오라 효과 확인
    }

    private void CheckAura()
    {
        if (equipCombo == -1 && equipComboAura.activeSelf == true) //aura 꺼주기
            equipComboAura.SetActive(false);
        if (equipCombo != -1 && equipComboAura.activeSelf == false) //aura 켜주기
        {
            equipComboAura.SetActive(true);
            switch (equipCombo)
            {
                case 0:
                    equipComboColor = new Color(0f, 1f, 0f, 0.5f);
                    break;
                case 1:
                    equipComboColor = new Color(0.8f, 0.8f, 0.8f, 0.5f);
                    break;
                case 2:
                    equipComboColor = new Color(0f, 1f, 1f, 0.5f);
                    break;
                case 3:
                    equipComboColor = new Color(1f, 1f, 0f, 0.5f);
                    break;
                case 4:
                    equipComboColor = new Color(1f, 0f, 0f, 0.5f);
                    break;
                case 5:
                    equipComboColor = new Color(1f, 0f, 1f, 0.5f);
                    break;
                default:
                    break;
            }
        }
    }

    public void CharacterHitted(float atk)
    {
        float damage = atk - def;
        if (damage <= 0)
            damage = 1f;

        hp -= damage;

        if (hp < 0)
        {
            hp = 0;
            GameManager.instance.isEnd = true;
        }

        AudioManager.instance.PlaySFX("PlayerHit");
        GameObject effect = ObjectPool.instance.queue[7].Dequeue();
        effect.SetActive(true);
        effect.transform.position = gameObject.transform.position;
    }

    //무기 콤보 효과 적용
    public void SetWeaponCombo(int wc)
    {
        int lastWeaponCombo = weaponCombo; //해제해야 할 효과를 넘겨준다.
        weaponCombo = wc;
        //무기직업 오의 효과
        if (lastWeaponCombo == 4)
            hps -= 3;
        if (weaponCombo == 4)
            hps += 3;
    }
    //장비 콤보 효과 적용
    public void SetEquipCombo(int ec)
    {
        int lastEquipCombo = equipCombo;//해제해야 할 효과를 넘겨준다.
        equipCombo = ec;
        //분신효과
        if (lastEquipCombo == 5)
            clone.SetActive(false);
        if (equipCombo == 5)
        {
            clone.SetActive(true);
            characterWeapon.RestartWeapon();
        }

    }
    //무기 콤보 효과 적용
    public void SetSideWeaponCombo(int wc)
    {
        int lastWeaponCombo = sideWeaponCombo; //해제해야 할 효과를 넘겨준다.
        sideWeaponCombo = wc;
    }
    //무기 콤보 효과 적용
    public void SetHiddenCombo(int wc)
    {
        int lastHiddenCombo = hiddenCombo; //해제해야 할 효과를 넘겨준다.
        hiddenCombo = wc;
    }
    public void SetDef(float defP)
    {
        def += defP;
    }

    //최종 스탯 적용
    public void SetAtkSpd(float atkspdP)
    {
        atkSpd -= atkspdP;
        if (atkSpd < 0.1f)
            atkSpd = 0.1f;
    }
    public void SetSpeed(float spdP)
    {
        spd += spdP;
        if (spd > 15f)
            spd = 15f;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Black"))
        {
            if (!GameManager.instance.isEnd && !GameManager.instance.isPause)
            {
                hp -= 10 / 60f;
                blackTime += 1 / 60f;
                if (blackTime >= 1f)
                {
                    AudioManager.instance.PlaySFX("PlayerHit");
                    GameObject effect = ObjectPool.instance.queue[7].Dequeue();
                    effect.SetActive(true);
                    effect.transform.position = gameObject.transform.position;
                    blackTime = 0f;
                }
            }
        }
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
            spd = tempSpd;
            crit = tempCrit;
            hps = tempHps; //초당 체력 회복
        }
    }
}
