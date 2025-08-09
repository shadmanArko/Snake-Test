using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Entities.MainMenu.DataClasses;
using _Scripts.Enums;
using UniRx;
using UnityEngine;
using Zenject;

namespace _Scripts.Entities.MainMenu.Model
{
    public class MainMenuModel : IMainMenuModel, IDisposable, IInitializable
    {
        private readonly CompositeDisposable _disposables;
        private readonly ReactiveProperty<MainMenuPageType> _currentPage = new(MainMenuPageType.Home);

        public IReadOnlyReactiveProperty<MainMenuPageType> CurrentPage => _currentPage;

        public MainMenuModel(CompositeDisposable disposables)
        {
            _disposables = disposables;
        }
        
        public void Initialize()
        {
            SetActivePage(MainMenuPageType.Home);
        }

        public void SetActivePage(MainMenuPageType pageType)
        {
            if (_currentPage.Value != pageType)
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
            page?.gameObject?.SetActive(isActive);
        }

        public void SetAllPagesInactive(List<MainMenuPage> mainMenuPages)
        {
            if (mainMenuPages == null) return;

            foreach (var page in mainMenuPages.Where(page => page?.gameObject != null))
            {
                page.gameObject.SetActive(false);
            }
        }

        public void QuitApplication()
        {
            Application.Quit();
        }

        public void Dispose()
        {
            _currentPage?.Dispose();
        }
        
        private MainMenuPage GetPageByType(MainMenuPageType pageType, List<MainMenuPage> mainMenuPages)
        {
            return mainMenuPages?.FirstOrDefault(page => page.type == pageType);
        }
    }
}