using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyScreen : MonoBehaviour
{

    [SerializeField]
    private GameObject roomPincodeText;
    private GameObject WebsocketManager;
    string pincode;

    public int requiredPlayersNumber;

    [SerializeField]
    private List<GameObject> playersListGameObject;

    void Start()
    {
        updatePlayersListInLobby();
    }
    public void setPincode(string getPincode) {
        pincode = getPincode;
        roomPincodeText.GetComponent<TextMeshProUGUI>().text = "Room " + pincode;

    }

    public void updatePlayersListInLobby()
    {
        WebsocketManager = GameObject.Find("WebsocketManager");
        var playersList = WebsocketManager.GetComponent<WebsocketManager>().playersList;

        foreach ( var player in playersListGameObject)
        {
            player.SetActive(false);
        }
        if (playersList.Count > 0)
        {
            for (int i = 0; i < playersList.Count; i++) 
            {
                GameObject playerNumber = playersListGameObject[playersList[i].id - 1];
                playerNumber.SetActive(true);
                if (playersList.Count >(requiredPlayersNumber - 1) && WebsocketManager.GetComponent<WebsocketManager>().isHost == true)
                {
                    transform.Find("Continue").gameObject.SetActive(true);
                }
                else
                {
                    transform.Find("Continue").gameObject.SetActive(false);
                }
                if (WebsocketManager.GetComponent<WebsocketManager>().playerID == playersList[i].id.ToString())
                    playerNumber.GetComponent<PlayerNumber>().isLocalClient = true;
            }

        }
    }
}