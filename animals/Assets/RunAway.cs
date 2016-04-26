using UnityEngine;
using System.Collections;

public class RunAway : MonoBehaviour {

	public GameObject fearObject;
	private Hammer hammer; 
	private Animator anim;
	private float walkSpeed = 2.0f;
	private bool jumpActive=false;
	Vector3 basePosition;
	float jumpSpeed = 1f;
	Vector3 fearObjectPos;



	// Use this for initialization
	void Start () {
		//fearObjectTransform = new Transform();
		hammer = fearObject.GetComponentInParent<Hammer> ();
		anim = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
	
		//Jump if hammer is hit
		if (hammer.hit || Input.GetKeyDown(KeyCode.Space))
		{
			//turn away from object
			fearObjectPos = new Vector3(fearObject.transform.position.x,transform.position.y,fearObject.transform.position.z);
			transform.LookAt (fearObjectPos);
			transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x,transform.rotation.eulerAngles.y -180,transform.rotation.eulerAngles.z);

			//Jump
			anim.SetTrigger("Jump");
		}

		if (jumpActive)
		{
			//Move up
			transform.position = basePosition + Vector3.up * anim.GetFloat("Up");

			//Move Forward
			basePosition += transform.forward * jumpSpeed * Time.deltaTime;
		}

	}

	//Called by Animation at current Screen
	void JumpStart()
	{
		basePosition = transform.position;
		jumpActive = true;
		Debug.Log ("Jump started");
	}

	//Called by Animation at current Screen
	void JumpEnd()
	{
		jumpActive = false;
		Debug.Log ("Jump ended");
	}

}
