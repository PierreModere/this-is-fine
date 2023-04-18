using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ToggleMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject menuToDisable;
    [SerializeField]
    private GameObject menuToEnable;

    public void disableMenu()
    {
        menuToDisable.SetActive(false);

        menuToDisable.tag = "Untagged";
    }
    public void enableMenu()
    {
        menuToEnable.SetActive(true);
        menuToEnable.transform.DOScale(new Vector3(1.05f, 1.05f, 1.05f), 0.1f).OnComplete(() => { menuToEnable.transform.DOScale(new Vector3(1f, 1f, 1f), 0.15f); });
        menuToEnable.tag = "activeScreen";
    }
}
