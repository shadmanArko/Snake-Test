using System;
using _Scripts.EventBus;
using CodeMonkey.Utils;
using MainMenu.Model;
using MainMenu.View;
using SceneFlowManagementSystem.Events;
using SoundSystem.Enums;
using SoundSystem.Events;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;

namespace MainMenu.Controller
{
    public class MainMenuController : IMainMenuController, IDisposable
    {
        private readonly IMainMenuModel _model;
        private readonly MainMenuView _view;
        private readonly IEventBus _eventBus;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private readonly UnitOfWork _unitOfWork;

        public MainMenuController(IMainMenuModel model, MainMenuView view, IEventBus eventBus, UnitOfWork unitOfWork)
        {
            _model = model;
            _view = view;
            _eventBus = eventBus;
            _unitOfWork = unitOfWork;
            Initialize();
        }

        public void Initialize()
        {
            _model.CurrentPage
                .Subscribe(pageType => {
                    _model.SetAllPagesInactive(_view.MainMenuPages);
                    _model.SetPageActive(pageType, _view.MainMenuPages, true);
                })
                .AddTo(_disposables);

            _view.PlayButton.ClickFunc += () => _eventBus.Publish(new LoadGameSceneEvent());
            
            _view.QuitButton.ClickFunc += () => _model.QuitApplication();
            
            _view.HowToPlayButton.ClickFunc += () => _model.SetActivePage(MainMenuPageType.HowToPlay);
            
            _view.BackButton.ClickFunc += () => _model.SetActivePage(MainMenuPageType.Home);

            SetupButtonSounds();
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

        public void Dispose()
        {
            _view.PlayButton.ClickFunc -= () => _eventBus.Publish(new LoadGameSceneEvent());
            _view.QuitButton.ClickFunc -= () => _model.QuitApplication();
            _view.HowToPlayButton.ClickFunc -= () => _model.SetActivePage(MainMenuPageType.HowToPlay);
            _view.BackButton.ClickFunc -= () => _model.SetActivePage(MainMenuPageType.Home);
            
            _disposables?.Dispose();
        }
    }
}