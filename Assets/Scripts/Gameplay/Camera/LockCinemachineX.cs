using Cinemachine;
using UnityEngine;

namespace Gameplay.Camera
{
    [ExecuteInEditMode]
    [SaveDuringPlay]
    [AddComponentMenu("")]
    public class LockCinemachineX : CinemachineExtension
    {
        [Tooltip("Lock the camera's Z position to this value")]
        public float m_XPosition = 10;

        public float m_YPosition = 12;

        protected override void PostPipelineStageCallback(
            CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage,
            ref CameraState state, float deltaTime)
        {
            if (stage == CinemachineCore.Stage.Body)
            {
                var pos = state.RawPosition;
                pos.x = m_XPosition;
                pos.y = m_YPosition;
                state.RawPosition = pos;
            }
        }
    }}