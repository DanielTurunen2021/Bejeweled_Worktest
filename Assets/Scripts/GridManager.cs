using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;


public class GridManager : MonoBehaviour
{
    [SerializeField] private int _GridX, _GridY;
    [SerializeField] private GameObject _GemBackground;
    [SerializeField] private Transform _CameraTransform;
    public float FillTime;
    private GamePiece[,] _Pieces;
    private GameObject newPiece;
    private GamePiece _PressedPiece;
    private GamePiece _EnteredPiece;
    
    public enum PieceType
    {
       EMPTY,
       RED,
       GREEN,
       BLUE,
       YELLOW,
       PURPLE,
       COUNT
    }

    //Custom struct that holds the gem prefab.
    [Serializable]
    public struct PiecePrefab
    {
        public PieceType type;
        public GameObject prefab;

    }

    //Dictionary that associates each gem type with a game object.
    private Dictionary<PieceType, GameObject> _PiecePrefabDict;
    public PiecePrefab[] PiecePrefabs;


    

    private void Start()
    {
        _PiecePrefabDict = new Dictionary<PieceType, GameObject>();
        
        for (int i = 0; i < PiecePrefabs.Length; i++)
        {
            if (!_PiecePrefabDict.ContainsKey(PiecePrefabs[i].type))
            {
                _PiecePrefabDict.Add(PiecePrefabs[i].type, PiecePrefabs[i].prefab);
                
            }
        }

        SetupGemsArray();
        MakeBackground2();
        StartCoroutine(Fill());
    }

    


    void MakeBackground()
    {
        for (int x = 0; x < _GridX; x++) {
            for (int y = 0; y < _GridY; y++)
            {
                var SpawnTile = Instantiate(_GemBackground,new Vector3(x, y), Quaternion.identity);
            }
        }

        _CameraTransform.position = new Vector3((float)_GridX / 2 -0.5f, (float)_GridY / 2 -0.5f, -10);
    }
    
    
    
    void MakeBackground2()
    {
        for (int i = 0; i < _GridX ; i+=3) {
            for (int j = 0; j < _GridY; j+=3)
            {
                _GemBackground = Instantiate(_GemBackground,GetBackgroundWorldPosition(i, j)  , Quaternion.identity);
                _GemBackground.transform.parent = transform;
                _GemBackground.name = "Background"; 
            }
        }
    }


    void SetupGemsArray()
    {
        _Pieces = new GamePiece[_GridX, _GridY];
        for (int i = 0; i < _GridX; i++) {
            for (int j = 0; j < _GridY; j++)
            {
                SpawnNewPiece(i, j, PieceType.EMPTY);
            }
        }
    }

    //called until the board is filled
    public IEnumerator Fill()
    {
        bool needsReFill = true;

        while (needsReFill)
        {
            yield return new WaitForSeconds(FillTime);
            
            while (FillStep()) {
                yield return new WaitForSeconds(FillTime);
            }
            needsReFill = ClearAllMatches();
        }
    }

    //moves each piece one step
    public bool FillStep()
    {
        bool movedPiece = false;

        /*Goes through the rows in reverse order and checks if pieces can be moved down
         we are ignoring the bottom row because it can't be moved down*/
        for (int y = _GridY -2; y >= 0; y--) {
            for (int x = 0; x < _GridY; x++)
            {
                //current position of the current piece
                GamePiece piece = _Pieces[x, y];
                
                    if (piece.IsMovable())
                    {
                        //position below the current piece we are checking.
                        GamePiece pieceBelow = _Pieces[x, y + 1];
                        //if the space below the current piece is empty
                        if (pieceBelow.PieceType == PieceType.EMPTY)
                        {
                            Destroy(pieceBelow.gameObject);
                            //move the current piece into the space below it.
                            piece.MovablePieceComponent.MovePiece(x, y +1, FillTime);
                            _Pieces[x, y + 1] = piece;
                            //Spawn a new piece at the previous location that is now empty.
                            SpawnNewPiece(x, y, PieceType.EMPTY);
                            movedPiece = true;
                        } 
                    }
            }
        }

      
        
        for (int x = 0; x < _GridX; x++)
        {
            GamePiece pieceBelow = _Pieces[x, 0];

            if (pieceBelow.PieceType == PieceType.EMPTY)
            {
                Destroy(pieceBelow.gameObject);
                GameObject newPiece = Instantiate(_PiecePrefabDict[PieceType.RED], GetWorldPosition(x, -1),
                    quaternion.identity);
                newPiece.transform.parent = transform;

                _Pieces[x, 0] = newPiece.GetComponent<GamePiece>();
                _Pieces[x, 0].Init(x, -1, this, PieceType.RED);
                _Pieces[x, 0].MovablePieceComponent.MovePiece(x, 0, FillTime);
                _Pieces[x, 0].ColorComponent.SetColor((ColorPiece.ColorType)Random.Range(0, 4));
                movedPiece = true;
            }
        }
        return movedPiece;
    }

   public Vector2 GetWorldPosition(int x, int y)
    {
        return new Vector2(transform.position.x - _GridX / 2.0f + x +0.5f,
            transform.position.y + _GridY / 2.0f - y -0.5f);
    }

   private Vector2 GetBackgroundWorldPosition(int x, int y)
   {
       return new Vector2(transform.position.x - _GridX / 2.0f + x + 0.5f,
           transform.position.y + _GridY / 2.0f - y - 0.5f);
   }

   public GamePiece SpawnNewPiece(int x, int y, PieceType pieceType)
   {
       
       newPiece = Instantiate(_PiecePrefabDict[pieceType], GetWorldPosition(x, y), quaternion.identity);
       newPiece.transform.parent = transform;

       _Pieces[x, y] = newPiece.GetComponent<GamePiece>();
       _Pieces[x, y].Init(x, y, this, pieceType);

       return _Pieces[x, y];
   }

   public bool isAdjacent(GamePiece gamePiece1, GamePiece gamePiece2)
   {
       //Debug.Log("Gamepiece1 XPos: " +gamePiece1.XPos + " Gamepiece2 Xpos: " + gamePiece2.XPos + "\n" + 
       //          "Gamepiece1 Ypos:" + gamePiece1.YPos + " Gamepiece2 YPos: " + gamePiece2.YPos);
       
       
       return (gamePiece1.XPos == gamePiece2.XPos && Mathf.Abs(gamePiece1.YPos - gamePiece2.YPos) == 1) ||
        (gamePiece1.YPos == gamePiece2.YPos && Mathf.Abs(gamePiece1.XPos - gamePiece2.XPos) == 1);
   }

   public void SwapPieces(GamePiece gamePiece1, GamePiece gamePiece2)
   {
       if (gamePiece1.IsMovable() && gamePiece2.IsMovable())
       {
           _Pieces[gamePiece1.XPos, gamePiece1.YPos] = gamePiece2;
           _Pieces[gamePiece2.XPos, gamePiece2.YPos] = gamePiece1;

           if (GetMatch(gamePiece1, gamePiece2.XPos, gamePiece2.YPos) != null ||
               GetMatch(gamePiece2, gamePiece1.XPos, gamePiece1.YPos) != null)
           {



               //Store gamepiece 1's coordinates temporarily so they don't get overwritten when the pieces move.
               int Piece1XPos = gamePiece1.XPos;
               int Piece1YPos = gamePiece1.YPos;

               gamePiece1.MovablePieceComponent.MovePiece(gamePiece2.XPos, gamePiece2.YPos, FillTime);
               gamePiece2.MovablePieceComponent.MovePiece(Piece1XPos, Piece1YPos, FillTime);

               ClearAllMatches();

               StartCoroutine(Fill());
           }
           else
           {
               _Pieces[gamePiece1.XPos, gamePiece1.YPos] = gamePiece1;
               _Pieces[gamePiece2.XPos, gamePiece2.YPos] = gamePiece2;
           }
       }
   }


   public void PressPiece(GamePiece PressedPiece)
   {
       _PressedPiece = PressedPiece;
   }

   public void EnterPiece(GamePiece EnteredPiece)
   {
       _EnteredPiece = EnteredPiece;
   }

   public void ReleasePiece()
   {
       if (isAdjacent(_PressedPiece, _EnteredPiece))
       {
           SwapPieces(_PressedPiece, _EnteredPiece);
       }
   }

   public List<GamePiece> GetMatch(GamePiece piece, int newX, int newY)
   {
       if (piece.IsColored())
       {
           //Variable to store the color of the pieces for later comparison
           ColorPiece.ColorType colorType = piece.ColorComponent.Color;
           //Lists for adjacent pieces in the vertical & horizontal directions and a list for matching pieces.
           List<GamePiece> HorizontalPieces = new List<GamePiece>();
           List<GamePiece> VerticalPieces = new List<GamePiece>();
           List<GamePiece> MatchingPieces = new List<GamePiece>();
           
         
           
           //first check horizontally
           HorizontalPieces.Add(piece);

           for (int Dir = 0; Dir <= 1; Dir++) {
               for (int xOffset = 1; xOffset < _GridX; xOffset++)
               {
                   int x;
                   if (Dir == 0)//Checking to the left
                   {
                       x = newX - xOffset;
                   }
                   else {//Checking to the right
                       x = newX + xOffset;
                   }

                   if (x < 0 || x >= _GridX) {
                       break;
                   }
                   //If we find a matching piece we add it to the horizontal list.
                   if (_Pieces[x, newY].IsColored() && _Pieces[x, newY].ColorComponent.Color == colorType) {
                       HorizontalPieces.Add(_Pieces[x, newY]);
                   }
                   else {
                       break; //If we stop finding matching pieces we break the loop.
                   } 
               }
           }

           if (HorizontalPieces.Count >= 3)
           {
               for (int i = 0; i < HorizontalPieces.Count; i++) {
                   MatchingPieces.Add(HorizontalPieces[i]);
               }
           }
           
           //Traverse vertically for L and T shapes
           
           if (HorizontalPieces.Count >= 3)
           {
               for (int i = 0; i < HorizontalPieces.Count; i++) {
                   for (int Dir = 0; Dir <= 1; Dir++) {
                       for (int yOffset = 1; yOffset < _GridY; yOffset++) {
                           int y;

                           if (Dir == 0) {//check upwards
                               y = newY - yOffset;
                           }
                           else { //Check downwards
                               y = newY + yOffset;
                           }
                           if(y < 0 || y >= _GridY){
                               break;
                           }

                           if (_Pieces[HorizontalPieces[i].XPos, y].IsColored() &&
                               _Pieces[HorizontalPieces[i].XPos, y].ColorComponent.Color == colorType) {
                               VerticalPieces.Add(_Pieces[HorizontalPieces[i].XPos, y]);
                           }
                           else {
                               break;
                           }
                       }
                   }
                   //If there are not enough vertical matches we clear the list and check the next horizontal piece for matches.
                   if (VerticalPieces.Count < 2) {
                       VerticalPieces.Clear();
                   }
                   else {
                       for (int j = 0; j < VerticalPieces.Count; j++) {
                           MatchingPieces.Add(VerticalPieces[j]);
                       }
                       break; 
                   }
               }
           }
           if (MatchingPieces.Count >= 3) {
               return MatchingPieces;
           }
           
           
           //second check vertically
           HorizontalPieces.Clear();
           VerticalPieces.Clear();
           VerticalPieces.Add(piece);

           for (int Dir = 0; Dir <= 1; Dir++) {
               for (int yOffset = 1; yOffset < _GridY; yOffset++)
               {
                   int y;
                   if (Dir == 0)//Checking up
                   {
                       y = newY - yOffset;
                   }
                   else {//Checking down
                       y = newY + yOffset;
                   }

                   if (y < 0 || y >= _GridY)
                   {
                       break;
                   }
                   //If we find a matching piece we add it to the horizontal list.
                   if (_Pieces[newX, y].IsColored() && _Pieces[newX, y].ColorComponent.Color == colorType) {
                       VerticalPieces.Add(_Pieces[newX, y]);
                   }
                   
                   if (VerticalPieces.Count >= 3)
                   {
                       for (int i = 0; i < VerticalPieces.Count; i++) {
                           MatchingPieces.Add(VerticalPieces[i]);
                       }
                   }
                   else {
                       break; //If we stop finding matching pieces we break the loop.
                   }  
               }
           }
           
           
           //Traverse horizontally if we found a match(L & T shape)
           if (VerticalPieces.Count >= 3)
           {
               for (int i = 0; i < HorizontalPieces.Count; i++) {
                   for (int Dir = 0; Dir <= 1; Dir++) {
                       for (int xOffset = 1; xOffset < _GridX; xOffset++) {
                           int x;

                           if (Dir == 0) {//check left
                               x = newX - xOffset;
                           }
                           else { //Check right
                               x = newX + xOffset;
                           }
                           if(x < 0 || x >= _GridX){
                               break;
                           }
                            
                           //Check if the current horizontal piece has the same color as the current horizontal piece.
                           if (_Pieces[x, VerticalPieces[i].YPos].IsColored() &&
                               _Pieces[x, VerticalPieces[i].YPos].ColorComponent.Color == colorType) {
                               VerticalPieces.Add(_Pieces[x, VerticalPieces[i].YPos]);
                           }
                           else {
                               break;
                           }
                       }
                   }
                   //If there are not enough vertical matches we clear the list and check the next horizontal piece for matches.
                   if (HorizontalPieces.Count < 2) {
                       HorizontalPieces.Clear();
                   }
                   else {
                       for (int j = 0; j < HorizontalPieces.Count; j++) {
                           MatchingPieces.Add(HorizontalPieces[j]);
                       }
                       break; 
                   }
               }
           }
           if (MatchingPieces.Count >= 3) {
               return MatchingPieces;
           }
       }
       return null;
   }

   public bool ClearPiece(int x, int y)
   {
       if (_Pieces[x, y].IsClearable() && !_Pieces[x, y].ClearablePieceComponent.isBeingCleared)
       {
           _Pieces[x, y].ClearablePieceComponent.Clear();
           SpawnNewPiece(x, y, PieceType.EMPTY);
           return true;
       }
       return false;
   }

   public bool ClearAllMatches()
   {
       bool needsReFill = false;

       for (int y = 0; y < _GridY; y++) {
           for (int x = 0; x < _GridX; x++) {
               if (_Pieces[x, y].IsClearable())
               {
                   List<GamePiece> match = GetMatch(_Pieces[x, y], x, y);

                   if (match != null) {
                       for (int i = 0; i < match.Count; i++) {
                           if (ClearPiece(match[i].XPos, match[i].YPos)) {
                               needsReFill = true;
                           }
                       }
                   }
               }
           }
       }
       return needsReFill;
   }
}



//newPiece = (GameObject)Instantiate(_PiecePrefabDict[PieceType.NORMAL], 
//    GetWorldPosition(i, j) ,quaternion.identity);
////newPiece.name = "piece(" + i + "," + j +")";
//newPiece.transform.parent = transform;
//
//_Pieces[i, j] = newPiece.GetComponent<GamePiece>();
//_Pieces[i, j].Init(i, j, this, PieceType.NORMAL);
//
//if (_Pieces[i, j].IsMovable())
//{
//    _Pieces[i, j].MovablePieceComponent.MovePiece(i,j);
//}

//if (_Pieces[i, j].IsColored())
//{
//    _Pieces[i, j].ColorComponent.SetColor((ColorPiece.ColorType)Random.Range(0, 5));
//}






//Traverse horizontally for L and T shapes
/*if (VerticalPieces.Count >= 3)
{
    for (int i = 0; i < VerticalPieces.Count; i++) {
        for (int Dir = 0; Dir <= 1; Dir++) {
            for (int xOffset = 1; xOffset < _GridY; xOffset++) {
                int x;

                if (Dir == 0) {//check left
                    x = newX - xOffset;
                }
                else { //Check right
                    x = newX + xOffset;
                }
                if(x < 0 || x >= _GridX){
                    break;
                }

                if (_Pieces[x, VerticalPieces[i].YPos].IsColored() &&
                    _Pieces[x,VerticalPieces[i].YPos].ColorComponent.Color == colorType) {
                    VerticalPieces.Add(_Pieces[x, VerticalPieces[i].YPos]);
                }
                else {
                    break;
                }
            }
        }
        if(HorizontalPieces.Count < 2){
            HorizontalPieces.Clear();
        }
        else
        {
            for (int j = 0; j < HorizontalPieces.Count; j++) {
                MatchingPieces.Add(VerticalPieces[j]);
            }
        }
    }
}*/

