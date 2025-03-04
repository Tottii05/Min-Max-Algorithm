using System.Collections.Generic;

public class Node
{
    public int PlayerXMove { get; set; }
    public int PlayerYMove { get; set; }
    public int Team { get; set; }
    public int[,] Matrix;
    public int Valuated { get; set; }
    public int Evaluated { get; set; }
    public Stack<Node> ChildPossibilities { get; set; }

    public Node(int[,] matrix, int team, int PlayerX, int PlayerY)
    {
        Matrix = matrix;
        Team = team;
        PlayerXMove = PlayerX;
        PlayerYMove = PlayerY;
        ChildPossibilities = new Stack<Node>();
    }
}
