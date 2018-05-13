using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour {

public Transform[] backgrounds;			// Array list of all the backgrounds and foregrounds to be parallaxes
private float[] parallaxScales;			// The proportion of the camera's movement to move the background by
public float smoothing = 1f;					// How is going the parallax is to be. Make sure to set this above 0

	
private  Transform cam;					// Reference to transform main camera
private Vector3 previousCamPos;			// The position of the camera in the previous frame.


void Awake ()
	{
		cam = Camera.main.transform;
	}
	// Use this for initialization
	void Start ()
	{
		previousCamPos = cam.position;

		parallaxScales = new float[backgrounds.Length];

		for (int i = 0; i < backgrounds.Length; i++) {

			parallaxScales[i]= backgrounds[i].position.z * -1;
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		for (int i = 0; i < backgrounds.Length; i++) {

			float parallax = (previousCamPos.x - cam.position.x) * parallaxScales [i];

			float backgroundTargetPosX = backgrounds[i].position.x + parallax;

			Vector3 backgroundTargetPos  = new Vector3(backgroundTargetPosX, backgrounds[i].position.y, backgrounds[i].position.z);

			backgrounds[i].position = Vector3.Lerp (backgrounds[i].position, backgroundTargetPos, smoothing * Time.deltaTime);


		}
		previousCamPos= cam.position;
	}
}
