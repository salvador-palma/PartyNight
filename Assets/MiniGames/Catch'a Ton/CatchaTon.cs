using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Linq;
using TMPro;
using UnityEngine.UI;
public class CatchaTon : NetworkBehaviour, MiniGameCore
{
    [SerializeField] private TimerToEnd EndTimer;
    [SerializeField] private Transform PlayerPrefab;
    [SerializeField] private Dictionary<ulong, int> teamDict;

    [SerializeField] private Dictionary<int,int> teamPoints; //team -> points
    [SerializeField] private int[] points = new int[4]{15,10,5,2};
    private void Start() {
        teamPoints = new Dictionary<int, int>();
        for(int i = 0; i!=4; i++){
            teamPoints[i] = 0;
        }
        teamDict = new Dictionary<ulong, int>();
    }
    public void AddPoints()
    {
        Dictionary<int, int> transformedDictionary = teamPoints.OrderByDescending(pair => pair.Value).Select((pair, index) => new { pair.Key, Index = index }).ToDictionary(item => item.Key, item => item.Index);
        foreach(ulong id in teamDict.Keys){
            PlayerData playerData =  GameState.Instance.getPlayerData(id);
            playerData.points += points[transformedDictionary[teamDict[id]]];
            GameState.Instance.playerDatas[GameState.Instance.getPlayerDataID(id)] = playerData;
        }
    }

    public void InitGame()
    {
        initTimerClientRpc(30);
    }
    [ClientRpc]
    public void initTimerClientRpc(int time){
        EndTimer.Init(time);
    }

    public void SpawnPlayers()
    {
       
        int i = 0;
        Vector2[,] positions = SpawnPoints();
        foreach(ulong clientID in NetworkManager.Singleton.ConnectedClientsIds){
            Transform PlayerTr = Instantiate(PlayerPrefab);
            PlayerTr.transform.position = positions[i%4,i/4];
            teamDict[clientID] = i%4;
            i++;
            PlayerTr.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientID, true);
        }
    }

    public Vector2[,] SpawnPoints()
    {
        Vector2[,] vec = new Vector2[4,2];
        GameObject[] points = GameObject.FindGameObjectsWithTag("Spawnpoints");
        for (int i = 0; i < points.Length; i++)
        {
            vec[0,i] = points[i].transform.position;
        }
        GameObject[] points2 = GameObject.FindGameObjectsWithTag("Spawnpoints2");
        for (int i = 0; i < points2.Length; i++)
        {
            vec[1,i] = points2[i].transform.position;
        }
        GameObject[] points3 = GameObject.FindGameObjectsWithTag("Spawnpoints3");
        for (int i = 0; i < points3.Length; i++)
        {
            vec[2,i] = points3[i].transform.position;
        }
        GameObject[] points4 = GameObject.FindGameObjectsWithTag("Spawnpoints4");
        for (int i = 0; i < points4.Length; i++)
        {
            vec[3,i] = points4[i].transform.position;
        }
        return vec;
    }

    public void AddPoint(int team, int amount){
        if(teamPoints.ContainsKey(team)){
            teamPoints[team] = amount;
        }else{
            teamPoints[team] += amount;
        }
    }


 

}
