using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class Basket : NetworkBehaviour 
{
    [SerializeField] CatchaTon Game;
    [SerializeField] int team;

    private void OnTriggerEnter2D(Collider2D other) {
        
        if(other.tag == "Player"){
            NetworkObject nObj = other.transform.parent.parent.GetComponent<NetworkObject>();
            if(!nObj.IsOwner){return;}
            
            nObj.gameObject.GetComponent<PlayerNetworkCatchaTon>().Flush(team);
            
        }
    }
}
