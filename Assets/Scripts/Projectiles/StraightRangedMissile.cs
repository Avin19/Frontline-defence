/*
* These will be similar to PlayerBomb in regards of Rigibody mechanics
* StraightRangedMissiles : These could be slower than the player and shoot upto a distance
*/

using FrontLineDefense.Global;
using UnityEngine;

namespace FrontLineDefense.Projectiles
{
    public class StraightRangedMissile : ProjectileBase
    {
        private Vector2 _playerTargetedPos;
        private const float _maxTargetDiff = 1f;

        protected override void OnDisable()
        {
            base.OnDisable();
            // if (!_GradualSpeedIncrease)
            _CurrentSpeedMult = _SpeedMult;
        }

        public override void SetStats(in Vector3 initialSpeedVec)
        {
            _CurrentSpeedMult = _SpeedMult;
            _turnSpeed = 1f;
            _playerTargetedPos = GameManager.Instance.PlayerTransform.position;
            // _SpeedVec = (GameManager.Instance.PlayerTransform.position - transform.position).normalized;
            _SpeedVec = initialSpeedVec;
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            if (Vector2.SqrMagnitude(transform.position - new Vector3(_playerTargetedPos.x, _playerTargetedPos.y, 0f))
                <= _maxTargetDiff * _maxTargetDiff)
                PoolManager.Instance.ObjectPool[(int)_PoolToUse].Release(gameObject);
        }
    }
}
