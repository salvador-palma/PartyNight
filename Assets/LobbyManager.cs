using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Lobbies;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using System;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using System.Threading.Tasks;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;

public class LobbyManager : MonoBehaviour
{

    public static LobbyManager Instance;
    
    public event EventHandler<OnLobbyListChangedEventsArgs> OnLobbyListChanged;
    public class OnLobbyListChangedEventsArgs : EventArgs{
        public List<Lobby> LobbyList;
    }

    private float HeartBeatTimer = 25f;
    private float LobbyUpdateTimer = 3f;
    private Lobby joinedLobby;
    private void Awake() {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitUnityAuth();
    }

    private async void InitUnityAuth(){

        if(UnityServices.State != ServicesInitializationState.Initialized){
            InitializationOptions initializationOptions = new InitializationOptions();
            initializationOptions.SetProfile(UnityEngine.Random.Range(0,100000000).ToString());
            await UnityServices.InitializeAsync(initializationOptions);
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

        }
    }

    private void Update() {
        HandleHeartBeat();
        HandleLobbyListUpdate();
    }

    private void HandleLobbyListUpdate()
    {
        if (joinedLobby == null && AuthenticationService.Instance.IsSignedIn && SceneManager.GetActiveScene().name == "Lobby")
        {
            LobbyUpdateTimer-= Time.deltaTime;
            if(LobbyUpdateTimer<0){
                LobbyUpdateTimer = 3f;
                ListLobbies();
            }
        }
        
    }

    private void HandleHeartBeat(){
        if(isLobbyHost()){
            HeartBeatTimer-= Time.deltaTime;
            if(HeartBeatTimer<0){
                HeartBeatTimer = 25f;
                LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
            }
        }
    }

    private bool isLobbyHost(){
        return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }
    public async void CreateLobby(string lobbyName, bool isPrivate){
        try{
            Logo logo = GameObject.Find("Logo").GetComponent<Logo>();
            logo.newTask(5);
            logo.Progress();
            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, 8, new CreateLobbyOptions{
                IsPrivate = isPrivate
            });
            logo.Progress();

            Allocation allocation = await AllocateRelay();
            logo.Progress();
            string relayCode = await GetRelayCode(allocation);
            logo.Progress();
            await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions{
                Data = new Dictionary<string, DataObject>{
                    {"RelayCode", new DataObject(DataObject.VisibilityOptions.Member, relayCode)}
                }
            });
            logo.Progress();
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));
            GameState.Instance.StartHost();
            logo.Progress();
            NetworkManager.Singleton.SceneManager.LoadScene("CharacterSelect", LoadSceneMode.Single);
            
        }catch(LobbyServiceException e){
            Debug.Log(e);
        }
        
    }

    private async Task<JoinAllocation> JoinRelay(string code){
       try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(code);
            return joinAllocation;
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
            return default;
        }
    }
    private async Task<string> GetRelayCode(Allocation allocation){
        try
        {
            String code = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            return code;
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
            return default;
        }
    }

    private async Task<Allocation> AllocateRelay(){
       try{
            Allocation allocation =  await RelayService.Instance.CreateAllocationAsync(7);
            return allocation;
        }catch(RelayServiceException e){
            Debug.Log(e);
            return default;
        }
    }
    public async void QuickJoin(){
        try
        {
            joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();
            string relayCode = joinedLobby.Data["RelayCode"].Value;
            JoinAllocation joinAllocation = await JoinRelay(relayCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

            GameState.Instance.StartClient();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
        
    }

    public Lobby getLobby(){
        return joinedLobby;
    }

    public async void JoinCode(string code){
        try
        {
            joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(code);
            string relayCode = joinedLobby.Data["RelayCode"].Value;
            JoinAllocation joinAllocation = await JoinRelay(relayCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
            GameState.Instance.StartClient();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
        
    }

    public async void JoinID(string id){
        try
        {
            Logo logo = GameObject.Find("Logo").GetComponent<Logo>();
            logo.newTask(4);
            joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(id);
            logo.Progress();
            string relayCode = joinedLobby.Data["RelayCode"].Value;
            JoinAllocation joinAllocation = await JoinRelay(relayCode);
            logo.Progress();
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
            logo.Progress();
            GameState.Instance.StartClient();
            logo.Progress();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
        
    }

    public async void DeleteLobby(){
        if(joinedLobby != null){
            try
            {
                await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);
                joinedLobby = null; 
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }

        }
    }
     public async void LeaveLobby(){
        if(joinedLobby != null){
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
                joinedLobby = null; 
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }

        }
     }

     private async void ListLobbies(){

        try{
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions{
                Filters = new List<QueryFilter>{
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                }
            };
            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);
            OnLobbyListChanged?.Invoke(this, new OnLobbyListChangedEventsArgs{
                LobbyList = queryResponse.Results
            });
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
        
        
        
     }

}
