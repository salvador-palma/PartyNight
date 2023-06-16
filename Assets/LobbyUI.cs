using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.Services.Lobbies.Models;
using System;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Button create;
    [SerializeField] private Button join;
    [SerializeField] private Button joinCode;

    [SerializeField] private TMP_InputField lobbyNameField; 
    [SerializeField] private TMP_InputField lobbyCodeField; 
    [SerializeField] private TMP_InputField nicknameField; 
    [SerializeField] private Toggle privateToggle; 

    [SerializeField] private Transform LobbyContainer;
    [SerializeField] private Transform LobbyTemplate;

    [SerializeField] private Animator ScrollerAnimator;

    public static LobbyUI Instance;

    private void Awake() {
        Instance = this;
        create.onClick.AddListener(()=> {
            FadeOutScroller();
            LobbyManager.Instance.CreateLobby(lobbyNameField.text, privateToggle.isOn);
        });
        join.onClick.AddListener(()=> {
            FadeOutScroller();
            LobbyManager.Instance.QuickJoin();});
        joinCode.onClick.AddListener(()=> {
            FadeOutScroller();
            LobbyManager.Instance.JoinCode(lobbyCodeField.text);});
        LobbyTemplate.gameObject.SetActive(false);

    }

    private void Start() {
        nicknameField.text = GameState.Instance.getPlayerName();
        nicknameField.onValueChanged.AddListener((string newText) => {
            GameState.Instance.setPlayerName(newText);
        });

        LobbyManager.Instance.OnLobbyListChanged += LobbyListChanger;
        UpdateLobbyList(new List<Lobby>());
    }

    private void LobbyListChanger(object sender, LobbyManager.OnLobbyListChangedEventsArgs e)
    {
        UpdateLobbyList(e.LobbyList);
    }

    private void UpdateLobbyList(List<Lobby> LobbyList){
        foreach(Transform t in LobbyContainer){
            if(t == LobbyTemplate) continue;
            Destroy(t.gameObject);
        }

        foreach(Lobby l in LobbyList){
            Transform lobbyTr = Instantiate(LobbyTemplate, LobbyContainer);
            lobbyTr.gameObject.SetActive(true);
            lobbyTr.GetComponent<LobbyTemplate>().SetLobby(l);
        }


    }

    private void OnDestroy() {
        LobbyManager.Instance.OnLobbyListChanged -= LobbyListChanger;
    }

    public void FadeOutScroller(){
        //ScrollerAnimator.gameObject.GetComponent<Scroller>().activated = true;
        ScrollerAnimator.Play("LobbyScreenFadeOut");
    }
}
