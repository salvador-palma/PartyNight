using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Lobbies.Models;
using TMPro;
using UnityEngine.UI;
public class LobbyTemplate : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI lobbyName;
    [SerializeField] private TextMeshProUGUI lobbyCapacity;
    
    private Lobby lobby;

    private void Awake() {
        GetComponent<Button>().onClick.AddListener(()=>{
            LobbyUI.Instance.FadeOutScroller();
            LobbyManager.Instance.JoinID(lobby.Id);
        });
    }
    public void SetLobby(Lobby lobby){
        this.lobby = lobby;
        lobbyName.text = lobby.Name;
        int n = lobby.AvailableSlots;
        int cap = lobby.MaxPlayers - n;
        lobbyCapacity.text = cap + "/" + lobby.MaxPlayers;
    }
}
