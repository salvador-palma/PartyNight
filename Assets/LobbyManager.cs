using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Authentication;
using TMPro;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{

    private Lobby host;
    private Lobby joinedLobby;

    [SerializeField] RelayManager relayManager;
    private float pingTimer = 20;

    [SerializeField] private GameObject LobbyPanel;
    [SerializeField] private GameObject LogInPanel;
    [SerializeField] private TextMeshProUGUI NickField;
    [SerializeField] private TextMeshProUGUI CodeField;
    [SerializeField] private GameObject PlayerTemplate;
    private Transform PlayerContainer;

    [SerializeField] private GameObject LobbyTemplate;
    private float updateTimer = 1.1f;
    // Start is called before the first frame update
    private async void Start()
    {

        InitializationOptions initializationOptions = new InitializationOptions();
        initializationOptions.SetProfile(Random.Range(0,1000000).ToString());
        await UnityServices.InitializeAsync();
        AuthenticationService.Instance.SignedIn += () => {
            Debug.Log("Signed In: " + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

    }
    private void Update() {
        
        HandlePing();
        HandleUpdate();
        
    }
    private async void HandlePing(){
        if(host != null){
            pingTimer -= Time.deltaTime;
            if(pingTimer<0){
                pingTimer=20;
                await LobbyService.Instance.SendHeartbeatPingAsync(host.Id);
            }
        }
    }
    private async void HandleUpdate(){
        if(joinedLobby != null){
            updateTimer -= Time.deltaTime;
            if(updateTimer<0){
                updateTimer=1.1f;
                Lobby lobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
                joinedLobby = lobby;
                UpdateLobbyUI();

                if(joinedLobby.Data["KeyToStart"].Value != "0"){

                    if(!isHost()){
                        relayManager.JoinRelay(joinedLobby.Data["KeyToStart"].Value);
                    }
                    //joinedLobby = null;
                    LobbyPanel.SetActive(false);
                }
            }
        }
    }

    public async void CreateLobby(){
        try{
            string lobbyName = "Test Lobby";
            int maxPlayers = 8;

            CreateLobbyOptions options = new CreateLobbyOptions{
                IsPrivate = false,
                Player = new Player{
                    Data = new Dictionary<string, PlayerDataObject>{
                        { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, NickField.text) },
                        { "PlayerColor", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, "#FFFFFF")}
                    }
                },
                Data = new Dictionary<string, DataObject>{
                    {"KeyToStart", new DataObject(DataObject.VisibilityOptions.Member, "0")}
                }
            };
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
            Debug.Log("Lobby Created: " + lobby.Name + " " + lobby.LobbyCode);
            host = lobby;
            joinedLobby = host;

        }catch(LobbyServiceException e){
            Debug.Log(e);
        }
        

        //UI
        EnterLobbyUI();
    }

 
    public bool isHost(){
        if(host == null){return false;}
        return host.LobbyCode == joinedLobby.LobbyCode;
    }

    public async void JoinLobby(){
        try{
            JoinLobbyByCodeOptions options = new JoinLobbyByCodeOptions{
                Player = new Player{
                    Data = new Dictionary<string, PlayerDataObject>{
                        { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, NickField.text) },
                        { "PlayerColor", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, "#FFFFFF")}
                    }
                }
            };
            string code = CodeField.text;
            code = code.Remove(code.Length - 1);
            Lobby lobby = await Lobbies.Instance.JoinLobbyByCodeAsync(code, options);
            joinedLobby = lobby;
            Debug.Log("Joined Lobby: " + lobby.LobbyCode);
            // getPlayers(host);

            //UI
            
        }catch(LobbyServiceException e){
            Debug.Log(e);
        }
        EnterLobbyUI();
        
    }

    // public void getPlayers(Lobby lobby){
    //     Debug.Log("Player in lobby " + lobby.LobbyCode);
    //     foreach(Player p in lobby.Players){
    //         Debug.Log(p.Data["PlayerName"].Value);
    //     }
    // }

    public async void UpdatePlayerColor(string color){
        try{
            await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId, new UpdatePlayerOptions{
                Data = new Dictionary<string, PlayerDataObject>{
                    { "PlayerColor", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, color)}
                }    
            });
        }catch(LobbyServiceException e){
            Debug.Log(e);
        }
    }

    public void LeaveLobby(){ 
        try{
            LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
            Debug.Log("Left Lobby: " + joinedLobby.LobbyCode);
            joinedLobby = null;
            host = null;
            
            //UI
            LobbyPanel.SetActive(false);
            LogInPanel.SetActive(true);
        }catch(LobbyServiceException e){
            Debug.Log(e);
        }
    }
    private void Kick(int index ){ 
        try{
            LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, joinedLobby.Players[index].Id);
        }catch(LobbyServiceException e){
            Debug.Log(e);
        }
    }

    private async void changeHost(){
        try{
            host = await Lobbies.Instance.UpdateLobbyAsync(host.Id, new UpdateLobbyOptions{
                HostId = joinedLobby.Players[1].Id
            });
        }catch(LobbyServiceException e){
            Debug.Log(e);
        }
    }

    private async void DeleteLobby(int index ){ 
        try{
            await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);
        }catch(LobbyServiceException e){
            Debug.Log(e);
        }
    }


    //UI METHODS
    private void EnterLobbyUI(){
        LobbyPanel.SetActive(true);
        LogInPanel.SetActive(false);
        LobbyPanel.transform.Find("Header").GetComponent<TextMeshProUGUI>().text = joinedLobby.LobbyCode;
        PlayerContainer = LobbyPanel.transform.Find("ContentPlayer");

        UpdateLobbyUI();
    }

    private void UpdateLobbyUI(){
        foreach(Transform t in PlayerContainer){
            Destroy(t.gameObject);
        }

        foreach(Player p in joinedLobby.Players){
            GameObject go = Instantiate(PlayerTemplate, PlayerContainer);
            go.transform.Find("Nickname").GetComponent<TextMeshProUGUI>().text = p.Data["PlayerName"].Value;

            Color newCol;
            if (ColorUtility.TryParseHtmlString(p.Data["PlayerColor"].Value, out newCol))
            {
                go.transform.Find("Avatar").GetComponent<Image>().color = newCol;
            }
            
            
        }
    }

    public async void StartGame(){
        try{
            string relayCode = await relayManager.CreateRelay();

            Lobby lobby = await Lobbies.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions{
                Data = new Dictionary<string, DataObject>{
                    {"KeyToStart", new DataObject(DataObject.VisibilityOptions.Member, relayCode)}
                }
            });

            joinedLobby = lobby;

            var status = NetworkManager.Singleton.SceneManager.LoadScene("Game", LoadSceneMode.Single);
            if(status != SceneEventProgressStatus.Started){
                Debug.Log("Failed to load Scene");
            }
        }catch(LobbyServiceException e){
            Debug.Log(e);
        }
    }
}
