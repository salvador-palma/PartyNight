using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;

public class RamJam : NetworkBehaviour,MiniGameCore
{
    private GameObject[] sheeps;
    private Dictionary<ulong,int> PlayerEndDict;
    private int rankPos;
    private bool hasFinishedLocal = false;

    [SerializeField] private Transform PlayerPrefab;
    public void InitGame()
    {
        foreach(GameObject go in sheeps){
            go.GetComponent<SheepMovement>().InitSheep();
        }
    }
    void Start()
    {
        PlayerEndDict = new Dictionary<ulong, int>();
        rankPos = 0;
        sheeps = GameObject.FindGameObjectsWithTag("obj1");
    }    
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Player" ){
            other.GetComponent<PlayerNetwork>().SetFinish();
        }
    }
    public void SetFinish(){
        PlayerFinishServerRpc(GameState.Instance.getLocalPlayerData().ID);
    }

    [ServerRpc(RequireOwnership = false)]
    public void PlayerFinishServerRpc(ulong id){
        PlayerEndDict[id] = rankPos;
        rankPos++;
        bool AllReady = true;
        foreach (ulong clientID in NetworkManager.Singleton.ConnectedClientsIds){
            if(!PlayerEndDict.ContainsKey(clientID) || PlayerEndDict[clientID] == -1){
                AllReady = false;
                break;
            }
        }
        if(AllReady){
            GameState.Instance.FinishGameServerRpc();
        }
    }
    public void AddPoints()
    {
        foreach(ulong id in PlayerEndDict.Keys){
            PlayerData playerData =  GameState.Instance.getPlayerData(id);
            int pointsAdded = GameState.Instance.PointsPerPosition[PlayerEndDict[id]];
            playerData.points += pointsAdded;
            playerData.added_points = pointsAdded;
            GameState.Instance.playerDatas[GameState.Instance.getPlayerDataID(id)] = playerData;
        }
    }

    public Vector2[] SpawnPoints()
    {
        GameObject[] points = GameObject.FindGameObjectsWithTag("Spawnpoints");
        Vector2[] SpawnPoints;
        SpawnPoints = new Vector2[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            SpawnPoints[i] = points[i].transform.position;
        }
        return SpawnPoints;
    }

    public void SpawnPlayers()
    {
        Debug.Log("Spawning");
        int i = 0;
        Vector2[] positions = SpawnPoints();
        foreach(ulong clientID in NetworkManager.Singleton.ConnectedClientsIds){
            Transform PlayerTr = Instantiate(PlayerPrefab);
            ClientNetworkTransform cnt = PlayerTr.GetComponent<ClientNetworkTransform>();
            cnt.SyncScaleX = true;
            cnt.SyncScaleY = true;
            PlayerTr.localScale = new Vector3(0.1094221f,0.1094221f,0.1094221f);
            PlayerTr.transform.position = positions[i];
            i++;
            PlayerTr.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientID, true);
        }
    }
}
