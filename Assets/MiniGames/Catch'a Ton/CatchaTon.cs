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
    [SerializeField] private Transform PelletPrefab;
    [SerializeField] private Dictionary<ulong, int> teamDict;

    [SerializeField] private Dictionary<int,int> teamPoints; //team -> points


    [SerializeField] private Transform[] PointsBoards;
    [SerializeField] private int[] points = new int[4]{15,10,5,2};
    [SerializeField] private Color[] teamColors;
    private Dictionary<int,TextMeshProUGUI> PlayerPoints = new Dictionary<int, TextMeshProUGUI>();

    [SerializeField] private BoxCollider2D[] SpawnArea;
    private Bounds[] colliderBounds;
    private Vector3[] colliderCenter;

    private float TimerToPellet;
    private float TimerToPelletFixed=1f;
    public bool GameOn = false;
    [SerializeField] public int[] pelletValues;
    [SerializeField]public Sprite[] sprites;
    private void Start() {
        if(IsServer){
            TimerToPellet = TimerToPelletFixed;
            colliderBounds = new Bounds[2];
            colliderCenter = new Vector3[2];
            colliderBounds[0] = SpawnArea[0].bounds;
            colliderCenter[0] = colliderBounds[0].center;
            colliderBounds[1] = SpawnArea[1].bounds;
            colliderCenter[1] = colliderBounds[1].center;
        }
        
        teamPoints = new Dictionary<int, int>();
        for(int i = 0; i!=4; i++){
            teamPoints[i] = 0;
        }
        teamDict = new Dictionary<ulong, int>();
    }

    private void Update() {
        if(!IsServer || !GameOn){return;}
        TimerToPellet-= Time.deltaTime;
        if(TimerToPellet <= 0){
            TimerToPellet = TimerToPelletFixed;
            SpawnPellet();
        }
    }
    public void AddPoints()
    {
        //debugTeams();
        List<int> l = GetSortedTeamIDs(teamPoints);
        
        foreach(ulong id in teamDict.Keys){
            PlayerData playerData =  GameState.Instance.getPlayerData(id);
            int team = teamDict[id];
            playerData.points += points[l.IndexOf(team)];
            GameState.Instance.playerDatas[GameState.Instance.getPlayerDataID(id)] = playerData;
        }
    }


    // public void debugTeams(){
    //     List<int> l = GetSortedTeamIDs(teamPoints);
    //     for (int i = 0; i < l.Count; i++)
    //     {
    //         Debug.Log("TeamID: " + l[i] + " with " + teamPoints[l[i]] + " points");
            
    //         foreach(ulong id in teamDict.Keys)
    //         {
    //             if(teamDict[id] == l[i]){
    //                 PlayerData playerData = GameState.Instance.getPlayerData(id);
    //                 Debug.Log(playerData.nickname);
    //             }
    //         }

    //     }
    // }

    public List<int> GetSortedTeamIDs(Dictionary<int, int> teamPointsDictionary)
    {
        List<int> sortedTeamIDs = teamPointsDictionary.OrderByDescending(kv => kv.Value).Select(kv => kv.Key).ToList();
        return sortedTeamIDs;
    }

    public void InitGame()
    {
        initTimerClientRpc(60);
        if(IsServer){
            for (int i = 0; i < 10; i++)
            {
                SpawnPellet();
            }
        }
        GameOn = true;
        
        
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
        SetupBoardsClientRpc();
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
    [ServerRpc(RequireOwnership = false)]
    public void AddPointServerRpc(int team, int amount){
        if(!teamPoints.ContainsKey(team)){
            teamPoints[team] = amount;
        }else{
            teamPoints[team] += amount;
        }
        AddPointClientRpc(team, teamPoints[team]);
        //debugPoints();
    
    }

    [ClientRpc]
    public void SetupBoardsClientRpc(){
        foreach(int teamID in teamPoints.Keys)
        {   
            
            TextMeshProUGUI pointText = PointsBoards[teamID].gameObject.GetComponentInChildren<TextMeshProUGUI>();
            pointText.text = 0.ToString();
            PlayerPoints[teamID] = pointText;
            
        }
    }
    
    [ClientRpc]
    private void AddPointClientRpc(int id, int totalPoints){
        PlayerPoints[id].text = totalPoints.ToString();
        //MiniGame.Instance.OnPlayerPointsChanged(id, totalPoints.ToString());
    }

    

    public void SpawnPellet(){  
        int r = Random.Range(0,2);
        float randomX = Random.Range(colliderCenter[r].x - colliderBounds[r].extents.x, colliderCenter[r].x + colliderBounds[r].extents.x);
        float randomY = Random.Range(colliderCenter[r].y - colliderBounds[r].extents.y, colliderCenter[r].y + colliderBounds[r].extents.y);
        Vector2 randomPos = new Vector2(randomX, randomY);
        Transform t = Instantiate(PelletPrefab, randomPos, Quaternion.identity);
        t.GetComponent<NetworkObject>().Spawn(true);
    }
 

}
