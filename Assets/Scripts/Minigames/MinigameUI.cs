using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    public GameObject goodFeedbackPrefab;
    public GameObject badFeedbackPrefab;

    public Vector3[] feedbackPositions = new Vector3[3];
    public Vector3[] feedbackRotation = new Vector3[3];

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
                gameObject.GetComponent<CanvasGroup>().DOFade(1f, 0.1f).OnComplete(() =>
                {
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

        WebsocketManager = GameObject.Find("WebsocketManager");
        var websocket = WebsocketManager.GetComponent<WebsocketManager>();
        websocket.sendFinishMinigameAnimation();

    }

    public void finishMinigameAnimation()
    {
        popUpAnimationText(FinishText);

        switch (minigameData.id)
        {
            case "1":
                minigameLogic.GetComponent<Minigame1>().finishMinigame();
                break;
            case "2":
                minigameLogic.GetComponent<Minigame2>().initMinigame();
                break;
            case "3":
                minigameLogic.GetComponent<Minigame3>().finishMinigame();
                break;
            case "4":
                minigameLogic.GetComponent<Minigame4>().finishMinigame();
                break;

        }
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
        mySequence.Append(textTarget.transform.DOScale(1.1f, 0.11f).SetDelay(1.2f).OnComplete(() =>
        {
            if (textTarget.name == "GoText")
            {
                initMinigame();
            };
        }));
        mySequence.Append(textTarget.transform.DOScale(0f, 0.08f).OnComplete(async () =>
        {
            textTarget.SetActive(false);
            if (textTarget.name == "GoText")
            {
                isPlaying = true;
            }
            if (textTarget.name == "FinishText" && GameData.isHost)
            {
                var websocket = GameObject.Find("WebsocketManager").GetComponent<WebsocketManager>().websocket;
                string sceneName = "ResultsScene";
                string json = "{'type': 'changeScene', 'params':{'code': '" + GameData.joinedRoomCode + "','sceneName':'" + sceneName + "'}}";
                await websocket.SendText(json);
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
            case "2":
                minigameLogic.GetComponent<Minigame2>().initMinigame();
                break;
            case "3":
                minigameLogic.GetComponent<Minigame3>().initMinigame();
                break;
            case "4":
                minigameLogic.GetComponent<Minigame4>().initMinigame();
                break;
        }

    }

    public void displayFeedback(bool isGoodFeedback)
    {
        int randomIndex;
        GameObject text;
        if (isGoodFeedback)
        {
            randomIndex = Random.Range(0, minigameData.goodFeedbacks.Length);
            text = Instantiate(goodFeedbackPrefab, new Vector3(0, 0, 0), Quaternion.identity,transform);
            text.GetComponent<TextMeshProUGUI>().text = minigameData.goodFeedbacks[randomIndex];
        }
        else
        {
            randomIndex = Random.Range(0, minigameData.badFeedbacks.Length);
            text = Instantiate(badFeedbackPrefab, new Vector3(0, 0, 0), Quaternion.identity,transform);
            text.GetComponent<TextMeshProUGUI>().text = minigameData.badFeedbacks[randomIndex];
        }

        Sequence feedBackAnim = DOTween.Sequence();

        int randomPosIndex = Random.Range(0, feedbackPositions.Length);

        feedBackAnim.Append(text.transform.DOLocalMove(feedbackPositions[randomPosIndex], 0f));
        feedBackAnim.Join(text.transform.DOLocalRotate(feedbackRotation[randomPosIndex], 0f));
        feedBackAnim.Append(text.transform.DOScale(1.05f, 0.1f));
        feedBackAnim.Append(text.transform.DOScale(1f, 0.1f));
        feedBackAnim.Append(text.transform.DOScale(1.1f, 0.11f).SetDelay(0.5f));
        feedBackAnim.Append(text.transform.DOScale(0f, 0.08f).OnComplete(() => {
            Destroy(text);
        }));

    }
}