using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerNetworkCatchaTon : PlayerNetwork
{
    

    [SerializeField] private float[] speeds;
   
    [SerializeField]private int[] pellets;

    
    [SerializeField] public SpriteRenderer[] catches;
    private CatchaTon Game;
    public override void Start() {
        base.Start();
        Game = (CatchaTon)MiniGame.Instance.MiniGameExtension;
        pellets =new int[3]{-1,-1,-1};
        updateAmountClientRpc(pellets);
    }
    
    public void Flush(int team){
        Game.AddPointServerRpc(team, getBasketValue());
        pellets = new int[3]{-1,-1,-1};

        for (int i = 0; i < catches.Length; i++)
        {
            catches[i].enabled = false;
        }
       
        callUpdateServerRpc(pellets);
    }

    public int getBasketValue(){
        int temp = 0;
        for (int i = 0; i < pellets.Length; i++)
        {
            if(pellets[i]==-1){continue;}
            temp += Game.pelletValues[pellets[i]];
        }
        return temp;
    }

    public void Pick(int type){
        

        pellets = AddPellet(pellets, type);
        for (int i = 0; i < catches.Length; i++)
        {
            if(pellets[i]==-1){catches[i].enabled=false;}
            else{
                catches[i].sprite = Game.sprites[pellets[i]];
                catches[i].enabled = true;
            }
        }
        
        callUpdateServerRpc(pellets);
        
    }

    public bool isFull(){
        return pellets[pellets.Length - 1] != -1;
    }

    public int[] AddPellet(int[] s, int val){
        int[] temp = new int[s.Length];
        bool check = false;
        for (int i = 0; i < s.Length; i++)
        {
            if(s[i]==-1 && !check){
                temp[i] = val;
                check = true;
            }else{
                temp[i] = s[i];
            }
        }
        return temp;
    }

    [ServerRpc(RequireOwnership = false)]
    public void callUpdateServerRpc(int[] inc){
        updateAmountClientRpc(inc);
        
    }

    [ClientRpc]
    public void updateAmountClientRpc(int[] inc){
        pellets = inc;
        int num = 0;
        for (int i = 0; i < catches.Length; i++)
        {
            if(pellets[i]==-1){catches[i].enabled=false;}
            else{
                catches[i].sprite = Game.sprites[pellets[i]];
                catches[i].enabled = true;
                num++;
            }
        }
        base.current_speed = speeds[num];
    }

    


}
