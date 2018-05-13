using UnityEngine;
using System.Collections;

public class Camara : MonoBehaviour {
/**	
	public GameObject player;

	private Rigidbody2D rigidPlayer;
//	private ControlPlayer01 ctrlPlayer;
	private Vector3 posInicial;
	private float offsetX;
	private bool alejada;
	private float nivelDeZoom;
	private float zoomInicial;

	private float velocidad;
	private float velocidadMaxima;

	void Start () {

		rigidPlayer = player.GetComponent<Rigidbody2D>();
		//ctrlPlayer = player.GetComponent <ControlPlayer01>();


		this.transform.GetComponent <Camera>().orthographic = true;

		zoomInicial = this.transform.GetComponent <Camera>().orthographicSize = 4.5f;
		nivelDeZoom = zoomInicial + 1f;

		alejada = false;


		//velocidadMaxima = ctrlPlayer.velocidadMaxima;
		posInicial= new Vector3 (player.transform.position.x  , player.transform.position.y + 2f, player.transform.position.z - 10f);

		transform.position = posInicial;
		offsetX = transform.position.x - player.transform.position.x;
	}

	void FixedUpdate ()
	{
		
		velocidad = Mathf.Abs(rigidPlayer.velocity.x);

		//if (velocidad >= velocidadMaxima && !alejada) { StartCoroutine (moverCamara (true)); }
		
		//if (velocidad < 1f && velocidad > 0f && alejada) { StartCoroutine (moverCamara (false)); }


	}
	void LateUpdate ()

	{
		if (ctrlPlayer.finalDeNivel != true && ctrlPlayer.muerto != true)
		{
			transform.position = new Vector3 (player.transform.position.x - offsetX, transform.position.y, transform.position.z);
				
			if (player.transform.position.y > this.GetComponent<Camera>().pixelHeight/2)			{
					transform.position = new Vector3 (player.transform.position.x,player.transform.position.y,transform.position.z);
				}

		}else{
			transform.position = new Vector3 (transform.transform.position.x , transform.position.y, transform.position.z);
		}
	}

	public IEnumerator moverCamara(bool alejar)
	{

		if (alejar == true)
		{
			yield return new WaitForSeconds (0.1f);

			for ( float b = zoomInicial; b < nivelDeZoom; b += 0.01f)
			{
				this.transform.GetComponent <Camera>().orthographicSize =  b;


			}


			alejada = true;

		}

		else
		{
			yield return new WaitForSeconds (0.1f);
			for ( float b = nivelDeZoom  ; b > zoomInicial; b -= 0.01f)
			{
				this.transform.GetComponent <Camera>().orthographicSize = b;

			}
			alejada= false;
		}
	}
**/
}