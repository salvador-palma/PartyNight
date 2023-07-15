using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class Pellet : NetworkBehaviour
{
    
    private int Type;
    private void Start() {
        Type = Random.Range(0,3);
        GetComponent<SpriteRenderer>().sprite = ((CatchaTon)MiniGame.Instance.MiniGameExtension).sprites[Type];
    }
    private void OnTriggerEnter2D(Collider2D other) {
        NetworkObject nObj = other.GetComponent<NetworkObject>();
        
        if(!nObj.IsOwner){return;}
        if(other.tag == "Player"){
            PlayerNetworkCatchaTon p = other.GetComponent<PlayerNetworkCatchaTon>();
            if(!p.isFull()){
                
                GetComponent<SpriteRenderer>().enabled = false;
                destroyObjectServerRpc();
                
                p.Pick(Type);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void destroyObjectServerRpc(){
        GetComponent<NetworkObject>().Despawn();
    }
}
