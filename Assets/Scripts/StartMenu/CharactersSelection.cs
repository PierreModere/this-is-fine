using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CharactersSelection : MonoBehaviour
{
    public GameData GameData;

    private GameObject WebsocketManager;
    UnityAction UA;

    public GameObject ReturnButton;
    public GameObject CancelButton;
    public GameObject OkButton;


    [SerializeField]
    private List<GameObject> charactersButtons;

    [SerializeField]
    private List<Sprite> selectedOutlinesSprites;

    [SerializeField]
    private List<Sprite> charactersSprites;
    [SerializeField]
    private List<Sprite> selectedCharactersSprites;
    [SerializeField]
    private List<Sprite> playerNumbersSprites;

    void Start()
    {
        WebsocketManager = GameObject.Find("WebsocketManager");
        addClickEventOnCharactersGrid();
        updateSelectedAndAvailableCharacters();
        CancelButton.GetComponent<Button>().onClick.AddListener(unselectCharacter);
        if (GameData.isHost)
            OkButton.SetActive(true);
        OkButton.GetComponent<Button>().onClick.AddListener(startGame);
    }

    private void OnEnable()
    {
        updateSelectedAndAvailableCharacters();
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
            characterSprite.GetComponentInChildren<Button>().interactable = false;

        }
    }

    void reenableClickEvents()
    {
        Transform CharactersGrid = transform.Find("CharactersGrid");
        for (int i = 0; i < CharactersGrid.childCount; i++)
        {
            GameObject characterSprite = CharactersGrid.GetChild(i).gameObject;
            characterSprite.GetComponentInChildren<Button>().interactable = true;

        }
    }



    async void selectCharacter(string characterName)
    {
        var websocket = WebsocketManager.GetComponent<WebsocketManager>().websocket;
        string playerID = GameData.playerID;
        string json = "{'type': 'selectCharacter', 'params':{'code': '" + GameData.joinedRoomCode + "','characterName':'" + characterName + "','id':'" + playerID + "'}}";
        await websocket.SendText(json);

    }

    async void unselectCharacter()
    {
        CancelButton.SetActive(false);
        var websocket = WebsocketManager.GetComponent<WebsocketManager>().websocket;
        string playerID = GameData.playerID;
        string json = "{'type': 'unselectCharacter', 'params':{'code': '" + GameData.joinedRoomCode + "','id':'" + playerID + "'}}";
        await websocket.SendText(json);
    }
    public void updateSelectedAndAvailableCharacters()
    {
        var playersList = GameData.playersList;
        hasSelectedCharacter();
        checkPlayersReadyState();

        foreach (GameObject button in charactersButtons)
        {
            button.GetComponent<Image>().sprite = selectedOutlinesSprites.Find(spr => spr.name == "characterBackgroundCircle");
            button.transform.Find("Mask").Find("Sprite").gameObject.GetComponent<Image>().sprite = charactersSprites.Find(sprite => sprite.name == button.name + "_idle");

            button.GetComponent<Button>().interactable = true;

            button.transform.Find("PlayerNumber").gameObject.SetActive(false);

        }

        Transform CharactersGrid = transform.Find("CharactersGrid");

        for (int i = 0; i < playersList.Count; i++)
        {

            if (playersList[i].selectedCharacter != "")
            {
                GameObject selectedCharacter = charactersButtons.Find(g => g.name == playersList[i].selectedCharacter).gameObject;
                selectedCharacter.GetComponent<Image>().sprite = selectedOutlinesSprites.Find(spr => spr.name == "selectedPlayer" + playersList[i].id);
                selectedCharacter.GetComponent<Button>().interactable = false;

                GameObject PlayerNumber = selectedCharacter.transform.Find("PlayerNumber").gameObject;
                PlayerNumber.GetComponent<Image>().sprite = playerNumbersSprites.Find(spr => spr.name == "player" + playersList[i].id);
                PlayerNumber.SetActive(true);

                GameObject Sprite = selectedCharacter.transform.Find("Mask").Find("Sprite").gameObject;
                if (Sprite.GetComponent<Image>().sprite.name == playersList[i].selectedCharacter.ToString() + "_idle" && !selectedCharacter.GetComponent<Button>().interactable)
                {
                    Sprite.GetComponent<Image>().sprite = selectedCharactersSprites.Find(sprite => sprite.name == playersList[i].selectedCharacter.ToString() + "_selected");
                    Sequence anim = DOTween.Sequence();
                    //anim.Append(Sprite.transform.DOLocalMoveY(20, 0.1f).SetEase(Ease.OutElastic).From());
                    anim.Join(Sprite.transform.DOScale(1.2f, 0.15f).From());
                }

            }
            
        }

    }


    void checkPlayersReadyState()
    {
        bool everyoneReady = true;
        var playersList = GameData.playersList;
        foreach (var player in playersList)
        {
            if (!player.isReady)
                everyoneReady = false;
        }
        if (everyoneReady && GameData.isHost)
        {
            OkButton.GetComponent<Button>().interactable = true;
        }
        else
        {
            OkButton.GetComponent<Button>().interactable = false;
        }
    }

    void hasSelectedCharacter()
    {
        if (GameData.selectedCharacter != "")
        {
            //removeClickEventOnCharactersGrid();
            CancelButton.SetActive(true);

            if (GameData.isHost)
                ReturnButton.SetActive(false);
        }
        else
        {
            //reenableClickEvents();
            if (GameData.isHost)
                ReturnButton.SetActive(true);
            else CancelButton.SetActive(false);
        }
    }

    async public void startGame()
    {
        var websocket = WebsocketManager.GetComponent<WebsocketManager>().websocket;
        string json = "{'type': 'startGame', 'params':{'code': '" + GameData.joinedRoomCode + "'}}";
        await websocket.SendText(json);
    }
}