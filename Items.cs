using UnityEngine;
using System.Collections;

public class Items : MonoBehaviour {
	public Rigidbody2D thisItem;
	private float DireccionH;
	private float direction;
	private PlayerControl player;

	void Start () {
		player = GameObject.FindObjectOfType<PlayerControl>().GetComponent<PlayerControl>();
		direction = player.getPlayerDirection();


		thisItem = this.GetComponent<Rigidbody2D> ();
		thisItem.angularDrag = 0f;
		thisItem.gravityScale = 0f;
		thisItem.mass = 1f;
		thisItem.AddForce (Vector2.right * 20f * direction, ForceMode2D.Impulse);

		//Destroy (this.gameObject, 1f);
	
	}

	void OnCollisionEnter2D (Collision2D other)
	{
		if (other.transform.name.ToLower ().Contains ("player")) {
			//Destroy (this.gameObject);
		}

		if (other.transform.tag.ToLower().Contains("obstaculos")){
			//Destroy (gameObject,2f);
			Debug.Log("destruyendo");

		}
	}
}
