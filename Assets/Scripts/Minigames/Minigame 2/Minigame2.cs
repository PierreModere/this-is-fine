using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

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

        // Shake background canvas
        Sequence shakeBackground = DOTween.Sequence();

        shakeBackground.Append(backgroundCanvas.transform.DORotate(new Vector3(0f, 0f, 25f), 0.2f));
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
                MinigameUI.displayFeedback(true);

                // Duplicate contract GameObject
                GameObject newContractObject = Instantiate(currentContractObject, new Vector3(0,0,0), Quaternion.identity, contractsList.transform);
                newContractObject.transform.localPosition = new Vector3(0, 0, 0);
                newContractObject.transform.SetAsFirstSibling();
                newContractObject.name = playerScore.ToString();
                int randomSpriteIndex = Random.Range(0, contractsSprites.Count);
                newContractObject.GetComponent<Image>().sprite = contractsSprites[randomSpriteIndex];

                // Stamp current contract and remove it
                AnimateStamp();
            }
        }
        else if (Input.touchCount == 0)
        {
            touching = 0;
        }
    }

    void AnimateStamp()
    {
        Sequence stampedContractAnim = DOTween.Sequence();

        stampedContractAnim.OnStart(() =>
        {
            isAnimating = true;
            Debug.Log("Animation starts! (" + isAnimating + ")");
        });

        // Right hand going down and display stamp
        stampedContractAnim.Append(rightHandGroup.transform.DOScale(new Vector3(0.9f, 0.9f, 0.9f), 0.2f).SetEase(Ease.InQuint))
            .Join(rightHandShadow.transform.DOScale(new Vector3(1f, 1f, 1f), 0.2f).SetEase(Ease.InQuint))
            .Join(rightHandGroup.transform.DOMoveX(rightHandPosition.x - 60f, 0.2f).SetEase(Ease.InQuint))
            .AppendCallback(() =>
            {
                stamp.SetActive(true);
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
            .Join(currentContractObject.transform.DORotate(new Vector3(0f, 0f, 25f), 0.2f))
            .Join(currentContractObject.transform.DOMoveX(currentContractObject.transform.position.x - 900f, 0.4f));
        
        // Left hand going up
        stampedContractAnim.Insert(0.6f, leftHandGroup.transform.DOScale(new Vector3(1f, 1f, 1f), 0.2f))
            .Join(leftHandShadow.transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.2f))
            .Join(leftHandGroup.transform.DORotate(new Vector3(0f, 0f, 0f), 0.2f))
            .Join(leftHandGroup.transform.DOMoveX(leftHandPosition.x, 0.2f));
        
        // Destroy current contract when the animation is complete
        stampedContractAnim.OnComplete(() =>
        {
            Destroy(currentContractObject.gameObject);

            isAnimating = false;
            Debug.Log("Animation complete! (" + isAnimating + ")");
        });
        
        // Use timeScale setting to accelerate animation (default 1f, quicker >1f, slower <1f)
        stampedContractAnim.timeScale = 1f;
    }
}
