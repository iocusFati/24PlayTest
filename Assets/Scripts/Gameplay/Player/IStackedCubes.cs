using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Infrastructure.States
{
    public interface IStackedCubes
    {
        ReactiveCollection<Transform> Cubes { get; }
    }
}