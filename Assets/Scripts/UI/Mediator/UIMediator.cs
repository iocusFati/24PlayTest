using Base.UI.Factory;
using UnityEngine;

namespace UI.Mediator
{
    public class UIMediator : IUIMediator
    {
        private readonly IUIFactory _uiFactory;
        
        private GameObject _startWindow;
        private GameObject _curtain;

        public UIMediator(IUIFactory uiFactory)
        {
            _uiFactory = uiFactory;
        }

        public void BootstrapInitialize() => 
            CreateCurtain();

        public void InitializeOnGameSceneLoaded()
        {
            _startWindow = _uiFactory.CreateStartWindow();
        }

        public void ShowStartWindow() => 
            _startWindow.gameObject.SetActive(true);

        public void HideStartWindow() => 
            _startWindow.gameObject.SetActive(false);

        public void ShowCurtain() => 
            _curtain.gameObject.SetActive(true);

        public void HideCurtain() => 
            _curtain.gameObject.SetActive(false);

        private void CreateCurtain()
        {
            _curtain = _uiFactory.CreateCurtain();

            Object.DontDestroyOnLoad(_curtain.gameObject);
        }
    }
}