﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveSelectionUI : MonoBehaviour
{
    [SerializeField] List<Text> moveTexts;
    [SerializeField] Color highlightColor;

    public void SetMoveData(List<Move> currentMoves, Move newMove)
    {
        for (int i = 0; i < currentMoves.Count; i++)
        {
            moveTexts[i].text = currentMoves[i].Name;
        }
        moveTexts[currentMoves.Count].text = newMove.Name;
    }

    public void UpdateMoveSelection(int selection)
    {
        for (int i = 0; i < 5; i++)
        {
            if (i == selection)
                moveTexts[i].color = highlightColor;
            else
                moveTexts[i].color = Color.black;
        }
    }

}
