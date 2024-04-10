using UnityEngine;

namespace Utilities
{
    public class DestroyMe : MonoBehaviour
    {
        public void Destroy()
        {
            Destroy(gameObject);
        }
    }
}