using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Minigame1 : MonoBehaviour
{
    public MinigameUI MinigameUI;
    public GameData GameData;
    private GameObject WebsocketManager;

    int playerProgressIndex;
    int[] indexesSuite = new int[25];

    public GameObject valve;
    public GameObject valveShadow;

    public GameObject goopFlow;
    public GameObject backPipe;

    public GameObject foregroundObjects;
    public GameObject backgroundObjects;

    public List<GameObject> allBlurryForegroundObj;

    public GameObject buttonInstruction;
    public GameObject controls;

    public List<Sprite> buttonsInstructionsSprites;
    public List<GameObject> buttonsGameobjects;

    private bool isMultiPressed=false;

    public float rotateAmount = 0.1f; // Amplitude du mouvement de secousse
    public float rotateSpeed = 20;
    public float wiggleAmount = 0.1f;
    public float wiggleSpeed = 5f; // Vitesse de la secousse
    public float wiggleInterval = 0.01f; // Intervalle de temps entre chaque secousse

    private Vector3 backPipeRotation; // Position d'origine de l'objet

    private void Start()
    {
        backPipeRotation = goopFlow.transform.localRotation.eulerAngles;

        InvokeRepeating("backPipeAnim", wiggleInterval, wiggleInterval);
        InvokeRepeating("frontPipesAnims", wiggleInterval, wiggleInterval);
    }
    public void initMinigame()
    {
        WebsocketManager = GameObject.Find("WebsocketManager");

        playerProgressIndex = 0;
        for (int i = 0; i < indexesSuite.Length; i++)
        {
            var randomIndex = Random.Range(0, buttonsInstructionsSprites.Count);
            indexesSuite[i] = randomIndex;
        }
  
        activeButtons();
        updateInstruction();
        buttonInstruction.GetComponent<Image>().DOFade(1f, 0.1f);
    }

    private void backPipeAnim()
    {
        // Calcule un décalage de secousse aléatoire dans les axes X, Y et Z
        float offsetX = Mathf.Sin(Time.time * rotateSpeed) * rotateAmount;
        float offsetZ = Mathf.Sin((Time.time + 0.5f) * rotateSpeed) * rotateAmount;

        // Applique la secousse en ajoutant le décalage à la position d'origine de l'objet
        backPipe.transform.DORotate(backPipeRotation + new Vector3(0, 0, offsetZ),0);
        backPipe.transform.Find("PipeEnd").DORotate(backPipeRotation + new Vector3(0, 0, Random.Range(-1.5f,-1f)*offsetZ), 0);
        GameObject.Find("GoopFlow").transform.DOLocalMoveX(1.7f - offsetX, 0);

    }

    private void frontPipesAnims() {
        float offsetX = Mathf.Sin(Time.time * wiggleSpeed) * wiggleAmount;
        float offsetY = Mathf.Sin(Time.time * wiggleSpeed) * wiggleAmount;

        foreach (Transform pipe in foregroundObjects.transform.Find("BackgroundPipes"))
        {
            pipe.DOLocalMove(pipe.localPosition + new Vector3(offsetX*Random.Range(-1f,1), offsetY*Random.Range(-1f, 1), 0), 0);
        }
        foreach (Transform pipe in foregroundObjects.transform.Find("MiddlegroundPipes"))
        {
            pipe.DOLocalMove(pipe.localPosition + new Vector3(offsetX * Random.Range(-1f, 1), offsetY * Random.Range(-1f, 1), 0), 0);
        }
        foreach (Transform pipe in foregroundObjects.transform.Find("ForegroundPipes"))
        {
            pipe.DOLocalMove(pipe.localPosition + new Vector3(offsetX * Random.Range(-1f, 1), offsetY * Random.Range(-1f, 1), 0), 0);
        }

    }

    void updateInstruction()
    {
        buttonInstruction.GetComponent<Image>().sprite = buttonsInstructionsSprites[indexesSuite[playerProgressIndex]];
        buttonInstruction.transform.DOScale(1.5f, 0.15f).OnComplete(() => { buttonInstruction.transform.DOScale(1f, 0.2f); });
    }
    public void checkButton(int index)
    {
        if (indexesSuite[playerProgressIndex] == index && !isMultiPressed)
        {
            Sequence valveRotation = DOTween.Sequence();

            valveRotation.Append(valve.transform.DORotate(new Vector3(0, 0, valve.transform.rotation.eulerAngles.z - 45f), 0.2f));
            valveRotation.Join(valveShadow.transform.DORotate(new Vector3(0, 0, valve.transform.rotation.eulerAngles.z - 45f), 0.2f));
            valveRotation.Join(goopFlow.transform.DOScaleX(goopFlow.transform.localScale.x + 0.03f, 0.3f));

            playerProgressIndex++;
            sendScore();
            MinigameUI.displayFeedback(true);
            if (playerProgressIndex < indexesSuite.Length)
                updateInstruction();
            else endMinigame();

        }
        else if (indexesSuite[playerProgressIndex] != index && !isMultiPressed)
        {
            MinigameUI.displayFeedback(false);

        }
    }

    void activeButtons()
    {
        controls.SetActive(true);
        controls.transform.DOLocalMoveY(-260, 0.4f).From().OnComplete(() =>
        {
            foreach (GameObject button in buttonsGameobjects)
            {
                button.GetComponent<Button>().interactable = true;
            }
        });
    }

    void endMinigame()
    {
        MinigameUI.endMinigame();
    }

    public void finishMinigame()
    {
        foreach (GameObject button in buttonsGameobjects)
        {
            button.GetComponent<Button>().interactable = false;
        }

        Sequence endCutscene = DOTween.Sequence();

        endCutscene.Append(buttonInstruction.GetComponent<Image>().DOFade(0f, 0.2f));
        endCutscene.Join(foregroundObjects.transform.DOScale(1.46f, 0.6f).SetEase(Ease.InOutSine));
        endCutscene.Join(backgroundObjects.transform.DOScale(1.3f, 0.8f).SetEase(Ease.InOutSine));

        endCutscene.Join(controls.transform.DOLocalMoveY(-260, 0.5f).SetEase(Ease.InOutSine));

        foreach (Transform element in backgroundObjects.transform)
        {
            if (element.gameObject.GetComponent<Image>() != null && element.Find("Blurry") != null)
            {
                element.gameObject.GetComponent<Image>().DOFade(1f, 0.5f);
                element.Find("Blurry").gameObject.GetComponent<Image>().DOFade(0f, 0.5f);
                element.Find("PipeEnd").gameObject.GetComponent<Image>().DOFade(1f, 0.5f);
                element.Find("PipeEnd").Find("Blurry").gameObject.GetComponent<Image>().DOFade(0f, 0.5f);
            }
        }

        foreach (GameObject element in allBlurryForegroundObj)
        {
            if (element.GetComponent<Image>() != null && element.transform.Find("Blurry") != null)
            {
                element.GetComponent<Image>().DOFade(0f, 0.6f);
                element.transform.Find("Blurry").gameObject.GetComponent<Image>().DOFade(1f, 0.4f);
            }
        }

    }

    void sendScore()
    {
        if (GameData.joinedRoomCode!="" && GameData.joinedRoomCode!=null)
            WebsocketManager.GetComponent<WebsocketManager>().sendScore(playerProgressIndex);
    }

     private void Update()
    {
        if (Input.touchCount > 1)
        {
            isMultiPressed = true;
        }
        else isMultiPressed = false;
    }
}
