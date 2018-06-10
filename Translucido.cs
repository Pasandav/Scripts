using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Translucido : MonoBehaviour {
    
	private List <SpriteRenderer> spriteTranslucentList = new List<SpriteRenderer>();
    private List <Color> spriteColorList = new List<Color>();
    private List <DatosSprite> ListaDeSprites = new List<DatosSprite>();

 
    private int higherOrderTranslucent = 0;
    private string lastCollisionName = "";

    public bool isTranslucent = false;

    [Range(0f,1f)]
	[Tooltip ("Cantidad de transparencia aplicada al SpriteRender")]
    [SerializeField]
    private float amountOfTrans = 0.5f;

	[Tooltip ("Tag que debe tener este GameObject para ser translucido")]
    [SerializeField]
    private string tagForTranslucent = "translucent";

    //Método cuando el collider entra en éste objeto.
    private void OnTriggerEnter2D (Collider2D other)
	{
        //Llama al método que crea una lista de SpriteRenders de éste objeto.
        GetSpritesWithTranslucent();

        // Si el Collider2d recibido (other), tiene el componente PlayerControl (!=null)
        if (other.GetComponentInParent<PlayerControl>() != null)
        {
            //Se iguala la variable hidden a false (ya no está oculto).
            other.GetComponentInParent<PlayerControl>().IsHidden = true;
            //Se llama al método para cambiar la trasparencia de este gameobject
            //Se iguala la variable isTranslucent a true (está translúcido).
            ChangeTranslucency();
            isTranslucent = true;
        }
        else if (other.transform.parent.name != lastCollisionName && !isTranslucent)
        {
            //Se añade a la lista de sprites ques están dentro de éste objeto.
            AddSpriteInnerList (other.GetComponent<SpriteRenderer>());

            //Se iguala la variable con el nombre del último objeto que ha entrado para que no colisione otra vez el mismo
            lastCollisionName = other.transform.parent.name;

            //LLama al metodo que cambia el color y sortingOrder del spriteRender que envía.
            ChangeToShadowSprite(other.GetComponent<SpriteRenderer>());
        }
	}

	// Método cuando el collider sale de éste objeto.
    private void OnTriggerExit2D (Collider2D other)
	{
		// Si el Collider2d (other) tiene el componente PlayerControl (!=null)
        if (other.GetComponentInParent<PlayerControl>() != null){

			//Se iguala la variable hidden a false (ya no está oculto).
            other.GetComponentInParent<PlayerControl>().IsHidden = false;

            //LLama al método que restaura la transparencia de este objeto.
            RestoreTranslucency();
		}
        else
        {
            //LLama al método que restaura el color y sortingOrder del spriteRender que acaba de salir.
            RestoreShadowSprite (other.GetComponent<SpriteRenderer>());
        }
    }

	//Método cuando el collider está dentro de este objeto.
    void OnTriggerStay2D (Collider2D other)
	{
        if (other.GetComponentInParent<PlayerControl>() != null)
        {
            if (!isTranslucent)
            {
                ChangeTranslucency();  
            }
        }
        else
        {
            if (isTranslucent)
            {
                RestoreShadowSprite(other.GetComponent<SpriteRenderer>());
            }
            else
            {
                GetSpritesWithTranslucent();
                ChangeToShadowSprite(other.GetComponent<SpriteRenderer>());
            }
        }	
	}

   

	void GetSpritesWithTranslucent ()
	{
        // Comprobamos que los hijos de este transform tienen el componente SpriteRender (!=null)
		// Comprobamos que los List que van a almacenar sprites y colores están vacíos.
		if (this.transform.GetComponentsInChildren<SpriteRenderer> () != null 
            && spriteTranslucentList.Count == 0 
            && spriteColorList.Count == 0)
        {
			//Recorre los componentes SpriteRender de los hijos de este transform y los almacena en "actualSprite" (1 en cada bucle)
			foreach (SpriteRenderer actualSprite in this.transform.GetComponentsInChildren <SpriteRenderer>())
 			{
                //Si el tag (convertido en minusculas), de el actual SpriteRender guardado en actualSprite , cumple la igualdad.
                if (actualSprite.tag.ToLower().Contains(tagForTranslucent))
                {
                    //Lo añade a la Lista de SpriteRender spriteTranslucentList
                    spriteTranslucentList.Add(actualSprite);
                    spriteColorList.Add(actualSprite.color);
                }
			}
            //Llama al método para coger el mayor SortingOrder de las lista de sprites translúcidos.
            GetHigherOrderInLayer (spriteTranslucentList);	
        }
    }

    //Recorre la lista de sprites con el tag translucent e iguala la variable higherOrderTranslucent con el sortinOrder más alto.
    private void GetHigherOrderInLayer (List <SpriteRenderer> translucentSprites)    
    {
        higherOrderTranslucent = 0;

        foreach (SpriteRenderer actualSprite in translucentSprites)
        {
            higherOrderTranslucent = higherOrderTranslucent < actualSprite.sortingOrder ? actualSprite.sortingOrder  : higherOrderTranslucent;
        }
    }
	

    // Recorre la lista de los sprites almacenados y les cambia la transparencia.
    private void ChangeTranslucency ()
	{
        for (int a = 0; a < spriteTranslucentList.Count; a++) 
        {	
            spriteTranslucentList [a].color = new Color (1f, 1f, 1f, amountOfTrans);
   		}
	}

    // Recorre la lista de los sprites almacenados y les restaura su color original.
    // Borra las lista de sprites y de colores y pone la varibe isTranslucent a false.
    void RestoreTranslucency ()
	{
		for (int a= 0 ; a < spriteTranslucentList.Count ; a++) 
			{
				spriteTranslucentList [a].color = spriteColorList[a];
			}

		spriteTranslucentList.Clear();
		spriteColorList.Clear();
        higherOrderTranslucent = 0;
        isTranslucent = false;
	}

    void AddSpriteInnerList(SpriteRenderer otro)
    {
        bool existe = false;
        foreach (DatosSprite actual in ListaDeSprites)
        {
            existe = actual.nombre != otro.transform.parent.name ? false : true;
        }

        if (!existe)
        {
            DatosSprite tmpSprite = new DatosSprite();
            tmpSprite.nombre = otro.transform.parent.name;
            tmpSprite.orden = otro.sortingOrder;
            tmpSprite.color = otro.color;

            ListaDeSprites.Add(tmpSprite);
        }
    }

    // Borra de la lista los datos (nombre,orden y color), del sprite que estaba dentro del objeto translucido.
    void DeleteSpriteInnerList(SpriteRenderer innerSprite)
    {
        for (int a = 0; a < ListaDeSprites.Count; ++a)
        {
            if (ListaDeSprites[a].nombre == innerSprite.transform.parent.name)
            {
                ListaDeSprites.RemoveAt(a);
                break;
            }
        }
    }

    // Recibe un SpriteRender y recorre la lista de sprites que están dentro del objeto translucido.
    // Si coinciden en el nombre: Cambia el color y el sortingOrder de SpriteRender recibido por el de la ListaDeSprites
    void RestoreShadowSprite (SpriteRenderer other)
    {
        // Recorre la lista con los datos del sprite que está dentro
        for (int a = 0; a < ListaDeSprites.Count; ++a)
        {
            if (ListaDeSprites[a].nombre == other.transform.parent.name)
            {
                other.color = ListaDeSprites[a].color;
                other.sortingOrder = ListaDeSprites[a].orden;
                break;
            }
        }
    }

    //Método que recibe un SpriteRender y le cambia: El sortinOrder más alto de la lista SpriteTranslucentList
    // y el color.
    void ChangeToShadowSprite(SpriteRenderer other)
    {
        other.sortingOrder = higherOrderTranslucent + 1;
        other.color = new Color(0f, 0f, 0f, 0.2f);
    }
}

//Clase para instanciar un <tipo>, basado en la lista de Sprites que está dentro del objeto translúcido (TranslucentList).

public  class DatosSprite
{
    public String nombre;
    public int orden;
    public Color color;
}
