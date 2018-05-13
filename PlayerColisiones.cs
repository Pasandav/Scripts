using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using CreativeSpore.SuperTilemapEditor;

public class PlayerColisiones : MonoBehaviour {

	// Los enemigos deben de estar en esta máscara
    public LayerMask collisionLayers;

	PlayerControl ctrlPlayer;
    SpriteRenderer thisSprite;
    Animator thisAnimator;

   
    private float direction;

    STETilemap tilemap;
    TileColliderData tilecol;
    Tile tile;
    TilemapChunk tileChunk;

    Vector2 checkGround;
    bool isGround;

    private Vector3 checkWall;
    private bool isWall;


    public Transform pieIzquierdo;
    public Transform pieDerecho;

    int hashIsGround;
    int hashIsWall;

    int hash_Barro;

    //private GameObject transformMovil = null;
    //public GameObject [] tipoDeEnemigo;

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(255f, 100f, 32f, 100f);
        Gizmos.DrawWireSphere(checkGround,0.1f);

        Gizmos.color = new Color(0f, 255f, 0f, 100f);
        Gizmos.DrawWireSphere(checkWall, 0.1f);
       // tileChunk.DrawColliders();

    }


    void Start () 
	{
		ctrlPlayer = this.transform.GetComponent <PlayerControl> ();
        thisSprite = this.GetComponentInChildren<SpriteRenderer>();
        thisAnimator = this.GetComponentInChildren<Animator>();

    }

    private void Awake()
    {
        hashIsGround = Animator.StringToHash("Ground");
        hashIsWall = Animator.StringToHash("Wall");

        hash_Barro = "Barro".GetHashCode();

    }

    private void FixedUpdate()
    {
        
        //CheckGround();
        //CheckWall();
    }


   

    private void CheckGround ()
    {
        checkGround.x = thisSprite.bounds.center.x;
        checkGround.y = thisSprite.bounds.min.y;

        isGround = Physics2D.OverlapCircle (checkGround, 0.1f, collisionLayers);
        thisAnimator.SetBool (hashIsGround, isGround);

        //ctrlPlayer.setIsGround(isGround);
    }

  


    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.transform.GetComponentInChildren<TilemapChunk>() != null)
        {
            tileChunk = other.transform.GetComponentInChildren<TilemapChunk>();

            if (tileChunk.tag.ToLower().Contains("barro"))
            {
                ctrlPlayer.setBarro(false);
            }

        }

       
    }


    void OnTriggerEnter2D (Collider2D other)
	{
        if (other.transform.GetComponentInChildren<TilemapChunk>() != null)
        {
            tileChunk = other.transform.GetComponentInChildren<TilemapChunk>();

            if (tileChunk.tag.ToLower().Contains("barro"))
            {
                ctrlPlayer.setBarro(true);
            }

        }
      

        // SI.. el valor de layer del objeto colisionado == el valor de layer seleccionado en "enemiesMask" del inspector...
        if ( (1 << other.gameObject.layer) == collisionLayers.value)
		{

           

            if (ctrlPlayer.getIsInmortal() == false && ctrlPlayer.muerto == false)
			{
				ctrlPlayer.rebotar ();

				if (other.tag.Contains ("Easy"))
				{
					other.GetComponent <Enemy_Controller>().Muere();
				}
			}
		}
	}

    void OnTriggerStay2D(Collider2D other)
    {
        

        if (other.transform.GetComponentInChildren<TilemapChunk>() != null )
        {
            tileChunk = other.transform.GetComponentInChildren<TilemapChunk>();

            if (tileChunk.tag.ToLower().Contains("barro"))
            {
                ctrlPlayer.setBarro(true);
            }

        }
        if ((1 << other.gameObject.layer) == collisionLayers.value)
        {
            if (ctrlPlayer.getIsInmortal() == false && ctrlPlayer.muerto == false)
            {
                //float direction;  

                if (other.transform.position.x > this.transform.position.x)
                //if (other.transform.position.x > this.transform.position.x)
                {
                    direction = -1f;
                }
                else
                {
                    direction = 1f;
                }


                //ctrlPlayer.Knock ( 1070f * direction);
            }
        }
    }
	

    void OnCollisionEnter2D (Collision2D other)
	{
        
        if (LayerMask.LayerToName (other.gameObject.layer).ToLower().Contains("enemigos"))
        //if ((1 << other.gameObject.layer) == collisionLayers.value)
			{


           // Debug.Log(tilemap.GetTileData(ctrlPlayer.gizmoRight.localPosition));
            //string Sor = tilemap.SortingLayerName;
           // Debug.Log(Sor);

            if (ctrlPlayer.getIsInmortal() == false && ctrlPlayer.muerto == false)
				{
                Debug.Log("inmortal");
                ctrlPlayer.setIsInmortal (true);
                //float direction;	
					if (other.transform.position.x > this.transform.position.x)
					{
						direction = -1f;
					}
					else
					{
						direction = 1f;
					}

				ctrlPlayer.setAsKnock (true);
				ctrlPlayer.setDirection (direction);

				//ctrlPlayer.Knock (500f,5f);
					
				Debug.Log ("RECIBE GOLPE EN DIRECCION: " +  direction);
					
					
            }else{
                ctrlPlayer.setAsKnock(false);
            }
			}
			
		}

	void OnCollisionExit2D (Collision2D other)
	{
        tileChunk = other.transform.GetComponentInChildren <TilemapChunk>();

        if (tileChunk.tag.ToLower().Contains("barro"))
        {
            ctrlPlayer.setBarro(false);

        }

       
        if (LayerMask.LayerToName(other.gameObject.layer).ToLower().Contains("enemigos"))
        //if ( (1 << other.gameObject.layer) == collisionLayers.value)
		{

            if (ctrlPlayer.getIsInmortal() == false && ctrlPlayer.muerto == false)
			{
				ctrlPlayer.setAsKnock (false);
               

			}
		}

	}
	

	




}