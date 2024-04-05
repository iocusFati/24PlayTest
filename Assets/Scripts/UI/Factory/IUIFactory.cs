using Infrastructure.Services;
using Infrastructure.States;

namespace Base.UI.Factory
{
    public interface IUIFactory : IService
    {
        void CreateGameUIRoot();
        HUD CreateHUD();
        LostPopUp CreateLostPopUp();
        ScreenFader CreateScreenFader();
    }
}