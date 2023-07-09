using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class Events : MonoBehaviour
{
    public void startPlayer(){
        PlayerNetwork.LocalInstance.current_speed = PlayerNetwork.speed;
        GameState.Instance.changeStateServerRpc();
        try{
            MiniGame.Instance.Init();
        }catch{
            Debug.Log("Usual Error");
        }
        
    }

    public void startGame(){
        
        CharacterSelect.Instance.CharSelectFadeOutEventServerRpc();
        
        
    }

    public void nextGame(){
        GameState.Instance.ResetDicts();
        GameState.Instance.LoadNextGame();
    }
    
    
}
