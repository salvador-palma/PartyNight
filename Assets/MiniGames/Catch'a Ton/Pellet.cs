using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class Pellet : NetworkBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) {
        if(!IsServer){return;}
        if(other.tag == "Player"){

            bool res = other.GetComponent<PlayerNetworkCatchaTon>().Pick();
            if(res){
                GetComponent<NetworkObject>().Despawn();
            }
            
        }
    }
}
