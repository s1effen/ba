using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ViewManager : MonoBehaviour {


	public Text pointText;
	public Text hitText;
	public GameObject canvas;
	public GameObject hitPointText;
	public GameObject infoCanvas;
	public GameObject subCanvas;
	public Color hitsColor;
	public Color hitPointsColor;
	public Color goalPointsColor;

	private bool hitPointAnimation;

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
		hitPointText.gameObject.SetActive(true);
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
		setHitPoints (value, goalPointsColor, 0);
	}

	public void setHitPoints(int value)
	{
		setHitPoints (value, hitPointsColor,2);
	}
	public void setHitPoints(int value, Color color, int speed)
	{
		if (!hitPointText.gameObject.activeSelf)
			hitPointText.gameObject.SetActive (true);
		GameObject hitPoint = Instantiate (hitPointText);
		Text points = hitPointText.GetComponent<Text> ();
		hitPoint.GetComponent<HitPointText> ().speed = speed;
		points.color = color;
		points.text = "+ " + value.ToString ();
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
