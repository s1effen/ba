using UnityEngine;
using System.Collections;

public class oculusReset : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	if(Input.GetKey ("r")){
			UnityEngine.VR.InputTracking.Recenter ();
	}
	}
}