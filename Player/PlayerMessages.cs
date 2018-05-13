using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerMessages : MonoBehaviour {
    
    public float duracion;
    public float velocidad;
    public int cantidadCambiodeEnergia;
	public string tipoIncremento;

	private Camera myCamera;
    public GUIStyle estiloDeTexto;
    
    private Transform myTransform;
	private Image Message;

	// Use this for initialization
	void Start () {
		myTransform = GameObject.Find ("Messages").GetComponent <Transform>();

		myCamera = GameObject.Find("PlayerV2").GetComponentInChildren <Camera>();

		if (GameObject.Find ("Canvas") != null)
		{
			GameObject canvas = GameObject.Find ("Canvas");
			Message = canvas.transform.Find ("ButtonE").GetComponent <Image>();
		}

		//Message.enabled = false;

		Destroy(gameObject, duracion);
	}
	
	// Update is called once per frame
	void Update () {
	   
	}

	void FixedUpdate () {
		ActualizarMovimiento();
	}

    private void OnGUI() {
        Rect textRect = CalcularRectanguloMensaje();
		//Message.enabled = true;
		Message.enabled = true;
		string mensaje = ObjenerMensaje();
        CambiarColorDeEstilo();

		GUI.TextField (textRect, mensaje, estiloDeTexto);
    }

    private void ActualizarMovimiento() {
        Vector3 paso = new Vector3(0,1,0) * Time.deltaTime * velocidad;
        myTransform.Translate(paso);

		Message.transform.position = myTransform.position;
    }

    private Rect CalcularRectanguloMensaje() {
		Vector2 position = myCamera.WorldToScreenPoint (myTransform.position);
        Rect rectanguloMensaje = new Rect(position.x - 50, Screen.height - position.y, 100, 30);
        return rectanguloMensaje;
    }

    private string ObjenerMensaje() {
		string mensaje;
        if (cantidadCambiodeEnergia > 0) {
			mensaje = tipoIncremento + " + " + cantidadCambiodeEnergia;
		} else {
			mensaje = tipoIncremento + " " + cantidadCambiodeEnergia;
		}
        	return mensaje;
    }

    private void CambiarColorDeEstilo() {
        if (cantidadCambiodeEnergia < 0){ estiloDeTexto.normal.textColor = Color.red;}
		else if (tipoIncremento == "Energía" || tipoIncremento == "Energy"){ estiloDeTexto.normal.textColor = Color.green;}
		if (tipoIncremento == "Velocidad" || tipoIncremento == "Speed") {   estiloDeTexto.normal.textColor = Color.blue;}
		if (tipoIncremento == "Casco" || tipoIncremento == "Helmet") 
		{ 
			estiloDeTexto.normal.textColor = Color.yellow;
			estiloDeTexto.fontSize = 10;
		} 
		if (tipoIncremento == "¡¡COMBO!!"){
			estiloDeTexto.normal.textColor = Color.magenta;
			estiloDeTexto.fontSize = 20;
		}



    }
}
