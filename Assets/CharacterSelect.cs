using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using System;


public class CharacterSelect : NetworkBehaviour
{
    public static CharacterSelect Instance;
    private static Dictionary<ulong, bool> PlayerReadyDict = new Dictionary<ulong, bool>();
    [SerializeField] Animator ElementsAnimator;

    public event EventHandler onReadyChange;
    private void Awake() {
        
        Instance = this;
       
    }
    public void setPlayerReady(){
        Debug.Log("Ready!");
        setPlayerReadyServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void setPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default){
        Debug.Log("DEBUG READY 0");
        syncPlayersReadyClientRpc(serverRpcParams.Receive.SenderClientId);
        PlayerReadyDict[serverRpcParams.Receive.SenderClientId] = true;

        bool AllReady = true;
        Debug.Log("DEBUG READY 1");
        foreach (ulong clientID in NetworkManager.Singleton.ConnectedClientsIds){
            Debug.Log("DEBUG READY 2 :" + clientID);
            if(!PlayerReadyDict.ContainsKey(clientID) || !PlayerReadyDict[clientID]){
                AllReady = false;
                break;
            }
        }
        if(AllReady){
            Debug.Log("All Ready!");
            // LobbyManager.Instance.DeleteLobby();
            // DisableScrollerClientRpc();
            // NetworkManager.Singleton.SceneManager.LoadScene(GameState.Instance.getRandomMinigame(), LoadSceneMode.Single);
            ClearReadyDictClientRpc();
            CharSelectFadeOutClientRpc();
        }
    }
    [ClientRpc]
    public void ClearReadyDictClientRpc(){
        PlayerReadyDict.Clear();
    }
    
    [ClientRpc]
    public void CharSelectFadeOutClientRpc(){
        ElementsAnimator.Play("Outro");
    }

    
    [ClientRpc]
    private void syncPlayersReadyClientRpc(ulong clientID){
        PlayerReadyDict[clientID] = true;
        onReadyChange?.Invoke(this, EventArgs.Empty);
    }

    public bool isPlayerReady(ulong clientId){
        return PlayerReadyDict.ContainsKey(clientId) && PlayerReadyDict[clientId];
    }

    public void CharSelectFadeOutEvent(){
        if(IsServer){
            CharSelectFadeOutEventServerRpc();
        }
    }
    
    [ServerRpc]
    public void CharSelectFadeOutEventServerRpc(){
        
        //LobbyManager.Instance.DeleteLobby();
        NetworkManager.Singleton.SceneManager.LoadScene(GameState.Instance.getRandomMinigame(), LoadSceneMode.Single);
    }
}
