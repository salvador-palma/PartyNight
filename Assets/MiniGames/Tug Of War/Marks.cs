using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using Unity.Netcode;
using System;
using Unity.Netcode.Components;

public class Marks : NetworkBehaviour {
 
    [SerializeField] float X;
    [SerializeField] private TugOfWar tugOfWar;
    private bool over = false;
    [SerializeField] private Transform[] spawnpoints;
    private float timeToFadeOut = 2f;
    private string tempStr;
    private void Start() {
        if(!IsServer){
            GetComponent<Marks>().enabled = false;
            return;
        }
        
    }

    private void Update() {

        if(over){
            if(timeToFadeOut > 0f){
                timeToFadeOut -= Time.deltaTime;
                if(timeToFadeOut <= 0){
                    tugOfWar.EndGame(tempStr);
                }
            }
            return;
        }
        if(transform.position.x > 1.57f){
            
            End(1);
            //tugOfWar.EndGame("Mark1");
        }else if(transform.position.x < -1.57f){
            
            End(2);
            //tugOfWar.EndGame("Mark2");
        } 
        
    }

    public Transform[] getSpawnpoints(){
        return spawnpoints;
    }

    public void End(int teamWinner){
        over = true;
        tugOfWar.ResetCameraClientRpc();
        if(teamWinner == 1){
            tempStr = "Mark1";
            GetComponent<NetworkAnimator>().SetTrigger("Win1");
        }else{
            tempStr = "Mark2";
            GetComponent<NetworkAnimator>().SetTrigger("Win2");
        }

    }
    
}
