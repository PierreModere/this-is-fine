using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System.Threading;

public class Minigame2 : MonoBehaviour
{
    bool isPlaying = false;
    int playerScore = 0;
    int touching = 0;
    public GameObject contractsList;
    public List<Sprite> contractsSprites;

    public GameObject leftHandGroup;
    private GameObject leftHand;
    private GameObject leftHandShadow;
    private Vector3 leftHandPosition;
    public GameObject rightHandGroup;
    private GameObject rightHand;
    private GameObject rightHandShadow;
    private Vector3 rightHandPosition;

    private GameObject currentContractObject;
    private GameObject stamp;
    private bool isAnimating = false;

    public GameObject backgroundCanvas;

    public GameData GameData;
    public MinigameUI MinigameUI;
    private GameObject WebsocketManager;


    public List<Sprite> leftHandSprites;
    public List<Sprite> shadowLeftHandSprites;
    public List<Sprite> rightHandSprites;
    public List<Sprite> shadowRightHandSprites;

    public List<Sprite> stampSprites;

    public List<GameObject> cameraMovementParents;
    public List<Vector3> cameraMovementParentsOriginalPositions;

    private float timer = 0.0f;

    private Sequence stampedContractAnim;


    private void OnEnable()
    {
        initCharacterAssets();
        cameraMovement();

        foreach (GameObject parent in cameraMovementParents)
        {
            parent.transform.DOLocalMove(cameraMovementParentsOriginalPositions[cameraMovementParents.IndexOf(parent)],0);
        }
    }

    void initCharacterAssets()
    {
        if (GameData.selectedCharacter != "") {
            leftHandGroup.transform.Find("Hand").gameObject.GetComponent<Image>().sprite = leftHandSprites.Find(spr => spr.name == "hand-left-"+GameData.selectedCharacter);
            leftHandGroup.transform.Find("Shadow").gameObject.GetComponent<Image>().sprite = shadowLeftHandSprites.Find(spr => spr.name == "hand-left-" + GameData.selectedCharacter);
                
            rightHandGroup.transform.Find("Hand").gameObject.GetComponent<Image>().sprite = rightHandSprites.Find(spr => spr.name == "hand-right-" + GameData.selectedCharacter);
            rightHandGroup.transform.Find("Shadow").gameObject.gameObject.GetComponent<Image>().sprite = shadowRightHandSprites.Find(spr => spr.name == "hand-right-" + GameData.selectedCharacter);

            contractsList.transform.Find("0").Find("Stamp").gameObject.GetComponent<Image>().sprite = stampSprites.Find(spr => spr.name == "stamp-" + GameData.selectedCharacter);
        }
    }

    void cameraMovement()
    {
        int randomZ = Random.Range(-2, 2);
        Vector3 randomPostion = new Vector3(Random.Range(-10,10), Random.Range(-10, 10), 0);
        Sequence cameraMove = DOTween.Sequence();

        foreach (GameObject parent in cameraMovementParents)
        {
            cameraMove.Join(parent.transform.DOLocalRotate(new Vector3(0, 0, randomZ) ,2f).SetEase(Ease.Linear))
                .Join(parent.transform.DOLocalMove(cameraMovementParentsOriginalPositions[cameraMovementParents.IndexOf(parent)] + randomPostion, 1f).SetEase(Ease.Linear));
        }

        cameraMove.OnComplete(() => { cameraMovement(); });
    }
    public void initMinigame()
    {
        WebsocketManager = GameObject.Find("WebsocketManager");
        isPlaying = true;

        // Left hand
        leftHand = leftHandGroup.transform.Find("Hand").gameObject;
        leftHandShadow = leftHandGroup.transform.Find("Shadow").gameObject;
        leftHandPosition = leftHandGroup.transform.position;

        // Right hand
        rightHand = rightHandGroup.transform.Find("Hand").gameObject;
        rightHandShadow = rightHandGroup.transform.Find("Shadow").gameObject;
        rightHandPosition = rightHandGroup.transform.position;
    }

    public void finishMinigame()
    {
        isPlaying = false;
    }
    void sendScore()
    {
        if (GameData.joinedRoomCode!="")
            WebsocketManager.GetComponent<WebsocketManager>().sendScore(playerScore);
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (Input.touchCount == 1)
        {
            touching++;

            if (touching == 1 && isPlaying)
            {
                currentContractObject = contractsList.transform.Find(playerScore.ToString()).gameObject;
                stamp = currentContractObject.transform.Find("Stamp").gameObject;

                // Increase player score
                playerScore++;
                sendScore();
                if (playerScore%8 == 0) MinigameUI.displayFeedback(true);

                // Duplicate contract GameObject
                GameObject newContractObject = Instantiate(currentContractObject, new Vector3(0,0,0), Quaternion.identity, contractsList.transform);
                newContractObject.transform.localPosition = new Vector3(0, 0, 0);
                newContractObject.transform.SetAsFirstSibling();
                newContractObject.name = playerScore.ToString();
                int randomSpriteIndex = Random.Range(0, contractsSprites.Count);
                newContractObject.GetComponent<Image>().sprite = contractsSprites[randomSpriteIndex];

                // Stamp current contract and remove it
                AnimateStamp(timer, playerScore-1);
                timer = 0.0f;
            }
        }
        else if (Input.touchCount == 0)
        {
            touching = 0;
        }


    }

    void AnimateStamp(float timeBetweenClick,int contractID)
    {
        if (stampedContractAnim.IsActive()) stampedContractAnim.Restart();
        stampedContractAnim = DOTween.Sequence();

        GameObject contract = contractsList.transform.Find(contractID.ToString()).gameObject;

        stampedContractAnim.OnStart(() =>
        {
            isAnimating = true;
            //Debug.Log("Animation starts! (" + isAnimating + ")");
        });

        // Right hand going down and display stamp
        stampedContractAnim.Append(rightHandGroup.transform.DOScale(new Vector3(0.9f, 0.9f, 0.9f), 0.2f).SetEase(Ease.InQuint))
            .Join(rightHandShadow.transform.DOScale(new Vector3(1f, 1f, 1f), 0.2f).SetEase(Ease.InQuint))
            .Join(rightHandGroup.transform.DOMoveX(rightHandPosition.x - 60f, 0.2f).SetEase(Ease.InQuint))
            .AppendCallback(() =>
            {
                contract.transform.Find("Stamp").gameObject.SetActive(true);
            });
        
        // Right hand going up
        stampedContractAnim.Append(rightHandGroup.transform.DOScale(new Vector3(1f, 1f, 1f), 0.2f))
            .Join(rightHandShadow.transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.2f))
            .Join(rightHandGroup.transform.DOMoveX(rightHandPosition.x, 0.2f));
            
        // Left hand going down at the same time
        stampedContractAnim.Join(leftHandGroup.transform.DOScale(new Vector3(0.9f, 0.9f, 0.9f), 0.2f))
            .Join(leftHandShadow.transform.DOScale(new Vector3(1f, 1f, 1f), 0.2f));
        
        // Left hand removing current contract
        stampedContractAnim.Append(leftHandGroup.transform.DORotate(new Vector3(0f, 0f, 25f), 0.2f))
            .Join(leftHandGroup.transform.DOMoveX(leftHandPosition.x - 250f, 0.2f))
            .Join(contract.transform.DORotate(new Vector3(0f, 0f, 25f), 0.2f))
            .Join(contract.transform.DOMoveX(contract.transform.position.x - 900f, 0.4f));
        
        // Left hand going up
        stampedContractAnim.Insert(0.6f, leftHandGroup.transform.DOScale(new Vector3(1f, 1f, 1f), 0.2f))
            .Join(leftHandShadow.transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.2f))
            .Join(leftHandGroup.transform.DORotate(new Vector3(0f, 0f, 0f), 0.2f))
            .Join(leftHandGroup.transform.DOMoveX(leftHandPosition.x, 0.2f));
        
        // Destroy current contract when the animation is complete
        stampedContractAnim.OnComplete(() =>
        {
            Destroy(contract);

            isAnimating = false;
            //Debug.Log("Animation complete! (" + isAnimating + ")");
        });

        // Use timeScale setting to accelerate animation (default 1f, quicker >1f, slower <1f)

        stampedContractAnim.timeScale = playerScore < 2 ? 1f : 1f * (1 + timeBetweenClick);
    }
}
