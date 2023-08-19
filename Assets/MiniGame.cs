using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public interface MiniGameCore{

    public void callAddPoint(int value,ulong clientID){return;} //Client function to call Server Rpc
    public void InitGame(); //Called after Animator Countdown only by Server
    public void AddPoints(); //Called on FinishGameServerRpc
    public void SpawnPlayers();
    public void Interact(ulong clientID){return;}
    public void SetFinish(){return;}
}

public class MiniGame : NetworkBehaviour {
    public static MiniGame Instance;
    [SerializeField] public MiniGameCore MiniGameExtension;

    [Header("General Settings")]
    [SerializeField] private bool CameraFollows;
    [SerializeField] private Transform whoToFollow;
    [SerializeField] private bool FollowX;
    [SerializeField] private bool FollowY;
    [SerializeField] private float CameraSpeed = 50f;
    [SerializeField] public bool IsByScore;

    [Header("MiniGame Settings")]
    [SerializeField] private Sprite TutorialImage;
    [SerializeField] private string GameDescription;

    private bool SetupDone = false;
    private GameObject LocalPlayer;

    
    
    private Dictionary<ulong,TextMeshProUGUI> PlayerPoints = new Dictionary<ulong, TextMeshProUGUI>();
    public NetworkList<ulong> IDS;


    private void Awake() {
        Instance = this;
        // if(IsByScore) PointBoardTemplate.gameObject.SetActive(false);
        IDS = new NetworkList<ulong>();

        MiniGameExtension = GetComponent<MiniGameCore>();
        if(MiniGameExtension == null){throw new UnassignedReferenceException("Error: No MiniGameCore Implemented");}
        
    }
    private void Start() {
        GameUI.Instance.SetMinigameDetails(TutorialImage, GameDescription);
        if(IsServer){
            foreach (ulong id in NetworkManager.Singleton.ConnectedClientsIds)
            {
                IDS.Add(id);
            }
        }


        
        
    }

    public void SetupBoards(){
        // if(!IsByScore){return;}
        // foreach (ulong id in IDS)
        // {   
        //     Transform tempTr = Instantiate(PointBoardTemplate,PointBoardContainer);
        //     PlayerData playerData = GameState.Instance.getPlayerData(id);
        //     tempTr.gameObject.SetActive(true);
        //     tempTr.GetComponent<Image>().color =  GameState.Instance.getColor(playerData.colorID);
        //     TextMeshProUGUI pointText = tempTr.gameObject.GetComponentInChildren<TextMeshProUGUI>();
        //     pointText.text = 0.ToString();
        //     PlayerPoints[id] = pointText;
            
        // }
    }
    
    public void Setup() {
        LocalPlayer = PlayerNetwork.LocalInstance.gameObject;
        if(whoToFollow == null){
            whoToFollow = LocalPlayer.transform;
        }
        SetupDone = true;
    }

    private void FixedUpdate() {
        if(CameraFollows && SetupDone){
            Vector3 cur = Camera.main.transform.position;
            Vector2 vec = Vector2.MoveTowards(Camera.main.transform.position, whoToFollow.position, CameraSpeed * Time.deltaTime);
            float fx = FollowX ? vec.x : cur.x;
            float fy = FollowY ? vec.y : cur.y;
            Camera.main.transform.position = new Vector3(fx,fy,-10f);
        }
    } 
    // public void OnPlayerPointsChanged(ulong id, string amount){
    //     PlayerPoints[id].text = amount;
    // }   
    public void Init(){
        if(IsServer){
            MiniGameExtension.InitGame();
        }
    }
    

    [ClientRpc]
    public void ResetCamClientRpc(){
        Transform t = new GameObject().transform;
        t.position = new Vector3(0,0,-10);
        whoToFollow = t;
    }
    

   
}

