
using UnityEngine;


public class CleanUp : MonoBehaviour
{
    

    public static void CleanAll(){
        Debug.Log("Destroying 0");
        Loop(GameObject.FindGameObjectsWithTag("NetMan"));
        Debug.Log("Destroying 1");
        Loop(GameObject.FindGameObjectsWithTag("GameMan"));
        Debug.Log("Destroying 2");
        Loop(GameObject.FindGameObjectsWithTag("LobbyMan"));
        Debug.Log("Destroying 3");
        
    }

    private static void Loop(GameObject[] gos){

        for (int i = 0; i < gos.Length; i++)
        {
            Debug.Log("Destroyed: " + gos[i].name);
            Destroy(gos[i]);
                
        }
        
    }

    
}
