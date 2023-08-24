using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class TugOfWar : NetworkBehaviour, MiniGameCore
{

    //TEAM GAME


    [SerializeField] private Transform PlayerPrefab;
    
    [SerializeField] private Dictionary<ulong, int> teamDict;
    [SerializeField] private Zoom camZoom;

    private Vector2[,] spawnpos = {{new Vector2(-35,-3),new Vector2(-28,-3),new Vector2(-21,-3),new Vector2(-14,-3)},
                                    {new Vector2(35,3),new Vector2(28,3),new Vector2(21,3),new Vector2(14,3)}};
    
    
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

    

    public void SpawnPlayers()
    {
        Marks marks = Rope.GetComponent<Marks>();
        Transform[] spawnpoints = marks.getSpawnpoints();
        int i = 0;
        
        foreach(ulong clientID in NetworkManager.Singleton.ConnectedClientsIds){
            Transform PlayerTr = Instantiate(PlayerPrefab);
            teamDict[clientID] = i%2;
            
           
            NetworkObject nObj = PlayerTr.GetComponent<NetworkObject>();

            nObj.SpawnAsPlayerObject(clientID, true);
            nObj.TrySetParent(spawnpoints[4*(i%2) + i/2]);
            PlayerTr.localPosition = Vector3.zero;
            PlayerTr.localScale = new Vector3(0.9f,0.9f,0.9f);
            PlayerTr.localRotation = Quaternion.Euler(0f,0f,0f);
            i++;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.name == "Mark1"){
            WinningTeam = 0;

            //GameState.Instance.FinishGameServerRpc();

        }else if(other.name == "Mark2"){
            WinningTeam = 1;
            //GameState.Instance.FinishGameServerRpc();
        }
    }

    public void AddPoints()
    {
        foreach(ulong id in teamDict.Keys){
            PlayerData playerData =  GameState.Instance.getPlayerData(id);
            if(teamDict[id] == WinningTeam){
                playerData.points += 10;
                playerData.added_points = 10;
            }else{
                playerData.points += 2;
                playerData.added_points = 2;
            }
            GameState.Instance.playerDatas[GameState.Instance.getPlayerDataID(id)] = playerData;

        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void MoveRopeServerRpc(ulong clientID){
        if(!Started){return;}
        
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
