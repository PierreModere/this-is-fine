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
    public GameObject NewAlertBackground;

    public List<Sprite> NewsAlertSprite;

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

        if (GameObject.Find("WebsocketManager").GetComponent<WebsocketManager>().receivedMinigameTime > 1)
        {
            GameData.isFirstMinigame = false;
            GameObject.Find("WebsocketManager").GetComponent<WebsocketManager>().changeScreenForEveryone("DashboardCanvas");
        }
        updateWinnersList();
    }

    void animPlayerSlots()
    {
        Sequence sequence = DOTween.Sequence();
        for (int i = 0; i < playerGameobjects.Count; ++i)
        {
            sequence.Append(playerGameobjects[i].transform.DOScale(1.05f,0.25f).SetEase(Ease.InOutBack).From());
        }
        sequence.OnComplete(() =>
        {
            if (GameData.minigameMode == "Battle") displayNewsCard();
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

            if (GameData.isFirstMinigame)
            {
                startToPlayText.SetActive(true);
                startToPlayText.transform.DOLocalMoveY(60, 0.2f).From();
                playersNumberToShow = 1;
            }
            else
            {
                startToPlayText.SetActive(false);
                if (GameData.minigameMode == "Battle") playersNumberToShow = sortedList.Count;
                else playersNumberToShow = 2;
            }

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
            animPlayerSlots();
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
            newsShow.Append(NewAlertBackground.GetComponent<Image>().DOFade(0.6f, 0).SetDelay(1).OnComplete(() =>
            {
                NewsAlertGameObject.SetActive(true);
                NewAlertBackground.SetActive(true);
            }));
            newsShow.Append(NewAlertBackground.GetComponent<Image>().DOFade(0f, 0.2f).From());
            newsShow.Join(NewsAlertGameObject.transform.DOScale(0.5f, 0.25f).SetEase(Ease.InOutBack).From());
        }
    }

    public void hideNewsCard()
    {
        Sequence newsHide = DOTween.Sequence();
        newsHide.Append(NewsAlertGameObject.transform.DOScale(0, 0.15f).SetEase(Ease.InBack));
        newsHide.Join(NewAlertBackground.GetComponent<Image>().DOFade(0f, 0.2f));
        newsHide.Join(NewsAlertGameObject.GetComponent<Image>().DOFade(0f, 0.15f)).OnComplete(() => {
            NewAlertBackground.SetActive(false);
            NewsAlertGameObject.SetActive(false);
            okButton.GetComponent<Button>().interactable = true;
        });
    }

}
