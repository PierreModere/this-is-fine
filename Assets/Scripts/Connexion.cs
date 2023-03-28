using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NativeWebSocket;
using Newtonsoft.Json;

public class Connexion: MonoBehaviour {
  [SerializeField] private GameObject Counter;
  string websocketURL ="ws://localhost:1234";
  WebSocket websocket;

  [Serializable]
  public class ParsedJSON {
    public string type {
      get;
      set;
    }
    public Params @params {
      get;
      set;
    }
  }
  public class Params {
    public string action {
      get;
      set;
    }
    public string data {
      get;
      set;
    }
  }

  private ParsedJSON _ParsedJSON;

  // Start is called before the first frame update
  async void Start() {
    websocket = new WebSocket(websocketURL);

    websocket.OnOpen += () => {
      Debug.Log("Connexion open!");
      // SendWebSocketMessage();
    };

    websocket.OnError += (e) => {
      Debug.Log("Error! " + e);
    };

    websocket.OnClose += (e) => {
      Debug.Log("Connexion closed!");
    };

    websocket.OnMessage += (bytes) => {
      var json = System.Text.Encoding.UTF8.GetString(bytes);
      _ParsedJSON = JsonConvert.DeserializeObject < ParsedJSON > (json);

      switch (_ParsedJSON.type) {
      case "createdRoom":
        Counter.GetComponent < TextMeshProUGUI > ().text = "Created room " + _ParsedJSON.@params.data;
        break;

      case "joinedRoom":
        Counter.GetComponent < TextMeshProUGUI > ().text = "Joined room " + _ParsedJSON.@params.data;
        break;
      default:
        // code block
        break;
      }

      // Counter.GetComponent<TextMeshProUGUI>().text=_ParsedJSON.type;
    };

    // waiting for messages
    await websocket.Connect();
  }

  void Update() {
    #if!UNITY_WEBGL || UNITY_EDITOR
    websocket.DispatchMessageQueue();
    #endif
  }

  async void SendWebSocketMessage() {
    // string json = "{'type': 'join', 'params':{'code': 'WSYQ7'}}";
    string json = "{'type': 'create'}";
    await websocket.SendText(json);
  }

  private async void OnApplicationQuit() {
    await websocket.Close();
  }

  public async void IncrementInteger() {
    string json = "{'type': 'action', 'params':{'action': 'IncrementInteger'}}";
    await websocket.SendText(json);
  }

  public async void CreateNewRoom() {
    string json = "{'type': 'create'}";
    await websocket.SendText(json);
  }

  public async void JoinRoom(string pincode) {
    string json = "{'type': 'join', 'params':{'code': '"+pincode+"'}}";
    await websocket.SendText(json);
  }
}