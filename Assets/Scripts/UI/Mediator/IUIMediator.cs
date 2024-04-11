namespace UI.Mediator
{
    public interface IUIMediator
    {
        void InitializeOnGameSceneLoaded();
        void ShowStartWindow();
        void HideStartWindow();
        void ShowCurtain();
        void HideCurtain();
        void BootstrapInitialize();
    }
}