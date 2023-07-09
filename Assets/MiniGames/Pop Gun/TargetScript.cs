using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
public class TargetScript : NetworkBehaviour 
{
    public static int MAX_ID=0;
    private const int X_RANGE = 8;
    private const int Y_RANGE = 4;
    [SerializeField] private GameObject TargetPrefab;
    [SerializeField] private GameObject TargetEffect;
    private static Dictionary<int,bool> TargetsHit = new Dictionary<int,bool>();
    public int value;
    public int ID;

    public override void OnNetworkSpawn(){

        TargetPrefab = (GameObject)Resources.Load("Target");
        gameObject.GetComponent<SpriteRenderer>().enabled =true;
    }

    private void Awake() {
        ID=MAX_ID;
        MAX_ID++;
    }

    [ServerRpc(RequireOwnership=false)]
    public void SpawnTargetServerRpc(float x, float y){
        GameObject TargetGO = Instantiate(TargetPrefab);
        Vector2 randomPos = new Vector2(x,y);
        TargetGO.transform.position = randomPos;
        TargetGO.GetComponent<NetworkObject>().Spawn(true);
    }

    [ServerRpc(RequireOwnership=false)]
    public void DespawnTargetServerRpc(ulong clientID){
        if(!TargetsHit.ContainsKey(ID)){
            MiniGame.Instance.MiniGameExtension.callAddPoint(value, clientID);
        }
        GetComponent<NetworkObject>().Despawn();
    }
    
    

    private void OnMouseDown() {
        float x = Random.Range(-X_RANGE,X_RANGE);
        float y = Random.Range(-Y_RANGE,Y_RANGE);
        GameObject.Find("EventSystem").GetComponent<AudioManager>().Play(true);
        SpawnTargetServerRpc(x,y);
        DespawnTargetServerRpc(NetworkManager.Singleton.LocalClientId);
        Instantiate(TargetEffect).transform.position = transform.position;
    }

  
    
}
