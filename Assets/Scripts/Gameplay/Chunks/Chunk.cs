using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Gameplay.Level
{
    public class Chunk : MonoBehaviour
    {
        [SerializeField] private Transform _end;
        [SerializeField] private List<GameObject> _pickups;

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

        private void OnEnable()
        {
            foreach (var pickup in _pickups) 
                pickup.gameObject.SetActive(true);
        }
    }
}