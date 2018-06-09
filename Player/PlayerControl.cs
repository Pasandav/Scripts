using UnityEngine;
using System.Collections;




public class PlayerControl : MonoBehaviour {

	/** VARIABLES PARA CHECKEAR CONTACTO DE LOS TRANSFORM DE ESTE OBJETO.
	 * 
	 **/
	
    private bool firstTouch;        // Variable tipo bool que almacena si acaba de tocar el suelo.

  
	[SerializeField]
	private LayerMask layerGroundAndWalls;	// Variable que almacena los valores indicados en inspector de las capas que deben detectar las variables booleanas 'isWall' e 'isGround'.

	/** VARIABLES DE MOVIMIENTO HORIZONTAL.
	 * 
	 **/
	[Header ("Movimiento HORIZONTAL")]
	[SerializeField]
	private float horizontalDirection;		// Variable que almacena el eje horizontal x (negativo izda, positivo dcha).
    private float lastDirection;

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


	/** VARIABLES DE SALTO INCREMENTAL
	 * 
	 **/
	[Header ("Variables de SALTO")]
	public bool startJump = false;				// Booleano que indica si se comienza a saltar.
	public float timePressing = 0.0f;			// Tiempo pulsando la tecla de salto
	public float maxTimeJump = 0.6f;			// Máximo tiempo que registra la pulsación
	public float minJumpForce = 7.8f;			// Fuerza minimima del salto.
	public float incrementJumpForce = 7f;		// Incrementos que se añaden al salto hasta que llega a la fuerza máxima.
    public float normalGravity = 9f;
    //public float height;						// Variable que contendrá la altura del salto.
	//public float drop;							// Variable que contendra la distancia de caida (drop)

	[Header ("Variables de AGARRADO")]
    public bool isGrip = false;					// Booleano que indica si está agarrandose a un muro.
	public float gripGravity = 2f;			// Gravedad cuando se agarra a un muro.
    //public bool lastGrip = false;

	[Space]

	[Header ("Variables de REBOTE")]
    [SerializeField] private bool isBounce = false;
    [SerializeField] private float bounceGravity = 2f;
    [SerializeField] private float bounceForceX;
    [SerializeField] private float bounceForceY;

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
    private int thisLayer;
    private int ignoreRaycastLayer;

    private bool isHidden = false;
    private string lastGround = "Vacío";
 

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
	
	private int hashGrip = Animator.StringToHash ("IsGrip");
	private int hashHorizontalX = Animator.StringToHash ("Direccion");
	
    private int hashHorizontalSpeed = Animator.StringToHash ("VelocidadH");
	private int hashVerticalSpeed = Animator.StringToHash ("VelocidadY");
    private int hashSubiendo = Animator.StringToHash("Subiendo");
    private int hashSlide = Animator.StringToHash("Derrapa");
	private int hashBounce = Animator.StringToHash ("Rebotando");
	private int hashPush = Animator.StringToHash ("Empujando");
	private int hashKnock = Animator.StringToHash ("Golpe");
	private int hashHidden = Animator.StringToHash ("Oculto");
    private int hashBarro = Animator.StringToHash ("Barro");
    private int hashAgachado = Animator.StringToHash("Agachado");


	private AudioSource sourceofSounds;
	// Arrays de sonidos (Arrastras los clips desde el Inspector)
	public AudioClip [] arrayOfSounds;

	void Awake ()
	{
        // COGEMOS COMPONENTES

        if (this.name.ToLower().Contains("player"))
        {
            // Asignamos componente SpriteRenderer del gameobject Sprite.
            thisSprite = this.transform.Find("Sprite").GetComponentInChildren<SpriteRenderer>();
            thisShadow = thisSprite.transform.Find("Sombra").GetComponentInChildren<SpriteRenderer>();

            
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
        thisRigid.gravityScale = normalGravity;	// Escala de gravedad.
		
		timePressing = maxTimeJump; 

		// Fuerzas x e y que se aplican al rebotar (bounce) si estamos agarrados (grip)
		bounceForceX = 5f;
		bounceForceY = 3f;

		currentForce = 0f;
		initialForce = 5f;
		maxVelocity = 2f;

		AcelerationForce = 5f;
		DecelerationForce = 75f;

        thisLayer = thisSprite.gameObject.layer;


        thisAnimator.SetInteger(hashAgachado, 0);
	}


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!knock)
        {
            knock = LayerMask.LayerToName(collision.gameObject.layer).ToLower().Contains("enemigos") ? true : false;
            directionKnock = collision.transform.position.x < this.transform.position.x ? 1f : -1f;
        }
    }


    void Update ()
	{
		
		// SI... Pulsamos la tecla de salto (Espacio)
		if (Input.GetKeyDown (KeyCode.Space)) {
			// Y ..SI .. estamos tocando suelo ('isGround')
            // Igualamos variable ('starJump') a true. (Comenzamos a saltar).
            // Igualamos a 0 la variable que controla el tiempo que pulsamos el salto (timePressing).
            if (Checks.IsGround())
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

        if (Input.GetKey (KeyCode.E) && Checks.IsWall())
		{
			//pushing = true;
		}

        if (Input.GetKeyUp (KeyCode.E) && Checks.IsWall())
		{
			//pushing = false;
		}

       

    }

	void FixedUpdate ()
	{
        // Si no nos golpean (knock) o no rebotamos (bounce)
        // Almacena el valor del eje X al pulsar las flechas horizontales (negativo izda, positivo dcha).
		if (!knock) {
			horizontalDirection = Input.GetAxisRaw ("Horizontal");

            //Igualamos la fuerza horizontal que se va a aplicar dependiendo del
            //valor absoluto de horizontal direcció
            currentForce = Mathf.Abs(horizontalDirection) > 0f ? 30f : 0f;

			// SI... estamos tocando suelo (isGround)..
            if (Checks.IsGround()) 
            {
                if (!lastGround.Contains(Checks.GetTypeOfGround()))
                {
                    lastGround = Checks.GetTypeOfGround();
                    Debug.Log("Valor de type of ground = " + Checks.GetTypeOfGround() + " y lastGround = " + lastGround);
                    // Sino... (estamos en barro)..
                    // Aumentamos el "rozamiento" del rigidbody
                    // Igualamos la fuerza minima de salto
                    if (Checks.GetTypeOfGround().Contains("barro"))
                    {
                        thisAnimator.SetBool(hashBarro, true);
                        thisRigid.drag = 40f;
                        minJumpForce = 1f;
                    }
                    // Si no estamos tocando barro (!isBarro)..
                    // Igualamos la velocidad máxima del rigidbody
                    // Igualamos la fuerza minima de salto
                    else
                    {
                        thisAnimator.SetBool(hashBarro, false);
                        thisRigid.drag = 0f;
                        minJumpForce = 7.8f;
                        thisShadow.color = new Color(0f, 0f, 0f, 0.5f);
                    }
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
            this.thisAnimator.SetBool(hashKnock, knock);
            Knock (1100f, directionKnock);
            }
		
        // Almacena el valor absoluto (positivo) de la velocidad del rigidbody.
        velocityX = Mathf.Abs(thisRigid.velocity.x);

        // SI.. la velocidad x del rigidbody es mayor o igual que la velocidad maxima horizontal ('maxVelocityX')
        // Igualamos la velocidad del rigidbody al valor de 'maxVelocityX'.
        thisRigid.velocity = velocityX >= maxVelocity && !isBounce ? new Vector2(maxVelocity * horizontalDirection, thisRigid.velocity.y) : thisRigid.velocity;

        /** Llama a funciones que comprueban
       * Si tocamos suelo.
       * Si tocamos pared.
       * La direccion del Sprite
       * Si nos agarramos a una pared.
       **/

        if (isHidden) 
        { 
            Detectable(false); 
        }
       
      

        CheckGround();
        Jump();
        Girar();
        SendToAnimator();
	}

    void SendToAnimator()
    {
        // Envía los valores al Animator.
        thisAnimator.SetFloat(hashHorizontalX, horizontalDirection);
        thisAnimator.SetFloat(hashVerticalSpeed, thisRigid.velocity.y);
        thisAnimator.SetFloat(hashSubiendo, thisRigid.velocity.normalized.y);
        thisAnimator.SetFloat(hashHorizontalSpeed, velocityX);
        thisAnimator.SetBool(hashBounce, isBounce);
        thisAnimator.SetBool("StartJump", startJump); 
        // Enviamos el valor de grip al ANIMATOR.
        thisAnimator.SetBool(hashGrip, isGrip);
        thisAnimator.SetBool(hashKnock, knock);

    }
	
    /** FUNCION CHECKEA SI SE ESTA TOCANDO SUELO.
    * 
    ***/
    void CheckGround()
    {
        // SI... TOCA SUELO (isGround)..
        if (Checks.IsGround())
        {
            // SI... no es el primer toque de suelo (llega de un salto)
            if (!firstTouch) { FirstTouch(); }
        }
        // SINO TOCA SUELO... isGround = false, (Aún está en el aire)
        else
        {
            firstTouch = false;
            Debug.DrawRay(Checks.CheckGroundPosition(), Vector2.down * Checks.HeightToGround);
            CheckGrip();
        }
    }

    // Funcion que Iguala los valores cuando toca suelo por primera vez después de un salto.
    private void FirstTouch()
    {
        firstTouch = true;
        isGrip = false;
        isBounce = false;

        Checks.HeightToGround = 0f;
        Checks.Drop = 0f;
        currentForce = 0f;

        Instantiate(dustJump, Checks.CheckGroundPosition(), dustJump.transform.rotation);
        sourceofSounds.PlayOneShot(arrayOfSounds[0], 0.3f);

        thisRigid.gravityScale = normalGravity;
        thisRigid.velocity = new Vector2(0f, 0f);
    }

    // FUNCION CHECKGRIP (Chequea si se está agarrando a una pared).
    void CheckGrip()
    {
        // SI... NO estamos tocando suelo y estamos tocando un muro
        if (Checks.IsWall())
        {
            // SI... la dirección es igual a la escala del transform y está callendo.
            if (horizontalDirection > 0f && this.transform.localScale.x > 0 && thisRigid.velocity.y < 0f ||
                horizontalDirection < 0f && this.transform.localScale.x < 0 && thisRigid.velocity.y < 0f)
            {
                isGrip = true;
                thisSprite.flipX = true;
                thisRigid.gravityScale = gripGravity;
                StartCoroutine(DustGrip(Checks.CheckWallPosition()));
            }
            else
            {
                // NO se está agarrando (isGrip)...
                // Se iguala la gravedad del rigidbody en función de si se está rebotando (isBounce)
                isGrip = false;
                thisRigid.gravityScale = isBounce ? bounceGravity : normalGravity;
            }
        }
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
        thisRigid.gravityScale = bounceGravity;
      
		thisRigid.AddForce (new Vector2 (-direction * bounceForceX, bounceForceY), ForceMode2D.Impulse);	
        sourceofSounds.PlayOneShot(arrayOfSounds[1], 1f);

        yield return new WaitForSeconds (0.5f);
	    
        isBounce = false;
        thisRigid.gravityScale = normalGravity;
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
			    Checks.IsGround() && thisSprite.flipX == true)
			{
				temp.x *= -1f;
			    thisSprite.flipX = false;
			}
            
        this.transform.localScale = temp;
	}
	
    /**
	 * Instancia el prefab almacenado en prefabDustGrip, en la posición indicada en posCheckWall
	 **/

    IEnumerator DustGrip (Vector3 posCheckWall)
	{
		posCheckWall.y += 0.65f;
		posCheckWall.x += 0.1f;
		Instantiate (prefabDustGrip , posCheckWall, thisSprite.transform.localRotation);
		yield return new WaitForSeconds (0.2f);
	}



   

    public void rebotar ()
	{
		thisRigid.AddForce  (new Vector2 (thisRigid.velocity.x, 800f) , ForceMode2D.Force);
		sourceofSounds.PlayOneShot( arrayOfSounds[1] ,1f);
	}


	/** FUNCION KNOCK (RECIBE UN GOLPE)
	 * 
	 **/
	public void Knock (float force, float dir)
	{
        

        thisRigid.AddForce(new Vector2(force * dir, 0f), ForceMode2D.Impulse);
        if (!inmortal)
        {
            
            sourceofSounds.PlayOneShot(arrayOfSounds[2], 1f);
            Detectable(false);
          
            StartCoroutine(recibeDaño());

        }


	}


	public IEnumerator recibeDaño ()
	{
        if (!muerto)
		{
			
            //yield return new WaitForSeconds(1f);
            //int temp = thisSprite.gameObject.layer;


			inmortal = true;

            //this.gameObject.layer = 2;

            StartCoroutine (Parpadea (0.2f));
			
            yield return new WaitForSeconds (0.5f);
            knock = false;
			thisAnimator.SetBool (hashKnock,knock);

			yield return new WaitForSeconds (inmortalTime);
			
			inmortal = false;
            Detectable(true);
		}
		else
		{
			//StartCoroutine (morir ());

		}
	}

	/** FUNCION Inmortal
	 * 
	 **/
	private IEnumerator Parpadea(float time)
	{
		while (inmortal)
		{
			thisSprite.color= new Color(1f,1f,1f,0.1f);
			yield return new WaitForSeconds (time);
			thisSprite.color= new Color(1f,1f,1f,1f);
			yield return new WaitForSeconds (time);
		}
	}

    private void Detectable (bool detect)
    {
        thisSprite.gameObject.layer = detect ? thisLayer : 2;
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

    public float GetPlayerDirection () { return  thisSprite.flipX ? -1f : 1f; }

	public bool getPushing () { return this.pushing ;}
	public void setPushing (bool isPush) 
	{ 
		this.pushing = isPush;
		thisAnimator.SetBool(hashPush,this.pushing);
	}


	//public void setIsGrip (bool isGrip) { this.isGrip = isGrip;}
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

    public bool IsHidden
    {
        set  
        { 
            this.isHidden = value;
            thisAnimator.SetBool(hashHidden, value);
        }

        get { return this.isHidden; }
    }


  
}







