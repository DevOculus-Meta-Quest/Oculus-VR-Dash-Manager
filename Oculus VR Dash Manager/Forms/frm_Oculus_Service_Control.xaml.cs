﻿using System;
using System.ServiceProcess;
using System.Timers;
using System.Windows;

namespace OVR_Dash_Manager.Forms
{
    /// <summary>
    /// Interaction logic for frm_Oculus_Service_Control.xaml
    /// </summary>
    public partial class frm_Oculus_Service_Control : Window
    {
        // OVRLibraryService
        // OVRService

        public frm_Oculus_Service_Control() => InitializeComponent();

        void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ClearLabels();

            Timer_Functions.CreateTimer("Oculus Service Checker", TimeSpan.FromSeconds(5), CallCheckServices);
            Timer_Functions.StartTimer("Oculus Service Checker");

            CheckServices();
        }

        void ClearLabels()
        {
            lbl_LibaryServer_Startup.Content = "";
            lbl_LibaryServer_State.Content = "";
            lbl_RuntimeServer_Startup.Content = "";
            lbl_RuntimeServer_State.Content = "";
        }

        void CallCheckServices(object sender, ElapsedEventArgs args)
        {
            Functions_Old.DoAction(this, new Action(delegate () { CheckServices(); }));
        }

        void CheckServices()
        {
            lbl_LibaryServer_Startup.Content = Service_Manager.GetStartup("OVRLibraryService");
            lbl_LibaryServer_State.Content = Service_Manager.GetState("OVRLibraryService");
            lbl_RuntimeServer_Startup.Content = Service_Manager.GetStartup("OVRService");
            lbl_RuntimeServer_State.Content = Service_Manager.GetState("OVRService");
        }

        void btn_Libary_Server_Manual_Click(object sender, RoutedEventArgs e)
        {
            Service_Manager.Set_Manual_Startup("OVRLibraryService");
            CheckServices();
        }

        void btn_Libary_Server_Automatic_Click(object sender, RoutedEventArgs e)
        {
            Service_Manager.Set_Automatic_Startup("OVRLibraryService");
            CheckServices();
        }

        void btn_Runtime_Server_Manual_Click(object sender, RoutedEventArgs e)
        {
            Service_Manager.Set_Manual_Startup("OVRService");
            CheckServices();
        }

        void btn_Runtime_Server_Automatic_Click(object sender, RoutedEventArgs e)
        {
            Service_Manager.Set_Automatic_Startup("OVRService");
            CheckServices();
        }

        bool Running(ServiceControllerStatus Status)
        {
            switch (Status)
            {
                case ServiceControllerStatus.Running:
                    return true;

                case ServiceControllerStatus.Stopped:
                    return false;

                case ServiceControllerStatus.Paused:
                    return true;

                case ServiceControllerStatus.StopPending:
                    return true;

                case ServiceControllerStatus.StartPending:
                    return true;

                default:
                    return false;
            }
        }

        void btn_Libary_Server_Stop_Click(object sender, RoutedEventArgs e)
        {
            Service_Manager.StopService("OVRLibraryService");
            CheckServices();
        }

        void btn_Libary_Server_Start_Click(object sender, RoutedEventArgs e)
        {
            Service_Manager.StartService("OVRLibraryService");
            CheckServices();
        }

        void btn_Runtime_Server_Stop_Click(object sender, RoutedEventArgs e)
        {
            Service_Manager.StopService("OVRService");
            CheckServices();
        }

        void btn_Runtime_Server_Start_Click(object sender, RoutedEventArgs e)
        {
            Service_Manager.StartService("OVRService");
            CheckServices();
        }

        void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Timer_Functions.StopTimer("Oculus Service Checker");
            Timer_Functions.DisposeTimer("Oculus Service Checker");
        }
    }
}