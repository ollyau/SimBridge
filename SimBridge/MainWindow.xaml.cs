﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SimBridge
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //for (var i = 0; i < 20; i++)
            //{
            //    var tailnumber = NNumber.Generate();
            //    Log.Instance.Debug($"{tailnumber}: {NNumber.Phonetic(tailnumber)}");
            //}

            var cfg = new Settings("settings.json");

            //var fs = new FlightSimulator();
            //fs.Initialize();

            var sim = new MockSimulatorImpl();

            var mqtt = new MQTTMessageProvider(sim);
            Task.Run(() => mqtt.Initialize(cfg));
        }
    }
}
