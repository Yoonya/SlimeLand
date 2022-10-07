using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHPBar : MonoBehaviour
{
    public Image hpBar;
    public Text maxHP;
    public Text hp;

    private CharacterStatus characterStatus;
    private Inventory inventory;

    // Start is called before the first frame update
    void Start()
    {
        characterStatus = FindObjectOfType<CharacterStatus>();
        inventory = FindObjectOfType<Inventory>();

        maxHP.text = characterStatus.maxHP.ToString();
        hp.text = characterStatus.hp.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        maxHP.text = characterStatus.maxHP.ToString();
        hp.text = characterStatus.hp.ToString("F1");
        hpBar.fillAmount = characterStatus.hp / characterStatus.maxHP;
    }
}
