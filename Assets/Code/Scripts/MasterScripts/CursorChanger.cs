using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorChanger : MonoBehaviour
{
    Action ChangeCursor;

    public Texture2D cursor;
    public Vector2 hotSpot;

    private void Awake()
    {
        CursorTextureChanged();
    }

    private void Update()
    {
        ChangeCursor();
    }

    public void CursorTextureChanged()
    {
        ChangeCursor = delegate ()
        {
            Cursor.SetCursor(cursor, hotSpot, CursorMode.Auto);
            ChangeCursor = delegate () { };
        };
    }
}
