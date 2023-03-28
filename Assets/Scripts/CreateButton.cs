using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CreateButton: MonoBehaviour {
  private GameObject Camera;
  public bool hasClickedCreateRoom;

  void Start() {
    Camera = GameObject.Find("MainCamera");
    hasClickedCreateRoom = false;
  }

  void Update() {
    if (GameObject.Find("Join")) {
      bool hasClickedJoinRoom = GameObject.Find("Join").GetComponent < JoinButton > ().hasClickedJoinRoom;
      if ((hasClickedCreateRoom || hasClickedJoinRoom) && gameObject.activeSelf) {
        gameObject.SetActive(false);
      }
    }
  }

  public void onClick() {
    Camera.GetComponent < Connexion > ().CreateNewRoom();
    hasClickedCreateRoom = true;
  }
}