using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class SplitDecision : NetworkBehaviour,MiniGameCore
{
    [SerializeField] private Transform PlayerPrefab;
    [SerializeField] Transform[] LevelTemplates;
    
    private float currentTranspose;
    [SerializeField] float transpose;

    private bool inited = false;
    private int rankPos;
    private Dictionary<ulong,int> PlayerEndDict;

    [SerializeField] private float camSpeed;
    [SerializeField] private float deathSpeed;
    private float nextX;
    [SerializeField] GameObject Camera;


    void Start()
    {
        PlayerEndDict = new Dictionary<ulong, int>();
        rankPos = NetworkManager.Singleton.ConnectedClientsIds.Count - 1;
        
        
        foreach (Transform t in LevelTemplates)
        {
            t.gameObject.SetActive(false);
        }

        if(IsServer){
            currentTranspose = transpose;
            nextX = transpose;
            InitCurrentTemplates();
        }
        
    }

    private void InitCurrentTemplates(){
        SpawnRandom();
        SpawnRandom();
    }
    private void SpawnRandom(){
        int n = Random.Range(0,2);
        SpawnTemplateClientRpc(pickRandomTemplate(), currentTranspose, n);
        currentTranspose += transpose;
    }
    private int pickRandomTemplate(){
        return Random.Range(0,LevelTemplates.Length);
    }
    [ClientRpc]
    public void SpawnTemplateClientRpc(int index, float transposeX, int Flip){
        Transform tr = Instantiate(LevelTemplates[index]);
        tr.gameObject.SetActive(true);
        tr.position += new Vector3(transposeX,0,0);
        if(Flip == 1){
            tr.localScale = new Vector3(1,-1,1);
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if(!inited || !IsServer){return;}
        
        //Camera.transform.position += new Vector3(camSpeed * Time.deltaTime, 0f,0f);
        transform.position += new Vector3(deathSpeed * Time.deltaTime, 0f,0f);
        if(transform.position.x >= nextX){
            nextX += transpose;
            SpawnRandom();
        }
        
    }

    

    
    public void AddPoints()
    {
        foreach(ulong id in PlayerEndDict.Keys){
            PlayerData playerData =  GameState.Instance.getPlayerData(id);
            int points = GameState.Instance.PointsPerPosition[PlayerEndDict[id]];
            playerData.points += points;
            playerData.added_points = points;
            GameState.Instance.playerDatas[GameState.Instance.getPlayerDataID(id)] = playerData;
        }
    }

    public void InitGame()
    {
        
        PlayPetrolFlowClientRpc();
    }
    [ClientRpc]
    private void PlayPetrolFlowClientRpc(){
        inited=true;
        MiniGame.Instance.whoToFollow = PlayerNetwork.LocalInstance.transform;
        MiniGame.Instance.CameraFollows = true;
        gameObject.GetComponent<Animator>().Play("PetrolFlow");
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
