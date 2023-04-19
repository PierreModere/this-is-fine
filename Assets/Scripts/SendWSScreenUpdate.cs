using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendWSScreenUpdate : MonoBehaviour
{
    [SerializeField]
    private GameObject ScreenToSwitchTo;
    private GameObject WebsocketManager;
    string pincode;

    // Start is called before the first frame update
    void Start()
    {
        WebsocketManager = GameObject.Find("WebsocketManager");
        var websocket = WebsocketManager.GetComponent<WebsocketManager>().websocket;
        pincode = WebsocketManager.GetComponent<WebsocketManager>().joinedRoomCode;
    }

    public async void sendScreenNameToSwitchTo()
    {
        pincode = WebsocketManager.GetComponent<WebsocketManager>().joinedRoomCode;
        if (ScreenToSwitchTo != null)
        {
            var websocket = WebsocketManager.GetComponent<WebsocketManager>().websocket;
            string json = "{'type': 'changeScreen', 'params':{'code': '" + pincode + "','screenName':'"+ScreenToSwitchTo.name+"'}}";
            await websocket.SendText(json);
        }
    }
}
