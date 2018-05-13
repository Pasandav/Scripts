using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreativeSpore.SuperTilemapEditor;


[RequireComponent(typeof(Rigidbody2D))]
public class CheckGroundAndWalls : MonoBehaviour {

    public LayerMask layersOfDetection;

    private Vector2 chkGround;
    private bool isGround;

    private Vector2 chkWall;
    private bool isWall;

    private float direction;

    private CapsuleCollider2D thisCapsuleCollider;
    private Animator thisAnimator;
    private TilemapChunk tilemapChunk;


    int hashIsGround = Animator.StringToHash("IsGround");
    int hashIsWall = Animator.StringToHash("IsWall");
    int hashDireccion = Animator.StringToHash("Direccion");

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(255f, 0f, 0f);
        Gizmos.DrawSphere(chkWall, 0.05f);

        Gizmos.color = new Color(0f, 255f, 0f);
        Gizmos.DrawSphere(chkGround, 0.05f);
    }
	

    // Use this for initialization
	void Start () 
    {
        thisCapsuleCollider = this.GetComponentInChildren<CapsuleCollider2D>();
        thisAnimator = this.GetComponentInChildren<Animator>();
    }
	

	private void FixedUpdate()
	{
        CheckGround();
        CheckWall();
        thisAnimator.SetFloat(hashDireccion, direction);
	}

    //Función que comprueba si se está tocando suelo.
    private void CheckGround()
    {
        //Situamos un Vector2 con posición x en en centro del capsulecollider e y en la parte inferior del capsulecollider.
        chkGround.x = thisCapsuleCollider.bounds.center.x;
        chkGround.y = thisCapsuleCollider.bounds.min.y;

        // Igualamos la variable booleana con el valor devuelto por la funcion de Physics2D
        // y enviamos el valor al booleano del Animator.
        isGround = Physics2D.OverlapCircle(chkGround, 0.1f, layersOfDetection);
        thisAnimator.SetBool(hashIsGround, isGround);
    }

    //Funcion que comprueba si se está tocando un muro.
    private void CheckWall()
    {
        //Situamos el vector2 con posición en y en la parte central inferior del capsulecollider.
        chkWall.y = thisCapsuleCollider.bounds.center.y;

        //Igualamos en variable "direction" (float), la escala local en x del transform (negativa izquierda, positiva derecha).
		direction = this.transform.localScale.x;

        //Si es mayor que 0 (positiva)....
        //Situamos el vector2 en x en la parte derecha del collider + 0.05
        //Si es menor que 0 (negativa)...
        //Situamos el vector2 en x en la parte izquierda del collider - 0.05
        chkWall.x = direction > 0 ? thisCapsuleCollider.bounds.max.x + 0.05f : thisCapsuleCollider.bounds.min.x - 0.05f;

        // Igualamos la variable booleana con el valor devuelto por la funcion de Physics2D
        // y enviamos el valor al booleano del Animator.
        isWall = Physics2D.OverlapCircle(chkWall, 0.1f, layersOfDetection);
        thisAnimator.SetBool(hashIsWall, isWall); 
    }

	private void OnCollisionStay2D(Collision2D collision)
	{
        tilemapChunk = collision.transform.GetComponent<TilemapChunk>() != null ? collision.transform.GetComponentInChildren<TilemapChunk>() : null;
	}


	//Función publica para coger el valor de la variables isWall.
	public bool GetIsWall() { return this.isWall; }

    public Vector2 PositionOfCheckWall() { return chkWall; }

    //Función publica para coger el valor de la variable isGround.
    public bool GetIsGround() { return this.isGround; }

    public string GetTypeOfGround()
    {
        if (tilemapChunk != null )
        {
            return tilemapChunk.SortingLayerName.ToLower();
        }
        else
        {
            return "Vacío";
        }
    }

}