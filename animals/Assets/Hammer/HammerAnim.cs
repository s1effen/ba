using UnityEngine;
using System.Collections;

public class HammerAnim : MonoBehaviour {

	public int move = 0;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

	}

	void hitbegin(){
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
