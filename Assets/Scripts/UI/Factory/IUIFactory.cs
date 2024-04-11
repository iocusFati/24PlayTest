using Infrastructure.Services;
using Infrastructure.States;
using UI.Windows;
using UnityEngine;

namespace Base.UI.Factory
{
    public interface IUIFactory : IService
    {
        LostPopUp CreateLostPopUp();
        ScreenFader CreateScreenFader();
        GameObject CreateStartWindow();
        GameObject CreateCurtain();
    }
}