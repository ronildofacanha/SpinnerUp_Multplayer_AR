using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LookAt : MonoBehaviour
{
    public GameObject lookAtMe; // Obj com TAG
    private Transform alvo;
    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag(lookAtMe.tag);
        alvo = player.GetComponent<Transform>();
    }

    void Update()
    {
            transform.LookAt(alvo);
    }
}
