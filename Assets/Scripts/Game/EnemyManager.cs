using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public Transform[] createLT;
    public Transform characterLT;

    public int enemyNum = 0; //적의 종류 설정
    public int round = 1; //현재 라운드

    public void CreateEnemy(int createLTNum)
    {
        GameObject tempEnemy = ObjectPool.instance.queue[14+enemyNum].Dequeue(); //오브젝트 풀링 사용

        //적 설정
        tempEnemy.GetComponent<Enemy>().enemyNum = enemyNum; // 적 종류
        tempEnemy.GetComponent<Enemy>().characterLT = characterLT;
        tempEnemy.GetComponent<Enemy>().atkSpd = 1;
       
        switch (enemyNum)
        {
            case 0: //스탠다드
                tempEnemy.GetComponent<Enemy>().MaxHP = 100 + (round - 1) * 80;
                tempEnemy.GetComponent<Enemy>().atk = 10 + (round - 1) * 2;            
                tempEnemy.GetComponent<Enemy>().spd = 2;
                break;
            case 1: //탱커
                tempEnemy.GetComponent<Enemy>().MaxHP = 150 + (round - 1) * 150;
                tempEnemy.GetComponent<Enemy>().atk = 10 + (round - 1) * 1;
                tempEnemy.GetComponent<Enemy>().spd = 1;
                break;
            case 2: //스피드
                tempEnemy.GetComponent<Enemy>().MaxHP = 80 + (round - 1) * 50;
                tempEnemy.GetComponent<Enemy>().atk = 10 + (round - 1) * 1;
                tempEnemy.GetComponent<Enemy>().spd = 3;
                break;
            case 3: //자폭
                tempEnemy.GetComponent<Enemy>().MaxHP = 80 + (round - 1) * 50;
                tempEnemy.GetComponent<Enemy>().atk = 10 + (round - 1) * 2;
                tempEnemy.GetComponent<Enemy>().spd = 1;
                break;
            case 4: //보스
                tempEnemy.GetComponent<Enemy>().MaxHP = 1000 + round / 5 * 1500;
                tempEnemy.GetComponent<Enemy>().atk = 20 + round / 5 * 20;
                tempEnemy.GetComponent<Enemy>().spd = 2 + round / 5 * 0.4f;
                break;
        }
        if (GameManager.instance.roundStack3 != 0 && enemyNum != 4) //난이도 업
            tempEnemy.GetComponent<Enemy>().spd = tempEnemy.GetComponent<Enemy>().spd * (1f + GameManager.instance.roundStack3 * 0.2f);
        tempEnemy.GetComponent<Enemy>().HP = tempEnemy.GetComponent<Enemy>().MaxHP;

        if (enemyNum == 0 || enemyNum == 3)
            tempEnemy.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
        else if(enemyNum == 1 || enemyNum == 4)
            tempEnemy.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        else
            tempEnemy.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        tempEnemy.transform.position = createLT[createLTNum].position;
        tempEnemy.SetActive(true);
        GameManager.instance.enemy++; //적 남은 수 추가
    }
}
