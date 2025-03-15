// #define TARGETING_TEST

using UnityEngine;

using FrontLineDefense.Global;
using FrontLineDefense.Projectiles;

namespace FrontLineDefense.Enemy
{
    public class EnemyMissileCarrier_1 : EnemyVehicleController_Base
    {
        [SerializeField] private float _rotateSpeed;
        [SerializeField] private PoolManager.PoolType _missilePool;
        private const float _maxZRotateAngle = -12f, _minRotateAngle = -178f;
        private const float _alignThreshold = 2f;

        // private const float _searchWaitTime = 2f;

        // public float zRotateAngle;         //Debugging
        // public Vector3 playerDirection;        //Debugging
        protected override void TargetPlayer()
        {
            // if (_ShotProjectileStatus != (byte)ShootStatus.AVAILABLE_TO_SHOOT
            //     && _ShotProjectileStatus != (byte)ShootStatus.RECHARGING) return;
#if !TARGETING_TEST
            // if (_ShotProjectileStatus != (byte)ShootStatus.SEARCHING_PLAYER) return;
            if ((_ShotProjectileStatus & (1 << (int)ShootStatus.SEARCHING_PLAYER)) == 0) return;
#endif

            Vector3 playerDirection = GameManager.Instance.PlayerTransform.position - _Turret.position;
            // _partToRotate.rotation = Quaternion.LookRotation(_playerDirection);

            //calculate the angle in radians and convert to  degrees
            float zRotateAngle = (Mathf.Atan2(playerDirection.y, playerDirection.x) * Mathf.Rad2Deg) - 180f;
            // zRotateAngle = Mathf.Clamp(zRotateAngle, 100f, 150f);
            if (zRotateAngle >= _minRotateAngle && zRotateAngle <= _maxZRotateAngle)
            {
                _Turret.localRotation = Quaternion.Lerp(_Turret.localRotation, Quaternion.Euler(0f, 0f, zRotateAngle), _rotateSpeed * Time.deltaTime);
                // Debug.Log($"Turret Local Euler : {_Turret.eulerAngles.z}");
                // if (Mathf.Abs(_Turret.localEulerAngles.z - zRotateAngle) <= 4f)          //Does not work
                if ((_ShotProjectileStatus & (1 << (int)ShootStatus.FOUND_PLAYER)) == 0
                    && Vector3.Angle(_Turret.right * -1f, playerDirection) <= _alignThreshold)
                {
                    // Debug.Log($"Turret Aligned : {_ShotProjectileStatus}");
                    _ShotProjectileStatus |= 1 << (int)ShootStatus.FOUND_PLAYER;
                }
            }
        }

        protected override async void Shoot()
        {
            // Debug.Log($"Shooting : {_ShotProjectileStatus}");
            GameObject shotProjectile = PoolManager.Instance.ObjectPool[(int)_missilePool].Get();
            shotProjectile.transform.position = _ShootPoint.position;
            shotProjectile.transform.rotation = _ShootPoint.rotation;
            shotProjectile.GetComponent<ProjectileBase>().SetStats(_ShootPoint.right * -1.0f, false);
            shotProjectile.SetActive(true);

            _ShotProjectileStatus = (int)ShootStatus.RECHARGING;
            // Debug.Log($"Recharging : {_ShotProjectileStatus}");
            await _CtTimer.WaitForSeconds(_ShootCooldown);
            // _ShotProjectileStatus = (int)ShootStatus.SEARCHING_PLAYER;
            _ShotProjectileStatus |= 1 << (int)ShootStatus.SEARCHING_PLAYER;
            // Debug.Log($"Searching : {_ShotProjectileStatus}");
            await _CtTimer.WaitForSeconds(_SearchWaitTime);
            // _ShotProjectileStatus = (int)ShootStatus.AVAILABLE_TO_SHOOT;
            _ShotProjectileStatus |= 1 << (int)ShootStatus.AVAILABLE_TO_SHOOT;
            // Debug.Log($"Available : {_ShotProjectileStatus}");
        }
    }
}