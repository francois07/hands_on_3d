using UnityEngine;
using MyMathTools;

public class OrbitalCamera : MonoBehaviour
{
    [SerializeField] Spherical _StartSphPos;   // the initial spherical position of the camera, angles are setup in degrees, and converted in radians in the Start method

    Spherical _SphPos;         // the current spherical position

    [SerializeField] Spherical _SphMin;    // the minimum values for the spherical coordinates, merely rho and phi are concerned, theta is not restricted to a specific range
    [SerializeField] Spherical _SphMax;    // the maximum values for the spherical coordinates, merely rho and phi are concerned, theta is not restricted to a specific range
    [SerializeField] Spherical _SphSpeed;  // the spherical speed, rho in m/s, theta in degree/pixel, phi in degree/pixel
    [SerializeField] Spherical _LerpCoefs;

    [SerializeField] Transform _Target;    // the object at the centre of the orbit

    Vector3 _PreviousMousePos;     // previous mouse position, useful to compute the mouse move vector
    Spherical _TargetSphPos;

    void SetSphericalPosition(Spherical sphPos)
    {
        transform.position = _Target.position + CoordConvert.SphericalToCartesian(sphPos);
        transform.LookAt(_Target); // the camera looks at the target object
        _SphPos = sphPos;
    }

    // Start is called before the first frame update
    void Start()
    {
        //Conversion from degrees to radians
        _StartSphPos.Theta *= Mathf.Deg2Rad;
        _StartSphPos.Phi *= Mathf.Deg2Rad;
        _SphMin.Phi *= Mathf.Deg2Rad;
        _SphMax.Phi *= Mathf.Deg2Rad;
        _SphSpeed.Theta *= Mathf.Deg2Rad;
        _SphSpeed.Phi *= Mathf.Deg2Rad;

        // Positions at start
        _SphPos = _StartSphPos;           // spherical position initialization
        _TargetSphPos = _SphPos;          // target position initialization

        SetSphericalPosition(_SphPos);     // camera position initialization

        _PreviousMousePos = Input.mousePosition;   // previous mouse position initialization
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // Please implement the following steps:
        // Let's compute the mouse motion vector (coordinates are in in pixels)
        // 1 - retrieve the current mouse position and store it into a local variable
        // 2 - compute the mouse motion vector by subtracting the previous mouse position (m_PreviousMousePos) from the current mouse position
        // 3 - update the previous mouse position variable with the current mouse position
        Vector3 currentMousePos = Input.mousePosition;
        Vector3 mouseMotion = currentMousePos - _PreviousMousePos;
        _PreviousMousePos = currentMousePos;

        // Let's compute the new spherical position of the camera
        // 4 - compute the spherical displacement of the camera during the frame
        // 5 - compute the new spherical position of the camera, taking into account the min and max limits for rho and phi
        // 6 - assign the new position of the camera by calling the SetSphericalPosition method
        Spherical sphDisplacement = new Spherical(
          Input.mouseScrollDelta.y * _SphSpeed.Rho,
          (Input.GetMouseButton(0) ? (mouseMotion.y * _SphSpeed.Theta) : 0),
          (Input.GetMouseButton(0) ? (mouseMotion.x * _SphSpeed.Phi) : 0)
        );

        if (Input.GetMouseButton(0) || sphDisplacement.Rho != 0)
        {
            _TargetSphPos = new Spherical(
              Mathf.Clamp(_SphPos.Rho + sphDisplacement.Rho, _SphMin.Rho, _SphMax.Rho),
              _SphPos.Theta + sphDisplacement.Theta,
              Mathf.Clamp(_SphPos.Phi + sphDisplacement.Phi, _SphMin.Phi, _SphMax.Phi)
            );
        }

        Spherical newSphPos = new Spherical(
          Mathf.Lerp(_SphPos.Rho, _TargetSphPos.Rho, Time.deltaTime * _LerpCoefs.Rho),
          Mathf.Lerp(_SphPos.Theta, _TargetSphPos.Theta, Time.deltaTime * _LerpCoefs.Theta),
          Mathf.Lerp(_SphPos.Phi, _TargetSphPos.Phi, Time.deltaTime * _LerpCoefs.Phi)
        );

        SetSphericalPosition(newSphPos);

        //Hints:
        // Input.mouseScrollDelta.y gives you the mouse wheel increment during the frame
        // Input.mousePosition gives you the current mouse position on the screen, in pixels
        // Input.GetMouseButton(1) returns true if the right mouse button is hold pressed
        // Mathf.Clamp clamps a value within a range defined by a min and a max
    }
}