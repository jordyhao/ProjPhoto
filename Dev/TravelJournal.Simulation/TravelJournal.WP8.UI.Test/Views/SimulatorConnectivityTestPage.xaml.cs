﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace TravelJournal.WP8.UI.Test.Views
{
    public partial class SimulatorConnectivityTestPage : PageBase
    {
        public SimulatorConnectivityTestPage()
        {
            InitializeComponent();
            ServerConnectivityTester tester = new ServerConnectivityTester();
            tester.ConnectServer((e) => { Test.Text = e.ToString(); });
        }
    }

    public class ServerConnectivityTester
    {
        private Action<bool> resultHandler;

        public void ConnectServer(Action<bool> resultHandler)
        {
            this.resultHandler = resultHandler;
            ServiceReference.ConnectivityTestServiceClient serviceClient = new ServiceReference.ConnectivityTestServiceClient();
            serviceClient.StartTestCompleted += serviceClient_StartTestCompleted;
            serviceClient.StartTestAsync();
        }

        void serviceClient_StartTestCompleted(object sender, ServiceReference.StartTestCompletedEventArgs e)
        {
            resultHandler(e.Result);
        }
    }
}