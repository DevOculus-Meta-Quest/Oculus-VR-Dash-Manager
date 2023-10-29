using OVR_Dash_Manager.Functions;
using System;
using System.Windows;

namespace OVR_Dash_Manager.Forms
{
public partial class OculusView : Window
{
private OculusControllerHandler _oculusHandler;

public OculusView()
{
InitializeComponent();
_oculusHandler = new OculusControllerHandler(UpdateListView);

// Start monitoring when the window is loaded
Loaded += (s, e) => StartMonitoringController();
    }

    private void UpdateListView(string message)
    {
    oculusInfoListView.Items.Add(new { Information = message });
    }

    public void StartMonitoringController()
    {
    _oculusHandler.MonitorController();
    }

    public void StopMonitoringController()
    {
    _oculusHandler.Cleanup();
    }

    // Override the OnClosed method to ensure that the Oculus session is cleaned up properly
    protected override void OnClosed(EventArgs e)
    {
    StopMonitoringController();
    base.OnClosed(e);
    }
    }
    }
