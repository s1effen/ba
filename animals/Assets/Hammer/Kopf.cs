using UnityEngine;
using System.Collections;

public class Kopf : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other) {
		GetComponentInParent<Hammer> ().touchedObject (other.gameObject);
	}
}
