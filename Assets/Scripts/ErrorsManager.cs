using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ErrorsManager : MonoBehaviour
{
    public List<GameObject> errorGameobjectList;

    public void manageErrors(string error)
    {
        GameObject activeErrorCanvas;
        if (error.Contains("is full") || error.Contains("not exist") || error.Contains("The host has closed the room"))
        {
            gameObject.SetActive(true);
            activeErrorCanvas = errorGameobjectList.Find(obj => obj.name == "RoomError");
            activeErrorCanvas.SetActive(true);
            activeErrorCanvas.transform.Find("Text").gameObject.GetComponent<TextMeshProUGUI>().text = error;
            activeErrorCanvas.transform.Find("OkButton").gameObject.GetComponent<Button>().onClick.AddListener(delegate {
                activeErrorCanvas.transform.Find("OkButton").gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
                activeErrorCanvas.SetActive(false);
                gameObject.SetActive(false);
            });

        }
    }
}