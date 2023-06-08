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

    public List <Sprite> TrashSprites;
    public GameObject trashPrefab;

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
            spawnTrash();
            Sequence valveRotation = DOTween.Sequence();

            valveRotation.Append(valve.transform.DORotate(new Vector3(0, 0, valve.transform.rotation.eulerAngles.z - 45f), 0.3f).SetEase(Ease.InOutBack));
            valveRotation.Join(valveShadow.transform.DORotate(new Vector3(0, 0, valve.transform.rotation.eulerAngles.z - 45f), 0.3f).SetEase(Ease.InOutBack));
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
            Sequence valveError = DOTween.Sequence();

            valveError.Append(valve.transform.DORotate(new Vector3(0, 0, valve.transform.rotation.eulerAngles.z + 170f), 0.3f).SetEase(Ease.InOutBack));
            valveError.Join(valveShadow.transform.DORotate(new Vector3(0, 0, valve.transform.rotation.eulerAngles.z + 170f), 0.3f).SetEase(Ease.InOutBack));
            //finishMinigame();
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

        endCutscene.Append(buttonInstruction.GetComponent<Image>().DOFade(0f, 0.3f));
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

    void spawnTrash()
    {
        GameObject trash = Instantiate(trashPrefab, new Vector3(0, 0, 0), Quaternion.identity, backgroundObjects.transform);
        trash.transform.localPosition = new Vector3(Random.Range(-12,18), 80, 0);
        trash.GetComponent<Image>().sprite = TrashSprites[Random.Range(0, TrashSprites.Count)];
        trash.transform.SetSiblingIndex(2);
        Sequence falling = DOTween.Sequence();

        falling.Append(trash.transform.DORotate(new Vector3(0, 0, Random.Range(-360, 360)), 1f));
        falling.Join(trash.transform.DOLocalMoveY(-130,1.8f).SetEase(Ease.InOutSine).OnComplete(() => { Destroy(trash); }));
        falling.Join(trash.GetComponent<Image>().DOFade(0f, 0.3f).SetDelay(1.2f));
    }

    void sendScore()
    {
        if (GameData.joinedRoomCode!="" && WebsocketManager != null)
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
