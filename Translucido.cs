using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Translucido : MonoBehaviour {

	public List <SpriteRenderer> spriteTranslucentList = new List<SpriteRenderer>();
	public List <Color> actualColor = new List<Color>();

	[Range(0f,1f)]
	[Tooltip ("Cantidad de transparencia aplicada al SpriteRender")]
	public float amountOfTrans = 0.5f;

	[Tooltip ("Tag que debe tener el SpriteRender para ser translucido")]
	public string tagForTranslucent = "translucent";


	private void OnTriggerEnter2D (Collider2D other)
	{
        // Si el Collider2d (other) tiene el componente PlayerControl (!=null)
        if (other.GetComponent<PlayerControl>() != null)
        {

            //Se iguala la variable hidden a false (ya no está oculto).
            other.GetComponent<PlayerControl>().setHidden(false);

            GetSpritesWithTranslucent(other);
        }


	}

	private void OnTriggerExit2D (Collider2D other)
	{
		// Si el Collider2d (other) tiene el componente PlayerControl (!=null)
		if (other.GetComponent<PlayerControl>() != null){

			//Se iguala la variable hidden a false (ya no está oculto).
			other.GetComponent<PlayerControl>().setHidden(false);

            RestoreTranslucency();
		}
       
    
    }

	void OnTriggerStay2D (Collider2D other)
	{
            //GetSpritesWithTranslucent(other);
			ChangeTranslucency();
	}


	void GetSpritesWithTranslucent (Collider2D other)
	{
		//Si el collider2d (other) tiene el componente PlayerControl..
		if (other.GetComponent<PlayerControl> () != null) 
        {
			//Usamos el método setHidden para poner al jugador en modo oculto.
			other.GetComponent<PlayerControl> ().setHidden (true);

			// Comprobamos que los hijos de este transform tienen el componente SpriteRender (!=null)
			// Comprobamos que los List que van a almacenar sprites y colores están vacíos.
			if (this.transform.GetComponentsInChildren<SpriteRenderer> () != null 
                && spriteTranslucentList.Count == 0 
                && actualColor.Count == 0) 
               
            {

				//Recorre el componente SpriteRender de los hijos de este transform y lo almacena en "actualSprite" (1 en cada bucle)
				foreach (SpriteRenderer actualSprite in this.transform.GetComponentsInChildren <SpriteRenderer>())
 				{
					//Si el tag (convertido en minusculas), de el actual SpriteRender guardado en actualSprite , cumple la igualdad.
					if (actualSprite.tag.ToLower ().Contains (tagForTranslucent)) 
                    {
						//Lo añade a la Lista de SpriteRender spriteTranslucentList
						spriteTranslucentList.Add (actualSprite);
						actualColor.Add (actualSprite.color);
					} 
				}
			}

            ChangeTranslucency();
		} 
       
    }

	void ChangeTranslucency ()
	{
		// Recorre la lista de los sprites almacenados y les cambia la transparencia.
		for (int a = 0; a < spriteTranslucentList.Count; a++) 
        {
			spriteTranslucentList [a].color = new Color (1f, 1f, 1f, amountOfTrans);
   		}
	}

	void RestoreTranslucency ()
	{
		for (int a= 0 ; a < spriteTranslucentList.Count ; a++) 
			{
				spriteTranslucentList [a].color = actualColor[a];
			}

			spriteTranslucentList.Clear();
			actualColor.Clear();
	}
}
