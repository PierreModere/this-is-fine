using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinnerSelection : MonoBehaviour
{
    public GameData GameData;

    public GameObject ReturnButton;
    public GameObject OkButton;

    public List<Sprite> charactersSprites;

    [SerializeField]
    private List<Sprite> selectedCharactersSprites;

    public List<Sprite> playerNumberSprites;

    public List<GameObject> playersGameobjects;

    [SerializeField]
    private List<Sprite> selectedOutlinesSprites;


    private GameObject WebsocketManager;

    bool isDuel;
    bool isSelected;
    GameObject selectedWinner;
    string selectedWinnerID;

    // Start is called before the first frame update
    void Start()
    {
        displayPlayers();

    }

    void unactivePlayersGameobjects()
    {
        foreach (GameObject playerGO in playersGameobjects)
        {
            playerGO.SetActive(false);
            playerGO.GetComponent<Button>().interactable = true;
        }
    }

    void displayPlayers()
    {
        unactivePlayersGameobjects();
        var playersList = GameData.playersList;

        for (int i = 0; i < playersList.Count; i++)
        {

            if (playersList[i].selectedCharacter != "")
            {
                GameObject playerGameObject = playersGameobjects.Find(g => g.name == playersList[i].selectedCharacter);
                playerGameObject.SetActive(true);

                string id = playersList[i].id.ToString();
                playerGameObject.GetComponent<Button>().onClick.AddListener(() => { selectWinner(id, playerGameObject); });


                GameObject PlayerNumber = playerGameObject.transform.Find("PlayerNumber").gameObject;
                PlayerNumber.GetComponent<Image>().sprite = playerNumberSprites.Find(spr => spr.name == "player" + playersList[i].id);
                PlayerNumber.SetActive(true);

                GameObject Sprite = playerGameObject.transform.Find("Mask").Find("Sprite").gameObject;
                Sprite.GetComponent<Image>().sprite = charactersSprites.Find(sprite => sprite.name == playersList[i].selectedCharacter + "_idle");
            }
        }
    }

    public void selectWinner(string id, GameObject player)
    {

        if (selectedWinner != null) selectedWinner.GetComponent<Image>().sprite = selectedOutlinesSprites.Find(spr => spr.name == "characterBackgroundCircle");
        isSelected = true;
        selectedWinnerID = id;
        selectedWinner = player;
        selectedWinner.GetComponent<Image>().sprite = selectedOutlinesSprites.Find(spr => spr.name == "selectedPlayer" + id);


        GameObject Sprite = selectedWinner.transform.Find("Mask").Find("Sprite").gameObject;
        Sprite.GetComponent<Image>().sprite = selectedCharactersSprites.Find(sprite => sprite.name == selectedWinner.name + "_selected");

        Sequence anim = DOTween.Sequence();
        anim.Append(Sprite.transform.DOLocalMoveY(20, 0.1f).SetEase(Ease.OutElastic).From());
        anim.Join(Sprite.transform.DOScale(1.2f, 0.15f).From());

        toggleOkButton();

    }

    void toggleOkButton()
    {
        OkButton.GetComponent<Button>().interactable = isSelected;
    }

    public void cancelSelection()
    {
        isSelected = false;
        selectedWinner = null;
        selectedWinnerID = null;

        WebsocketManager = GameObject.Find("WebsocketManager");

        WebsocketManager.GetComponent<WebsocketManager>().resetDuelStatus();
        GameData.isDuelHost = false;

        toggleOkButton();
    }

    public async void confirmSelection()
    {
        if (selectedWinnerID != null && selectedWinnerID != "")
        {
            WebsocketManager = GameObject.Find("WebsocketManager");

            var websocket = WebsocketManager.GetComponent<WebsocketManager>().websocket;

            string json = "{'type': 'selectWinner', 'params':{'code': '" + GameData.joinedRoomCode + "','id':'" + selectedWinnerID + "'}}";
            await websocket.SendText(json);

            isSelected = false;
            selectedWinner.SetActive(isSelected);
            selectedWinner = null;
            selectedWinnerID = null;
            toggleOkButton();
        }
    }
}
