using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static WebsocketManager;

public class MinigameInstructions : MonoBehaviour
{
    public GameData GameData;

    private GameObject WebsocketManager;

    public GameObject GoButton;
    public GameObject ReadyButton;
    public GameObject minigameMode;
    public GameObject minigameTitle;
    public GameObject minigamePreview;
    public GameObject instructionText;

    public List<Sprite> charactersSprites;
    public List<GameObject> playersGameobjects;
    public List<Minigame> minigamesList;


    // Start is called before the first frame update
    void Start()
    {
        initScreen();
    }

    void OnEnable()
    {
        initScreen();
    }

    void initScreen()
    {
        if ((GameData.isHost && GameData.minigameMode != "Duel") || GameData.isDuelHost)
        {
            GoButton.SetActive(true);
            ReadyButton.SetActive(false);
        }
        else
        {
            ReadyButton.SetActive(true);
            GoButton.SetActive(false);
        }
        if (GameData.displayedMinigameID != "")
        {
            setMinigameData();
            setMinigameMode();
            updatePlayersList();
        }
    }

    public void updatePlayersList()
    {
        var playersList = GameData.playersList;
        unactiveAllPlayersGameobjects();
        checkPlayersReadyState();

        for (int i = 0; i < playersList.Count; i++)
        {

            if (playersList[i].selectedCharacter != "")
            {
                GameObject playerGameObject = playersGameobjects.Find(g => g.name == "Player" + playersList[i].id);

                if (GameData.minigameMode == "Duel" && playersList[i].isDuel == true)
                    playerGameObject.SetActive(true);
                else if (GameData.minigameMode == "Battle")
                    playerGameObject.SetActive(true);

                switch (playersList[i].id.ToString())
                {
                    case "1":
                        playerGameObject.GetComponent<Image>().color = new Color32(255, 0, 0, 255);
                        break;
                    case "2":
                        playerGameObject.GetComponent<Image>().color = new Color32(0, 0, 255, 255);
                        break;
                    case "3":
                        playerGameObject.GetComponent<Image>().color = new Color32(0, 255, 0, 255);
                        break;
                    case "4":
                        playerGameObject.GetComponent<Image>().color = new Color32(255, 255, 0, 255);
                        break;
                }

                GameObject playerCharacter = playerGameObject.transform.Find("PlayerCharacter").gameObject;
                playerCharacter.GetComponent<Image>().sprite = charactersSprites.Find(spr => spr.name == playersList[i].selectedCharacter);

                if (playersList[i].isReady)
                {
                    GameObject isReadyIcon = playerGameObject.transform.Find("isReady").gameObject;
                    isReadyIcon.SetActive(true);
                    isReadyIcon.transform.DOScale(1.05f, 0.1f).OnComplete(() => { isReadyIcon.transform.DOScale(1f, 0.1f); });
                }
            }
        }
    }

    public void setMinigameMode()
    {
        TextMeshProUGUI minigameModeText = minigameMode.GetComponent<TextMeshProUGUI>();
        if (GameData.minigameMode == "Duel")
        {
            minigameModeText.text = "Duel";
        }
        else if (GameData.minigameMode == "Battle")
        {
            minigameModeText.text = "Bataille";
        }

    }

    public void setMinigameData()
    {
        Minigame displayedMinigame = minigamesList.Find(mg => mg.id == GameData.displayedMinigameID);

        if (displayedMinigame != null)
        {
            minigameTitle.GetComponent<TextMeshProUGUI>().text = displayedMinigame.title;
            instructionText.GetComponent<TextMeshProUGUI>().text = displayedMinigame.instruction;
            minigamePreview.GetComponent<Image>().sprite = displayedMinigame.preview;
        }
    }

    void checkPlayersReadyState()
    {
        bool everyoneReady = true;

        List<ClientsList> playersList;

        if (GameData.minigameMode == "Duel")
            playersList = GameData.playersList.FindAll(player => player.isDuel);

        else playersList = GameData.playersList;

        foreach (var player in playersList)
        {
            if (!player.isReady && player.id != int.Parse(GameData.playerID))
                everyoneReady = false;
        }
        if ((everyoneReady && GameData.isHost && GameData.minigameMode != "Duel") || (everyoneReady && GameData.isDuelHost))
        {
            GoButton.GetComponent<Button>().interactable = true;
        }
        else
        {
            GoButton.GetComponent<Button>().interactable = false;
        }
    }

    void unactiveAllPlayersGameobjects()
    {
        foreach (GameObject player in playersGameobjects)
        {
            player.SetActive(false);
        }
    }

    public async void sendReady()
    {
        WebsocketManager = GameObject.Find("WebsocketManager");

        var websocket = WebsocketManager.GetComponent<WebsocketManager>().websocket;
        string json = "{'type': 'playerIsReady', 'params':{'code': '" + GameData.joinedRoomCode + "','id':'" + GameData.playerID + "'}}";
        await websocket.SendText(json);
        ReadyButton.GetComponent<Button>().interactable = false;
    }

    public async void lauchMinigame()
    {
        WebsocketManager = GameObject.Find("WebsocketManager");

        var websocket = WebsocketManager.GetComponent<WebsocketManager>().websocket;
        string sceneName = "Minigame" + GameData.displayedMinigameID;
        string json = "{'type': 'changeScene', 'params':{'code': '" + GameData.joinedRoomCode + "','sceneName':'" + sceneName + "'}}";
        await websocket.SendText(json);
    }
}
