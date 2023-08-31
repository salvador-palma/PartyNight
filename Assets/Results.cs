using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Results : NetworkBehaviour
{
    
    [SerializeField] private Button Menu;

   


    // Start is called before the first frame update
    public void Start()
    {
        if(!IsServer){
            Destroy(Menu.gameObject);
        }else{
            Menu.onClick.AddListener(()=>{
                //CleanUp.CleanAll();
                //SceneManager.LoadScene("Lobby");
                //SceneManager.LoadScene("CharacterSelect");
                GameState.Instance.ResetPoints();
                playOutroClientRpc();
            });
        }
        
        
        GameState.Instance.GetFinalLeaderBoard();
        
    }

    [ClientRpc]
    private void playOutroClientRpc(){
        GameUI.Instance.gameObject.GetComponent<Animator>().Play("Outro");
    }

    public void endGame(){
        if(IsServer){
            NetworkManager.Singleton.SceneManager.LoadScene("CharacterSelect", LoadSceneMode.Single);
        }
        
    }
    
    

   
}
