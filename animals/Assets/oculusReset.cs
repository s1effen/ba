using UnityEngine;
using System.Collections;

public class oculusReset : MonoBehaviour {

	Manager manager;
	// Use this for initialization
	void Start () {
		manager = GameObject.Find ("GameManager").GetComponent<Manager> ();

		//object temp = Persistent.getValue (this.gameObject.name + "-VRCenter");
		//if (temp != null)
		//	transform.position = transform.position - (Vector3)temp;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey ("r") || OVRInput.GetDown(OVRInput.Button.DpadDown) && manager.condition == Manager.Condition.INIT){
			UnityEngine.VR.InputTracking.Recenter ();
			//Persistent.setValue (this.gameObject.name + "-VRCenter", UnityEngine.VR.InputTracking.GetLocalPosition (UnityEngine.VR.VRNode.CenterEye));
			//Persistent.setValue (this.gameObject.name + "-Pos", transform.position);
	}
	}
}