using System;
using UnityEngine;
using Utils;

namespace Gameplay.Level
{
    public class Chunk : MonoBehaviour
    {
        [SerializeField] private Transform _end;

        private string _id;
        public string Id
        {
            get
            {
                if (_id is null or "")
                {
                    UniqueId uniqueId = GetComponent<UniqueId>();

                    Id = uniqueId.Id;

                    return _id;
                }

                return _id;
            }
            private set => 
                _id = value;
        }

        public Vector3 End => _end.position;
    }
}