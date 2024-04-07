using UnityEngine;

namespace Infrastructure.Services.Pool
{
    public class ParticleCallback : MonoBehaviour
    {
        private Release _release;

        private void OnParticleSystemStopped()
        {
            _release.Invoke();
        }

        public void SetRelease(Release release) => 
            _release = release;
    }
}