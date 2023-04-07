using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErrorsManager : MonoBehaviour
{
    public List<GameObject> errorGameobjectList;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

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
        /*    switch (error)
            {
                case error.Contains("Unable to connect to the remote server") || error.Contains("WebSocket failed"):
                    gameObject.SetActive(true);
                    activeErrorCanvas = errorGameobjectList.Find(obj => obj.name == "NoWSConnexion");
                    activeErrorCanvas.SetActive(true);
                    break;
            }*/
    }
}