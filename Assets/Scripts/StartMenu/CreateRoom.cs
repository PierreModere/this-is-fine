using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CreateRoom : MonoBehaviour
{
    private GameObject WebsocketManager;
    string pincode; 
    void Start()
    {
        WebsocketManager = GameObject.Find("WebsocketManager");
    }

    void Update()
    {
        pincode = WebsocketManager.GetComponent<WebsocketManager>().joinedRoomCode;
    }

    async public void createRoom()
    {
        var websocket = WebsocketManager.GetComponent<WebsocketManager>().websocket;
        string json = "{'type': 'create'}";
        await websocket.SendText(json);
    }

    async public void deleteCreatedRoom()
    {
        if (pincode != null)
        {
            var websocket = WebsocketManager.GetComponent<WebsocketManager>().websocket;
            string json = "{'type': 'delete', 'params':{'code': '" + pincode + "'}}";
            await websocket.SendText(json);
        }
    }
}