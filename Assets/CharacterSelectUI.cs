using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.Services.Lobbies.Models;
public class CharacterSelectUI : MonoBehaviour
{
    [SerializeField] private Button readyBtn;
    [SerializeField] private Button menuBtn;
    [SerializeField] private TextMeshProUGUI codeText;
    
    private void Awake() {
        readyBtn.onClick.AddListener(()=>{
            CharacterSelect.Instance.setPlayerReady();
        });
        menuBtn.onClick.AddListener(()=>{
            LobbyManager.Instance.LeaveLobby();
            NetworkManager.Singleton.Shutdown();
            SceneManager.LoadScene("StartMenu");
        });
    }

    private void Start() {
        Lobby lobby = LobbyManager.Instance.getLobby();
        codeText.text = lobby.LobbyCode;
    }
}
