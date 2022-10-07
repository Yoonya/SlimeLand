using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    private void OnEnable()
    {
        Invoke("DestroyObject", 0.3f);
    }


    private void DestroyObject()
    {
        ObjectPool.instance.queue[19].Enqueue(gameObject);
        gameObject.SetActive(false);
    }

}
