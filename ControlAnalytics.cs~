using UnityEngine;
using System.Collections.Generic;
// Reference the Unity Analytics namespace
using UnityEngine.Analytics;

public class ControlAnalytics : MonoBehaviour {

public	float version = 1.0f;

	void Start ()
	{
		RegistraInicio ();
	}

	public void RegistraInicio ()
	{
		Debug.Log ("Registra Inicio!");
		Analytics.CustomEvent ("gameStart", new Dictionary<string, object>
		                       
		   { });
	}




	public void RegistraFin ()
	{
		Debug.Log ("Registra Fin!");
		float secsJuego = Time.time;
		int energyPlayer = GameObject.FindWithTag ("Player01").GetComponent<PlayerControl> ().energia;

		if (GameObject.Find ("JefeDeObra") != null){
			//int energyJefe = GameObject.Find ("JefeDeObra").GetComponent<JefeDeObra> ().energia;
		

		Analytics.CustomEvent ("gameOver", new Dictionary <string, object>
		                      {
			{"seconds" , secsJuego},
			{"energyPlayer", energyPlayer},
			//{"energyEnemy" , energyJefe}
		});


		}
	}
}