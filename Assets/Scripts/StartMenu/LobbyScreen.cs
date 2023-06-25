using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField]
    private List<Sprite> playersNamesSprites;
    void Start()
    {
        updatePlayersListInLobby();
        transform.Find("Return").gameObject.GetComponent<Button>().onClick.AddListener(quitLobby);
    }
    public void setPincode(string getPincode)
    {
        pincode = getPincode;
        roomPincodeText.GetComponent<TextMeshProUGUI>().text = pincode;
    }

    public void updatePlayersListInLobby()
    {
        var playersList = GameData.playersList;

        foreach (var player in playersListGameObject)
        {
            player.SetActive(false);
        }
        if (playersList !=null && playersList.Count > 0)
        {
            for (int i = 0; i < playersList.Count; i++)
            {
                GameObject playerNumber = playersListGameObject[playersList[i].id - 1];
                playerNumber.SetActive(true);
                if (playersList.Count > (requiredPlayersNumber - 1) && GameData.isHost == true)
                {
                    transform.Find("Ok").gameObject.GetComponent<Button>().interactable = true;
                }
                else
                {
                    transform.Find("Ok").gameObject.GetComponent<Button>().interactable = false;
                }
                if (GameData.playerID == playersList[i].id.ToString()) { 
                    playerNumber.GetComponent<Image>().sprite = playersNamesSprites.Find(spr => spr.name == "player"+ GameData.playerID);
                    playerNumber.transform.Find("PlayerNumber").gameObject.GetComponent<TextMeshProUGUI>().color = Color.white;
                    playerNumber.transform.Find("PlayerName").gameObject.GetComponent<TextMeshProUGUI>().color = Color.white;
                }

                //playerNumber.GetComponent<PlayerNumber>().isLocalClient = true;
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
            GameData.isHost = false;
            GameData.playerID = "";
            GameData.playersList = null;
            GameData.joinedRoomCode = "";
        }
    }

}