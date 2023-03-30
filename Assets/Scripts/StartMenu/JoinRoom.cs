using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class JoinRoom : MonoBehaviour
{
    [SerializeField]
    private GameObject PinInput;
    private GameObject WebsocketManager;

    void Start()
    {
        WebsocketManager = GameObject.Find("WebsocketManager");
    }

    async public void joinRoom()
    {
        var pincode = PinInput.GetComponent<TMP_InputField>().text.ToUpper();
        var websocket = WebsocketManager.GetComponent<WebsocketManager>().websocket;
        string json = "{'type': 'join', 'params':{'code': '" + pincode + "'}}";
        await websocket.SendText(json);
    }
}