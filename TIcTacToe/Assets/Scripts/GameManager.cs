using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public enum States
{
    CanMove,
    CantMove
}
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public BoxCollider2D collider;
    public GameObject token1, token2;
    public int Size = 3;
    public int[,] Matrix;
    public int lastMoveX;
    public int lastMoveY;
    [SerializeField] private States state = States.CanMove;
    public Camera camera;
    public int[,] auxiliarMatrix;
    void Start()
    {
        Instance = this;
        Matrix = new int[Size, Size];
        Calculs.CalculateDistances(collider, Size);
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                Matrix[i, j] = 0; // 0: desocupat, 1: fitxa jugador 1, -1: fitxa IA;
            }
        }
    }
    private void Update()
    {
        if (state == States.CanMove)
        {
            Vector3 m = Input.mousePosition;
            m.z = 10f;
            Vector3 mousepos = camera.ScreenToWorldPoint(m);
            if (Input.GetMouseButtonDown(0))
            {
                if (Calculs.CheckIfValidClick((Vector2)mousepos, Matrix))
                {
                    state = States.CantMove;
                    if (Calculs.EvaluateWin(Matrix) == 2)
                        StartCoroutine(WaitingABit());
                }
            }
        }
    }
    private IEnumerator WaitingABit()
    {
        yield return new WaitForSeconds(1f);
        RandomAI();
        MinMaxAlgorithm();
    }
    public void RandomAI()
    {
        Node initialNode = new Node(Matrix, -1, lastMoveX, lastMoveY);
        MatrixGenerator(Matrix.Length, Matrix.Length, initialNode);
        state = States.CanMove;
    }
    public void DoMove(int x, int y, int team, ref int[,] matrix)
    {
        matrix[x, y] = team;
        if (team == 1)
            Instantiate(token1, Calculs.CalculatePoint(x, y), Quaternion.identity);
        else
            Instantiate(token2, Calculs.CalculatePoint(x, y), Quaternion.identity);
        int result = Calculs.EvaluateWin(Matrix);
        switch (result)
        {
            case 0:
                Debug.Log("Draw");
                break;
            case 1:
                Debug.Log("You Win");
                break;
            case -1:
                Debug.Log("You Lose");
                break;
            case 2:
                if (state == States.CantMove)
                    state = States.CanMove;
                break;
        }
    }
    public void MatrixGenerator(int x, int y, Node initialNode)
    {
        for (int i = 0; i < initialNode.Matrix.GetLength(0); i++)
        {
            for (int j = 0; j < initialNode.Matrix.GetLength(1); j++)
            {
                if (initialNode.Matrix[i, j] == 0)
                {
                    int[,] newMatrix = MatrixCloner(initialNode.Matrix);
                    newMatrix[i, j] = -1;
                    Node node = new Node(newMatrix, -1, i, j);
                    initialNode.ChildPossibilities.Push(node);
                    int result = Calculs.EvaluateWin(newMatrix);
                    if (result == 2)
                    {
                        MatrixGenerator(initialNode.Matrix.GetLength(0), initialNode.Matrix.GetLength(1), node);
                    }
                    else
                    {
                        node.Valuated = result;
                    }
                }
            }
        }
    }

    public int[,] MatrixCloner(int[,] matrix)
    {
        int[,] newMatrix = new int[matrix.GetLength(0), matrix.GetLength(1)];
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                newMatrix[i, j] = matrix[i, j];
            }
        }
        return newMatrix;
    }
    public void MinMaxAlgorithm()
    {
        int bestScore = int.MinValue;
        int moveX = -1, moveY = -1;
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                if (Matrix[i, j] == 0)
                {
                    Matrix[i, j] = -1;
                    int score = Minimax(Matrix, 0, false);
                    Matrix[i, j] = 0;

                    if (score > bestScore)
                    {
                        bestScore = score;
                        moveX = i;
                        moveY = j;
                    }
                }
            }
        }
        if (moveX != -1 && moveY != -1)
        {
            DoMove(moveX, moveY, -1, ref Matrix);
        }
    }
    private int Minimax(int[,] board, int depth, bool isMaximizing)
    {
        int result = Calculs.EvaluateWin(board);
        if (result == -1) return 10 - depth;
        if (result == 1) return depth - 10;
        if (result == 0) return 0;

        if (isMaximizing)
        {
            int bestScore = int.MinValue;
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    if (board[i, j] == 0)
                    {
                        board[i, j] = -1;
                        int score = Minimax(board, depth + 1, false);
                        board[i, j] = 0;
                        bestScore = Mathf.Max(bestScore, score);
                    }
                }
            }
            return bestScore;
        }
        else 
        {
            int bestScore = int.MaxValue;
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    if (board[i, j] == 0)
                    {
                        board[i, j] = 1;
                        int score = Minimax(board, depth + 1, true);
                        board[i, j] = 0;
                        bestScore = Mathf.Min(bestScore, score);
                    }
                }
            }
            return bestScore;
        }
    }

}
