using System;
using System.Collections.Generic;
using UnityEngine;

namespace Infrastructure
{
    public class Updater : MonoBehaviour, IUpdater
    {
        private readonly List<IUpdatable> _updatables = new();
        private readonly List<IFixedUpdatable> _fixedUpdatables = new();
        
        public void Update()
        {
            foreach (var updatable in _updatables) 
                updatable.Update();
        }

        private void FixedUpdate()
        {
            foreach (var fixedUpdatable in _fixedUpdatables)
            {
                fixedUpdatable.FixedUpdate();
            }
        }

        public void AddUpdatable(IUpdatable updatable)
        {
            _updatables.Add(updatable);
        }

        public void AddFixedUpdatable(IFixedUpdatable fixedUpdatable) => 
            _fixedUpdatables.Add(fixedUpdatable);
    }
}