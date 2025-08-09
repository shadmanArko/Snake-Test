using System;
using _Scripts.Entities.MainMenu.Model;
using _Scripts.Entities.MainMenu.View;
using _Scripts.Enums;
using _Scripts.Events;
using _Scripts.Services.EventBus.Core;
using _Scripts.Services.Utils;
using UniRx;
using Zenject;

namespace _Scripts.Entities.MainMenu.Controller
{
    public class MainMenuController : IMainMenuController, IDisposable, IInitializable
    {
        private readonly IMainMenuModel _model;
        private readonly IMainMenuView _view;
        private readonly IEventBus _eventBus;
        private readonly CompositeDisposable _disposables;

        public MainMenuController(IMainMenuModel model, IMainMenuView view, IEventBus eventBus, CompositeDisposable disposables)
        {
            _model = model;
            _view = view;
            _eventBus = eventBus;
            _disposables = disposables;
        }

        public void Initialize()
        {
            BindModelToView();
            SetupButtonActions();
            SetupButtonSounds();
        }
        
        public void Dispose()
        {
            UnsubscribeButtonActions();
        }

        private void BindModelToView()
        {
            _model.CurrentPage
                .Subscribe(pageType =>
                {
                    _model.SetAllPagesInactive(_view.MainMenuPages);
                    _model.SetPageActive(pageType, _view.MainMenuPages, true);
                })
                .AddTo(_disposables);
        }

        private void SetupButtonActions()
        {
            _view.PlayButton.ClickFunc += OnPlayButtonClicked;
            _view.QuitButton.ClickFunc += OnQuitButtonClicked;
            _view.HowToPlayButton.ClickFunc += OnHowToPlayButtonClicked;
            _view.BackButton.ClickFunc += OnBackButtonClicked;
        }

        private void UnsubscribeButtonActions()
        {
            if (_view?.PlayButton != null)
                _view.PlayButton.ClickFunc -= OnPlayButtonClicked;
            
            if (_view?.QuitButton != null)
                _view.QuitButton.ClickFunc -= OnQuitButtonClicked;
            
            if (_view?.HowToPlayButton != null)
                _view.HowToPlayButton.ClickFunc -= OnHowToPlayButtonClicked;
            
            if (_view?.BackButton != null)
                _view.BackButton.ClickFunc -= OnBackButtonClicked;
        }

        private void SetupButtonSounds()
        {
            SetupButtonSound(_view.PlayButton);
            SetupButtonSound(_view.QuitButton);
            SetupButtonSound(_view.HowToPlayButton);
            SetupButtonSound(_view.BackButton);
        }
        
        private void SetupButtonSound(Button_UI button)
        {
            if (button == null) return;
    
            button.MouseOverOnceFunc += () => _eventBus.Publish(new PlaySfxEvent { ClipName = SoundClipName.ButtonOver });
            button.ClickFunc += () => _eventBus.Publish(new PlaySfxEvent { ClipName = SoundClipName.ButtonClick });
        }

        private void OnPlayButtonClicked()
        {
            _eventBus.Publish(new LoadGameSceneEvent());
        }

        private void OnQuitButtonClicked()
        {
            _model.QuitApplication();
        }

        private void OnHowToPlayButtonClicked()
        {
            _model.SetActivePage(MainMenuPageType.HowToPlay);
        }

        private void OnBackButtonClicked()
        {
            _model.SetActivePage(MainMenuPageType.Home);
        }
    }
}