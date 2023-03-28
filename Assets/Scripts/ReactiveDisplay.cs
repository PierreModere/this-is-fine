using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactiveDisplay: MonoBehaviour {
  bool hasClickedCreateRoom;
  bool hasClickedJoinRoom;
  // Start is called before the first frame update
  void Start() {
    hasClickedCreateRoom = GameObject.Find("Create").GetComponent < CreateButton > ().hasClickedCreateRoom;
  }

  // Update is called once per frame
  void Update() {
    hasClickedCreateRoom = gameObject.transform.Find("Create").GetComponent < CreateButton > ().hasClickedCreateRoom;

    if (hasClickedCreateRoom || hasClickedJoinRoom) {
      gameObject.SetActive(false);
    } else {
      gameObject.SetActive(true);
    }
  }
}