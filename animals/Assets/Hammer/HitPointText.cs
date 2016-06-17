using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class HitPointText : MonoBehaviour {

	public bool animation = true;
	public int speed = 2;
	private Text hitPointText;

	// Use this for initialization
	void Start () {
		hitPointText = GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (!animation)
			return;
		if (hitPointText.fontSize > 40) {
			hitPointText.fontSize -= speed;
		} else {
			Destroy (this.gameObject);
		}
	}
}
