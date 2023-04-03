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

    [SerializeField]
    private List<GameObject> playersListGameObject;
    void Start()
    {
        WebsocketManager = GameObject.Find("WebsocketManager");
        pincode = WebsocketManager.GetComponent<WebsocketManager>().joinedRoomCode;
        var playersList = WebsocketManager.GetComponent<WebsocketManager>().playersList;

    }

    void Update()
    {
        pincode = WebsocketManager.GetComponent<WebsocketManager>().joinedRoomCode;
        roomPincodeText.GetComponent<TextMeshProUGUI>().text = "Room " + pincode;
        var playersList = WebsocketManager.GetComponent<WebsocketManager>().playersList;
        if (playersList.Count > 0)
        {
            for (int i = 0; i < playersList.Count; i++)
            {
                GameObject playerNumber = playersListGameObject[playersList[i].id - 1];
                playerNumber.SetActive(true);
                if (WebsocketManager.GetComponent<WebsocketManager>().playerID == playersList[i].id.ToString())
                    playerNumber.GetComponent<PlayerNumber>().isClient = true;
            }
        }

    }
}