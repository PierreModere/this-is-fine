using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System.Collections.Generic;
using DG.Tweening;

public class CharactersSelection : MonoBehaviour
{
    private GameObject WebsocketManager;
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
    }

    void addClickEventOnCharactersGrid()
    {
        Transform CharactersGrid = transform.Find("CharactersGrid");

        for (int i = 0; i < CharactersGrid.childCount; i++)
        {
            GameObject characterSprite = CharactersGrid.GetChild(i).gameObject;
            Button button = characterSprite.GetComponentInChildren<Button>();

            if (button != null)
            {
                string characterName = characterSprite.name;

                button.onClick.AddListener(delegate {
                    selectCharacter(characterName);
                });

            }
        }
    }

    void removeClickEventOnCharactersGrid()
    {
        Transform CharactersGrid = transform.Find("CharactersGrid");
        for (int i = 0; i < CharactersGrid.childCount; i++)
        {
            GameObject characterSprite = CharactersGrid.GetChild(i).gameObject;
            Button button = characterSprite.GetComponentInChildren<Button>();

            if (button != null)
            {
                button.onClick.RemoveAllListeners();

            }
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
        transform.Find("Cancel").DOScale(0f, 0f);
        addClickEventOnCharactersGrid();
        var websocket = WebsocketManager.GetComponent<WebsocketManager>().websocket;
        string playerID = WebsocketManager.GetComponent<WebsocketManager>().playerID;
        string json = "{'type': 'unselectCharacter', 'params':{'code': '" + pincode + "','id':'" + playerID + "'}}";
        await websocket.SendText(json);
    }

    public void updateSelectedAndAvailableCharacters()
    {
        if (WebsocketManager.GetComponent<WebsocketManager>().selectedCharacter != "" && WebsocketManager.GetComponent<WebsocketManager>().selectedCharacter != null)
        {
            removeClickEventOnCharactersGrid();
            transform.Find("Cancel").DOScale(1f , 0.4f );
            transform.Find("Cancel").gameObject.GetComponent<Button>().onClick.AddListener(unselectCharacter);
        }

        foreach (GameObject player in selectedCharactersGameobject)
        {
            player.SetActive(false);
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

        var playersList = WebsocketManager.GetComponent<WebsocketManager>().playersList;

        if (playersList.Count > 0)
        {
            for (int i = 0; i < playersList.Count; i++)
            {
                selectedCharactersGameobject[i].SetActive(true);

                GameObject selectedCharacterSprite = selectedCharactersGameobject.Find(g => g.name == "Player" + playersList[i].id.ToString()).transform.Find("Character").gameObject;
                

                if (playersList[i].selectedCharacter != null && playersList[i].selectedCharacter != "")
                {
                    selectedCharacterSprite.SetActive(true);
                    selectedCharacterSprite.GetComponent<Image>().sprite = charactersSprites.Find(spr => spr.name == playersList[i].selectedCharacter);
                    Image characterInGridSprite = CharactersGrid.Find(playersList[i].selectedCharacter).gameObject.GetComponent<Image>();
                    var tempColor = characterInGridSprite.color;
                    tempColor.a = 0.5f;
                    characterInGridSprite.color = tempColor;
                    CharactersGrid.Find(playersList[i].selectedCharacter).gameObject.GetComponent<Button>().enabled = false;
                }
                else
                {
                    selectedCharacterSprite.GetComponent<Image>().sprite = null;
                    selectedCharacterSprite.SetActive(false);
                }
            }

        }
    }

}