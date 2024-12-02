// The Projectiles have custom physics
// Wont be able to achieve through normal implementation

// Multiple scripts wont matter as the number of scripts would remain the same, 
// if there is a super-controller script, then it could matter

using UnityEngine;

using FrontLineDefense.Global;

namespace FrontLineDefense.Projectiles
{
    [RequireComponent(typeof(Rigidbody))]
    public class ProjectileBase : MonoBehaviour, IStatComponent
    {
        // private Rigidbody _projectileRb;

        // Reference : Projectile may have its own speed irrespective of plane's speed
        [SerializeField] protected float _SpeedMult = 1f;
        protected Vector3 _SpeedVec;
        protected bool _ReleasedToPool;
        [SerializeField] protected float _Damage = 1f;
        [SerializeField] protected PoolManager.PoolType _PoolToUse;
        // private const float _positionLerpVal = 0.5f;
        // private float _inititalRot;

        private void OnDisable() { _ReleasedToPool = false; }

        // Start is called before the first frame update
        void Start()
        {
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
            //Apply Movement
            transform.position = transform.position + (_SpeedVec * Time.deltaTime);
            // _SpeedVec = _SpeedVec + (new Vector3(0f, UniversalConstants._gravity, 0f) * Time.deltaTime * _ScalePhysics);
        }

        protected virtual void FixedUpdate()
        {
            //Apply Rotation
            //calculate the angle in radians and convert to  degrees 
            float angle = Mathf.Atan2(_SpeedVec.normalized.y, _SpeedVec.normalized.x) * Mathf.Rad2Deg;

            // Apply rotation to the airplace to point in the direction of movement
            transform.rotation = Quaternion.Euler(0, 0, angle - 180);

            // _projectileRb.AddForce(Vector3.right * 15f, ForceMode.Acceleration);

            // transform.position = Vector3.Lerp(transform.position, transform.position + _SpeedVec, _positionLerpVal);
        }

        public void SetStats(in Vector2 initialSpeed)
        {
            // _inititalRot = transform.eulerAngles.z;
            _SpeedVec = new Vector3(initialSpeed.x * _SpeedMult, initialSpeed.y * _SpeedMult, 0f);
        }

        private void OnTriggerEnter(Collider other)
        {
            // Debug.Log($"Hit | Collider : {other.name} | Tag : {other.tag}");
            // gameObject.SetActive(false);
            if (_ReleasedToPool) return;

            _ReleasedToPool = true;
            PoolManager.Instance.ObjectPool[(int)_PoolToUse].Release(gameObject);
            if (other.CompareTag(UniversalConstants.Player))
                other.GetComponent<IStatComponent>().TakeDamage(_Damage);
        }

        // Projectiles are one-shot, so they dont need extra checks for DamageTaken
        public void TakeDamage(float damageTaken)
        {
            if (_ReleasedToPool) return;

            _ReleasedToPool = true;
            PoolManager.Instance.ObjectPool[(int)_PoolToUse].Release(gameObject);
        }
    }
}