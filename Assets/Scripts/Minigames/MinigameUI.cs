using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class MinigameUI : MonoBehaviour
{
    // Start is called before the first frame update
    public GameData GameData;
    public Minigame minigameData;
    public GameObject minigameLogic;
    public bool isPlaying;

    public GameObject timerGameobject;
    public TextMeshProUGUI TimerUI;
    float timeLeft;
    bool isCutscene = true;
    float cutsceneTimeLeft;

    public GameObject GoText;
    public GameObject FinishText;

    public List<GameObject> playersGameobjects;
    public List<Sprite> charactersSprites;

    private GameObject WebsocketManager;

    void Start()
    {
        cutsceneTimeLeft = minigameData.cutsceneTime;
        timeLeft = minigameData.gameTime;
        TimerUI.text = timeLeft.ToString();
        gameObject.GetComponent<CanvasGroup>().alpha = 0f;
    }

    private void OnEnable()
    {
        if (GameData.playersList != null) updatePlayersListAndScore();
        gameObject.GetComponent<CanvasGroup>().alpha = 0f;
        if (!minigameData.hasTimer) timerGameobject.SetActive(false);

    }

    void Update()
    {
        if (isCutscene)
        {
            if (cutsceneTimeLeft > 0)
            {
                cutsceneTimeLeft -= Time.deltaTime;
            }
            else
            {
                cutsceneTimeLeft = 0;
                isCutscene = false;
                gameObject.GetComponent<CanvasGroup>().DOFade(1f, 0.1f).OnComplete(() => {
                    popUpAnimationText(GoText);
                });

            }
        }
        if (isPlaying && minigameData.hasTimer)
        {
            if (timeLeft > 0)
            {
                timeLeft -= Time.deltaTime;
                updateTimer(timeLeft);
            }
            else
            {
                timeLeft = 0;
                isPlaying = false;
                endMinigame();

            }
        }
    }

    void updateTimer(float currentTime)
    {
        currentTime += 1;

        float seconds = Mathf.FloorToInt(currentTime % 60);

        TimerUI.text = seconds.ToString();
    }

    public void endMinigame()
    {
        popUpAnimationText(FinishText);
     }

    public void updatePlayersListAndScore()
    {
        var playersList = GameData.playersList;
        unactiveAllPlayersGameobjects();

        for (int i = 0; i < playersList.Count; i++)
        {

            if (playersList[i].selectedCharacter != "")
            {
                GameObject playerGameObject = playersGameobjects.Find(g => g.name == "Player" + playersList[i].id);

                if (GameData.minigameMode == "Duel" && playersList[i].isDuel == true && !playerGameObject.activeSelf)
                    playerGameObject.SetActive(true);
                else if (GameData.minigameMode == "Battle" && !playerGameObject.activeSelf)
                    playerGameObject.SetActive(true);

                GameObject playerCharacter = playerGameObject.transform.Find("PlayerCharacter").gameObject;
                if (playerCharacter.GetComponent<Image>().sprite == null)
                    playerCharacter.GetComponent<Image>().sprite = charactersSprites.Find(spr => spr.name == playersList[i].selectedCharacter);

                GameObject playerScore = playerGameObject.transform.Find("PlayerScore").gameObject;
                if (playersList[i].score != null && playersList[i].score != "")
                    playerScore.GetComponent<TextMeshProUGUI>().text = playersList[i].score;

            }
        }
    }
    void unactiveAllPlayersGameobjects()
    {
        foreach (GameObject player in playersGameobjects)
        {
            player.SetActive(false);
        }
    }

    void popUpAnimationText(GameObject textTarget)
    {
        textTarget.SetActive(true);

        Sequence mySequence = DOTween.Sequence();

        mySequence.Append(textTarget.transform.DOScale(1.05f, 0.1f));
        mySequence.Append(textTarget.transform.DOScale(1f, 0.1f));
        mySequence.Append(textTarget.transform.DOScale(1.1f, 0.11f).SetDelay(1.2f).OnComplete(() => {
            if (textTarget.name == "GoText")
            {
                initMinigame();
            }; }));
               mySequence.Append(textTarget.transform.DOScale(0f, 0.08f).OnComplete(() => {
            textTarget.SetActive(false);
            if (textTarget.name == "GoText")
            {
                isPlaying = true;
            }
            if (textTarget.name == "FinishText")
            {
                WebsocketManager = GameObject.Find("WebsocketManager");
                var websocket = WebsocketManager.GetComponent<WebsocketManager>();
                websocket.endMinigame();
            }
        }));

    }

    void initMinigame()
    {
        switch (minigameData.id)
        {
            case "1":
                minigameLogic.GetComponent<Minigame1>().initMinigame();
                break;
        }
       
    }
}