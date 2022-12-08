using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyMathTools;

public class Animate : MonoBehaviour
{
    [SerializeField] float _TranslationSpeed;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Spherical sph = new Spherical(2, Mathf.PI / 2, Mathf.PingPong(Time.time, Mathf.PI));

        transform.position = CoordConvert.SphericalToCartesian(sph);
    }
}
