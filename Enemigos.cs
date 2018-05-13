using UnityEngine;
using System.Collections;

public class Enemigos : MonoBehaviour {


	public Transform [] transformPeones;
	public Collider2D [] colliderPeones;
	// Use this for initialization

	void Start () {
		transformPeones = new Transform[transform.childCount];

		int cuantos = 0; 
		foreach ( Transform transformEnemigo in transform)
		{

			transformPeones [cuantos++] = transformEnemigo;
		}

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
