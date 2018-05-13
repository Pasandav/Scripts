﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class FinDeNivel : MonoBehaviour {

	public Camera camara;
	private int nivel;

	// Use this for initialization
	void Start () 
	{
		// Almacenamos el numero entero de la escena activa.
		nivel= SceneManager.GetActiveScene().buildIndex;

		// Si el objeto tien un hijo que llama "Main Camera", cogemos su componenete
		//if ( transform.FindChild("Main Camera") != null) { camara = transform.FindChild ("Main Camera").GetComponent <Camera>(); }
	}
	
	// Update is called once per frame
	void Update () 
	{
		//if (this.GetComponent<PlayerControl>().finalDeNivel) { this.transform.Translate (Vector3.right * 0.01f ); }
	}

	void OnTriggerEnter2D (Collider2D other){

		if (other.tag == "EndOfLevel")
		{
			AudioSource sonido = other.gameObject.GetComponent <AudioSource>();
			AudioClip final = sonido.clip;
			sonido.PlayOneShot (final);
			camara.transform.parent = null;
			StartCoroutine (siguienteNivel());

		}
	}


	private IEnumerator siguienteNivel ()

	{

		yield return new WaitForSeconds (2.0f);
		SceneManager.LoadScene (nivel+1);
	}
}
