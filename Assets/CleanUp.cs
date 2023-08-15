using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CleanUp : MonoBehaviour
{
    public void Awake(){
        
        if(SceneManager.GetActiveScene().name == "StartMenu"){
            DeletePermanentObjects();
        }
    }

    private void DeletePermanentObjects()
    {
        GameObject NM = GameObject.Find("NetworkManager");
        GameObject GM = GameObject.Find("GameManager");
        GameObject LM = GameObject.Find("LobbyManager");
        

        if(NM != null) Destroy(NM);
        if(GM != null) Destroy(GM);
        if(LM != null) Destroy(LM);
        
        
    }
}
