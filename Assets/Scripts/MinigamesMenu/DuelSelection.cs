using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DuelSelection : MonoBehaviour
{
    public GameData GameData;

    public GameObject ReturnButton;
    public GameObject OkButton;

    public List<Sprite> charactersSprites;
    public List<GameObject> playersGameobjects;

    private GameObject WebsocketManager;

    bool isDuel;
    bool isSelected;
    GameObject selectedContester;
    string selectedContesterID;

    // Start is called before the first frame update
    void Start()
    {
       displayPlayers();

    }

    void unactivePlayersGameobjects()
    {
        foreach (GameObject playerGO in playersGameobjects)
        {
            playerGO.SetActive(false);
        }
    }

    void displayPlayers()
    {
        unactivePlayersGameobjects();
        var playersList = GameData.playersList;

        for (int i = 0; i < playersList.Count; i++)
        {

            if (playersList[i].selectedCharacter != "")
            {
                GameObject playerGameObject = playersGameobjects.Find(g => g.name == "Player" + playersList[i].id);
                playerGameObject.SetActive(true);

                string playerID = playerGameObject.transform.Find("PlayerColor").Find("PlayerNumber").gameObject.GetComponent<TextMeshProUGUI>().text.Substring(1);

                playerGameObject.GetComponent<Button>().onClick.AddListener(() => { selecteContester(playerID, playerGameObject); });

                switch (playersList[i].id.ToString())
                {
                    case "1":
                        playerGameObject.transform.Find("PlayerColor").transform.GetComponent<Image>().color = new Color32(255, 0, 0, 255);
                        break;
                    case "2":
                        playerGameObject.transform.Find("PlayerColor").transform.GetComponent<Image>().color =  new Color32(0, 0, 255, 255);
                        break;
                    case "3":
                        playerGameObject.transform.Find("PlayerColor").transform.GetComponent<Image>().color =  new Color32(0, 255, 0, 255);
                        break;
                    case "4":
                        playerGameObject.transform.Find("PlayerColor").transform.GetComponent<Image>().color = new Color32(255, 255, 0, 255);
                        break;
                }

                playerGameObject.GetComponent<Image>().sprite = charactersSprites.Find(spr => spr.name == playersList[i].selectedCharacter);

                if (playersList[i].isDuel)
                {   
                    playerGameObject.GetComponent<Button>().interactable = false;
                    GameObject selectedFrame = playerGameObject.transform.Find("Selected").gameObject;
                    selectedFrame.SetActive(true);
                }
            }
        }
    }

    public void selecteContester(string id, GameObject player)
    {
        if (selectedContester != null) selectedContester.transform.Find("Selected").gameObject.SetActive(false);
        isSelected = true;
        selectedContesterID = id;
        selectedContester = player;
        selectedContester.transform.Find("Selected").gameObject.SetActive(isSelected);
        selectedContester.transform.DOScale(1.05f, 0.1f).OnComplete(() => { selectedContester.transform.DOScale(1f, 0.1f); });

        toggleOkButton();
    }

    void toggleOkButton()
    {
        OkButton.GetComponent<Button>().interactable = isSelected;
    }
    
    public void cancelSelection()
    {
        isSelected = false;
        selectedContester = null;
        selectedContesterID = null;

        WebsocketManager.GetComponent<WebsocketManager>().resetDuelStatus();
        GameData.isDuelHost = false;

        toggleOkButton();
    }

    public async void confirmSelection()
    {
        if (selectedContesterID != null && selectedContesterID != "")
        {
            WebsocketManager = GameObject.Find("WebsocketManager");

            var websocket = WebsocketManager.GetComponent<WebsocketManager>().websocket;

            string json = "{'type': 'selectDuelContester', 'params':{'code': '" + GameData.joinedRoomCode + "','id':'" + selectedContesterID + "'}}";
            await websocket.SendText(json);

            GameData.isDuelHost = true;

            isSelected = false;
            selectedContester.SetActive(isSelected);
            selectedContester = null;
            selectedContesterID = null;
            toggleOkButton();
        }
    }
}
