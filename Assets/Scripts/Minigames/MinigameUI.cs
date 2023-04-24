using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MinigameUI : MonoBehaviour
{
    // Start is called before the first frame update

    public Minigame minigameData;
    public bool isPlaying;

    public GameObject TimerGameobject;
    TextMeshProUGUI TimerUI;
    float timeLeft;
    bool isCutscene = true;
    float cutsceneTimeLeft;

    private GameObject WebsocketManager;

    void Start()
    {
        cutsceneTimeLeft = minigameData.cutsceneTime;
        timeLeft = minigameData.gameTime;
        TimerUI = TimerGameobject.GetComponent<TextMeshProUGUI>();
        TimerUI.text= timeLeft.ToString();
    }

    void Update()
    {
       if (isCutscene)
        {
            if (cutsceneTimeLeft > 0)
            {
                cutsceneTimeLeft -= Time.deltaTime;
            }
            else
            {
                Debug.Log("Fin de la cutscene !");
                cutsceneTimeLeft = 0;
                isCutscene = false;
                isPlaying = true;
            }
        }
       if (isPlaying)
        {
            if (timeLeft > 0)
            {
                timeLeft -= Time.deltaTime;
                updateTimer(timeLeft);
            }
            else
            {
                Debug.Log("Time is UP!");
                timeLeft = 0;
                isPlaying = false;
            }
        }
    }
          

    void updateTimer(float currentTime)
    {
        currentTime += 1;

        float seconds = Mathf.FloorToInt(currentTime % 60);

        TimerUI.text = seconds.ToString();
    }

    public void endMinigame()
    {
        WebsocketManager = GameObject.Find("WebsocketManager");
        var websocket = WebsocketManager.GetComponent<WebsocketManager>();
        websocket.endMinigame();

    }
}
