using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorBtnUI : MonoBehaviour
{
    [SerializeField] int colorID;
    [SerializeField] Image image; 
    [SerializeField] GameObject selectedGo;


    private void Awake() {
        GetComponent<Button>().onClick.AddListener(()=>{
            GameState.Instance.ChangePlayerColor(colorID);
        });
    }
    private void Start() {
        GameState.Instance.OnPlayerDataNetworkListChanged += OnPlayerListChanged;
        image.color = GameState.Instance.getColor(colorID);
        selectedUpdate();
    }

    private void OnPlayerListChanged(object sender, EventArgs e)
    {
        selectedUpdate();
    }

    private void selectedUpdate(){
        if(GameState.Instance.getLocalPlayerData().colorID == colorID){
            selectedGo.SetActive(true);
        }else{
            selectedGo.SetActive(false);
        }
    }

    private void OnDestroy() {
        GameState.Instance.OnPlayerDataNetworkListChanged -= OnPlayerListChanged;
    }
}
