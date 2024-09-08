using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetris : MonoBehaviour
{
    public Color[] colors;
    public GameObject block;
    public bool spawn;
    public float nextBlockSpawnTime = 0.5f;
    public float blockFallSpeed = 0.5f;
    public int gameOverHeight = 2;
    public float moveDownDelay = 0.15f;
    public int[,] board;
    
    private int currentRot;
    private bool gameOver;
    private GameObject pivot;
    private readonly List<Transform> shapes = new();
    private float _moveDownDelayCounter = 0f;
    
    private void Start()
    {
        board = new int[12, 24];
        GenBoard();
        InvokeRepeating(nameof(moveDown), blockFallSpeed, blockFallSpeed);
    }


    private void Update()
    {
        if (spawn && shapes.Count == 4)
        {
            var a = shapes[0].transform.position;
            var b = shapes[1].transform.position;
            var d = shapes[2].transform.position;
            var c = shapes[3].transform.position;


            if (Input.GetKeyDown(KeyCode.LeftArrow))
                if (CheckUserMove(a, b, c, d, true))
                {
                    a.x -= 1;
                    b.x -= 1;
                    c.x -= 1;
                    d.x -= 1;

                    pivot.transform.position = new Vector3(pivot.transform.position.x - 1, pivot.transform.position.y,
                        pivot.transform.position.z);

                    shapes[0].transform.position = a;
                    shapes[1].transform.position = b;
                    shapes[2].transform.position = c;
                    shapes[3].transform.position = d;
                }

            if (Input.GetKeyDown(KeyCode.RightArrow))
                if (CheckUserMove(a, b, c, d, false))
                {
                    a.x += 1;
                    b.x += 1;
                    c.x += 1;
                    d.x += 1;

                    pivot.transform.position = new Vector3(pivot.transform.position.x + 1, pivot.transform.position.y,
                        pivot.transform.position.z);

                    shapes[0].transform.position = a;
                    shapes[1].transform.position = b;
                    shapes[2].transform.position = c;
                    shapes[3].transform.position = d;
                }

            if (Input.GetKey(KeyCode.DownArrow))
            {
                if (_moveDownDelayCounter <= 0)
                {
                    _moveDownDelayCounter = moveDownDelay;
                    moveDown();
                }

                _moveDownDelayCounter -= Time.deltaTime;
            }


            if (Input.GetKeyDown(KeyCode.Space))
                Rotate(shapes[0].transform, shapes[1].transform, shapes[2].transform, shapes[3].transform);
        }


        if (!spawn && !gameOver)
        {
            StartCoroutine(nameof(Wait));
            spawn = true;

            currentRot = 0;
        }
    }


    private void moveDown()
    {
        if (shapes.Count != 4) return;
        var a = shapes[0].transform.position;
        var b = shapes[1].transform.position;
        var c = shapes[2].transform.position;
        var d = shapes[3].transform.position;

        if (CheckMove(a, b, c, d))
        {
            a = new Vector3(Mathf.RoundToInt(a.x), Mathf.RoundToInt(a.y - 1.0f), a.z);
            b = new Vector3(Mathf.RoundToInt(b.x), Mathf.RoundToInt(b.y - 1.0f), b.z);
            c = new Vector3(Mathf.RoundToInt(c.x), Mathf.RoundToInt(c.y - 1.0f), c.z);
            d = new Vector3(Mathf.RoundToInt(d.x), Mathf.RoundToInt(d.y - 1.0f), d.z);

            pivot.transform.position = new Vector3(pivot.transform.position.x, pivot.transform.position.y - 1,
                pivot.transform.position.z);

            shapes[0].transform.position = a;
            shapes[1].transform.position = b;
            shapes[2].transform.position = c;
            shapes[3].transform.position = d;
        }
        else
        {
            Destroy(pivot.gameObject);


            board[Mathf.RoundToInt(a.x), Mathf.RoundToInt(a.y)] = 1;
            board[Mathf.RoundToInt(b.x), Mathf.RoundToInt(b.y)] = 1;
            board[Mathf.RoundToInt(c.x), Mathf.RoundToInt(c.y)] = 1;
            board[Mathf.RoundToInt(d.x), Mathf.RoundToInt(d.y)] = 1;


            checkRow(1);
            checkRow(gameOverHeight);


            shapes.Clear();
            spawn = false;
        }
    }


    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(nextBlockSpawnTime);
        SpawnShape();
    }


    private bool CheckMove(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        if (board[Mathf.RoundToInt(a.x), Mathf.RoundToInt(a.y - 1)] == 1) return false;
        if (board[Mathf.RoundToInt(b.x), Mathf.RoundToInt(b.y - 1)] == 1) return false;
        if (board[Mathf.RoundToInt(c.x), Mathf.RoundToInt(c.y - 1)] == 1) return false;
        if (board[Mathf.RoundToInt(d.x), Mathf.RoundToInt(d.y - 1)] == 1) return false;

        return true;
    }


    private bool CheckUserMove(Vector3 a, Vector3 b, Vector3 c, Vector3 d, bool dir)
    {
        if (dir)
        {
            if (board[Mathf.RoundToInt(a.x - 1), Mathf.RoundToInt(a.y)] == 1 ||
                board[Mathf.RoundToInt(b.x - 1), Mathf.RoundToInt(b.y)] == 1 ||
                board[Mathf.RoundToInt(c.x - 1), Mathf.RoundToInt(c.y)] == 1 ||
                board[Mathf.RoundToInt(d.x - 1), Mathf.RoundToInt(d.y)] == 1) return false;
        }
        else
        {
            if (board[Mathf.RoundToInt(a.x + 1), Mathf.RoundToInt(a.y)] == 1 ||
                board[Mathf.RoundToInt(b.x + 1), Mathf.RoundToInt(b.y)] == 1 ||
                board[Mathf.RoundToInt(c.x + 1), Mathf.RoundToInt(c.y)] == 1 ||
                board[Mathf.RoundToInt(d.x + 1), Mathf.RoundToInt(d.y)] == 1) return false;
        }

        return true;
    }


    private void GenBoard()
    {
        var material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));

        for (var x = 0; x < board.GetLength(0); x++)
        for (var y = 0; y < board.GetLength(1); y++)
            if (x < 11 && x > 0)
            {
                if (y > 0 && y < board.GetLength(1) - 2)
                {
                    board[x, y] = 0;
                }
                else if (y < board.GetLength(1) - 2)
                {
                    board[x, y] = 1;
                    var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.position = new Vector3(x, y, 0);
                    material.color = Color.black;
                    cube.GetComponent<Renderer>().material = material;
                    cube.transform.parent = transform;
                    cube.GetComponent<Collider>().isTrigger = true;
                }
            }
            else if (y < board.GetLength(1) - 2)
            {
                board[x, y] = 1;
                var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = new Vector3(x, y, 0);
                material.color = Color.black;
                cube.GetComponent<Renderer>().material = material;
                cube.transform.parent = transform;
            }
    }


    private void SpawnShape()
    {
        var shape = Random.Range(0, 6);
        var height = board.GetLength(1) - 4;
        var xPos = board.GetLength(0) / 2 - 1;


        pivot = new GameObject("RotateAround");

        var color = colors[Random.Range(0, colors.Length)];


        if (shape == 0)
        {
            pivot.transform.position = new Vector3(xPos, height + 1, 0);

            shapes.Add(GenBlock(new Vector3(xPos, height, 0), color));
            shapes.Add(GenBlock(new Vector3(xPos - 1, height, 0), color));
            shapes.Add(GenBlock(new Vector3(xPos, height + 1, 0), color));
            shapes.Add(GenBlock(new Vector3(xPos + 1, height + 1, 0), color));


            Debug.Log("Spawned SShape");
        }

        else if (shape == 1)
        {
            pivot.transform.position = new Vector3(xPos + 0.5f, height + 1.5f, 0);

            shapes.Add(GenBlock(new Vector3(xPos, height, 0), color));
            shapes.Add(GenBlock(new Vector3(xPos, height + 1, 0), color));
            shapes.Add(GenBlock(new Vector3(xPos, height + 2, 0), color));
            shapes.Add(GenBlock(new Vector3(xPos, height + 3, 0), color));

            Debug.Log("Spawned IShape");
        }

        else if (shape == 2)
        {
            pivot.transform.position = new Vector3(xPos + 0.5f, height + 0.5f, 0);

            shapes.Add(GenBlock(new Vector3(xPos, height, 0), color));
            shapes.Add(GenBlock(new Vector3(xPos + 1, height, 0), color));
            shapes.Add(GenBlock(new Vector3(xPos, height + 1, 0), color));
            shapes.Add(GenBlock(new Vector3(xPos + 1, height + 1, 0), color));

            Debug.Log("Spawned OShape");
        }

        else if (shape == 3)
        {
            pivot.transform.position = new Vector3(xPos, height + 2, 0);

            shapes.Add(GenBlock(new Vector3(xPos, height, 0), color));
            shapes.Add(GenBlock(new Vector3(xPos + 1, height, 0), color));
            shapes.Add(GenBlock(new Vector3(xPos, height + 1, 0), color));
            shapes.Add(GenBlock(new Vector3(xPos, height + 2, 0), color));

            Debug.Log("Spawned JShape");
        }


        else if (shape == 4)
        {
            pivot.transform.position = new Vector3(xPos, height, 0);

            shapes.Add(GenBlock(new Vector3(xPos, height, 0), color));
            shapes.Add(GenBlock(new Vector3(xPos - 1, height, 0), color));
            shapes.Add(GenBlock(new Vector3(xPos + 1, height, 0), color));
            shapes.Add(GenBlock(new Vector3(xPos, height + 1, 0), color));

            Debug.Log("Spawned TShape");
        }


        else if (shape == 5)
        {
            pivot.transform.position = new Vector3(xPos, height + 1, 0);

            shapes.Add(GenBlock(new Vector3(xPos, height, 0), color));
            shapes.Add(GenBlock(new Vector3(xPos - 1, height, 0), color));
            shapes.Add(GenBlock(new Vector3(xPos, height + 1, 0), color));
            shapes.Add(GenBlock(new Vector3(xPos, height + 2, 0), color));

            Debug.Log("Spawned LShape");
        }


        else
        {
            pivot.transform.position = new Vector3(xPos, height + 1, 0);

            shapes.Add(GenBlock(new Vector3(xPos, height, 0), color));
            shapes.Add(GenBlock(new Vector3(xPos + 1, height, 0), color));
            shapes.Add(GenBlock(new Vector3(xPos, height + 1, 0), color));
            shapes.Add(GenBlock(new Vector3(xPos - 1, height + 1, 0), color));

            Debug.Log("Spawned ZShape");
        }
    }


    private Transform GenBlock(Vector3 pos, Color color)
    {
        var obj = Instantiate(block.transform, pos, Quaternion.identity);
        obj.GetComponent<MeshRenderer>().material.color = color;
        obj.tag = "Block";

        return obj;
    }


    private void checkRow(int y)
    {
        var blocks = GameObject.FindGameObjectsWithTag("Block");
        var count = 0;

        for (var x = 1; x < board.GetLength(0) - 1; x++)
            if (board[x, y] == 1)
                count++;


        if (y == gameOverHeight && count > 0)
        {
            Debug.LogWarning("Game over");
            gameOver = true;
        }

        if (count == 10)
        {
            for (var cy = y; cy < board.GetLength(1) - 3; cy++)
            for (var cx = 1; cx < board.GetLength(0) - 1; cx++)
                foreach (var go in blocks)
                {
                    var height = Mathf.RoundToInt(go.transform.position.y);
                    var xPos = Mathf.RoundToInt(go.transform.position.x);

                    if (xPos == cx && height == cy)
                    {
                        if (height == y)
                        {
                            board[xPos, height] = 0;
                            Destroy(go.gameObject);
                        }
                        else if (height > y)
                        {
                            board[xPos, height] = 0;
                            board[xPos, height - 1] = 1;
                            go.transform.position = new Vector3(xPos, height - 1, go.transform.position.z);
                        }
                    }
                }

            checkRow(y);
        }
        else if (y + 1 < board.GetLength(1) - 3)
        {
            checkRow(y + 1);
        }
    }


    private void Rotate(Transform a, Transform b, Transform c, Transform d)
    {
        a.parent = pivot.transform;
        b.parent = pivot.transform;
        c.parent = pivot.transform;
        d.parent = pivot.transform;

        currentRot += 90;
        if (currentRot == 360) currentRot = 0;

        pivot.transform.localEulerAngles = new Vector3(0, 0, currentRot);

        a.parent = null;
        b.parent = null;
        c.parent = null;
        d.parent = null;

        if (CheckRotate(a.position, b.position, c.position, d.position) == false)
        {
            a.parent = pivot.transform;
            b.parent = pivot.transform;
            c.parent = pivot.transform;
            d.parent = pivot.transform;

            currentRot -= 90;
            pivot.transform.localEulerAngles = new Vector3(0, 0, currentRot);

            a.parent = null;
            b.parent = null;
            c.parent = null;
            d.parent = null;
        }
    }


    private bool CheckRotate(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        if (Mathf.RoundToInt(a.x) < board.GetLength(0) - 1)
        {
            if (board[Mathf.RoundToInt(a.x), Mathf.RoundToInt(a.y)] == 1) return false;
        }
        else
        {
            return false;
        }

        if (Mathf.RoundToInt(b.x) < board.GetLength(0) - 1)
        {
            if (board[Mathf.RoundToInt(b.x), Mathf.RoundToInt(b.y)] == 1) return false;
        }
        else
        {
            return false;
        }

        if (Mathf.RoundToInt(c.x) < board.GetLength(0) - 1)
        {
            if (board[Mathf.RoundToInt(c.x), Mathf.RoundToInt(c.y)] == 1) return false;
        }
        else
        {
            return false;
        }

        if (Mathf.RoundToInt(d.x) < board.GetLength(0) - 1)
        {
            if (board[Mathf.RoundToInt(d.x), Mathf.RoundToInt(d.y)] == 1) return false;
        }
        else
        {
            return false;
        }

        return true;
    }
}