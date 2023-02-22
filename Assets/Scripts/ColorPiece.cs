using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class ColorPiece : MonoBehaviour
{
    private Dictionary<ColorType, Sprite> _ColorSpriteDict;
    private ColorType _Color;
    private SpriteRenderer _spriteRenderer;
    public ColorSprite[] ColorSprites;
    
    private void Awake()
    {
        //_spriteRenderer = transform.Find("piece").GetComponent<SpriteRenderer>();
        _ColorSpriteDict = new Dictionary<ColorType, Sprite>();
        //TODO: Fix the below line.
        //_spriteRenderer.transform.Find("Piece"); 
        for (int i = 0; i < ColorSprites.Length; i++)
        {
            if (!_ColorSpriteDict.ContainsKey(ColorSprites[i].color))
            {
                _ColorSpriteDict.Add(ColorSprites[i].color, ColorSprites[i].sprite);
            }
        }
    }

    public enum ColorType
    {
        RED,
        GREEN,
        BLUE,
        YELLOW,
        PURPLE
    }
    
    [Serializable]
    public struct ColorSprite
    {
        public ColorType color;
        public Sprite sprite;
    }

    public ColorType Color
    {
        get { return _Color; }
        set { SetColor(value); }
    }

    public int numColors
    {
        get => ColorSprites.Length;
    }
    
    public void SetColor(ColorType newColor)
    {
        _Color = newColor;
        if (_ColorSpriteDict.ContainsKey(newColor))
        {
            _spriteRenderer.sprite = _ColorSpriteDict[newColor];
        }
    }
}
