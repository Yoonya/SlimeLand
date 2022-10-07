using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEffect : MonoBehaviour
{
    private ParticleSystem ps;
    public int type; // 0 = hit, 1 = die, 2 = launcherExplosion, 3 = explosion/slash, 4 = destroy

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (ps)
        {
            if (!ps.IsAlive())
            {
                if (type == 0)
                {
                    ObjectPool.instance.queue[7].Enqueue(gameObject);
                    gameObject.SetActive(false);
                }
                else if (type == 1)
                {
                    ObjectPool.instance.queue[8].Enqueue(gameObject);
                    gameObject.SetActive(false);
                }
                else if (type == 2)
                {
                    ObjectPool.instance.queue[9].Enqueue(gameObject);
                    gameObject.SetActive(false);
                }
                else if (type == 3)
                {
                    gameObject.SetActive(false);
                }
                else if (type == 4)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
