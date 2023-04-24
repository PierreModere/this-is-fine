using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class JoinRoom : MonoBehaviour
{
    [SerializeField]
    private GameObject PinInput;
    private GameObject WebsocketManager;

    void Start()
    {
        addClickEventOnKeys();
    }

    void addClickEventOnKeys()
    {

        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject childObject = transform.GetChild(i).gameObject;

            Button childButton = childObject.GetComponentInChildren<Button>();

            if (childButton != null)
            {
                string buttonValue = childObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;

                if (buttonValue != "delete" && buttonValue != "RET.")
                {
                    childButton.onClick.AddListener(delegate {
                        addDigitToPin(buttonValue);
                    });
                }
                if (buttonValue == "delete")
                {
                    childButton.onClick.AddListener(deleteLastDigit);
                }
            }
        }

    }

    void addDigitToPin(string buttonValue)
    {
        if (PinInput.GetComponent<TMP_InputField>().text.Length < 4)
        {
            PinInput.GetComponent<TMP_InputField>().text = PinInput.GetComponent<TMP_InputField>().text + buttonValue;
        }

        if (PinInput.GetComponent<TMP_InputField>().text.Length > 3)
        {
            joinRoom();
        }

    }

    void deleteLastDigit()
    {
        if (PinInput.GetComponent<TMP_InputField>().text.Length > 0)
        {
            PinInput.GetComponent<TMP_InputField>().text = PinInput.GetComponent<TMP_InputField>().text.Remove(PinInput.GetComponent<TMP_InputField>().text.Length - 1);
        }

    }

    async public void joinRoom()
    {
        WebsocketManager = GameObject.Find("WebsocketManager");
        string pincode = PinInput.GetComponent<TMP_InputField>().text.ToUpper();
        var websocket = WebsocketManager.GetComponent<WebsocketManager>().websocket;
        string json = "{'type': 'join', 'params':{'code': '" + pincode + "'}}";
        await websocket.SendText(json);
    }

    void OnDisable()
    {
        PinInput.GetComponent<TMP_InputField>().text = "";
    }

}