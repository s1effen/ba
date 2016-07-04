using UnityEngine;
using System.Collections;
using UnityEngine.VR;


public class Manager : MonoBehaviour {

	/*
	Enum for different Conditions
	*/
	public enum Condition{
		SETUP,						//User input for participant id
		INIT,						//User can get handy with controls
		TRAINING,					//User can do training with a ball
		CONDITION_INSECT,			//User should bring insect to box. When hit, insect disappears.
		CONDITION_INSECT_DEATH,		//User should bring insect to box. When hit, insect dies.
		CONDITION_INSECT_KILL,		//User should kill insect. When hit, insect dies.
		CONDITION_REPTILE,
		CONDITION_REPTILE_DEATH,
		CONDITION_REPTILE_KILL,
		CONDITION_MAMMAL,
		CONDITION_MAMMAL_DEATH,
		CONDITION_MAMMAL_KILL,
		FINAL
	};
	public Condition condition = Condition.SETUP;

	/*
	Enum for different Groups (Desktop or VR)
	*/
	public enum Group{
		PC,
		VR
	};
	public Group group;
	private ViewManager viewManager;
	public int points = 0;
	public int hits = 0;
	int hitstotal;
	int pointstotal;
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
	private bool conditionChangable = false;
	private bool pointsCollectible = true;
	public int conditonLoops = 1;
	public int conditionLoopCount = 1;
	private bool spaceEnabled = true;
	public bool dead = false;
	public StartArea startArea;
	Vector3 animalLastPos = Vector3.zero;

	// Use this for initialization
	void Start () {
		viewManager = GameObject.Find("ViewManager").GetComponent<ViewManager> ();

		//Initialize logger
		logger = new Logger ();
		logger.write("Group:" + group.ToString());

		/*
			Initialize setup based on group
		*/
		viewManager.mode (group);
		switch (group) {
		case Group.PC:
			if (hammerContainerPC != null) {
				hammer = hammerContainerPC.GetComponent<Hammer> ();
			}
			VRSettings.enabled = false;
			VRPN.SetActive (false);
			hammerContainerVR.SetActive (false);
			hammerContainerPC.SetActive (true);
			cameraVR.SetActive (false);
			cameraPC.SetActive (true);

            break;
		case Group.VR:
			if (hammerContainerVR != null) {
				hammer = hammerContainerVR.GetComponent<Hammer> ();
			}
			VRSettings.enabled = true;
			VRPN.SetActive (true);
			hammerContainerPC.SetActive (false);
			hammerContainerVR.SetActive (true);
			cameraPC.SetActive (false);
			cameraVR.SetActive (true);
            break;
		}

		//start initial condition
		changeCondition (condition);
	}

	// Update is called once per frame
	void Update () {
		OVRInput.Update();

		//Quit application when pressed Q
		if (Input.GetKeyDown(KeyCode.Q)){
			changeCondition (Condition.FINAL);
		}
			
		/*
			react to user input via space key to toggle conditions or hide messages
		*/
		if (spaceEnabled && Input.GetKeyDown(KeyCode.Space) || (OVRInput.GetDown(OVRInput.Button.One) && startArea.free)) {
			logger.logUserEntry ("Space");

			/*
			 if condition should have more than one trial, activate other trial, otherwise next condition.
			*/
			if (conditionChangable) {
				if (conditionLoopCount == conditonLoops) {
					conditionLoopCount = 1;
					changeCondition (condition + 1);
				}else{
					conditionLoopCount++;
					changeCondition (condition);
				}
			} else {
				spaceInCondition (); //just to hide user messages
			}
		}
	}

	//Get Hammer Gameobject
    public GameObject getHammer(){
        return hammer.transform.GetChild(0).FindChild("Kopf").gameObject;
    }
		
	//Gets Event from user input field
	public void userInputSetup(object sender, System.EventArgs e)
	{
		ViewManager.InputEventArgs ie = (ViewManager.InputEventArgs)e;
		logger.init(ie.input, group);
		changeCondition (condition +1);
	}

	/*
	Changes condition to new condition or loads next trial of the same condition.
	All necessary steps are done for initialisation like reset counter, change animal etc.
	*/
	void changeCondition(Condition newCondition){

		/*
		Logging. Only inactive in Setup or Final condition.
		*/
		if (newCondition != Condition.SETUP && newCondition != Condition.FINAL) {
			if (!newCondition.Equals (condition)) {
				logger.endCondition (condition.ToString ());
				logger.beginCondition (newCondition, conditonLoops);
			} else {
				logger.beginTrial (condition, conditonLoops,conditionLoopCount);
			}
		}

		condition = newCondition;
		switch(condition){
		case Condition.SETUP:
			deactivateHammer ();
			viewManager.getTextInput ("Participant ID",userInputSetup);
			break;
		case Condition.INIT:
			viewManager.hideAllMessages ();
			initHammer ();
			pointsCollectible = false;
			switch (group) {
			case Group.PC:
				viewManager.setInfoText ("Please move the hammer to get in touch with controls. The experiment can be aborted at any time by pressing 'q' or telling the Assistant.\n");
				viewManager.setSubText ("Press <Space> to start");
				break;
			case Group.VR:
				viewManager.setInfoText ("Please check the controls, and the head-mounted-display. The Assistant can help you calibrating the scene. The experiment can be aborted at any time by telling the Assistant.\n");
				viewManager.setSubText ("Press central button to start training");				
				break;
			}
			conditionChangable = false;
			break;
		case Condition.TRAINING:
			resetCounter ();
			initHammer ();
			viewManager.hideAllMessages ();
			viewManager.setInfoText ("Please move the ball into the box by hitting near the ball.");
			switch (group) {
			case Group.PC:
				viewManager.setSubText ("Press <Space> to begin");
				break;
			case Group.VR:
				startArea.gameObject.SetActive (true);
				hammer.freeze ();
				viewManager.setSubText ("Press central button to begin");				
				break;
			}
			activateRandomBox ();
			activateAnimal (0);
			conditionChangable = false;
			break;
		case Condition.CONDITION_INSECT:
		case Condition.CONDITION_INSECT_DEATH:
			resetCounter ();
			initHammer ();
			viewManager.hideAllMessages ();
			viewManager.setInfoText ("Please move the spider into the box.");
			switch (group) {
			case Group.PC:
				viewManager.setSubText ("Press <Space> to begin");
				break;
			case Group.VR:
				startArea.gameObject.SetActive(true);	
				hammer.freeze ();
				viewManager.setSubText ("Press central button to begin");				
				break;
			}			
			activateRandomBox ();
			activateAnimal (1);
			conditionChangable = false;
			break;
		case Condition.CONDITION_INSECT_KILL:
			resetCounter ();
			initHammer ();
			viewManager.hideAllMessages ();
			viewManager.setInfoText ("Please kill the spider.");
			switch (group) {
			case Group.PC:
				viewManager.setSubText ("Press <Space> to begin");
				break;
			case Group.VR:
				startArea.gameObject.SetActive(true);	
				hammer.freeze ();
				viewManager.setSubText ("Press central button to begin");				
				break;
			}			
			deactivateBoxes ();
			activateAnimal (1);
			conditionChangable = false;
			break;
		case Condition.CONDITION_REPTILE:
		case Condition.CONDITION_REPTILE_DEATH:
			resetCounter ();
			initHammer ();
			viewManager.hideAllMessages ();
			viewManager.setInfoText ("Please move the gecko into the box.");
			switch (group) {
			case Group.PC:
				viewManager.setSubText ("Press <Space> to begin");
				break;
			case Group.VR:
				startArea.gameObject.SetActive(true);
				hammer.freeze ();
				viewManager.setSubText ("Press central button to begin");				
				break;
			}			
			activateRandomBox ();
			activateAnimal (2);
			conditionChangable = false;
			break;
		case Condition.CONDITION_REPTILE_KILL:
			resetCounter ();
			initHammer ();
			viewManager.hideAllMessages ();
			viewManager.setInfoText ("Please kill the gecko.");
			switch (group) {
			case Group.PC:
				viewManager.setSubText ("Press <Space> to begin");
				break;
			case Group.VR:
				startArea.gameObject.SetActive(true);	
				hammer.freeze ();
				viewManager.setSubText ("Press central button to begin");				
				break;
			}			deactivateBoxes ();
			activateAnimal (2);
			conditionChangable = false;
			break;
		case Condition.CONDITION_MAMMAL:
		case Condition.CONDITION_MAMMAL_DEATH:
			resetCounter ();
			initHammer ();
			viewManager.hideAllMessages ();
			viewManager.setInfoText ("Please move the cat into the box.");
			switch (group) {
			case Group.PC:
				viewManager.setSubText ("Press <Space> to begin");
				break;
			case Group.VR:
				startArea.gameObject.SetActive(true);
				hammer.freeze ();
				viewManager.setSubText ("Press central button to begin");				
				break;
			}			activateRandomBox ();
			activateAnimal (3);
			conditionChangable = false;
			break;
		case Condition.CONDITION_MAMMAL_KILL:
			resetCounter ();
			initHammer ();
			viewManager.hideAllMessages ();
			viewManager.setInfoText ("Please kill the cat.");
			switch (group) {
			case Group.PC:
				viewManager.setSubText ("Press <Space> to begin");
				break;
			case Group.VR:
				startArea.gameObject.SetActive(true);	
				hammer.freeze ();
				viewManager.setSubText ("Press central button to begin");				
				break;
			}
			deactivateBoxes ();
			activateAnimal (3);
			conditionChangable = false;
			break;
		case Condition.FINAL:
			viewManager.hideAllMessages ();
			deactivateHammer ();
			destroyAnimal ();
			activateAllAnimals ();
			logger.appendSummary ();
			viewManager.setHits (hitstotal);
			viewManager.setPoints (pointstotal);
			switch (group) {
			case Group.PC:
				viewManager.setSubText ("Thank you for participating. Press <Space>");
				break;
			case Group.VR:
				viewManager.setSubText ("Thank you for participating. Press central button.");		
				break;
			}
			conditionChangable = false;
			break;
		}
	}

    void spaceInCondition(){
		switch(condition){
		case Condition.INIT:
			viewManager.hideInfoText ();
			resetCounter ();
			initHammer ();
			switch (group) {
			case Group.PC:
				viewManager.setSubText ("Press <Space> to begin training round");
				break;
			case Group.VR:
				viewManager.setInfoText ("Please move the hammer to get in touch with controls. You can hit by moving it up and down. There will be shown hitpoints.\n");
				viewManager.setSubText ("Press central button to begin training round");				
				break;
			}
			conditionLoopCount = conditonLoops;
			conditionChangable = true;
			break;
		case Condition.TRAINING:
			viewManager.hideInfoText ();
			resetCounter ();
			initHammer ();
			switch (group) {
			case Group.PC:
				viewManager.setSubText ("Press <Space> to end training round and begin experiment");
				break;
			case Group.VR:
				startArea.gameObject.SetActive(false);
				viewManager.setSubText ("Press central button to end training round and begin experiment");
				break;
			}
			conditionLoopCount = conditonLoops;
			conditionChangable = true;
			break;
		case Condition.CONDITION_INSECT:
		case Condition.CONDITION_INSECT_DEATH:
		case Condition.CONDITION_INSECT_KILL:
			startArea.gameObject.SetActive (false);
			hammer.unfreeze ();
			switch (group) {
			case Group.PC:
				viewManager.hideAllMessages();
				break;
			case Group.VR:
				viewManager.hideAllMessages();
				break;
			}
			break;
		case Condition.CONDITION_REPTILE:
		case Condition.CONDITION_REPTILE_DEATH:
		case Condition.CONDITION_REPTILE_KILL:
			startArea.gameObject.SetActive(false);
			hammer.unfreeze ();
			switch (group) {
			case Group.PC:
				viewManager.hideAllMessages();
				break;
			case Group.VR:
				viewManager.hideAllMessages();
				break;
			}
			break;
		case Condition.CONDITION_MAMMAL:
		case Condition.CONDITION_MAMMAL_DEATH:
		case Condition.CONDITION_MAMMAL_KILL:
			hammer.unfreeze ();
			startArea.gameObject.SetActive(false);
			switch (group) {
			case Group.PC:
				viewManager.hideAllMessages();
				break;
			case Group.VR:
				viewManager.hideAllMessages();
				break;
			}
			break;
		case Condition.FINAL:
			Application.Quit();
			break;
		}
	}


	void initHammer(){
		Vector3 resetPosition = hammer.reset ();
		hammer.unfreeze ();
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
		if (!pointsCollectible)
			return;
		hits++;
		hitstotal++;
		if (activeAnimal) {
			logger.logHit (condition,conditonLoops,conditionLoopCount,target,activeAnimal.transform.position,hits);
		} else {
			logger.logHit (condition,conditonLoops,conditionLoopCount,target,animalLastPos,hits);
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

	void resetCondition(){
		switch(condition){
		case Condition.TRAINING:
			activateRandomBox ();
			activateAnimal (0);
			resetCounter ();
			conditionChangable = false;
			break;
		case Condition.CONDITION_MAMMAL:
			break;
		case Condition.CONDITION_INSECT:
			break;
		case Condition.CONDITION_REPTILE:
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
		if (!pointsCollectible)
			return;
		viewManager.setGoalPoints (add); 
		points += add;
		pointstotal += add;
		viewManager.setPoints(points);
		logger.logGoalPoints (add, points);
	}

	public void addHitPoints(int add){
		if (!pointsCollectible)
			return;
		viewManager.setHitPoints (add); 
		points += add;
		pointstotal += add;
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
		animalLastPos = Vector3.zero;
		activeAnimal.SetActive(true);

	}

	public void activateAllAnimals (){
		for (int i=0;i < animals.Length;i++) {
			GameObject animTemp = Instantiate (animals[i]);
			animTemp.transform.SetParent(GameObject.Find ("Animals").transform);
			animTemp.transform.localPosition = Vector3.left * 0.2f * (animals.Length -1)/2 + Vector3.right * 0.2f * i;
			animTemp.SetActive(true);
		}
	}

	public GameObject getActiveAnimal(){
		return activeAnimal;
	}


	public void hit(Vector3 target){
		switch(condition){
		case Condition.INIT:
			addHit (target);
			addHitPoints (-10);
			break;
		case Condition.TRAINING:
			addHit (target);
			addHitPoints (-10);
			break;
		case Condition.CONDITION_INSECT:
		case Condition.CONDITION_INSECT_DEATH:
		case Condition.CONDITION_INSECT_KILL:
			addHit (target);
			if (!dead)
				addHitPoints (-10);
			break;
		case Condition.CONDITION_REPTILE:
		case Condition.CONDITION_REPTILE_DEATH:
		case Condition.CONDITION_REPTILE_KILL:
			addHit (target);
			if (!dead)
				addHitPoints (-10);
			break;
		case Condition.CONDITION_MAMMAL:
		case Condition.CONDITION_MAMMAL_DEATH:
		case Condition.CONDITION_MAMMAL_KILL:
			addHit (target);
			if (!dead)
				addHitPoints (-10);
			break;
		}
	}

	public void hitAnimal(){
		Debug.Log ("Hit animal");
		if (group == Group.VR && !startArea.free)
			return;
		switch(condition){
		case Condition.TRAINING:
			break;
		case Condition.CONDITION_INSECT:
		case Condition.CONDITION_REPTILE:
		case Condition.CONDITION_MAMMAL:
			destroyAnimal ();
			freezeHammer ();
			addHitPoints (-30);
			pointsCollectible = false;
			logger.logEnteredGoal (condition,conditonLoops,conditionLoopCount,"Disappear");
			switch (group) {
			case Group.PC:
				showBothTextInSeconds(2,"Congratulations.","Press <Space> for next round");
				break;
			case Group.VR:
				showBothTextInSeconds(2,"Congratulations.","Press central button for next round");
				break;
			}
			conditionChangable = true;
			break;
		case Condition.CONDITION_REPTILE_DEATH:
		case Condition.CONDITION_INSECT_DEATH:
		case Condition.CONDITION_MAMMAL_DEATH:
			if (dead)
				return;
			logger.logEnteredGoal (condition,conditonLoops,conditionLoopCount,"Kill");
			killAnimal ();
			freezeHammer ();
			addHitPoints(-30);
			pointsCollectible = false;
			switch (group) {
			case Group.PC:
				showBothTextInSeconds(2,"Congratulations.","Press <Space> for next round");
				break;
			case Group.VR:
				showBothTextInSeconds(2,"Congratulations.","Press central button for next round");
				break;
			}
			conditionChangable = true;
			break;
		case Condition.CONDITION_REPTILE_KILL:
		case Condition.CONDITION_INSECT_KILL:
		case Condition.CONDITION_MAMMAL_KILL:
			if (dead)
				return;
			killAnimal ();
			freezeHammer();
			addGoalPoints (50);
			logger.logEnteredGoal (condition,conditonLoops,conditionLoopCount,"Kill");
			switch (group) {
			case Group.PC:
				showBothTextInSeconds(2,"Congratulations. You killed the Animal.","Press <Space> for next round");
				break;
			case Group.VR:
				showBothTextInSeconds(2,"Congratulations. You killed the Animal.","Press central button for next round");
				break;
			}
			conditionChangable = true;
			break;
		}
	}

	public void destroyAnimal(){
		if (activeAnimal) {
			logger.logDestroyAnimal (activeAnimal.name);
			animalLastPos = activeAnimal.transform.position;
			Destroy (activeAnimal);
		}
	}

	public void killAnimal(){
		dead = true;
		activeAnimal.GetComponent<Animator> ().SetTrigger ("Die");
		switch (condition) {
		case Condition.CONDITION_MAMMAL:
		case Condition.CONDITION_MAMMAL_DEATH:
		case Condition.CONDITION_MAMMAL_KILL:
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

		Debug.Log ("Hit " + other.name);

		if (other.name == "Kopf")
			hitAnimal ();
		switch(condition){
		case Condition.TRAINING:
			if (other.name == "Trigger") {
				addGoalPoints (30);
				logger.logEnteredGoal (condition,conditonLoops,conditionLoopCount,"Box");
				viewManager.setInfoText ("Congratulations. It entered the box.");
				//activeAnimal.GetComponent<Rigidbody>().useGravity = false;
			}

			if (other.name == "Floor" || other.name == "outside") {
				resetConditionInSeconds (2);
				logger.logEnteredGoal (condition,conditonLoops,conditionLoopCount,"Floor");
				viewManager.setInfoText ("It fell off the Table. Please move the ball into the boxes.");
				switch (group) {
				case Group.PC:
					viewManager.setSubText ("Press <Space> to play again");
					break;
				case Group.VR:
					viewManager.setSubText ("Press central button to play again");
					break;
				}
				conditionChangable = false;
			}
			break;
		case Condition.CONDITION_MAMMAL:
		case Condition.CONDITION_INSECT:
		case Condition.CONDITION_REPTILE:
		case Condition.CONDITION_REPTILE_DEATH:
		case Condition.CONDITION_INSECT_DEATH:
		case Condition.CONDITION_MAMMAL_DEATH:
			if (other.name == "Trigger") {
				addGoalPoints (30);
				logger.logEnteredGoal (condition,conditonLoops,conditionLoopCount,"Box");
				viewManager.setInfoText ("Congratulations. It entered the box.");
				switch (group) {
				case Group.PC:
					viewManager.setSubText ("Press <Space> for next round");
					break;
				case Group.VR:
					viewManager.setSubText ("Press central button for next round");
					break;
				}
				conditionChangable = true;

			}

			if (other.name == "Floor" || other.name == "outside") {
				logger.logOffTable (activeAnimal.transform.position);
				Destroy (activeAnimal);
				logger.logEnteredGoal (condition,conditonLoops,conditionLoopCount,"Floor");
				viewManager.setInfoText ("It fell off the Table.");
				switch (group) {
				case Group.PC:
					viewManager.setSubText ("Press <Space> for next round");
					break;
				case Group.VR:
					viewManager.setSubText ("Press central button for next round");
					break;
				}
				conditionChangable = true;
			}
			break;
		case Condition.CONDITION_REPTILE_KILL:
		case Condition.CONDITION_INSECT_KILL:
		case Condition.CONDITION_MAMMAL_KILL:
			if (other.name == "Floor" || other.name == "outside") {
				logger.logOffTable (activeAnimal.transform.position);
				Destroy (activeAnimal);
				logger.logEnteredGoal (condition,conditonLoops,conditionLoopCount,"Floor");
				viewManager.setInfoText ("Congratulations.");
				switch (group) {
				case Group.PC:
					viewManager.setSubText ("Press <Space> for next round");
					break;
				case Group.VR:
					viewManager.setSubText ("Press central button for next round");
					break;
				}
				conditionChangable = true;
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

	void changeConditionInSeconds(int seconds){
		StartCoroutine (IEchangeConditionInSeconds (seconds));

	}

	IEnumerator IEchangeConditionInSeconds(int seconds) {
		yield return new WaitForSeconds(seconds);
		changeCondition (condition + 1);
	}

	void resetConditionInSeconds(int seconds){
			StartCoroutine (IEresetConditionInSeconds (seconds));

	}

	IEnumerator IEresetConditionInSeconds(int seconds) {
		yield return new WaitForSeconds(seconds);
		resetCondition ();
	}

	void OnApplicationQuit() {
		if(logger.initialized)
			logger.close ();
	}
}