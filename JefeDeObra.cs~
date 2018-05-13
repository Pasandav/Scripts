using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class JefeDeObra : MonoBehaviour {
/**
	// Creamos float para velocidad.
	public int energia = 100;

	public bool mirandoDerecha = false;	
	public float velocidad = -4.5f;

	public float fuerzaLanzamiento = -500;
	public bool golpeEnCasco = false;
	public bool lanzaLadrillo;
	public bool reirse = false;
	public bool muere =false;

	public bool enviaEstado = false;

	// Instancias para el Canvas.
	public Image imgCanvas; 
	public Slider SliderJefe;
	public Text textoEnergiaJefe;


	//Instancias del GizmoJefe		
	Animator jefeAnimator;
//	int andandoHash = Animator.StringToHash ("Andando");
//	int lanzandoHash = Animator.StringToHash ("Lanzando");
//	int lanzaHash = Animator.StringToHash ("Lanzar");
	int chafaHash = Animator.StringToHash ("Chafar");
	int rieHash = Animator.StringToHash ("Reirse");
	int muereHash = Animator.StringToHash ("Muere");
	Rigidbody2D jefeRigid;
	Transform lanzador;

	public Rigidbody2D ladrillo;
	//private Rigidbody2D copiaLadrillo;

	// Instancias del sonido.
	AudioSource sonidos;
	public AudioClip clipGolpe; 
	public AudioClip clipRisa;
	public AudioClip clipLanzar;
	

	// Instancias del GizmoPlayer.
	Rigidbody2D playerRigid;
	public Transform target;
	public Vector3 playerPosicion;
	 

	ControlAnalytics ctrAnalytics;
	ControlPlayer01 ctrlPlayer;

	// Use this for initialization
	void Start () {
		//copiaLadrillo = GetComponent <Rigidbody2D>();
		jefeAnimator =  transform.FindChild ("SpriteJefe").GetComponent <Animator> ();
		jefeRigid = GetComponent <Rigidbody2D> ();
		lanzador = transform.FindChild ("lanzador").GetComponent <Transform>();

		jefeRigid.mass = 80f;
		jefeRigid.gravityScale = 20f;

		playerRigid = GameObject.Find ("PlayerV2").GetComponent <Rigidbody2D> ();
		sonidos = GetComponent <AudioSource> ();

		energia = 100;
		lanzaLadrillo = false;

		ctrAnalytics = GameObject.Find ("MusicaFondo").GetComponent <ControlAnalytics> ();
		ctrlPlayer = GameObject.FindGameObjectWithTag ("Player").GetComponent <ControlPlayer01>();
		jefeAnimator.SetBool ("Andando", true);
		jefeAnimator.SetBool ("Lanzando", false);

		 
	}

	void Update (){
		SliderJefe.value = energia;
		textoEnergiaJefe.text = energia.ToString ();

		// Comprobacion de la posicion de player.
		if (playerRigid != null) {
			playerPosicion = new Vector2 (playerRigid.transform.position.x, playerRigid.transform.position.y);
		}

		//Debug.Log (playerPosicion);
		if (energia <= 0){
			energia = 0;
			muere= true;
			morir ();
			if (!enviaEstado){
				ctrAnalytics.RegistraFin();
				enviaEstado = true;
			}

			Invoke ("destruir",3);
		}




		/**
		if (jefeAnimator.GetBool (andandoHash) && Random.value < 1f / (60f * 2f) && !lanzaLadrillo )
		{

			jefeAnimator.SetBool ("Lanzando", true);
			jefeAnimator.SetBool ("Andando", false);
			lanzaLadrillo = true;
			
			copiaLadrillo = Instantiate (ladrillo , lanzador.transform.position , lanzador.transform.localRotation) as Rigidbody2D;
			
		}

		if (copiaLadrillo == null && jefeAnimator.GetCurrentAnimatorStateInfo (0).IsName ("Lanzar")) {

			jefeAnimator.SetBool ("Lanzando" , false);
			jefeAnimator.SetBool ("Andando" , true);
			lanzaLadrillo= false;
		}

	}

	// Update is called once per frame
	void FixedUpdate () {

		Quaternion gira = lanzador.transform.rotation;
		gira.z += 50f;
		lanzador.transform.rotation = gira;

		transform.Translate (-Vector2.left * velocidad * Time.deltaTime); // Usamos 



	
		if (golpeEnCasco) {
			chafar ();
		} else {
			andar ();
		}
	
	}

	// ========== COLISIONES (ENTRADAS)

	void OnCollisionEnter2D (Collision2D otro)
	{
		// Colision con tag Obstaculos.
		// Llamamos a la función girar().
	
		if (otro.gameObject.tag == "Obstaculos" || otro.gameObject.tag == "Proteccion" || otro.gameObject.tag == "Limites") 
		{
			girar();
		}
		
		// Colision con Jugador, sin golpe en casco y sin morir
		// Llamamos a la funcion Parar, luego a reir() y luego Invoke a girar() con retardo de 2 segundos.
		if (otro.gameObject.name == "Player" && !golpeEnCasco && !muere && ctrlPlayer.muerto == false) 
		{
			parar ();
			reir ();
			Invoke ("girar",2);
		}
	}
	
	void OnCollisionExit2D(Collision2D otro)
	{
		// Deja de colisionar con Player
		if (otro.gameObject.name == "Player"  && !muere && !golpeEnCasco && !reirse) 
		{
			jefeAnimator.SetTrigger ("Andar");
		}
	}

	void OnTriggerEnter2D(Collider2D otro) 
	{	
		if (otro.gameObject.name == "limiteDer") 
		{		
			girar ();	
		}
	}

	//============== FUNCIONES =================

	float mideDistanciaX ()
	{
		float distanciaX;
		if (!playerRigid.Equals( null)) 
		{
			playerPosicion = new Vector3 (playerRigid.transform.position.x, playerRigid.transform.position.y, playerRigid.transform.position.z);
			distanciaX = Mathf.RoundToInt (playerPosicion.x - this.transform.position.x);
		} else 
		{
			distanciaX = 0f;
		}
			return distanciaX;
	}

	public void andar ()
	{

		jefeAnimator.SetTrigger ("Andar");
	}


	
	public void girar ()
	{	
		//reirse = false;
		jefeAnimator.SetBool (rieHash, false);
		jefeAnimator.SetTrigger ("Andar");
		mirandoDerecha = !mirandoDerecha;
		if (velocidad == 0f) 
		{
			andar();
			if (mirandoDerecha) 
			{
				velocidad = 4.5f;
			} else 
			{
				velocidad = -4.5f;
			}
		} else 
		{
			velocidad *= -1;
		}

		var escalar = transform.localScale;
		escalar.x = escalar.x *-1;
		transform.localScale = escalar;
	}

	public void chafar ()
	{

			golpeEnCasco = false;
			sonidos.PlayOneShot (clipGolpe, 1f);
			jefeAnimator.SetTrigger (chafaHash);
			energia -= 5;

	}

	public void morir ()
	{
		parar ();
		jefeAnimator.SetTrigger (muereHash);
	}

	void destruir ()
	{
		Destroy (this.gameObject);
	}

	public void reir ()
	{
		//if (!reirse) 
		//{
			reirse = true;
			jefeAnimator.SetBool (rieHash, true);
			sonidos.PlayOneShot (clipRisa, 0.09f);
		//} else 
		//{
		//	reirse = false;
		//	jefeAnimator.SetBool (rieHash,false);
		//}
	}

	public void parar ()
	{
		velocidad = 0f;
	}
	**/
}
