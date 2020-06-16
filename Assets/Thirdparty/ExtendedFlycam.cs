using UnityEngine;
using System.Collections;
 
public class ExtendedFlycam : MonoBehaviour
{
 
	/*
	EXTENDED FLYCAM
		Desi Quintans (CowfaceGames.com), 17 August 2012.
		Based on FlyThrough.js by Slin (http://wiki.unity3d.com/index.php/FlyThrough), 17 May 2011.
 
	LICENSE
		Free as in speech, and free as in beer.
 
	FEATURES
		WASD/Arrows:    Movement
		          Q:    Climb
		          E:    Drop
                      Shift:    Move faster
                    Control:    Move slower
                        End:    Toggle cursor locking to screen (you can also press Ctrl+P to toggle play mode on and off).
	*/

	//The only things I changed in this script were updating the cursor/lock functionality and dividing Time.deltaTime by the timeScale.
	public float cameraSensitivity = 90;
	public float climbSpeed = 4;
	public float normalMoveSpeed = 10;
	public float slowMoveFactor = 0.25f;
	public float fastMoveFactor = 3;
 
	private float rotationX;
	private float rotationY;
	private bool locked = true;
 
	void Start ()
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		rotationX = transform.rotation.x;
		rotationY = transform.rotation.y;
	}
 
	void Update ()
	{
		if (locked) 
		{
			rotationX += Input.GetAxis("Mouse X") * cameraSensitivity * (Time.deltaTime / Time.timeScale);
			rotationY += Input.GetAxis("Mouse Y") * cameraSensitivity * (Time.deltaTime / Time.timeScale);
			rotationY = Mathf.Clamp (rotationY, -90, 90);
	
			transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
			transform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);
	
			if (Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift))
			{
				transform.position += transform.forward * (normalMoveSpeed * fastMoveFactor) * Input.GetAxis("Vertical") * (Time.deltaTime / Time.timeScale);
				transform.position += transform.right * (normalMoveSpeed * fastMoveFactor) * Input.GetAxis("Horizontal") * (Time.deltaTime / Time.timeScale);
			}
			else if (Input.GetKey (KeyCode.LeftControl) || Input.GetKey (KeyCode.RightControl))
			{
				transform.position += transform.forward * (normalMoveSpeed * slowMoveFactor) * Input.GetAxis("Vertical") * (Time.deltaTime / Time.timeScale);
				transform.position += transform.right * (normalMoveSpeed * slowMoveFactor) * Input.GetAxis("Horizontal") * (Time.deltaTime / Time.timeScale);
			}
			else
			{
				transform.position += transform.forward * normalMoveSpeed * Input.GetAxis("Vertical") * (Time.deltaTime / Time.timeScale);
				transform.position += transform.right * normalMoveSpeed * Input.GetAxis("Horizontal") * (Time.deltaTime / Time.timeScale);
			}
		}
 
 
		if (Input.GetKey (KeyCode.Q)) {transform.position += transform.up * climbSpeed * (Time.deltaTime / Time.timeScale);}
		if (Input.GetKey (KeyCode.E)) {transform.position -= transform.up * climbSpeed * (Time.deltaTime / Time.timeScale);}
 
		if (Input.GetKeyDown (KeyCode.Mouse1))
		{
			locked = !locked;
			Cursor.lockState = (locked) ? CursorLockMode.Locked : CursorLockMode.None;
			Cursor.visible = !locked;
		}
	}
}