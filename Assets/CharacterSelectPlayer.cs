using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;
using Unity.Collections;

public struct PlayerData : IEquatable<PlayerData>, INetworkSerializable, IComparable<PlayerData>{
    public ulong ID;
    public int colorID;
    public int hairID;
    public int eyesID;
    public FixedString64Bytes nickname;
    public int points;
    public int added_points;

    public int CompareTo(PlayerData other)
    {
        return other.points - points;
    }

    public bool Equals(PlayerData other)
    {
        return ID == other.ID && colorID == other.colorID && nickname == other.nickname && points == other.points && added_points == other.added_points && hairID==other.hairID && eyesID == other.eyesID;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ID);
        serializer.SerializeValue(ref colorID);
        serializer.SerializeValue(ref eyesID);
        serializer.SerializeValue(ref hairID);
        serializer.SerializeValue(ref nickname);
        serializer.SerializeValue(ref points);
        serializer.SerializeValue(ref added_points);
    }
}
public class CharacterSelectPlayer : MonoBehaviour
{

    [SerializeField] private GameObject readyText;
    [SerializeField] private int playerIndex;
    [SerializeField] private PlayerVisual playerVisual;
    [SerializeField] private TextMeshProUGUI nickText;
    
    private void Start() {
        
        GameState.Instance.OnPlayerDataNetworkListChanged += PlayerData_OnListChanged;
        CharacterSelect.Instance.onReadyChange += UpdatePlayerReadyText;

        

        UpdatePlayer();
    }

    private void UpdatePlayerReadyText(object sender, EventArgs e)
    {
        UpdatePlayer();
    }

    private void UpdatePlayer(){
        if(GameState.Instance.IsPlayerIndexConnected(playerIndex)){
            Show();
            PlayerData playerData = GameState.Instance.getPlayerData(playerIndex);
            readyText.SetActive(CharacterSelect.Instance.isPlayerReady(playerData.ID));
            playerVisual.setPlayerColor(GameState.Instance.getColor(playerData.colorID));
            nickText.text = playerData.nickname.ToString();
            playerVisual.setPlayerEyes(playerData.eyesID);
            playerVisual.setPlayerHair(playerData.hairID);

        }else{
            Hide();
        }
    }

    private void PlayerData_OnListChanged(object sender, EventArgs e)
    {
        UpdatePlayer();
    }

    private void Show(){
        gameObject.SetActive(true);
    }
    private void Hide(){
        gameObject.SetActive(false);
    }

    private void OnDestroy() {
        GameState.Instance.OnPlayerDataNetworkListChanged -= PlayerData_OnListChanged;
    }
}
