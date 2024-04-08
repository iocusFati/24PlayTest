using System.Collections.Generic;
using UnityEngine;

namespace Infrastructure.States
{
    public interface IStackedCubes
    {
        List<Transform> Cubes { get; }
    }
}