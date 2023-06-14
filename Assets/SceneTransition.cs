using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private Animator anim;
    public string FadeOutScene;

    
    public void NextScene(){
        anim.SetBool("Next",true);
    }

    public void Load(string scene){
        SceneManager.LoadScene(scene);
    }

}
