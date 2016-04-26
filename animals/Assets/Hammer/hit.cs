using UnityEngine;
using System.Collections;

public class hit : MonoBehaviour {

	public GameObject container;
	public GameObject kopf;
	public int move = 0;
	/*
	Vector3 targetup;
	Vector3 targetdown;
	Vector3 originPos;
	*/

	// Use this for initialization
	void Start () {
	


	}
	
	// Update is called once per frame
	void Update () {
		/*
		move = 0;

		if (move == 1) { //up
			float step = 5 * Time.deltaTime;
			container.transform.position = Vector3.MoveTowards (transform.position, targetup, step);
			
		} else if (move == 2) { //down
			float step = 7 * Time.deltaTime;
			container.transform.position = Vector3.MoveTowards (transform.position, targetdown, step);
		} else if (move == 3)
		{ //Go Back to initial position
			if(container.transform.position.y - originPos.y > 0.001f)
			{
				float step = 7 * Time.deltaTime;
				container.transform.position = Vector3.MoveTowards (container.transform.position, originPos, step);
			}
			else{
				move = 0;
			}

		}*/
	}

	void hitbegin(){
		/*
		originPos = container.transform.position;
		RaycastHit hit;
		Collider collider;
		if (Physics.Raycast (kopf.transform.position, -Vector3.up, out hit, 20)) {
			print ("There is " + hit.collider.gameObject.name + " in front of the object!");
			targetdown = hit.point;
		} else {
			//targetdown = transform.position + Vector3.up * 0.1f;
		}
		targetup = transform.position + Vector3.up * 0.1f;
		*/
		move = 1;
		//Move Hammer up a bit.
	}
	

	void hitpeak(){
	move = 2;
	}


	void hitend(){
	move = 3;
	}
}
