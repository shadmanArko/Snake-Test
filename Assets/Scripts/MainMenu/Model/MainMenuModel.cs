using System;
using MainMenu.View;
using UniRx;
using UnityEngine;

namespace MainMenu.Model
{
    public class MainMenuModel : IMainMenuModel, IDisposable
    {
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
    
        private readonly ReactiveProperty<MainMenuPageType> _currentPage = 
            new ReactiveProperty<MainMenuPageType>(MainMenuPageType.Home);

        public IReadOnlyReactiveProperty<MainMenuPageType> CurrentPage => _currentPage;

        public MainMenuModel()
        {
            Initialize();
        }
        
        public void QuitApplication()
        {
            Application.Quit();
        }

        public void Initialize()
        {
            // Start with Home page active
            SetActivePage(MainMenuPageType.Home);
        }

        public void SetActivePage(MainMenuPageType pageType)
        {
            var previousPage = _currentPage.Value;
        
            if (previousPage != pageType)
            {
                _currentPage.Value = pageType;
            }
        }

        public bool IsPageActive(MainMenuPageType pageType)
        {
            return _currentPage.Value == pageType;
        }

        public void Dispose()
        {
            _disposables?.Dispose();
            _currentPage?.Dispose();
        }
    }
}