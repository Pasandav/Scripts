using UnityEngine;
using System.Collections;

public class Chispazo : MonoBehaviour {



	void OnCollisionEnter2D (Collision2D otro){
		if (otro.gameObject.tag == "Player"){
		Debug.Log (otro.gameObject.name);
		}
	}

	void OnTriggerEnter2d (Collider2D otro){

		Debug.Log (otro.gameObject.name);
	}
}
