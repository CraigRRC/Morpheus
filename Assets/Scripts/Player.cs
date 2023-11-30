using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public Magnet[] magnetsInLvl;
    private AudioSource audioSource;
    private PlayerMovement movementScript;
    public Sprite deathSprite;
    private Animator playerAnimator;
    private PlayerState playerState = PlayerState.Alive;
    public int currentPolaritySwitches;
    public BoxCollider2D playerDeathBox;
    public float lethalImpactForce = 11f;
    //public delegate void SwitchPolarity();
    //public event SwitchPolarity OnSwitchPolarity;

    private void Awake()
    {
        movementScript = GetComponent<PlayerMovement>();
        playerAnimator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (magnetsInLvl != null)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                currentPolaritySwitches++;
                //OnSwitchPolarity?.Invoke();
                if(UIData.Instance != null)
                    UIData.Instance.ReduceBattery();
                foreach (var magnet in magnetsInLvl)
                {
                    if(magnet != null)
                        magnet.FlipPolarity();
                }
            }
        }

        if(playerState == PlayerState.Dead)
        {
            magnetsInLvl = null;
            Destroy(gameObject, 3f);
            //Added for Level 433, to soft reset level each time
            //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Hazard
        if(collision.gameObject.layer == 10)
        {
            //die
            if(deathSprite != null)
            {
                PlayerDead();
            }

        }

        if (collision.relativeVelocity.magnitude > lethalImpactForce)
        {
            PlayerDead();
        }

        if (collision.gameObject.layer == 8)
        {
            //Super hacked together probably... But, it works!
            //When messing around with the contact I found that the magnitude was consistantly 2 when on either side of the box.
            if (Vector2.Distance(collision.GetContact(0).normal, transform.right) == 2 ||
                Vector2.Distance(collision.GetContact(0).normal, transform.right * -1) == 2)
            {
                playerAnimator.SetBool("IsPushing", true);
            }
            else
            {
                //Prevents buggy behaviour.
                playerAnimator.SetBool("IsPushing", false);
            }

        }
       
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.layer == 8)
        {
            //Returns the player back to walk/jump.
            playerAnimator.SetBool("IsPushing", false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Magnet 
        if (collision.gameObject.layer == 12)
        {
            playerAnimator.SetBool("InMagnet", true);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //Magnet 
        if (collision.gameObject.layer == 12)
        {
            playerAnimator.SetBool("InMagnet", true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //Magnet 
        if (collision.gameObject.layer == 12)
        {
            playerAnimator.SetBool("InMagnet", false);
        }
    }

    private void PlayerDead()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
        playerAnimator.SetBool("isDead", true);
        playerState = PlayerState.Dead;
        GetComponent<BoxCollider2D>().enabled = false;
        playerDeathBox.enabled = true;
        movementScript.enabled = false;
    }

    public PlayerState GetPlayerState() { return playerState; }
    public void CallPlayerDead() { PlayerDead(); }
    public void SetMaxPolaritySwitches(int current) { currentPolaritySwitches = current; }
    public int GetMaxPolaritySwitches() {  return currentPolaritySwitches; }

}


public enum PlayerState
{
    Alive,
    Dead,
}
