using UnityEngine;

using FrontLineDefense.Global;
using FrontLineDefense.Projectiles;

namespace FrontLineDefense.Enemy
{
    public class EnemyAA : EnemyVehicleController_Base
    {
        [SerializeField] private float _rotateSpeed;
        private const float _maxZRotateAngle = -12f, _minRotateAngle = -178f;
        private float _shootTime;
        private const float _alignThreshold = 15f;

        // float zRotateAngle;         //Debugging
        protected override void TargetPlayer()
        {
            Vector3 playerDirection = GameManager.Instance.PlayerTransform.position - _Turret.position;

            //calculate the angle in radians and convert to  degrees
            float zRotateAngle = (Mathf.Atan2(playerDirection.y, playerDirection.x) * Mathf.Rad2Deg) - 180f;
            if (zRotateAngle >= _minRotateAngle && zRotateAngle <= _maxZRotateAngle)
            {
                _Turret.localRotation = Quaternion.Lerp(_Turret.localRotation, Quaternion.Euler(0f, 0f, zRotateAngle), _rotateSpeed * Time.deltaTime);
                //Can make a condition to check if the turret is facing the player and shoot then only
                // But the player wont just appear on top of the vehicle, so no need for now

                if ((_ShotProjectileStatus & (1 << (int)ShootStatus.FOUND_PLAYER)) == 0
                    && Vector3.Angle(_Turret.right * -1f, playerDirection) <= _alignThreshold)
                {
                    // Debug.Log($"Turret Aligned : {_ShotProjectileStatus}");
                    _ShotProjectileStatus |= 1 << (int)ShootStatus.FOUND_PLAYER;
                }
            }
        }

        protected override void Shoot()
        {
            _ShotProjectileStatus = 0;
            _ShotProjectileStatus |= 1 << (int)ShootStatus.SHOT_PROJECTILE;
            _shootTime = 0f;
            // Debug.Log($"SHooting | ShootTime : {_shootTime}");
            GameObject shotProjectile = PoolManager.Instance.ObjectPool[(int)PoolManager.PoolType.AA_BULLET].Get();
            shotProjectile.transform.position = _ShootPoint.position;
            shotProjectile.transform.rotation = _ShootPoint.rotation;
            // Debug.Log($"SHooting Before ACtive| Instantiated Pos : {shotProjectile.transform.position}");
            shotProjectile.SetActive(true);
            // Debug.Log($"SHooting After Active| Instantiated Pos : {shotProjectile.transform.position}");
            // shotProjectile.GetComponent<ProjectileBase>().SetStats(_Turret.right * -1.0f);
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            //Shoot in some intervals
            // We are not looking for perfect time-interval, so this will do to reduce performance cost
            if (_shootTime > _ShootCooldown)
            {
                // Debug.Log($"Shoot Available | CoolDown : {_ShootCooldown} | _shootTime : {_shootTime}");
                _ShotProjectileStatus |= 1 << (int)ShootStatus.AVAILABLE_TO_SHOOT;
            }
            else
            {
                // _ShotProjectileStatus = 0;
                // _ShotProjectileStatus &= ~(1 << (int)ShootStatus.AVAILABLE_TO_SHOOT);
                _shootTime += Time.deltaTime;
                // Debug.Log($"Recharging After| CoolDown : {_ShootCooldown} | _shootTime : {_shootTime}");
            }
        }
    }
}