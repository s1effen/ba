﻿using UnityEngine;
using System.Collections;

public class Hammer : MonoBehaviour {

	public GameObject rotateAround;
	public GameObject hammer;
	public GameObject kopf;
	public GameObject tisch;
	public HammerAnim hammerAnim;
	public bool mouseControl = true;
	public float mouseSpeed = 0.05f;
	public float yDistanceToTableForHit = 0.01f;
	public float yDistanceToTableForNoHit = 0.5f;
	public float yDistanceToTable;
	public bool hit = false;
	public bool hitEnabled = false;
	public bool freezed = false;
	private Manager manager;
	private bool calibrated = false;
	private Vector3 target;
	private float baseHeight;
	private float offsetY;
	Vector3 initPosition;
	Quaternion initRotation;
	bool init;

	private Animator anim;
	public float yBase;

	// Use this for initialization
	void Start () {
		initPosition = transform.position;
		initRotation = transform.rotation;
		manager = GameObject.Find ("GameManager").GetComponent<Manager> ();
		Renderer kopfRend = hammer.transform.FindChild ("Kopf").GetComponent<Renderer>();

		offsetY = kopfRend.bounds.size.y / 2;
		baseHeight = transform.position.y;

		anim = hammer.GetComponent<Animator> ();
		if (!mouseControl) {
		object temp = Persistent.getValue ("Hammer-PosLoc");
			if (temp != null) {
				calibrated = true;
				hammer.transform.localPosition = (Vector3)temp;
				temp = Persistent.getValue ("Hammer-yBase");
				if (temp != null) {
					yBase = (float) temp;
					calibrated = true;
				}

			} else {
				yBase = hammer.transform.position.y;
			}
			hitEnabled = false;
		} else {
			hitEnabled = true;
		}
	}
	
	// Update is called once per frame
	void Update () {
		//Calibrate Hammer to table
		if ((Input.GetKeyDown ("c") ||OVRInput.GetDown(OVRInput.Button.Two)) && manager.condition == Manager.Condition.INIT) {
			yBase = hammer.transform.position.y;
			RaycastHit hitPoint;
			if (Physics.Raycast (hammer.transform.position + Vector3.up * 5.0f, -Vector3.up, out hitPoint, 20)) {
				Debug.Log ("Hammer calibrated to " + (hitPoint.point.y + offsetY));
				hammer.transform.position = new Vector3(hammer.transform.position.x,hitPoint.point.y + offsetY,hammer.transform.position.z);
				yBase = hitPoint.point.y + offsetY;
				yDistanceToTable = Mathf.Abs (hammer.transform.position.y - yBase);
				calibrated = true;
				Persistent.setValue ("Hammer-PosLoc", hammer.transform.localPosition);
				Persistent.setValue ("Hammer-yBase", yBase);

			}
		}

		movement ();

		if (calibrated && (yDistanceToTable > yDistanceToTableForNoHit))
			hitEnabled = true;
		
		hitObject();
		yDistanceToTable = Mathf.Abs (hammer.transform.position.y - yBase);

		
		switch (hammerAnim.move) {
		case 4:
			if ((transform.position.y - baseHeight) < 0) {
				transform.position = Vector3.MoveTowards (transform.position, new Vector3 (transform.position.x, baseHeight + offsetY, transform.position.z), 0.5f * Time.deltaTime);
			} else {
				hammerAnim.move = 0;
			}
			break;
		case 2:
			transform.position = Vector3.MoveTowards (transform.position, new Vector3 (transform.position.x, target.y + offsetY, transform.position.z), 0.3f * Time.deltaTime);
			break;
		case 3:
			if (GetComponent<AudioSource> ())
				GetComponent<AudioSource> ().Play ();
			hit = true;
			manager.hit(target);
			hammerAnim.move = 4;
			break;
		default:
			hit = false;
			break;
		}

	}

	public void freeze(){
		hitEnabled = false;
		freezed = true;
	}

	public void unfreeze(){
		//hitEnabled = true;
		freezed = false;
	}

	public void deactivate (){
		hammer.SetActive (false);
		hitEnabled = false;
	}

	public void activate(){
		hammer.SetActive (true);
		hitEnabled = false;
		freezed = false;
	}

	public Vector3 reset(){
		activate ();
		if (mouseControl) {
			transform.position = initPosition;
			transform.rotation = initRotation;
		}
		return initPosition;
	}


	bool hitObject()
	{
		if (freezed || (!mouseControl && !hitEnabled))
			return false;
		if ((!mouseControl && (yDistanceToTable < yDistanceToTableForHit)) || mouseControl && Input.GetMouseButtonDown (0))
		{
			RaycastHit hitPoint;
			if (Physics.Raycast (kopf.transform.position, -Vector3.up, out hitPoint, 20)) {
				
				//Only hits that hit the table area are valid
				if (hitPoint.collider.gameObject.Equals (tisch)) {
					target = hitPoint.point;
					anim.SetTrigger("hit");
                    if (!mouseControl) {
                        hit = true;
                        manager.hit(target);
                        hammerAnim.move = 4;
                    }
                    
                    hitEnabled = false;
					return true;					
				}
			}

		}
		return false;
	}

	public void touchedObject(GameObject touched)
	{
		Debug.Log("Touched " + touched.name);
	}

	void movement()
	{
		if (manager.activeMessageBox ())
			return;
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