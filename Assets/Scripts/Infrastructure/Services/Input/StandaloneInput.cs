namespace Infrastructure.Services.Input
{
    public class StandaloneInput : InputService
    {
        protected override bool IsMoving() => 
            UnityEngine.Input.GetMouseButton(0);

        protected override bool CanStartMoving() => 
            UnityEngine.Input.GetMouseButtonDown(0);
    }
}