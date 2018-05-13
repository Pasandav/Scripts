using UnityEngine;
using System.Collections;

public class PlayerLayersControl : MonoBehaviour {

	//private Vector3 detectWall;
	private  SpriteRenderer spritePlayer;
	private SpriteRenderer leftSprite;
	private SpriteRenderer rightSprite;

	private SpriteRenderer downRightSprite;

	private SpriteRenderer upSprite;

	private SpriteRenderer lastSpriteLeft;
	private SpriteRenderer lastSpriteRight;

	private Collider2D downCollider;

	private SpriteRenderer LayerObstacleVert;

	private RaycastHit2D leftCollision;
	private RaycastHit2D rightCollision;
	private RaycastHit2D downLeftCollision;
	private RaycastHit2D downRightCollision;


	private RaycastHit2D upCollision;
	private RaycastHit2D downCollision;
	private RaycastHit2D collisionVertical;


	private Vector3 originRay;
	public Transform leftFeet;
	public Transform rightFeet;

	[Header ("Capas a detectar")]
	public LayerMask whatLayer;
	private PlayerControl scriptControlPlayer;
	[Header ("Rayos de detección")]
	//public Transform rayHorizontalStart;

	private Vector2 rayHorizontalStart;
	//private float dirHorizontal;
	//private Vector3 chkDistanceHoriz;
	//private Transform chkDistanceVert;
	// Use this for initialization
	public float distanceRight;
	public float distanceLeft;
	public float distanceUp;
	public float distanceDown;
	void Start () 
	{

		spritePlayer = this.transform.Find ("Sprite").GetComponent <SpriteRenderer>();
		//chkDistanceHoriz = this.transform.FindChild ("WallCheck").GetComponent <Transform>();
		//chkDistanceVert = this.transform.FindChild ("GroundCheck").GetComponent <Transform>();
	}

	void Update ()
	{

	}


	// Update is called once per frame
	void FixedUpdate ()
	{
		//detectWall = spritePlayer.bounds.center;
		rayHorizontalStart = spritePlayer.bounds.center;
		originRay.x = spritePlayer.bounds.center.x;
		originRay.y = spritePlayer.bounds.min.y;




		leftCollision = Physics2D.Raycast (rightFeet.position, (Vector2.left), 1f, whatLayer);
		Debug.DrawRay (rightFeet.position, (Vector2.left) * 1f, Color.blue);

		rightCollision = Physics2D.Raycast (leftFeet.position, (Vector2.right), 1f, whatLayer);
		Debug.DrawRay (leftFeet.position, (Vector2.right) * 1f, Color.red);

		upCollision = Physics2D.Raycast (originRay, Vector2.up, 1f, whatLayer);
		Debug.DrawRay (originRay, Vector2.up * 1f, Color.magenta);

		downRightCollision = Physics2D.Raycast (leftFeet.position, Vector2.down, 1f, whatLayer);
		Debug.DrawRay (leftFeet.position, Vector2.down * 1f, Color.red);

		downLeftCollision = Physics2D.Raycast (rightFeet.position, Vector2.down, 1f, whatLayer);
		Debug.DrawRay (rightFeet.position, Vector2.down * 1f, Color.blue);
		//collisionVertical = Physics2D.Raycast (rayVerticalStart, Vector2.down * 2f, whatLayer);
		//Debug.DrawRay (rayHorizontalStart , Vector2.down * 2f, Color.yellow);


	
		/**
		if (spritePlayer.flipX) {
			dirHorizontal = -1;
		} else {
			dirHorizontal = 1;
		}
		**/
		if (leftCollision.collider != null && leftCollision.transform.GetComponent <SpriteRenderer> () != null) {
			lastSpriteRight = rightSprite;
			leftSprite = leftCollision.transform.GetComponent <SpriteRenderer> ();
			distanceLeft = leftCollision.distance;

			if (rightFeet.transform.position.x > leftCollision.transform.position.x ||
				rightFeet.transform.position.y > leftSprite.gameObject.transform.position.y  +  leftSprite.bounds.max.y
				&& leftFeet.transform.position.y > leftSprite.gameObject.transform.position.y +  leftSprite.bounds.max.y) {
				spritePlayer.sortingOrder = leftSprite.sortingOrder + 1;
			} else {

				
			}

		} else {
			distanceRight = 100f;
			//Debug.Log ("ultimo: " + lastSpriteRight.name + " - Actual: " + rightSprite.name);
		}


		if (rightCollision.collider != null && rightCollision.transform.GetComponent <SpriteRenderer> () != null) {
			rightSprite = rightCollision.transform.GetComponent <SpriteRenderer> ();
			distanceRight = rightCollision.distance;
			Debug.Log ("Pie: " + rightFeet.gameObject.transform.position.y +" / alto obstaculo: " + (rightSprite.gameObject.transform.position.y + rightSprite.bounds.max.y));

			if (leftFeet.transform.position.x < rightCollision.transform.position.x ||
				rightFeet.position.y < rightSprite.gameObject.transform.position.y + rightSprite.bounds.max.y
				&& leftFeet.position.y < rightSprite.gameObject.transform.position.y +  rightSprite.bounds.max.y) {
				 
					spritePlayer.sortingOrder = rightSprite.sortingOrder - 1;
				}
		}





	
	}




		/**
		if (downSprite != null && spritePlayer.transform.position.y > downCollider.bounds.max.y) {
			spritePlayer.sortingOrder = downSprite.sortingOrder + 1;
			Debug.Log("Encima de : " + downSprite.name);

		} else {
			if (leftSprite != null) {
				if (rightSprite != null) {
					if (distanceLeft < distanceRight) {
						spritePlayer.sortingOrder = leftSprite.sortingOrder + 1;
					} else {
						spritePlayer.sortingOrder = rightSprite.sortingOrder - 1;
					}
				} else {
					spritePlayer.sortingOrder = leftSprite.sortingOrder + 1;
				}

			}
			}
	}**/
		/**
		if (distanceRight < distanceLeft) {	
			if (rightSprite != null) 
			{
				//if (spritePlayer.transform.position.y >= (rightSprite.transform.position.y + rightSprite.bounds.max.y)) 
				//{
					//spritePlayer.sortingOrder = rightSprite.sortingOrder + 1;
				//} else {
					spritePlayer.sortingOrder = rightSprite.sortingOrder - 1;
				//}
			}//spritePlayer.sortingOrder = rightSprite.sortingOrder - 1;
			
		} 
		else if (
		{

		  if (leftSprite != null || downSprite !=null && spritePlayer.transform.position.y + spritePlayer.bounds.center.y >= downSprite.transform.position.y + downSprite.bounds.max.y) 
			{
				//if (spritePlayer.transform.position.y >= (leftSprite.transform.position.y + leftSprite.bounds.max.y)) 
				//{
					spritePlayer.sortingOrder = leftSprite.sortingOrder + 1;
			} 
				 

				//spritePlayer.sortingOrder = leftSprite.sortingOrder + 1;

		}

		else if (downSprite != null) 
		{
			if (spritePlayer.transform.position.y + spritePlayer.bounds.center.y >= (downSprite.transform.position.y + downSprite.bounds.max.y))
			{

				Debug.Log(downSprite.transform.name);

				spritePlayer.sortingOrder = downSprite.sortingOrder + 1;
			}
		}

		**/
	


}
