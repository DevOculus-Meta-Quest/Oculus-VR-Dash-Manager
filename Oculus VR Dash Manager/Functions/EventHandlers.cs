using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OVR_Dash_Manager.Functions
{
    public class EventHandlers
    {
        private MainWindow _window;

        public EventHandlers(MainWindow window)
        {
            _window = window;
        }

        public void SteamVRStatusChanged()
        {
            //... logic for when SteamVR status changes ...
        }

        //... other event-handling methods ...
    }
}
