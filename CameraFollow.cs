﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

public Transform target;
public float damping = 1f;
public float lookAheadFactor = 3f;
public float lookAheadReturnSpeed = 0.5f;
public float lookAheadMoveThreshold = 0.1f;

float offsetZ;
	Vector3 lastTargetPosition;
	Vector3 currentVelocity;
	Vector3 lookAheadPos;

 

	// Use this for initialization
	void Start () {
	lastTargetPosition = target.position;
	offsetZ = (transform.position - target.position).z;
	transform.parent = null;
	}
	
	// Update is called once per frame
	void Update ()
	{
		//this.transform.position = new Vector3 (cam.position.x , this.transform.position.y, this.transform.position.z);
		float xMoveDelta = (target.position - lastTargetPosition).x;
		bool updateLookAheadTarget = Mathf.Abs (xMoveDelta) > lookAheadMoveThreshold;

		if (updateLookAheadTarget) {
			lookAheadPos = lookAheadFactor * Vector3.right * Mathf.Sign (xMoveDelta);
		} else {
			lookAheadPos = Vector3.MoveTowards (lookAheadPos, Vector3.zero, Time.deltaTime * lookAheadReturnSpeed);
		}

		Vector3 aheadTargetPos = target.position + lookAheadPos + Vector3.forward * offsetZ;
		Vector3 newPos = Vector3.SmoothDamp (transform.position, aheadTargetPos, ref currentVelocity, damping);
		if (target.position.y < 1f) {
			newPos = Vector3.SmoothDamp (new Vector3 (transform.position.x, target.position.y + 3.2f,transform.position.z),target.position,ref currentVelocity, damping);
			//newPos.y = target.position.y + 3.2f;
		}
		transform.position = newPos;
		lastTargetPosition = target.position;

	}
}
