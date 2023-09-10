using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;


public class GameUI : NetworkBehaviour
{
    public static GameUI Instance;


    [SerializeField] private Transform leaderboardContainer;
    [SerializeField] private Transform leaderboardTemplate;

    [SerializeField] private Animator Scroller;
    [SerializeField] private Image TutorialImage;
    [SerializeField] private TextMeshProUGUI descriptionText;

    private Animator MiniGameIntroAnim;
    

    [SerializeField] private TimerToNext NextTimer;
    private void Awake() {
        Instance = this;
    }
    private void Start() {
        MiniGameIntroAnim = GameObject.Find("GameEffect").GetComponent<Animator>();
        leaderboardTemplate.gameObject.SetActive(false);
    }

    public void SetMinigameDetails(Sprite tutorial, string descr){
        TutorialImage.sprite = tutorial;
        descriptionText.text = descr;
    }
    public void setReady(){
        
        Scroller.Play("ReadyTransition");
        MiniGameIntroAnim.Play("Outro");
        //CountdownPanel.SetActive(true);
       
        GameState.Instance.setPlayerReadyServerRpc();
    }

    public void ShowWin(){
        Scroller.Play("FinishLine");
    }

    public void InitLeaderboard(){
        Scroller.Play("Finish");
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
        yield return new WaitForSecondsRealtime(7);
        StartMiniGameFadeOutClientRpc();
        
    }
    [ClientRpc]
    public void StartMiniGameFadeOutClientRpc(){
         Scroller.Play("MiniGameFadeOut");
    }
    public void AddLeaderBoard(PlayerData playerData){
        Transform leaderTr = Instantiate(leaderboardTemplate, leaderboardContainer);
        leaderTr.Find("Nickname").GetComponent<TextMeshProUGUI>().text = playerData.nickname.ToString();
        leaderTr.Find("Points").GetComponent<TextMeshProUGUI>().text = playerData.points.ToString();
        Transform pointsadd = leaderTr.Find("PointsAdd"); 
        if(pointsadd != null){pointsadd.GetComponent<TextMeshProUGUI>().text = "+" + playerData.added_points.ToString();}
        
        leaderTr.gameObject.SetActive(true); 
    }
    

    public void setAnimatorCountdown(){
        Scroller.Play("CountDown");
        if(IsServer){
            MiniGame.Instance.MiniGameExtension.SpawnPlayers();
        }
        
    }

    public void next(){
        if(IsServer){
            GameState gs = GameState.Instance;
            gs.ResetDicts();
            if(gs.hasWinner()){
                gs.LoadWinningScreen();
            }else{
                gs.LoadNextGame();
            }
            
        }
    }

    public void playMiniGameIntro(){
        MiniGameIntroAnim.Play("Intro");
    }

    public void ShowTutorial(){
        Debug.Log("ShowTutorial");
        MiniGameIntroAnim.transform.Find("Tutorial").GetComponent<Animator>().Play("ShowTutorial");
    }

    

   
}
