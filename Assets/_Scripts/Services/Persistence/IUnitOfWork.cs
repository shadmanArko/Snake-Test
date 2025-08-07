using _Scripts.Services.Persistence.Repositories;

namespace _Scripts.Services.Persistence
{
    public interface IUnitOfWork
    {
        Levels Levels { get; }
        void Save();
    }
}