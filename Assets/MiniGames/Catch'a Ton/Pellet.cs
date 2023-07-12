using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class Pellet : NetworkBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) {
        NetworkObject nObj = other.GetComponent<NetworkObject>();
        
        if(!nObj.IsOwner){return;}
        if(other.tag == "Player"){
            PlayerNetworkCatchaTon p = other.GetComponent<PlayerNetworkCatchaTon>();
            if(p.ball_amount < PlayerNetworkCatchaTon.MAX_BALLS){
                destroyObjectServerRpc();
                p.Pick();
            }
            
            
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void destroyObjectServerRpc(){
        GetComponent<NetworkObject>().Despawn();
    }
}
