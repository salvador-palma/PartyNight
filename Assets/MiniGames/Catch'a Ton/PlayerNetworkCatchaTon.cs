using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNetworkCatchaTon : PlayerNetwork
{
    const int MAX_BALLS = 3;
    [SerializeField] public int ball_amount=0;
    private CatchaTon Game;
    public override void Start() {
        base.Start();
        Game = (CatchaTon)MiniGame.Instance.MiniGameExtension;
    }
    
    public void Flush(int team){
        Game.AddPoint(team, ball_amount);
        ball_amount=0;
    }

    public bool Pick(){
        if(ball_amount >= MAX_BALLS){
            return false;
        }else{
            ball_amount++;
            return true;
        }
        
    }
}
