namespace OVR_Dash_Manager.Functions
{
    public class EventHandlers
    {
        MainWindow _window;

        public EventHandlers(MainWindow window) => _window = window;

        public void SteamVRStatusChanged()
        {
            // ... logic for when SteamVR status changes ...
        }

        // ... other event-handling methods ...
    }
}