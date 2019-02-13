using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    Up,
    Down,
    Left,
    Right
}

public class SnakeGame : MonoBehaviour
{
    private BoardManager boardManager;

    private bool boost = false;

    private Direction heading = Direction.Up;
    private Direction previousHeading = Direction.Down;

    public GameObject StartText;

    public float moveTime = 0.2f;

    private float timer = 1.0f;

    bool startGame = false;
    bool dead = true;
   

    public void Start()
    {
        boardManager = GameObject.Find("BoardManager").GetComponent<BoardManager>();
    }

    public void Update()
    {
        if (startGame)
        {
            boardManager.InitBoard(12, 8);

            boardManager.NewGame();
            startGame = false;
            dead = false;
            StartText.SetActive(false);
        }
        else
        {
            if (!dead)
            {
                HandleInput();

                if (boost)
                {
                    timer -= Time.deltaTime * 2;
                }
                else
                {
                    timer -= Time.deltaTime;
                }

                if (timer < 0)
                {
                    previousHeading = heading;
                    dead = !boardManager.Move(heading);

                    if (dead)
                    {
                        // We died just now, show message
                        StartText.SetActive(true);

                    }


                    timer += moveTime;
                }
            }
            else
            {
                // Restart game
                if (dead && !startGame && Input.GetKeyUp(KeyCode.Space))
                {
                    startGame = true;
                }
            }
        }
    }

    private void HandleInput()
    {
        // Todo queue up inputs to make the handling better. ??
        bool canTurnBack = boardManager.GetSnakeLength() <= 1;
        if(Input.GetAxisRaw("Horizontal") > 0 && (canTurnBack || previousHeading != Direction.Left))
        {
            heading = Direction.Right;
        } else if (Input.GetAxisRaw("Horizontal") < 0 && (canTurnBack || previousHeading != Direction.Right))
        {
            heading = Direction.Left;
        } else if (Input.GetAxisRaw("Vertical") > 0 && (canTurnBack || previousHeading != Direction.Down))
        {
            heading = Direction.Up;
        } else if (Input.GetAxisRaw("Vertical") < 0 && (canTurnBack || previousHeading != Direction.Up))
        {
            heading = Direction.Down;
        }
        // Boost?
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            boost = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            boost = false;
        }
    }

}










