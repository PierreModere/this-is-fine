using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class JoinButton: MonoBehaviour {
  private GameObject Camera;
  private GameObject PinInput;
  public bool hasClickedJoinRoom;

  void Start() {
    Camera = GameObject.Find("MainCamera");
    hasClickedJoinRoom = false;
    PinInput = GameObject.Find("PincodeInput");
    PinInput.SetActive(false);
  }

  void Update() {
    if (GameObject.Find("Create")) {
      bool hasClickedCreateRoom = GameObject.Find("Create").GetComponent < CreateButton > ().hasClickedCreateRoom;
      if (hasClickedCreateRoom) {
        gameObject.SetActive(false);
        PinInput.SetActive(false);
      } else {
        gameObject.SetActive(true);
      }
    }
  }

  public void onClick() {
    if (!hasClickedJoinRoom) {
      hasClickedJoinRoom = true;
      PinInput.SetActive(true);

    } else {
      var pincode = PinInput.GetComponent < TMP_InputField > ().text.ToUpper();
      Camera.GetComponent < Connexion > ().JoinRoom(pincode);
    }
  }
}