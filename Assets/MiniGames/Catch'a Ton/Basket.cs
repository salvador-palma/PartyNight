using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class Basket : NetworkBehaviour 
{
    [SerializeField] CatchaTon Game;
    [SerializeField] int team;

    private void OnTriggerEnter2D(Collider2D other) {
        if(!IsServer){return;}
        if(other.tag == "Player"){
            other.GetComponent<PlayerNetworkCatchaTon>().Flush(team);
        }
    }
}