using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Entities.MainMenu.DataClasses;
using _Scripts.Entities.MainMenu.Enums;
using UniRx;
using UnityEngine;

namespace _Scripts.Entities.MainMenu.Model
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
        
        public void SetPageActive(MainMenuPageType pageType, List<MainMenuPage> mainMenuPages, bool isActive)
        {
            var page = GetPageByType(pageType, mainMenuPages);
            
            if (page?.gameObject != null)
            {
                page.gameObject.SetActive(isActive);
            }
        }

        public void SetAllPagesInactive(List<MainMenuPage> mainMenuPages)
        {
            foreach (var page in mainMenuPages)
            {
                if (page?.gameObject != null)
                {
                    page.gameObject.SetActive(false);
                }
            }
        }
        
        private MainMenuPage GetPageByType(MainMenuPageType pageType, List<MainMenuPage> mainMenuPages)
        {
            return mainMenuPages?.FirstOrDefault(page => page.type == pageType);
        }

        public void Dispose()
        {
            _disposables?.Dispose();
            _currentPage?.Dispose();
        }
    }
}