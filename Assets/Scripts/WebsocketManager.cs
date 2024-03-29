using DG.Tweening;
using NativeWebSocket;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WebsocketManager : MonoBehaviour
{

    public GameData GameData;

    public WebSocket websocket;

    public GameObject ReconnectionButton;

    public List<ClientsList> playersList;

    public class ParsedJSON
    {
        public string type
        {
            get;
            set;
        }
        public Params @params
        {
            get;
            set;
        }
    }

    public class Data
    {
        public List<ClientsList> clientsList
        {
            get;
            set;
        }
        public string message
        {
            get;
            set;
        }
    }

    public class Params
    {
        public string action
        {
            get;
            set;
        }
        public Data data
        {
            get;
            set;
        }
    }

    public class ClientsList
    {
        public int id
        {
            get;
            set;
        }
        public string selectedCharacter
        {
            get;
            set;
        }

        public string score
        {
            get;
            set;
        }

        public bool isReady
        {
            get;
            set;
        }
        public bool isDuel
        {
            get;
            set;
        }
    }

    private ParsedJSON _ParsedJSON;

    [DllImport("__Internal")]
    private static extern void savePlayerData(string id, string roomCode);

    [DllImport("__Internal")]
    private static extern string getPlayerDataFromLocalStorage(string property);

    [DllImport("__Internal")]
    private static extern void console(string str);

    [DllImport("__Internal")]
    private static extern void clearLocalStorage();

    void Start()
    {
        GameData.resetGameData();
        WebsocketConnect(false);
    }

    async void WebsocketConnect(bool isReconnecting)
    {
        websocket = new WebSocket(GameData.websocketURL);

        websocket.OnOpen += () => {
            Debug.Log("Connexion open!");
            if (isReconnecting)
            {
                reconnectToRoom(GameData.playerID, GameData.joinedRoomCode);
            }
        };

        websocket.OnError += (e) => {
            //WebsocketConnect(true);
        };

        websocket.OnClose += (e) => {
            WebsocketConnect(true);
        };

        websocket.OnMessage += (bytes) => {
            var json = System.Text.Encoding.UTF8.GetString(bytes);
            var jsonResult = JsonConvert.DeserializeObject(json).ToString();

            _ParsedJSON = JsonConvert.DeserializeObject<ParsedJSON>(jsonResult);

            switch (_ParsedJSON.type)
            {
                case "createdRoom":
                    showLobbyScreen(true);
                    GameData.isHost = true;

                    break;
                case "joinedRoom":
                    showLobbyScreen(false);
                    GameData.isHost = false;
                    break;
                case "hasBeenInARoom":
                    if (_ParsedJSON.@params.@data.message != "")
                    {
                        if (_ParsedJSON.@params.@data.message.Split(char.Parse("-"))[0] != GameData.currentScene && _ParsedJSON.@params.@data.message.Split(char.Parse("-"))[0] != "undefined")
                        {
                            GameData.currentScene = _ParsedJSON.@params.@data.message.Split(char.Parse("-"))[0];
                            GameObject.Find("SceneManager").GetComponent<ChangeScene>().playTransitionAnim();
                        }

                        if (GameObject.FindWithTag("activeScreen").name != _ParsedJSON.@params.@data.message.Split(char.Parse("-"))[1])
                            changeScreenForEveryone(_ParsedJSON.@params.@data.message.Split(char.Parse("-"))[1]);

                    }
                    break;
                case "getMyPlayerID":
                    if (!Application.isEditor && GameData.playerID == "")
                    {
                        savePlayerData(_ParsedJSON.@params.@data.message, GameData.joinedRoomCode);
                    }
                    GameData.playerID = _ParsedJSON.@params.@data.message;
                    break;
                case "getMySelectedCharacter":
                    GameData.selectedCharacter = _ParsedJSON.@params.@data.message;
                    break;
                case "receivedPlayersList":
                    GameData.playersList = _ParsedJSON.@params.@data.clientsList;

                    if (GameObject.FindWithTag("activeScreen") != null)

                    {
                        switch (GameObject.FindWithTag("activeScreen").name)
                        {
                            case "LobbyCanvas":
                                FindInactiveObjectByName("LobbyCanvas").GetComponent<LobbyScreen>().updatePlayersListInLobby();
                                break;
                            case "CharactersSelectionCanvas":
                                FindInactiveObjectByName("CharactersSelectionCanvas").GetComponent<CharactersSelection>().updateSelectedAndAvailableCharacters();
                                break;
                            case "MinigameInstructionsCanvas":
                                FindInactiveObjectByName("MinigameInstructionsCanvas").GetComponent<MinigameInstructions>().updatePlayersList();
                                break;

                            case "MinigameInfoCanvas":
                                FindInactiveObjectByName("MinigameInfoCanvas").GetComponent<MinigameUI>().updatePlayersListAndScore();
                                break;
                        }
                    }

                    break;
                case "receivedFirstMinigameID":
                    GameData.firstMinigameID = _ParsedJSON.@params.@data.message;
                    GameData.displayedMinigameID = GameData.firstMinigameID;
                    break;
                case "receivedSelectedMinigame":
                    GameData.displayedMinigameID = _ParsedJSON.@params.@data.message;
                    break;
                case "setMinigameMode":
                    GameData.minigameMode = _ParsedJSON.@params.@data.message;
                    break;
                case "finishMinigameAnimation":
                    FindInactiveObjectByName("MinigameInfoCanvas").GetComponent<MinigameUI>().finishMinigameAnimation();
                    break;
                case "changedScreen":
                    changeScreenForEveryone(_ParsedJSON.@params.@data.message);
                    break;
                case "changedScene":
                    string sceneName = _ParsedJSON.@params.@data.message;
                    if (sceneName != "")
                    {
                        GameData.currentScene = sceneName;
                        GameObject.Find("SceneManager").GetComponent<ChangeScene>().playTransitionAnim();
                    }
                    break;
                case "serverErrorMessage":
                    var ErrorsManager = FindInactiveObjectByName("ErrorCanvas").GetComponent<ErrorsManager>();
                    ErrorsManager.manageErrors(_ParsedJSON.@params.@data.message);
                    break;
                case "selectedWinner":
                    displayWinnerCutscene(_ParsedJSON.@params.@data.message);
                    break;
                default:
                    // code block
                    break;
            }

        };
        // waiting for messages
        await websocket.Connect();
    }

    void displayWinnerCutscene(string id)
    {
        if (id != "")
        {

            if (!Application.isEditor)
            {
                clearLocalStorage();
            }

            GameData.winnerID = id;
            GameData.currentScene = "WinnerCinematicScene";
            GameObject.Find("FadePanel").GetComponent<Image>().DOFade(1, 0.65f).OnComplete(() => {
                SceneManager.LoadScene(GameData.currentScene, LoadSceneMode.Single);
            });

        }
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket.DispatchMessageQueue();
#endif
    }

    private async void OnApplicationQuit()
    {
        if (GameData.joinedRoomCode != null && GameData.joinedRoomCode != "")
        {
            string json = "{'type': 'leave', 'params':{'code': '" + GameData.joinedRoomCode + "','id': '" + GameData.playerID + "'}}";
            await websocket.SendText(json);
        }
        GameData.resetGameData();
        await websocket.Close();
    }

    void showLobbyScreen(bool isHost)
    {
        GameData.joinedRoomCode = _ParsedJSON.@params.@data.message;
        if (isHost)
        {
            GameObject.Find("CreateRoomCanvas").tag = "Untagged";
            GameObject.Find("CreateRoomCanvas").SetActive(false);

        }
        else
        {
            GameObject.Find("JoinRoomCanvas").tag = "Untagged";
            GameObject.Find("JoinRoomCanvas").SetActive(false);
        }
        FindInactiveObjectByName("LobbyCanvas").SetActive(true);
        FindInactiveObjectByName("LobbyCanvas").transform.DOScale(new Vector3(1.03f, 1.03f, 1.03f), 0.1f).OnComplete(() => {
            FindInactiveObjectByName("LobbyCanvas").transform.DOScale(new Vector3(1f, 1f, 1f), 0.15f);
        });
        FindInactiveObjectByName("LobbyCanvas").tag = "activeScreen";
        FindInactiveObjectByName("LobbyCanvas").GetComponent<LobbyScreen>().setPincode(GameData.joinedRoomCode);
    }

    public void changeScreenForEveryone(string screenName)
    {
        Debug.Log(screenName);
        GameObject screenToEnable = FindInactiveObjectByName(screenName);
        GameObject screenToDisable = GameObject.FindWithTag("activeScreen");
        if (screenToEnable != null && screenToDisable != null && screenToDisable.name != "StartMenuCanvas")
        {
            screenToEnable.SetActive(true);
            screenToEnable.tag = "activeScreen";
            screenToDisable.SetActive(false);
            screenToDisable.tag = "Untagged";
            screenToEnable.transform.DOScale(new Vector3(1.05f, 1.05f, 1.05f), 0.1f).OnComplete(() => {
                screenToEnable.transform.DOScale(new Vector3(1f, 1f, 1f), 0.15f);
            });
        }
    }

    public async void sendScreenNameToSwitchTo(GameObject ScreenToSwitchTo)
    {
        string json = "{'type': 'changeScreen', 'params':{'code': '" + GameData.joinedRoomCode + "','screenName':'" + ScreenToSwitchTo.name + "'}}";
        await websocket.SendText(json);
    }

    public async void sendSelectedMinigame(string minigameID)
    {

        string json = "{'type': 'selectMinigame', 'params':{'code': '" + GameData.joinedRoomCode + "','minigameID':'" + minigameID + "'}}";
        await websocket.SendText(json);
    }

    public async void sendMinigameMode(string mode, string id)
    {
        string json;
        if (id != null && id != "") json = "{'type': 'setMinigameMode', 'params':{'code': '" + GameData.joinedRoomCode + "','mode':'" + mode + "','id':'" + id + "'}}";
        else json = "{'type': 'setMinigameMode', 'params':{'code': '" + GameData.joinedRoomCode + "','mode':'" + mode + "'}}";
        await websocket.SendText(json);
    }

    public async void resetDuelStatus()
    {
        string json = "{'type': 'resetDuelStatus', 'params':{'code': '" + GameData.joinedRoomCode + "'}}";
        await websocket.SendText(json);
        GameData.isDuelHost = false;

    }

    public async void sendScore(int score)
    {
        string json = "{'type': 'updatePlayerScore', 'params':{'code': '" + GameData.joinedRoomCode + "','id':'" + GameData.playerID + "','score':'" + score + "'}}";
        await websocket.SendText(json);
    }

    public async void sendFinishMinigameAnimation()
    {
        string json = "{'type': 'endMinigame', 'params':{'code': '" + GameData.joinedRoomCode + "'}}";
        await websocket.SendText(json);
    }

    public async void returnToDashboard()
    {
        string json = "{'type': 'returnToDashboard', 'params':{'code': '" + GameData.joinedRoomCode + "'}}";
        await websocket.SendText(json);
        GameData.isDuelHost = false;

    }

    public async void checkPlayerHasBeenInRoom()
    {
        if (!Application.isEditor)
        {
            string id = getPlayerDataFromLocalStorage("playerID");
            string roomCode = getPlayerDataFromLocalStorage("roomCode");
            if (roomCode != null && id != null)
            {
                string json = "{'type': 'checkRoomExistance', 'params':{'code': '" + roomCode + "','id':'" + id + "'}}";
                await websocket.SendText(json);
            }
        }
    }

    async void reconnectToRoom(string playerID, string roomCode)
    {
       // if (!Application.isEditor) console("tentative de reconnexion");
        string json = "{'type': 'reconnectPlayer', 'params':{'code': '" + roomCode + "','id':'" + playerID + "'}}";
        await websocket.SendText(json);
    }

    public async void returnToRoom()
    {
        string playerID = getPlayerDataFromLocalStorage("playerID");
        string roomCode = getPlayerDataFromLocalStorage("roomCode");

        string json = "{'type': 'reconnectPlayer', 'params':{'code': '" + roomCode + "','id':'" + playerID + "'}}";
        await websocket.SendText(json);
    }

    GameObject FindInactiveObjectByName(string name)
    {
        Transform[] objs = Resources.FindObjectsOfTypeAll<Transform>() as Transform[];
        foreach (Transform obj in objs)
        {
            if (obj.name == name && !obj.gameObject.activeInHierarchy || obj.name == name && obj.gameObject.activeInHierarchy)
            {
                return obj.gameObject;
            }
        }
        return null;
    }
}