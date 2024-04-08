using UnityEngine;

namespace Infrastructure.Services.Input
{
    public abstract class InputService : IInputService
    {
        private Vector2 _mousePosition1;
        private Vector2 _mousePosition2;
        private float _magnitudeBetweenPositions;
        private float _lastDisplacement;

        public float GetMovement()
        {
            if (StoppedMoving())
            {
                _lastDisplacement = 0;
                Debug.Log("Stopped");
                return 0;
            }

            if (CanStartMoving())
            {
                _mousePosition1 = UnityEngine.Input.mousePosition;
            }

            if (IsMoving())
            {
                _mousePosition2 = UnityEngine.Input.mousePosition;
                float currentDisplacement = _mousePosition2.x - _mousePosition1.x;

                if (currentDisplacement != 0)
                {
                    if (_lastDisplacement == 0)
                    {
                        Debug.Log("Last");
                    }
                    float result = currentDisplacement - _lastDisplacement;
                    _lastDisplacement = currentDisplacement;

                    return result;
                }

                _lastDisplacement = 0;
            }


            return 0;
        }


        public abstract bool CanStartMoving();
        protected abstract bool IsMoving();
        protected abstract bool StoppedMoving();
    }
}