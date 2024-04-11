using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows
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