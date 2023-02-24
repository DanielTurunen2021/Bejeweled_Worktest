using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GamePiece : MonoBehaviour
{
    private int _GamePieceXPos;
    private int _GamePieceYPos;
    private GridManager.PieceType _pieceType;
    private GridManager _gridManager;
    private MovablePiece _movablePieceComponent;
    private ColorPiece _ColorComponent;
    private ClearablePiece _clearablePieceComponent;

    private void Awake()
    {
        _movablePieceComponent = GetComponent<MovablePiece>();
        _ColorComponent = GetComponent<ColorPiece>();
        _clearablePieceComponent = GetComponent<ClearablePiece>();
    }

    public int XPos
    {
        get
        {
            return _GamePieceXPos; 
            
        }
        set
        {
            if(IsMovable())
                _GamePieceXPos = value;
        }
    }

    public int YPos
    {
        get
        {
            return _GamePieceYPos;
        }
        set
        {
            if(IsMovable())
                _GamePieceYPos = value;
        }
    }

    

    

    public GridManager.PieceType PieceType
    {
        get { return _pieceType; }
    }

    public GridManager GridManagerRef
    {
        get { return _gridManager; }
    }

    public MovablePiece MovablePieceComponent
    {
        get { return _movablePieceComponent; }
    }

    public ColorPiece ColorComponent
    {
        get => _ColorComponent;
    }

    public ClearablePiece ClearablePieceComponent => _clearablePieceComponent;


    public bool IsMovable()
    {
        return _movablePieceComponent != null;
    }

    public bool IsColored()
    {
        return _ColorComponent != null;
    }

    public bool IsClearable()
    {
        return _clearablePieceComponent != null;
    }

    public void Init(int xpos, int ypos, GridManager gridManager, GridManager.PieceType pieceType)
    {
        _GamePieceXPos = xpos;
        _GamePieceXPos = ypos;
        _gridManager = gridManager;
        _pieceType = pieceType;
    }

    private void OnMouseDown()
    {
        _gridManager.PressPiece(this);
    }

    private void OnMouseEnter()
    {
        _gridManager.EnterPiece(this);
    }

    private void OnMouseUp()
    {
        _gridManager.ReleasePiece();
    }
}
