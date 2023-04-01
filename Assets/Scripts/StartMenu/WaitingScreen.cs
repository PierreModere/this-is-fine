using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaitingScreen : MonoBehaviour
{

    [SerializeField]
    private GameObject roomPincodeText;
    private GameObject WebsocketManager;
    string pincode;

    // Start is called before the first frame update
    void Start()
    {
        WebsocketManager = GameObject.Find("WebsocketManager");
        pincode =  WebsocketManager.GetComponent<WebsocketManager>().joinedRoomCode;
    }

    // Update is called once per frame
    void Update()
    {
        pincode = WebsocketManager.GetComponent<WebsocketManager>().joinedRoomCode;
        roomPincodeText.GetComponent<TextMeshProUGUI>().text = "Room " + pincode;

    }
}
