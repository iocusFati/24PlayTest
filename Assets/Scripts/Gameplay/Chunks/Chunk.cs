using UnityEngine;
using Utils;

namespace Gameplay.Level
{
    public class Chunk : MonoBehaviour
    {
        [SerializeField] private Transform _end;

        private UniqueId _uniqueId;
        
        private string _id;

        public string Id
        {
            get
            {
                if (_id is null or "")
                {
                    if (_uniqueId is null)
                    {
                        _uniqueId = GetComponent<UniqueId>();
                    }

                    Id = _uniqueId.Id;
                }

                return _id;
            }
            private set => 
                _id = value;
        }

        public Vector3 End => _end.position;

        private void Awake()
        {
            _uniqueId = GetComponent<UniqueId>();
            
            Id = _uniqueId.Id;
        }
    }
}