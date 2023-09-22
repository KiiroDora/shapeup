using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5.0f; //default stats for speed and jump power
    public float jumpPower = 8.0f;

    bool isgr; //is Grounded boolean
    public int special = 0; //special ability 0 -> none, 1 -> dash, 2-> high jump, 3-> shoot, 4-> glow

    public Transform spawn; //spawn position
    public ParticleSystem particle; //set particle system

    public bool specialbool = true; //bools for checking special move and jump timer
    public bool jumpbool = true;
    public bool djump; //bool for checking double jump to only allow one extra jump

    Rigidbody2D rb2d;
    SpriteRenderer sprt;
    Collider2D coll;

    Rigidbody2D squarebody;
    Sprite squaresprt;
    Collider2D squarecoll;
    Vector3 squaresize;
    Color squarecolor;


    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        sprt = GetComponent<SpriteRenderer>();
        coll = GetComponent<Collider2D>();

        squarebody = rb2d; //deposit default information to these variables
        squarecoll = coll;
        squaresprt = sprt.sprite;
        squarecolor = sprt.color;
        squaresize = transform.localScale;
    }


    //IEnumerators
    public IEnumerator JTimer(float time)
    {
        jumpbool = false;
        yield return new WaitForSecondsRealtime(time);
        jumpbool = true;
    }

    public IEnumerator STimer(float time)
    {
        specialbool = false;
        yield return new WaitForSecondsRealtime(time);
        specialbool = true;
    }

    public IEnumerator GTimer(float time) //capsule special, changes player state to spike bounce then back to normal after a given time 
    {
        tag = "Guard";
        sprt.color = Color.blue;
        yield return new WaitForSecondsRealtime(time);
        tag = "Player";
        sprt.color = squarecolor;
    }


    //Collisions
    void OnCollisionEnter2D(Collision2D collision) 
    {
        if (collision.collider.CompareTag("Ground")) //if grounded, set bool to true
        {
            isgr = true;
        }

        if (collision.collider.CompareTag("Enemy") && !gameObject.CompareTag("Sync") || 
           (collision.collider.CompareTag("Spike") && !CompareTag("Guard"))) //if it hits enemy + not on sync mode OR if it hits spike + not on guard mode 
        {
            Death();
        }

        if (collision.collider.CompareTag("Spike") && CompareTag("Guard")) //if hits spike + on guard mode
        {
            rb2d.AddForce(8f * Vector2.up, ForceMode2D.Impulse);
        }

        if (collision.collider.CompareTag("Enemy") && gameObject.CompareTag("Sync")) //if it hits enemy + on sync mode 
        {
            sprt.sprite = collision.gameObject.GetComponent<SpriteRenderer>().sprite; //take sprite, scale and special move data from enemy
            transform.localScale = collision.transform.localScale;
            special = collision.gameObject.GetComponent<EnemyScript>().special;

            switch (special) //change stats to new form
            {
                case 1: //circle
                    speed = 8.0f;
                    jumpPower = 6.0f;

                    coll.enabled = false;
                    GetComponent<CircleCollider2D>().enabled = true;
                    GetComponent<PolygonCollider2D>().enabled = false;
                    GetComponent<CapsuleCollider2D>().enabled = false;

                    break;
                case 2: //triangle
                    speed = 3.0f;
                    jumpPower = 10.0f;

                    coll.enabled = false;
                    GetComponent<CircleCollider2D>().enabled = false;
                    GetComponent<PolygonCollider2D>().enabled = true;
                    GetComponent<CapsuleCollider2D>().enabled = false;

                    break;

                case 3: //capsule
                    speed = 3.0f;
                    jumpPower = 5.0f;

                    rb2d.SetRotation(90);

                    coll.enabled = false;
                    GetComponent<CircleCollider2D>().enabled = false;
                    GetComponent<PolygonCollider2D>().enabled = false;
                    GetComponent<CapsuleCollider2D>().enabled = true;

                    break;

                //case 4: (star? pentagon?)
                    //break;

            }
        }
    }

    void OnCollisionExit2D(Collision2D collision) //the moment the collision with ground is over, set ground bool to false and allow double jumps again
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isgr = false;
            djump = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Kill")) //if it hits killzone or spike (when not in guard mode)
        {
            Death();
        }

        else if (collision.CompareTag("Goal"))
        {
            GameController.ScreenActive(1,true); //make the win screen visible
            Time.timeScale = 0;
        }

        else if (collision.CompareTag("Checkpoint")) 
        {
            spawn.position = collision.transform.position; //set respawn position to the checkpoint's position
            collision.gameObject.SetActive(false); //set the checkpoint to inactive
        }

        else if (collision.CompareTag("Coin"))
        {
            GameController.coins += 10; //increase coin score
            collision.gameObject.SetActive(false); //set the coin to be inactive
        }

    }


    //Other methods
    public void Jump() 
    {
        if (jumpbool) //checks if timer is done via bool
        {
            rb2d.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse); //jump by applied vertical force
            StartCoroutine(JTimer(0.3f)); //start timer again
        }
    }

    public void Specials() 
    {
        if (specialbool) //checks if timer is done via bool
        {
            switch (special)
            {
                case 1: //dash
                    rb2d.AddForce(new Vector2(Input.GetAxis("Horizontal") * 5, 0), ForceMode2D.Impulse); //add vertical force in direction of movement
                    StartCoroutine(STimer(0.5f)); //start timer
                    break;

                case 2: //hi-jump (airborne only)
                    if (!isgr && djump)
                    {
                        rb2d.constraints = RigidbodyConstraints2D.FreezePosition;
                        rb2d.constraints = RigidbodyConstraints2D.None;
                        rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
                        rb2d.AddForce(0.8f * jumpPower * Vector2.up, ForceMode2D.Impulse); //do a second, weaker jump than usual
                        djump = false; //set double jump bool to false
                        StartCoroutine(STimer(1f)); //start timer
                    }
                    break;

                case 3: //bounce on spikes instead of dying
                    StartCoroutine(GTimer(0.5f));
                    StartCoroutine(STimer(0.5f));
                    break;

                //case 4: glow?
                    //break;
            }          
        }
    }

    void Resetsquare()
    {
        rb2d = squarebody; //reset features
        coll = squarecoll;
        sprt.sprite = squaresprt;
        transform.localScale = squaresize;

        rb2d.constraints = RigidbodyConstraints2D.None;
        rb2d.constraints = RigidbodyConstraints2D.FreezeRotation; //remove all constraints and add back the rotation lock
        coll.enabled = true;
        GetComponent<CircleCollider2D>().enabled = false;
        GetComponent<PolygonCollider2D>().enabled = false;
        GetComponent<CapsuleCollider2D>().enabled = false;

        speed = 5.0f; //reset stats
        jumpPower = 8.0f;

        tag = "Player"; //reset tag
        special = 0; //reset special
    }
    public void Death() 
    {
        GameController.coins *= 0.75f; //coins get reduced by three fourths
        particle.transform.position = transform.position; //set particle position to death position
        sprt.enabled = false; //set the sprite to be invisible
        rb2d.constraints = RigidbodyConstraints2D.FreezePosition; //freeze the player's movement
        coll.enabled = false; //disable the box collider
        particle.Play();
        Invoke(nameof(Respawn), particle.main.duration); //reverse these conditions, then teleport player to spawn point after particle animation ends
    }
    public void Respawn()
    {
        Resetsquare(); //reset features to default
        transform.position = spawn.position; //"respawn" aka teleport to spawnpoint
        sprt.enabled = true; //sprite is visible again
        particle.Stop();

        GameController.EnemyRespawn(); //set all enemies to active to prevent getting stuck
    }


    //Updates

    void Update()
    {
        
        //CONTROLS//

        RaycastHit2D ray = Physics2D.BoxCast(transform.position, transform.localScale, 0, Vector2.down, 2f); //cast a box shaped bundle of rays to detect ground

        if (Input.GetKeyDown(KeyCode.Space) && isgr && ray.collider != null && ray.collider.CompareTag("Ground")) //when Space is pressed and player is DEFINITELY on ground
        {
            Jump(); //jump method that adds delay via coroutine
        }

        if (Input.GetKeyDown(KeyCode.Q)) //when Q is pressed
        {
            Specials(); //special move method that adds delay via coroutine
        }

        if (Input.GetKey(KeyCode.E) && GameController.coins != 0) //hold E to swap to Sync mode...
        {

            sprt.color = Color.green; //placeholder color change!
            tag = "Sync"; 
        }

        if (Input.GetKeyUp(KeyCode.E) || GameController.coins <= 0 ) //...return to normal form when button is released
        {
            sprt.color = squarecolor;
            tag = "Player";
        }

        if (Input.GetKeyDown(KeyCode.X) && special != 0)  //if X is pressed while there is a special ability
        {
            particle.transform.position = transform.position; //set particle system position
            Resetsquare(); //reset back to square
            particle.Play();
        }
    }

    private void FixedUpdate()
    {
        float translationX = Input.GetAxis("Horizontal") * Time.fixedDeltaTime * speed; //horizontal movement parameter with input

        if (rb2d.constraints != RigidbodyConstraints2D.FreezePosition)
        {
            transform.Translate(translationX, 0, 0, Space.World); //move the player in respect to the world axis, otherwise it will go crazy when rotated
        }

        if (CompareTag("Sync")) //coins get depleted in Sync mode
        {
            GameController.coins--;
        }
    }
}
