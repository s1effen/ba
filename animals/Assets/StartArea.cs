using UnityEngine;
using System.Collections;

public class StartArea : MonoBehaviour {

	public GameObject hammer;
	public Manager manager;
	public Material active;
	public Material inactive;
	public MeshRenderer mr;
	public bool free = true;
	bool visible = false;
	// Use this for initialization
	void Start () {
		mr = GetComponent<MeshRenderer> ();
		manager = GameObject.Find ("GameManager").GetComponent<Manager> ();
		hammer = manager.getHammer();
	}
	
	// Update is called once per frame
	void Update () {
			if (mr.bounds.Contains (hammer.transform.position)) {
				mr.material = active;
				free = false;
			} else {
				mr.material = inactive;
				free = true;
			}
		}
}
