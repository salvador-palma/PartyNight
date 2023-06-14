using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class GameUI : NetworkBehaviour
{
    public static GameUI Instance;
    [SerializeField] private GameObject TutorialPanel;
    [SerializeField] private GameObject CountdownPanel;
    [SerializeField] private GameObject WinPanel;
    [SerializeField] private GameObject WinWaitingUI;
    [SerializeField] private GameObject WinLeaderboardUI;
    [SerializeField] private Transform leaderboardContainer;
    [SerializeField] private Transform leaderboardTemplate;
    
    [SerializeField] private Transform PlayerPrefab;

    [SerializeField] private TimerToNext NextTimer;
    private void Awake() {
        Instance = this;
    }
    public void setReady(){
        leaderboardTemplate.gameObject.SetActive(false);
        TutorialPanel.SetActive(false);
        CountdownPanel.SetActive(true);
        GameState.Instance.setPlayerReadyServerRpc();
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
        GameState.Instance.ResetDicts();
        GameState.Instance.LoadNextGame();
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
            int i = 0;
            foreach(ulong clientID in NetworkManager.Singleton.ConnectedClientsIds){
                Transform PlayerTr = Instantiate(PlayerPrefab);
                Vector2[] positions = MiniGame.Instance.SpawnPoints;
                PlayerTr.transform.position = positions[i];
                i++;
                PlayerTr.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientID, true);
            }
        }
        
    }

    

   
}
