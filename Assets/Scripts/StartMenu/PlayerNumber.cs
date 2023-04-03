using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerNumber : MonoBehaviour
{
    public bool isClient;

    // Start is called before the first frame update
    void Start()
    {
        isClient = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isClient)
        {
            gameObject.GetComponent<TextMeshProUGUI>().color = new Color(0.3176471f, 0.9686275f, 1f);
        }
        else
        {
            gameObject.GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 1f);

        }
    }
}