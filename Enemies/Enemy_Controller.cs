using UnityEngine;
using System.Collections;


public class Enemy_Controller : MonoBehaviour {
	

    CapsuleCollider2D cuerpo;
    CircleCollider2D cabeza;
   
	Animator thisAnimator;
	Rigidbody2D thisRigid;

    SpriteRenderer thisSprite;
    public Color initialColor = new Color(1f, 1f, 1f, 1f);
    private Color shadowColor = new Color(0f, 0f, 0f, 0.3f);
    public int initialOrder;

    CheckGroundAndWalls checks;

    PlayerControl playerScript;

	Vector3 ojos;

    AudioSource sonidosPeon;
    public AudioClip pisada;

	private RaycastHit2D whatDetect;
	public LayerMask detectLayer;

	public  GameObject [] Items;

	public int maxItems = 0;

	private float initialScale;

	public float speed;



    private float randomDirection;


	[Header("Min y Max tiempo andando")] 
	[Range(1, 5)] public byte minAndando = 5;
	[Range (10,15)] public byte maxAndando = 15;
    
	[Header("Min y Max tiempo sentado")]
    [Range(1, 5)] public byte minSentado = 1;
    [Range(1,5)] public byte maxSentado = 4;

    // Variables que indican el tiempo que va a estar andando (aleatorio)
    private float actualTiempoAndando;
    private byte maximoTiempoAndando;
    // Variables que indican el tiempo que va a estar sentado.
    private float actualTiempoSentado;
    private byte proximoTiempoSentado;

    private bool sentado;
    private bool girando;

    [SerializeField]private bool prohibido = false;


    [Header ("Distancia para pillar")][Range (1,5)] public byte distanciaPillar;
    
	public bool pillado;
    public bool inmortal;
    private string capaAnterior;

	// Use this for initialization
    int hashPillado = Animator.StringToHash ("Pillado");
	int hashVelocidad = Animator.StringToHash ("Velocidad");
    int hashGira = Animator.StringToHash("Gira");
    int hashSentado = Animator.StringToHash("Sentado");
    int hashDistancia = Animator.StringToHash("Distancia");
    int hashDireccion = Animator.StringToHash("Direccion");
	private int hashHeight = Animator.StringToHash("Height");

    void Start () 
    {	
        cuerpo = this.GetComponentInChildren<CapsuleCollider2D> ();
        cabeza = this.GetComponentInChildren <CircleCollider2D>();
        thisAnimator = this.GetComponentInChildren<Animator>();
        thisSprite = this.GetComponentInChildren<SpriteRenderer>();

		sonidosPeon = this.GetComponent <AudioSource>();
		thisRigid = this.GetComponent<Rigidbody2D>();
		checks = this.GetComponent <CheckGroundAndWalls>();

		initialScale = this.transform.localScale.x;
        initialOrder = thisSprite.sortingOrder;

		GenerarDireccion();
        
		speed = 1f;
        pillado = false;
        distanciaPillar = 3;

        maximoTiempoAndando = (byte)Random.Range(minAndando, maxAndando);
        actualTiempoAndando = 0;

        proximoTiempoSentado = (byte)Random.Range(minSentado, maxSentado);
        actualTiempoSentado = 0;

    }
	
	void FixedUpdate () 
	{
		thisRigid.velocity = new Vector2(2 * randomDirection * speed, thisRigid.velocity.y);
		if (thisRigid.velocity.x > 0.5f)
		{
			thisRigid.AddForce(Vector2.right * speed, ForceMode2D.Force);
		}
        Pillando();

        CheckAndarYSentar();
        //inmortal = player.gameObject.GetComponentInParent<PlayerControl>().getIsInmortal();

        EnviarAlAnimator();
		


	}

    private void EnviarAlAnimator()
    {
        thisAnimator.SetBool(hashPillado, pillado);
        thisAnimator.SetFloat(hashVelocidad, Mathf.Abs(thisRigid.velocity.x));
        thisAnimator.SetBool(hashSentado, sentado);
        thisAnimator.SetFloat(hashDireccion, randomDirection);
        thisAnimator.SetFloat(hashDistancia, Mathf.Abs(whatDetect.distance));
    }

    private void OnCollisionExit2D(Collision2D otro)
    {
        string capa = LayerMask.LayerToName(otro.gameObject.layer).ToLower();

        if (capa.Contains("obstaculos") && prohibido == true)
        {
            prohibido = false;
        }
    }

    private void OnCollisionStay2D(Collision2D otro)
    {
        string capa = LayerMask.LayerToName(otro.gameObject.layer).ToLower();

        if (capa.Contains("obstaculos"))
        {
            prohibido = true;
        }
    }

    void OnCollisionEnter2D (Collision2D otro)
    {
        string capa = LayerMask.LayerToName (otro.gameObject.layer).ToLower();
        string etiqueta = otro.transform.gameObject.tag.ToLower();


        if (checks.IsWall() && !pillado && !girando && !sentado)
        {
            StartCoroutine(PararYGirar());   
        }

        if (otro.gameObject.tag.ToLower().Contains("player"))
        {
            
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        thisSprite.sortingOrder = initialOrder;
        thisSprite.color = initialColor;
    }

    private void OnTriggerStay2D(Collider2D otro)
    {
        if (otro.gameObject.GetComponent<Translucido>() != null)
        {
            Translucido objTrans = otro.gameObject.GetComponent<Translucido>();

            thisSprite.sortingOrder = objTrans.IsTranslucent ? initialOrder : objTrans.HigherOrderInLayer + 1;
            thisSprite.color = objTrans.IsTranslucent ? initialColor : shadowColor;
        }  
    }

    void OnTriggerEnter2D (Collider2D otro)
	{
        if (otro.gameObject.GetComponent<Translucido>() != null)
        {
            Translucido objTrans = otro.gameObject.GetComponent<Translucido>();

            thisSprite.sortingOrder = objTrans.IsTranslucent ? initialOrder : objTrans.HigherOrderInLayer + 1;
            thisSprite.color = objTrans.IsTranslucent ? initialColor : shadowColor;
        }

		
        if (otro.gameObject.name == "Destructor" )
		{
			Destroy (this.gameObject);
			//StartCoroutine  (destruye ());
		}

		if (otro.gameObject.name == "FinDeNivel"){
			//Girar ();
		}
	}

    /** Método GenerarDireccion
     * 
     * 
     **/
    void GenerarDireccion()
	{
		randomDirection = (Random.value > 0.5f ? initialScale : - initialScale);

		this.transform.localScale = new Vector2(randomDirection, this.transform.localScale.y);
    }

    // Metodo Pillando
    void Pillando()
    {

        ojos = cabeza.bounds.min;
        Debug.DrawRay(ojos, (Vector2.right * randomDirection) * 5f, Color.green);

        whatDetect = Physics2D.Raycast(ojos, Vector2.right * randomDirection, distanciaPillar, detectLayer);

        // Si el raycast devuelve un collider...
       
        string nombreDeCapa = whatDetect.collider != null ? LayerMask.LayerToName(whatDetect.collider.gameObject.layer).ToLower() : null;
        pillado = nombreDeCapa != null && nombreDeCapa.Contains("player") && !prohibido && whatDetect.distance <= distanciaPillar  ? true : false;
			
		if (pillado)
		{
            Debug.DrawRay(ojos, ((Vector2.right * randomDirection) * whatDetect.distance), Color.red);

            if (speed <= 1) { speed += 0.4f; }


        }
        else 
		{ 
			if (speed > 1) { speed -= 0.4f; }
        }
       
		
            
    }

    // Checkea cuanto y cuando anda o se sienta.
    void CheckAndarYSentar()
    {
        // Si NO está sentado.
        if (!sentado && !pillado && !girando)
        {
            //Si el valor entero de los segundos que lleva andando es menor que
            if (Mathf.FloorToInt(actualTiempoAndando) < maximoTiempoAndando)
            {
                actualTiempoAndando += Time.deltaTime;
            }
            else if (Mathf.FloorToInt(actualTiempoAndando) == maximoTiempoAndando)
            {
                actualTiempoAndando = 0;
                maximoTiempoAndando = (byte)Random.Range(minAndando, maxAndando);
                sentado = true;
                speed = 0f;
            }
        }

        // Si está sentado...
        if (sentado)
        {
            //Si los segundos estando sentado son menores que los almacenados en próximoTiempoSentado.
            if (Mathf.FloorToInt(actualTiempoSentado) < proximoTiempoSentado)
            {
                actualTiempoSentado += Time.deltaTime;
            }
            else if (Mathf.FloorToInt(actualTiempoSentado) == proximoTiempoSentado)
            {
                actualTiempoSentado = 0;
                proximoTiempoSentado = (byte)Random.Range(minSentado, maxSentado);
                sentado = false;
                speed = 1f;
            }
        }
    }

    //METODO PARARYGIRAR

    private IEnumerator PararYGirar()
    {
		speed = 0f;
        thisAnimator.SetFloat(hashVelocidad, speed);

		girando = true;
        thisAnimator.SetBool(hashGira, girando);

		if (this.transform.name.ToLower().Contains("peon"))
        {
            yield return new WaitForSeconds(1.3f);
            speed = 1f;
        }

        Vector2 temp = this.transform.localScale;
        temp.x *= -1;
        this.transform.localScale = temp;
       
        randomDirection *= -1;
        girando = false;
        thisAnimator.SetBool(hashGira, girando);
    }
	
    // METODO SONIDO PISADA
    public void SonidoPisada()
    {
        sonidosPeon.volume = 0.1f;
        sonidosPeon.PlayOneShot(pisada);
    }


    public void Muere(){
		

		if (this.gameObject != null){
			thisAnimator.SetBool ("Muere", true);

			cuerpo.enabled = false;
			cabeza.enabled = false;
			thisRigid.AddForce (Vector2.up * 5f,ForceMode2D.Impulse);
			//rigidbody.AddForce ((- rigidbody.velocity) *5, ForceMode2D.Impulse);

			if (Items.Length > 0 && maxItems == 0){


				Instantiate (Items [Random.Range (0, Items.Length )], this.transform.position, this.transform.rotation);
				//maxItems += 1;
			}


			sonidosPeon.PlayOneShot (pisada);


			StartCoroutine (destruye ());


		}

	}

   

    private IEnumerator Reir ()
	{
		thisAnimator.SetBool ("Reir", true);
		yield return new WaitForSeconds (1.5f);
		thisAnimator.SetBool ("Reir", false);
	}

	private IEnumerator destruye (){
		yield return new WaitForSeconds (1);
		thisRigid.simulated = false;

		yield return new WaitForSeconds (1);

		Destroy (this.gameObject);
	}

    public float GetDirection ()
    {
        return this.randomDirection;
    }

}

