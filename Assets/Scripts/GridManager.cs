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
    private GamePiece[,] _Pieces;
    private GameObject newPiece;
    
    public enum PieceType
    {
       NORMAL,
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
    
    private void Awake()
    {
        //throw new NotImplementedException();
    }
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
        //_CameraTransform.position = new Vector3((float)_GridX / 2 -0.5f, (float)_GridY / 2 -0.5f, -10);
    }


    void SetupGemsArray()
    {
        _Pieces = new GamePiece[_GridX, _GridY];
        for (int i = 0; i < _GridX; i++) {
            for (int j = 0; j < _GridY; j++)
            {
                newPiece = Instantiate(_PiecePrefabDict[PieceType.NORMAL], 
                    GetWorldPosition(i, j) ,quaternion.identity);
                newPiece.name = "piece(" + i + "," + j +")";
                newPiece.transform.parent = transform;

                _Pieces[i, j] = newPiece.GetComponent<GamePiece>();
                _Pieces[i, j].Init(i, j, this, PieceType.NORMAL);

                if (_Pieces[i, j].IsMovable())
                {
                    _Pieces[i, j].MovablePieceComponent.MovePiece(i,j);
                }

                if (_Pieces[i, j].IsColored())
                {
                    _Pieces[i, j].ColorComponent.SetColor((ColorPiece.ColorType)Random.Range(0, 5));
                }
            }
        }
        //_CameraTransform.position = new Vector3((float)_GridX / 2 -0.5f, (float)_GridY / 2 -0.5f, -10);
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
}




