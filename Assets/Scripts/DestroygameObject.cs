using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroygameObject: MonoBehaviour
{

    public float lifetime = 3.0f;
    void Start()
    {
        Destroy(gameObject, lifetime);
    }

}
