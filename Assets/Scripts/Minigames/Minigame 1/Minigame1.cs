using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SocialPlatforms.Impl;

public class Minigame1 : MonoBehaviour
{
    int playerProgressIndex;
    int[] indexesSuite = new int[25];

    public GameObject valve;
    public GameObject buttonInstruction;
    public GameObject controls;

    public List<Sprite> buttonsInstructionsSprites;
    public List<GameObject> buttonsGameobjects;

    public MinigameUI MinigameUI;
    private GameObject WebsocketManager;

    public void initMinigame()
    {
        WebsocketManager = GameObject.Find("WebsocketManager");

        playerProgressIndex = 0;
        for (int i = 0; i < indexesSuite.Length; i++)
        {
            var randomIndex = Random.Range(0, buttonsInstructionsSprites.Count);
            indexesSuite[i] = randomIndex;
        }
        activeButtons();
        updateInstruction();
        buttonInstruction.GetComponent<Image>().DOFade(1f, 0.1f);
    }
    void updateInstruction()
    {
        buttonInstruction.GetComponent<Image>().sprite = buttonsInstructionsSprites[indexesSuite[playerProgressIndex]];
        buttonInstruction.transform.DOScale(1.5f, 0.15f).OnComplete(() => { buttonInstruction.transform.DOScale(1f, 0.2f); });
    }
    public void checkButton(int index)
    {
        if (indexesSuite[playerProgressIndex] == index)
        {
            valve.transform.DORotate(new Vector3(0, 0, valve.transform.rotation.eulerAngles.z -45f), 0.2f);
            playerProgressIndex++;
            sendScore();
            if (playerProgressIndex < indexesSuite.Length)
                updateInstruction();
            else endMinigame();

        }
    }

    void activeButtons()
    {
        controls.transform.DOLocalMoveY(-1348, 0f);
        controls.transform.DOLocalMoveY(-585, 0.4f).OnComplete(() => {
            foreach (GameObject button in buttonsGameobjects)
            {
                button.GetComponent<Button>().interactable = true;
            }
        });
        }

    void endMinigame()
    {
        foreach (GameObject button in buttonsGameobjects)
        {
            button.GetComponent<Button>().interactable = false;
        }
        buttonInstruction.GetComponent<Image>().DOFade(0f, 0.2f);
        MinigameUI.endMinigame();
    }
    void sendScore()
    {
        WebsocketManager.GetComponent<WebsocketManager>().sendScore(playerProgressIndex);
    }
}
