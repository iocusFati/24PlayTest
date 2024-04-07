using UnityEngine;

namespace Gameplay.Camera
{
    public class BillBoardEffect : MonoBehaviour
    {
        private Transform mainCamera;

        private void Start()
        {
            mainCamera = UnityEngine.Camera.main.transform;
        }

        private void Update()
        {
            transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
                mainCamera.transform.rotation * Vector3.up);
        }

    }
}