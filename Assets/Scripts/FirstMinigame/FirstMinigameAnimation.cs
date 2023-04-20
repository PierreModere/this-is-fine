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

    public float timeInterval = 0.08f; // intervalle de temps entre chaque changement de sprite
    public float totalTime = 2.5f; // temps total pour le changement de sprite
    private int currentSpriteIndex; // index actuel dans le tableau de sprites
    private float timeElapsed; // temps écoulé depuis le début du changement de sprite
    private bool isChangingSprite; // flag indiquant si le changement de sprite est en cours


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
        // Add a movement tween at the beginning
        test.Append(OkButton.transform.DOLocalMoveY(-715f, 0.2f));
        test.Append(OkButton.transform.DOLocalMoveY(-1150, 0.3f));

    }

    void setRandomFirstMinigame()
    {
        var randomID = Random.Range(1, minigamesList.Count);
        WebsocketManager.GetComponent<WebsocketManager>().sendSelectedMinigame(randomID.ToString());
    }

    public void displaySelectedMinigame()
    {
        ChangeSpriteOverTime();
    }

    // fonction pour changer le sprite toutes les `timeInterval` secondes pendant `totalTime` secondes
    public void ChangeSpriteOverTime()
    {
        // ne commencez pas un nouveau changement de sprite si un est déjà en cours
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
            FirstMinigamePreview.GetComponent<Image>().sprite = minigamesList[currentSpriteIndex].preview;

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

        string displayedMinigameID = WebsocketManager.GetComponent<WebsocketManager>().displayedMinigameID;

        Sequence mySequence = DOTween.Sequence();

        mySequence.Append(FirstMinigamePreview.transform.DOScale(7.3f, 0.1f).OnComplete(() => {
            FirstMinigamePreview.GetComponent<Image>().sprite = minigamesList.Find(mg => mg.id == displayedMinigameID).preview;
        }));
        mySequence.Append(FirstMinigamePreview.transform.DOScale(7f, 0.15f));


        // réinitialiser les variables
        currentSpriteIndex = 0;
        timeElapsed = 0f;
        isChangingSprite = false;
    }
}
