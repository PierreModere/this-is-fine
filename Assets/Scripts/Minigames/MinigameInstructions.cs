using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static WebsocketManager;

public class MinigameInstructions : MonoBehaviour
{
    public GameObject GoButton;
    public GameObject ReadyButton;
    public GameObject minigameMode;
    public GameObject minigameTitle;
    public GameObject instructionText;

    bool isDuel = false;

    private GameObject WebsocketManager;
    public List<Minigame> minigamesList;
    string displayedMinigameID;

    // Start is called before the first frame update
    void Start()
    {
        WebsocketManager = GameObject.Find("WebsocketManager");
        bool isHost = WebsocketManager.GetComponent<WebsocketManager>().isHost;
        displayedMinigameID = WebsocketManager.GetComponent<WebsocketManager>().displayedMinigameID;
        if (isHost)
        {
            GoButton.SetActive(true);
            ReadyButton.SetActive(false);
        }
        else
        {
            ReadyButton.SetActive(true);
            GoButton.SetActive(false);
        }
        if (displayedMinigameID!= "") setMinigameData();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updatePlayersList()
    {
        var playersList = WebsocketManager.GetComponent<WebsocketManager>().playersList;
        checkPlayersReadyState();

    }

    public void setMinigameMode()
    {
        TextMeshProUGUI minigameModeText = minigameMode.GetComponent<TextMeshProUGUI>();
        if (isDuel) minigameModeText.text = "Duel";
        else minigameModeText.text = "Bataille";
    }

    void setMinigameData() {
        Minigame displayedMinigame = minigamesList.Find(mg => mg.id == displayedMinigameID);
        minigameTitle.GetComponent<TextMeshProUGUI>().text = displayedMinigame.title;
        instructionText.GetComponent<TextMeshProUGUI>().text = displayedMinigame.instruction;
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
        {
            GoButton.GetComponent<Button>().interactable = true;
        }
        else
        {
            GoButton.GetComponent<Button>().interactable = false;
        }
    }
}
