using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System.Collections.Generic;

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

    async void selectCharacter(string characterName)
    {
        var websocket = WebsocketManager.GetComponent<WebsocketManager>().websocket;
        string playerID = WebsocketManager.GetComponent<WebsocketManager>().playerID;
        string json = "{'type': 'selectCharacter', 'params':{'code': '" + pincode + "','characterName':'" + characterName + "','id':'" + playerID + "'}}";
        await websocket.SendText(json);

    }

    public void updateSelectedAndAvailableCharacters()
    {
        var playersList = WebsocketManager.GetComponent<WebsocketManager>().playersList;

        foreach (var player in selectedCharactersGameobject)
        {
            player.SetActive(false);
        }

        Transform CharactersGrid = transform.Find("CharactersGrid");

        if (playersList.Count > 0)
        {
            for (int i = 0; i < playersList.Count; i++)
            {
                selectedCharactersGameobject[i].SetActive(true);



                if (playersList[i].selectedCharacter != null && playersList[i].selectedCharacter != "")
                {
                    selectedCharactersGameobject.Find(g => g.name == "Player" + playersList[i].id.ToString()).transform.Find("Character").gameObject.SetActive(true);
                    selectedCharactersGameobject.Find(g => g.name == "Player" + playersList[i].id.ToString()).transform.Find("Character").gameObject.GetComponent<Image>().sprite = charactersSprites.Find(spr => spr.name == playersList[i].selectedCharacter);
                    Image characterSprite = CharactersGrid.Find(playersList[i].selectedCharacter).gameObject.GetComponent<Image>();
                    var tempColor = characterSprite.color;
                    tempColor.a = 0.5f;
                    characterSprite.color = tempColor;
                    CharactersGrid.Find(playersList[i].selectedCharacter).gameObject.GetComponent<Button>().enabled = false;
                }
                else
                {
                    selectedCharactersGameobject.Find(g => g.name == "Player" + playersList[i].id.ToString()).transform.Find("Character").gameObject.SetActive(false);
                    selectedCharactersGameobject.Find(g => g.name == "Player" + playersList[i].id.ToString()).transform.Find("Character").gameObject.GetComponent<Image>().sprite = null;
                    Image characterSprite = CharactersGrid.Find(playersList[i].selectedCharacter).gameObject.GetComponent<Image>();
                    var tempColor = characterSprite.color;
                    tempColor.a = 1f;
                    characterSprite.color = tempColor;
                    CharactersGrid.Find(playersList[i].selectedCharacter).gameObject.GetComponent<Button>().enabled = false;
                }
            }

        }
    }

}