using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace FrontLineDefense.Test
{
    public class ObjectRotaionTest_1 : MonoBehaviour
    {
        public Transform ObjectToFollow;
        public float MoveSpeedMult = 5f;
        public float TurnSpeed = 0.75f;

        private Vector3 _ogRotAngles, _targetAngles;
        private float _xRotationAngle;
        private bool _leftAligned;
        private void Start()
        {
            _ogRotAngles = transform.eulerAngles;
            _targetAngles = _ogRotAngles;

            if (transform.eulerAngles.y > 90f)
                _leftAligned = true;
        }

        // Update is called once per frame
        void Update()
        {
            // RotationTest1();
            // RotationTest2();
            // RotationTest3();
            RotationTest4();
        }

        void RotationTest4()
        {
            Vector3 _SpeedVec = (ObjectToFollow.position - transform.position).normalized;

            if (_leftAligned)
            {
                _xRotationAngle = (Mathf.Atan2(_SpeedVec.y, _SpeedVec.x) * Mathf.Rad2Deg) - 180f;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(new Vector3(_xRotationAngle, -90f, 0)), TurnSpeed * Time.deltaTime);
            }
            else
            {
                _xRotationAngle = Mathf.Atan2(_SpeedVec.y, _SpeedVec.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(new Vector3(_xRotationAngle * -1f, 90f, 0)), TurnSpeed * Time.deltaTime);
            }
        }

        void RotationTest3()
        {
            Vector3 _SpeedVec = (ObjectToFollow.position - transform.position).normalized;

            _SpeedVec.z = 0f;
            Quaternion lookRotation = Quaternion.LookRotation(_SpeedVec, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, TurnSpeed * Time.deltaTime);

            // _xRotationAngle = (Mathf.Atan2(_SpeedVec.y, _SpeedVec.x) * Mathf.Rad2Deg) - 180f;
            // transform.Rotate(Vector3.right, _xRotationAngle * Time.deltaTime);
        }

        void RotationTest2()
        {
            // _targetAngles.x -= 0.5f;

            Vector3 _SpeedVec = (ObjectToFollow.position - transform.position).normalized;
            _xRotationAngle = (Mathf.Atan2(_SpeedVec.y, _SpeedVec.x) * Mathf.Rad2Deg) - 180f;
            _targetAngles = transform.eulerAngles;
            // _targetAngles.z = 0f;

            _targetAngles.x = Mathf.Lerp(_targetAngles.x, _xRotationAngle, TurnSpeed * Time.deltaTime);

            //Something to stop this from freaking out
            if (_targetAngles.x <= -180f)
                _targetAngles.x = 180f;
            else if (_targetAngles.x < 0 && _targetAngles.x > -0.5f)
                _targetAngles.x = -1f;

            transform.eulerAngles = _targetAngles;
        }

        void RotationTest1()
        {
            Vector3 _SpeedVec = (ObjectToFollow.position - transform.position).normalized;

            Quaternion lookRotation;

            // /*
            if (Mathf.Abs(transform.position.z - ObjectToFollow.position.z) <= 1f)
            {
                // Adjust rotation speed based on distance (closer = faster alignment)
                float t = Mathf.Clamp01(1 - (ObjectToFollow.position.z / transform.position.z)); // Closer to 0 = slower, closer to 1 = faster
                float rotationSpeed = Mathf.Lerp(TurnSpeed, 2f, t);

                float xRotationAngle = Mathf.Atan2(_SpeedVec.y, _SpeedVec.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(xRotationAngle * -1f, 90f, 0f), rotationSpeed * Time.deltaTime);      //Not Effective

                // float yRotationAngle = Mathf.Atan2(_SpeedVec.x, _SpeedVec.z) * Mathf.Rad2Deg;
                // transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, yRotationAngle, 0f), _turnSpeed * Time.deltaTime);      //Not Effective

                // float zRotationAngle = Mathf.Atan2(_SpeedVec.y, _SpeedVec.x) * Mathf.Rad2Deg;
                // transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, 0f, zRotationAngle), _turnSpeed * Time.deltaTime);      //Not Effective
            }
            // else if (Mathf.Abs(transform.position.z - ObjectToFollow.position.z) <= 2f)
            // {
            //     float xRotationAngle = Mathf.Atan2(_SpeedVec.y, _SpeedVec.x) * Mathf.Rad2Deg;
            //     transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(xRotationAngle * -1f, 0f, 0f), _turnSpeed * Time.deltaTime);      //Not Effective
            //     transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(transform.eulerAngles.x, 90f, 0f), _turnSpeed * 0.5f * Time.deltaTime);      //Not Effective
            // }
            else
            {
                lookRotation = Quaternion.LookRotation(_SpeedVec, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, TurnSpeed * Time.deltaTime); //Not Effective
            }
            // */

            /*
            if (Mathf.Abs(transform.position.z - ObjectToFollow.position.z) <= 0.1f)
                lookRotation = Quaternion.LookRotation(new Vector3(_SpeedVec.x, _SpeedVec.y, 0f), Vector3.up);
            else
                lookRotation = Quaternion.LookRotation(_SpeedVec, Vector3.up);

            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, _turnSpeed * Time.deltaTime); //Not Effective
            */

            transform.Translate(Vector3.forward * MoveSpeedMult * Time.deltaTime);

            // float zRotationAngle = Mathf.Atan2(_SpeedVec.y, _SpeedVec.x) * Mathf.Rad2Deg;
            // float yRotationAngle = Mathf.Atan2(_SpeedVec.x, _SpeedVec.z) * Mathf.Rad2Deg;
            // transform.rotation = Quaternion.Euler(0f, 0f, zRotationAngle - 180);      //Not Effective

            // transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, 0f, zRotationAngle - 180), _turnSpeed * Time.deltaTime);      //Not Effective
            // transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, yRotationAngle + 90, 0f), _turnSpeed * Time.deltaTime);      //Not Effective

            // transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, yRotationAngle + 90, zRotationAngle - 180), _turnSpeed * Time.deltaTime);      //Not Effective
        }
    }
}
