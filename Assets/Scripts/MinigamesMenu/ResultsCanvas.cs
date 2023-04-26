using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

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


    void Start()
    {
        initScreen();
    }

    void initScreen()
    {
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
}
