using System;
using System.Windows.Controls;
using System.Windows;


namespace OVR_Dash_Manager
{
    public class Hover_Button
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

        public bool IsHovering()
        {
            // This method should return true if the button is in a hovering state.
            // If you have a property that tracks whether the button is hovering, return that.
            // Otherwise, you might need to implement some logic to determine whether the button is hovering.
            return Hovering;
        }

        public void UpdateHoverState()
        {
            // This method should update the hover state of the button.
            // If the button has been hovering long enough to trigger the action, trigger it and reset the hover state.
            // Otherwise, update the progress bar to reflect the elapsed time.

            if ((DateTime.Now - Hover_Started).TotalSeconds >= Hovered_Seconds_To_Activate)
            {
                // If the hover has been active long enough, trigger the action and reset the hover state.
                Hover_Complete_Action.Invoke();
                Reset();
            }
            else
            {
                // If the hover has not been active long enough, update the progress bar.
                Bar.Value = (DateTime.Now - Hover_Started).TotalSeconds * 1000; // Update progress bar
            }
        }

        public void CheckHovering()
        {
            if (Hovering)
            {
                if (Check_SteamVR)
                {
                    if (!Properties.Settings.Default.Ignore_SteamVR_Status_HoverButtonAction)
                    {
                        if (!Software.Steam.Steam_VR_Server_Running)
                            return;
                    }
                }

                TimeSpan Passed = DateTime.Now.Subtract(Hover_Started);

                // Ensure that the UI update is performed on the UI thread
                Application.Current.Dispatcher.Invoke(() => Bar.Value = Passed.TotalMilliseconds);

                if (Passed.TotalSeconds >= Hovered_Seconds_To_Activate)
                {
                    Reset();
                    Bar.Value = Bar.Maximum;
                    Hover_Complete_Action.Invoke();
                }
            }
        }


    }
}