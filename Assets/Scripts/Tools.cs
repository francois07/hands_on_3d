using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tools : MonoBehaviour
{
  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {

  }
}

namespace MyMathTools
{
  [System.Serializable]
  public struct Polar
  {
    public float Rho;
    public float Theta;

    public Polar(Polar pol)
    {
      this.Rho = pol.Rho;
      this.Theta = pol.Theta;
    }

    public Polar(float rho, float theta)
    {
      this.Rho = rho;
      this.Theta = theta;
    }
  }

  [System.Serializable]
  public struct Cylindrical
  {
    public float Rho;
    public float Theta;
    public float y;

    public Cylindrical(Cylindrical cyl)
    {
      this.Rho = cyl.Rho;
      this.Theta = cyl.Theta;
      this.y = cyl.y;
    }

    public Cylindrical(float rho, float theta, float y)
    {
      this.Rho = rho;
      this.Theta = theta;
      this.y = y;
    }
  }

  [System.Serializable]
  public struct Spherical
  {
    public float Rho;
    public float Theta;
    public float Phi;

    public Spherical(Spherical sph)
    {
      this.Rho = sph.Rho;
      this.Theta = sph.Theta;
      this.Phi = sph.Phi;
    }

    public Spherical(float rho, float theta, float phi)
    {
      this.Rho = rho;
      this.Theta = theta;
      this.Phi = phi;
    }

    public static Spherical operator +(Spherical s1, Spherical s2)
    {
      return new Spherical(s1.Rho + s2.Rho, s1.Theta + s2.Theta, s1.Phi + s2.Phi);
    }
  }

  public static class CoordConvert
  {
    public static Vector2 PolarToCartesian(Polar polar)
    {
      return polar.Rho * new Vector2(Mathf.Cos(polar.Theta), Mathf.Sin(polar.Theta));
    }

    public static Polar CartesianToPolar(Vector2 cart, bool keepThetaPositive = true)
    {
      Polar polar = new Polar(cart.magnitude, 0);
      if (Mathf.Approximately(polar.Rho, 0)) polar.Theta = 0;
      else
      {
        polar.Theta = Mathf.Asin(cart.y / polar.Rho);
        if (cart.x < 0) polar.Theta = Mathf.PI - polar.Theta;
        if (keepThetaPositive && polar.Theta < 0) polar.Theta += 2 * Mathf.PI;
      }
      return polar;
    }

    public static Vector3 CylindricalToCartesian(Cylindrical cyl)
    {
      return new Vector3(cyl.Rho * Mathf.Cos(cyl.Theta), cyl.y, cyl.Rho * Mathf.Sin(cyl.Theta));
    }

    public static Vector3 SphericalToCartesian(Spherical sph)
    {
      float r = sph.Rho;
      float theta = sph.Theta;
      float phi = sph.Phi;

      return new Vector3(
        r * Mathf.Cos(phi) * Mathf.Sin(theta),
        r * Mathf.Sin(theta) * Mathf.Sin(phi),
        r * Mathf.Cos(theta));
    }

    public static Spherical CartesianToSpherical(Vector3 car)
    {
      float rho = Mathf.Sqrt(car.x * car.x + car.y * car.y + car.z * car.z);
      float theta;
      float phi;

      if (rho == 0)
      {
        theta = 0;
        phi = 0;
      }
      else
      {
        theta = Mathf.Atan2(car.y, car.x);
        phi = Mathf.Acos(car.z / rho);
      }

      return new Spherical(rho, theta, phi);
    }
  }
}