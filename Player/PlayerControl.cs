using UnityEngine;
using System.Collections;


enum mirando {izquierza = 1 , derecha = -1};

public class PlayerControl : MonoBehaviour {

	/** VARIABLES PARA CHECKEAR CONTACTO DE LOS TRANSFORM DE ESTE OBJETO.
	 * 
	 **/
	//private Vector2 detectWall;     // Vector3 que almacena la posición del Transform detector de muros
	//private bool isWall;			// Variable tipo Booleano que almacena si se está tocando muro.

    private Vector2 detectGround;   // Vector3 que almacena la posición del Transform detector de suelo
    private string typeOfGround;


    private bool firstTouch;        // Variable tipo bool que almacena si acaba de tocar el suelo.

    /*PRUEBA
    */
    //private bool groundLeft;        // Booleano que almacena si el transform groundLeft toca el suelo.
    //private bool groundRight;       // Booleano que almacena si el transform groundLeft toca el suelo.


	[SerializeField]
	private LayerMask layerGroundAndWalls;	// Variable que almacena los valores indicados en inspector de las capas que deben detectar las variables booleanas 'isWall' e 'isGround'.

	/** VARIABLES DE MOVIMIENTO HORIZONTAL.
	 * 
	 **/
	[Header ("Movimiento HORIZONTAL")]
	[SerializeField]
	private float horizontalDirection;		// Variable que almacena el eje horizontal x (negativo izda, positivo dcha).
	
    //public float forceX;				// Variable que almacena el empuje utilizado en la fuerza horizontal.
	[Tooltip ("Velocidad horizontal")]
	public float velocityX;				// Variable que almacena la velocidad horizontal del rigidbody
	public float maxVelocityX;			// Variable que almacena la velocidad máxima horizontal que puede tener el rigidbody

	public float initialForce;
	public float maxVelocity;
	public float currentForce = 30f;
	public float AcelerationForce;
	public float DecelerationForce;

	private bool isSliding;				// Variable bool que indica si está derrapando.
	private bool pushing;               // Variable bool que indica si está empujando.
    private bool isBarro = false;

	/** VARIABLES DE SALTO INCREMENTAL
	 * 
	 **/
	[Header ("Variables de SALTO")]
	public bool startJump = false;				// Booleano que indica si se comienza a saltar.
	public float timePressing = 0.0f;			// Tiempo pulsando la tecla de salto
	public float maxTimeJump = 0.6f;			// Máximo tiempo que registra la pulsación
	public float minJumpForce = 7.8f;			// Fuerza minimima del salto.
	public float incrementJumpForce = 7f;		// Incrementos que se añaden al salto hasta que llega a la fuerza máxima.
	public float height;						// Variable que contendrá la altura del salto.
	public float drop;							// Variable que contendra la distancia de caida (drop)

	[Header ("Variables de AGARRADO")]
    public bool isGrip = false;					// Booleano que indica si está agarrandose a un muro.
	public float gripGravity = 2f;			// Gravedad cuando se agarra a un muro.
    public bool lastGrip = false;

	[Space]

	[Header ("Variables de REBOTE")]
    public bool isBounce = false;                 // Variable booleana que indica si está rebotando.
    public float bounceForceX;
	public float bounceForceY;				// Booleano que indica si estamos rebotando entre paredes.

	//public bool jumping = false;
	[Header ("Prefabs para POLVO")]
	[SerializeField] private GameObject dustJump;					// Variable de tipo GameObject que contendrá el prefab PolvoSalto
	[SerializeField] private GameObject prefabDustGrip;			// GameObject publico para indicar en Inspector el Prefab polvo de agarre.
	[SerializeField] private GameObject dustSlide;		// Variable de tipo GameObject que contendrá el prefab PolvoDerrape
	
    /**
	 * 
	 **/
	public int energia = 100;
	public bool inmortal = false;
	private float inmortalTime = 2f;
	public bool muerto = false;
	public bool knock = false;
	private float directionKnock = 0f;
	private bool hidden = false;

    /** VARIABLES QUE INSTANCIAN COMPONENTES
	 * 
	 **/
	private SpriteRenderer thisSprite;			// 
	private Animator thisAnimator;
	private Rigidbody2D thisRigid;

    private CheckGroundAndWalls Checks;

    private SpriteRenderer thisShadow;
	/** HASH DE VARIABLES (PARA ANIMATOR)
	 * 
	 **/
	// Almacenamos en variables tipo 'int' el identificador de los valores del Animator. (Más eficiente).
	
    //private int hashIsGround = Animator.StringToHash ("Ground");
	private int hashIsWall = Animator.StringToHash ("Wall");
	private int hashGrip = Animator.StringToHash ("Agarrado");
	private int hashHorizontalX = Animator.StringToHash ("Direccion");
	private int hashHorizontalSpeed = Animator.StringToHash ("VelocidadH");
	private int hashVerticalSpeed = Animator.StringToHash ("VelocidadY");
	private int hashHeight = Animator.StringToHash ("Altura");
    private int hashSubiendo = Animator.StringToHash("Subiendo");
    private int hashSlide = Animator.StringToHash("Derrapa");
	private int hashDrop = Animator.StringToHash ("Caida");
	private int hashBounce = Animator.StringToHash ("Rebotando");
	private int hashPush = Animator.StringToHash ("Empujando");
	private int hashKnock = Animator.StringToHash ("Golpe");
	private int hashHidden = Animator.StringToHash ("Oculto");
    private int hashBarro = Animator.StringToHash ("Barro");
    int hashAgachado = Animator.StringToHash("Agachado");


	private AudioSource sourceofSounds;
	// Arrays de sonidos (Arrastras los clips desde el Inspector)
	public AudioClip [] arrayOfSounds;

	//private int llamadas;
	
   // public Transform gizmoWall;
	//public Transform gizmoRight;
	//public Transform gizmoLeft;

    //private Transform gizmoCheckWall;

	void Awake ()
	{
        /** COGEMOS COMPONENTES
		 * 
		 **/


        if (this.name.ToLower().Contains("player"))
        {
            // Asignamos componente SpriteRenderer del gameobject Sprite.
            thisSprite = this.transform.Find("Sprite").GetComponentInChildren<SpriteRenderer>();
			thisShadow = this.transform.Find("Sombra").GetComponent<SpriteRenderer>();
            
			// Asignamos componente Animator del gameobject Sprite 		
            thisAnimator = this.transform.GetComponentInChildren <Animator>();
           		
            // Asignamos componente RigidBody2D del gameobject
            thisRigid = this.GetComponent <Rigidbody2D> ();											
			
            sourceofSounds = this.GetComponent <AudioSource>();

            Checks = this.GetComponent<CheckGroundAndWalls>();

            
        }


	}

	void Start ()
	{
		
		/** INICIALIZACION DE VARIABLES
		 * 
		 **/

		// Asignamos valor que sirve de Fuerza de empuje para el movimiento horizontal
		maxVelocityX= 2f;		// Asignamos velocidad máxima horizontal.
		thisRigid.gravityScale = 9f;	// Escala de gravedad.
		//toca = 0;
		//minJumpForce = 12f;			// Fuerza de salto mínima.
		//incrementJumpForce = 12f; // Incrementos que se añaden a la fuerza en el salto

		timePressing = maxTimeJump; 

		// Fuerzas x e y que se aplican al rebotar (bounce) si estamos agarrados (grip)
		bounceForceX = 3f;
		bounceForceY = 5f;

		currentForce = 0f;
		initialForce = 5f;
		maxVelocity = 2f;

		AcelerationForce = 5f;
		DecelerationForce = 75f;

        thisAnimator.SetInteger(hashAgachado, 0);
	}
		

	void Update ()
	{
		
		// SI... Pulsamos la tecla de salto (Espacio)
		if (Input.GetKeyDown (KeyCode.Space)) {
			// Y ..SI .. estamos tocando suelo ('isGround')
            // Igualamos variable ('starJump') a true. (Comenzamos a saltar).
            // Igualamos a 0 la variable que controla el tiempo que pulsamos el salto (timePressing).
            if (Checks.GetIsGround())

            {
				startJump = true;
				timePressing = 0f;
            } 
            // Sino.. Si se está agarrando ('isGrip').
            // Igualamos variable rebote ('isBounce') a true.
            // Llamamos a la corutina Bouncing.
            // Igualamos el tiempo que pulsamos el salto ('timePressing'), al máximo tiempo permitido ('maxTimeJump').
            else if (isGrip) 
            {
                isBounce = true;
				StartCoroutine (Bouncing (horizontalDirection));
				timePressing = maxTimeJump;
            }
            // Sino.. es que ya estamos en el aire
            // Igualamos la variable ('starJump') a false.
            // Igualamos el tiempo que pulsamos el salto ('timePressing'), al máximo tiempo permitido ('maxTimeJump').
            else
            {
                startJump = false;
                timePressing = maxTimeJump;
            }
        }

        // SI... Mantenemos pulsada la tecla salto (Espacio) y el tiempo pulsada (timePressing) es menor que el tiempo máximo pulsando (maxTimeJump)
        if (Input.GetKey(KeyCode.Space) && timePressing < maxTimeJump)
        {
            timePressing += Time.deltaTime;
        }

        // Si... Soltamos la tecla salto (Espacio)....Reiniciamos a 0 la variable 'timePressing'
        if (Input.GetKeyUp(KeyCode.Space))
        {
            timePressing = maxTimeJump;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            thisAnimator.SetInteger(hashAgachado, 1);
        }

        if (Input.GetKey (KeyCode.DownArrow)){
            thisAnimator.SetInteger(hashAgachado, 2);
        }

        if (Input.GetKeyUp (KeyCode.DownArrow)){
            if (thisAnimator.GetInteger(hashAgachado)!= 3 )
            {
                thisAnimator.SetInteger(hashAgachado, 3);
            }
            else
            {
                thisAnimator.SetInteger(hashAgachado, 1);
            }

        }

        if (Input.GetKey (KeyCode.E) && Checks.GetIsWall())
		{
			//pushing = true;
		}

        if (Input.GetKeyUp (KeyCode.E) && Checks.GetIsWall())
		{
			//pushing = false;
		}

       

    }

	void FixedUpdate ()
	{
        // Si no nos golpean (knock) o no rebotamos (bounce)
        // Almacena el valor del eje X al pulsar las flechas horizontales (negativo izda, positivo dcha).
        //if (!knock)
        detectGround.x = thisSprite.bounds.center.x;
        detectGround.y = thisSprite.bounds.min.y;




        if (!knock) {
			horizontalDirection = Input.GetAxisRaw ("Horizontal");

            //Igualamos la fuerza aplicada (currentFoce)
            currentForce = Mathf.Abs(horizontalDirection) > 0f ? 30f : 0f;


			// SI... estamos tocando suelo (isGround)..
            if (Checks.GetIsGround()) 
            {
                


                typeOfGround = Checks.GetTypeOfGround();
                Debug.Log("Valor de type of ground= " + typeOfGround);

                // Sino... (estamos en barro)..
                // Igualamos la velocidad máxima del rigidbody
                // Igualamos la fuerza minima de salto
                if (typeOfGround.Contains("barro"))
                {
                    thisAnimator.SetBool(hashBarro, true);
                    maxVelocity = 0.01f;
                    minJumpForce = 1f;
                    currentForce = 20f;
                }
                // Si no estamos tocando barro (!isBarro)..
                // Igualamos la velocidad máxima del rigidbody
                // Igualamos la fuerza minima de salto

                else
                {
                    maxVelocity = 2f;
                    minJumpForce = 7.8f;
                    thisAnimator.SetBool(hashBarro, false);
                    thisShadow.color = new Color(0f, 0f, 0f, 0.5f);

                }

                //Añadimos una fuerza de: valor almacenado en: horizontalDirection (-1 o 1) * valor almacenado en currentFore * Vector2.right (1,0).
				thisRigid.AddForce (horizontalDirection * Vector2.right * currentForce , ForceMode2D.Force);
            }
			//SINO... Es que estamos saltando,callendo o rebotando.
			else 
            {
                thisShadow.color = new Color(0f, 0f, 0f, 0f);
                startJump = false;
                //Si, no estamos rebotando (!isBounce)
                // Velocidad en horizontal si estamos saltando pero no rebotando (!bounce)
				if (!isBounce) 
                {
					thisRigid.AddForce ((horizontalDirection * currentForce / 3f * Vector2.right), ForceMode2D.Force);
				} 
                else 
				{
					// Velocidad en horizontal si estamos rebotando (bounce = true);
					thisRigid.AddForce ((horizontalDirection * bounceForceX * Vector2.right) , ForceMode2D.Force);
				}
			}
		} 
        //Sino... nos están golpeando.
        //Llamamos a la funcion knock.
        else {
			Knock (250f, 6f);
            Debug.Log("GOLPIÑO");
		}
		
        // Almacena el valor absoluto (positivo) de la velocidad del rigidbody.
        velocityX = Mathf.Abs(thisRigid.velocity.x);

        // SI.. la velocidad x del rigidbody es mayor o igual que la velocidad maxima horizontal ('maxVelocityX')
        if (velocityX >= maxVelocityX && !isBounce)
       // if (Mathf.Abs (thisRigid.velocity.x) >= maxVelocity && !isBounce)																		
		{
			// Igualamos la velocidad del rigidbody al valor de 'maxVelocityX'.
			thisRigid.velocity = new Vector2( maxVelocity * horizontalDirection , thisRigid.velocity.y);
		}


        /** Llama a funciones que comprueban
       * Si tocamos suelo.
       * Si tocamos pared.
       * La direccion del Sprite
       * Si nos agarramos a una pared.
       **/
        CheckGround();
        CheckGrip();
        Girar();


        Jump();

        // Envía los valores al Animator.
		thisAnimator.SetFloat (hashHorizontalX, horizontalDirection);
		thisAnimator.SetFloat(hashVerticalSpeed, thisRigid.velocity.y);
        thisAnimator.SetFloat (hashSubiendo, thisRigid.velocity.normalized.y);
		thisAnimator.SetFloat (hashHorizontalSpeed, velocityX);
		thisAnimator.SetBool (hashBounce, isBounce);
        thisAnimator.SetBool("StartJump", startJump);
       
	}

	/** FUNCION JUMP ( SALTO INCREMENTAL ).
	 * 
	 **/
	void Jump ()
	{
        /** Si se comienza a saltar (startJump) y no se está rebotando (!bounce)
		*	Se añade una fuerza de tipo impulso al rigidbody... En el vector 0,1 (arriba) multiplicado por la fuerza minima de salto (minJumpForce)
		*	Se reproduce el sonido almacenado en la posicion uno de la matriz de sonidos (salto).
		*	Se iguala la variable starJump a false (ya estamos saltando).
		**/
		if (startJump && !isBounce && !isGrip)
        {
            
            thisRigid.AddForce(minJumpForce * Vector2.up, ForceMode2D.Impulse);
            //StartCoroutine(StartJump(0.2f));

            startJump = false;



            // SI.. el tiempo presionando (timePressing) es menor que el maximo tiempo de salto (MaxTimeJump).
            if (timePressing < maxTimeJump && !isGrip && !isBounce)
            {
                // Añadimos una fuerza de incrementoJumpForce en un Vector2 (0,1), de tipo Force. 
                thisRigid.AddForce(incrementJumpForce * Vector2.up, ForceMode2D.Force);
            }
            // SI NO... (Si el tiempo presionado (timePressing) es mayor que el máximo tiempo de salto (MaxTimeJump)).		
            else
            {
                // Igualamos el tiempo presionando la tecla a el maximo tiempo de salto.
                timePressing = maxTimeJump;
            }
        }
	}
		
	/* Función Bouncing
	*/
	IEnumerator Bouncing (float direction)
	{
		thisRigid.gravityScale = 0.2f;
        thisRigid.mass = 0.7f;
		thisRigid.AddForce (new Vector2 (-direction * bounceForceX, bounceForceY), ForceMode2D.Impulse);	
		
        yield return new WaitForSeconds (0.8f);
	    
        isBounce = false;
        thisRigid.gravityScale = 9f;
        thisRigid.mass = 0.7f;
	}


	/** FUNCION Girar (COMPRUEBA HACIA DONDE DEBE MIRAR EL SPRITE)
	 *  
	 **/
 	void Girar()
	{
       
        // Almacenamos la escala local del transform en variable tipo Vector2
        Vector2 temp = this.transform.localScale;
      
			if (horizontalDirection * velocityX > 0 && this.transform.localScale.x < 0 ||
				horizontalDirection * velocityX < 0 && this.transform.localScale.x > 0 ||
			    Checks.GetIsGround() && thisSprite.flipX == true)
			{
				temp.x *= -1f;
			    thisSprite.flipX = false;
			}
            
        this.transform.localScale = temp;
	}

	
   
	/** FUNCION CHECKGRIP (Chequea si se está agarrando a una pared).
	 * 
	 **/
	
    void CheckGrip ()
	{
		// SI... Estamos tocando un muro (isWall).... y NO estamos tocando suelo.
		if (Checks.GetIsWall() && !Checks.GetIsGround()) 
		{
			// SI... el sprite NO está girado (mira a la derecha).
			// Y...  El movimiento horizontal (horizontalMove) es igual a 1 (derecha).
			// Y...  La altura (height) < 0 (está cayendo)

			if (horizontalDirection > 0f && this.transform.localScale.x > 0 ||
			    horizontalDirection < 0f && this.transform.localScale.x < 0)
			{
				isGrip = true;
				thisSprite.flipX = true;
			}
			else
			{
				isGrip = false;
			}
        }
		else 
		{
			isGrip = false;   
		}

		
        if (isGrip)
		{
            // Bajamos la gravedad del rigid al valor de gripGravity.
            // Ejecutamos la corrutina que Instancia el polvo de agarre.

            //thisSprite.flipX=true;
			if (thisRigid.velocity.y < 0)
			{
				thisRigid.gravityScale = gripGravity;
			}

            StartCoroutine(dustGrip(Checks.PositionOfCheckWall()));

		}
		
		// Enviamos el valor de grip al ANIMATOR.
		thisAnimator.SetBool (hashGrip, isGrip);											
	}
		

	/** FUNCION CHECKEA SI SE ESTA TOCANDO SUELO.
	* 
	***/
    void CheckGround ()
    {
      
		// SI... TOCA SUELO (isGround)..
        if (Checks.GetIsGround())
		{
			isBounce = false;
           
           
			// SI... no es el primer toque de suelo (llega de un salto)

			if (!firstTouch)
			{	
				// Instanciamos el gameobject que genera particulas de polvo al tocar el suelo en la posición y rotación del transform chkGround. 
				// Asigna a firstTouch a true
				// Reproduce el sonido guardado en el array de sonidos a un volumen igual que 'drop' (caida) / 0.5
				// Reinicia a 0 la variable drop (caida), 
				// Almacena la posicion Y del rigidBody cuando llega de un salto.
				
				height = 0f;
				drop = 0f;
				isGrip = false;
				isBounce = false;
                Instantiate(dustJump, detectGround, dustJump.transform.rotation);

				firstTouch = true;
				sourceofSounds.PlayOneShot (arrayOfSounds[0], 0.3f );
				thisRigid.gravityScale = 9f;

                thisRigid.velocity  = new Vector2 (0f,0f);
				currentForce = 0;
			}
		}
		// SINO TOCA SUELO... isGround = false, (Aún está en el aire)
		else
		{
			// No ha tocado el suelo (firstTouch = false).

			//isGround= false;
			firstTouch = false;

            /** DETECCION DE LA ALTURA Y CAIDA DEL RIGIDBODY
            * Almacena en 'height' la distancia devuelta por raycast2D de la posición del transform (chkGround), apuntando abajo.
            **/
            height = Physics2D.Raycast(detectGround, Vector2.down, 100f).distance;
            Debug.DrawRay(detectGround, Vector2.down*100f);
        }	

		// Si el valor absoluto de la altura mayor que 0f Y.. drop (caida) es menor que valor absoluto de la altura..
			if (Mathf.Abs(height) > 0f && drop < Mathf.Abs(height)) 
			{ 
				// Igualamos drop (caida), a el valor absoluto de altura (redondeando sin decimal para que funcione el sonido de caida)
				drop = Mathf.Abs(Mathf.Round (height * 1f) / 1f); 
			} 
		// Enviamos los valores al Animator.
		//thisAnimator.SetBool (hashIsGround, isGround);
		thisAnimator.SetFloat (hashHeight, height);
		thisAnimator.SetFloat (hashDrop, drop);
		//thisAnimator.SetBool("pieIzdo",groundLeft);
		//thisAnimator.SetBool("pieDcho",groundRight);
		//thisAnimator.SetInteger ("Tocando",toca);

	}

	/**
	 * Instancia el prefab almacenado en prefabDustGrip, en la posición indicada en posCheckWall
	 **/

	IEnumerator dustGrip (Vector3 posCheckWall)
	{
		posCheckWall.y += 0.65f;
		posCheckWall.x += 0.1f;
		Instantiate (prefabDustGrip , posCheckWall, thisSprite.transform.localRotation);
		yield return new WaitForSeconds (0.2f);
	}


    // PAUSA ANTES DE SALTO //
    IEnumerator StartJump (float seconds)
    {
        
        yield return new WaitForSeconds(seconds);

       

        //sourceofSounds.PlayOneShot (arrayOfSounds[1] , 1f);

    }

	
    public void rebotar ()
	{
		thisRigid.AddForce  (new Vector2 (thisRigid.velocity.x, 800f) , ForceMode2D.Force);
		sourceofSounds.PlayOneShot( arrayOfSounds[1] ,1f);
	}


	/** FUNCION KNOCK (RECIBE UN GOLPE)
	 * 
	 **/
	public void Knock (float force, float alt)
	{
		
		//thisAnimator.SetBool (hashKnock , knock);

			//thisRigid.velocity = Vector2.zero;
	
		thisRigid.AddForce (new Vector2 (force * directionKnock, alt), ForceMode2D.Force);


		StartCoroutine (recibeDaño ());
			

	}


	public IEnumerator recibeDaño ()
	{
		if (!muerto)
		{
			sourceofSounds.PlayOneShot(arrayOfSounds [2], 1f);
            //yield return new WaitForSeconds(1f);

            Debug.Log("golpazo");

			inmortal = true;
			StartCoroutine (Inmortal ());
			yield return new WaitForSeconds (0.5f);
			knock = false;
			thisAnimator.SetBool (hashKnock,knock);

			yield return new WaitForSeconds (inmortalTime);
			//knock = false;
			inmortal = false;


		}
		else
		{
			//StartCoroutine (morir ());

		}
	}

	/** FUNCION Inmortal
	 * 
	 **/
	public IEnumerator Inmortal()
	{
		while (inmortal)
		{
			thisSprite.color= new Color(1f,1f,1f,0.1f);
			yield return new WaitForSeconds (0.05f);
			thisSprite.color= new Color(1f,1f,1f,1f);
			yield return new WaitForSeconds (0.05f);


		}
	}


    /**
	IEnumerator Derrapar (float movHoriz)
	{

		// Variable sliding a true (se está deslizando)
		// Enviamos ese valor al Animator.
		// Reproducimos sonido del array correspondiente
		//sliding = true;
		if (!isSliding){
			
			isSliding = true;
			thisAnimator.SetBool (hashSlide, isSliding);
			sourceofSounds.PlayOneShot (arrayOfSounds[3] , 0.1f);

			//AnimatorStateInfo infoAnimator = animator.GetCurrentAnimatorStateInfo (0);
			

			//yield return new WaitForSeconds (0.3f);

			Vector3 temp = detectGround;

			if (movHoriz < 0)
			{
				temp.x = thisSprite.bounds.max.x;
			}
			else
			{
				temp.x = thisSprite.bounds.min.x;
			}

			temp.y = thisSprite.bounds.min.y;
			detectGround = temp;

			yield return new WaitForSeconds (0.2f);

			if (dustSlide != null) 
			{
				Instantiate (dustSlide, temp, dustSlide.transform.localRotation); 
			}

			thisAnimator.SetBool (hashSlide, isSliding);


			yield return new WaitForSeconds (0.8f);


			thisSprite.flipX = !thisSprite.flipX;
		
		}else {isSliding = false;}

	}

	**/



    // Getter y Setters ( para Cambiar o consultar valores de variables privadas desde otros Scripts).
    //public float getPlayerDirection () { return horizontalDirection; }
    public float GetDirection() { return horizontalDirection; }

	public float getPlayerDirection ()
	{
		if (thisSprite.flipX == true) {
			return -1f;
		} else {
			return 1f;
		}
	}

	public bool getPushing () { return this.pushing ;}
	public void setPushing (bool isPush) 
	{ 
		this.pushing = isPush;
		thisAnimator.SetBool(hashPush,this.pushing);
	}

	//public bool getIsGround () { return this.isGround;}

    /**
	public void setIsGround (bool isGround) 
	{ 
		this.isGround = isGround;
		thisAnimator.SetBool(hashIsGround,isGround);
	}
	**/

	//public void setIsWall (bool isWall) { this.isWall = isWall;}
	//public bool getIsWall () { return isWall; }

	public void setIsGrip (bool isGrip) { this.isGrip = isGrip;}
	public bool getIsGrip () { return this.isGrip; }

    public bool getIsInmortal() { return this.inmortal; }
    public void setIsInmortal (bool isInmortal) { this.inmortal = isInmortal; }

	public bool getAsKnock () { return this.knock; }
	public void setAsKnock (bool knock) 
	{ 
		this.knock = knock;
		thisAnimator.SetBool (hashKnock, this.knock);
	}

	public void setDirection (float direction)
	{  
		directionKnock = direction; 
	}

	public void setHidden (bool hidden)
	{
		this.hidden = hidden;
		thisAnimator.SetBool (hashHidden, hidden);
	}

	public bool getHidden ()
	{
		return hidden;
	}

    public void setBarro (bool barro){
        thisAnimator.SetBool (hashBarro,barro);
        isBarro = barro;
    }
}







