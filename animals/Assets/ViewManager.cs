using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ViewManager : MonoBehaviour {


	public Text pointText;
	public Text hitText;
	public GameObject canvas;
	public GameObject hitPointTextPrefab;
	public GameObject infoCanvas;
	public GameObject subCanvas;
	public GameObject inputCanvas;
	public Color hitsColor;
	public Color hitPointsColor;
	public Color goalPointsColor;
	private bool hitPointAnimation;
	private System.EventHandler handler;

	// Use this for initialization
	void Start () {
		hideCanvas(infoCanvas);
		hideCanvas(subCanvas);
		hitText.color = hitsColor;
		pointText.color = hitPointsColor;
	}
	
	// Update is called once per frame
	void Update () {

	}

	public 	bool activeMessageBox(){
		return infoCanvas.activeSelf;
	}

	public void hideAllMessages(){
		hideCanvas (inputCanvas);
		hideCanvas(infoCanvas);
		hideCanvas(subCanvas);
	}

	public void hideInfoText(){
		hideCanvas(infoCanvas);
	}

	public void showTotalPoints()
	{
		pointText.gameObject.SetActive(true);
		hitText.gameObject.SetActive(true);
	}

	public void showHitPoints()
	{
		pointText.gameObject.SetActive(true);
	}

	public void setPoints(int value)
	{
		if (!pointText.gameObject.activeSelf)
			pointText.gameObject.SetActive (true);
		pointText.text = "Points: " + value.ToString ();
	}

	public void setHits(int value)
	{
		if (!hitText.gameObject.activeSelf)
			hitText.gameObject.SetActive (true);
		hitText.text = "Hits: " + value.ToString ();
	}

	public void setGoalPoints(int value)
	{
		setHitPoints (value, goalPointsColor, 1);
	}

	public void setHitPoints(int value)
	{
		setHitPoints (value, hitPointsColor,2);
	}
	public void setHitPoints(int value, Color color, int speed)
	{
		GameObject hitPoint = Instantiate (hitPointTextPrefab);
		Text points = hitPoint.GetComponent<Text> ();
		hitPoint.GetComponent<HitPointText> ().speed = speed;
		points.color = color;
		string prefix = "+ ";
		if (value < 0)
			prefix = "";
		points.text = prefix + value.ToString ();
		hitPoint.transform.SetParent (canvas.transform);
		hitPoint.SetActive (true);
	}
		

	public void setInfoText(string text){
		if (!infoCanvas.activeSelf)
			infoCanvas.SetActive (true);
		setText (infoCanvas, text);
	}

	public void setSubText(string text){
		if (!subCanvas.activeSelf)
			subCanvas.SetActive (true);
		setText (subCanvas, text);
	}

	//------- User Input

	public void getTextInput(string placeholder,System.EventHandler textHandler){
		if (!inputCanvas.activeSelf)
			inputCanvas.SetActive (true);
		setText (inputCanvas, placeholder);
		handler = textHandler;
	}

	public void textInput(InputField field){
		InputEventArgs args = new InputEventArgs();
		args.input = field.text;
		handler.Invoke (this, args);
	}
		
	public class InputEventArgs : System.EventArgs
	{
		public string input { get; set; }
	}

	//------- User Input

	void hideCanvas(GameObject canvas){
		canvas.SetActive(false);
	}
	void setText(GameObject panel, string text){
		Text textField = panel.GetComponentInChildren<Text> ();
		if (textField == null)
			return;
		textField.text = text;
		if(!panel.activeSelf)
			panel.SetActive(true);
	}

}
