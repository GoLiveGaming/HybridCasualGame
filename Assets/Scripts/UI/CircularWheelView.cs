using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularWheelView : MonoBehaviour
{
    [Range(5, 360)] public float wheelAngle = 180;
    [Range(0.1f, 1000)] public float wheelRadius;

    [SerializeField, ReadOnly] private int numObjects;
    [SerializeField, ReadOnly] private List<GameObject> childObjects = new();

    void FixedUpdate()
    {
        childObjects.Clear();

        foreach (Transform t in transform)
        {
            childObjects.Add(t.gameObject);
        }
        numObjects = transform.childCount;
        float angle = wheelAngle / numObjects;

        for (int i = 0; i < numObjects; i++)
        {
            float x = Mathf.Cos(Mathf.Deg2Rad * angle * i) * wheelRadius;
            float y = Mathf.Sin(Mathf.Deg2Rad * angle * i) * wheelRadius;
            childObjects[i].transform.localPosition = new Vector3(x, y, 0);
        }
    }
}