using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

/*
Helper class to hold all entries, being sortable
*/
public class LogEntry: System.IComparable<LogEntry>{
	public System.DateTime timestamp {get;set;}
	public string text {get;set;}
	public int CompareTo(LogEntry other) {
		return timestamp.CompareTo(other.timestamp);
	}

	public LogEntry(string text){
		this.timestamp = System.DateTime.Now;
		this.text = text;
	}
}

public class Logger{
	string folder = "results";
	string filename = "";
	string filenameSummary = "results";
	string suffix = ".log";
	string filepathSummary;
	string filepath;
	string id;
	Manager.Group group;
	public bool initialized = false;
	List<string> buffer = new List<string>();
	TextWriter w;
	TextWriter wSummary;
	List<LogEntry> bufferSummary = new List<LogEntry>(); 

	public Logger(){
		
	}

	public Logger(string text){
		buffer.Add (text);
	}
		
	public void init(string id, Manager.Group group){
		
		this.id = id;
		this.group = group;
		if (!Directory.Exists(folder)) 
			Directory.CreateDirectory(folder);
		filename = "Experiment_" + id;
		filepath = folder + "/" + filename + suffix;
		filepathSummary = folder + "/" + filenameSummary + suffix;
		if(File.Exists(filepath))
			filename += "_2";
			filepath = folder + "/" + filename + suffix;
		w = TextWriter.Synchronized(File.AppendText(filepath));
		if (!File.Exists (filepathSummary)) {
			wSummary = TextWriter.Synchronized(File.AppendText(filepathSummary));
			wSummary.WriteLine ("Date;Time;Participant;Group;Condition;Trials;Trial;Hit;Distance;Goal");
		} else {
			wSummary = TextWriter.Synchronized(File.AppendText(filepathSummary));
		}

		initialized = true;

		//Create summary of all gameObjects:
		GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
		write ("name" + ";" + "position" + ";" + "rotation" + ";" + "scale");
		foreach(GameObject go in allObjects){
			write (go.name + ";" + go.transform.position.ToString() + ";" + go.transform.rotation.eulerAngles.ToString() + ";" + go.transform.localScale.ToString());
		}
		linebreak();

		//Create Init-Data about current participant for re-checking data if necessary
		write("Participant:" + id);
		foreach (string s in buffer)
			write (s);
		flush ();
		Debug.Log("New file " + filepath + " created.");

	}

	//Wirtes line to perticipant logfile
	public void write(string s){
		if (initialized) {
			w.WriteLine (s);
		} else {
			buffer.Add (s);
		}
	}

	//Writes line to buffer for main logfile
	public void writeSummary(string id, Manager.Group group, Manager.Condition condition,int trials, int trial, string hit, string distance, string goal){
		bufferSummary.Add(new LogEntry(id + ";" + group.ToString() + ";" + condition.ToString() + ";" + trials.ToString() + ";" + trial.ToString() + ";" + hit + ";" + distance + ";" + goal));
	}

	//Sorts data for main logfile and appends it.
	public void appendSummary(){
		bufferSummary.Sort ();
		foreach (LogEntry entry in bufferSummary) {
			wSummary.WriteLine(entry.timestamp.ToString("yyyy-MM-dd") + ";" +  entry.timestamp.ToString("HH:mm:ss.fff")+ ";" + entry.text);
		}
		wSummary.Flush ();
	}

	//flushes participant logfile
	private void flush(){
		if (initialized) {
			w.Flush ();
		}
	}

	//writes data if condition ends to participant logfile
	public void endCondition(string state){
		write("End Time:" + System.DateTime.Now.ToString());
		flush ();
	}

	//wirtes data if condition begins to participant logfile
	public void beginCondition(Manager.Condition condition, int trials){
		linebreak ();
		write("Condition:" + condition.ToString());
		write("Start Time:" + System.DateTime.Now.ToString());
		write("Trials:" + trials.ToString());
		flush ();
		if (condition == Manager.Condition.INIT || condition == Manager.Condition.TRAINING)
			return;
		writeSummary (id, group, condition, trials, 1, "-", "-", "-");
	}

	//writes data if trial begins to participant logfile
	public void beginTrial(Manager.Condition condition,int trials, int trial){
		linebreak (40);
		write("Trial:" + trial);
		write("Start Time:" + System.DateTime.Now.ToString());
		if (condition == Manager.Condition.INIT || condition == Manager.Condition.TRAINING)
			return;
		flush ();
		writeSummary (id, group, condition, trials, trial, "-", "-", "-");
	}

	//creates linebreaks in participant logfile
	void linebreak(){
		linebreak (80);
	}
	void linebreak(int length){
		string s = "";
		write(s.PadLeft(length,'-'));
	}

	//writes data if animal jumps off table to participant logfile
	public void logOffTable(Vector3 position){
		write ("OffTable;" + System.DateTime.Now.ToString() + ";" + position.ToString());
	}

	//writes data if hammer is reset to participant logfile
	public void logResetHammer(Vector3 position){
		write ("ResetHammer;" + System.DateTime.Now.ToString() + ";" + position.ToString());
	}

	//writes data if points are reset to participant logfile
	public void logResetPoints(int points, int hits){
		write ("ResetHitCounter;" + System.DateTime.Now.ToString() + ";" + points.ToString() + ";" + hits.ToString());
	}

	//writes data if hammer is hitting something to participant logfile
	public void logHit(Manager.Condition condition,int trials,int trial,Vector3 position, int count){
		if (condition != Manager.Condition.INIT && condition != Manager.Condition.TRAINING)
			writeSummary (id, group, condition, trials,trial,count.ToString(),"-","-");
		write ("Hit;" + System.DateTime.Now.ToString() + ";" + count.ToString() + ";" + position.ToString());
	}

	//writes data if hammer is hitting something to participant logfile
	public void logHit(Manager.Condition condition, int trials, int trial, Vector3 position, Vector3 animalPosition, int count){
		float distance  = Vector3.Distance(Vector3.Scale(position,new Vector3(1,0,1)),Vector3.Scale(animalPosition,new Vector3(1,0,1)));
		if (condition != Manager.Condition.INIT && condition != Manager.Condition.TRAINING)
			writeSummary (id, group, condition, trials, trial,count.ToString(),distance.ToString(),"-");
		write ("Hit;" + System.DateTime.Now.ToString() + ";" + count.ToString() + ";" + position.ToString() + ";" + animalPosition.ToString() + ";" + distance.ToString());
	}

	//writes hit points to participant logfile
	public void logHitPoints(int points, int total){
		write ("HitPoints;" + System.DateTime.Now.ToString() + ";" + points.ToString() + ";" + total.ToString());
	}

	//writes goal points to participant logfile
	public void logGoalPoints(int points, int total){
		write ("GoalPoints;" + System.DateTime.Now.ToString() + ";" + points.ToString() + ";" + total.ToString());
	}

	//writes destroy animal event to participant logfile
	public void logDestroyAnimal(string name){
		write ("DestroyAnimal;" + System.DateTime.Now.ToString() + ";" + name);
	}

	//writes kill animal event to participant logfile
	public void logKillAnimal(string name){
		write ("KillAnimal;" + System.DateTime.Now.ToString() + ";" + name);
	}	

	//writes entered goal event to participant logfile
	public void logEnteredGoal(Manager.Condition condition, int trial, int trials, string goal){
		write ("EnteredGoal;" + System.DateTime.Now.ToString());
		if (condition == Manager.Condition.INIT || condition == Manager.Condition.TRAINING)
			return;
		writeSummary (id, group, condition, trials, trial,"-","-",goal);
	}

	//writes goal position to participant logfile
	public void logBoxPosition(int num){
		write ("BoxActivate;" + System.DateTime.Now.ToString() + ";" + num.ToString());
	}

	//writes keyboard event to participant logfile
	public void logUserEntry(string key){
		write ("UserEntry;" + System.DateTime.Now.ToString() + ";" + key);
	}

	//Closes file writer
	public void close(){
		w.Close ();
		wSummary.Close ();
	}
}
