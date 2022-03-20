using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class ObjectDrag : MonoBehaviour
{
    public abstract void PlaceObject();

    public abstract void TranslateIntoGrid();

    public abstract void RotateObject();
}
