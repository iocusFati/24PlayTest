namespace Infrastructure.Services.Input
{
    public interface IInputService : IService
    {
        float GetMovement();
        bool CanStartMoving();
    }
}