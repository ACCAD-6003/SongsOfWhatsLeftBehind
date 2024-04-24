using UnityEngine;

namespace Controller
{
    public class SwapToUIOnStart : MonoBehaviour
    {
        private void Start()
        {
            UIController.Instance.SwapToUI();
        }
    }
}