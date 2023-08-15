using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Matchable : Movable
{

    private Cursor cursor;
    private int type;

    public int Type
    {
        get 
        {
            return type;
        }
    }

    private SpriteRenderer spriteRenderer;

    //where is this matchable in the grid?
    public Vector2Int position;

    private void Awake()
    {
        cursor = Cursor.Instance;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetType(int type, Sprite sprite, Color color)
    {
        this.type = type;
        spriteRenderer.sprite = sprite;
        spriteRenderer.color = color;
    }

    private void OnMouseDown()
    {
        print("Mouse Down at (" + position.x + ", " + position.y + ")");
        cursor.SelectFirst(this);
    }

    private void OnMouseUp()
    {
        print("Mouse Up at (" + position.x + ", " + position.y + ")");
        cursor.SelectFirst(null);
    }

    private void OnMouseEnter()
    {
        print("Mouse Enter at (" + position.x + ", " + position.y + ")");
        cursor.SelectSecond(this);
    }

    public override string ToString()
    {
        return gameObject.name;
    }
}
