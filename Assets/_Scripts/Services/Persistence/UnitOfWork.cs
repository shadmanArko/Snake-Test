using System;
using _Scripts.Services.Persistence.Repositories;
using UnityEngine;
using Zenject;

namespace _Scripts.Services.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _dataContext;

        private readonly Levels _levels;

        public UnitOfWork(DataContext dataContext, Levels levels)
        {
            _dataContext = dataContext;
            _levels = levels;
        }

        public Levels Levels => _levels;


        public async void Save()
        {
            try
            {
                await _dataContext.Save();
            }
            catch (Exception e)
            {
                Debug.LogError($"Unable to save data: {e}");
            }
        }
        
    }
}
