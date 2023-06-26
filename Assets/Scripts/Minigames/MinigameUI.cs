using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using static WebsocketManager;

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

    public GameObject StartAndStop;

    public List<GameObject> playersGameobjects;
    public List<Sprite> charactersSprites;

    public GameObject goodFeedbackPrefab;
    public GameObject badFeedbackPrefab;

    public Vector3[] feedbackPositions = new Vector3[3];
    public Vector3[] feedbackRotation = new Vector3[3];

    private GameObject WebsocketManager;

    Color redColor = new Color(0.9176471f, 0.2745098f, 0.2705882f, 1);
    Color baseColor = new Color(0.1647059f, 0.2156863f, 0.3058824f, 1);

    void Start()
    {
        cutsceneTimeLeft = minigameData.cutsceneTime;
        timeLeft = minigameData.gameTime;
        TimerUI.text = timeLeft.ToString();
        TimerUI.color = baseColor;
        gameObject.GetComponent<CanvasGroup>().alpha = 0f;
    }

    private void OnEnable()
    {
        resetPlayersReadiness();
        if (GameData.playersList != null) updatePlayersListAndScore();
        gameObject.GetComponent<CanvasGroup>().alpha = 0f;
        if (!minigameData.hasTimer) timerGameobject.SetActive(false);

        TimerUI.color = baseColor;

        // Add DOTween animation to isLeading
        var playersList = GameData.playersList;
        for (int i = 0; i < playersList.Count; i++) {
            if (playersList[i].selectedCharacter != "") {
                GameObject playerGameObject = playersGameobjects.Find(g => g.name == "Player" + playersList[i].id);
                GameObject isLeading = playerGameObject.transform.Find("IsLeading").gameObject;

                isLeading.transform.DORotate(new Vector3(0f, 0f, 360f), 10f, RotateMode.FastBeyond360)
                    .SetLoops(-1, LoopType.Restart)
                    .SetEase(Ease.Linear);
            }
        }
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

        if (TimerUI.text != seconds.ToString())
        {
            TimerUI.text = seconds.ToString();

            if (seconds == 3 || seconds == 2 || seconds == 1 || seconds == 0)
            {
                TimerUI.color = redColor;
                timerGameobject.transform.DOScale(1.5f, 0.2f).SetEase(Ease.OutBack).From();
            }
            else timerGameobject.transform.DOScale(1.1f, 0.1f).From();

        }
    }

    public void endMinigame()
    {

        WebsocketManager = GameObject.Find("WebsocketManager");
        var websocket = WebsocketManager.GetComponent<WebsocketManager>();
        websocket.sendFinishMinigameAnimation();

    }

    public void finishMinigameAnimation()
    {
        startPopUpAnimationText("stop");
        TimerUI.text = "0";

        switch (minigameData.id)
        {
            case "1":
                minigameLogic.GetComponent<Minigame1>().finishMinigame();
                break;
            case "2":
                minigameLogic.GetComponent<Minigame2>().finishMinigame();
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
        if (!isPlaying) checkPlayersReadyState();

        var playersList = GameData.playersList;
        unactiveAllPlayersGameobjects();

        List<WebsocketManager.ClientsList> sortedList = playersList.OrderByDescending(x => int.Parse(x.score)).ToList();

        for (int i = 0; i < sortedList.Count; i++)
        {

            if (sortedList[i].selectedCharacter != "")
            {
                GameObject playerGameObject = playersGameobjects.Find(g => g.name == "Player" + sortedList[i].id);

                if (GameData.minigameMode == "Duel" && sortedList[i].isDuel == true && !playerGameObject.activeSelf)
                    playerGameObject.SetActive(true);
                else if (GameData.minigameMode == "Battle" && !playerGameObject.activeSelf)
                    playerGameObject.SetActive(true);

                GameObject playerCharacter = playerGameObject.transform.Find("PlayerCharacter").gameObject;
                GameObject isLeading = playerGameObject.transform.Find("IsLeading").gameObject;

                if (i == 0) {
                    playerCharacter.GetComponent<Image>().sprite = charactersSprites.Find(spr => spr.name == "ui-score-" + sortedList[i].selectedCharacter + "_first");

                    isLeading.SetActive(true);
                } else {
                    if (i == sortedList.Count - 1) {
                        playerCharacter.GetComponent<Image>().sprite = charactersSprites.Find(spr => spr.name == "ui-score-" + sortedList[i].selectedCharacter + "_last");
                    } else {
                        playerCharacter.GetComponent<Image>().sprite = charactersSprites.Find(spr => spr.name == "ui-score-" + sortedList[i].selectedCharacter + "_mid");
                    }

                    isLeading.SetActive(false);
                }

                GameObject playerScore = playerGameObject.transform.Find("PlayerScore").gameObject;
                if (sortedList[i].score != null && sortedList[i].score != "")
                    playerScore.GetComponent<TextMeshProUGUI>().text = sortedList[i].score;

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

    public void startPopUpAnimationText(string type)
    {
        StartAndStop.SetActive(true);
        StartAndStop.GetComponent<Animator>().Play(type+"Anim");
    }

    public async void popUpAnimationEnd(string type)
    {
        StartAndStop.SetActive(false);

        if (type == "start")
        {
            initMinigame();
            isPlaying = true;
        }
        if (type == "stop" && GameData.isHost)
        {
            var websocket = GameObject.Find("WebsocketManager").GetComponent<WebsocketManager>().websocket;
            string sceneName = "ResultsScene";
            string json = "{'type': 'changeScene', 'params':{'code': '" + GameData.joinedRoomCode + "','sceneName':'" + sceneName + "'}}";
            await websocket.SendText(json);
        }
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

        feedBackAnim.Append(text.transform.DOLocalMove(feedbackPositions[0], 0f));
        feedBackAnim.Append(text.transform.DOScale(0.85f, 0.1f).SetEase(Ease.OutBack).From());
        feedBackAnim.Join(text.transform.DOLocalMoveY(text.transform.localPosition.y + 100 , 0.15f).SetEase(Ease.InBack).SetDelay(0.6f));
        feedBackAnim.Join(text.GetComponent<TextMeshProUGUI>().DOFade(0, 0.15f));
        feedBackAnim.OnComplete(() => {
            Destroy(text);
        });

    }

    void resetPlayersReadiness()
    {  

        foreach (var player in GameData.playersList)
        {
            player.isReady = false;
        }
   
    }
    void checkPlayersReadyState()
    {

        bool everyoneReady = true;

        List<ClientsList> playersList;

        if (GameData.minigameMode == "Duel")
            playersList = GameData.playersList.FindAll(player => player.isDuel);

        else playersList = GameData.playersList;

        foreach (var player in playersList)
        {
            if (!player.isReady)
                everyoneReady = false;
        }
        if (everyoneReady)
        {
            Debug.Log("Tout le monde est prêt !");
            gameObject.GetComponent<CanvasGroup>().DOFade(1f, 0.1f).OnComplete(() =>
            {
                startPopUpAnimationText("start");
            });
        }
        else Debug.Log("Pas tout le monde est prêt !");
    }
}