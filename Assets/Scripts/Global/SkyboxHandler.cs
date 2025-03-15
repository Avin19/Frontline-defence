
using UnityEngine;

namespace FrontLineDefense
{
    public class SkyboxHandler : MonoBehaviour
    {
        [SerializeField] private Material[] skyboxMaterials;
        // Start is called before the first frame update
        [SerializeField] private int x;
        void Start()
        {

            x = Random.Range(0, skyboxMaterials.Length - 1);
            RenderSettings.skybox = skyboxMaterials[x];

        }

    }
}
