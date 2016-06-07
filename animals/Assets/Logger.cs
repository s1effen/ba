using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class Logger{

	string folder = "results";
	string filename = "";
	string suffix = ".log";
	string filepath;
	public bool initialized = false;
	List<string> buffer = new List<string>();
	TextWriter w;

	public Logger(){
	}
	public Logger(string text){
		buffer.Add (text);
	}
		
	public void init(string id){
		if (!Directory.Exists(folder)) 
			Directory.CreateDirectory(folder);
		filename = "Experiment_" + id;
		filepath = folder + "/" + filename + suffix;
		if(File.Exists(filepath))
			filename += "_2";
			filepath = folder + "/" + filename + suffix;
		w = TextWriter.Synchronized(File.AppendText(filepath));
		initialized = true;

		GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
		write ("name" + ";" + "position" + ";" + "rotation" + ";" + "scale");
		foreach(GameObject go in allObjects){
			write (go.name + ";" + go.transform.position.ToString() + ";" + go.transform.rotation.eulerAngles.ToString() + ";" + go.transform.localScale.ToString());
		}
		linebreak();

		write("Participant:" + id);
		foreach (string s in buffer)
			write (s);
		flush ();
		Debug.Log("New file " + filepath + " created.");

	}

	public void write(string s){
		if (initialized) {
			w.WriteLine (s);
		} else {
			buffer.Add (s);
		}
	}

	private void flush(){
		if (initialized) {
			w.Flush ();
		}
	}

	public void endCondition(string state){
		write("End Time:" + System.DateTime.Now.ToString());
		flush ();
	}

	public void beginCondition(string state, int trials){
		linebreak ();
		write("Condition:" + state);
		write("Start Time:" + System.DateTime.Now.ToString());
		write("Trials:" + trials.ToString());
		flush ();
	}

	public void beginTrial(string state, int trial){
		linebreak (40);
		write("Trial:" + trial);
		write("Start Time:" + System.DateTime.Now.ToString());
		flush ();
	}

	void linebreak(){
		linebreak (80);
	}
	void linebreak(int length){
		string s = "";
		write(s.PadLeft(length,'-'));
	}

	public void logOffTable(Vector3 position){
		write ("OffTable;" + System.DateTime.Now.ToString() + ";" + position.ToString());
	}

	public void logResetHammer(Vector3 position){
		write ("ResetHammer;" + System.DateTime.Now.ToString() + ";" + position.ToString());
	}
	public void logResetPoints(int points, int hits){
		write ("ResetHitCounter;" + System.DateTime.Now.ToString() + ";" + points.ToString() + ";" + hits.ToString());
	}
	public void logHit(Vector3 position, int count){
		write ("Hit;" + System.DateTime.Now.ToString() + ";" + count.ToString() + ";" + position.ToString());
	}

	public void logHit(Vector3 position, Vector3 animalPosition, int count){
		float distance  = Vector3.Distance(Vector3.Scale(position,new Vector3(1,0,1)),Vector3.Scale(animalPosition,new Vector3(1,0,1)));
		write ("Hit;" + System.DateTime.Now.ToString() + ";" + count.ToString() + ";" + position.ToString() + ";" + animalPosition.ToString() + ";" + distance.ToString());
	}

	public void logHitPoints(int points, int total){
		write ("HitPoints;" + System.DateTime.Now.ToString() + ";" + points.ToString() + ";" + total.ToString());
	}
	public void logGoalPoints(int points, int total){
		write ("GoalPoints;" + System.DateTime.Now.ToString() + ";" + points.ToString() + ";" + total.ToString());
	}
	public void logDestroyAnimal(string name){
		write ("DestroyAnimal;" + System.DateTime.Now.ToString() + ";" + name);
	}
	public void logKillAnimal(string name){
		write ("KillAnimal;" + System.DateTime.Now.ToString() + ";" + name);
	}	
	public void logEnteredGoal(){
		write ("EnteredGoal;" + System.DateTime.Now.ToString());
	}
	public void logBoxPosition(int num){
		write ("BoxActivate;" + System.DateTime.Now.ToString() + ";" + num.ToString());
	}
	public void logUserEntry(string key){
		write ("UserEntry;" + System.DateTime.Now.ToString() + ";" + key);
	}
	public void close(){
		w.Close ();
	}
}
