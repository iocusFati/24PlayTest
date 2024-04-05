using System;
using Infrastructure.Services;
using UnityEngine;

namespace Infrastructure.States
{
    public interface IPlayerFactory : IService
    {
        event Action<Player> OnPlayerCreated;
        
        Player CreatePlayer(Transform at);
    }
}