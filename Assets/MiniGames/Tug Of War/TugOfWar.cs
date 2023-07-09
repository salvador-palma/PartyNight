using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class TugOfWar : NetworkBehaviour, MiniGameCore
{

    //TEAM GAME


    [SerializeField] private Transform PlayerPrefab;
    [SerializeField] private Transform PlayerVisual;
    [SerializeField] private Dictionary<ulong, int> teamDict;
    [SerializeField] private Zoom camZoom;

    
    
    
    private bool Started = false;
    [SerializeField] float RopeSpeed;
    private int WinningTeam;
    [SerializeField] private GameObject Rope;

    private void Start() {
        teamDict = new Dictionary<ulong, int>();
    }

    public void InitGame()
    {
        Started = true;
    }

    public Vector2[,] SpawnPoints()
    {
        Vector2[,] vec = new Vector2[2,4];
        GameObject[] points = GameObject.FindGameObjectsWithTag("Spawnpoints");
        for (int i = 0; i < points.Length; i++)
        {
            vec[0,i] = points[i].transform.position;
        }
        GameObject[] points2 = GameObject.FindGameObjectsWithTag("Spawnpoints2");
        for (int i = 0; i < points.Length; i++)
        {
            vec[1,i] = points2[i].transform.position;
        }
        return vec;
    }

    public void SpawnPlayers()
    {
        int i = 0;
        Vector2[,] positions = SpawnPoints();
        foreach(ulong clientID in NetworkManager.Singleton.ConnectedClientsIds){
            Transform PlayerTr = Instantiate(PlayerPrefab);
            PlayerTr.transform.position = positions[i%2,i/2];
            teamDict[clientID] = i%2;
            Debug.Log("Server Side: " + clientID);
            i++;
            PlayerTr.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientID, true);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.name == "Mark1"){
            WinningTeam = 0;
            GameState.Instance.FinishGameServerRpc();

        }else if(other.name == "Mark2"){
            WinningTeam = 1;
            GameState.Instance.FinishGameServerRpc();
        }
    }

    public void AddPoints()
    {
        foreach(ulong id in teamDict.Keys){
            PlayerData playerData =  GameState.Instance.getPlayerData(id);
            if(teamDict[id] == WinningTeam){
                playerData.points += 10;
            }else{
                playerData.points += 2;
            }
            GameState.Instance.playerDatas[GameState.Instance.getPlayerDataID(id)] = playerData;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void MoveRopeServerRpc(ulong clientID){
        if(!Started){return;}
        foreach (ulong id in teamDict.Keys)
        {
            Debug.Log("Server: " + id);
        }
        Debug.Log("Server: " + 123 + " ");
        bool IsPositive = teamDict[clientID] != 0;
        if(IsPositive){
            Vector3 vec = new Vector3(RopeSpeed, 0f, 0f);
            Rope.transform.position += vec;
        }else{
            Vector3 vec = new Vector3(RopeSpeed, 0f, 0f);
            Rope.transform.position -= vec;
        }
    }

    public void Interact(ulong clientID){
        camZoom.ZoomIn();
        MoveRopeServerRpc(clientID);
    }

    public void EndGame(string winningSide){
        ResetCameraClientRpc();
        if(winningSide == "Mark1"){
            WinningTeam = 1;
        }else{
            WinningTeam = 0;
        }
        GameState.Instance.FinishGameServerRpc();
    }

    [ClientRpc]
    public void ResetCameraClientRpc(){
        camZoom.Reset();
    }

}
