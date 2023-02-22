using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MovablePiece : MonoBehaviour
{
    private GamePiece _Piece;
    private IEnumerator _MoveCoroutine;
    

    private void Awake()
    {
        _Piece = GetComponent<GamePiece>();
    }

    
    public void MovePiece(int newXPos, int newYPos, float timeToPlay)
    {
        if (_MoveCoroutine != null)
        {
            StartCoroutine(_MoveCoroutine);
        }

        _MoveCoroutine = MoveCoroutine(newXPos, newYPos, timeToPlay);
        StartCoroutine(_MoveCoroutine);
        
        
        //_Piece.XPos = newXPos;
        //_Piece.YPos = newYPos;
//
        //_Piece.transform.localPosition = _Piece.GridManagerRef.GetWorldPosition(newXPos, newYPos);
    }

    private IEnumerator MoveCoroutine(int newXPos, int newYPos, float timeToPlay)
    {
        _Piece.XPos = newXPos;
        _Piece.YPos = newYPos;
        
        Vector3 startPosition = transform.position;
        Vector3 endPosition = _Piece.GridManagerRef.GetWorldPosition(newXPos, newYPos);

        for (float t = 0; t <= 1 * timeToPlay; t += Time.deltaTime)
        {
            _Piece.transform.position = Vector3.Lerp(startPosition, endPosition, t / timeToPlay);
            yield return 0;
        }
        _Piece.transform.position = endPosition;
    }

    public void Move(int newX, int newY, float timeToPlay)
    {
        if (_MoveCoroutine != null)
        {
            StopCoroutine(_MoveCoroutine);
        }

        _MoveCoroutine = MoveCoroutine(newX, newY, timeToPlay);
        StartCoroutine(_MoveCoroutine);
    }
    
}
