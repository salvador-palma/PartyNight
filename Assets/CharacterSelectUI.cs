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
    [SerializeField] private TextMeshProUGUI codeTextShadow;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI nameTextShadow;
    
    private void Awake() {
        readyBtn.onClick.AddListener(()=>{
            readyBtn.GetComponent<Animator>().Play("ReadyButton");
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
        codeTextShadow.text = lobby.LobbyCode;
        nameText.text = lobby.Name;
        nameTextShadow.text = lobby.Name;

        
    }
}
