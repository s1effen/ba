using UnityEngine;
using System.Collections;


public class Manager : MonoBehaviour {

	public enum State{
		INIT,
		TRAINING,
		CONDITION_INSECT,
		CONDITION_INSECT_DEATH,
		CONDITION_REPTILE,
		CONDITION_REPTILE_DEATH,
		CONDITION_MAMMAL,
		CONDITION_MAMMAL_DEATH,
		FINAL
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
    public GameObject cameraPC;
    public GameObject cameraVR;
    private Hammer hammer;
	private GameObject activeAnimal;
	private int activeBox = -1;
	private bool hitPointAnimation;
	private bool stateChangable = false;
	private bool pointsCollectible = true;
	public int conditonLoops = 1;
	public int conditionLoopCount = 1;

	// Use this for initialization
	void Start () {
		viewManager = GameObject.Find("ViewManager").GetComponent<ViewManager> ();
		switch (mode) {
		case Mode.PC:
			if (hammerContainerPC != null) {
				hammer = hammerContainerPC.GetComponent<Hammer> ();
            }
                hammerContainerVR.SetActive(false);
                cameraVR.SetActive(false);
                cameraPC.SetActive(true);
                break;
		case Mode.VR:
			if(hammerContainerVR!=null){
				hammer = hammerContainerVR.GetComponent<Hammer> ();
			}
                hammerContainerPC.SetActive(false);
                cameraPC.SetActive(false);
                cameraVR.SetActive(true);
                break;
		}

		changeState (state);
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Space)) {
			if (stateChangable) {
				if (conditionLoopCount == conditonLoops) {
					conditionLoopCount = 1;
					changeState (state + 1);
				}else{
					conditionLoopCount++;
					changeState (state);
				}
			} else {
				spaceInState ();
			}
		}
	}

    public GameObject getHammer(){
        return hammer.transform.GetChild(0).FindChild("Kopf").gameObject;
        }

    void spaceInState(){
		switch(state){
		case State.INIT:
			viewManager.hideInfoText ();
			viewManager.setSubText ("Press <Space> to begin training round");
			conditionLoopCount = conditonLoops;
			stateChangable = true;
			break;
		case State.TRAINING:
			viewManager.hideInfoText ();
			viewManager.setSubText ("Press <Space> to end training round and begin experiment");
			conditionLoopCount = conditonLoops;
			stateChangable = true;
			break;
		case State.CONDITION_INSECT:
		case State.CONDITION_INSECT_DEATH:
			viewManager.hideAllMessages();
			break;
		case State.CONDITION_REPTILE:
		case State.CONDITION_REPTILE_DEATH:
			viewManager.hideAllMessages();
			break;
		case State.CONDITION_MAMMAL:
		case State.CONDITION_MAMMAL_DEATH:
			viewManager.hideAllMessages();
			break;
		}
	}
	void changeState(State newState){
		state = newState;

		switch(state){
		case State.INIT:
			resetCounter ();
			initHammer ();
			viewManager.setInfoText ("Please move the hammer to get in touch with controls.\n\n Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec volutpat, est feugiat rutrum cursus, felis ante porttitor nisl, eget dapibus velit dolor id eros. Donec ac libero nisl. Morbi lorem orci, efficitur pulvinar scelerisque et, vestibulum a diam. Aliquam erat volutpat. ");
			viewManager.setSubText ("Press <Space> to start");
			stateChangable = false;
			break;
		case State.TRAINING:
			resetCounter ();
			initHammer ();
			viewManager.setInfoText ("Please move the ball into the box.");
			viewManager.setSubText ("Press <Space> to begin");
			activateRandomBox ();
			activateAnimal (0);
			stateChangable = false;
			initHammer ();
			break;
		case State.CONDITION_INSECT:
		case State.CONDITION_INSECT_DEATH:
			resetCounter ();
			initHammer ();
			viewManager.setInfoText ("Please move the spider into the box.");
			viewManager.setSubText ("Press <Space> to begin");
			activateRandomBox ();
			activateAnimal (1);
			stateChangable = false;
			break;
		case State.CONDITION_REPTILE:
		case State.CONDITION_REPTILE_DEATH:
			resetCounter ();
			initHammer ();
			viewManager.setInfoText ("Please move the gecko into the box.");
			viewManager.setSubText ("Press <Space> to begin");
			activateRandomBox ();
			activateAnimal (2);
			stateChangable = false;
			break;
		case State.CONDITION_MAMMAL:
		case State.CONDITION_MAMMAL_DEATH:
			resetCounter ();
			initHammer ();
			viewManager.setInfoText ("Please move the cat into the box.");
			viewManager.setSubText ("Press <Space> to begin");
			activateRandomBox ();
			activateAnimal (3);
			stateChangable = false;
			break;
		case State.FINAL:
			viewManager.hideAllMessages ();
			viewManager.setInfoText ("Thank you for participating.");
			stateChangable = false;
			break;
		}
	}

	void initHammer(){
		hammer.reset ();
	}

	private void addHit(){
		hits++;
		viewManager.setHits (hits);
	}
	public bool activeMessageBox(){
		return viewManager.activeMessageBox ();
	}

	void resetCounter(){
		points = 100;
		hits = 0;
		viewManager.setPoints (points);
		viewManager.setHits (hits);
		pointsCollectible = true;
	}

	void resetState(){
		switch(state){
		case State.TRAINING:
			activateRandomBox ();
			activateAnimal (0);
			resetCounter ();
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
		if (!pointsCollectible)
			return;
		viewManager.setGoalPoints (add); 
		points += add;
		viewManager.setPoints(points);
	}

	public void addHitPoints(int add){
		if (!pointsCollectible)
			return;
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
		activeAnimal = Instantiate (animals [anim]);
		activeAnimal.transform.SetParent(GameObject.Find ("Animals").transform);
		activeAnimal.transform.localPosition = Vector3.zero;
		activeAnimal.SetActive(true);

	}

	public GameObject getActiveAnimal(){
		return activeAnimal;
	}


	public void hit(Vector3 target){
		switch(state){
		case State.INIT:
			addHit ();
			addHitPoints (-10);
			break;
		case State.TRAINING:
			addHit ();
			addHitPoints (-10);
			break;
		case State.CONDITION_INSECT:
		case State.CONDITION_INSECT_DEATH:
			addHit ();
			addHitPoints (-10);
			break;
		case State.CONDITION_REPTILE:
		case State.CONDITION_REPTILE_DEATH:
			addHit ();
			addHitPoints (-10);
			break;
		case State.CONDITION_MAMMAL:
		case State.CONDITION_MAMMAL_DEATH:
			addHit ();
			addHitPoints (-10);
			break;
		}
	}

	public void hitAnimal(){
		switch(state){
		case State.TRAINING:
			break;
		case State.CONDITION_INSECT:
		case State.CONDITION_REPTILE:
		case State.CONDITION_MAMMAL:
			Destroy (activeAnimal);
			addHitPoints (-30);
			pointsCollectible = false;
			viewManager.setSubText ("Press <Space> for next round");
			stateChangable = true;
			break;
		case State.CONDITION_REPTILE_DEATH:
		case State.CONDITION_INSECT_DEATH:
		case State.CONDITION_MAMMAL_DEATH:
			activeAnimal.GetComponent<Animator> ().SetTrigger ("Die");
			addHitPoints(-30);
			pointsCollectible = false;
			viewManager.setSubText ("Press <Space> for next round");
			stateChangable = true;
			break;
		}
	}

	public void hitSomething(GameObject other){

		if (other.name == "Kopf")
			hitAnimal ();
			
		switch(state){
		case State.TRAINING:
			if (other.name == "Trigger") {
				addGoalPoints (30);
				viewManager.setInfoText ("Congratulations. The object entered the box.");
				activeAnimal.GetComponent<Rigidbody>().useGravity = false;
			}

			if (other.name == "Floor") {
				resetStateInSeconds (2);
				viewManager.setInfoText ("It fell off the Table. Please move the ball into the boxes.");
				viewManager.setSubText ("Press <Space> to play again");
				stateChangable = false;
			}
			break;
		case State.CONDITION_MAMMAL:
		case State.CONDITION_INSECT:
		case State.CONDITION_REPTILE:
		case State.CONDITION_REPTILE_DEATH:
		case State.CONDITION_INSECT_DEATH:
		case State.CONDITION_MAMMAL_DEATH:
			if (other.name == "Trigger") {
				addGoalPoints (30);
				viewManager.setInfoText ("Congratulations. The object entered the box.");
			}

			if (other.name == "Floor") {
				viewManager.setSubText ("Press <Space> for next round");
				stateChangable = true;
			}
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