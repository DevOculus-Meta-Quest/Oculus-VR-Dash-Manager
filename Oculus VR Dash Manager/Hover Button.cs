using System;
using System.Windows.Controls;

namespace OVR_Dash_Manager
{
    internal class Hover_Button
    {
        // Indicates whether the hover button is enabled or not
        public bool Enabled { get; set; } = false;

        // Indicates whether the hover state is active
        public bool Hovering { get; private set; } = false;

        // Stores the time when the hover state started
        public DateTime Hover_Started { get; private set; } = DateTime.Now;

        // Specifies the duration in seconds to activate the hover state
        public int Hovered_Seconds_To_Activate { get; set; } = 5;

        // Action to be executed when hover is complete
        public Action Hover_Complete_Action { get; set; }

        // Progress bar to display hover progress
        public ProgressBar Bar { get; set; }

        // Indicates whether to check SteamVR or not
        public bool Check_SteamVR { get; set; }

        // Resets the hover state and progress bar
        public void Reset()
        {
            Hovering = false;
            Hover_Started = DateTime.Now;
            Bar.Value = 0;
        }

        // Sets the hover state to active and initializes the progress bar
        public void SetHovering()
        {
            Hovering = true;
            Hover_Started = DateTime.Now;
            Bar.Value = 10;
        }

        // Stops the hover state and resets the progress bar
        public void StopHovering()
        {
            Reset();
        }
    }
}