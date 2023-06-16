using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Unity.Netcode;
using System;
using System.Linq;

public class GameState : NetworkBehaviour {




    enum State{WAITING, INGAME, FINISHED}
    [HideInInspector] public static GameState Instance {get; private set;}

    private static Dictionary<ulong, bool> PlayerReadyDict = new Dictionary<ulong, bool>();
    
    private NetworkList<PlayerData> playerDatas;

    private static Dictionary<ulong, int> PlayerEndDict = new Dictionary<ulong, int>();
    private static Dictionary<ulong, int> PlayerPointsDict = new Dictionary<ulong, int>();


    private NetworkVariable<State> state = new NetworkVariable<State>(State.WAITING);

    public event EventHandler OnPlayerDataNetworkListChanged;

    
    [SerializeField] private List<Color> playerColors;
    [SerializeField] private List<string> Minigames;
    [SerializeField] private List<int> PointsPerPosition;
    private int rankPos;
    private string PlayerName;
    
    private void Awake() {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        PlayerName = PlayerPrefs.GetString("Nick", "PlayerName" + UnityEngine.Random.Range(0,1100000));
        playerDatas = new NetworkList<PlayerData>();
        playerDatas.OnListChanged += playerDatas_OnListChanged;

        
    }

    private void InitEndClients(){
        rankPos=0;
        foreach(ulong id in NetworkManager.Singleton.ConnectedClientsIds){
            PlayerEndDict[id] = -1;
        }
    }

    private void playerDatas_OnListChanged(NetworkListEvent<PlayerData> changedList){
        OnPlayerDataNetworkListChanged?.Invoke(this, EventArgs.Empty);
    }  

    public string getPlayerName(){
        return PlayerName;
    }
    public void setPlayerName(string n){
        PlayerName = n;
        PlayerPrefs.SetString("Nick", n);
    }

    public void StartHost(){
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += onServerDisconnect;
        NetworkManager.Singleton.StartHost();
    }
    public void StartClient(){

       // NetworkManager.Singleton.OnClientDisconnectCallback += onClientDisconnect;
        NetworkManager.Singleton.OnClientConnectedCallback += onClientDisconnect;
        NetworkManager.Singleton.StartClient();
    }

    private void onClientDisconnect(ulong obj)
    {
        changeNickServerRpc(getPlayerName());
    }

    [ServerRpc(RequireOwnership=false)]
    private void changeNickServerRpc(string nick, ServerRpcParams serverRpcParams = default){
        int playerIndex = getPlayerDataID(serverRpcParams.Receive.SenderClientId);
        PlayerData playerData = playerDatas[playerIndex];
        playerData.nickname = nick;
        playerDatas[playerIndex] = playerData;
    }

    private void onServerDisconnect(ulong clientID)
    {
        for (int i = 0; i < playerDatas.Count; i++)
        {
            PlayerData playerData = playerDatas[i];
            if(playerData.ID == clientID){
                playerDatas.RemoveAt(i);
            }
        }
    }

    private void NetworkManager_OnClientConnectedCallback(ulong clientID){
        
        playerDatas.Add(new PlayerData{
            ID = clientID,
            colorID = GetDefaultColor(),
            points = 0
        });
        changeNickServerRpc(getPlayerName());
    }

    private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse){
        if(SceneManager.GetActiveScene().name != "CharacterSelect"){
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game has already started";
            return;
        }
        if(NetworkManager.Singleton.ConnectedClientsIds.Count >= 8){
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game is full";
            return;
        }
        connectionApprovalResponse.Approved = true;
    }
    

    [ServerRpc(RequireOwnership = false)]
    public void setPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default){
        PlayerReadyDict[serverRpcParams.Receive.SenderClientId] = true;

        bool AllReady = true;
        foreach (ulong clientID in NetworkManager.Singleton.ConnectedClientsIds){
            if(!PlayerReadyDict.ContainsKey(clientID) || !PlayerReadyDict[clientID]){
                AllReady = false;
                break;
            }
        }
        if(AllReady){
            InitEndClients();
            setPlayerReadyClientRpc();
        }
    }

    [ClientRpc]
    public void setPlayerReadyClientRpc(){
        GameUI.Instance.setAnimatorCountdown();
    }

    [ServerRpc(RequireOwnership = false)]
    public void changeStateServerRpc(){
        state.Value = State.INGAME;
    }


    public bool IsPlayerIndexConnected(int index){
        
        return index < playerDatas.Count;
    }
    
    public PlayerData getPlayerData(int index){
        return playerDatas[index];
    }


    public Color getColor(int colorID){
        return playerColors[colorID];
    }

    public PlayerData getLocalPlayerData(){
        return getPlayerData(NetworkManager.Singleton.LocalClientId);
    }

    public PlayerData getPlayerData(ulong clientID){
        foreach(PlayerData p in playerDatas){
           if(p.ID == clientID){
                return p;
           } 
        }
        return default;
    }

    public int getPlayerDataID(ulong clientID){
        for (int i = 0; i < playerDatas.Count; i++)
        {
            if(playerDatas[i].ID == clientID){
                return i;
            }
        }
        return -1;
    }

    public void ChangePlayerColor(int colorID){
        ChangePlayerColorServerRpc(colorID);
    }

    [ServerRpc(RequireOwnership = false)]
    public void ChangePlayerColorServerRpc(int colorID, ServerRpcParams serverRpcParams = default){
        if(!isColorAvail(colorID)){
            return;
        }
        int playerIndex = getPlayerDataID(serverRpcParams.Receive.SenderClientId);
        PlayerData playerData = playerDatas[playerIndex];
        playerData.colorID = colorID;
        playerDatas[playerIndex] = playerData;
    }

    

    private bool isColorAvail(int colorID){
        foreach(PlayerData p in playerDatas){
            if(p.colorID == colorID){
                return false;
            }
        }
        return true;
    }

    private int GetDefaultColor(){
        for (int i = 0; i < playerColors.Count; i++)
        {
            if(isColorAvail(i)){
                return i;
            }
        }
        return -1;
    }

    public void Kick(ulong clientID){
        NetworkManager.Singleton.DisconnectClient(clientID);
    }

    

    [ServerRpc(RequireOwnership = false)]
    public void PlayerFinishServerRpc(ServerRpcParams serverRpcParams = default){
        PlayerEndDict[serverRpcParams.Receive.SenderClientId] = rankPos;
        rankPos++;
        bool AllReady = true;
        foreach (ulong clientID in NetworkManager.Singleton.ConnectedClientsIds){
            
            if(!PlayerEndDict.ContainsKey(clientID) || PlayerEndDict[clientID] == -1){
                AllReady = false;
                break;
            }
        }
        if(AllReady){
            if(MiniGame.Instance.IsByScore){
                givePointsByScores();
            }else{
                givePointsByArrival();
            }
            
            InitLeaderboardClientRpc();
            List<PlayerData> DataList = new List<PlayerData>();
            foreach(PlayerData playerData in playerDatas){
                DataList.Add(playerData);
            }
            DataList.Sort();
            foreach (PlayerData d in DataList)
            {
                AddLeaderboardClientRpc(d);
                
            }
        }
    }

    public string getRandomMinigame(){
        return Minigames[UnityEngine.Random.Range(0, Minigames.Count)];
    }

    private void givePointsByArrival(){
        int i = 0;
        foreach(ulong id in PlayerEndDict.Keys){
            PlayerData playerData =  getPlayerData(id);
            playerData.points += PointsPerPosition[PlayerEndDict[id]];
            i++;
            playerDatas[getPlayerDataID(id)] = playerData;
        }
    }
    private void givePointsByScores(){

        Dictionary<ulong, int> transformedDictionary = PlayerPointsDict.OrderByDescending(pair => pair.Value).Select((pair, index) => new { pair.Key, Index = index }).ToDictionary(item => item.Key, item => item.Index);
        int i = 0;
        foreach(ulong id in transformedDictionary.Keys){
            PlayerData playerData =  getPlayerData(id);
            playerData.points += PointsPerPosition[transformedDictionary[id]];
            i++;
            playerDatas[getPlayerDataID(id)] = playerData;
        }
    }

    [ServerRpc(RequireOwnership=false)]
    public void AddPointServerRpc(int amount, ServerRpcParams serverRpcParams = default){
        Debug.Log("DEBUGGING: " + serverRpcParams.Receive.SenderClientId);
        if(PlayerPointsDict.ContainsKey(serverRpcParams.Receive.SenderClientId)){
            PlayerPointsDict[serverRpcParams.Receive.SenderClientId] += amount;
        }else{
            PlayerPointsDict[serverRpcParams.Receive.SenderClientId] = amount;
        }
        
    }
    [ServerRpc(RequireOwnership=false)]
    public void DebugPointsServerRpc(ServerRpcParams serverRpcParams = default){
        foreach(ulong id in PlayerPointsDict.Keys){
            Debug.Log("User: " + id + " with " + PlayerPointsDict[id] + " points");
        }
    }

    [ClientRpc]
    private void InitLeaderboardClientRpc(){
        GameUI.Instance.InitLeaderboard();
    }
    [ClientRpc]
    private void AddLeaderboardClientRpc(PlayerData pd){
        
        GameUI.Instance.AddLeaderBoard(pd);
    }

    private void debugPoints(){
        foreach(PlayerData p in playerDatas){
            Debug.Log("Player: " + p.ID + " with " + p.points + " points");
        }
    }


    public void LoadNextGame(){
        NetworkManager.Singleton.SceneManager.LoadScene(GameState.Instance.getRandomMinigame(), LoadSceneMode.Single);
    }
    
    public void ResetDicts(){
        PlayerReadyDict.Clear();
        PlayerEndDict.Clear();
    }

    



}

