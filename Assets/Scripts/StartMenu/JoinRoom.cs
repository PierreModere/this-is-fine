using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class JoinRoom : MonoBehaviour
{
    [SerializeField]
    private GameObject PinInput;
    [SerializeField]
    private GameObject joinButton;
    private GameObject WebsocketManager;

    void Start()
    {
        WebsocketManager = GameObject.Find("WebsocketManager");
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

                if (buttonValue != "join" && buttonValue != "delete")
                {
                    childButton.onClick.AddListener(delegate {
                        addDigitToPin(buttonValue);
                    });
                }
                if (buttonValue == "delete")
                {
                    childButton.onClick.AddListener(deleteLastDigit);
                }

                if (buttonValue == "join")
                {
                    childButton.onClick.AddListener(joinRoom);
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
            joinButton.SetActive(true);
        }

    }

    void deleteLastDigit()
    {
        if (PinInput.GetComponent<TMP_InputField>().text.Length > 0)
        {
            PinInput.GetComponent<TMP_InputField>().text = PinInput.GetComponent<TMP_InputField>().text.Remove(PinInput.GetComponent<TMP_InputField>().text.Length - 1);
            joinButton.SetActive(false);
        }

    }

    async public void joinRoom()
    {
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