using System;
using Base.UI.Entities.Windows;
using UnityEngine;
using UnityEngine.UI;

namespace Infrastructure.States
{
    public class LostPopUp : Window
    {
        [SerializeField] private Button _replayButton;
        
        public event Action OnReplayButtonClicked;

        private void Awake()
        {
            _replayButton.onClick.AddListener(() => OnReplayButtonClicked?.Invoke());
        }
    }
}