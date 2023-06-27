using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DuelSelection : MonoBehaviour
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
    GameObject selectedContester;
    string selectedContesterID;

    // Start is called before the first frame update
    private void OnEnable()
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

            if (playersList[i].selectedCharacter != "" && playersList[i].id.ToString() != GameData.playerID)
            {
                GameObject playerGameObject = playersGameobjects.Find(g => g.name == playersList[i].selectedCharacter);
                playerGameObject.SetActive(true);

                string id = playersList[i].id.ToString();
                playerGameObject.GetComponent<Button>().onClick.AddListener(() => { selecteContester(id, playerGameObject); });


                GameObject PlayerNumber = playerGameObject.transform.Find("PlayerNumber").gameObject;
                PlayerNumber.GetComponent<Image>().sprite = playerNumberSprites.Find(spr => spr.name == "player" + playersList[i].id);
                PlayerNumber.SetActive(true);

                GameObject Sprite = playerGameObject.transform.Find("Mask").Find("Sprite").gameObject;
                Sprite.GetComponent<Image>().sprite = charactersSprites.Find(sprite => sprite.name == playersList[i].selectedCharacter + "_idle");

                if (playersList[i].isDuel)
                  {
                    playerGameObject.GetComponent<Button>().interactable = false;
                    playerGameObject.GetComponent<Image>().sprite = selectedOutlinesSprites.Find(spr => spr.name == "selectedPlayer" + playersList[i].id.ToString());
                }
            }
        }
    }

    public void selecteContester(string id, GameObject player)
    {

        if (selectedContester != null) selectedContester.GetComponent<Image>().sprite = selectedOutlinesSprites.Find(spr => spr.name == "characterBackgroundCircle");
        isSelected = true;
        selectedContesterID = id;
        selectedContester = player;
        selectedContester.GetComponent<Image>().sprite = selectedOutlinesSprites.Find(spr => spr.name == "selectedPlayer" + id);


        GameObject Sprite = selectedContester.transform.Find("Mask").Find("Sprite").gameObject;
        Debug.Log(selectedContester.name);
        Sprite.GetComponent<Image>().sprite = selectedCharactersSprites.Find(sprite => sprite.name == selectedContester.name + "_selected");

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
        selectedContester.GetComponent<Image>().sprite = selectedOutlinesSprites.Find(spr => spr.name == "characterBackgroundCircle");
        selectedContester.transform.Find("Mask").Find("Sprite").GetComponent<Image>().sprite = selectedCharactersSprites.Find(sprite => sprite.name == selectedContester.name + "_idle");

        isSelected = false;
        selectedContester = null;
        selectedContesterID = null;

        WebsocketManager = GameObject.Find("WebsocketManager");

        WebsocketManager.GetComponent<WebsocketManager>().resetDuelStatus();
        GameData.isDuelHost = false;

        toggleOkButton();
    }

    public async void confirmSelection()
    {
        if (selectedContesterID != null && selectedContesterID != "")
        {
            WebsocketManager = GameObject.Find("WebsocketManager");

            var websocket = WebsocketManager.GetComponent<WebsocketManager>().websocket;

            string json = "{'type': 'selectDuelContester', 'params':{'code': '" + GameData.joinedRoomCode + "','id':'" + selectedContesterID + "'}}";
            await websocket.SendText(json);

            GameData.isDuelHost = true;

            isSelected = false;
            selectedContester.GetComponent<Image>().sprite = selectedOutlinesSprites.Find(spr => spr.name == "characterBackgroundCircle");
            selectedContester.transform.Find("Mask").Find("Sprite").GetComponent<Image>().sprite = selectedCharactersSprites.Find(sprite => sprite.name == selectedContester.name + "_idle");
            selectedContester = null;
            selectedContesterID = null;
            toggleOkButton();
        }
    }
}
