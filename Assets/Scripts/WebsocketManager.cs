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
    string websocketURL = "ws://localhost:1234";
    public string joinedRoomCode;
    public string playerID;
    public WebSocket websocket;
    public GameObject WaitingInRoomCanvas;

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
        public string chosenCharacter
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
            Debug.Log("Error! " + e);
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
                    break;
                case "joinedRoom":
                    GameObject.Find("JoinRoomCanvas").SetActive(false);
                    WaitingInRoomCanvas.SetActive(true);
                    joinedRoomCode = _ParsedJSON.@params.@data.message;
                    break;
                case "getMyPlayerID":
                    playerID = _ParsedJSON.@params.@data.message;
                    break;
                case "receivedPlayersList":
                    playersList = _ParsedJSON.@params.@data.clientsList;
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
        await websocket.Close();
    }

}   