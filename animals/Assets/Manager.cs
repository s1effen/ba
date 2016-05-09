using UnityEngine;
using System.Collections;


public class Manager : MonoBehaviour {

	public enum State{
		INIT,
		TRAINING,
		CONDITION_INSECT,
		CONDITION_INSECT_DEATH,
		CONDITION_REPTILE,
		CONDITION_MAMMAL
	};
	public State state = State.INIT;

	public enum Mode{
		PC,
		VR
	};
	public Mode mode;
	private ViewManager viewManager;
	public int points = 0;
	public int hits = 0;
	public GameObject[] animals;
	public GameObject[] boxes;
	public GameObject hammerContainerPC;
	public GameObject hammerContainerVR;
	private int activeAnimalPrefab = -1;
	private GameObject activeAnimal;
	private int activeBox = -1;
	private bool hitPointAnimation;
	private bool stateChangable = false;
	private Transform hammerContainerPCInitTransform;
	private Transform hammerContainerVRInitTransform;

	// Use this for initialization
	void Start () {
		viewManager = GameObject.Find("ViewManager").GetComponent<ViewManager> ();

		if (hammerContainerPC != null) {
			hammerContainerPCInitTransform = hammerContainerPC.transform;
			hammerContainerPC.SetActive (false);
		}

		if (hammerContainerVR != null) {
			hammerContainerVRInitTransform = hammerContainerVR.transform;
			hammerContainerVR.SetActive (false);
		}

		changeState (state);
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space)) {
			if (stateChangable) {
				changeState (state + 1);
			} else {
				spaceInState ();
			}
		}
	}

	void spaceInState(){
		switch(state){
		case State.INIT:
			viewManager.hideInfoText ();
			viewManager.setSubText ("Press <Space> to begin training round");
			stateChangable = true;
			break;
		case State.TRAINING:
			viewManager.hideInfoText ();
			viewManager.setSubText ("Press <Space> to end training round and begin experiment");
			stateChangable = true;
			break;
		case State.CONDITION_MAMMAL:
			break;
		case State.CONDITION_INSECT:
		case State.CONDITION_INSECT_DEATH:
			viewManager.hideAllMessages();
			break;
		case State.CONDITION_REPTILE:
			break;
		}
	}
	void changeState(State newState){

		state = newState;

		switch(state){
		case State.INIT:
			resetCounter ();
			viewManager.setInfoText ("Please move the hammer to get in touch with controls.\n\n Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec volutpat, est feugiat rutrum cursus, felis ante porttitor nisl, eget dapibus velit dolor id eros. Donec ac libero nisl. Morbi lorem orci, efficitur pulvinar scelerisque et, vestibulum a diam. Aliquam erat volutpat. ");
			viewManager.setSubText ("Press <Space> to start");
			stateChangable = false;
			break;
		case State.TRAINING:
			resetCounter ();
			viewManager.setInfoText ("Please move the ball into the boxes.");
			viewManager.setSubText ("Press <Space> to begin");
			activateRandomBox ();
			activateAnimal (0);
			stateChangable = false;
			initHammer ();
			break;
		case State.CONDITION_MAMMAL:
			break;
		case State.CONDITION_INSECT:
			resetCounter ();
			initHammer ();
			viewManager.setInfoText ("Please move the spider into the boxes.");
			viewManager.setSubText ("Press <Space> to begin");
			activateRandomBox ();
			activateAnimal (1);
			stateChangable = false;

			break;
		case State.CONDITION_INSECT_DEATH:
			resetCounter ();
			initHammer ();
			viewManager.setInfoText ("Please move the spider into the boxes.");
			viewManager.setSubText ("Press <Space> to begin");
			activateRandomBox ();
			activateAnimal (1);
			stateChangable = false;

			break;
		case State.CONDITION_REPTILE:
			break;
		}
	}

	void initHammer(){
		switch (mode) {
		case Mode.PC:
			if (hammerContainerPC != null) {
				hammerContainerPC.SetActive (false);
				hammerContainerPC.transform.position = hammerContainerPCInitTransform.position;
				hammerContainerPC.transform.rotation = hammerContainerPCInitTransform.rotation;
				hammerContainerPC.SetActive (true);
			}
			break;
		case Mode.VR:
			if(hammerContainerVR!=null){
				hammerContainerVR.SetActive (false);
				hammerContainerVR.transform.position = hammerContainerVRInitTransform.position;
				hammerContainerVR.transform.rotation = hammerContainerVRInitTransform.rotation;
				hammerContainerVR.SetActive (true);				
			}
			break;
		}
	}

	private void addHit(){
		hits++;
		viewManager.setHits (hits);
	}
	public bool activeMessageBox(){
		return viewManager.activeMessageBox ();
	}

	void resetCounter(){
		points = 0;
		hits = 0;
		viewManager.setPoints (points);
		viewManager.setHits (hits);
	}

	void resetState(){
		switch(state){
		case State.TRAINING:
			activateRandomBox ();
			activateAnimal (0);
			points = 0;
			viewManager.setPoints (points);
			hits = 0;
			viewManager.setHits (hits);
			stateChangable = false;
			break;
		case State.CONDITION_MAMMAL:
			break;
		case State.CONDITION_INSECT:
			break;
		case State.CONDITION_REPTILE:
			break;
		}
	}

	public void hit(Vector3 target){
		switch(state){
		case State.TRAINING:
			addHit ();
			addPointsBasedOnDistance (target);
			break;
		case State.CONDITION_MAMMAL:
			break;
		case State.CONDITION_INSECT:
			addHit ();
			addPointsBasedOnDistance (target);
			break;
		case State.CONDITION_INSECT_DEATH:
			addHit ();
			addPointsBasedOnDistance (target);
			break;
		case State.CONDITION_REPTILE:
			break;
		}
	}
	public void activateRandomBox(){
		deactivateBoxes ();
		activeBox =  (int)Mathf.Round (Random.Range(0,boxes.Length));
		if(activeBox < boxes.Length)
			boxes[activeBox].SetActive (true);
	}

	public void deactivateBoxes(){
		foreach (GameObject box in boxes)
			box.SetActive (false);
	}

	public void addGoalPoints(int add){
		viewManager.setGoalPoints (add); 
		points += add;
		viewManager.setPoints(points);
	}

	public void addHitPoints(int add){
		viewManager.setHitPoints (add); 
		points += add;
		viewManager.setPoints (points);
	}

	public void addPointsBasedOnDistance(Vector3 referencePoint){
		if (activeAnimal == null)
			return;
		float distance = Vector2.Distance(new Vector2(referencePoint.x,referencePoint.y),new Vector2(activeAnimal.transform.position.x,activeAnimal.transform.position.y));
		addHitPoints((int)Mathf.Round((Mathf.Pow(0.8f-distance,4)*10))*5);
	}

	public void activateAnimal(int anim){
		if (activeAnimal != null)
			Destroy (activeAnimal);
		activeAnimalPrefab = anim;
		activeAnimal = Instantiate (animals [activeAnimalPrefab]);
		activeAnimal.transform.SetParent(GameObject.Find ("Animals").transform);
		activeAnimal.transform.localPosition = Vector3.zero;
		activeAnimal.SetActive(true);

	}

	public GameObject getActiveAnimal(){
		return activeAnimal;
	}



	public void hitAnimal(){
		switch(state){
		case State.TRAINING:
			break;
		case State.CONDITION_MAMMAL:
			break;
		case State.CONDITION_INSECT:
			Destroy (activeAnimal);
			changeStateInSeconds (2);
			break;
		case State.CONDITION_INSECT_DEATH:
			activeAnimal.GetComponent<Animator> ().SetTrigger ("Die");
			changeStateInSeconds (5);
			break;
		case State.CONDITION_REPTILE:
			break;
		}
	}

	public void hitSomething(GameObject other){

		if (other.name == "Kopf")
			hitAnimal ();
			
		switch(state){
		case State.TRAINING:
			if (other.name == "Trigger") {
				addGoalPoints (50);
				viewManager.setInfoText ("Congratulations. The object entered the box.");
				activeAnimal.GetComponent<Rigidbody>().useGravity = false;
			}

			if (other.name == "Floor") {
				resetStateInSeconds (2);
				viewManager.setInfoText ("It fell off the Table. Please move the ball into the boxes.");
				viewManager.setSubText ("Press <Space> to play again");
			}
			break;
		case State.CONDITION_MAMMAL:
			break;
		case State.CONDITION_INSECT:

			break;
		case State.CONDITION_REPTILE:
			break;
		}
	}

	void changeStateInSeconds(int seconds){
		StartCoroutine (IEchangeStateInSeconds (2));

	}

	IEnumerator IEchangeStateInSeconds(int seconds) {
		yield return new WaitForSeconds(seconds);
		changeState (state + 1);
	}

	void resetStateInSeconds(int seconds){
		StartCoroutine (IEresetStateInSeconds (2));

	}

	IEnumerator IEresetStateInSeconds(int seconds) {
		yield return new WaitForSeconds(seconds);
		resetState ();
	}

}