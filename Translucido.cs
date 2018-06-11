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
        //Llama al método que crea una lista de SpriteRenders que contienen el tag indicado, en éste objeto.
        GetSpritesWithTranslucent();
        //Llama al método para coger el mayor SortingOrder de las lista de sprites translúcidos.
        //GetHigherOrderInLayer(spriteTranslucentList);

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
       
    }

	//Método cuando el collider está dentro de este objeto.
    void OnTriggerStay2D (Collider2D other)
	{
        //Si el collider tiene el componente PlayerControl y éste objeto no está translucido.
        //Llama al método para que cambie la transparencia de éste objeto.
        if (other.GetComponentInParent<PlayerControl>() != null && !isTranslucent)
        {
                ChangeTranslucency();
        }	
	}

   
    //Método que cambia la transparencia de los SpriteRenders de éste objeto.
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
        }
    }

    //Método que Recorre la lista de SpriteRenders de éste objeto con el tag translucent 
    //e iguala la variable higherOrderTranslucent con el sortinOrder más alto encontrado en la lista.
    //public int GetHigherOrderInLayer (List <SpriteRenderer> translucentSprites)    
    public int HigherOrderInLayer
    {
        get
        {
            higherOrderTranslucent = 0;
            GetSpritesWithTranslucent();

            foreach (SpriteRenderer actualSprite in spriteTranslucentList)
            {
                higherOrderTranslucent = higherOrderTranslucent < actualSprite.sortingOrder ? actualSprite.sortingOrder : higherOrderTranslucent;
            }
            return higherOrderTranslucent;
        }
    }
	

    // Método que Recorre la lista de los SpriteRenders de éste objeto almacenados, y les cambia la transparencia.
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

    //Método que recorre la lista del tipo DatosSprite.
    //Sino existe. Lo añade a dicha lista 
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

    public bool IsTranslucent
    {
        get { return isTranslucent; }
    }

    public string TagTranslucent

    {
        get { return tagForTranslucent; }
    }
}


//Clase para instanciar un <tipo>, basado en la lista de Sprites que está dentro del objeto translúcido (TranslucentList).

public  class DatosSprite
{
    public String nombre;
    public int orden;
    public Color color;
}
