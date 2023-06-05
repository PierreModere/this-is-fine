using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DashboardCanvas : MonoBehaviour
{
    public GameData GameData;

    public GameObject BattleButton;
    public GameObject DuelButton;
    public GameObject EndGameButton;

    public GameObject CancelButton;
    public GameObject OkButton;

    public GameObject DashboardSubtitle;




    private GameObject WebsocketManager;

    bool isHost;
    bool isSelected;
    GameObject selectedButton;
    string selectedMode;

    void Start()
    {

        if (GameData.isHost)
        {

            BattleButton.GetComponent<Button>().interactable = true;
            EndGameButton.GetComponent<Button>().interactable = true;

            BattleButton.GetComponent<Button>().onClick.AddListener(() => { buttonOnClick("Battle", BattleButton); });
            EndGameButton.GetComponent<Button>().onClick.AddListener(() => { buttonOnClick("EndGame", EndGameButton); });
        }

        DuelButton.GetComponent<Button>().onClick.AddListener(() => { buttonOnClick("Duel", DuelButton); });

    }

    private void OnEnable()
    {
        toggleBottomButtons();
        updateSubtitle();


    }

    void toggleBottomButtons()
    {
        CancelButton.GetComponent<Button>().interactable = isSelected;
        OkButton.GetComponent<Button>().interactable = isSelected;
    }

    public void buttonOnClick(string mode, GameObject button)
    {
        if (selectedButton != null) selectedButton.transform.Find("Selected").gameObject.SetActive(false);
        isSelected = true;
        selectedMode = mode;
        selectedButton = button;
        selectedButton.transform.Find("Selected").gameObject.SetActive(isSelected);
        selectedButton.transform.DOScale(1.05f, 0.1f).OnComplete(() => { selectedButton.transform.DOScale(1f, 0.1f); });

        updateSubtitle();
        toggleBottomButtons();
    }

    public void cancelSelection()
    {
        isSelected = false;
        selectedButton.transform.Find("Selected").gameObject.SetActive(isSelected);
        selectedButton = null;
        selectedMode = null;
        updateSubtitle();
        toggleBottomButtons();
    }

    public void confirmSelection()
    {
        if (selectedMode != null && selectedMode != "")
        {
            WebsocketManager = GameObject.Find("WebsocketManager");

            if (selectedMode == "Duel") WebsocketManager.GetComponent<WebsocketManager>().sendMinigameMode(selectedMode, GameData.playerID);
            else WebsocketManager.GetComponent<WebsocketManager>().sendMinigameMode(selectedMode, null);
            isSelected = false;
            selectedButton.transform.Find("Selected").gameObject.SetActive(isSelected);
            selectedButton = null;
            selectedMode = null;
        }
    }

    void updateSubtitle()
    {
        TextMeshProUGUI subtitleText = DashboardSubtitle.GetComponent<TextMeshProUGUI>();

        switch (selectedMode)
        {
            case "Battle":
                subtitleText.text = "C’est l’heure de la bataille ?";
                break;
            case "Duel":
                subtitleText.text = "Tu souhaites provoquer un duel ?";
                break;
            case "EndGame":
                subtitleText.text = "";
                break;
            case null:
                subtitleText.text = "";
                break;
        }
    }

}
