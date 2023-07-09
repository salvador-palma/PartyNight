using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using Unity.Netcode;
using System;

public class Marks : NetworkBehaviour {
 
    [SerializeField] float X;
    [SerializeField] private TugOfWar tugOfWar;
    private bool over = false;

    private void Start() {
        if(!IsServer){
            GetComponent<Marks>().enabled = false;
            return;
        }
        
    }

    private void Update() {
        int s = checkMark();
        if(s != 0 && !over){
            over = true;
            if(s==1){
                tugOfWar.EndGame("Mark1");
            }else{
                tugOfWar.EndGame("Mark2");
            }
        }
    }
    private int checkMark(){
        if(transform.position.x < -X){
            return 2;  
        }else if(transform.position.x > X){
            return 1;
        }
        return 0;
    }
}
