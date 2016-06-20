using UnityEngine;
using System.Collections;

public class StartArea : MonoBehaviour {

	public GameObject hammer;
	public Manager manager;
	public ViewManager viewManager;
	public Material active;
	public Material inactive;
	public MeshRenderer mr;
	public bool free = true;
	bool visible = false;
	// Use this for initialization
	void Start () {
		mr = GetComponent<MeshRenderer> ();
		manager = GameObject.Find ("GameManager").GetComponent<Manager> ();
		viewManager = GameObject.Find ("ViewManager").GetComponent<ViewManager> ();
		hammer = manager.getHammer();
	}
	
	// Update is called once per frame
	void Update () {
			if (mr.bounds.Contains (hammer.transform.position)) {
				mr.material = active;
				free = false;
				viewManager.setSubWarning ("Please move the hammer out of the box.");
			} else {
				mr.material = inactive;
				free = true;
				viewManager.resetSubText();

			}
		}
}
