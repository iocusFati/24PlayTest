namespace Infrastructure.Services.Input
{
    public class StandaloneInput : InputService
    {
        public override bool CanStartMoving() => 
            UnityEngine.Input.GetMouseButtonDown(0);

        protected override bool IsMoving() => 
            UnityEngine.Input.GetMouseButton(0);

        protected override bool StoppedMoving() => 
            UnityEngine.Input.GetMouseButtonUp(0);
    }
}