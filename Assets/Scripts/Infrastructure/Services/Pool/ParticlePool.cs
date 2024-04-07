using System;
using UnityEngine;
using Zenject;
using IAssets = Infrastructure.AssetProviderService.IAssets;

namespace Infrastructure.Services.Pool
{
    public class ParticlePool : PathPool<ParticleSystem>
    {
        public ParticlePool(string path, IAssets assets) : base(path, assets)
        {
        }

        protected override ParticleSystem Spawn()
        {
            ParticleSystem particleSystem = base.Spawn();
            ParticleCallback particleCallback = particleSystem.GetComponent<ParticleCallback>();
            
            particleSystem.transform.SetParent(_parent);
            
            particleCallback.SetRelease(() => Release(particleSystem));
            
            return particleSystem;
        }
    }
}