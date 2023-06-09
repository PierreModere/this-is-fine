using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class FirstMinigameAnimation : MonoBehaviour
{
    public GameData GameData;

    public GameObject InstructionText;
    public GameObject FirstMinigamePreview;
    public GameObject FirstMinigamePreviewShadow;
    public GameObject OkButton;

    private GameObject WebsocketManager;

    public List<Minigame> minigamesList;

    public float timeInterval = 0.08f;
    public float totalTime = 2.5f; 
    private int currentSpriteIndex; 
    private float timeElapsed; 
    private bool isChangingSprite; 

    int randomID;

    private void OnEnable()
    {
        initScreen();
    }

    void initScreen()
    {

        WebsocketManager = GameObject.Find("WebsocketManager");

        if (!GameData.isFirstMinigame) WebsocketManager.GetComponent<WebsocketManager>().changeScreenForEveryone("DashboardCanvas");

        if (GameData.isHost)
        {
            OkButton.SetActive(true);
        }
        else
        {
            OkButton.SetActive(false);
        }

        Sequence mySequence = DOTween.Sequence();

        mySequence.Append(InstructionText.transform.DOScale(1.25f, 0.15f));
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
        test.Append(OkButton.transform.DOLocalMoveY(-715f, 0.2f));
        test.Append(OkButton.transform.DOLocalMoveY(-1150, 0.3f)).OnComplete(()=> { WebsocketManager.GetComponent<WebsocketManager>().sendSelectedMinigame(GameData.firstMinigameID); });
        
    }

    public void displaySelectedMinigame()
    {
        ChangeSpriteOverTime();
    }

    public void ChangeSpriteOverTime()
    {
        if (isChangingSprite)
        {
            return;
        }

        // initialiser les variables
        currentSpriteIndex = 0;
        timeElapsed = 0f;
        isChangingSprite = true;

        // d�marrer une coroutine pour changer le sprite
        StartCoroutine(ChangeSpriteCoroutine());
    }

    // coroutine pour changer le sprite toutes les `timeInterval` secondes pendant `totalTime` secondes
    private IEnumerator ChangeSpriteCoroutine()
    {
        while (timeElapsed < totalTime)
        {
            // changer le sprite affich� sur le composant Image
            FirstMinigamePreview.GetComponent<Image>().sprite = minigamesList[currentSpriteIndex].preview;

            // incr�menter l'index du sprite actuel
            currentSpriteIndex++;

            // si on a atteint la fin du tableau de sprites, revenir au d�but
            if (currentSpriteIndex >= minigamesList.Count)
            {
                currentSpriteIndex = 0;
            }

            // attendre `timeInterval` secondes avant de changer le sprite suivant
            yield return new WaitForSeconds(timeInterval);

            // ajouter le temps �coul� depuis le d�but du changement de sprite
            timeElapsed += timeInterval;
        }

        string displayedMinigameID = GameData.displayedMinigameID;

        Debug.Log(displayedMinigameID);

        Sequence mySequence = DOTween.Sequence();

        mySequence.Append(FirstMinigamePreview.transform.DOScale(7.3f, 0.1f).OnComplete(() =>
        {
            FirstMinigamePreview.GetComponent<Image>().sprite = minigamesList.Find(mg => mg.id == displayedMinigameID).preview;
        }));
        mySequence.Append(FirstMinigamePreview.transform.DOScale(7f, 0.15f));   


        // r�initialiser les variables
        currentSpriteIndex = 0;
        timeElapsed = 0f;
        isChangingSprite = false;
    }
}
