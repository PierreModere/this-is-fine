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

    public GameObject LectureGameobject;
    public GameObject CharacterSprite;
    public GameObject DialogBox;


    public List<string> SentencesList;
    List<string> shuffledList;
    int currentSentenceIndex = 0;


    public GameObject HandPrefab;
    public List<GameObject> HandObjects;

    int wavesNumber = 5;
    int handNumberByWave = 8;

    bool isPlaying = false;

    public List<Sprite> charactersSprites;
    int playerProgressIndex;
    int playerScore;

    public float speed; // vitesse de la levitation


    private void OnEnable()
    {
        if (GameData.selectedCharacter != "") CharacterSprite.GetComponent<Image>().sprite = charactersSprites.Find(spr => spr.name == GameData.selectedCharacter);
        else CharacterSprite.GetComponent<Image>().sprite = charactersSprites.Find(spr => spr.name == "youngwoman");
    }

    void Update()
    {
        if (isPlaying)
        {
            foreach (GameObject hand in HandObjects)
            {
                hand.transform.localPosition = new Vector3(hand.transform.localPosition.x + Random.Range(-1, 1) * Mathf.Sin(speed * Time.time), hand.transform.localPosition.y + Random.Range(-1, 1) * Mathf.Sin(speed * Time.time), hand.transform.localPosition.z + Random.Range(-1, 1) * Mathf.Sin(speed * Time.time));
            }
        }
    
    }
    public void initMinigame()
    {
        //WebsocketManager = GameObject.Find("WebsocketManager");

       

        isPlaying = true;
        playerProgressIndex = 0;
        playerScore = 0;
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

    void speechAnim()
    {
        Sequence speechAnim = DOTween.Sequence();
        //Zoom
        speechAnim.Append(LectureGameobject.transform.DOScale(1.8f, 0.5f).SetEase(Ease.InElastic));
        //Parle
        speechAnim.Append(DialogBox.GetComponent<Image>().DOFade(1f, 0).SetDelay(0.3f));
        // Associe phrase
        DialogBox.transform.Find("Text").gameObject.GetComponent<TextMeshProUGUI>().text = GetRandomSentence();
        speechAnim.Join(CharacterSprite.transform.DOScaleY(1.2f, 0.05f));
        speechAnim.Append(CharacterSprite.transform.DOScaleY(1, 0.1f));
        // Apparition bulle de texte
        speechAnim.Join(DialogBox.transform.Find("Text").gameObject.GetComponent<TextMeshProUGUI>().DOFade(1f, 0));
        speechAnim.Join(DialogBox.transform.DOLocalMoveY(-175f, 0.25f));
        // Fin parle
        speechAnim.Append(CharacterSprite.transform.DOScaleY(1.1f, 0.1f));
        speechAnim.Append(CharacterSprite.transform.DOScaleY(1, 0.1f));
    
        // Dézoom
        speechAnim.Append(LectureGameobject.transform.DOScale(1f, 0.15f).SetDelay(1.5f));
        speechAnim.Join(DialogBox.GetComponent<Image>().DOFade(0f, 0.2f));
        // Disparition bulle de texte
        speechAnim.Join(DialogBox.transform.Find("Text").gameObject.GetComponent<TextMeshProUGUI>().DOFade(0f, 0.2f));
        speechAnim.Join(DialogBox.transform.DOLocalMoveY(-50f, 0.25f)).OnComplete(() =>
        {
            StartCoroutine(spawnWave());
        });
 
    }


    private IEnumerator spawnWave()
    {
        for (int i = 0; i < handNumberByWave; i++)
        {
            spawnHand();

            yield return new WaitForSeconds(0.2f);
        }
    }

    public void spawnHand()
    {
        GameObject hand = Instantiate(HandPrefab, transform.position, Quaternion.identity,transform);
        hand.transform.DORotate(new Vector3(0, 0, Random.Range(0, 360)), 0).OnComplete(() =>
        {
            
            Vector3 displacement = hand.transform.rotation * Vector3.right * 800f;

            Sequence handArriving = DOTween.Sequence();

            handArriving.Append(hand.transform.DOLocalMove(displacement, 0.5f).SetEase(Ease.InOutElastic)).OnComplete(() =>
            {
                HandObjects.Add(hand);
                addDragEvent(hand);
            });

        });
    }

    void addDragEvent(GameObject hand)
    {
        var onDrag = new EventTrigger.Entry();
        onDrag.eventID = EventTriggerType.Drag;
        onDrag.callback.AddListener((data) => { removeHand((BaseEventData)data,hand); });

        var trig = hand.AddComponent<EventTrigger>();

        trig.triggers.Add(onDrag);


    }

    public void removeHand(BaseEventData data,GameObject hand)
    {
        if (isPlaying)
        {
            Vector3 displacement = hand.transform.rotation * Vector3.right * -800f;

            hand.transform.DOLocalMove(displacement, 0.3f).OnComplete(() =>
            {
                HandObjects.Remove(hand);
                Destroy(hand);

                checkHandNumber();
            });
        }
    }

    void checkHandNumber()
    {
        if (HandObjects.Count < 1)
        {
            playerProgressIndex++;
            playerScore++;
            sendScore();
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
   void sendScore()
    {
        if (GameData.joinedRoomCode != "")
            WebsocketManager.GetComponent<WebsocketManager>().sendScore(playerScore);
    }

    void shuffleSentences()
    {
        System.Random random = new System.Random();

        shuffledList = new List<string>(SentencesList);

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

}
