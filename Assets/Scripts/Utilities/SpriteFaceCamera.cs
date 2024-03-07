using System;
using UnityEngine;

namespace Utilities
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteFaceCamera : MonoBehaviour
    {
        private const string RED_CAMERA_TAG = "RedCamera";
        private const string BLUE_CAMERA_TAG = "BlueCamera";
        private const string RED_CAMERA_LAYER = "OnlyRed";
        private const string BLUE_CAMERA_LAYER = "OnlyBlue";
        
        [SerializeField] private PlayerIdentifier player;

        private Camera cameraToFace;

        private void Start()
        {
            cameraToFace = player == PlayerIdentifier.Red
                ? GameObject.FindGameObjectWithTag(RED_CAMERA_TAG).GetComponent<Camera>()
                : GameObject.FindGameObjectWithTag(BLUE_CAMERA_TAG).GetComponent<Camera>();
            gameObject.layer = player == PlayerIdentifier.Red
                ? LayerMask.NameToLayer(RED_CAMERA_LAYER)
                : LayerMask.NameToLayer(BLUE_CAMERA_LAYER);
        }
        
        private void Update()
        {
            var cameraRotation = cameraToFace.transform.rotation;
            transform.LookAt(transform.position + cameraRotation * Vector3.forward,
                cameraRotation * Vector3.up);
        }

        private enum PlayerIdentifier
        {
            Red,
            Blue
        }
    }
}