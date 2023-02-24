using System;
using System.Collections.Generic;
using UnityEngine;

public class ColorPiece : MonoBehaviour
{
    private Dictionary<ColorType, Sprite> _ColorSpriteDict;
    private ColorType color;
    private SpriteRenderer _spriteRenderer;
    public ColorSprite[] ColorSprites;
    
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _ColorSpriteDict = new Dictionary<ColorType, Sprite>();
        
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
        PURPLE,
    }
    
    [Serializable]
    public struct ColorSprite
    {
        public ColorType color;
        public Sprite sprite;
    }

    public ColorType Color
    {
        get { return color; }
        set { SetColor(value); }
    }

    public int numColors
    {
        get => ColorSprites.Length;
    }
    
    public void SetColor(ColorType newColor)
    {
        color = newColor;
        if (_ColorSpriteDict.ContainsKey(newColor))
        {
            _spriteRenderer.sprite = _ColorSpriteDict[newColor];
        }
    }
}
