using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class MiniGame : NetworkBehaviour {
    public static MiniGame Instance;
    [SerializeField] private bool CameraFollows;
    [SerializeField] public bool IsByScore;

    private bool SetupDone = false;
    private GameObject LocalPlayer;
    public Vector2[] SpawnPoints;
    private void Awake() {
        Instance = this;
    }
    private void Start() {
        GameObject[] points = GameObject.FindGameObjectsWithTag("Spawnpoints");
        SpawnPoints = new Vector2[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            SpawnPoints[i] = points[i].transform.position;
        }
    }
    public void Setup() {
        LocalPlayer = PlayerNetwork.LocalInstance.gameObject;
        SetupDone = true;
    }

    private void LateUpdate() {
        if(CameraFollows && SetupDone){
            Vector3 camPos = Camera.main.transform.position;
            camPos.x = LocalPlayer.transform.position.x;
            Camera.main.transform.position = camPos;
        }
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Player"){
            other.GetComponent<PlayerNetwork>().SetFinish();
        }
    }
    public void DebugPoints(){
        GameState.Instance.DebugPointsServerRpc();
    }

   
}

