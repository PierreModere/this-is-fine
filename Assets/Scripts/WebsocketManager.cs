using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NativeWebSocket;
using Newtonsoft.Json;

public class WebsocketManager : MonoBehaviour
{
    [SerializeField]
    private string websocketURL;
    public string joinedRoomCode;
    public string playerID;
    public string selectedCharacter;
    public bool isHost;
    public WebSocket websocket;
    public GameObject LobbyCanvas;
    public GameObject ErrorCanvas;

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
    }

    private ParsedJSON _ParsedJSON;

    async void Start()
    {
        websocket = new WebSocket(websocketURL);

        websocket.OnOpen += () => {
            Debug.Log("Connexion open!");
        };

        websocket.OnError += (e) => {
            var ErrorsManager = ErrorCanvas.GetComponent<ErrorsManager>();
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
                    GameObject.Find("CreatedRoomPincode").GetComponent<TextMeshProUGUI>().text = "Created room " + _ParsedJSON.@params.data.message;
                    joinedRoomCode = _ParsedJSON.@params.@data.message;
                    isHost = true;
                    break;
                case "joinedRoom":
                    GameObject.Find("JoinRoomCanvas").tag = "Untagged";
                    GameObject.Find("JoinRoomCanvas").SetActive(false);
                    LobbyCanvas.SetActive(true);
                    LobbyCanvas.tag = "activeScreen";
                    joinedRoomCode = _ParsedJSON.@params.@data.message;
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
                    LobbyCanvas.GetComponent<LobbyScreen>().updatePlayersListInLobby();
                    if (GameObject.FindWithTag("activeScreen").name == "CharactersSelectionCanvas")
                    {
                        FindInactiveObjectByName("CharactersSelectionCanvas").GetComponent<CharactersSelection>().updateSelectedAndAvailableCharacters();
                    }
                    break;
                case "changedScreen":
                    changeScreenForEveryone(_ParsedJSON.@params.@data.message);
                    break;
                case "serverErrorMessage":
                    var ErrorsManager = ErrorCanvas.GetComponent<ErrorsManager>();
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

    void changeScreenForEveryone(string screenName)
    {
        var screenToEnable = FindInactiveObjectByName(screenName);
        var screenToDisable = GameObject.FindWithTag("activeScreen");
        if (screenToEnable != null && screenToDisable != null)
        {
            Debug.Log("ok!");
            screenToEnable.SetActive(true);
            screenToEnable.tag = "activeScreen";
            screenToDisable.SetActive(false);
            screenToDisable.tag = "Untagged";
        }
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