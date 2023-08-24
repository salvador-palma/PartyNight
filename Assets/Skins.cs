using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skins
{
    public static Skins Instance;
    int EyeAmount = 5;
    int HairAmount = 5;
    List<Sprite> EyeList;
    List<Sprite> HairList;
    private Skins(){
        Instance = this;
        EyeList = new List<Sprite>();
        HairList = new List<Sprite>();
        EyeList.Add(null);
        HairList.Add(null);
        for (int i = 1; i <= EyeAmount; i++)
        {
            EyeList.Add(Resources.Load<Sprite>("Skins/Eyes"+i));
        }
        for (int i = 1; i <= HairAmount; i++)
        {
            HairList.Add(Resources.Load<Sprite>("Skins/Hair"+i));
        }
    }

    public static Skins GetInstance(){
        if(Instance!=null){return Instance;}
        return new Skins();
    }

    public Sprite getEye(int index){
        if(index>=EyeAmount+1){throw new ArgumentException("This eye index wasn't implemented yet");}
        return EyeList[index];
    }
    public Sprite getHair(int index){
        if(index>=HairAmount+1){throw new ArgumentException("This hair index wasn't implemented yet");}
        return HairList[index];
    }
    public List<Sprite> getHairList(){
        return HairList;
    }
    public List<Sprite> getEyeList(){
        return EyeList;
    }



    
    
}
