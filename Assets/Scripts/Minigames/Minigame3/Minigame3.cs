using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Minigame3 : MonoBehaviour
{
    public GameData GameData;
    public MinigameUI MinigameUI;
    private GameObject WebsocketManager;

    public GameObject controls;
    public GameObject fillButton;
    public GameObject pistonGameobject;

    public List<GameObject> cartridgesList;
    public GameObject CartridgesLine;
    public GameObject cartridgePrefab;
    private GameObject currentCartridge;
    int moveOffset = 434;
    int inkLevelMax = 216;

    int playerProgressIndex;
    double playerScore;

    int scoreMax = 10;
    int scoreRange = 10 * 2;

    private bool isHolding = false;
    public bool isAbleToFill = true;

    int pistonUpPos = 610;
    int pistonDownPos = 390;

    private float timer = 0.0f;
    private float interval = 0.04f;
    public float inkIncreaseLevel = 2f;


      // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (isHolding && currentCartridge != null && timer >= interval)  {
            fillUpCurrentCartridge();
            inkIncreaseLevel *= 1.15f;
            timer = 0.0f;
        }
    }

    public void initMinigame()
    {
        WebsocketManager = GameObject.Find("WebsocketManager");

        playerProgressIndex = 0;
        playerScore = 0;
        currentCartridge = cartridgesList[playerProgressIndex];
        activeButtons();
        addNewCartridge();

    }

    public void finishMinigame()
    {
        isAbleToFill = false;
        fillButton.GetComponent<Button>().interactable = false;
    }

    void activeButtons()
    {
        controls.transform.DOLocalMoveY(-1300, 0f);
        controls.transform.DOLocalMoveY(-700, 0.4f);       
    }

    public void onButtonHold()
    {
        if (isAbleToFill) isHolding = true;

    }
    public void onButtonRelease()
    {
        if (isAbleToFill && currentCartridge.transform.Find("InkLevelMask").Find("InkLevel").gameObject.GetComponent<RectTransform>().sizeDelta.y>0) addNewCartridge();
        isHolding = false;
        
    }

    void moveLineAnimation()
    {
        Transform LineTransform = CartridgesLine.transform; 

        Sequence lineMoveAnim = DOTween.Sequence();
        lineMoveAnim.Append(LineTransform.transform.DOLocalMoveX(LineTransform.localPosition.x - moveOffset, 0.3f)).OnComplete(() =>
        {
            pistonDownAnimation();
        });
    }
    void pistonUpAnimation()
    {
        Sequence downAnim = DOTween.Sequence();
        downAnim.Append(pistonGameobject.transform.DOLocalMoveY(pistonUpPos + 20, 0.2f));
        downAnim.Append(pistonGameobject.transform.DOLocalMoveY(pistonUpPos, 0.1f)).OnComplete(() =>
        {
            moveLineAnimation();
        });

    }
    void pistonDownAnimation()
    {
        Sequence downAnim = DOTween.Sequence();
        downAnim.Append(pistonGameobject.transform.DOLocalMoveY(pistonDownPos-20, 0.2f));
        downAnim.Append(pistonGameobject.transform.DOLocalMoveY(pistonDownPos, 0.1f)).OnComplete(() =>
        {
            inkIncreaseLevel = 2f;
            fillButton.GetComponent<Button>().interactable = true;
            isAbleToFill = true;
        });

    }

    void addNewCartridge()
    {
        playerProgressIndex++;
        checkInkLevel();
        isAbleToFill = false;

        fillButton.GetComponent<Button>().interactable = false;

        GameObject cartridge = Instantiate(cartridgePrefab, new Vector3(0, 0, 0), Quaternion.identity, CartridgesLine.transform);
        cartridge.name = "Cartridge" + (playerProgressIndex + 2);

        cartridgesList.Add(cartridge);
        currentCartridge = cartridgesList[playerProgressIndex];

        pistonUpAnimation();

    }

    void fillUpCurrentCartridge()
    {
        RectTransform inkLevel = currentCartridge.transform.Find("InkLevelMask").Find("InkLevel").gameObject.GetComponent<RectTransform>();
        inkLevel.sizeDelta = new Vector2(43, inkLevel.sizeDelta.y+inkIncreaseLevel);
        if (inkLevel.sizeDelta.y>=inkLevelMax)
        {
            if (isAbleToFill) addNewCartridge();
            isHolding = false;
        }
    }

    void checkInkLevel()
    {
        float inkLevel = currentCartridge.transform.Find("InkLevelMask").Find("InkLevel").gameObject.GetComponent<RectTransform>().sizeDelta.y;
        double score = Math.Round(scoreMax - Mathf.Abs(((inkLevel * scoreRange) / inkLevelMax) - scoreMax), 0, MidpointRounding.AwayFromZero);

        if (score > 8 && score < 11) MinigameUI.displayFeedback(true);
        if (score < 8 || score > 11) MinigameUI.displayFeedback(false);

        playerScore += score;
        Debug.Log(score);
        sendScore();



    }

    void sendScore()
    {
        if (GameData.joinedRoomCode != "")
            WebsocketManager.GetComponent<WebsocketManager>().sendScore(Convert.ToInt32(playerScore));
    }


}
