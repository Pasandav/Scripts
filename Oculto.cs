using UnityEngine;
using System.Collections;

public class HiddeTrap : MonoBehaviour {


	void OnTriggerEnter2D (Collider2D other)
	{

		if (other.tag.ToLower () == "player") {


		}
	}


}
