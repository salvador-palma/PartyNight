using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Linq;
using UnityEngine.UI;
using TMPro;
public class PopGun : NetworkBehaviour, MiniGameCore
{
    [SerializeField] private TimerToEnd EndTimer;
    private const int SpawnAmount = 4;
    private const string SpawnItem = "Target";
    private Vector2 SpawnRange = new Vector2(8,4);

    private Dictionary<ulong,int> PlayerPointsDict;

    [SerializeField] private Transform PlayerPrefab;

    [SerializeField] private Transform PointBoardContainer;
    [SerializeField] private Transform PointBoardTemplate;
    private Dictionary<ulong,TextMeshProUGUI> PlayerPoints = new Dictionary<ulong, TextMeshProUGUI>();
    

    


    private void Start() {
        
        PointBoardTemplate.gameObject.SetActive(false);
        PlayerPointsDict = new Dictionary<ulong, int>();

        
        
    }
    public void InitGame()
    {

        SetupBoardsClientRpc();
        initTimerClientRpc(20);
        
        for (int i = 0; i < SpawnAmount; i++)
        {
                GameObject TargetGO = Instantiate((GameObject)Resources.Load(SpawnItem));
                float x = SpawnRange.x;
                float y = SpawnRange.y;
                Vector2 randomPos = new Vector2(Random.Range(-x,x),Random.Range(-y,y));
                TargetGO.transform.position = randomPos;
                TargetGO.GetComponent<NetworkObject>().Spawn(true);
        }
    }
    [ClientRpc]
    public void initTimerClientRpc(int time){
        EndTimer.Init(time);
    }



    public void AddPoints()
    {
        Dictionary<ulong, int> transformedDictionary = PlayerPointsDict.OrderByDescending(pair => pair.Value).Select((pair, index) => new { pair.Key, Index = index }).ToDictionary(item => item.Key, item => item.Index);
        foreach(ulong id in transformedDictionary.Keys){
            PlayerData playerData =  GameState.Instance.getPlayerData(id);
            int addedPoints = GameState.Instance.PointsPerPosition[transformedDictionary[id]];
            playerData.points += addedPoints;
            playerData.added_points = addedPoints;
            GameState.Instance.playerDatas[GameState.Instance.getPlayerDataID(id)] = playerData;
        }
    }

    public void callAddPoint(int value, ulong clientID){
        AddPointServerRpc(value, clientID);
    }
    [ServerRpc(RequireOwnership=false)]
    public void AddPointServerRpc(int amount, ulong id){
        if(PlayerPointsDict.ContainsKey(id)){
            PlayerPointsDict[id] += amount;
        }else{
            PlayerPointsDict[id] = amount;
        }
        AddPointClientRpc(id, PlayerPointsDict[id]);
        
    }
    [ClientRpc]
    private void AddPointClientRpc(ulong id, int totalPoints){
        PlayerPoints[id].text = totalPoints.ToString();
        //MiniGame.Instance.OnPlayerPointsChanged(id, totalPoints.ToString());
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
        int i = 0;
        Vector2[] positions = SpawnPoints();
        foreach(ulong clientID in NetworkManager.Singleton.ConnectedClientsIds){
            Transform PlayerTr = Instantiate(PlayerPrefab);
            PlayerTr.transform.position = positions[i];
            i++;
            PlayerTr.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientID, true);
        }
    }

    [ClientRpc]
    public void SetupBoardsClientRpc(){
        foreach (ulong id in MiniGame.Instance.IDS)
        {   
            Transform tempTr = Instantiate(PointBoardTemplate,PointBoardContainer);
            PlayerData playerData = GameState.Instance.getPlayerData(id);
            tempTr.gameObject.SetActive(true);
            tempTr.GetComponent<Image>().color =  GameState.Instance.getColor(playerData.colorID);
            TextMeshProUGUI pointText = tempTr.gameObject.GetComponentInChildren<TextMeshProUGUI>();
            pointText.text = 0.ToString();
            PlayerPoints[id] = pointText;
            
        }
    }
}
