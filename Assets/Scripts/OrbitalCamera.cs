using UnityEngine;
using MyMathTools;

public class OrbitalCamera : MonoBehaviour
{
  [SerializeField] Spherical m_StartSphPos;   // the initial spherical position of the camera, angles are setup in degrees, and converted in radians in the Start method
  Spherical m_TargetSphPos;   // the target spherical position
  Spherical m_SphPos;         // the current spherical position

  [SerializeField] Spherical m_SphMin;    // the minimum values for the spherical coordinates, merely Rho and Theta are concerned, Phi is not restricted to a specific range
  [SerializeField] Spherical m_SphMax;    // the maximum values for the spherical coordinates, merely Rho and Theta are concerned, Phi is not restricted to a specific range
  [SerializeField] Spherical m_SphSpeed;  // the spherical speed, Rho in m/s, Phi in degree/pixel, Theta in degree/pixel
  [SerializeField] Spherical m_SphLerpSpeed;  // the lerping coefficients

  [SerializeField] Transform m_Target;    // the object at the centre of the orbit

  Vector3 m_PreviousMousePos;     // previous mouse position, useful to compute the mouse move vector

  void SetSphericalPosition(Spherical sphPos)
  {
    transform.position = m_Target.position + CoordConvert.SphericalToCartesian(sphPos);
    transform.LookAt(m_Target); // the camera looks at the target object
  }

  // Start is called before the first frame update
  void Start()
  {
    //Conversion from degrees to radians
    m_StartSphPos.Phi *= Mathf.Deg2Rad;    // angles are converted to radians
    m_StartSphPos.Theta *= Mathf.Deg2Rad;      // angles are converted to radians

    m_SphMin.Theta *= Mathf.Deg2Rad;
    m_SphMax.Theta *= Mathf.Deg2Rad;

    m_SphMin.Phi *= Mathf.Deg2Rad;
    m_SphMax.Phi *= Mathf.Deg2Rad;

    m_SphSpeed.Phi *= Mathf.Deg2Rad;
    m_SphSpeed.Theta *= Mathf.Deg2Rad;

    // Positions at start
    m_SphPos = m_StartSphPos;           // spherical position initialization
    m_TargetSphPos = m_SphPos;          // target position initialization

    SetSphericalPosition(m_SphPos);     // camera positionin itialization

    m_PreviousMousePos = Input.mousePosition;   // previous mouse position initialization
  }

  // Update is called once per frame
  void LateUpdate()
  {
    //

    // Please implement the following steps:
    // Let's compute the mouse motion vector (coordinates are in in pixels)
    // 1 - retrieve the current mouse position and store it into a local variable
    // 2 - compute the mouse motion vector by subtracting the previous mouse position (m_PreviousMousePos) from the current mouse position
    // 3 - update the previous mouse position variable with the current mouse position

    // Let's compute the new spherical position of the camera
    // 4 - compute the spherical displacement of the camera during the frame
    // 5 - compute the new spherical position of the camera, taking into account the min and max limits for Rho and Theta
    // 6 - assign the new position of the camera by calling the SetSphericalPosition method

    //Hints:
    // Input.mouseScrollDelta.y gives you the mouse wheel increment during the frame
    // Input.mousePosition gives you the current mouse position (in screen reference frame, so the coordinates' unit is in pixels)
    // Input.GetMouseButton(1) returns true if the right mouse button is hold clicked
    // Mathf.Clamp clamps a value within a range defined by a min and a max


    Vector3 currentMousePos = Input.mousePosition;
    Vector3 mouseVect = currentMousePos - m_PreviousMousePos;
    m_PreviousMousePos = currentMousePos;

    m_TargetSphPos.Rho = Mathf.Clamp(m_TargetSphPos.Rho + m_SphSpeed.Rho * Input.mouseScrollDelta.y, m_SphMin.Rho, m_SphMax.Rho);

    if (Input.GetMouseButton(1))
    {
      m_TargetSphPos.Phi = Mathf.Clamp(m_TargetSphPos.Phi + mouseVect.x * m_SphSpeed.Phi, m_SphMin.Phi, m_SphMax.Phi);
      m_TargetSphPos.Theta = Mathf.Clamp(m_TargetSphPos.Theta + mouseVect.y * m_SphSpeed.Theta, m_SphMin.Theta, m_SphMax.Theta);
    }

    // m_SphPos = m_SphPos.Lerp(m_TargetSphPos, new Spherical(m_SphLerpSpeed.Rho * Time.deltaTime, m_SphLerpSpeed.Phi * Time.deltaTime, m_SphLerpSpeed.Theta * Time.deltaTime));

    m_SphPos = new Spherical(
    Mathf.Lerp(m_SphPos.Rho, m_TargetSphPos.Rho, m_SphLerpSpeed.Rho),
    Mathf.Lerp(m_SphPos.Phi, m_TargetSphPos.Phi, m_SphLerpSpeed.Phi),
    Mathf.Lerp(m_SphPos.Theta, m_TargetSphPos.Theta, m_SphLerpSpeed.Theta)
  );

    SetSphericalPosition(m_SphPos);
  }
}