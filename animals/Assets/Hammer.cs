using UnityEngine;
using System.Collections;

public class Hammer : MonoBehaviour {

	public GameObject rotateAround;
	public GameObject hammer;
	public bool mouseControl = true;
	public float mouseSpeed = 0.05f;
	public bool hit;
	private Animator anim;


	// Use this for initialization
	void Start () {
		anim = hammer.GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
		movement ();
		hit = hitObject();
	}

	bool hitObject()
	{
		if (!mouseControl)
			return false;
		if (Input.GetMouseButtonDown (0))
		{
			anim.SetTrigger("hit");
			Debug.Log("hiiiiit");
			return true;
		}

		return false;
	}



	void movement()
	{
		if (!mouseControl)
			return;
		
		float h = mouseSpeed * Input.GetAxis("Mouse X");
		float v = mouseSpeed * Input.GetAxis("Mouse Y");
		if(h != 0 || v != 0)
			transform.RotateAround(rotateAround.transform.position, Vector3.up, 40 * h);
			//transform.position += transform.forward * v;
			transform.position = transform.position + new Vector3(h,0,v); 
	}
}
