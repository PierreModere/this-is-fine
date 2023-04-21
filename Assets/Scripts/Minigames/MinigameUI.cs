using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameUI : MonoBehaviour
{
    // Start is called before the first frame update

    public Minigame minigameData;

    private GameObject WebsocketManager;


    public bool isPlaying;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void endMinigame()
    {
        WebsocketManager = GameObject.Find("WebsocketManager");
        var websocket = WebsocketManager.GetComponent<WebsocketManager>();
        websocket.endMinigame();

    }
}
