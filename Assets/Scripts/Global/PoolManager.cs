using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace FrontLineDefense.Global
{
    public class PoolManager : MonoBehaviour
    {
        public enum PoolType
        {
            BOMB, FOLLOWING_MISSILES, STRAIGHT_RANGED_MISSILES, AA_BULLET, SMISSILE_SHOOTER,
            FMISSILE_SHOOTER, ENEMY_AA, PLAYER_BULLET, COIN
        }

        [System.Serializable]
        internal class Pools
        {
            public PoolType _poolType;
            public GameObject _prefab;
        }

        #region Singleton
        private static PoolManager _instance;
        public static PoolManager Instance { get => _instance; }

        private void Awake()
        {
            if (_instance == null)
                _instance = this;
            else
                Destroy(gameObject);
        }
        #endregion Singleton

        [SerializeField] Pools[] _poolsToMake;
        public List<ObjectPool<GameObject>> ObjectPool;

        private void Start()
        {
            ObjectPool = new List<ObjectPool<GameObject>>();

            InitializePools();
        }

        private void InitializePools()
        {
            ObjectPool<GameObject> poolToMake;
            for (int i = 0; i < _poolsToMake.Length; i++)
            {
                GameObject poolParent = new GameObject(_poolsToMake[i]._poolType.ToString());
                poolParent.transform.SetParent(transform);

                int tempPoolIndex = i;              //Avoid wrong indexing
                poolToMake = new ObjectPool<GameObject>(
                    createFunc: () => CreatePoolObject(_poolsToMake[tempPoolIndex]._poolType, poolParent.transform),
                    // actionOnGet: ReturnPooledObject,
                    actionOnRelease: ReleasePoolObject,
                    defaultCapacity: 10,
                    maxSize: 15
                );

                ObjectPool.Add(poolToMake);
            }
        }

        private GameObject CreatePoolObject(PoolType poolType, Transform poolParent)
        {
            GameObject poolObject = Instantiate(_poolsToMake[(int)poolType]._prefab, poolParent);
            poolObject.name = $"{poolType}_{ObjectPool[(int)poolType].CountAll}";
            poolObject.SetActive(false);
            return poolObject;
        }

        private void ReturnPooledObject(GameObject poolObject)
        {
            poolObject.SetActive(true);
        }

        private void ReleasePoolObject(GameObject poolObject)
        {
            poolObject.SetActive(false);

            /*
            string comparisonName = poolObject.name.Substring(0, poolObject.name.IndexOf('_'));
            PoolType poolToReleaseFrom;

            try
            {
                if (System.Enum.TryParse<PoolType>(comparisonName, out poolToReleaseFrom))
                {
                    // ObjectPool[(int)poolToReleaseFrom].Release(poolObject);              //Bruh
                    poolObject.SetActive(false);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Pooltype : [{comparisonName}] not found!! | Error : {ex.Message}");
            }
            */
            // string.Equals(comparisonName.Substring(0, comparisonName.IndexOf('_')), PoolType.BOMB.ToString());
        }
    }
}