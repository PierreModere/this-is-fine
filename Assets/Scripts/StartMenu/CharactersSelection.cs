using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;
using TMPro;
using Unity.VisualScripting;

public class CharactersSelection : MonoBehaviour
{
    private GameObject WebsocketManager;
    UnityAction UA;
    string pincode;

    [SerializeField]
    private List<GameObject> selectedCharactersGameobject;
    [SerializeField]
    private List<Sprite> charactersSprites;

    void Start()
    {
        WebsocketManager = GameObject.Find("WebsocketManager");
        pincode = WebsocketManager.GetComponent<WebsocketManager>().joinedRoomCode;
        addClickEventOnCharactersGrid();
        updateSelectedAndAvailableCharacters();
        transform.Find("Cancel").gameObject.GetComponent<Button>().onClick.AddListener(unselectCharacter);

    }

    void addClickEventOnCharactersGrid()
    {
        Transform CharactersGrid = transform.Find("CharactersGrid");


        for (int i = 0; i < CharactersGrid.childCount; i++)
        {
            GameObject characterSprite = CharactersGrid.GetChild(i).gameObject;
            Button button = characterSprite.GetComponentInChildren<Button>();
                string characterName = characterSprite.name;
                UA = new UnityAction(() => selectCharacter(characterName));
                button.onClick.AddListener(UA);

            
        }
    }

    void removeClickEventOnCharactersGrid()
    {
        Transform CharactersGrid = transform.Find("CharactersGrid");
        for (int i = 0; i < CharactersGrid.childCount; i++)
        {
            GameObject characterSprite = CharactersGrid.GetChild(i).gameObject;
      /*      Button button = characterSprite.GetComponentInChildren<Button>();
            button.interactable = false;*/

        }
    }

    void reenableClickEvents()
    {
        Transform CharactersGrid = transform.Find("CharactersGrid");


        for (int i = 0; i < CharactersGrid.childCount; i++)
        {
            GameObject characterSprite = CharactersGrid.GetChild(i).gameObject;
/*            characterSprite.GetComponentInChildren<Button>().enabled = false;
*/            
        }
    }

 

    async void selectCharacter(string characterName)
    {
        var websocket = WebsocketManager.GetComponent<WebsocketManager>().websocket;
        string playerID = WebsocketManager.GetComponent<WebsocketManager>().playerID;
        string json = "{'type': 'selectCharacter', 'params':{'code': '" + pincode + "','characterName':'" + characterName + "','id':'" + playerID + "'}}";
        await websocket.SendText(json);

    }

    async void unselectCharacter()
    {
        transform.Find("Cancel").localScale = new Vector3(0f, 0f, 0f);
        reenableClickEvents();
        var websocket = WebsocketManager.GetComponent<WebsocketManager>().websocket;
        string playerID = WebsocketManager.GetComponent<WebsocketManager>().playerID;
        string json = "{'type': 'unselectCharacter', 'params':{'code': '" + pincode + "','id':'" + playerID + "'}}";
        await websocket.SendText(json);
    }
    public void updateSelectedAndAvailableCharacters()
    {
        var playersList = WebsocketManager.GetComponent<WebsocketManager>().playersList;
        checkPlayersReadyState();
        hasSelectedCharacter();

        foreach (GameObject selectedCharacter in selectedCharactersGameobject)
        {
            selectedCharacter.SetActive(false);
        }

        Transform CharactersGrid = transform.Find("CharactersGrid");

        foreach (Transform characterInGrid in CharactersGrid)
        {
            Image characterInGridSprite = characterInGrid.gameObject.GetComponent<Image>();
            var tempColor = characterInGridSprite.color;
            tempColor.a = 1f;
            characterInGridSprite.color = tempColor;
            characterInGrid.gameObject.GetComponent<Button>().enabled = true;

        }


        for (int i = 0; i < playersList.Count; i++)
        {

        if (playersList[i].selectedCharacter != "")
            {
                GameObject selectedCharacterFrame = selectedCharactersGameobject.Find(g => g.name == playersList[i].selectedCharacter).gameObject;
                selectedCharacterFrame.transform.Find("PlayerColor").Find("PlayerNumber").GetComponent<TextMeshProUGUI>().text= playersList[i].id.ToString();
                selectedCharacterFrame.SetActive(true);

                switch (playersList[i].id.ToString())
                {
                    case "1":
                        selectedCharacterFrame.transform.Find("PlayerColor").GetComponent<Image>().color = new Color32(255, 0, 0, 255);
                        break;
                    case "2":
                        selectedCharacterFrame.transform.Find("PlayerColor").GetComponent<Image>().color = new Color32(0, 0, 255, 255);
                        break;
                    case "3":
                        selectedCharacterFrame.transform.Find("PlayerColor").GetComponent<Image>().color = new Color32(0, 255, 0, 255);
                        break;
                    case "4":
                        selectedCharacterFrame.transform.Find("PlayerColor").GetComponent<Image>().color = new Color32(255, 255, 0, 255);
                        break;
                }
                if (playersList[i].selectedCharacter == WebsocketManager.GetComponent<WebsocketManager>().selectedCharacter)
                {
                    selectedCharacterFrame.transform.Find("LocalSelected").gameObject.SetActive(true);
                    selectedCharacterFrame.transform.Find("LocalSelected").gameObject.GetComponent<Image>().sprite = charactersSprites.Find(spr => spr.name == "SelectedFrame"+playersList[i].id.ToString());
                }
                else
                {
                    selectedCharacterFrame.transform.Find("LocalSelected").gameObject.SetActive(false); 
                }
            }
        }

    }


    void checkPlayersReadyState()
    {
        bool everyoneReady = true;
        var playersList = WebsocketManager.GetComponent<WebsocketManager>().playersList;
        foreach (var player in playersList)
        {
            if (!player.isReady)
                everyoneReady = false;
        }
        if (everyoneReady && WebsocketManager.GetComponent<WebsocketManager>().isHost)
            transform.Find("Play").localScale = new Vector3(1f, 1f, 1f);
        else transform.Find("Play").localScale = new Vector3(0f, 0f, 0f);


    }

    void hasSelectedCharacter()
    {
        if (WebsocketManager.GetComponent<WebsocketManager>().selectedCharacter != "")
        {
            removeClickEventOnCharactersGrid();
            transform.Find("Cancel").localScale = new Vector3(1f, 1f, 1f);
        }
    }
}