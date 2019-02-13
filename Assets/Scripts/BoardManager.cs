using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BoardPieceType
{
    Snake,
    Fruit,
    None
}

public class BoardPiece
{
    public int x;
    public int y;
    public GameObject gameObject;
    public BoardPieceType type = BoardPieceType.None;
}


public class SnakeBit : BoardPiece
{
    
    public SnakeBit()
    {
        type = BoardPieceType.Snake;
    }
}

public class Fruit: BoardPiece
{
    public Fruit()
    {
        type = BoardPieceType.Fruit;
    }
}

public class Snake
{
    public SnakeBit Head;
    public SnakeBit Tail;

    public LinkedList<SnakeBit> body;

    public Snake()
    {
        body = new LinkedList<SnakeBit>();
    }

    public SnakeBit PopTail()
    {
        SnakeBit PrevTail = Tail;
        if (Tail != null)
        {
            body.RemoveLast();
            if (body.Count > 0)
            {
                Tail = body.Last.Value;
            }
        }
        return PrevTail;
    }

    public void PushHead(SnakeBit newHead)
    {
        body.AddFirst(newHead);
        Head = newHead;
    }

}


public class BoardManager : MonoBehaviour
{
    private int freeSpaces = 0;

    private GameObjectPool snakePool;
    private GameObjectPool fruitPool;
    private GameObjectPool borderPool;

    public GameObject SnakeBodyPrefab;
    public GameObject FruitPrefab;
    public GameObject BorderPrefab;

    public GameObject BoardBottomLeft;
    public GameObject BoardTopRight;

    private Vector3 bottomLeft;
    private Vector3 topRight;

    private float bottom;
    private float top;
    private float left;
    private float right;

    private float size;
    private float shift;

    int sizeX = 10;
    int sizeY = 10;

    public BoardPiece[,] board;

    private Snake snake;

    public int fruitNumber = 1;

    public BoardManager()
    {
             
    }

    public int GetSnakeLength()
    {
        return snake.body.Count;
    }

    public void Start()
    {
        snakePool = new GameObjectPool(SnakeBodyPrefab);
        fruitPool = new GameObjectPool(FruitPrefab);
        borderPool = new GameObjectPool(BorderPrefab);

        bottom = BoardBottomLeft.transform.position.y;
        left = BoardBottomLeft.transform.position.x;
        top = BoardTopRight.transform.position.y;
        right = BoardTopRight.transform.position.x;
    }

    public void InitBoard(int sizeX, int sizeY)
    {
        freeSpaces = sizeX * sizeY;
        borderPool.ClearAll();
        board = new BoardPiece[sizeX, sizeY];
        this.sizeX = sizeX;
        this.sizeY = sizeY;

        size = Mathf.Min((top - bottom) / sizeY, (right - left) / sizeX);
        shift = size / 2;

        for (int x = -1; x <= sizeX ; x++)
        {
            GameObject border = borderPool.GetObject();
            PlaceGameObjectAt(x, -1, border);
            border = borderPool.GetObject();
            PlaceGameObjectAt(x, sizeY, border);
        }
        for (int y = 0; y <= sizeY; y++)
        {
            GameObject border = borderPool.GetObject();
            PlaceGameObjectAt(-1, y, border);
            border = borderPool.GetObject();
            PlaceGameObjectAt(sizeX, y, border);
        }



    }

    public void PlaceGameObjectAt(int x, int y, GameObject gameObject)
    {
        // Dynamic sizes will have to make custom calculation to position game objects in the 
        // virtual grid.
        float bx = left + size * x + shift;
        float by = bottom + size * y + shift;
        gameObject.transform.position = new Vector3(bx, by, 0);
        gameObject.transform.localScale = new Vector3(size, size, 1);
    }


    private SnakeBit MakeSnakeBit(int x, int y)
    {
        SnakeBit bit = new SnakeBit
        {
            x = x,
            y = y
        };
        bit.gameObject = snakePool.GetObject();
        PlaceGameObjectAt(x, y, bit.gameObject);
        return bit;
    }

    public void NewGame()
    {
        snakePool.ClearAll();
        fruitPool.ClearAll();
        snake = new Snake();
        int midX = sizeX / 2;
        int midY = sizeY / 2;
        snake = new Snake();
        AddSnakeHead(midX, midY);
        snake.Tail = snake.Head;

        for (int i = 0; i < fruitNumber; i++)
        {
            PlaceFruit();
        }
    }

    private void AddSnakeHead(int x, int y)
    {
        SnakeBit newHead = MakeSnakeBit(x, y);
        snake.PushHead(newHead);
        board[x, y] = newHead;
        freeSpaces -= 1;
    }

    private void AddFruit(int x, int y)
    {
        Fruit fruit = new Fruit();
        fruit.x = x;
        fruit.y = y;
        fruit.type = BoardPieceType.Fruit;
        fruit.gameObject = fruitPool.GetObject();
        PlaceGameObjectAt(x, y, fruit.gameObject);
        board[x, y] = fruit;
        freeSpaces -= 1;
    }

    public void PlaceFruit()
    {
        int tries = 0;
        bool success = false;
        System.Random rnd = new System.Random();

        int x = 0;
        int y = 0;

        while (!success && tries < 200)
        {
            tries += 1;
            x = rnd.Next(0, sizeX - 1);
            y = rnd.Next(0, sizeY - 1);
            if (board[x,y] == null)
            {
                success = true;
            }
        }
        if (success)
        {
            AddFruit(x, y);
        }
        else
        {
            // Then what???
            // TODO: track all empty spaces, and choose from them to make sure that the 
            // fruit can get placed, and does get placed.
            Debug.Log("could not place fruit");
        }
    }

    public bool Move(Direction direction)
    {
        int nhx = 0;
        int nhy = 0;

        if (direction == Direction.Up)
        {
            nhx = snake.Head.x;
            nhy = snake.Head.y + 1;
        }
        if (direction == Direction.Down)
        {
            nhx = snake.Head.x;
            nhy = snake.Head.y - 1;
        }
        if (direction == Direction.Left)
        {
            nhx = snake.Head.x - 1;
            nhy = snake.Head.y;
        }
        if (direction == Direction.Right)
        {
            nhx = snake.Head.x + 1;
            nhy = snake.Head.y;
        }
        if (nhx < 0 || nhx >= sizeX || nhy < 0 || nhy >= sizeY)
        {
            Debug.Log("Hit a wall");
            // Hit a wall !!!
            return false;
        }
        else if (board[nhx, nhy] == null)
        {
            Debug.Log("Moving");
            // Empty space, gonna step there.
            SnakeBit tail = snake.PopTail();
            // Remove tail from previous board position
            board[tail.x, tail.y] = null;
            // Move the tail to the new position
            board[nhx, nhy] = tail;
            tail.x = nhx;
            tail.y = nhy;
            PlaceGameObjectAt(nhx, nhy, tail.gameObject);
            // Make it the new head
            snake.PushHead(tail);
        } else if (board[nhx, nhy].type == BoardPieceType.Snake)
        {
            Debug.Log("Hit the tail");
            // We hit our own tail! (TODO prevent immidiate step back)
            return false;
        } else if (board[nhx, nhy].type == BoardPieceType.Fruit)
        {
            Debug.Log("Ate a fruit");
            // We ate a fruite, so we grow.
            // Remove the fruit, and free it up in the pool
            BoardPiece fruit = board[nhx, nhy];
            fruitPool.ReleaseObject(fruit.gameObject);
            board[nhx, nhy] = null;
            // Add a snake part instead
            AddSnakeHead(nhx, nhy);
            PlaceFruit();
        }
        else
        {
            Debug.Log("You should not be here...");
            Debug.Log(board[nhx, nhy].type);
        }

        return true;

    }

}
