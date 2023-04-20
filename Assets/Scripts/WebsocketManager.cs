using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NativeWebSocket;
using Newtonsoft.Json;
using DG.Tweening;

public class WebsocketManager : MonoBehaviour
{
    [SerializeField]
    private string websocketURL;
    public string joinedRoomCode;
    public string playerID;
    public string selectedCharacter;
    public string displayedMinigameID;
    public string minigameMode;
    public bool isHost;
    public WebSocket websocket; 

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

    async void Start()
    {
        websocket = new WebSocket(websocketURL);

        websocket.OnOpen += () => {
            Debug.Log("Connexion open!");
        };

        websocket.OnError += (e) => {
            var ErrorsManager = FindInactiveObjectByName("ErrorCanvas").GetComponent<ErrorsManager>();
            ErrorsManager.manageErrors(e);
        };

        websocket.OnClose += (e) => {
            Debug.Log("Connexion closed!");
        };

        websocket.OnMessage += (bytes) => {
            var json = System.Text.Encoding.UTF8.GetString(bytes);
            var jsonResult = JsonConvert.DeserializeObject(json).ToString();

            _ParsedJSON = JsonConvert.DeserializeObject<ParsedJSON>(jsonResult);

            switch (_ParsedJSON.type)
            {
                case "createdRoom":
                    showLobbyScreen(true);
                    isHost = true;
                    break;
                case "joinedRoom":
                    showLobbyScreen(false);
                    isHost = false;
                    break;
                case "getMyPlayerID":
                    playerID = _ParsedJSON.@params.@data.message;
                    break;
                case "getMySelectedCharacter":
                    selectedCharacter = _ParsedJSON.@params.@data.message;
                    break;
                case "receivedPlayersList":
                    playersList = _ParsedJSON.@params.@data.clientsList;

                    if (GameObject.FindWithTag("activeScreen")!= null)

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
                        }
                    }

                    break;
                case "receivedSelectedMinigame":
                    displayedMinigameID = _ParsedJSON.@params.@data.message;

                    if (GameObject.FindWithTag("activeScreen") != null)

                    {
                        switch (GameObject.FindWithTag("activeScreen").name)
                        {
                            case "FirstMinigameInstructionCanvas":
                                FindInactiveObjectByName("FirstMinigameInstructionCanvas").GetComponent<FirstMinigameAnimation>().displaySelectedMinigame();
                                break;
                        }
                    }

                    break;
                case "setMinigameMode":
                    minigameMode = _ParsedJSON.@params.@data.message;
                    break;
                case "changedScreen":
                    changeScreenForEveryone(_ParsedJSON.@params.@data.message);
                    break;
                case "changedScene":
                    string sceneName = _ParsedJSON.@params.@data.message;
                    if (sceneName != "")
                        GameObject.Find("SceneManager").GetComponent<ChangeScene>().changeScene(sceneName);
                    break;
                        case "serverErrorMessage":
                    var ErrorsManager = FindInactiveObjectByName("ErrorCanvas").GetComponent<ErrorsManager>();
                    ErrorsManager.manageErrors(_ParsedJSON.@params.@data.message);
                    break;

                default:
                    // code block
                    break;
            }

        };
        // waiting for messages
        await websocket.Connect();
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket.DispatchMessageQueue();
#endif
    }

    private async void OnApplicationQuit()
    {
        if (joinedRoomCode != null && joinedRoomCode != "")
        {
            string json = "{'type': 'leave', 'params':{'code': '" + joinedRoomCode + "','id': '" + playerID + "'}}";
            await websocket.SendText(json);
        }
        await websocket.Close();
    }

    public async void OnWebglClose()
    {
        if (joinedRoomCode != null && joinedRoomCode != "")
        {
            string json = "{'type': 'leave', 'params':{'code': '" + joinedRoomCode + "','id': '" + playerID + "'}}";
            await websocket.SendText(json);
        }
        await websocket.Close();
    }

    void showLobbyScreen(bool isHost)
    {
        joinedRoomCode = _ParsedJSON.@params.@data.message;
        if(isHost)
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
        FindInactiveObjectByName("LobbyCanvas").transform.DOScale(new Vector3(1.03f, 1.03f, 1.03f), 0.1f).OnComplete(() => { FindInactiveObjectByName("LobbyCanvas").transform.DOScale(new Vector3(1f, 1f, 1f), 0.15f); });
        FindInactiveObjectByName("LobbyCanvas").tag = "activeScreen";
        FindInactiveObjectByName("LobbyCanvas").GetComponent<LobbyScreen>().setPincode(joinedRoomCode);
    }

    void changeScreenForEveryone(string screenName)
    {
        var screenToEnable = FindInactiveObjectByName(screenName);
        var screenToDisable = GameObject.FindWithTag("activeScreen");
        if (screenToEnable != null && screenToDisable != null)
        {
            screenToEnable.SetActive(true);
            screenToEnable.transform.DOScale(new Vector3(1.03f, 1.03f, 1.03f), 0.1f).OnComplete(() => { screenToEnable.transform.DOScale(new Vector3(1f, 1f, 1f), 0.15f); });
            screenToEnable.tag = "activeScreen";
            screenToDisable.SetActive(false);
            screenToDisable.tag = "Untagged";
        }
    }

    public async void sendSelectedMinigame(string minigameID)
    {
        string json = "{'type': 'selectMinigame', 'params':{'code': '" + joinedRoomCode + "','minigameID':'" + minigameID + "'}}";
        await websocket.SendText(json);
    }

    public async void sendMinigameMode(string mode, string id)
    {
        string json;
        if (id != null && id != "") json = "{'type': 'setMinigameMode', 'params':{'code': '" + joinedRoomCode + "','mode':'" + mode + "','id':'" + id + "'}}"; 
        else json = "{'type': 'setMinigameMode', 'params':{'code': '" + joinedRoomCode + "','mode':'" + mode + "'}}";
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