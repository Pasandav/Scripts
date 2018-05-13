using UnityEngine;
using System.Collections;

public class ControlLadrillo : MonoBehaviour {

	/**
	
	public Animator ladrilloAnim;	


	private Rigidbody2D ladrilloRigid;





	AudioSource sonido;
	public AudioClip clipChocaLadrillo;
	public AudioClip clipSaleLadrillo;


	public ControlPlayer01 ScriptPlayer;

	JefeDeObra ScriptJefe;

	public bool lanzamiento;


	public float fuerzaLanzamiento; 

	public int desapareceHash = Animator.StringToHash ("Desaparece");

	void Start () 
	{
		ladrilloRigid = this.GetComponent <Rigidbody2D>();

		ladrilloAnim = this.transform.FindChild ("SpriteLadrillo"). GetComponent  <Animator> ();





		ScriptPlayer = GameObject.Find ("Player"). GetComponent <ControlPlayer01> ();

		ScriptJefe = GameObject.Find ("JefeDeObra").GetComponent <JefeDeObra> ();


		sonido = this.GetComponent <AudioSource> ();



	

	}

	void Update ()
	{

		if (ScriptJefe.lanzaLadrillo.Equals (true)) 
		{
			Debug.Log (ScriptJefe.name.ToString() +  "ANTES:" + ScriptJefe.lanzaLadrillo);
			ScriptJefe.lanzaLadrillo =(false);
			Debug.Log ("DESPUES:" + ScriptJefe.lanzaLadrillo);
			fuerzaLanzamiento = Random.Range (15f, 20f);
		
			if (ScriptJefe.mirandoDerecha == true) 
			{
				fuerzaLanzamiento *= 1; 
			} 
			else 
			{ 	
				fuerzaLanzamiento *= -1f;
			}
			ladrilloRigid.velocity = new Vector2 (fuerzaLanzamiento, 1);
			sonido.PlayOneShot (clipSaleLadrillo, 0f);

		}
	}

	// ========== COLISIONES (ENTRADAS)
	void OnCollisionEnter2D(Collision2D colision) 
	{
		lanzamiento = false;

		if (colision.gameObject.name == "Player") 
		{
			ScriptPlayer.chocar ("Ladrillo" , 500f);
			ScriptPlayer.cambiaEnergia (-5 , "Energía");
			ladrilloAnim.SetTrigger (desapareceHash);
			sonido.PlayOneShot (clipChocaLadrillo , 0.5f);
			Destroy (this.gameObject , 0.3f);
		}
			
		/** Chocamos contra los Objetos indicados en los tags
		 * Reproducimos el sonido del ladrillo al chocar.
		 * Destruimos el ladrillo.
		 
		if (colision.gameObject.tag == "Suelo" ||
			colision.gameObject.tag == "Obstaculos")
		{
			ladrilloAnim.SetTrigger (desapareceHash);
			sonido.PlayOneShot (clipChocaLadrillo , 0.5f);
			Destroy (this.gameObject ,0.6f);
		}



		if (colision.gameObject.name == "limiteDer" ) 
		{	
			Destroy (this.gameObject);	
		}

		ScriptJefe.lanzaLadrillo.Equals(false);
	}

	// ========== TRIGGERS (ENTRADAS)
	void OnTriggerEnter2D(Collider2D otro) 
	{	
		//lanzando = false;
	
	}
**/
}
