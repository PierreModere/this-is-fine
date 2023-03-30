using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject menuToDisable;
    [SerializeField]
    private GameObject menuToEnable;

    public void disableMenu()
    {
        menuToDisable.SetActive(false);
    }
    public void enableMenu()
    {
        menuToEnable.SetActive(true);
    }
}
