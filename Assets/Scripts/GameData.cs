
using System.Collections.Generic;
using UnityEngine;
using static WebsocketManager;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/GameData", order = 2)]
public class GameData : ScriptableObject
{
    public string websocketURL;
    public string joinedRoomCode;
    public string playerID;
    public string selectedCharacter;
    public string displayedMinigameID;
    public string minigameMode;
    public bool isHost;
    public bool isDuelHost;

    public List<ClientsList> playersList;
}