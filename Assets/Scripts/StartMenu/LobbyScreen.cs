using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyScreen : MonoBehaviour
{
    public GameData GameData;

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
        transform.Find("Return").gameObject.GetComponent<Button>().onClick.AddListener(quitLobby);
    }
    public void setPincode(string getPincode) {
        pincode = getPincode;
        roomPincodeText.GetComponent<TextMeshProUGUI>().text = "Room " + pincode;

    }

    public void updatePlayersListInLobby()
    {
        var playersList = GameData.playersList;

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
                if (playersList.Count >(requiredPlayersNumber - 1) && GameData.isHost == true)
                {
                    transform.Find("Ok").gameObject.SetActive(true);
                }
                else
                {
                    transform.Find("Ok").gameObject.SetActive(false);
                }
                if (GameData.playerID == playersList[i].id.ToString())
                    playerNumber.GetComponent<PlayerNumber>().isLocalClient = true;
            }

        }
    }

    async public void quitLobby()
    {
        if (pincode != "")
        {
            string json;
            WebsocketManager = GameObject.Find("WebsocketManager");
            var websocket = WebsocketManager.GetComponent<WebsocketManager>().websocket;
            json = "{'type': 'leave', 'params':{'code': '" + pincode + "','id': '" + GameData.playerID + "'}}";
            
            await websocket.SendText(json);
            GameData.joinedRoomCode = "";
        }
    }

}