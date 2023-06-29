using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DashboardCanvas : MonoBehaviour
{
    public GameData GameData;

    public GameObject BattleButton;
    public GameObject DuelButton;
    public GameObject EndGameButton;

    public GameObject CancelButton;
    public GameObject OkButton;

    public GameObject DashboardSubtitle;
    public GameObject WinnerSelectionCanvas;

    private GameObject WebsocketManager;


    bool isHost;
    bool isSelected;
    GameObject selectedButton;
    string selectedMode;

    void Start()
    {

        if (GameData.isHost)
        {

            BattleButton.GetComponent<Button>().interactable = true;
            EndGameButton.GetComponent<Button>().interactable = true;

            foreach (Transform child in BattleButton.transform.Find("Mask"))
            {
                if (child.gameObject.GetComponent<Image>())
                {
                    child.gameObject.GetComponent<Image>().DOFade(1, 0);
                }
            }

            foreach (Transform child in EndGameButton.transform.Find("Mask"))
            {
                if (child.gameObject.GetComponent<Image>())
                {
                    child.gameObject.GetComponent<Image>().DOFade(1, 0);
                }
            }

            BattleButton.GetComponent<Button>().onClick.AddListener(() => { buttonOnClick("Battle", BattleButton); });
            EndGameButton.GetComponent<Button>().onClick.AddListener(() => { buttonOnClick("EndGame", EndGameButton); });
        }

        else
        {
            BattleButton.GetComponent<Button>().interactable = false;
            EndGameButton.GetComponent<Button>().interactable = false;

            foreach (Transform child in BattleButton.transform.Find("Mask"))
            {
                if (child.gameObject.GetComponent<Image>())
                {
                    child.gameObject.GetComponent<Image>().DOFade(0.3f, 0);
                }
            }

            foreach (Transform child in EndGameButton.transform.Find("Mask"))
            {
                if (child.gameObject.GetComponent<Image>())
                {
                    child.gameObject.GetComponent<Image>().DOFade(0.3f, 0);
                }
            }
        }

        DuelButton.GetComponent<Button>().onClick.AddListener(() => { buttonOnClick("Duel", DuelButton); });

    }

    private void OnEnable()
    {
        toggleBottomButtons();
        updateSubtitle();
        buttonAppearance();
        resetButtonAnimations();
    }

    void buttonAppearance()
    {
        Sequence buttonsUp = DOTween.Sequence();

        buttonsUp.SetDelay(0.25f);
        buttonsUp.Append(BattleButton.transform.DOLocalMoveY(BattleButton.transform.localPosition.y-30, 0.2f).SetEase(Ease.OutBack).From());
        buttonsUp.Join(DuelButton.transform.DOLocalMoveY(DuelButton.transform.localPosition.y - 35, 0.15f).SetEase(Ease.OutBack).From());
        buttonsUp.Join(EndGameButton.transform.DOLocalMoveY(EndGameButton.transform.localPosition.y - 35, 0.15f).SetEase(Ease.OutBack).From());

    }
    void resetButtonAnimations()
    {
        BattleButton.transform.Find("Mask").Find("ButtonAnim").gameObject.GetComponent<Animator>().Play("battleUnselected");
        DuelButton.transform.Find("Mask").Find("ButtonAnim").gameObject.GetComponent<Animator>().Play("battleUnselected");
        EndGameButton.transform.Find("Mask").Find("ButtonAnim").gameObject.GetComponent<Animator>().Play("battleUnselected");
    }

    void toggleBottomButtons()
    {

        CancelButton.GetComponent<Button>().interactable = isSelected;
        OkButton.GetComponent<Button>().interactable = isSelected;
    }

    public void buttonOnClick(string mode, GameObject button)
    {
        if (selectedButton != null) { 
            selectedButton.transform.Find("Selected").gameObject.SetActive(false);
            selectedButton.transform.Find("Mask").Find("ButtonAnim").gameObject.GetComponent<Animator>().Play(selectedMode.ToLower() + "Unselection");
            if (selectedButton.transform.Find("Mask").Find("ButtonBackground") != null)
            {
                Transform spike = selectedButton.transform.Find("Mask").Find("ButtonBackground");
                spike.DORotate(new Vector3(0, 0, spike.localEulerAngles.z + 90), 2f).SetEase(Ease.OutSine);
            }
        }
      
        isSelected = true;
        selectedMode = mode;
        selectedButton = button;
        if (selectedButton.transform.Find("Mask").Find("ButtonBackground") != null)
        {
            Transform spike = selectedButton.transform.Find("Mask").Find("ButtonBackground");
            spike.DORotate(new Vector3(0, 0, spike.localEulerAngles.z - 90), 3f).SetEase(Ease.OutSine);
        }
        Transform selectedFrame = selectedButton.transform.Find("Selected");
        selectedFrame.gameObject.SetActive(isSelected);
        selectedFrame.DOScale(1.20f, 0.2f).SetEase(Ease.OutBack).From();
        selectedButton.transform.Find("Mask").Find("ButtonAnim").gameObject.GetComponent<Animator>().Play(selectedMode.ToLower()+"Selection");
        updateSubtitle();
        toggleBottomButtons();
    }

    public void cancelSelection()
    {
        isSelected = false;
        selectedButton.transform.Find("Selected").gameObject.SetActive(isSelected);
        selectedButton.transform.Find("Mask").Find("ButtonAnim").gameObject.GetComponent<Animator>().Play(selectedMode.ToLower() + "Unselection");
        if (selectedButton.transform.Find("Mask").Find("ButtonBackground") != null)
        {
            Transform spike = selectedButton.transform.Find("Mask").Find("ButtonBackground");
            spike.DORotate(new Vector3(0, 0, spike.localEulerAngles.z + 90), 2f).SetEase(Ease.OutSine);
        }
        selectedButton = null;
        selectedMode = null;
        updateSubtitle();
        toggleBottomButtons();
    }

    public void confirmSelection()
    {
        if (selectedMode != null && selectedMode != "")
        {
            WebsocketManager = GameObject.Find("WebsocketManager");

            switch (selectedMode)
            {
                case "Duel":
                    WebsocketManager.GetComponent<WebsocketManager>().sendMinigameMode(selectedMode, GameData.playerID);
                    break;
                case "Battle":
                    WebsocketManager.GetComponent<WebsocketManager>().sendMinigameMode(selectedMode, null);
                    break;
                case "EndGame":
                    gameObject.GetComponent<ToggleMenu>().enableMenu();
                    gameObject.GetComponent<ToggleMenu>().disableMenu();
                    break;
            }

            isSelected = false;
            selectedButton.transform.Find("Selected").gameObject.SetActive(isSelected);
            selectedButton = null;
            selectedMode = null;
        }
    }

    void updateSubtitle()
    {
        TextMeshProUGUI subtitleText = DashboardSubtitle.GetComponent<TextMeshProUGUI>();
        DashboardSubtitle.transform.DOScale(1.2f, 0.15f).SetEase(Ease.OutBack).From();
        switch (selectedMode)
        {
            case "Battle":
                subtitleText.text = "C�est l�heure de la bataille ?";
                break;
            case "Duel":
                subtitleText.text = "Tu provoques un duel ?";
                break;
            case "EndGame":
                subtitleText.text = "L'heure a sonn� ?";
                break;
            case null:
                subtitleText.text = "Que se passe-t-il ?";
                break;
        }
    }

}
