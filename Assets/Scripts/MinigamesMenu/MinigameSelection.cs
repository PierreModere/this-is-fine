using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinigameSelection : MonoBehaviour
{
    public GameData GameData;

    public GameObject ReturnButton;
    public GameObject OkButton;

    public GameObject Title;
    public GameObject Instruction;

    public Transform minigameButtonsContainer;

    private GameObject WebsocketManager;

    bool isHost;
    bool isSelected;
    GameObject selectedMinigame;
    string selectedMinigameID;

    void Start()
    {
        addClickEvents();
    }

    private void OnEnable()
    {
        titleApperance();
    }

    void addClickEvents()
    {

        for (int i = 0; i < minigameButtonsContainer.childCount; i++)
        {
            GameObject minigameButton = minigameButtonsContainer.GetChild(i).gameObject;

             if (minigameButton.name != "Locked")
             {
                var button = minigameButton.GetComponent<Button>();
                string minigameID = minigameButton.transform.Find("MinigameID").gameObject.GetComponent<Text>().text;
                button.onClick.AddListener(() => { selectMinigame(minigameID, minigameButton.gameObject); });
             }
        }



    }

    void titleApperance()
    {
        Sequence showTitle = DOTween.Sequence();

        showTitle.AppendCallback(() => { Title.SetActive(true); });
        showTitle.Append(Title.transform.DOLocalMoveY(200, 0.2f).SetEase(Ease.OutBack).From());
        showTitle.AppendInterval(1.3f);
        showTitle.AppendCallback(() => { Instruction.SetActive(true);});
        showTitle.Append(Instruction.transform.DOScale(0.8f, 0.2f).SetEase(Ease.OutBack).From());
        showTitle.AppendCallback(() => { Title.SetActive(false); });

    }

    void toggleOkButton()
    {
        OkButton.GetComponent<Button>().interactable = isSelected;
    }

    public void selectMinigame(string id, GameObject button)
    {
        if (selectedMinigame != null) selectedMinigame.transform.Find("Selected").gameObject.SetActive(false);
        isSelected = true;
        selectedMinigameID = id;
        selectedMinigame = button;
        selectedMinigame.transform.Find("Selected").gameObject.SetActive(isSelected);
        selectedMinigame.transform.Find("Selected").DOScale(1.20f, 0.2f).SetEase(Ease.OutBack).From();

        toggleOkButton();
    }

    public void cancelSelection()
    {
        WebsocketManager = GameObject.Find("WebsocketManager");

        isSelected = false;
        if (selectedMinigame != null) selectedMinigame.transform.Find("Selected").gameObject.SetActive(isSelected);
        selectedMinigame = null;
        selectedMinigameID = null;
        WebsocketManager.GetComponent<WebsocketManager>().resetDuelStatus();
        GameData.isDuelHost = false;
        Instruction.SetActive(true);    
        toggleOkButton();
    }

    public void confirmSelection()
    {
        if (selectedMinigameID != null && selectedMinigameID != "")
        {
            WebsocketManager = GameObject.Find("WebsocketManager");

            WebsocketManager.GetComponent<WebsocketManager>().sendSelectedMinigame(selectedMinigameID);
            toggleOkButton();

            isSelected = false;
            selectedMinigame.SetActive(isSelected);
            selectedMinigame = null;
            selectedMinigameID = null;
        }
    }
}
