using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Events : MonoBehaviour
{
    public void startPlayer(){
        PlayerNetwork.LocalInstance.current_speed = PlayerNetwork.speed;
        GameState.Instance.changeStateServerRpc();
    }
    
}
