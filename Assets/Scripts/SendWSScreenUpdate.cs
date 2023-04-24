using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendWSScreenUpdate : MonoBehaviour
{
    [SerializeField]
    private GameObject ScreenToSwitchTo;
    private GameObject WebsocketManager;
   
    public void sendScreenNameToSwitchTo()
    {
        if (ScreenToSwitchTo != null)
        {
            WebsocketManager = GameObject.Find("WebsocketManager");
            WebsocketManager.GetComponent<WebsocketManager>().sendScreenNameToSwitchTo(ScreenToSwitchTo);
        }
    }
}
