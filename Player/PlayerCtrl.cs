using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour {
	
	private Rigidbody2D thisRigid;
	private SpriteRenderer thisSprite;
	private Animator thisAnimator;


	[Header ("Variables de movimiento")]
	public float fuerzaHorizontal;
	public float movimientoHorizontal;
	
    [Header ("Checks de contacto")]
	public bool tocaSuelo;

	[Header ("Tocando suelo")]
	public Vector3 chkSuelo;
	public float radioChkSuelo = 0.04f;
	public LayerMask  capasDeColision;

	[Header ("Salto")]
	public float altura;
	public int caida;
	private int hashSuelo = Animator.StringToHash("Ground");

	void Awake ()
	{
		fuerzaHorizontal = 35f;



	}
	void Start ()

	{
		
		//thisRigid = GetComponent <Rigidbody2D> ();
		thisSprite = transform.Find ("Sprite").GetComponent<SpriteRenderer> ();
		thisAnimator = transform.Find ("Sprite").GetComponent<Animator> ();

	}
	
	// Update is called once per frame
	void Update () {

		ckeckeaSuelo ();
	}

	void FixedUpdate (){

		movimientoHorizontal = Input.GetAxisRaw ("Horizontal");

	}


	void ckeckeaSuelo ()
	{
		
		chkSuelo.x = thisSprite.bounds.center.x;
		chkSuelo.y = thisSprite.bounds.min.y;

		Debug.DrawRay (chkSuelo, Vector3.down, Color.red);
		tocaSuelo = Physics2D.OverlapCircle (chkSuelo, radioChkSuelo, capasDeColision);
		if (tocaSuelo) 
		{
			if (caida > 5)
			{
				Debug.Log ("resta energía por caida");
			}
			altura = 0f;
			caida = 0;

		} 
		else 
		{
			altura = Physics2D.Raycast (chkSuelo, Vector2.down).distance;

		}

		if (Mathf.RoundToInt (altura) > caida) 
		{
			caida = Mathf.RoundToInt(altura);
		}

		thisAnimator.SetBool (hashSuelo, tocaSuelo);
	}
}
