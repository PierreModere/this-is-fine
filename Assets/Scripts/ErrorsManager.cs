using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ErrorsManager : MonoBehaviour
{
    public List<GameObject> errorGameobjectList;

    public void manageErrors(string error)
    {
        Debug.Log(error);
        GameObject activeErrorCanvas;
        if (error.Contains("Unable to connect to the remote server") || error.Contains("WebSocket error"))
        {
            gameObject.SetActive(true);
            activeErrorCanvas = errorGameobjectList.Find(obj => obj.name == "NoWSConnexion");
            activeErrorCanvas.SetActive(true);
        }
        if (error.Contains("is full") || error.Contains("not exist") || error.Contains("The host has closed the room"))
        {
            gameObject.SetActive(true);
            activeErrorCanvas = errorGameobjectList.Find(obj => obj.name == "RoomError");
            activeErrorCanvas.SetActive(true);
            activeErrorCanvas.transform.Find("RoomErrorText").gameObject.GetComponent<TextMeshProUGUI>().text = error;
        }
    }
}