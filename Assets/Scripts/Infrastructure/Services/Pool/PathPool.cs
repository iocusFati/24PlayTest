using UnityEngine;
using IAssets = Infrastructure.AssetProviderService.IAssets;

namespace Infrastructure.Services.Pool
{
    public class PathPool<TPoolable> : BasePool<TPoolable> where TPoolable : Component
    {
        private readonly string _path;
        
        private readonly IAssets _assets;

        public PathPool(string path, IAssets assets)
        {
            _path = path;
            _assets = assets;
        }

        protected override TPoolable Spawn() => 
            _assets.Instantiate<TPoolable>(_path, _parent);
    }
}