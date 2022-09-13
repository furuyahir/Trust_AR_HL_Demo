using UnityEngine;
using System.Collections;

public class KeyHandler : MonoBehaviour {

	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			Application.Quit();
		}
				if (Input.GetKey (KeyCode.LeftShift)) {
						GetComponent<MouseLook> ().enabled = true;
				} else {
						GetComponent<MouseLook> ().enabled = false;
				}
	}
}
