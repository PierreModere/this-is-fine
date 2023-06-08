using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class Minigame2 : MonoBehaviour
{
    bool isPlaying = false;
    int playerScore = 0;
    int touching = 0;
    public GameObject contractsList;
    public GameObject contractPrefab;

    public GameObject leftArm;
    public GameObject rightArm;

    public GameData GameData;
    public MinigameUI MinigameUI;
    private GameObject WebsocketManager;

    Vector3 spawnPosition = new Vector3(0, 0, 0);
    float intervalBetweenClick;
    public float speed;
    Vector3 rightArmDefaultPos;

    public void initMinigame()
    {
        WebsocketManager = GameObject.Find("WebsocketManager");
        rightArmDefaultPos = rightArm.transform.position;
        isPlaying = true;
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
        //rightArm.transform.localPosition = new Vector3(rightArmDefaultPos.x * Mathf.Sin(speed * Time.time), rightArmDefaultPos.y * Mathf.Sin(speed * Time.time), 0);


        if (Input.touchCount == 1)
        {
            touching++;

            if (touching == 1 && isPlaying)
            {
                leftArm.transform.DORotate(new Vector3(0, 0, -30), 0.1f);

                GameObject currentContractObject = contractsList.transform.Find(playerScore.ToString()).gameObject;

                // Increase player score
                playerScore++;
                sendScore();
                MinigameUI.displayFeedback(true);


                // Duplicate contract GameObject
                GameObject newContractObject = Instantiate(contractPrefab, new Vector3(0,0,0), Quaternion.identity, contractsList.transform);
                newContractObject.transform.localPosition = spawnPosition;
                newContractObject.transform.SetAsFirstSibling();
                newContractObject.name = playerScore.ToString();


                GameObject stamp = currentContractObject.transform.Find("Stamp").gameObject;
                stamp.SetActive(true);

                Sequence stampedContractAnim = DOTween.Sequence();
                stampedContractAnim.Append(stamp.transform.DOScale(1f, 0.25f).SetEase(Ease.OutElastic));
                stampedContractAnim.Append(currentContractObject.transform.DOLocalMoveX(-1080f, 0.3f));
                stampedContractAnim.Join(currentContractObject.transform.DORotate(new Vector3(0, 0, 25), 0.3f));
                stampedContractAnim.Join(leftArm.transform.DORotate(new Vector3(0, 0, 20), 0.2f));

                stampedContractAnim.OnComplete(() => {
                    Debug.Log("Destroy contract : " + currentContractObject.name);
                    Destroy(currentContractObject.gameObject);
                    leftArm.transform.DORotate(new Vector3(0, 0, -30), 0.1f);
                }); 
            }
        }
        else if (Input.touchCount == 0)
        {
            touching = 0;
        }
    }
}
