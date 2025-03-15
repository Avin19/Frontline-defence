// #define TARGETING_TEST

using UnityEngine;

using FrontLineDefense.Global;
using FrontLineDefense.Projectiles;

namespace FrontLineDefense.Enemy
{
    public class EnemyMissileCarrier_2 : EnemyVehicleController_Base
    {
        [SerializeField] private float _rotateSpeed;
        [SerializeField] private PoolManager.PoolType _missilePool;
        [SerializeField] private Transform[] _shootPoints;
        private byte _currentShootPointIndex;
        private const float _alignThreshold = 2f;
        private const float _minYAngle = 90f, _maxYAngle = 270f;
        // private const float _searchWaitTime = 2f;

        protected override void OnDisable()
        {
            base.OnDisable();
            _currentShootPointIndex = 0;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            //Fill the shoot points with missiles

        }

        // public float yRotateAngle;         //Debugging
        // public Vector3 playerDirection;        //Debugging
        protected override void TargetPlayer()
        {
            // if (_ShotProjectileStatus != (byte)ShootStatus.AVAILABLE_TO_SHOOT
            //     && _ShotProjectileStatus != (byte)ShootStatus.RECHARGING) return;
#if !TARGETING_TEST
            // if (_ShotProjectileStatus != (byte)ShootStatus.SEARCHING_PLAYER) return;
            if ((_ShotProjectileStatus & (1 << (int)ShootStatus.SEARCHING_PLAYER)) == 0) return;
#endif


            // _Turret.localRotation = Quaternion.Lerp(_Turret.localRotation,
            //             Quaternion.LookRotation(new Vector3(playerDirection.x, playerDirection.y, 0f), Vector3.up),
            //             _rotateSpeed * Time.fixedDeltaTime);

            // Debug.Log($"Turret Euler : {_Turret.eulerAngles} | yRotateAngle : {yRotateAngle} | Diff : {_Turret.eulerAngles.y - yRotateAngle}");

            if ((_ShotProjectileStatus & (1 << (int)ShootStatus.FOUND_PLAYER)) == 0
            && (_ShotProjectileStatus & (1 << (int)ShootStatus.AVAILABLE_TO_SHOOT)) != 0
            // && ((yRotateAngle * -1f) - _Turret.eulerAngles.y) <= _alignThreshold)
            && (Mathf.Abs(_minYAngle - _Turret.eulerAngles.y) <= 1f || Mathf.Abs(_Turret.eulerAngles.y - _maxYAngle) <= 1f))
            // && Vector3.Angle(_Turret.right * -1f, playerDirection) <= _alignThreshold)
            {
                // Debug.Log($"Turret Aligned : {_ShotProjectileStatus} | Euler Value : {_Turret.eulerAngles.y}");
                _ShotProjectileStatus |= 1 << (int)ShootStatus.FOUND_PLAYER;
                return;
            }

            Vector3 playerDirection = (GameManager.Instance.PlayerTransform.position - _Turret.position).normalized;
            // _partToRotate.rotation = Quaternion.LookRotation(_playerDirection);

            //calculate the angle in radians and convert to  degrees
            float yRotateAngle = (Mathf.Atan2(playerDirection.z, playerDirection.x) * Mathf.Rad2Deg) + 90f;
            _Turret.localRotation = Quaternion.Lerp(_Turret.localRotation, Quaternion.Euler(-30f, yRotateAngle, 0f), _rotateSpeed * Time.fixedDeltaTime);
            // _Turret.localRotation = Quaternion.Euler(0f, yRotateAngle, -30f);
        }

        protected override async void Shoot()
        {
            GameObject shotProjectile = PoolManager.Instance.ObjectPool[(int)_missilePool].Get();
            shotProjectile.transform.position = _ShootPoint.position;
            // Debug.Log($"Shoot Point | Local Euler Angle : {_ShootPoint.localEulerAngles} | Euler Angle : {_ShootPoint.eulerAngles}");

            bool leftAligned = false;
            if (_ShootPoint.eulerAngles.y > 150)
                leftAligned = true;
            shotProjectile.transform.rotation = _ShootPoint.rotation;

            shotProjectile.GetComponent<ProjectileBase>().SetStats(_ShootPoint.right * -1.0f, leftAligned);
            shotProjectile.SetActive(true);

            _ShotProjectileStatus = (byte)ShootStatus.RECHARGING;
            await _CtTimer.WaitForSeconds(_ShootCooldown);
            _ShotProjectileStatus |= 1 << (int)ShootStatus.SEARCHING_PLAYER;
            await _CtTimer.WaitForSeconds(_SearchWaitTime);
            _ShotProjectileStatus |= 1 << (int)ShootStatus.AVAILABLE_TO_SHOOT;
        }
    }
}