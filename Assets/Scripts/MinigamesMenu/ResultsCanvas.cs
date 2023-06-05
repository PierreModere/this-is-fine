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

    public GameObject BattleText;
    public GameObject DuelText;
    public GameObject FirstMinigameText;


    public List<GameObject> winnersGameobjects;

    public List<Sprite> charactersSprites;

    public GameObject Bandeau;
    public GameObject NewsInstruction;
    public GameObject NewsCardName;
    public GameObject NewsCardCase;

    public List<NewsCard> NewsCardsList;
    List<NewsCard> newsCards;
    int currentNewsCardIndex = 0;


    void Start()
    {
        initScreen();
    }

    void initScreen()
    {
        if (newsCards == null) shuffleNewsCards();

        displayNewsCard();

        if ((GameData.isHost && GameData.minigameMode != "Duel") || GameData.isDuelHost)
        {
            okButton.SetActive(true);
        }
        else
        {
            okButton.SetActive(false);
        }

        if (GameData.isFirstMinigame)
        {
            FirstMinigameText.SetActive(true);
            BattleText.SetActive(false);
            DuelText.SetActive(false);
        }
        else
        {
            FirstMinigameText.SetActive(false);

            if (GameData.minigameMode == "Battle")
            {
                BattleText.SetActive(true);
                DuelText.SetActive(false);
            }
            else
            {
                BattleText.SetActive(false);
                DuelText.SetActive(true);
            }
        }
        updateWinnersList();


        if (GameObject.Find("WebsocketManager").GetComponent<WebsocketManager>().receivedMinigameTime > 1)
        {
            GameData.isFirstMinigame = false;
            GameObject.Find("WebsocketManager").GetComponent<WebsocketManager>().changeScreenForEveryone("DashboardCanvas");
        }
    }

    public void updateWinnersList()
    {
        var playersList = GameData.playersList;
        unactiveWinnersGameobjects();

        List<ClientsList> sortedList = playersList.OrderByDescending(x => int.Parse(x.score)).ToList();

        if (GameData.isFirstMinigame)
        {
            GameObject winnerGameobject = winnersGameobjects[2];

            winnerGameobject.SetActive(true);

            GameObject playerCharacter = winnerGameobject.transform.Find("PlayerCharacter").gameObject;
            if (playerCharacter.GetComponent<Image>().sprite == null)
                playerCharacter.GetComponent<Image>().sprite = charactersSprites.Find(spr => spr.name == sortedList[0].selectedCharacter);

            GameObject PlayerNumber = winnerGameobject.transform.Find("PlayerNumber").gameObject;
            PlayerNumber.GetComponent<TextMeshProUGUI>().text = "Joueur " + sortedList[0].id;
        }

        else
        {
            if (GameData.minigameMode == "Battle")
            {
                for (int i = 0; i < 2; i++)
                {
                    GameObject winnerGameobject = winnersGameobjects[i];

                    winnerGameobject.SetActive(true);

                    GameObject playerCharacter = winnerGameobject.transform.Find("PlayerCharacter").gameObject;
                    if (playerCharacter.GetComponent<Image>().sprite == null)
                        playerCharacter.GetComponent<Image>().sprite = charactersSprites.Find(spr => spr.name == sortedList[i].selectedCharacter);

                    GameObject PlayerNumber = winnerGameobject.transform.Find("PlayerNumber").gameObject;
                    PlayerNumber.GetComponent<TextMeshProUGUI>().text = "Joueur " + sortedList[i].id;

                }
            }
            else
            {
                GameObject winnerGameobject = winnersGameobjects[2];

                winnerGameobject.SetActive(true);

                GameObject playerCharacter = winnerGameobject.transform.Find("PlayerCharacter").gameObject;
                if (playerCharacter.GetComponent<Image>().sprite == null)
                    playerCharacter.GetComponent<Image>().sprite = charactersSprites.Find(spr => spr.name == sortedList[0].selectedCharacter);

                GameObject PlayerNumber = winnerGameobject.transform.Find("PlayerNumber").gameObject;
                PlayerNumber.GetComponent<TextMeshProUGUI>().text = "Joueur " + sortedList[0].id;
            }
        }

    }

    void unactiveWinnersGameobjects()
    {
        foreach (GameObject winner in winnersGameobjects)
        {
            winner.SetActive(false);
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
            NewsCardName.GetComponent<Image>().sprite = GetRandomNewsCard().newsTitle;
            NewsCardCase.GetComponent<TextMeshProUGUI>().text = "sur " + GetRandomNewsCard().boardCase;
            Bandeau.SetActive(true);

            Sequence newsAlert = DOTween.Sequence();

            newsAlert.Append(Bandeau.transform.DOLocalMoveX(1080, 0.25f).SetEase(Ease.InOutBounce).From().OnComplete(() => { NewsCardName.SetActive(true); }));
            newsAlert.Append(NewsInstruction.GetComponent<TextMeshProUGUI>().DOFade(0,0.2f).From());
            newsAlert.Append(NewsCardName.transform.DOScale(0.8f,0.35f).SetEase(Ease.OutElastic).From());

        }

    }

    public void hideNewsCard()
    {
        Bandeau.SetActive(false);
        NewsCardName.SetActive(false);

   }

    void shuffleNewsCards()
    {
        System.Random random = new System.Random();

        newsCards = new List<NewsCard>(NewsCardsList);

        int n = newsCards.Count;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            NewsCard value = newsCards[k];
            newsCards[k] = newsCards[n];
            newsCards[n] = value;
        }
    }

    NewsCard GetRandomNewsCard()
    {
        NewsCard sentence = newsCards[currentNewsCardIndex];
        currentNewsCardIndex = (currentNewsCardIndex + 1) % newsCards.Count;
        return sentence;
    }
}
