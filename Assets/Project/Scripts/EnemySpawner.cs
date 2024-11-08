using UnityEngine;



public class EnemySpawner : MonoBehaviour
{
    // List of models that need to generate
    // Random selecting one gameobject
    [SerializeField] private GameObject[] enemys;

    private void Start()
    {
        Instantiate(enemys[Random.Range(0, enemys.Length)], transform.position, Quaternion.identity);
    }

}


