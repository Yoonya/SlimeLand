using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour
{
    //type 0  = potion, 1 = diamond
    //0 = 3hp, 1 = 5pt
    public int type = -1;

    private CharacterStatus characterStatus;

    private void Start()
    {
        characterStatus = FindObjectOfType<CharacterStatus>();
    }

    private void OnEnable()
    {
        Invoke("DestroyObject", 10f); //적당한 시간 후에 삭제
    }

    private void DestroyObject()
    {
        ObjectPool.instance.queue[12 + type].Enqueue(gameObject);
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (type == 0) //포션은 3회복
            {
                characterStatus.hp += 3;
                if (characterStatus.hp > characterStatus.maxHP)
                    characterStatus.hp = characterStatus.maxHP;
            }
            else if (type == 1)
            {
                GameManager.instance.killScore += 5;              
            }
            AudioManager.instance.PlaySFX("Item");
            DestroyObject();
        }
    }
}
