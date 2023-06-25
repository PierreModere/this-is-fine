using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusicManager : MonoBehaviour
{
    [SerializeField] private AudioClip audioClip;
    bool isPlaying = false;
    // Start is called before the first frame update

    private void Awake()
    {
        // means it's in a minigame scene
        if (GameObject.Find("MinigameInfoCanvas"))
        {
            gameObject.GetComponent<AudioSource>().clip = GameObject.Find("MinigameInfoCanvas").GetComponent<MinigameUI>().minigameData.backgroundMusic;
        }
        else
        {
            gameObject.GetComponent<AudioSource>().clip = audioClip;

        }
    }
    void Start()
    {
    
        if (GameObject.Find("WebsocketManager")){
            GameData GameData = GameObject.Find("WebsocketManager").GetComponent<WebsocketManager>().GameData;
            if (GameData != null && GameData.isHost && !isPlaying)
            {
                gameObject.GetComponent<AudioSource>().Play();
            }
        }
      }
    

    private void OnDisable()
    {
        isPlaying = false;
    }
}
