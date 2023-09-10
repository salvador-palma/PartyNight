using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
        CharacterSelect.Instance.CharSelectFadeOutEvent();
    }

    public void readyGame(){
        GameState.Instance.setPlayerReadyServerRpc();
    }

    public void nextGame(){
        GameUI.Instance.next();
    }

    public void ResetLoad(Logo load){
        load.newTask(5);
    }
    
    public void DisableSelf(){
        gameObject.SetActive(false);
    }

    public void endGame(){
        GameObject.Find("MiniGame").GetComponent<Results>().endGame();
    }

    public void playMinigameIntro(){
        GameUI.Instance.playMiniGameIntro();
    }
    
    
}
