using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NativeWebSocket;

public class Connexion : MonoBehaviour
{
  WebSocket websocket;

  // Start is called before the first frame update
  async void Start()
  {
    websocket = new WebSocket("ws://localhost:6969");

    websocket.OnOpen += () =>
    {
      Debug.Log("Connexion open!");
      SendWebSocketMessage();
    };

    websocket.OnError += (e) =>
    {
      Debug.Log("Error! " + e);
    };

    websocket.OnClose += (e) =>
    {
      Debug.Log("Connexion closed!");
    };

    websocket.OnMessage += (bytes) =>
    {
    var message = System.Text.Encoding.UTF8.GetString(bytes);
    Debug.Log("Server : " + message);
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

  async void SendWebSocketMessage()
  {
    string json = "{'type': 'join', 'params':{'code': 'WSYQ7'}}";
    // string json = "{'type': 'create'}";

    await websocket.SendText(json);
  }

  private async void OnApplicationQuit()
  {
    await websocket.Close();
  }
}