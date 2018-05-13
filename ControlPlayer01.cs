using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

//using System.Collections.Generic;
// Reference the Unity Analytics namespace
using UnityEngine.Analytics;

public class ControlPlayer01 : MonoBehaviour {
	/**
	// Instanciamos variables públicas de componentes del CANVAS.
	// Instanciate public vars for use Canvas components.

	private Image imagenVidas; 
	private Slider barraDeEnergia;
	private Text valorSliderEnergia;
	private Text Score;

	// Inicializamos sonidos.
	// AudioSource de la musica de fondo para controlar su velocidad.
	AudioSource  musicaFondo;
	// AudioSource para controlar los sonidos.
	public AudioSource SonidosPlayer;
	// Clips de Sonidos (Arrastras los clips desde el Inspector)
	public AudioClip Item;
	public AudioClip Salto;
	public AudioClip Shandwich;
	public AudioClip Pisa;
	public AudioClip Chispas;
	public AudioClip Combo;
	public AudioClip Golpe;


	// Variables para Instanciar al jugador Player 
	  
	private Rigidbody2D rigidPlayer;
	private Animator animatorPlayer;
	private SpriteRenderer spritePlayer;
	private SpriteRenderer sombra;

	public GameObject marcadorPuntos;
	public GameObject ObjDerrape;
	public GameObject polvoRebote;

	public Transform retroAlimentacion;
	public Camera camara;

	public GameObject resplandor;


	private JefeDeObra ctrlJefe;
	private ControlAnalytics ctrEscena;
	private Movil ctrlMovil;


	public float velocidadMaxima;
	public float impulsoMovimiento;

	private float fuerzaSalto = 580f;

	public float fuerzaDeChoque = 1500f;

	public int energia = 100;
	public int score = 0;

	public bool mirandoDerecha = true;
	private float movimientoHorizontal;
	private float movimientoVertical;


	private bool tocaMovil = false;
	public bool agarrandoMovil = false;	

	public bool tocaCasco = false;
	public int combo;





	public bool protegido = false;
	public int tiempoInmortal = 2;
	public bool inmortal = false;
	public bool muerto = false;

	public bool finalDeNivel;


	public bool enviaEstado = false;

	int groundHash = Animator.StringToHash ("Ground");
	int wallHash = Animator.StringToHash ("Wall");
	int saltaHash = Animator.StringToHash ("Saltar");
	int paraHash = Animator.StringToHash ("Parar");
	int velHorizHash = Animator.StringToHash ("VelocidadH");
	int muereHash = Animator.StringToHash ("Morir");
	int MovHorizontalHash = Animator.StringToHash ("MovimientoHorizontal");
	int energiaHash = Animator.StringToHash ("Energia");
	//int sueloHash = Animator.StringToHash ("Suelo");

	// Variables que comprueban si se está en el aire.
	private bool pisa;
	private bool enElAire;
	public bool isGround;
	public bool isWall;

	// Variables que comprueban si se realiza un rebote en la pared.
	public bool rebotando;
	private bool agarrado;
	private Transform groundCheck;

	private Transform wallCheck;
	private float wallRadius = 0.3f;
	private float groundRadius = 0.35f;

	private float altura;
	public LayerMask whatLayer;


	SpriteRenderer hide;

	void Start () {
		
		// Si encuentra un objeto llamado "Canvas".

		if (GameObject.Find("Canvas") != null) 
		{
			GameObject canvas = GameObject.Find("Canvas");
			if (canvas.transform.FindChild ("ImagenVidas") != null) { imagenVidas = GameObject.Find ("ImagenVidas").GetComponent <Image> ();}
			if (GameObject.Find ("SliderPlayer") != null) { barraDeEnergia = GameObject.Find ("SliderPlayer").GetComponent <Slider>();}
			if (GameObject.Find ("ValorSliderPlayer") != null) { valorSliderEnergia = GameObject.Find ("ValorSliderPlayer").GetComponent <Text>();}
			if (GameObject.Find ("Score") != null) { Score = GameObject.Find ("Score").GetComponent <Text>();}
		}

		rigidPlayer = this.GetComponent <Rigidbody2D> ();
		animatorPlayer = this.transform.FindChild ("Sprite").GetComponent <Animator> ();
		spritePlayer = this.transform.FindChild ("Sprite").GetComponent <SpriteRenderer> ();
		groundCheck = this.transform.FindChild("GroundCheck").GetComponent <Transform>();
		wallCheck = this.transform.FindChild("WallCheck").GetComponent<Transform>();
		sombra = this.transform.FindChild("GroundCheck").GetComponent <SpriteRenderer>();
		SonidosPlayer = this.GetComponent <AudioSource> ();

		if (GameObject.Find ("MusicaFondo") != null) { musicaFondo = GameObject.Find ("MusicaFondo"). GetComponent <AudioSource> ();}


		//Instanciamos el Script del manejo del jefe y el poligonCollider del casco dandole valor de rebote
		if (GameObject.Find ("JefeDeObra") != null) { ctrlJefe = GameObject.Find ("JefeDeObra").GetComponent <JefeDeObra> (); }

		if (GameObject.Find ("MusicaFondo") != null) { ctrEscena = GameObject.Find ("MusicaFondo").GetComponent <ControlAnalytics> ();}
	
		if (GameObject.FindWithTag ("Movil") != null) { ctrlMovil = GameObject.FindWithTag ("Movil").GetComponent <Movil>(); }


		rigidPlayer.mass = 1f;
		rigidPlayer.gravityScale = 5f;
		impulsoMovimiento = 14f;
		velocidadMaxima = 19f;	
		fuerzaSalto = 500;

	}



	// Update is called once per frame
	void FixedUpdate () {
		
		// Se almacena en la variable boolena 'isGround' si el transform
		//isGround = Physics2D.OverlapCircle (groundCheck.position,groundRadius,whatLayer);
		//isWall = Physics2D.OverlapCircle (wallCheck.position,wallRadius,whatLayer);
		altura =  rigidPlayer.transform.position.y * rigidPlayer.velocity.normalized.y;
		movimientoVertical = rigidPlayer.velocity.y;	
	
		if (!muerto && !finalDeNivel)
		{

				//movimientoHorizontal = Input.GetAxis ("Horizontal");
				
				
			// Si está tocando suelo....
			if (isGround)
			{
				/** Aplicamos fuerza en dirección dada por 'movimientoHorizontal' (-1 , 1).
				Reiniciamos variable 'combo' a 0. (No está haciendo Combos).
				Variable enElAire = false. (No está en el aire).
				Variable 'rebotando' = false. (No está rebotando).

				rigidPlayer.mass = 1f;
				//rigidPlayer.AddForce (Vector3.right * movimientoHorizontal * impulsoMovimiento, ForceMode2D.Force);	
				enElAire = false;
				rebotando = false;
				agarrado = false;
				combo = 0;
				girar();



				// Si la variable 'pisa' = false.... (Acaba de tocar suelo despues de un salto)
				if (!pisa) 
				{ 
					/**
					 * Reproduce sonido de pisada.
					 * Instancia un Prefab 'ObjDerrape' en la posición del transform ligado al objeto (Particulas de polvo).
					 * Asigna a la variable 'pisa' el valor true. (para que no se reproduzca ni el sonido ni instancie el prefab hasta que no vuelva a saltar).
					 * El sprite 'sombra' se vuelve mate (Se quita toda la trasparencia).
					 
					SonidosPlayer.PlayOneShot(Pisa,0.09f); 
					GameObject copiaDerrape = Instantiate (ObjDerrape, this.transform.position, this.transform.rotation) as GameObject;
					pisa = true;
					sombra.color = new Color(0f,0f,0f,1f);
				}
			}
			// Sino... Si: NO está tocando suelo (isGround = false) .... y NO está tocando una pared ('isWall' = false)....
			else if (!isWall) 
			{
				/**
				 * Variable 'enElAire' = true. (Está en el aire).
				 * Variable 'pisa' = false. (No acaba de pisar el suelo).
				 * El sprite 'sombra' se vuelve transparente. (Se añade transparencia).
				 * Aplicamos fuerza en dirección dada por 'movimientoHorizontal' * 'impulsoMovimiento' partido por 4 (menos fuerza en el aire).
				 
				enElAire = true;
				pisa = false;
				agarrado = false;

				sombra.color = new Color(0f,0f,0f,0.1f);
				rigidPlayer.AddForce (Vector3.right * movimientoHorizontal * (impulsoMovimiento / 2f), ForceMode2D.Force);	

			}
			else  // Sino... Está tocando una pared.
			{
				if ( movimientoHorizontal != 0 && altura < 0)
				{
					rigidPlayer.mass = 0.5f;
					if (!agarrado) 
					{ 
						agarrado = true;

						//rebotando = false;
						girar();
					}
					else
					{
						
					}
				
				}
				else
				{
					agarrado = false;
					rebotando= false;

				}
			}
		}

	
		
		
			// Si el jugador cambia de direccion (h tiene un signo diferente de 'velocity.x') o
			// If the player is changing direction (h has a different sign to velocity.x) or hasn't reached maxSpeed yet...
			//if ( movimientoHorizontal * Mathf.Round( this.transform.Translate ) < velocidadMaxima) {
				// ... Añade una fuerza ('AddForce') al jugador.
				// ... add a force to the player.
				
			//this.transform.Translate (Vector3.right * movimientoHorizontal * aceleracion);
			//		float vel = 1f * movimientoHorizontal * aceleracion;

				

	
			if (Mathf.Abs (this.rigidPlayer.velocity.x) > Mathf.Abs( velocidadMaxima) && rebotando == false ) {
				// ... Iguala la velocidad del jugador a 'maxSpeed' en el eje x.
				// ... set the player's velocity to the maxSpeed in the x axis.
				this.rigidPlayer.velocity  = new Vector2 (velocidadMaxima * movimientoHorizontal , this.rigidPlayer.velocity.y);
				//camara.transform.position = new Vector3(transform.position.x, camy,(camz - rigidPlayer.velocity.x * movimientoHorizontal));	
			}
			else
			{
				//camara.transform.position = new Vector3(transform.position.x,camy,camz );
			}
			
		// Asignamos valor de energia al valor energia del Animator.
		animatorPlayer.SetFloat("VelocidadV", Mathf.Abs(rigidPlayer.transform.position.y));
		animatorPlayer.SetFloat("Altura" ,rigidPlayer.transform.position.y * rigidPlayer.velocity.normalized.y);
		animatorPlayer.SetFloat(MovHorizontalHash, movimientoHorizontal);
		animatorPlayer.SetFloat(velHorizHash , Mathf.Abs( this.rigidPlayer.velocity.x));
		//animatorPlayer.SetBool (wallHash , isWall);	
		animatorPlayer.SetInteger (energiaHash, energia);
		//animatorPlayer.SetBool (groundHash, isGround);
		animatorPlayer.SetBool ("Agarrando", agarrado);
		animatorPlayer.SetBool ("Rebotando" , rebotando);

		
		}

		void Update ()
	{

		// Si encontramos un GameObject que se llama 'Canvas'...
		// If found a GameObject named 'Canvas'....
		if (GameObject.Find ("Canvas") !=null)
		{
			/** Asignamos valores almacenados en 'energia' a un Slider y el mismo valor como texto.
			Tambien asignamos el valor de la variable 'score' a un texto hijo del Canvas.

			barraDeEnergia.value = energia;
			valorSliderEnergia.text = energia.ToString ();
			Score.text = score.ToString ();
		}
			
			
					
		if (Input.GetKeyDown (KeyCode.Space))
		{
			rigidPlayer.mass = 1f;
			if (isGround) 
			{
				Vector2 salto = new Vector2 (( movimientoHorizontal * impulsoMovimiento) , fuerzaSalto);
				rigidPlayer.AddForce (salto);
				SonidosPlayer.PlayOneShot (Salto, 1f);
			}
			else if (agarrado )
			{
				
				Vector3 salto = new Vector3 ( (1f * - Mathf.Round( movimientoHorizontal)) * 500f, 600f  ,0f );
				Instantiate (ObjDerrape, this.transform.position, this.transform.rotation);

				rigidPlayer.AddForce (salto, ForceMode2D.Force);
				SonidosPlayer.PlayOneShot (Salto, 1f);
				rebotando = true;

				agarrado = false;
				girar();
				//mirandoDerecha = !mirandoDerecha;
				//spritePlayer.flipX = !spritePlayer.flipX;
			}
		}

		if (Input.GetKeyDown (KeyCode.E) && tocaMovil)
		{
			agarrandoMovil = !agarrandoMovil;
			ctrlMovil.agarrado = agarrandoMovil;
		}

		}


	//============= COLISIONES (ENTRADAS) ===============

	void OnCollisionEnter2D (Collision2D colision) 
	{

		//if (isGround) {SonidosPlayer.PlayOneShot (Pisa,0.09f);}

		if (colision.gameObject.tag == "Suelo" || 
			colision.gameObject.tag =="Obstaculos" || 
			colision.gameObject.tag == "Movil")

		{
			/**
			if (colision.gameObject.GetComponent<SpriteRenderer>() != null){
				int ordenObstaculo = colision.gameObject.GetComponent<SpriteRenderer>().sortingOrder;
				
				if (spritePlayer.transform.position.x < colision.transform.position.x && 
					spritePlayer.transform.position.y < colision.transform.position.y ){
					spritePlayer.sortingOrder = ordenObstaculo -1;
					Debug.Log ("Player a la izquierda");
				} else if (spritePlayer.transform.position.x > colision.transform.position.x && spritePlayer.transform.position.y < colision.transform.position.y){
					Debug.Log ("Player a la Derecha");
					spritePlayer.sortingOrder = ordenObstaculo +1;
				}
			}


			//animatorPlayer.SetBool ("Suelo", true);
			//SonidosPlayer.PlayOneShot (Pisa,0.09f);
			tocaCasco = false;
			//tocaSuelo = true;
			polvoDeSalto();

			//combo = 0;
		}

		if (colision.gameObject.tag == "Proteccion" )
		{
			protegido = true;
		}



		if (colision.gameObject.name == "JefeDeObra"  && !inmortal && !muerto)
		{

			chocar (colision.gameObject.name, 5500f);
			cambiaEnergia (-10 , "Energia");
			compruebaEnergia();
			StartCoroutine (recibeDaño());
			combo = 0;

		}

		 if (colision.gameObject.tag == "Peon" && !inmortal && !muerto)
		{
			Vector2 frenar = rigidPlayer.velocity;						// Almacenamos la velocidad del rigidbody en un Vector2.
			frenar.x = 0;												// Cambiamos el valor x del vector creado a 0.
			rigidPlayer.velocity = frenar;	

			animatorPlayer.SetBool("Daño",true);

			chocar (colision.gameObject.name, 250f);
			cambiaEnergia (-5 , "Energia");
			compruebaEnergia();
			StartCoroutine (recibeDaño());
			combo = 0;
		}

		// Si colisiona con algún objeto de la capa Items...
		// If collision with any GameObject stored in the "Items" layer...
		if (colision.gameObject.tag ==  "Items"){ 

			// Suma 10 puntos, aumenta la velocidad máxima en 1.0f y aumenta la velocidad de la musica (pitch)
			// Siempre y cuando velocidad máxima sea menor o igual que 8.
			// Destruye el objeto colisionado.
			if (colision.gameObject.name == "RodilloRojo" && velocidadMaxima <= 8f){
				SonidosPlayer.PlayOneShot (Item , 0.5f);
				score += 50;
				velocidadMaxima += 1f;
				musicaFondo.pitch += 0.1f;
				cambiaEnergia (10 , "Velocidad");
				Destroy (colision.gameObject);
			}

			// Suma 10 a variable energia.
			// Siempre que energia < 100
			if (colision.gameObject.tag == "Energy" && energia < 100){
				int incremento = 50;
				SonidosPlayer.PlayOneShot(Shandwich,0.5f);

				if (incremento + energia > 100) { incremento = 100 - energia; }

				cambiaEnergia (incremento , "Energia");
			
				Destroy (colision.gameObject);

			}
		
			/**
			if (colision.gameObject.name == "Sandwich" && energia < 100){
				int incremento = 50;
				SonidosPlayer.PlayOneShot(Shandwich,0.5f);
				if (incremento + energia > 100) { incremento = 100 - energia; }
				cambiaEnergia (incremento , "Energía");
				ResplandorActivo = false;
				Destroy (colision.gameObject);
			}

		}
		Debug.Log( "CHOQUE con: " + colision.gameObject.name);
	}
	
	//============= COLISIONES (SALIDAS) ===============

	void OnCollisionExit2D (Collision2D colision) 
	{
		//if (GameObject.Find ("Canvas") != null) { imagenVidas.color = new Color (1f,1f,1f); }
		
		if (colision.gameObject.name == "JefeDeObra" && !muerto && !inmortal)
		{
			//animatorPlayer.SetTrigger (paraHash);
		}

	
		/** 
		if (  colision.gameObject.tag == "Obstaculos" || colision.gameObject.tag == "Movil" && !muerto) 
		{
			tocaSuelo = false;
			animatorPlayer.SetBool ("Suelo", false);
			spritePlayer.sortingOrder = 10;
		}

		if (colision.gameObject.tag == "Proteccion" )
		{
			protegido = false;
		}
		/**
		if (colision.gameObject.tag == "Movil"){
			caja.isKinematic = true;
			caja.transform.parent = null;
			Debug.Log("NO PADRE");
		}
	
	}


// ========= COLISIONES TRIGGERS (STAY) ==========
	void OnTriggerStay2D(Collider2D colider){

		if (colider.gameObject.tag == "Movil"){
			

			
			tocaMovil = true;
		}

		if (colider.transform.FindChild ( "Chispas") && !protegido) {

			rigidPlayer.AddForce (Vector2.one * 15f, ForceMode2D.Impulse);
		}

		if (colider.gameObject.tag =="Hide"){ 

			Oculto oculto = colider.gameObject.GetComponent <Oculto>();
			oculto.activo = true;
			//hide.color = new Color (1f,1f,1f,0.5f);
		}
	}

	
	//============= COLISIONES TRIGGERS (ENTRADAS) ===============	


	void OnTriggerEnter2D(Collider2D other) {
		





		// Colision con el triger del Prefab "Agujero".
		if (other.gameObject.name == "Agujero" || other.gameObject.name == "Destructor"){
			energia = 0;
			//camara.transform.parent = null;

			compruebaEnergia ();
			StartCoroutine (morir ());
		}


		if (other.gameObject.name == "FinDeNivel") { finalDeNivel = true; }


		// Toca el trigger del prefab "Chispas".
		if (other.gameObject.name == "Chispas" && !inmortal && !protegido) {
			

			rigidPlayer.AddForce (Vector2.one * 15f, ForceMode2D.Impulse);





			SonidosPlayer.PlayOneShot (Chispas,0.5f);
			cambiaEnergia (-5 , "Energia");
			compruebaEnergia();
			StartCoroutine (recibeDaño ());
			animatorPlayer.SetBool (saltaHash, false);
		}

		if (other.gameObject.tag== "Impulsores")
		{
			other.transform.FindChild ("Muelle").GetComponent<Animator>().SetBool("TocaMuelle", true);
			other.transform.FindChild ("Muelle").GetComponent<Animator>().SetBool("PararMuelle", true);
			if (isGround){
				
				rigidPlayer.AddForce (Vector2.up * 9f ,ForceMode2D.Impulse );
			}else{
				rigidPlayer.AddForce (Vector2.up * 25f,ForceMode2D.Impulse);
			}
		}

		if (other.gameObject.tag == "Plataformas"){

			//tocaSuelo = true;
			animatorPlayer.SetBool ("Suelo", true);
			SonidosPlayer.PlayOneShot(Pisa,0.09f);
			polvoDeSalto();
		
		}



	}
	

	//============= COLISIONES TRIGGERS (SALIDAS) ===============	

	void OnTriggerExit2D (Collider2D other)
	{
		if (other.gameObject.name == "JefeDeObra" ) 
		{
			tocaCasco = false;
			ctrlJefe.golpeEnCasco = false;
			//animatorPlayer.SetBool ("Suelo", false);
			//tocaSuelo = false;
		
		}

		if (other.gameObject.name == "Chispas")
		{
			imagenVidas.color = new Color (1f,1f,1f);
		}

		if (other.gameObject.tag == "Movil")
		{
			tocaMovil = false;
		}


		if (other.gameObject.tag =="Hide"){ 

			Oculto oculto = other.gameObject.GetComponent <Oculto>();
			oculto.activo = false;
		}

		if (other.gameObject.tag == "Impulsores")
		{
			
			other.transform.FindChild ("Muelle").GetComponent<Animator>().SetBool("PararMuelle", false);

			other.transform.FindChild ("Muelle").GetComponent<Animator>().SetBool("TocaMuelle", false);


		}
	}
	


	//============= FUNCIONES ===============	

	private	void compruebaEnergia ()
	{
		if (energia <= 0 && !muerto) {
			
			muerto = true;

			//Application.LoadLevel ("GameOver");
			
			if (!enviaEstado) {
				ctrEscena.RegistraFin ();
				
				enviaEstado = true;
			}
			//Destroy (this.gameObject, 3);
			//this.gameObject.SetActive (false);
		}

	}

	public void cambiaEnergia(int cantidad, string tipo) {

		if (tipo == "Energia" || tipo == "Energy") 
		{
			energia += cantidad;

			if (cantidad < 0) { imagenVidas.color = new Color(1f, 0f, 0f); }
		}

		InstanciarRetroalimentacionEnergia (cantidad, tipo);
	}

	
	private void InstanciarRetroalimentacionEnergia(int incremento, string tipoIncremento) {
		GameObject retroalimentcionGO = null;

		if (retroAlimentacion != null) {
			retroalimentcionGO = (GameObject) Instantiate (marcadorPuntos, retroAlimentacion.position, retroAlimentacion.rotation);

		} else {
			retroalimentcionGO = (GameObject) Instantiate (marcadorPuntos, transform.position, transform.rotation);
		}
		retroalimentcionGO.GetComponent <PlayerMessages> ().cantidadCambiodeEnergia = incremento;
		retroalimentcionGO.GetComponent <PlayerMessages> ().tipoIncremento = tipoIncremento;
	}


	/* Funcíon girar.
	 * Almacena las coordenadas locales del Sprite en una variable.
	 * Multiplica el eje x por -1.
	 * Almacena las nuevas coordenadas en las coordenadas locales del sprite.
	 * Instancia el objeto derrape dependiendo de la velocidad del personaje y si está tocando suelo.
	 
	public void girar () 
	{
		
			
		if (movimientoHorizontal > 0 && !mirandoDerecha && !agarrado & !rebotando) 
		{ 
			spritePlayer.flipX = false;
			//Vector3 giraWall = wallCheck.position;
			//giraWall.x = spritePlayer.bounds.center.x + 0.35f;
			//wallCheck.position = giraWall;
			mirandoDerecha = !mirandoDerecha;

		}

		if (movimientoHorizontal < 0 && mirandoDerecha  && !agarrado & !rebotando) 
		{ 
			spritePlayer.flipX = true;
			//Vector3 giraWall = wallCheck.position;
			//giraWall.x = spritePlayer.bounds.center.x - 0.35f;
			//wallCheck.position = giraWall;
			mirandoDerecha=!mirandoDerecha;
		}


		if (agarrado && !rebotando) 
		{
			
			spritePlayer.flipX = !spritePlayer.flipX;
			mirandoDerecha = !mirandoDerecha;	
		}

		if (rebotando)
		{
			Vector3 giraWall = wallCheck.position;
			if (mirandoDerecha)
			{
				giraWall.x = spritePlayer.bounds.center.x - 0.35f;
				Debug.Log ("Rebotado Hacia la DERECHA");
			}
			else if (!mirandoDerecha)
			{
				giraWall.x = spritePlayer.bounds.center.x + 0.35f;
				Debug.Log ("Rebotado Hacia la IZDA");
			}

			wallCheck.position = giraWall;
		}
	}


	
		

	public void rebotar ()
	{
		rigidPlayer.AddForce  (Vector2.up * 12f , ForceMode2D.Impulse);
			SonidosPlayer.PlayOneShot( Golpe,1f);
	}


	public void chocar (string objeto, float fuerzaGolpe)
	{
		if (mirandoDerecha){
			fuerzaGolpe *= -1;
		}else
		{
			fuerzaGolpe *= 1;
		}

		if (objeto == "JefeDeObra") 
		{
			if (ctrlJefe.mirandoDerecha == false) 
			{
				fuerzaGolpe *= -1;
			} 
			else 
			{
				fuerzaGolpe *= 1;
			}
		}

		if (objeto == "Chispas" ){
			if (mirandoDerecha){
				fuerzaGolpe *= -1;
			} else {
				fuerzaGolpe *= 1;
			}
		}

		rigidPlayer.AddForce (  Vector2.right * fuerzaGolpe, ForceMode2D.Force)  ;
		Debug.Log ("Golpe Propinado" + Vector2.right * fuerzaGolpe);
	}


	private void polvoDeSalto(){

		//GameObject humoSuelo = null;
		
		if (ObjDerrape != null){
			//humoSuelo = Instantiate (ObjDerrape , spritePlayer.transform.position, ObjDerrape.transform.rotation) as GameObject;
			
		}else{
			//humoSuelo = Instantiate (ObjDerrape , spritePlayer.transform.position, transform.rotation) as GameObject;
		}
	}


	public   IEnumerator recibeDaño ()
	{
		if (!muerto)
		{
			
			SonidosPlayer.PlayOneShot(Golpe);
			inmortal = true;
			StartCoroutine (hacerInmortal ());
			yield return new WaitForSeconds (tiempoInmortal);
			inmortal = false;

		}
		else
		{
			StartCoroutine (morir ());

		}
	}


	public IEnumerator hacerInmortal()
	{
		while (inmortal){
			
			spritePlayer.color= new Color(1f,1f,1f,0.1f);
			yield return new WaitForSeconds (0.05f);
			spritePlayer.color= new Color(1f,1f,1f,1f);

			yield return new WaitForSeconds (0.05f);
			animatorPlayer.SetBool("Daño",false);
		}
		
	}

	public IEnumerator morir ()
	{
		if (!finalDeNivel)
		{
			animatorPlayer.SetTrigger (muereHash);
			muerto = true;
			//camara.transform.parent = null;
			yield return new WaitForSeconds (1.8f);
			rigidPlayer.simulated = false;
		
			SceneManager.LoadScene (SceneManager.GetActiveScene().buildIndex);

		}

	}

**/
}
