using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{
    public Vector2 size = new (3, 3);
    public float spacing = 40;
    public GameObject prefab;
    
    void Start()
    {
        Vector3 originalPos = prefab.transform.position;
        
        for (int y = 0; y < size.x; y++)
        {
            for (int x = 0; x < size.y; x++)
            {
                if (!(x == 0 && y == 0)) // if is not original object
                {
                    Vector3 pos = new Vector3(originalPos.x + spacing * x, originalPos.y, originalPos.z + spacing * y);

                    GameObject newObject = Instantiate(prefab, transform, true);
                    newObject.transform.position = pos;
                }
            }
        }
    }
}
