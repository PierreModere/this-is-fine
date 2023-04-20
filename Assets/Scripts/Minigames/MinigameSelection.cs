using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class MinigameSelection : MonoBehaviour
{
    public GameObject ReturnButton;
    public GameObject OkButton;

    public List<GameObject> minigameButtons;

    private GameObject WebsocketManager;

    bool isHost;
    bool isSelected;
    GameObject selectedMinigame;
    string selectedMinigameID;

    void Start()
    {
        WebsocketManager = GameObject.Find("WebsocketManager");
        isHost = WebsocketManager.GetComponent<WebsocketManager>().isHost;

        if (isHost)
        {
            foreach (GameObject minigameButton in minigameButtons)
            {
                var button = minigameButton.GetComponent<Button>();
                string minigameID = minigameButton.transform.Find("MinigameID").transform.GetComponent<Text>().text;
                button.onClick.AddListener(() => { selectMinigame(minigameID, minigameButton); });
            }
  

        }
    }

    void toggleOkButton()
    {
        OkButton.GetComponent<Button>().interactable = isSelected;
    }

    public void selectMinigame(string id,GameObject button)
    {
        if (selectedMinigame != null) selectedMinigame.transform.Find("Selected").gameObject.SetActive(false);
        isSelected = true;
        selectedMinigameID = id;
        selectedMinigame = button;
        selectedMinigame.transform.Find("Selected").gameObject.SetActive(isSelected);
        selectedMinigame.transform.DOScale(1.05f, 0.1f).OnComplete(() => { selectedMinigame.transform.DOScale(1f, 0.1f); });

        toggleOkButton();
    }

    public void cancelSelection()
    {
        isSelected = false;
        selectedMinigame.transform.Find("Selected").gameObject.SetActive(isSelected);
        selectedMinigame = null;
        selectedMinigameID = null;
        toggleOkButton();
    }

    public void confirmSelection()
    {
        if (selectedMinigameID != null && selectedMinigameID != "")
        {
            WebsocketManager.GetComponent<WebsocketManager>().sendSelectedMinigame(selectedMinigameID);
            cancelSelection();
        }
    }


}
