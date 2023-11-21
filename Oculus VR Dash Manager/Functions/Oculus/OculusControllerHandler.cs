using nkast.LibOVR;
using System;

namespace OVR_Dash_Manager.Functions.Oculus
{
    public class OculusControllerHandler
    {
        private OvrClient _ovrClient;
        private OvrSession _ovrSession;
        private Action<string> _updateUiAction;

        public OculusControllerHandler(Action<string> updateUiAction)
        {
            _updateUiAction = updateUiAction;
            InitializeOvrClient();
        }

        private void InitializeOvrClient()
        {
            var result = OvrClient.TryInitialize(out _ovrClient);

            if (result >= 0 && _ovrClient != null)
            {
                _updateUiAction("Oculus client initialized successfully.");
                result = _ovrClient.TryCreateSession(out _ovrSession);

                if (result >= 0)
                {
                    _updateUiAction("Oculus session created successfully.");
                }
                else
                {
                    _updateUiAction("Failed to create Oculus session.");
                }
            }
            else
            {
                _updateUiAction("Failed to initialize Oculus client.");
            }
        }

        public void MonitorController()
        {
            if (_ovrSession != null)
            {
                var connectedControllers = _ovrSession.GetConnectedControllerTypes();
                _updateUiAction($"Connected Controllers: {connectedControllers}");

                var result = _ovrSession.GetInputState(OvrControllerType.Touch, out OvrInputState inputState);

                if (result >= 0)
                {
                    // You can add more details here based on the inputState
                    _updateUiAction($"Button Pressed: {inputState.Buttons}");
                }
                else
                {
                    _updateUiAction("Failed to get controller input state.");
                }
            }
            else
            {
                _updateUiAction("No active Oculus session.");
            }
        }

        public void Cleanup()
        {
            _ovrSession?.Dispose();
            _ovrClient?.Dispose();
        }
    }
}