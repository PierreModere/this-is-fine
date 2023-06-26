using System;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minigame3 : MonoBehaviour
{
    public GameData GameData;
    public MinigameUI MinigameUI;
    private GameObject WebsocketManager;

    public GameObject controls;
    public GameObject fillButton;
    public GameObject pistonGameobject;
    private Animator pistonAnimator;

    public List<GameObject> cartridgesList;
    public GameObject CartridgesLine;
    public GameObject cartridgePrefab;
    private GameObject currentCartridge;
    int moveOffset = 500;
    int inkLevelMax = 216;

    public GameObject InkFlow;
    public float InkFlowHeight;
    private float InkFlowPositionY;

    int playerProgressIndex;
    double playerScore;

    int scoreMax = 10;
    int scoreRange = 10 * 2;

    private bool isHolding = false;
    public bool isAbleToFill = true;

    int pistonUpPos = 580;
    int pistonDownPos = 530;

    private float timer = 0.0f;
    private float interval = 0.04f;
    public float inkIncreaseLevel = 2f;

    public GameObject pipes;
    public float wiggleAmount = 0.1f;
    public float wiggleSpeed = 5f; // Vitesse de la secousse
    public float wiggleInterval = 0.01f;

    Animator beltAnimator;

    public List<GameObject> beltSFX;
    public List<GameObject> buttonSFX;

    bool hasPlaySound = false;

    void Start()
    {
        InkFlow.transform.localScale = new Vector3(1f, 0f, 1f);

        InkFlowPositionY = InkFlow.transform.localPosition.y;

        InvokeRepeating("pipesAnim", wiggleInterval, wiggleInterval);

    }

    void Update()
    {
        timer += Time.deltaTime;

        if (isHolding && isAbleToFill && currentCartridge != null && timer >= interval)  {

            if (!hasPlaySound)
            {
                GameObject.Find("Ink").GetComponent<AudioSource>().DOFade(1, 0);
                GameObject.Find("Ink").GetComponent<AudioSource>().Play();
                hasPlaySound = true;
            }
                
            fillUpCurrentCartridge();
            inkIncreaseLevel *= 1.15f;
            timer = 0.0f;
        }
    }

    public void initMinigame()
    {
        WebsocketManager = GameObject.Find("WebsocketManager");

        beltAnimator = transform.Find("MovingBelt").gameObject.GetComponent<Animator>();

        pistonAnimator = pistonGameobject.GetComponent<Animator>();

        playerProgressIndex = 0;
        playerScore = 0;
        currentCartridge = cartridgesList[playerProgressIndex];
        pistonDownAnimation();
    }

    public void finishMinigame()
    {
        isAbleToFill = false;
        fillButton.GetComponent<Button>().interactable = false;
    }

    private void pipesAnim()
    {
        float offsetX = Mathf.Sin(Time.time * wiggleSpeed) * wiggleAmount;
        float offsetY = Mathf.Sin(Time.time * wiggleSpeed) * wiggleAmount;

        foreach (Transform pipe in pipes.transform)
        {
            pipe.DOLocalMoveX(pipe.localPosition.x + offsetX * UnityEngine.Random.Range(-0.2f, 0.2f), 0);
        }

    }

    public void onButtonHold()
    {
        buttonSFX[UnityEngine.Random.Range(0, 1)].GetComponent<AudioSource>().Play();

        if (!isHolding) isHolding = true;
        if (isAbleToFill)
        {

            InkFlow.transform.DOScaleY(1f, 0.2f);
        }
    }

    public void onButtonRelease()
    {
        GameObject.Find("Ink").GetComponent<AudioSource>().DOFade(0, 0.2f);
        if (isAbleToFill && currentCartridge.transform.Find("InkLevelMask").Find("InkLevel").gameObject.GetComponent<RectTransform>().sizeDelta.y>0)
        {
            isHolding = false;

            addNewCartridge();
        }
    }

    void moveLineAnimation()
    {
        beltSFX[1].GetComponent<AudioSource>().pitch = UnityEngine.Random.Range(0.85f, 1.1f);
        beltSFX[1].GetComponent<AudioSource>().Play();
        Transform LineTransform = CartridgesLine.transform;

        beltAnimator.Play("movingBelt");

        Sequence lineMoveAnim = DOTween.Sequence();
        lineMoveAnim.Append(LineTransform.transform.DOLocalMoveX(LineTransform.localPosition.x - moveOffset, 0.5f).SetEase(Ease.OutBack)).OnComplete(() =>
        {
            beltAnimator.Play("notMovingBelt");
            pistonDownAnimation();
        });
    }
    void pistonUpAnimation()
    {
        pistonAnimator.Play("machine_going-up");

        Sequence downAnim = DOTween.Sequence();
        downAnim.Append(pistonGameobject.transform.DOLocalMoveY(pistonUpPos + 20, 0.2f));
        downAnim.Append(pistonGameobject.transform.DOLocalMoveY(pistonUpPos, 0.1f)).OnComplete(() =>
        {
            GameObject.Find("MachineUp").GetComponent<AudioSource>().Play();
            moveLineAnimation();
            pistonAnimator.Play("machine_idle-top");

        });

        InkFlow.transform.DOLocalMoveY(InkFlowPositionY - InkFlowHeight, 0.2f).OnComplete(() =>
        {
            InkFlow.transform.localScale = new Vector3(1f, 0f, 1f);
            InkFlow.transform.localPosition = new Vector3(InkFlow.transform.localPosition.x, InkFlowPositionY, InkFlow.transform.localPosition.z);
        });
    }
    void pistonDownAnimation()
    {
        hasPlaySound = false;
        GameObject.Find("MachineDown").GetComponent<AudioSource>().Play();
        pistonAnimator.Play("machine_going-down");

        Sequence downAnim = DOTween.Sequence();
        downAnim.Append(pistonGameobject.transform.DOLocalMoveY(pistonDownPos - 20, 0.2f));
        downAnim.Append(pistonGameobject.transform.DOLocalMoveY(pistonDownPos, 0.1f)).OnComplete(() =>
        {
            inkIncreaseLevel = 2f;
            fillButton.GetComponent<Button>().interactable = true;
            isAbleToFill = true;

            pistonAnimator.Play("machine_idle-bottom");
        });

    }

    void addNewCartridge()
    {
        playerProgressIndex++;
        checkInkLevel();
        isAbleToFill = false;

        fillButton.GetComponent<Button>().interactable = false;

        GameObject cartridge = Instantiate(cartridgePrefab, new Vector3(0, 0, 0), Quaternion.identity, CartridgesLine.transform);
        cartridge.GetComponent<RectTransform>().sizeDelta = new Vector2(391, 337);
        cartridge.name = "Cartridge" + (playerProgressIndex + 2);

        cartridgesList.Add(cartridge);
        currentCartridge = cartridgesList[playerProgressIndex];

        pistonUpAnimation();

    }

    void fillUpCurrentCartridge()
    {
        RectTransform inkLevel = currentCartridge.transform.Find("InkLevelMask").Find("InkLevel").gameObject.GetComponent<RectTransform>();
        inkLevel.sizeDelta = new Vector2(116, inkLevel.sizeDelta.y+inkIncreaseLevel);
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

        if (score >= 8 && score < 12) MinigameUI.displayFeedback(true);
        if (score < 8 || score >= 12) MinigameUI.displayFeedback(false);

        playerScore += score;
        sendScore();



    }

    void sendScore()
    {
        if (GameData.joinedRoomCode != "")
            WebsocketManager.GetComponent<WebsocketManager>().sendScore(Convert.ToInt32(playerScore));
    }


}
