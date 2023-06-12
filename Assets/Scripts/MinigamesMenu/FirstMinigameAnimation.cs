using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class FirstMinigameAnimation : MonoBehaviour
{
    public GameData GameData;

    public GameObject InstructionText;
    public GameObject FirstMinigamePreview;
    public GameObject Preview;

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

        InstructionText.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = "Définissons qui va commencer...";

        Sequence mySequence = DOTween.Sequence();

        FirstMinigamePreview.SetActive(false);
        mySequence.Append(InstructionText.transform.DOLocalMoveY(0f, 0));
        mySequence.Join(InstructionText.transform.Find("Text").DOScale(1.2f, 0.2f).SetEase(Ease.InOutBack).SetDelay(0.3f).From());
        mySequence.Append(InstructionText.transform.Find("Text").DOScale(1.1f, 0.15f).SetDelay(1).OnStart(() =>
        {
            InstructionText.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = "Avec un mini-jeu !";
            InstructionText.transform.Find("Text").DOScale(1f, 0.15f).SetDelay(0.15f);
            displaySelectedMinigame();
        }));

        mySequence.Append(InstructionText.transform.DOLocalMoveY(680f, 0.4f).SetDelay(0.6f).OnComplete(() => {
            FirstMinigamePreview.SetActive(true);
            FirstMinigamePreview.transform.DOScale(1.1f, 0.25f).SetEase(Ease.InOutBack).From();
        }));

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

        // démarrer une coroutine pour changer le sprite
        StartCoroutine(ChangeSpriteCoroutine());
    }

    // coroutine pour changer le sprite toutes les `timeInterval` secondes pendant `totalTime` secondes
    private IEnumerator ChangeSpriteCoroutine()
    {
        while (timeElapsed < totalTime)
        {
            // changer le sprite affiché sur le composant Image
            Preview.GetComponent<Image>().sprite = minigamesList[currentSpriteIndex].preview;

            // incrémenter l'index du sprite actuel
            currentSpriteIndex++;

            // si on a atteint la fin du tableau de sprites, revenir au début
            if (currentSpriteIndex >= minigamesList.Count)
            {
                currentSpriteIndex = 0;
            }

            // attendre `timeInterval` secondes avant de changer le sprite suivant
            yield return new WaitForSeconds(timeInterval);

            // ajouter le temps écoulé depuis le début du changement de sprite
            timeElapsed += timeInterval;
        }

        string displayedMinigameID = GameData.displayedMinigameID;


        Sequence randomEndSequence = DOTween.Sequence();

        randomEndSequence.Append(FirstMinigamePreview.transform.DOScale(1.1f, 0.2f).SetEase(Ease.InOutBack).From());

        randomEndSequence.OnStart(()=>
        {
            Preview.GetComponent<Image>().sprite = minigamesList.Find(mg => mg.id == displayedMinigameID).preview;
        });

        randomEndSequence.OnComplete(() =>
        {
            OkButton.GetComponent<Button>().interactable = true;
        });


        // réinitialiser les variables
        currentSpriteIndex = 0;
        timeElapsed = 0f;
        isChangingSprite = false;
    }
}
