using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class SaveManager : IDisposable
{
    private readonly UnitOfWork _unitOfWork;
    private CancellationTokenSource _periodicSaveCts;
    private volatile bool _isSaving;
    private bool _isDisposed;
    private readonly CompositeDisposable _disposables;
    
    private readonly ReactiveProperty<bool> _isSavingReactive = new ReactiveProperty<bool>(false);
    private readonly ReactiveProperty<DateTime> _lastSaveTime = new ReactiveProperty<DateTime>();
    private readonly Subject<Exception> _saveError = new Subject<Exception>();
    
    public SaveManager(UnitOfWork unitOfWork, CompositeDisposable disposables)
    {
        _unitOfWork = unitOfWork;
        _disposables = disposables;
        _periodicSaveCts = new CancellationTokenSource();
        
        _isSavingReactive.AddTo(_disposables);
        _lastSaveTime.AddTo(_disposables);
        _saveError.AddTo(_disposables);
        
        // Set up error logging
        _saveError.Subscribe(ex => Debug.LogError($"Save error: {ex.Message}"))
            .AddTo(_disposables);
    }

    private async UniTask Save()
    {
        if (_isSaving || _isDisposed) return;

        try
        {
            _isSaving = true;
            _isSavingReactive.Value = true;
            
            _unitOfWork.Save();
            _lastSaveTime.Value = DateTime.Now;
        }
        catch (Exception ex)
        {
            _saveError.OnNext(ex);
        }
        finally
        {
            _isSaving = false;
            _isSavingReactive.Value = false;
        }
    }

    public void StartPeriodicSave(int intervalSeconds = 5)
    {
        Observable.Interval(TimeSpan.FromSeconds(intervalSeconds))
            .TakeUntil(Observable.FromEvent(
                h => Application.quitting += h,
                h => Application.quitting -= h))
            .Subscribe(async _ => 
            {
                if (!_isDisposed && !_isSaving)
                {
                    await Save();
                }
            })
            .AddTo(_disposables);
    }

    public void Dispose()
    {
        if (_isDisposed) return;
        
        _isDisposed = true;
        _periodicSaveCts?.Cancel();
        _periodicSaveCts?.Dispose();
        _periodicSaveCts = null;
        
        // Do final save
        Save().Forget();
        
        _disposables.Dispose();
    }
}
