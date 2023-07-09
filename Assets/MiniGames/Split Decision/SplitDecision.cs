using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class SplitDecision : MonoBehaviour,MiniGameCore
{
    [SerializeField] private Transform PlayerPrefab;
    [SerializeField] GameObject[] LevelTemplates;
    [SerializeField] GameObject TemplateZero;
    [SerializeField] Transform Grid;

    GameObject[] currentTemplates;
    [SerializeField] Camera cam;
    
    [SerializeField] float camSpeed;
    private float deltaY;

    [SerializeField] float transpose;

    [SerializeField] Transform deadWall;

    private bool inited = false;

    private int rankPos;
    private Dictionary<ulong,int> PlayerEndDict;

    void Start()
    {
        PlayerEndDict = new Dictionary<ulong, int>();
        rankPos = NetworkManager.Singleton.ConnectedClientsIds.Count - 1;
        InitCurrentTemplates();
        Grid.transform.position = new Vector3(0,0.5f,0);
    }

    private void InitCurrentTemplates(){
        currentTemplates = new GameObject[3];
        currentTemplates[0] = Instantiate(TemplateZero);
        currentTemplates[0].transform.parent = Grid;
        for(int i = 1; i<3; i++){
            int n = Random.Range(0, LevelTemplates.Length);
            Vector3 vec = new Vector3(transpose/3, 0f, 0f);
            currentTemplates[i] = Instantiate(LevelTemplates[n]);
            currentTemplates[i].transform.parent = Grid;
            currentTemplates[i].transform.position = vec * i;
            
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!inited){return;}
       
        Vector3 vec = deadWall.position;
        float f = camSpeed * Time.deltaTime;
        vec.x += f;
        deadWall.position = vec;
        deltaY += f;
        if(deltaY >= transpose/3){
            deltaY = 0;
            NextTemplate();
        }
        
    }

    

    private void NextTemplate(){
        int n = Random.Range(0, LevelTemplates.Length);
        
        Vector3 vec = currentTemplates[0].transform.position;
        Destroy(currentTemplates[0].gameObject);
        currentTemplates[0] = currentTemplates[1];
        currentTemplates[1] = currentTemplates[2];
        currentTemplates[2] = Instantiate(LevelTemplates[n]);

        currentTemplates[2].transform.parent= Grid;
        vec.x += transpose;
        currentTemplates[2].transform.position = vec;


    }
    public void AddPoints()
    {
        foreach(ulong id in PlayerEndDict.Keys){
            PlayerData playerData =  GameState.Instance.getPlayerData(id);
            playerData.points += GameState.Instance.PointsPerPosition[PlayerEndDict[id]];
            GameState.Instance.playerDatas[GameState.Instance.getPlayerDataID(id)] = playerData;
        }
    }

    public void InitGame()
    {
        inited=true;
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
        rankPos--;
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

   
}
