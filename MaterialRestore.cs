using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialRestore : MonoBehaviour
{
    Material originalMat;
    private int change = 0;
    // Start is called before the first frame update
    void Start()
    {
        originalMat = GetComponent<Renderer>().material;
    }

    public void Restore()
    {
        change = 1;
        GetComponent<Renderer>().material = originalMat;
    }
}