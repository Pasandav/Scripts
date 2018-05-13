using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour {

	void Update () {
		if (Input.GetKey (KeyCode.Space))
		{
			
			//Application.LoadLevel("Nivel01");
			SceneManager.LoadScene ("Nivel01");
		}
	
		if (Input.GetKey (KeyCode.Escape)){

			Application.CancelQuit ();
		}
	
	}
}
