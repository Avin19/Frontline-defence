// #define TEST_SHOOT_OFF

using System.Threading;
using UnityEngine;

using FrontLineDefense.Utils;
using FrontLineDefense.Global;

namespace FrontLineDefense.Enemy
{
    public abstract class EnemyVehicleController_Base : MonoBehaviour, IStatComponent
    {
        // Some turrets might rotate a full 180 on z-axis | some will rotate to some 
        // constraints on z-axis and then rotate on y-axis to face the player 
        // [SerializeField] protected Transform _PlayerTransform, _Turret;
        [SerializeField] protected Transform _Turret;
        [SerializeField] protected PoolManager.PoolType _VehiclePoolType;
        [SerializeField] protected float _ShootCooldown, _DetectionRange, _Health;
        [SerializeField] protected Transform _ShootPoint;
        [SerializeField] protected RectTransform _healthBarRect;
        /// <summary> 0: Available to Shoot | 1 : Shot | 2 : Recharging | 3 : Recharging Complete </summary>
        protected byte _ShotProjectileStatus;
        protected bool _ReleasedToPool;
        protected float _OgHealth;
        // private float _shootTime;

        protected CustomTimer _CtTimer;
        protected CancellationTokenSource _Cts;

        protected virtual void OnDisable()
        {
            // _Health = _OgHealth;                //To reset stats if needed
            _Cts.Cancel();
        }

        protected virtual void OnEnable()
        {
            _Cts = new CancellationTokenSource();
        }

        protected virtual void Start()
        {
            _OgHealth = _Health;
            _CtTimer = new CustomTimer();
        }

        protected virtual void FixedUpdate()
        {
            if (Mathf.Abs(GameManager.Instance.PlayerTransform.position.x - transform.position.x) <= _DetectionRange)
            {
                TargetPlayer();

#if !TEST_SHOOT_OFF
                if (!GameManager.Instance.PlayerDead && _ShotProjectileStatus == (byte)ShootStatus.AVAILABLE_TO_SHOOT)
                    Shoot();
#endif
            }

            //Shoot in some intervals
            // We are not looking for perfect time-interval, so this will do to reduce performance cost
            /*if (_shootTime > _ShootCooldown && _ShotProjectileStatus == (byte)ShootStatus.RECHARGE_DONE)
            {
                _ShotProjectileStatus = (byte)ShootStatus.AVAILABLE_TO_SHOOT;
                _shootTime = 0f;
            }
            else
                _shootTime += Time.fixedDeltaTime;
            */
        }

        protected abstract void TargetPlayer();
        protected abstract void Shoot();

        public virtual void TakeDamage(float damageTaken)
        {
            if (_ReleasedToPool) return;

            _Health -= damageTaken;
            _healthBarRect.anchorMax = new Vector2(_Health / _OgHealth, 1f);
            if (_Health <= 0f)
            {
                _ReleasedToPool = true;
                PoolManager.Instance.ObjectPool[(int)_VehiclePoolType].Release(gameObject);
            }
        }
    }
}