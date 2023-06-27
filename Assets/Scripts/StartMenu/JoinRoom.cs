using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
                string buttonValue = childObject.name;

                if (buttonValue != "Delete" && buttonValue != "Return")
                {
                    childButton.onClick.AddListener(delegate {
                        addDigitToPin(buttonValue);
                    });
                }
                if (buttonValue == "Delete")
                {
                    childButton.onClick.AddListener(deleteLastDigit);
                }
            }
        }

    }

    void addDigitToPin(string buttonValue)
    {
        if (PinInput.GetComponent<TextMeshProUGUI>().text.Length < 4 || PinInput.GetComponent<TextMeshProUGUI>().text == "----")
        {
            if (PinInput.GetComponent<TextMeshProUGUI>().text == "----") PinInput.GetComponent<TextMeshProUGUI>().text = "" + buttonValue;
            else PinInput.GetComponent<TextMeshProUGUI>().text = PinInput.GetComponent<TextMeshProUGUI>().text + buttonValue;


            if (PinInput.GetComponent<TextMeshProUGUI>().text.Length > 3 && PinInput.GetComponent<TextMeshProUGUI>().text != "----")
            {
                joinRoom();
            }

        }
    }

    void deleteLastDigit()
    {
        if (PinInput.GetComponent<TextMeshProUGUI>().text.Length > 0 && PinInput.GetComponent<TextMeshProUGUI>().text != "----")
        {
            PinInput.GetComponent<TextMeshProUGUI>().text = PinInput.GetComponent<TextMeshProUGUI>().text.Remove(PinInput.GetComponent<TextMeshProUGUI>().text.Length - 1);
            if (PinInput.GetComponent<TextMeshProUGUI>().text.Length == 0)
            {
                PinInput.GetComponent<TextMeshProUGUI>().text = "----";
            }
        }

    }

    async public void joinRoom()
    {
        WebsocketManager = GameObject.Find("WebsocketManager");
        string pincode = PinInput.GetComponent<TextMeshProUGUI>().text.ToUpper();
        var websocket = WebsocketManager.GetComponent<WebsocketManager>().websocket;
        string json = "{'type': 'join', 'params':{'code': '" + pincode + "'}}";
        await websocket.SendText(json);
    }

    void OnDisable()
    {
        PinInput.GetComponent<TextMeshProUGUI>().text = "----";
    }

}