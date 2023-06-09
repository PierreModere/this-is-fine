using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;


using static WebsocketManager;

public class ResultsCanvas : MonoBehaviour
{
    public GameData GameData;
    public GameObject okButton;

    public GameObject startToPlayText;


    public List<GameObject> playerGameobjects;

    public List<Sprite> charactersSprites;

    public GameObject NewsAlertGameObject;
    public GameObject FadeBackground;

    public List<Sprite> NewsAlertSprite;

    public GameObject FirstPlayerToPlayAlert;

    void Start()
    {
        initScreen();
    }

    void initScreen()
    {

        if ((GameData.isHost && GameData.minigameMode != "Duel") || GameData.isDuelHost)
        {
            okButton.SetActive(true);
            okButton.GetComponent<Button>().interactable = false;
        }
        else
        {
            okButton.SetActive(false);
        }

        if (GameObject.Find("WebsocketManager").GetComponent<WebsocketManager>().receivedMinigameTime > 2)
        {
            GameData.isFirstMinigame = false;
            // GameObject.Find("WebsocketManager").GetComponent<WebsocketManager>().changeScreenForEveryone("DashboardCanvas");
        }
        updateWinnersList();
    }

    void animPlayerSlots(int firstPlayerID)
    {
        Sequence sequence = DOTween.Sequence();
        for (int i = 0; i < playerGameobjects.Count; ++i)
        {
            sequence.Append(playerGameobjects[i].transform.DOScale(1.2f, 0.25f).SetEase(Ease.InOutBack).SetDelay(0.15f).From());
        }
        sequence.OnComplete(() => {
            if (GameData.minigameMode == "Battle")
            {
                if (GameData.isFirstMinigame) displayFirstPlayerToPlayer(firstPlayerID);
                else displayNewsCard();
            }

        });
    }
    public void updateWinnersList()
    {
        var playersList = GameData.playersList;
        unactiveWinnersGameobjects();

        List<ClientsList> sortedList = playersList.OrderByDescending(x => int.Parse(x.score)).ToList();


        if (sortedList.Count > 0)
        {
            int playersNumberToShow;
            if (GameData.minigameMode == "Battle") playersNumberToShow = sortedList.Count;
            else playersNumberToShow = 2;
            

            for (int i = 0; i < playersNumberToShow; i++)
            {
                GameObject playerGameobject = playerGameobjects[i];

                playerGameobject.SetActive(true);

                GameObject playerFace = playerGameobject.transform.Find("PlayerFace").gameObject;
                if (playerFace.GetComponent<Image>().sprite == null)
                    if (i == 0) playerFace.GetComponent<Image>().sprite = charactersSprites.Find(spr => spr.name == "ui-score-"+sortedList[i].selectedCharacter+"_win");
                    else playerFace.GetComponent<Image>().sprite = charactersSprites.Find(spr => spr.name == "ui-score-" + sortedList[i].selectedCharacter + "_last");
                GameObject PlayerName = playerGameobject.transform.Find("PlayerName").gameObject;
                PlayerName.GetComponent<TextMeshProUGUI>().text = "Joueur " + sortedList[i].id;

                GameObject playerReward = playerGameobject.transform.Find("Reward").gameObject;

                if (GameData.isFirstMinigame)
                {
                    playerReward.SetActive(false);
                }
                else
                {

                    if (GameData.minigameMode == "Battle")
                    {
                        playerReward.SetActive(true);
                    }
                    else
                    {
                        playerReward.SetActive(false);
                    }
                }

            }
            animPlayerSlots(sortedList[0].id);
        }


    }

    void unactiveWinnersGameobjects()
    {
        foreach (GameObject player in playerGameobjects)
        {
            player.SetActive(false);
        }
    }

    public void returnToDashboard()
    {
        GameObject.Find("WebsocketManager").GetComponent<WebsocketManager>().returnToDashboard();
    }

    void displayFirstPlayerToPlayer(int firstPlayerID)
    {
        string color = "";
        switch (firstPlayerID)
        {
            case 1:
                color = "ea4645";
                break;
            case 2:
                color = "8acF72";
                break;

            case 3:
                color = "7ebbe9";
                break;

            case 4:
                color = "a079ac";
                break;
        }
        string text = "Le <color=#"+color+">Joueur "+ firstPlayerID.ToString() +"</color> lance\r\nle dé en premier";
        FirstPlayerToPlayAlert.transform.Find("Text").gameObject.GetComponent<TextMeshProUGUI>().text = text;

        Sequence popUp = DOTween.Sequence();

        FirstPlayerToPlayAlert.SetActive(true);
        FadeBackground.SetActive(true);
        popUp.Append(FadeBackground.GetComponent<Image>().DOFade(0.6f, 0.2f));
        popUp.Join(FirstPlayerToPlayAlert.transform.DOScale(0.5f, 0.25f).SetEase(Ease.InOutBack).From());
    }

    public void hideFirstPlayer()
    {
        Sequence firstPlayerHide = DOTween.Sequence();
        firstPlayerHide.Append(FirstPlayerToPlayAlert.transform.DOScale(0, 0.15f).SetEase(Ease.InBack));
        firstPlayerHide.Join(FadeBackground.GetComponent<Image>().DOFade(0f, 0.2f));
        firstPlayerHide.Join(FirstPlayerToPlayAlert.GetComponent<Image>().DOFade(0f, 0.15f)).OnComplete(() => {
            FadeBackground.SetActive(false);
            FirstPlayerToPlayAlert.SetActive(false);
            displayNewsCard();
        });
    }

    public void displayNewsCard()
    {
        if (GameData.isHost)
        {
            if (GameData.isFirstMinigame)
            {
                NewsAlertGameObject.GetComponent<Image>().sprite = NewsAlertSprite[0];
            }
            else NewsAlertGameObject.GetComponent<Image>().sprite = NewsAlertSprite[1];

            Sequence newsShow = DOTween.Sequence();
            newsShow.Append(FadeBackground.GetComponent<Image>().DOFade(0.6f, 0).SetDelay(0.6f).OnComplete(() =>
            {
                NewsAlertGameObject.SetActive(true);
                FadeBackground.SetActive(true);
            }));
            newsShow.Append(FadeBackground.GetComponent<Image>().DOFade(0.6f, 0.2f));
            newsShow.Join(NewsAlertGameObject.transform.DOScale(0.5f, 0.25f).SetEase(Ease.InOutBack).From());
        }
    }

    public void hideNewsCard()
    {
        Sequence newsHide = DOTween.Sequence();
        newsHide.Append(NewsAlertGameObject.transform.DOScale(0, 0.15f).SetEase(Ease.InBack));
        newsHide.Join(FadeBackground.GetComponent<Image>().DOFade(0f, 0.2f));
        newsHide.Join(NewsAlertGameObject.GetComponent<Image>().DOFade(0f, 0.15f)).OnComplete(() => {
            FadeBackground.SetActive(false);
            NewsAlertGameObject.SetActive(false);
            okButton.GetComponent<Button>().interactable = true;
        });
    }

}
