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
    public int value;
    public int ID;

    public override void OnNetworkSpawn(){
        TargetPrefab = (GameObject)Resources.Load("Target");
    }

    [ServerRpc(RequireOwnership=false)]
    public void SpawnTargetServerRpc(float x, float y){
        
        GameObject TargetGO = Instantiate(TargetPrefab);
        Vector2 randomPos = new Vector2(x,y);
        TargetGO.transform.position = randomPos;
        TargetGO.GetComponent<NetworkObject>().Spawn(true);
    }

    [ServerRpc(RequireOwnership=false)]
    public void DepawnTargetServerRpc(){
        GetComponent<NetworkObject>().Despawn();
    }
    
    

    private void OnMouseDown() {
        float x = Random.Range(-X_RANGE,X_RANGE);
        float y = Random.Range(-Y_RANGE,Y_RANGE);
        SpawnTargetServerRpc(x,y);
        PlayerNetwork.LocalInstance.AddPoint(value);
        DepawnTargetServerRpc();
    }
}
