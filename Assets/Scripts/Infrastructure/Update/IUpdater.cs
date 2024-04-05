using Infrastructure.States;

namespace Infrastructure
{
    public interface IUpdater
    {
        void AddUpdatable(IUpdatable updatable);
        void AddFixedUpdatable(IFixedUpdatable fixedUpdatable);
    }
}