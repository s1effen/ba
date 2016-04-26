using UnityEngine;
using System.Collections;

public class Hammer : MonoBehaviour {

	public GameObject rotateAround;
	public GameObject hammer;
	public bool mouseControl = true;
	public float mouseSpeed = 0.05f;
	public float yDistanceToTableForHit = 0.01f;
	public float yDistanceToTableForNoHit = 0.5f;
	public float yDistanceToTable;
	public bool hit;
	public bool hitEnabled = false;
	private bool calibrated = false;

	private Animator anim;
	public float yBase;
	// Use this for initialization
	void Start () {
		anim = hammer.GetComponent<Animator> ();
		yBase = hammer.transform.position.y;
		hitEnabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		movement ();

		if (calibrated && (yDistanceToTable > yDistanceToTableForNoHit))
			hitEnabled = true;
		
		hit = hitObject();
		yDistanceToTable = Mathf.Abs (hammer.transform.position.y - yBase);

		//Calibrate Hammer to table
		if (Input.GetKeyDown ("c")) {
			yBase = hammer.transform.position.y;
			RaycastHit hitPoint;
			if (Physics.Raycast (hammer.transform.position, -Vector3.up, out hitPoint, 20)) {
				Renderer kopfRend = hammer.transform.FindChild ("Kopf").GetComponent<Renderer>();
				float offsetY = kopfRend.bounds.size.y / 2 + hitPoint.point.y;
				Debug.Log ("Hammer calibrated to " + offsetY);
				hammer.transform.position = new Vector3(hammer.transform.position.x,offsetY,hammer.transform.position.z);
				yBase = offsetY;
				yDistanceToTable = Mathf.Abs (hammer.transform.position.y - yBase);
				calibrated = true;
			}
		}

		if (Input.GetKeyDown (KeyCode.E))
			hitEnabled = !hitEnabled;
		


	}

	bool hitObject()
	{
		if (!hitEnabled)
			return false;
		if ((!mouseControl && (yDistanceToTable < yDistanceToTableForHit)) || mouseControl && Input.GetMouseButtonDown (0))
		{
			//anim.SetTrigger("hit");
			Debug.Log("hiiiiit");
			hitEnabled = false;
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
