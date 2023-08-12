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
        Menu.onClick.AddListener(()=>{
            SceneManager.LoadScene("StartMenu");
        });
        
        GameState.Instance.GetFinalLeaderBoard();
        
    }
    
    

   
}
