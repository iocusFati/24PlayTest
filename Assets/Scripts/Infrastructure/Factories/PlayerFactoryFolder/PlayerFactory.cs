using System;
using Infrastructure.AssetProviderService;
using UnityEngine;
using Zenject;

namespace Infrastructure.States
{
    public class PlayerFactory : IPlayerFactory
    {
        private readonly IInstantiator _container;

        public event Action<Player> OnPlayerCreated;

        public PlayerFactory(IInstantiator container)
        {
            _container = container;
        }

        public Player CreatePlayer(Transform at)
        {
            Player player = _container.InstantiatePrefabResourceForComponent<Player>(AssetPaths.Player,
                at.position, at.rotation,
                new GameObject("Holder").transform);

            player.transform.SetParent(null);
            
            OnPlayerCreated.Invoke(player);

            return player;
        }
    }
}