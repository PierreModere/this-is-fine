using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuelSelection : MonoBehaviour
{
    public GameObject ReturnButton;
    public GameObject OkButton;

    public List<Sprite> charactersSprites;
    private List<Sprite> selectedFramesSprites;

    private GameObject WebsocketManager;

    bool isDuel;
    bool isSelected;
    GameObject selectedContester;
    string selectedMinigameID;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
