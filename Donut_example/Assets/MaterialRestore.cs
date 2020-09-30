using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialRestore : MonoBehaviour
{
    Material originalMat;

    // Start is called before the first frame update
    void Start()
    {
        originalMat = GetComponent<Renderer>().material;
    }

    public void Restore()
    {
        GetComponent<Renderer>().material = originalMat;
    }
}