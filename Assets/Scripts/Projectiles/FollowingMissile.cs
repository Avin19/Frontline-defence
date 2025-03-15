// #define OFFSET_ANGLE_TEST

/*
* These will be similar to PlayerBomb in regards of Rigibody mechanics
* FollowingMissiles : These will be slower than the player and follow the player
*/

using FrontLineDefense.Global;
using UnityEngine;

namespace FrontLineDefense.Projectiles
{
    public class FollowingMissile : ProjectileBase
    {
        // private Vector3 _prevSpeedVec;
        // private float _xyOffsetAngle = 5;
        private const float _speedDeltaTimeMult = 7f;

        // protected override void Start()
        // {
        //     base.Start();
        //     _turnSpeed = 13f;
        // }

        protected override void OnDisable()
        {
            base.OnDisable();
            _CurrentSpeedMult = 0f;
        }

        protected override void Update()
        {
            // Vector3 projectedVec = (GameManager.Instance.PlayerTransform.position - transform.position).normalized;
            // _SpeedVec = Vector3.Lerp(_SpeedVec, projectedVec, _turnSpeed);
            _SpeedVec = (GameManager.Instance.PlayerTransform.position - transform.position).normalized;

            // Doesnt work as intended | Missile sometimes slows down and takes a sharp turn
#if OFFSET_ANGLE_TEST
            //Offsetting by certain angle
            /*
            *   |   cosA    -sinA   |
            *   |   sinA    cosA    |
            *   x' = x*cosA - y*sinA; y' = x*sinA + y*cosA;
            */
            _SpeedVec.x -= (_SpeedVec.x * Mathf.Cos(_xyOffsetAngle) - _SpeedVec.y * Mathf.Sin(_xyOffsetAngle));
            _SpeedVec.y -= (_SpeedVec.x * Mathf.Sin(_xyOffsetAngle) + _SpeedVec.y * Mathf.Cos(_xyOffsetAngle));
            // _SpeedVec.y *= _turnSpeed;
#endif

            // if (_CurrentSpeedMult < _SpeedMult)
            // _CurrentSpeedMult += Time.deltaTime * _speedDeltaTimeMult;
            _CurrentSpeedMult = Mathf.Clamp(_CurrentSpeedMult + (Time.deltaTime * _speedDeltaTimeMult), 0, _SpeedMult);

            // _currentSpeedMult += Time.deltaTime * _speedDeltaTimeMult;
            // _currentSpeedMult = Mathf.Clamp(_currentSpeedMult, 1, _SpeedMult);

            // This was for only rotating in the x-y axis as the missile reaches the player
            /*if (Mathf.Abs(transform.position.z - GameManager.Instance.PlayerTransform.position.z) <= 1f)
            {
                float xRotationAngle = Mathf.Atan2(_SpeedVec.y, _SpeedVec.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(xRotationAngle * -1f, 90f, 0f), _turnSpeed * Time.deltaTime);      //Almost Perfect
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(xRotationAngle * -1f, 90f, 0f), _turnSpeed * Time.deltaTime);      //Almost Perfect
            }
            else
            {
                Quaternion lookRotation = Quaternion.LookRotation(_SpeedVec, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, _turnSpeed * Time.deltaTime);
            }*/

            if (_LeftAligned)
            {
                float xRotationAngle = (Mathf.Atan2(_SpeedVec.y, _SpeedVec.x) * Mathf.Rad2Deg) - 180f;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(xRotationAngle, -90f, 0f), _turnSpeed * Time.deltaTime);      //Almost Perfect
            }
            else
            {
                float xRotationAngle = Mathf.Atan2(_SpeedVec.y, _SpeedVec.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(xRotationAngle * -1f, 90f, 0f), _turnSpeed * Time.deltaTime);      //Almost Perfect
            }
            base.Update();
        }
    }
}
