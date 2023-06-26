using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
using static UnityEngine.EventSystems.EventTrigger;
using System.Collections;
using TMPro;

public class Minigame4 : MonoBehaviour
{
    public GameData GameData;
    public MinigameUI MinigameUI;
    private GameObject WebsocketManager;

    public GameObject AllFrontObjects;
    public GameObject DialogBox;
    public GameObject Public;

    public Image BlurLayer;

    List<string> shuffledList;
    int currentSentenceIndex = 0;

    public List<Sprite> HandSprites;

    public GameObject HandPrefab;
    public List<GameObject> HandObjects;

    public float handMovementAmount;

    int wavesNumber = 5;
    int handNumberByWave = 8;

    bool isPlaying = false;

    public List<Sprite> charactersSprites;
    int playerProgressIndex = 0;
    int playerScore = 0;

    public float speed; // vitesse de la levitation
    public List<GameObject> charactersGameobjects;
    GameObject selectedCharacter;

    ///////// TYPE WRITER EFFECT

    string writer;
    float timeBtwChars = 0.07f;


    [SerializeField] float talkingDuration;

    public List<string> oldmanSentences;
    public List<string> oldwomanSentences;
    public List<string> youngmanSentences;
    public List<string> youngwomanSentences;

    public GameObject crowdSFX;
    public List<GameObject> shhtSFX;
    public GameObject slideInSFX;
    public GameObject slideOutSFX;

    private void OnEnable()
    {
        BlurLayer.material.DOFloat(0, "_Size", 0);

        foreach (GameObject ch in charactersGameobjects)
        {
            ch.SetActive(false);
        }

        selectedCharacter = charactersGameobjects.Find(go => go.name == GameData.selectedCharacter);
        selectedCharacter.SetActive(true);
    }

    void Update()
    {
        if (isPlaying)
        {
            foreach (GameObject hand in HandObjects)
            {
                hand.transform.localPosition = new Vector3(hand.transform.localPosition.x + Random.Range(-handMovementAmount, handMovementAmount) * Mathf.Sin(speed * Time.time), hand.transform.localPosition.y + Random.Range(-handMovementAmount, handMovementAmount) * Mathf.Sin(speed * Time.time), hand.transform.localPosition.z + Random.Range(-handMovementAmount, handMovementAmount) * Mathf.Sin(speed * Time.time));
            }
        }

    }
    public void initMinigame()
    {
        WebsocketManager = GameObject.Find("WebsocketManager");

        isPlaying = true;
        shuffleSentences();
        initNewWave();
    }

    void endMinigame()
    {
        MinigameUI.endMinigame();
    }

    public void finishMinigame()
    {
        isPlaying = false;
    }

    void initNewWave()
    {
        speechAnim();
    }

    void sendScore()
    {
        if (GameData.joinedRoomCode!="")
            WebsocketManager.GetComponent<WebsocketManager>().sendScore(playerScore);
    }

    void speechAnim()
    {
        Sequence speechAnim = DOTween.Sequence();
        //Zoom
        speechAnim.Append(AllFrontObjects.transform.DOScale(1, 0.7f));
        speechAnim.InsertCallback(0.0f, () => shhtSFX[Random.Range(0, shhtSFX.Count - 1)].GetComponent<AudioSource>().Play());
        speechAnim.Join(crowdSFX.GetComponent<AudioSource>().DOFade(0.5f, 0.6f));
        speechAnim.Join(BlurLayer.material.DOFloat(0f, "_Size", 0.6f));
        speechAnim.Join(Public.transform.DOScale(1.7f, 0.5f));


        // Apparition bulle de texte
        DialogBox.SetActive(true);
        speechAnim.Join(DialogBox.transform.DOScale(0.7f, 0));
        speechAnim.Append(DialogBox.GetComponent<Image>().DOFade(1, 0.4f).SetDelay(0.3f));
        speechAnim.Join(DialogBox.transform.DOScale(1, 0.4f));

        speechAnim.Join(DialogBox.transform.Find("Text").gameObject.GetComponent<TextMeshProUGUI>().DOFade(1, 0).OnStart(() => {
            // Associe phrase
            StartTalking(GetRandomSentence());
        }));
    }

    public void hideDialogBox()
    {

        Sequence dialogHide = DOTween.Sequence();

        // D�zoom
        dialogHide.Append(AllFrontObjects.transform.DOScale(0.8f, 0.4f).SetDelay(1.5f));
        dialogHide.Join(crowdSFX.GetComponent<AudioSource>().DOFade(1.0f, 0.4f));
        dialogHide.Join(BlurLayer.material.DOFloat(2.2f, "_Size", 0.4f));
        dialogHide.Join(Public.transform.DOScale(1, 0.4f).OnStart(() => {
            StartCoroutine(spawnWave());
        }));

        // Disparition bulle de texte
        dialogHide.Join(DialogBox.GetComponent<Image>().DOFade(0, 0.4f));
        dialogHide.Join(DialogBox.transform.Find("Text").gameObject.GetComponent<TextMeshProUGUI>().DOFade(0, 0.2f));
        dialogHide.Join(DialogBox.transform.DOScale(0.6f, 0.4f));

        dialogHide.OnComplete(() => {
            DialogBox.SetActive(false);
        });
    }

    private IEnumerator spawnWave()
    {
        for (int i = 0; i < handNumberByWave; i++)
        {
            spawnHand();

            yield
            return new WaitForSeconds(0.2f);
        }
    }

    public void spawnHand()
    {
        GameObject hand = Instantiate(HandPrefab, transform.position, Quaternion.identity, transform);
        hand.GetComponent<Image>().sprite = HandSprites[Random.Range(0, HandSprites.Count)];
        hand.transform.SetSiblingIndex(Public.transform.GetSiblingIndex());
        hand.transform.DORotate(new Vector3(0, 0, Random.Range(0, 360)), 0).OnComplete(() => {

            Vector3 displacement = hand.transform.rotation * Vector3.right * 250f;

            slideInSFX.GetComponent<AudioSource>().Play();

            Sequence handArriving = DOTween.Sequence();

            handArriving.Append(hand.transform.DOLocalMove(displacement, 0.4f).SetEase(Ease.OutBack)).OnComplete(() => {
                HandObjects.Add(hand);
                addDragEvent(hand);
            });

        });
    }

    void addDragEvent(GameObject hand)
    {
        var onDrag = new EventTrigger.Entry();
        onDrag.eventID = EventTriggerType.EndDrag;
        onDrag.callback.AddListener((data) => {
            removeHand(data, hand);
        });

        var trig = hand.AddComponent<EventTrigger>();
        trig.triggers.Add(onDrag);

    }

    public void removeHand(BaseEventData data, GameObject hand)
    {
        if (isPlaying)
        {
            Vector3 displacement = hand.transform.rotation * Vector3.right * -230f;

            slideOutSFX.GetComponent<AudioSource>().Play();

            hand.transform.DOLocalMove(displacement, 0.3f).OnComplete(() => {
                HandObjects.Remove(hand);
                Destroy(hand);
                checkHandNumber();

                playerScore++;
                sendScore();
            });
        }
    }

    void checkHandNumber()
    {
        if (HandObjects.Count == 0)
        {
            playerProgressIndex++;

            if (playerProgressIndex == wavesNumber)
            {
                endMinigame();
                Debug.Log("finis !");
            }
            else
            {
                MinigameUI.displayFeedback(true);
                initNewWave();
            }
        }
    }

    void shuffleSentences()
    {
        System.Random random = new System.Random();



        switch (GameData.selectedCharacter)
        {
            case "youngman":
                shuffledList = new List<string>(youngmanSentences);
                break;
            case "youngwoman":
                shuffledList = new List<string>(youngwomanSentences);
                break;
            case "oldwoman":
                shuffledList = new List<string>(oldwomanSentences);
                break;
            case "oldman":
                shuffledList = new List<string>(oldmanSentences);
                break;

        }

        int n = shuffledList.Count;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            string value = shuffledList[k];
            shuffledList[k] = shuffledList[n];
            shuffledList[n] = value;
        }
    }

    string GetRandomSentence()
    {
        string sentence = shuffledList[currentSentenceIndex];
        currentSentenceIndex = (currentSentenceIndex + 1) % shuffledList.Count;
        return sentence;
    }

    public void StartTalking(string sentence)
    {
        writer = sentence;
        timeBtwChars = talkingDuration / writer.Length;

        DialogBox.transform.Find("Text").gameObject.GetComponent<TextMeshProUGUI>().text = "";

        switch (GameData.selectedCharacter)
        {
            case "youngman":
                selectedCharacter.GetComponent<youngman_Animation>().moveLeftArm();
                break;
            case "youngwoman":
                selectedCharacter.GetComponent<youngwoman_Animation>().moveLeftArm();
                break;
            case "oldwoman":
                selectedCharacter.GetComponent<oldwoman_Animation>().moveLeftArm();
                break;
            case "oldman":
                 selectedCharacter.GetComponent<oldman_Animation>().moveArms();
            break;

        }

        StartCoroutine("TypeWriterTMP");
    }

    IEnumerator TypeWriterTMP()
    {
        for (int i = 0; i < writer.Length; i++)
        {
            char c = writer[i];

            DialogBox.transform.Find("Text").gameObject.GetComponent<TextMeshProUGUI>().text += c;

            switch (GameData.selectedCharacter)
            {
                case "youngman":
                    selectedCharacter.GetComponent<youngman_Animation>().changeMouth();
                    break;
                case "youngwoman":
                    selectedCharacter.GetComponent<youngwoman_Animation>().changeMouth();
                    break;
                case "oldwoman":
                    selectedCharacter.GetComponent<oldwoman_Animation>().changeMouth();
                    break;
                case "oldman":
                    selectedCharacter.GetComponent<oldman_Animation>().changeMouth();
                    break;

            }

            if (i == writer.Length - 1)
            {
                switch (GameData.selectedCharacter)
                {
                    case "youngman":
                        selectedCharacter.GetComponent<youngman_Animation>().closeMouth();
                        break;
                    case "youngwoman":
                        selectedCharacter.GetComponent<youngwoman_Animation>().closeMouth();
                        break;
                    case "oldwoman":
                        selectedCharacter.GetComponent<oldwoman_Animation>().closeMouth();
                        break;
                    case "oldman":
                        selectedCharacter.GetComponent<oldman_Animation>().closeMouth();
                    break;
                }
                hideDialogBox();
            }

            yield
            return new WaitForSeconds(timeBtwChars);
        }
    }
}