using UnityEngine;
using System.Collections;

public class moveInBounds : MonoBehaviour {

	Bounds parentBounds;
	bool rotate = true;
	public float speed = 0.01f;
	public float boundRotate = 180;
	public float timeRotate = 60;
	// Use this for initialization
	void Start () {
		parentBounds = transform.parent.GetComponent<Collider>().bounds;
	}
	
	// Update is called once per frame
	void Update () {
		transform.position += transform.forward * speed;
		if (!parentBounds.Contains (transform.position))
			transform.Rotate(Vector3.up * boundRotate);
		if (rotate) {
			rotate = false;
			rotateInSeconds (1,timeRotate);
		}
	}

	void rotateInSeconds(int seconds, float max){
		StartCoroutine (IErotateInSeconds (seconds, max));

	}

	IEnumerator IErotateInSeconds(int seconds, float max) {
		yield return new WaitForSeconds(seconds);
		roatateRandom (max);
		rotate = true;

	}

	void roatateRandom(float max)
	{
		float angle = Random.Range (-max, max);
		transform.Rotate(Vector3.up * angle);
	}
}
