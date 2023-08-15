using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;
using Microsoft.Unity.VisualStudio.Editor;

public class GameUI : NetworkBehaviour
{
    public static GameUI Instance;

    [SerializeField] private GameObject CountdownPanel;
    [SerializeField] private GameObject WinPanel;
    [SerializeField] private GameObject WinWaitingUI;
    [SerializeField] private GameObject WinLeaderboardUI;
    [SerializeField] private Transform leaderboardContainer;
    [SerializeField] private Transform leaderboardTemplate;

    [SerializeField] private Animator Scroller;
    [SerializeField] private UnityEngine.UI.Image TutorialImage;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private string description;
    [SerializeField] private Sprite tutorial;
    
    

    [SerializeField] private TimerToNext NextTimer;
    private void Awake() {
        Instance = this;
    }
    private void Start() {
        TutorialImage.sprite = tutorial;
        descriptionText.text = description;
    }
    public void setReady(){
        leaderboardTemplate.gameObject.SetActive(false);
        Scroller.Play("ReadyTransition");
        CountdownPanel.SetActive(true);
       
        // GameState.Instance.setPlayerReadyServerRpc();
    }

    public void ShowWin(){
        WinPanel.SetActive(true);
        WinWaitingUI.SetActive(true);
        WinLeaderboardUI.SetActive(false);
    }

    public void InitLeaderboard(){
        WinWaitingUI.SetActive(false);
        WinLeaderboardUI.SetActive(true);
        foreach(Transform t in leaderboardContainer){
            if(t == leaderboardTemplate) continue;
            Destroy(t.gameObject);
        }
        NextTimer.activated = true;
        if(IsServer){
            //Debug.Log("HERE DEBUG!");
            StartCoroutine("NextGame");
        }
    }

    IEnumerator NextGame(){
        yield return new WaitForSecondsRealtime(5);
        StartMiniGameFadeOutClientRpc();
        
    }
    [ClientRpc]
    public void StartMiniGameFadeOutClientRpc(){
         Scroller.Play("MiniGameFadeOut");
    }
    public void AddLeaderBoard(PlayerData playerData){
        Transform leaderTr = Instantiate(leaderboardTemplate, leaderboardContainer);
        leaderTr.Find("Name").GetComponent<TextMeshProUGUI>().text = playerData.nickname.ToString();
        leaderTr.Find("Points").GetComponent<TextMeshProUGUI>().text = playerData.points.ToString();
        leaderTr.gameObject.SetActive(true); 
    }
    

    public void setAnimatorCountdown(){
        CountdownPanel.GetComponent<Animator>().Play("Countdown");
        if(IsServer){
            MiniGame.Instance.MiniGameExtension.SpawnPlayers();
        }
        
    }

    public void next(){
        if(IsServer){
            GameState gs = GameState.Instance;
            if(gs.hasWinner()){
                gs.LoadWinningScreen();
            }else{
                GameState.Instance.ResetDicts();
                GameState.Instance.LoadNextGame();
            }
            
        }
    }

    

   
}
