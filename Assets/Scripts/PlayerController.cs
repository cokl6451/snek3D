using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 10;
    private Vector3 moveDir;
    public int playerScore;
    public bool playerIsAlive = true;

    private PlayerTail playerTail;
    public Text scoreText;

    // Reference the FoodSpawner script
    private FoodSpawner foodSpawner;

    // Create an event for game over
    public static event Action OnGameOver;

    // Add a minimum distance for tail collision
    public float minTailCollisionDistance = 0.8f;

    void Start()
    {
        playerTail = GetComponent<PlayerTail>();

        // Find the FoodSpawner script in the scene
        foodSpawner = FindObjectOfType<FoodSpawner>();
        if (foodSpawner == null)
        {
            Debug.LogError("FoodSpawner not found in the scene.");
        }
    }

    void Update()
    {
            moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
            
    }

    void FixedUpdate()
    {
        GetComponent<Rigidbody>().MovePosition(GetComponent<Rigidbody>().position + transform.TransformDirection(moveDir) * moveSpeed * Time.deltaTime);
    }

    // Detect collisions with food objects and tail
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Food"))
        {
            Destroy(other.gameObject);
            //Add 25 tail prefabs since the world is pretty big
            for(int i = 0; i < 25; i++){
                playerTail.GrowTail();
            }

            //Replace the food
            if (foodSpawner != null)
            {
                foodSpawner.SpawnFood();
            }

            //Increase the score
            //Hardcoded to one at the moment but can be changed later if we have multiple types of food
            addScore(1);
        }
        else if (other.CompareTag("Tail"))
        {
            //Add a grace period so the head isn't constantly detecting the first tail object since it's embedded in the head's sphere collider
            if ((other.transform.position - transform.position).magnitude > minTailCollisionDistance)
            {
                if (OnGameOver != null)
                {
                    OnGameOver();

                    // Snek can no longer move
                    // playerIsAlive is not doing anything right now
                    // maybe there is better way to of  stopping the game instead of setting player moveSpeed to 0
                    playerIsAlive = false;
                    moveSpeed = 0;
                }
            }
        }
    }

    public void addScore(int score)
    {
        playerScore += score;
        scoreText.text = playerScore.ToString();
    }
}
