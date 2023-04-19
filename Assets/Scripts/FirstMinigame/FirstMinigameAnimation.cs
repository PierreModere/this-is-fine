using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Newtonsoft.Json.Bson;

public class FirstMinigameAnimation : MonoBehaviour
{
    public GameObject InstructionText;
    public GameObject FirstMinigamePreview;
    public GameObject FirstMinigamePreviewShadow;
    public GameObject OkButton;

    private GameObject WebsocketManager;

    public List<Minigame> minigamesList;


    // Start is called before the first frame update
    void Start()
    {

        WebsocketManager = GameObject.Find("WebsocketManager");
        bool isHost = WebsocketManager.GetComponent<WebsocketManager>().isHost;
        if (isHost)
        {
            OkButton.SetActive(true);
        }
        else
        {
            OkButton.SetActive(false);
        }

        setRandomFirstMinigame();

        // Grab a free Sequence to use
        Sequence mySequence = DOTween.Sequence();
        // Add a movement tween at the beginning
        mySequence.Append(InstructionText.transform.DOScale(1.25f, 0.15f));
        // Add a rotation tween as soon as the previous one is finished
        mySequence.Append(InstructionText.transform.DOScale(1f, 0.2f));
        mySequence.Append(InstructionText.transform.DOLocalMoveY(775f, 0.7f).SetDelay(0.6f));
        mySequence.Join(FirstMinigamePreview.transform.DOLocalMoveY(75f, 0.8f));
        mySequence.Join(FirstMinigamePreviewShadow.transform.DOLocalMoveY(60f, 0.82f));

        mySequence.Append(OkButton.transform.DOLocalMoveY(-740f, 0.8f).SetDelay(-0.2f));

         OkButton.GetComponent<Button>().onClick.AddListener(onBtnClick);


    }

    public void onBtnClick()
    {
        Sequence test = DOTween.Sequence();
        // Add a movement tween at the beginning
        test.Append(OkButton.transform.DOLocalMoveY(-715f, 0.2f));
        test.Append(OkButton.transform.DOLocalMoveY(-1150, 0.3f));

    }

    void setRandomFirstMinigame()
    {
        var randomID = Random.Range(1, minigamesList.Count);
        FirstMinigamePreview.GetComponent<Image>().sprite = minigamesList[randomID-1].preview;
        WebsocketManager.GetComponent<WebsocketManager>().sendSelectedMinigame(randomID);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
