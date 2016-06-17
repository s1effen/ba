using UnityEngine;
using System.Collections;

public class pptAxis : MonoBehaviour {

	public GameObject objectGetPositionFrom;
	public bool position;
	public Axis posX;
	public Axis posY;
	public Axis posZ;

	public bool orientation;
	public bool orientationX;
	public Axis orientX;
	public float a;
	public float offsetX = 0f;
	public bool orientationY;
	public Axis orientY;
	public float offsetY = 0f;
	public bool orientationZ;
	public Axis orientZ;
	public float offsetZ = 0f;


	public enum Axis{
		X,
		Xneg,
		Y,
		Yneg,
		Z,
		Zneg
	};
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (position) {
			transform.position = new Vector3 (pos (posX), pos (posY), pos (posZ));
		}
		if (orientation) {
			float x = 0f, y = 0f, z = 0f;
			if (orientationX)
				x = orient (orientX) + offsetX;
			if (orientationY)
				y = orient (orientY) + offsetY;
			if (orientationZ)
				z = orient (orientZ) + offsetZ;			
			transform.rotation = Quaternion.Euler(new Vector3 (x,y,z));
		}

	}

	float pos(Axis a){
		switch (a) {
		case Axis.X:
			return objectGetPositionFrom.transform.position.x;
			break;
		case Axis.Xneg:
			return -objectGetPositionFrom.transform.position.x;
			break;
		case Axis.Y:
			return objectGetPositionFrom.transform.position.y;
			break;
		case Axis.Yneg:
			return -objectGetPositionFrom.transform.position.y;
			break;
		case Axis.Z:
			return objectGetPositionFrom.transform.position.z;
			break;
		case Axis.Zneg:
			return -objectGetPositionFrom.transform.position.z;
			break;
		default:
			return 0;
			break;
		}
	}

	float orient(Axis a){
		switch (a) {
		case Axis.X:
			return objectGetPositionFrom.transform.rotation.eulerAngles.x;
			break;
		case Axis.Xneg:
			return -1 * objectGetPositionFrom.transform.rotation.eulerAngles.x;
			break;
		case Axis.Y:
			return objectGetPositionFrom.transform.rotation.eulerAngles.y;
			break;
		case Axis.Yneg:
			return -1 * objectGetPositionFrom.transform.rotation.eulerAngles.y;
			break;
		case Axis.Z:
			return objectGetPositionFrom.transform.rotation.eulerAngles.z;
			break;
		case Axis.Zneg:
			return -1 * objectGetPositionFrom.transform.rotation.eulerAngles.z;
			break;
		default:
			return 0;
			break;
		}
	}
}
