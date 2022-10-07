using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//enemy�� hpbar�� sprite�̱� ������ image�� filled�� �ƴ� �Ǻ����� ������ scale�� ����Ѵ�.
public class EnemyHPBar : MonoBehaviour
{
    public Enemy enemy;
    public GameObject hpBar;
    public float fillAmount = 0f;

    // Update is called once per frame
    void Update()
    {
        fillAmount = enemy.HP / enemy.MaxHP;
        hpBar.transform.localScale = new Vector3(fillAmount, 1f, 1f);
    }
}
