public static class InputManager
{
    public static Controls InputActions
    {
        get
        {
            if (_inputActions == null)
            {
                _inputActions = new Controls();
                _inputActions.Enable();
            }
            return _inputActions;
        }
    }

    private static Controls _inputActions;
}