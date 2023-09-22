using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardScript : MonoBehaviour
{
    Rigidbody2D rb2d;
    public GameObject player;
    public Vector2 spawnpoint;
    public int distLimit = 20;
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        spawnpoint = transform.position; //set the spawn point position 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Kill")) //if the object contacts the player and kills it or it gets to the kill zone
        {
            distLimit = 20; //reset distance limit
            rb2d.constraints = RigidbodyConstraints2D.FreezePosition; //reset the velocity 
            transform.position = spawnpoint; //reset its position
            rb2d.constraints = RigidbodyConstraints2D.FreezePositionY;
        }

        else if (collision.CompareTag("Boundary")) //if the object hits its natural boundary
        {
            rb2d.constraints = RigidbodyConstraints2D.FreezePosition; //reset the velocity
            rb2d.constraints = RigidbodyConstraints2D.None; //it falls down
        }
    }

    void FixedUpdate()
    {
        float dist = Vector2.Distance(transform.position, player.transform.position); //take distance between object and player

        if (dist < distLimit && dist > 10) { //if the distance is between 20-10
            rb2d.AddForce(new Vector2(1.5f, 0)); //add 2f force continuously at a fixed rate to cause accelaration
            if (distLimit == 20) { distLimit = 40; } //set the distance detection limit to 40
        } 

        else if (dist < 10 && dist > 5)
        {
             rb2d.AddForce(new Vector2(1f, 0));
        }
    }
}
