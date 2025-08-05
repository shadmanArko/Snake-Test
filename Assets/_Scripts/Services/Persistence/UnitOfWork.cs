using _Scripts.Services.Persistence.Repositories;

namespace _Scripts.Services.Persistence
{
    public class UnitOfWork
    {
        private readonly DataContext _dataContext;

        private readonly Levels _levels;

        public UnitOfWork(DataContext dataContext, Levels levels)
        {
            _dataContext = dataContext;
            _levels = levels;
        }

        public Levels Levels => _levels;


        public async void Save() => await _dataContext.Save();

    }
}
