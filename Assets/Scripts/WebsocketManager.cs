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
    public WebSocket websocket;

    [Serializable]
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
    public class Params
    {
        public string action
        {
            get;
            set;
        }
        public string data
        {
            get;
            set;
        }
    }

    private ParsedJSON _ParsedJSON;

    // Start is called before the first frame update
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
            _ParsedJSON = JsonConvert.DeserializeObject<ParsedJSON>(json);

            switch (_ParsedJSON.type)
            {
                case "createdRoom":
                    GameObject.Find("CreatedRoomPincode").GetComponent<TextMeshProUGUI>().text = "Created room " + _ParsedJSON.@params.data;
                    joinedRoomCode = _ParsedJSON.@params.data;
                    break;

                case "joinedRoom":

                    GameObject.Find("JoinedRoomPincode").GetComponent<TextMeshProUGUI>().text = "Joined room " + _ParsedJSON.@params.data;
                    joinedRoomCode = _ParsedJSON.@params.data;
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