﻿using UnityEngine;
using System.Collections;

public class PolvoDerrape : MonoBehaviour {

	private float duracion = 0.8f;
	private float direccion;
	private float y;
	Transform thisTransform;

	void Start () 
	{
		thisTransform = GetComponent <Transform> ();
		direccion = GetComponent <PlayerControl> ().GetPlayerDirection();

		if (direccion < 0f)
		{
			y += 90f;
		}
		else
		{
			y -= 90f;
		}

		thisTransform.rotation = Quaternion.Euler (0,y,0); 

		Destroy (gameObject, duracion);
	}
}
