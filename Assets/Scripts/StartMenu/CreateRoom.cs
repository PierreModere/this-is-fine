using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CreateRoom : MonoBehaviour
{
    private GameObject WebsocketManager;
    void Start()
    {
        WebsocketManager = GameObject.Find("WebsocketManager");
    }

    async public void createRoom()
    {
        var websocket = WebsocketManager.GetComponent<WebsocketManager>().websocket;
        string json = "{'type': 'create'}";
        await websocket.SendText(json);
    }
}