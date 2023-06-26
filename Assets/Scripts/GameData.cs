
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
    public string firstMinigameID;
    public bool isFirstMinigame;
    public string minigameMode;
    public bool isHost;
    public bool isDuelHost;
    public string currentScene;
    public string winnerID;

    public List<ClientsList> playersList;

    public void resetGameData()
    {
        joinedRoomCode = "";
        playerID = "";
        playersList = null;
        selectedCharacter = "";
        displayedMinigameID = "";
        firstMinigameID = "";
        minigameMode = "Battle";
        isFirstMinigame = true;
        isDuelHost = false;
        isHost = false;
        currentScene = "";
        winnerID = "";
    }
}