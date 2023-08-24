using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Unity.Netcode;
using System;
using System.Linq;

public class GameState : NetworkBehaviour {




    enum State{WAITING, INGAME, FINISHED}

    private int MAXPOINTS = 10;
    [HideInInspector] public static GameState Instance {get; private set;}

    private static Dictionary<ulong, bool> PlayerReadyDict = new Dictionary<ulong, bool>();
    
    public NetworkList<PlayerData> playerDatas;

    private NetworkVariable<State> state = new NetworkVariable<State>(State.WAITING);

    public event EventHandler OnPlayerDataNetworkListChanged;

    
    [SerializeField] private List<Color> playerColors;
    [SerializeField] private List<string> Minigames;
    [SerializeField] public List<int> PointsPerPosition;
    
    private string PlayerName;
    
    private void Awake() {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        PlayerName = PlayerPrefs.GetString("Nick", "PlayerName" + UnityEngine.Random.Range(0,1100000));
        playerDatas = new NetworkList<PlayerData>();
        playerDatas.OnListChanged += playerDatas_OnListChanged;

        
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
        changeNickServerRpc(getPlayerName(), SkinVisual.HairID, SkinVisual.EyeID);
        
    }

    [ServerRpc(RequireOwnership=false)]
    private void changeNickServerRpc(string nick,int hairID, int eyeID, ServerRpcParams serverRpcParams = default){
        int playerIndex = getPlayerDataID(serverRpcParams.Receive.SenderClientId);
        PlayerData playerData = playerDatas[playerIndex];
        playerData.nickname = nick;
        playerData.eyesID = eyeID;
        playerData.hairID = hairID;
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
        changeNickServerRpc(getPlayerName(), SkinVisual.HairID, SkinVisual.EyeID);
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
            setPlayerReadyClientRpc();
        }
    }

    [ClientRpc]
    public void setPlayerReadyClientRpc(){
        GameUI.Instance.setAnimatorCountdown();
        MiniGame.Instance.SetupBoards();
        
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

    
    [ServerRpc]
    public void FinishGameServerRpc(){
        
        MiniGame.Instance.MiniGameExtension.AddPoints();
        MiniGame.Instance.ResetCamClientRpc();
        InitLeaderboardClientRpc(true);
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
    public string getRandomMinigame(){
        return Minigames[UnityEngine.Random.Range(0, Minigames.Count)];
    }

    


    [ClientRpc]
    private void InitLeaderboardClientRpc(bool directConnect){
        if(directConnect){GameUI.Instance.ShowWin();}
        GameUI.Instance.InitLeaderboard();
    }
    [ClientRpc]
    private void AddLeaderboardClientRpc(PlayerData pd){
        Debug.Log("Player: " + pd.nickname + " Points: " + pd.points);
        GameUI.Instance.AddLeaderBoard(pd);
    }

    public void LoadNextGame(){
        NetworkManager.Singleton.SceneManager.LoadScene(GameState.Instance.getRandomMinigame(), LoadSceneMode.Single);
    }
    
    public void ResetDicts(){
        PlayerReadyDict.Clear();
    }

    public bool hasWinner(){
        foreach(PlayerData p in playerDatas){
            if(p.points >= MAXPOINTS){
                return true;
            }
        }
        return false;
    }

    public void LoadWinningScreen(){
        NetworkManager.Singleton.SceneManager.LoadScene("FinalScreen", LoadSceneMode.Single);
    }

    public void GetFinalLeaderBoard(){
        List<PlayerData> l = new List<PlayerData>();
        foreach (PlayerData pd in playerDatas)
        {
            l.Add(pd);
        }
        l.Sort();
        foreach (PlayerData pd in l)
        {
            GameUI.Instance.AddLeaderBoard(pd);
           
        }
        
    }
    

    

    



}

