using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
//Potentally a parent class for all interactable objects in the story maps.
//Base functionality:
//1) On trigger enter
//2) F sprite appears above player.
//3) If player is standing in the trigger and presses F. Do something.
//Do something will be different for each interactable, therefore might be a good idea to make child classes.
public class Interactable : MonoBehaviour
{

    private BoxCollider2D trigger;
    private GameObject fKeySprite;

    protected virtual void Awake()
    {
        trigger = GetComponent<BoxCollider2D>();
        trigger.isTrigger = true;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        //Player
        if (collision.gameObject.layer == 11)
        {
            if(fKeySprite == null)
            {
                //Display F key.
                var Temp = collision.GetComponentsInChildren<GameObject>(true);
                foreach (var spriteRenderer in Temp)
                {
                    if (!spriteRenderer.activeSelf)
                    {
                        fKeySprite = spriteRenderer;
                    }
                }
            }
            else
            {
                fKeySprite.SetActive(true);
            }
            
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        //Player
        if (collision.gameObject.layer == 11)
        {
            fKeySprite.SetActive(false);
        }
    }
}