using UnityEngine;
using System.Collections;

public class RunAway : MonoBehaviour {

	public GameObject fearObject;
	public Hammer hammer;
	private Animator anim;
	private bool jumpActive=false;
	Vector3 basePosition;
	Vector3 baseForward;
	public float speed = 1f;
	public float up = 0f;
	Vector3 fearObjectPos;
	public bool selfAnimated;
	Manager manager;


	// Use this for initialization
	void Start () {
		manager = GameObject.Find ("GameManager").GetComponent<Manager> ();
        fearObject = manager.getHammer();
		hammer = fearObject.GetComponentInParent<Hammer> ();
		anim = GetComponent<Animator> ();
		up = 0;
	}
	
	// Update is called once per frame
	void Update () {
	
		//Jump if hammer is hit
		if (hammer.hit && !manager.dead)
		{
			//turn away from object
			fearObjectPos = new Vector3(fearObject.transform.position.x,transform.position.y,fearObject.transform.position.z);
			if (!selfAnimated) {
				transform.LookAt (fearObjectPos);
				transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x,transform.rotation.eulerAngles.y -180,transform.rotation.eulerAngles.z);
			}

			//Jump
			anim.SetTrigger("Jump");
			if(Random.Range(-1,3)>0)
				if(GetComponent<AudioSource> () != null)
					GetComponent<AudioSource> ().Play ();
			hammer.hit = false;
		}

		if (jumpActive)
		{
			//Move up
			if (selfAnimated) {
				//rotate
				transform.Rotate(100 * Time.deltaTime,0,0);

				//Jump up
				transform.position = basePosition + Vector3.up * up * 2;

				//Move Forward
				basePosition += baseForward * speed * Time.deltaTime;

			} else {
				//Jump up
				transform.position = basePosition + Vector3.up * anim.GetFloat ("Up");

				//Move Forward
				basePosition += transform.forward * speed * Time.deltaTime;
			}


		}

	}

	//Called by Animation at current Screen
	void JumpStart()
	{
		basePosition = transform.position;
		if (selfAnimated) {
			baseForward = transform.position - fearObjectPos;
			baseForward.Normalize ();

		}
		jumpActive = true;
	}

	//Called by Animation at current Screen
	void JumpEnd()
	{
		Debug.Log ("Jump end");
		jumpActive = false;
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.Equals (fearObject)) {
			manager.hitAnimal ();
		} else {
			manager.hitSomething (other.gameObject);
		}
	}
}
