using Infrastructure.Services;
using UnityEngine;

namespace Infrastructure.States
{
    public interface IPlayerFactory : IService
    {
        Player CreatePlayer(Transform at);
    }
}