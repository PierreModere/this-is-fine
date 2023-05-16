using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minigame2 : MonoBehaviour
{
    int playerScore = 0;
    int touching = 0;
    public GameObject contractsList;
    public GameData GameData;
    private GameObject WebsocketManager;

    public void initMinigame()
    {
        WebsocketManager = GameObject.Find("WebsocketManager");
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

            if (touching == 1)
            {
                // Duplicate contract GameObject
                GameObject contractObject = contractsList.transform.GetChild(0).gameObject;
                Instantiate(contractObject, contractObject.transform.position, Quaternion.identity, contractsList.transform);

                // Destroy contract if more than 2
                if (contractsList.transform.childCount > 2)
                {
                    Destroy(contractsList.transform.GetChild(0).gameObject);
                }

                // Increase player score
                playerScore++;
                sendScore();
                Debug.Log("Score: " + playerScore);
            }
        }
        else if (Input.touchCount == 0)
        {
            touching = 0;
        }
    }
}
