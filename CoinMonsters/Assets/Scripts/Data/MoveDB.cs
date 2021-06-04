using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveDB
{

    static Dictionary<string, Move> moves;

    public static void Init()
    {
        moves = new Dictionary<string, Move>();

        Move[] moveList = Resources.LoadAll<Move>("");

        foreach (Move move in moveList)
        {
            moves[move.Name] = move;
        }
    }

    public static Move GetMoveByName(string name)
    {
        return moves[name];
    }

}
