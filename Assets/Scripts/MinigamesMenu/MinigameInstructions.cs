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
    //public GameObject minigameMode;
    public GameObject minigameTitle;
    public GameObject minigamePreview;
    public GameObject minigamePreviewAll;
    public GameObject instructionText;

    public List<Minigame> minigamesList;
    public List<GameObject> playersGameobjects;
    public List<Sprite> charactersSprites;
    public List<Sprite> playerBgSprites;
    public List<Sprite> playerReadySprites;


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
            //setMinigameMode();
            unactiveAllPlayersGameobjects();
           if (GameData.playersList != null && GameData.playersList.Count > 0) updatePlayersList();
        }
        Sequence popUpPreview = DOTween.Sequence();
        popUpPreview.Append(minigamePreviewAll.transform.DOScale(1.3f, 0));
        popUpPreview.Join(instructionText.GetComponent<TextMeshProUGUI>().DOFade(0, 0));
        popUpPreview.Append(minigamePreviewAll.transform.DOScale(1f, 0.25f).SetEase(Ease.InOutBack));
        popUpPreview.Append(instructionText.GetComponent<TextMeshProUGUI>().DOFade(1,0.4f));

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

                GameObject playerCharacter = playerGameObject.transform.Find("PlayerCharacter").gameObject;
                playerCharacter.GetComponent<Image>().sprite = charactersSprites.Find(spr => spr.name == playersList[i].selectedCharacter+ "_score_first");

                if (GameData.playerID == playersList[i].id.ToString())
                {
                    playerGameObject.transform.Find("isReady").GetComponent<Image>().sprite = playerReadySprites.Find(spr => spr.name == "player" + GameData.playerID + "Ready");
                    playerGameObject.GetComponent<Image>().sprite = playerBgSprites.Find(spr => spr.name == "player" + GameData.playerID + "Background");
                    playerGameObject.transform.Find("PlayerNumber").gameObject.GetComponent<TextMeshProUGUI>().color = new Color(255, 255, 255, 255);
                }

                if (playersList[i].isReady && !playerGameObject.transform.Find("isReady").gameObject.activeInHierarchy)
                {
                    GameObject isReadyIcon = playerGameObject.transform.Find("isReady").gameObject;
                    isReadyIcon.SetActive(true);
                    isReadyIcon.transform.DOScale(1.05f, 0.1f).OnComplete(() => { isReadyIcon.transform.DOScale(1f, 0.1f); });
                }
            }
        }
    }

/*    public void setMinigameMode()
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

    }*/

    public void setMinigameData()
    {
        Minigame displayedMinigame = minigamesList.Find(mg => mg.id == GameData.displayedMinigameID);

        if (displayedMinigame != null)
        {
            minigameTitle.GetComponent<TextMeshProUGUI>().text = displayedMinigame.title;
            instructionText.GetComponent<TextMeshProUGUI>().text = displayedMinigame.instruction;
            minigamePreview.GetComponent<Animator>().Play("minigame"+displayedMinigame.id);
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
            player.transform.Find("isReady").gameObject.SetActive(false);
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
