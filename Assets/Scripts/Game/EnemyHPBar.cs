using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//enemy의 hpbar는 sprite이기 때문에 image의 filled가 아닌 피봇값을 조정한 scale을 사용한다.
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
