using UnityEngine;
using System.Collections;


public class Manager : MonoBehaviour {

	public enum State{
		SETUP,
		INIT,
		TRAINING,
		CONDITION_INSECT,
		CONDITION_INSECT_DEATH,
		CONDITION_INSECT_KILL,
		CONDITION_REPTILE,
		CONDITION_REPTILE_DEATH,
		CONDITION_REPTILE_KILL,
		CONDITION_MAMMAL,
		CONDITION_MAMMAL_DEATH,
		CONDITION_MAMMAL_KILL,
		FINAL
	};
	public State state = State.SETUP;

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
	public GameObject VRPN;
	public bool bloodActive = true;
	public GameObject blood_prefab;
	public GameObject blood_layer;
    private Hammer hammer;
	private GameObject activeAnimal;
	private Logger logger;
	private int activeBox = -1;
	private bool hitPointAnimation;
	private bool stateChangable = false;
	private bool pointsCollectible = true;
	public int conditonLoops = 1;
	public int conditionLoopCount = 1;
	private bool spaceEnabled = true;
	public bool dead = false;

	// Use this for initialization
	void Start () {
		viewManager = GameObject.Find("ViewManager").GetComponent<ViewManager> ();
		logger = new Logger ();
		logger.write("Mode:" + mode.ToString());
		switch (mode) {
		case Mode.PC:
			if (hammerContainerPC != null) {
				hammer = hammerContainerPC.GetComponent<Hammer> ();
			}
			VRPN.SetActive (false);
            hammerContainerVR.SetActive(false);
			hammerContainerPC.SetActive(true);
            cameraVR.SetActive(false);
            cameraPC.SetActive(true);
            break;
		case Mode.VR:
			if(hammerContainerVR!=null){
				hammer = hammerContainerVR.GetComponent<Hammer> ();
			}
			VRPN.SetActive (true);
            hammerContainerPC.SetActive(false);
			hammerContainerVR.SetActive(true);
            cameraPC.SetActive(false);
            cameraVR.SetActive(true);
            break;
		}

		changeState (state);
	}

	// Update is called once per frame
	void Update () {
		if (spaceEnabled && Input.GetKeyDown(KeyCode.Space)) {
			logger.logUserEntry ("Space");
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
		

	public void userInputSetup(object sender, System.EventArgs e)
	{
		ViewManager.InputEventArgs ie = (ViewManager.InputEventArgs)e;
		logger.init(ie.input);
		changeState (state +1);
	}

	void changeState(State newState){
		if (!newState.Equals (state)) {
			logger.endCondition (state.ToString ());
			logger.beginCondition (newState.ToString (),conditonLoops);
		} else {
			logger.beginTrial (state.ToString (), conditionLoopCount);
		}
		state = newState;
		switch(state){
		case State.SETUP:
			deactivateHammer ();
			viewManager.getTextInput ("Participant ID",userInputSetup);
			break;
		case State.INIT:
			resetCounter ();
			initHammer ();
			viewManager.hideAllMessages ();
			viewManager.setInfoText ("Please move the hammer to get in touch with controls.\n\n");
			viewManager.setSubText ("Press <Space> to start");
			stateChangable = false;
			break;
		case State.TRAINING:
			resetCounter ();
			initHammer ();
			viewManager.hideAllMessages ();
			viewManager.setInfoText ("Please move the ball into the box by hitting near the ball.");
			viewManager.setSubText ("Press <Space> to begin");
			activateRandomBox ();
			activateAnimal (0);
			stateChangable = false;
			break;
		case State.CONDITION_INSECT:
		case State.CONDITION_INSECT_DEATH:
			resetCounter ();
			initHammer ();
			viewManager.hideAllMessages ();
			viewManager.setInfoText ("Please move the spider into the box.");
			viewManager.setSubText ("Press <Space> to begin");
			activateRandomBox ();
			activateAnimal (1);
			stateChangable = false;
			break;
		case State.CONDITION_INSECT_KILL:
			resetCounter ();
			initHammer ();
			viewManager.hideAllMessages ();
			viewManager.setInfoText ("Please kill the spider.");
			viewManager.setSubText ("Press <Space> to begin");
			deactivateBoxes ();
			activateAnimal (1);
			stateChangable = false;
			break;
		case State.CONDITION_REPTILE:
		case State.CONDITION_REPTILE_DEATH:
			resetCounter ();
			initHammer ();
			viewManager.hideAllMessages ();
			viewManager.setInfoText ("Please move the gecko into the box.");
			viewManager.setSubText ("Press <Space> to begin");
			activateRandomBox ();
			activateAnimal (2);
			stateChangable = false;
			break;
		case State.CONDITION_REPTILE_KILL:
			resetCounter ();
			initHammer ();
			viewManager.hideAllMessages ();
			viewManager.setInfoText ("Please kill the gecko.");
			viewManager.setSubText ("Press <Space> to begin");
			deactivateBoxes ();
			activateAnimal (2);
			stateChangable = false;
			break;
		case State.CONDITION_MAMMAL:
		case State.CONDITION_MAMMAL_DEATH:
			resetCounter ();
			initHammer ();
			viewManager.hideAllMessages ();
			viewManager.setInfoText ("Please move the cat into the box.");
			viewManager.setSubText ("Press <Space> to begin");
			activateRandomBox ();
			activateAnimal (3);
			stateChangable = false;
			break;
		case State.CONDITION_MAMMAL_KILL:
			resetCounter ();
			initHammer ();
			viewManager.hideAllMessages ();
			viewManager.setInfoText ("Please kill the cat.");
			viewManager.setSubText ("Press <Space> to begin");
			deactivateBoxes ();
			activateAnimal (3);
			stateChangable = false;
			break;
		case State.FINAL:
			viewManager.hideAllMessages ();
			deactivateHammer ();
			viewManager.setInfoText ("Thank you for participating.");
			viewManager.setSubText ("Press <Space> to end game");
			stateChangable = false;
			break;
		}
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
		case State.CONDITION_INSECT_KILL:
			viewManager.hideAllMessages();
			break;
		case State.CONDITION_REPTILE:
		case State.CONDITION_REPTILE_DEATH:
		case State.CONDITION_REPTILE_KILL:
			viewManager.hideAllMessages();
			break;
		case State.CONDITION_MAMMAL:
		case State.CONDITION_MAMMAL_DEATH:
		case State.CONDITION_MAMMAL_KILL:
			viewManager.hideAllMessages();
			break;
		case State.FINAL:
			Application.Quit();
			break;
		}
	}


	void initHammer(){
		Vector3 resetPosition = hammer.reset ();
		logger.logResetHammer (resetPosition);
	}

	void activateHammer(){
		hammer.activate ();
	}

	void freezeHammer(){
		hammer.freeze ();
	}

	void deactivateHammer(){
		hammer.deactivate ();
	}

	private void addHit(Vector3 target){
		hits++;
		if (activeAnimal) {
			logger.logHit (target,activeAnimal.transform.position,hits);
		} else {
			logger.logHit (target,hits);
		}
		viewManager.setHits (hits);
	}
	public bool activeMessageBox(){
		return viewManager.activeMessageBox ();
	}

	void resetCounter(){
		points = 100;
		hits = 0;
		dead = false;
		viewManager.setPoints (points);
		viewManager.setHits (hits);
		logger.logResetPoints (points, hits);
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
		if (activeBox < boxes.Length) {
			boxes[activeBox].SetActive (true);
			logger.logBoxPosition (activeBox);
		}

	}

	public void deactivateBoxes(){
		foreach (GameObject box in boxes)
			box.SetActive (false);
	}

	public void addGoalPoints(int add){
		logger.logEnteredGoal ();
		if (!pointsCollectible)
			return;
		viewManager.setGoalPoints (add); 
		points += add;
		viewManager.setPoints(points);
		logger.logGoalPoints (add, points);
	}

	public void addHitPoints(int add){
		if (!pointsCollectible)
			return;
		viewManager.setHitPoints (add); 
		points += add;
		viewManager.setPoints (points);
		logger.logHitPoints (add, points);
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
			addHit (target);
			addHitPoints (-10);
			break;
		case State.TRAINING:
			addHit (target);
			addHitPoints (-10);
			break;
		case State.CONDITION_INSECT:
		case State.CONDITION_INSECT_DEATH:
		case State.CONDITION_INSECT_KILL:
			addHit (target);
			if (!dead)
				addHitPoints (-10);
			break;
		case State.CONDITION_REPTILE:
		case State.CONDITION_REPTILE_DEATH:
		case State.CONDITION_REPTILE_KILL:
			addHit (target);
			if (!dead)
				addHitPoints (-10);
			break;
		case State.CONDITION_MAMMAL:
		case State.CONDITION_MAMMAL_DEATH:
		case State.CONDITION_MAMMAL_KILL:
			addHit (target);
			if (!dead)
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
			destroyAnimal ();
			freezeHammer ();
			addHitPoints (-30);
			pointsCollectible = false;
			showBothTextInSeconds(2,"Congratulations.","Press <Space> for next round");
			stateChangable = true;
			break;
		case State.CONDITION_REPTILE_DEATH:
		case State.CONDITION_INSECT_DEATH:
		case State.CONDITION_MAMMAL_DEATH:
			if (dead)
				return;
			killAnimal ();
			freezeHammer ();
			addHitPoints(-30);
			pointsCollectible = false;
			showBothTextInSeconds(2,"Congratulations.","Press <Space> for next round");
			stateChangable = true;
			break;
		case State.CONDITION_REPTILE_KILL:
		case State.CONDITION_INSECT_KILL:
		case State.CONDITION_MAMMAL_KILL:
			if (dead)
				return;
			killAnimal ();
			freezeHammer();
			addGoalPoints (50);
			showBothTextInSeconds(2,"Congratulations. You killed the Animal.","Press <Space> for next round");
			stateChangable = true;
			break;
		}
	}

	public void destroyAnimal(){
		logger.logDestroyAnimal (activeAnimal.name);
		Destroy (activeAnimal);
	}

	public void killAnimal(){
		dead = true;
		activeAnimal.GetComponent<Animator> ().SetTrigger ("Die");
		switch (state) {
		case State.CONDITION_MAMMAL:
		case State.CONDITION_MAMMAL_DEATH:
		case State.CONDITION_MAMMAL_KILL:
			BoxCollider bc = activeAnimal.GetComponent<BoxCollider> ();
			bc.center -= new Vector3 (0, 0.02f, 0);
			bc.size -= new Vector3 (0, 0.12f, 0);
			break;
		}
		if (bloodActive) {
			GameObject blood  = GameObject.Instantiate (blood_prefab);
			blood.transform.SetParent (activeAnimal.transform);
			blood.transform.localPosition = Vector3.zero;
			ParticleSystem particle = blood.GetComponentInChildren<ParticleSystem> ();
			particle.collision.SetPlane (0, blood_layer.transform);
			particle.Play ();
		}

		logger.logKillAnimal (activeAnimal.name);
	}

	public void hitSomething(GameObject other){

		if (other.name == "Kopf")
			hitAnimal ();
		switch(state){
		case State.TRAINING:
			if (other.name == "Trigger") {
				addGoalPoints (30);
				viewManager.setInfoText ("Congratulations. It entered the box.");
				activeAnimal.GetComponent<Rigidbody>().useGravity = false;
			}

			if (other.name == "Floor" || other.name == "outside") {
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
		case State.CONDITION_REPTILE_KILL:
		case State.CONDITION_INSECT_KILL:
		case State.CONDITION_MAMMAL_KILL:
			if (other.name == "Trigger") {
				addGoalPoints (30);
				viewManager.setInfoText ("Congratulations. It entered the box.");
				viewManager.setSubText ("Press <Space> for next round");
				stateChangable = true;

			}

			if (other.name == "Floor" || other.name == "outside") {
				logger.logOffTable (activeAnimal.transform.position);
				Destroy (activeAnimal);
				viewManager.setInfoText ("It fell off the Table.");
				viewManager.setSubText ("Press <Space> for next round");
				stateChangable = true;
			}
			break;
		}
	}

	void showBothTextInSeconds(int seconds, string infotext, string subtext)
	{
		spaceEnabled = false;
		StartCoroutine (IEshowBothTextInSeconds (seconds, infotext, subtext));
	}

	IEnumerator IEshowBothTextInSeconds(int seconds, string infotext, string subtext) {
		yield return new WaitForSeconds(seconds);
		viewManager.setInfoText (infotext);
		viewManager.setSubText (subtext);
		spaceEnabled = true;
	}

	void showInfoTextInSeconds(int seconds, string text)
	{
		spaceEnabled = false;
		StartCoroutine (IEshowInfoTextInSeconds (seconds, text));
	}

	IEnumerator IEshowInfoTextInSeconds(int seconds, string text) {
		yield return new WaitForSeconds(seconds);
		viewManager.setInfoText (text);
		spaceEnabled = true;
	}

	void changeStateInSeconds(int seconds){
		StartCoroutine (IEchangeStateInSeconds (seconds));

	}

	IEnumerator IEchangeStateInSeconds(int seconds) {
		yield return new WaitForSeconds(seconds);
		changeState (state + 1);
	}

	void resetStateInSeconds(int seconds){
			StartCoroutine (IEresetStateInSeconds (seconds));

	}

	IEnumerator IEresetStateInSeconds(int seconds) {
		yield return new WaitForSeconds(seconds);
		resetState ();
	}

	void OnApplicationQuit() {
		if(logger.initialized)
			logger.close ();
	}
}