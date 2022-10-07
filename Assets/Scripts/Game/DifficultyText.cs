using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyText : MonoBehaviour
{
    private void OnEnable()
    {
        Invoke("DestroyObject", 3f);
    }

    private void DestroyObject()
    {
        gameObject.SetActive(false);
    }
}
