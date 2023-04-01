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
/*        menuToDisable.transform.DOMoveX(-425, 3);
*/      menuToDisable.SetActive(false);
    }
    public void enableMenu()
    {
        menuToEnable.SetActive(true);
    }
}
