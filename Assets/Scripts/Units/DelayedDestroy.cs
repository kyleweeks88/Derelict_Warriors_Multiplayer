using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedDestroy : MonoBehaviour
{
    [SerializeField] float delayTime = 1f;
    float destroyTime = -1f;

    void Awake() 
    {
        destroyTime = Time.time + delayTime;
    }

    void Update() 
    {
        if(destroyTime != -1 && Time.time >= destroyTime)
        {
            destroyTime = -1f;
            Destroy(gameObject);
        }
    }
}
