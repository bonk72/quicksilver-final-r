using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPwrUp : MonoBehaviour
{
    public float atkIncr;
    public float spdIncr;
    public float dashReduce;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnButtonPress()
    {

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        weapon weap = player.GetComponentInChildren<weapon>();
        Movement move = player.GetComponent<Movement>();

        move.increaseCurrMoveSpeed(spdIncr);
        weap.incrAtkMult(atkIncr);
        move.reduceDashCD(dashReduce);
        GetComponentInParent<powerUpScreen>().ButtonClicked(this.gameObject);
        

    }
    

}
