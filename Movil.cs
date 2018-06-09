using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Movil : MonoBehaviour {


	private Transform playerTransform;		// Variable tipo Trasform que comprobará la posición del Jugador
	private SpriteRenderer playerSprite;
	//private ControlPlayer01 playerAgarra;

	private SpriteRenderer thisSprite;		// Variable tipo SpriteRenderer que contendrá éste sprite
	private Rigidbody2D thisRigid;

	private Canvas thisCanvas;
	public Image [] imagesOfCanvas;

	private SpriteRenderer otherSprite;		// Variable tipo SpriteRenderer que contendrá el sprite más cercano

	public LayerMask detectLayer;			// Variable tipo LayerMask indicará las capas en el Inspector para detectar



	public GameObject dust;

	private RaycastHit2D rayRight;	// Guarda la información detectada por el RayCast
	private float direction;	// Indica en que dirección se lanzará el RayCast

	private PlayerControl ctrlPlayer;

	//	private int thisOrderInLayer;
	private int colisionOrderInLayer;
	private float posicionObstaculo;
	public bool choqueObstaculo;
	public bool agarrado = false;

    //GameObject [] player; 

    CheckGroundAndWalls Checks;
	//Vector3 ancho;	


	void Start () {

		// Almacenamos en un array de GameObjects, todos los que están en el tag "Player".
		// Buscamos en el array el transform que no de nulo en rigidbody (Padre de todos los demás).
		// Almacenamos el transform de ese gameobject en 'playertransform'.

		GameObject [] player = GameObject.FindGameObjectsWithTag ("Player");
		foreach (GameObject play in player) {
			if (play.GetComponent <Rigidbody2D>() != null)
			{
				playerTransform = play.GetComponent <Transform>();
			}
			if (play.GetComponent <Animator>() != null)
			{
				//playerSprite = play.GetComponent <SpriteRenderer>();
			}
			if (play.GetComponent <PlayerControl>() != null)
			{
				ctrlPlayer = play.GetComponent <PlayerControl>();
			}
		}
        Checks = this.GetComponent<CheckGroundAndWalls>();

        // Almacenamos en las variables creadas los Componentes de cada GameObject necesario.
		thisSprite = GetComponent <SpriteRenderer> ();
		thisRigid = GetComponent <Rigidbody2D>();
		thisCanvas = this.transform.Find ("Canvas").GetComponent <Canvas>();
		imagesOfCanvas = thisCanvas.GetComponentsInChildren <Image>();


		// Activamos la variable isKinematic de éste RigidBody (No le afecta la física
		// Desactivamos el canvas que muestra el Tip
		thisRigid.isKinematic = true;
		thisCanvas.enabled = false;

	}
	
	// Update is called once per frame
	void Update () {

		Vector2 startRay = thisSprite.bounds.center;


		if ( playerTransform.position.x < this.transform.position.x)
		{
			direction = 1f;
			startRay.x = thisSprite.bounds.max.x;
			//thisSprite.sortingOrder = playerSprite.sortingOrder + 1;
		}
		else
		{
			direction = -1f;
			startRay.x = thisSprite.bounds.min.x;
			//thisSprite.sortingOrder = playerSprite.sortingOrder - 1;
		}
			
		rayRight = Physics2D.Raycast (startRay, Vector2.right * direction, 1f , detectLayer);
		Debug.DrawRay (startRay, (Vector3.right * direction) * 1f, Color.cyan);


		// Mueve el orden de capas según esté a la derecha o a la izquierda de un obstaculo.
		if (rayRight.collider != null && rayRight.transform.GetComponent <SpriteRenderer> () != null)
		{
			otherSprite = rayRight.transform.GetComponent <SpriteRenderer> ();

			if (thisSprite.transform.position.x < otherSprite.transform.position.x)
			{
				thisSprite.sortingOrder = otherSprite.sortingOrder - 1;
			}else{
				thisSprite.sortingOrder = otherSprite.sortingOrder + 1;
			}
		}
	}


	void FixedUpdate (){





		/**
		if (agarrado){

			Vector3 posicionMovil = this.transform.position;
			//rigid.isKinematic = false;



			posicionMovil.x = posicionPlayer.position.x + 0.1f;
			this.transform.localPosition = posicionMovil;

			this.transform.parent = posicionPlayer;


			//Vector3 posX = this.transform.position;
			//posX.x = movimientoX.position.x;
			//this.transform.position -= posX;
		}else{ 
			rigid.isKinematic = false;
			this.transform.parent = null;
			}

	}

	void OnTriggerEnter2D (Collider2D otro){

		if (otro.gameObject.name == "Destructor"){
			Destroy (this.gameObject, 1f);
		}
		**/
	}


	void OnCollisionStay2D (Collision2D other)
	{
        if (other.transform.tag == "Player" && Checks.IsWall())
		{
			if (ctrlPlayer.getPushing() == false)
			{
				foreach ( Image a in imagesOfCanvas)
				{
                    if (a.name.ToLower().Contains("button"))
					{
						a.enabled = true;
					}else{
						a.enabled = false;
					}
				}
				thisCanvas.enabled = true;
				thisRigid.isKinematic = true;
			}else{
				if (playerTransform.position.x < this.transform.position.x)
				{
					foreach (Image a in imagesOfCanvas)
					{
                        if (a.name.ToLower().Contains ("right"))
						{
							a.enabled = true;
						}else{
							a.enabled = false;
						}
					}
				}else{
					foreach (Image a in imagesOfCanvas)
					{
                        if (a.name.ToLower().Contains ("left"))
						{
							a.enabled = true;
						}else{
							a.enabled = false;
						}
					}
				}
				thisRigid.isKinematic = false;

				if (Mathf.Abs( thisRigid.velocity.x) > 0.3f) { 
					thisCanvas.enabled = false;
				}else{
					thisCanvas.enabled = true;
				}
			
			}


		}
	}


		void OnCollisionExit2D (Collision2D other)
		{
			thisCanvas.enabled = false;
		    thisRigid.isKinematic = true;
		}

}
