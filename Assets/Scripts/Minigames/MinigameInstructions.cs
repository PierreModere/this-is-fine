using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;
using static WebsocketManager;

public class MinigameInstructions : MonoBehaviour
{
    public GameObject GoButton;
    public GameObject ReadyButton;
    public GameObject minigameMode;
    public GameObject minigameTitle;
    public GameObject minigamePreview;
    public GameObject instructionText;
    public List<Sprite> charactersSprites;
    public List<GameObject> playersGameobjects;

    private GameObject WebsocketManager;
    public List<Minigame> minigamesList;
    string displayedMinigameID;

    bool isHost;
    bool isDuelHost;

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
        WebsocketManager = GameObject.Find("WebsocketManager");
        isHost = GameObject.Find("WebsocketManager").GetComponent<WebsocketManager>().isHost;
        isDuelHost = GameObject.Find("WebsocketManager").GetComponent<WebsocketManager>().isDuelHost;

        displayedMinigameID = WebsocketManager.GetComponent<WebsocketManager>().displayedMinigameID;

        if (isHost || isDuelHost)
        {
            GoButton.SetActive(true);
            ReadyButton.SetActive(false);
        }
        else
        {
            ReadyButton.SetActive(true);
            GoButton.SetActive(false);
        }
        if (displayedMinigameID != "") setMinigameData();

        setMinigameMode();
        updatePlayersList();
    }

    public void updatePlayersList()
    {
        var playersList = WebsocketManager.GetComponent<WebsocketManager>().playersList;
        unactiveAllPlayersGameobjects();
        checkPlayersReadyState();

        for (int i = 0; i < playersList.Count; i++)
        {

            if (playersList[i].selectedCharacter != "")
            {
                GameObject playerGameObject = playersGameobjects.Find(g => g.name == "Player"+playersList[i].id);

                if (WebsocketManager.GetComponent<WebsocketManager>().minigameMode=="Duel" && playersList[i].isDuel==true) 
                    playerGameObject.SetActive(true);   
                else if (WebsocketManager.GetComponent<WebsocketManager>().minigameMode == "Battle")
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
        var mode = WebsocketManager.GetComponent<WebsocketManager>().minigameMode;
        TextMeshProUGUI minigameModeText = minigameMode.GetComponent<TextMeshProUGUI>();
        if (mode == "Duel")
        {
            minigameModeText.text = "Duel";
        }
        else if (mode == "Battle")
        {
            minigameModeText.text = "Bataille";
        }
      
    }

    public void setMinigameData() {
        Minigame displayedMinigame = minigamesList.Find(mg => mg.id == displayedMinigameID);
        minigameTitle.GetComponent<TextMeshProUGUI>().text = displayedMinigame.title;
        instructionText.GetComponent<TextMeshProUGUI>().text = displayedMinigame.instruction;
        minigamePreview.GetComponent<Image>().sprite = displayedMinigame.preview;
    }

    void checkPlayersReadyState()
    {
        bool everyoneReady = true;

        List<ClientsList> playersList;

        if (WebsocketManager.GetComponent<WebsocketManager>().minigameMode == "Duel")
            playersList = WebsocketManager.GetComponent<WebsocketManager>().playersList.FindAll(player => player.isDuel);
       
        else playersList = WebsocketManager.GetComponent<WebsocketManager>().playersList;

        foreach (var player in playersList)
        {
            if (!player.isReady && player.id!= int.Parse(WebsocketManager.GetComponent<WebsocketManager>().playerID))
                everyoneReady = false;
        }
        if (everyoneReady && isHost || (everyoneReady && isDuelHost))
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
        var websocket = WebsocketManager.GetComponent<WebsocketManager>().websocket;
        string json = "{'type': 'playerIsReady', 'params':{'code': '" + WebsocketManager.GetComponent<WebsocketManager>().joinedRoomCode + "','id':'" + WebsocketManager.GetComponent<WebsocketManager>().playerID + "'}}";
        await websocket.SendText(json);
        ReadyButton.GetComponent<Button>().interactable = false;
    }

    public async void lauchMinigame()
    {
        var websocket = WebsocketManager.GetComponent<WebsocketManager>().websocket;
        string sceneName = "Minigame"+ WebsocketManager.GetComponent<WebsocketManager>().displayedMinigameID;
        string json = "{'type': 'changeScene', 'params':{'code': '" + WebsocketManager.GetComponent<WebsocketManager>().joinedRoomCode + "','sceneName':'" + sceneName + "'}}";
        await websocket.SendText(json);
    }
}
