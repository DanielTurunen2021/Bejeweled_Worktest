using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovablePiece : MonoBehaviour
{
    private GamePiece _Piece;
    

    private void Awake()
    {
        _Piece = GetComponent<GamePiece>();
    }

    
    public void MovePiece(int newXPos, int newYPos)
    {
        _Piece.XPos = newXPos;
        _Piece.YPos = newYPos;

        _Piece.transform.localPosition = _Piece.GridManagerRef.GetWorldPosition(newXPos, newYPos);
    }
}
