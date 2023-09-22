using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public float speed = 3.0f;

    public ParticleSystem particle;
    public GameObject player;

    public int special; //what power will it give the player?

    Rigidbody2D rb2d;
    SpriteRenderer sprt;
    public Vector3 forward;
    public float detectDist; //the distance the enemy starts chasing
    public int roamingType; //0 - Default, 1 - Chase

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        sprt = gameObject.GetComponent<SpriteRenderer>();
        forward = transform.right;
        detectDist = 5f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Translate(speed * Time.deltaTime * -forward, Space.World); //move towards the direction enemy faces
        
        if(roamingType == 1 && Vector2.Distance(new Vector2 (transform.position.x, 0), new Vector2 (player.transform.position.x, 0)) < detectDist) {
            //if roaming type is Chase and distance between enemy and player is lower than the detect distance
            if ((transform.position.x - player.transform.position.x < -2 && forward.x > 0) || (transform.position.x - player.transform.position.x > 2 && forward.x < 0))
            {
                //depending on the difference between the x positions and the direction the enemy currently faces, if the enemy isn't facing the player,
                forward = -forward; //turn around
            } 
        }
    }

    void death()
    {
        particle.transform.position = transform.position; //set particle position
        particle.Play();
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Sync"))
        {
            death();
        }

        if (collision.collider.CompareTag("Enemy")) 
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collision.collider);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Kill")) //if the enemy touches kill zone, it dies
        {
            death();
        }

        else if (collision.CompareTag("Turn")) //if it hits a Turn trigger
        {
            forward = -forward;
        }
    }
}
