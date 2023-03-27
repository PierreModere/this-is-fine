using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CreateButton : MonoBehaviour {
	public Button yourButton;
    public GameObject Camera;

	void Start () {
		Button btn = yourButton.GetComponent<Button>();
		btn.onClick.AddListener(TaskOnClick);
	}

	void TaskOnClick(){
        Debug.Log(Camera.GetComponent<Connexion>());
        Camera.GetComponent<Connexion>().IncrementInteger();
	}
}