// The Projectiles have custom physics
// Wont be able to achieve through normal implementation

// Multiple scripts wont matter as the number of scripts would remain the same, 
// if there is a super-controller script, then it could matter

using UnityEngine;

using FrontLineDefense.Global;

namespace FrontLineDefense.Projectiles
{
    [RequireComponent(typeof(Rigidbody))]
    public abstract class ProjectileBase : MonoBehaviour
    {
        // private Rigidbody _projectileRb;

        // Reference : Projectile may have its own speed irrespective of plane's speed
        [SerializeField] protected float _SpeedMult = 1f;
        protected Vector3 _SpeedVec;
        protected bool _ReleasedToPool;
        [SerializeField] protected float _Damage = 1f;
        [SerializeField] protected PoolManager.PoolType _PoolToUse;
        protected float _turnSpeed = 0.75f;
        // [SerializeField] protected bool _GradualSpeedIncrease;
        protected float _CurrentSpeedMult = 1f;
        // private const float _positionLerpVal = 0.5f;
        // private float _inititalRot;
        protected bool _LeftAligned;

        protected virtual void OnDisable() { _ReleasedToPool = false; }

        protected virtual void OnEnable() { }

        // Start is called before the first frame update
        protected virtual void Start()
        {
            // if (!_GradualSpeedIncrease)
            // _CurrentSpeedMult = _SpeedMult;

            // _projectileRb = GetComponent<Rigidbody>();
            // _projectileRb.AddForce(Vector3.right * 10f, ForceMode.Impulse);      //Test
        }

        /*
            The projectile goes forward and changes direction simlutaneously. As it reaches peak, 
            it starts to curve down and proceed downwards.

            |       /        |               |               |
            |      /         |     /         |               |
            |     /          |    /          |   /           |   
            |    ----->      |   ----->      |   ----->      |   ----->
            |                |    \          |   \           |   \ 
            |                |               |    \          |    \        
            |                |               |               |     \
            |___________     |___________    |___________    |___________
            Need to Calculate Curve.
            (-) Move in small Increments
        */
        protected virtual void Update()
        {
            // Apply Movement
            // transform.position = transform.position + (_SpeedVec * Time.deltaTime * _CurrentSpeedMult);

            // Missile with forward rotation
            /*
            // transform.Translate(Vector3.forward * _CurrentSpeedMult * Time.deltaTime);
            // float zRotationAngle = Mathf.Atan2(_SpeedVec.y, _SpeedVec.x) * Mathf.Rad2Deg;
            // transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(zRotationAngle - 180f, -90f, 0f), _turnSpeed * Time.deltaTime);      //Not Effective even when the missile is rotated to move with forward
            */

            // _SpeedVec = _SpeedVec + (new Vector3(0f, UniversalConstants._gravity, 0f) * Time.deltaTime * _ScalePhysics);

            //Missile with left rotation
            /*
            // transform.position = transform.position + (transform.right * -1.0f * Time.deltaTime * _CurrentSpeedMult);
            transform.Translate(Vector3.left * _CurrentSpeedMult * Time.deltaTime);
            float zRotationAngle = Mathf.Atan2(_SpeedVec.y, _SpeedVec.x) * Mathf.Rad2Deg;
            float yRotationAngle = Mathf.Atan2(_SpeedVec.x, _SpeedVec.z) * Mathf.Rad2Deg;
            // transform.rotation = Quaternion.Euler(0f, 0f, zRotationAngle - 180);      //Not Effective
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, 0f, zRotationAngle - 180), _turnSpeed * Time.deltaTime);      //Not Effective
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, yRotationAngle + 90, 0f), _turnSpeed * Time.deltaTime);      //Not Effective
            // transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, yRotationAngle + 90, zRotationAngle - 180), _turnSpeed * Time.deltaTime);      //Not Effective
            */

            // Apply Movement
            transform.Translate(Vector3.forward * _CurrentSpeedMult * Time.deltaTime);

            // transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(_SpeedVec), _turnSpeed * Time.deltaTime);      //Not Effective
            // transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0f, 0f, zRotationAngle), _turnSpeed * Time.deltaTime);      //Does not work
        }

        // private float _rotationAngle;            //For Debugging
        protected virtual void FixedUpdate()
        {
            //Apply Rotation
            //calculate the angle in radians and convert to  degrees 
            // float zRotationAngle = Mathf.Atan2(_SpeedVec.y, _SpeedVec.x) * Mathf.Rad2Deg;
            // _rotationAngle = Mathf.Lerp(_rotationAngle, Mathf.Atan2(_SpeedVec.normalized.y, _SpeedVec.normalized.x) * Mathf.Rad2Deg, _turnSpeed);        //Weird Turning

            // Apply rotation to the airplace to point in the direction of movement
            // transform.rotation = Quaternion.Euler(0, 0, _rotationAngle - 180);
            // transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, 0f, zRotationAngle - 180), _turnSpeed);      //Not Effective

            // _projectileRb.AddForce(Vector3.right * 15f, ForceMode.Acceleration);

            // transform.position = Vector3.Lerp(transform.position, transform.position + _SpeedVec, _positionLerpVal);
        }

        public virtual void SetStats(in Vector3 initialSpeed, in bool leftAligned)
        {
            // _inititalRot = transform.eulerAngles.z;
            _LeftAligned = leftAligned;
            _SpeedVec = new Vector3(initialSpeed.x * _SpeedMult, initialSpeed.y * _SpeedMult, 0f);
        }

        private void OnTriggerEnter(Collider other)
        {
            // Debug.Log($"Hit | Collider : {other.name} | Tag : {other.tag}");
            // gameObject.SetActive(false);
            if (_ReleasedToPool) return;

            _ReleasedToPool = true;
            PoolManager.Instance.ObjectPool[(int)_PoolToUse].Release(gameObject);
            if (other.CompareTag(UniversalConstants.StatComponent))
                other.GetComponent<IStatComponent>().TakeDamage(_Damage);
        }

        // Projectiles are one-shot, so they dont need extra checks for DamageTaken
        /*public void TakeDamage(float damageTaken)
        {
            if (_ReleasedToPool) return;

            _ReleasedToPool = true;
            PoolManager.Instance.ObjectPool[(int)_PoolToUse].Release(gameObject);
        }*/
    }

    public interface TargetingProjectiles
    {
        void SetTargetingStats(in Transform _playerTransform);
    }
}