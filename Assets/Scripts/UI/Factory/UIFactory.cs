using Infrastructure.AssetProviderService;
using UI.Windows;
using UnityEngine;
using Zenject;
using IAssets = Infrastructure.AssetProviderService.IAssets;

namespace Base.UI.Factory
{
    public class UIFactory : IUIFactory
    {
        private readonly IAssets _assets;

        private Canvas _gameRoot;

        [Inject]
        public UIFactory(IAssets assets)
        {
            _assets = assets;
        }

        public void CreateGameUIRoot() => 
            _gameRoot = CreateUIRoot("GameRoot");
        
        public LostPopUp CreateLostPopUp() => 
            CreateUIEntity<LostPopUp>(AssetPaths.LostPopUp);

        public GameObject CreateStartWindow() => 
            CreateUIEntity<GameObject>(AssetPaths.StartWindow);

        public GameObject CreateCurtain()
        {
            GameObject curtain = _assets.Instantiate<GameObject>(AssetPaths.Curtain);
            curtain.SetActive(false);

            return curtain;
        }

        public ScreenFader CreateScreenFader() => 
            CreateUIEntity<ScreenFader>(AssetPaths.ScreenFader);

        private TEntity CreateUIEntity<TEntity>(string path, Canvas parent = null) where TEntity : Object
        {
            SetParentIfNull();

            TEntity entity = _assets.Instantiate<TEntity>(path, parent.transform);

            return entity;

            void SetParentIfNull()
            {
                if (parent is not null) 
                    return;

                if (_gameRoot == null) 
                    CreateGameUIRoot();
                
                parent = _gameRoot;
            }
        }

        private Canvas CreateUIRoot(string name, int order = 0)
        {
            Canvas canvas = _assets.Instantiate<Canvas>(AssetPaths.UIRoot);
            canvas.name = name;
            canvas.sortingOrder = order;

            return canvas;
        }
    }
}