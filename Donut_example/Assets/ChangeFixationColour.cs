using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeFixationColour : MonoBehaviour
{
    public GameObject Sphere;
    public Color color = Color.magenta;
    int Isyellow = 0;

    //void Start()
    //{
    //    Color newColor = Sphere.GetComponent<Renderer>().material.color;
    //    newColor = Color.magenta;
    //    Sphere.GetComponent<Renderer>().material.SetColor("_Color", newColor);

    //}
        public int TurnYellow(int ranInt)
    {


        if (ranInt % 2 == 0)
        {
            Color newColor = Sphere.GetComponent<Renderer>().material.color;
            newColor = Color.yellow;
            Sphere.GetComponent<Renderer>().material.SetColor("_Color", newColor);
            Isyellow = 1;
            return Isyellow;

        }


        else
        {
            Color newColor = Sphere.GetComponent<Renderer>().material.color;
            newColor = Color.magenta;
            Sphere.GetComponent<Renderer>().material.SetColor("_Color", newColor);
            Isyellow = 0;
            return Isyellow;
        }


    }

   
}
