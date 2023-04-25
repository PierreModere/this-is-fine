using TMPro;
using UnityEngine;

public class PlayerNumber : MonoBehaviour
{
    public bool isLocalClient = false;


    // Update is called once per frame
    void Update()
    {
        if (isLocalClient)
        {
            gameObject.GetComponent<TextMeshProUGUI>().color = new Color(0.3176471f, 0.9686275f, 1f);
        }
        else
        {
            gameObject.GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 1f);

        }
    }
}