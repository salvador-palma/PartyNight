using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNetworkStatic : PlayerNetwork
{

    public override void Start() {
        PlayerData playerData = GameState.Instance.getPlayerData(OwnerClientId);
        skinTr.gameObject.GetComponent<SpriteRenderer>().color = GameState.Instance.getColor(playerData.colorID);
        skinTr.Find("Hair").GetComponent<SpriteRenderer>().sprite = Skins.GetInstance().getHair(playerData.hairID);
        skinTr.Find("Eyes").GetComponent<SpriteRenderer>().sprite = Skins.GetInstance().getEye(playerData.eyesID);
    }
    public override void Update()
    {
        if(!IsOwner || Finished){return;}
        if(Input.GetKeyDown(KeyCode.Space)){
            MiniGame.Instance.MiniGameExtension.Interact(OwnerClientId);
        }
        
    }
    public override void FixedUpdate() {
       
    }
}
