using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Utils;

namespace Infrastructure.States
{
    public class PlayerCollisions
    {
        private readonly PlayerStack _playerStack;
        
        private readonly Dictionary<GameObject, CompositeDisposable> _subscribedCubes = new();

        public PlayerCollisions(PlayerStack playerStack)
        {
            _playerStack = playerStack;
            _playerStack.OnStacked += SubscribeToStackedCube;
            _playerStack.OnRemoved += UnsubscribeFromStackedCube;
        }

        private void UnsubscribeFromStackedCube(GameObject cube)
        {
            _subscribedCubes[cube].Clear();
            _subscribedCubes.Remove(cube);
        }

        private void SubscribeToStackedCube(GameObject cube)
        {
            CompositeDisposable disposable = new CompositeDisposable();
            _subscribedCubes.Add(cube, disposable);
            
            cube.OnCollisionEnterAsObservable()
                .Where(other => other.gameObject.CompareTag(Tags.Wall))
                .Subscribe(other =>
                {
                    _playerStack.RemoveCube(cube);
                })
                .AddTo(disposable);
        }
    }
}