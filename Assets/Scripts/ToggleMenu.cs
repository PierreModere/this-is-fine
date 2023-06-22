using DG.Tweening;
using UnityEngine;

public class ToggleMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject menuToDisable;
    [SerializeField]
    private GameObject menuToEnable;

    private GameObject planetBg;
    float planetDefaultY = -245;
    float planetTargetY = 465;
    private void Start()
    {
        if (GameObject.Find("background")) planetBg = GameObject.Find("background");
    }

    public void disableMenu()
    {
        if (menuToDisable.name != "StartMenuCanvas")
        {
            menuToDisable.SetActive(false);
            menuToDisable.tag = "Untagged";
        }

        if (menuToEnable.name == "MenuCanvas" && planetBg != null)
        {
            planetBg.transform.DOLocalMoveY(planetDefaultY, 0.8f);
        }
    }
    public void enableMenu()
    {
        menuToEnable.SetActive(true);
        menuToEnable.transform.DOScale(new Vector3(1.05f, 1.05f, 1.05f), 0.1f).OnComplete(() => { menuToEnable.transform.DOScale(new Vector3(1f, 1f, 1f), 0.15f); });
        menuToEnable.tag = "activeScreen";

        if ((menuToEnable.name == "CreateRoomCanvas" || menuToEnable.name == "JoinRoomCanvas") && planetBg!= null)
        {
            planetBg.transform.DOLocalMoveY(planetTargetY, 0.8f);
        }
    }
}
