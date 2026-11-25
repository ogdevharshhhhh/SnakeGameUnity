
using System.Collections.Generic;
using UnityEngine;

public class SnakeController : MonoBehaviour
{
    public float moveSpeed = 0.001f;
    public GameObject food;
    public GameObject bodyPrefab;

    public float gridSize = 0.6f;
    private Vector2 direction = Vector2.right;

    private List<Transform> bodyParts =
        new List<Transform>();

    private float timer;
    private bool gameOver = false;

    void Update()
    {
        if (gameOver)
            return;

        // Controls
        if (Input.GetKeyDown(KeyCode.UpArrow) ||
            Input.GetKeyDown(KeyCode.W))
        {
            if (direction != Vector2.down)
                direction = Vector2.up;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) ||
            Input.GetKeyDown(KeyCode.S))
        {
            if (direction != Vector2.up)
                direction = Vector2.down;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) ||
            Input.GetKeyDown(KeyCode.A))
        {
            if (direction != Vector2.right)
                direction = Vector2.left;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) ||
            Input.GetKeyDown(KeyCode.D))
        {
            if (direction != Vector2.left)
                direction = Vector2.right;
        }

        // Movement timer
        timer += Time.deltaTime;

        if (timer >= moveSpeed)
        {
            timer = 0;
            MoveSnake();
        }
    }
    Vector2 GetLastPosition()
    {
        if (bodyParts.Count == 0)
            return transform.position - (Vector3)direction;

        return bodyParts[bodyParts.Count - 1].position;
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == food)
        {
            GrowSnake(GetLastPosition());
            SpawnFood();

            Debug.Log("Food Eaten!");
        }
    }


    void MoveSnake()
    {
        // Store old head position
        Vector2 previousPosition = transform.position;

        // Store the position behind the tail
        Vector2 tailPosition = previousPosition;

        if (bodyParts.Count > 0)
        {
            tailPosition = bodyParts[bodyParts.Count - 1].position;
        }

        // Move body segments
        for (int i = 0; i < bodyParts.Count; i++)
        {
            Vector2 currentPosition = bodyParts[i].position;

            bodyParts[i].position = previousPosition;

            previousPosition = currentPosition;
        }

        // Move head
        transform.position += (Vector3)(direction * gridSize);

        // Wall collision
        if (Mathf.Abs(transform.position.x) > 8 ||
            Mathf.Abs(transform.position.y) > 4)
        {
            GameOver();
            return;
        }

        // Body collision
        foreach (Transform part in bodyParts)
        {
            if ((Vector2)transform.position == (Vector2)part.position)
            {
                GameOver();
                return;
            }
        }

        // Food collision
        if (Vector2.Distance(transform.position, food.transform.position) < 0.01f)
        {
            GrowSnake(tailPosition);
            SpawnFood();

            Debug.Log("Food Eaten!");
        }
    }


    void GrowSnake(Vector2 position)
    {
        GameObject newPart = Instantiate(
            bodyPrefab,
            position,
            Quaternion.identity
        );

        bodyParts.Add(newPart.transform);
    }

    void SpawnFood()
    {
        int x = Random.Range(-10, 11);
        int y = Random.Range(-5, 6);

        food.transform.position = new Vector2(
            x * gridSize,
            y * gridSize
        );
    }

  /*  bool IsOccupied(Vector2 position)
    {
        if ((Vector2)transform.position == position)
            return true;

        foreach (Transform part in bodyParts)
        {
            if ((Vector2)part.position == position)
                return true;
        }

        return false;
    }*/

    void GameOver()
    {
        gameOver = true;

        Debug.Log("GAME OVER!");
    }
}
