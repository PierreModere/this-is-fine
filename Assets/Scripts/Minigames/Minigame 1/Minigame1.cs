using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Minigame1 : MonoBehaviour
{
    int playerProgressIndex;
    int[] indexesSuite = new int[5];

    public GameObject valve;
    public GameObject buttonInstruction;
    public GameObject controls;
    public GameObject redAlert;

    public List<Sprite> buttonsInstructionsSprites;
    public List<GameObject> buttonsGameobjects;

    public GameObject FeedbackTextPrefab;

    public MinigameUI MinigameUI;
    public GameData GameData;
    private GameObject WebsocketManager;

    public bool isMultiPressed=false;
    Vector3 valvePos;

    public void initMinigame()
    {
        WebsocketManager = GameObject.Find("WebsocketManager");

        valvePos = valve.transform.position;


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
        if (indexesSuite[playerProgressIndex] == index && !isMultiPressed)
        {
            valve.transform.DORotate(new Vector3(0, 0, valve.transform.rotation.eulerAngles.z - 45f), 0.2f);
            playerProgressIndex++;
            sendScore();
            MinigameUI.displayFeedback(true);
            if (playerProgressIndex < indexesSuite.Length)
                updateInstruction();
            else endMinigame();

        }
        else if (indexesSuite[playerProgressIndex] != index && !isMultiPressed)
        {
            MinigameUI.displayFeedback(false);

            valve.transform.DOShakePosition(0.3f, 20f).OnComplete(() => {
                valve.transform.DOMove(valvePos, 0.1f).SetAutoKill(false);
            });

            redAlert.GetComponent<Image>().DOFade(0.25f,0.1f).OnComplete(() => {
                redAlert.GetComponent<Image>().DOFade(0, 0.2f);
            });

            foreach (Transform child in GameObject.Find("BackgroundCanvas").transform)
            {
                Vector3 defaultPos = child.position;
                child.DOShakePosition(0.3f, 15f).OnComplete(()=> {
                    child.DOMove(defaultPos, 0.1f);
                });
            }
        }
    }

    void activeButtons()
    {
        controls.transform.DOLocalMoveY(-1348, 0f);
        controls.transform.DOLocalMoveY(-585, 0.4f).OnComplete(() =>
        {
            foreach (GameObject button in buttonsGameobjects)
            {
                button.GetComponent<Button>().interactable = true;
            }
        });
    }

    void endMinigame()
    {
        MinigameUI.endMinigame();
    }

    public void finishMinigame()
    {
        foreach (GameObject button in buttonsGameobjects)
        {
            button.GetComponent<Button>().interactable = false;
        }
        buttonInstruction.GetComponent<Image>().DOFade(0f, 0.2f);
    }

    void sendScore()
    {
        if (GameData.joinedRoomCode!="" && GameData.joinedRoomCode!=null)
            WebsocketManager.GetComponent<WebsocketManager>().sendScore(playerProgressIndex);
    }

     private void Update()
    {
        if (Input.touchCount > 1)
        {
            isMultiPressed = true;
        }
        else isMultiPressed = false;
    }
}
