using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
  [SerializeField] float translationSpeed_;
  [SerializeField] float rotationSpeed_;
  //   Rigidbody rb_;

  void Awake()
  {
    // rb_ = gameObject.GetComponent<Rigidbody>();
  }

  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {
    // Kinematic Behaviors
    float vAxis = Input.GetAxisRaw("Vertical");
    float hAxis = Input.GetAxisRaw("Horizontal");
    float jumpAxis = Input.GetAxisRaw("Jump");

    Vector3 moveVect =
    (translationSpeed_ * Time.deltaTime)
    * (vAxis * transform.forward + hAxis * transform.right + jumpAxis * transform.up);
    transform.Translate(moveVect, Space.World);

    float rotAngle = hAxis * rotationSpeed_ * Time.deltaTime;
    transform.Rotate(transform.up, rotAngle);
  }

  /*
    void FixedUpdate()
    {
      float vAxis = Input.GetAxisRaw("Vertical");
      float hAxis = Input.GetAxisRaw("Horizontal");

      // Teleportation mode

      Vector3 moveVect = vAxis * transform.forward * Time.fixedDeltaTime * translationSpeed_;
       Vector3 newPos = rb_.position + moveVect;
       rb_.MovePosition(newPos); 
       Quaternion qRot = Quaternion.AngleAxis(hAxis * rotationSpeed_ * Time.fixedDeltaTime, transform.up);
       Quaternion qFinalOrient = qRot * transform.rotation;

       rb_.MoveRotation(qFinalOrient); 

      Vector3 targetVelocity = transform.forward * vAxis * Time.fixedDeltaTime * translationSpeed_;
      Vector3 velocityChange = targetVelocity - rb_.velocity;

      rb_.AddForce(velocityChange, ForceMode.VelocityChange);

      Vector3 targetAngularVelocity = hAxis * rotationSpeed_ * transform.up * Time.fixedDeltaTime;
      Vector3 angularVelocityChange = targetAngularVelocity - rb_.angularVelocity;

      rb_.AddTorque(angularVelocityChange, ForceMode.VelocityChange);
    }
  */

}
