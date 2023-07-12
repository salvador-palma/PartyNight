using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerNetworkCatchaTon : PlayerNetwork
{
    public const int MAX_BALLS = 3;
    [SerializeField] public int ball_amount=0;
    [SerializeField] public SpriteRenderer[] catches;
    private CatchaTon Game;
    public override void Start() {
        base.Start();
        Game = (CatchaTon)MiniGame.Instance.MiniGameExtension;
        updateAmountClientRpc(0);
    }
    
    public void Flush(int team){
        Game.AddPoint(team, ball_amount);
        updateAmountClientRpc(0);
    }

    public void Pick(){
        ball_amount++;
        int temp = ball_amount;
        bool check = true;
        for (int i = 0; i < catches.Length; i++)
        {
            if(i == ball_amount){check = !check;}
            catches[i].enabled = check;
        }
        callUpdateServerRpc(ball_amount);
    }

    [ServerRpc(RequireOwnership = false)]
    public void callUpdateServerRpc(int inc){
        updateAmountClientRpc(inc);
    }

    [ClientRpc]
    public void updateAmountClientRpc(int inc){
        ball_amount = inc;
        int temp = ball_amount;
        bool check = true;
        for (int i = 0; i < catches.Length; i++)
        {
            if(i == ball_amount){check = !check;}
            catches[i].enabled = check;
        }
    }


}
