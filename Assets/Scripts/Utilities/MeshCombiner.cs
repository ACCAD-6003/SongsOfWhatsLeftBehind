using UnityEngine;

namespace Utilities
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshCollider))]
    public class MeshCombiner : MonoBehaviour
    {
        [SerializeField] bool hideChildren = true;
    
        void Start()
        {
            MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
            CombineInstance[] combine = new CombineInstance[meshFilters.Length];

            int i = 0;
            while (i < meshFilters.Length)
            {
                combine[i].mesh = meshFilters[i].sharedMesh;
                combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
                meshFilters[i].gameObject.SetActive(!hideChildren);

                i++;
            }

            Mesh mesh = new Mesh();
            mesh.CombineMeshes(combine);
            transform.GetComponent<MeshFilter>().sharedMesh = mesh;
            transform.GetComponent<MeshCollider>().sharedMesh = mesh;
            transform.gameObject.SetActive(true);
        }
    }
}