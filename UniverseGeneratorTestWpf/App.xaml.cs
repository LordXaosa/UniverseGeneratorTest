using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace UniverseGeneratorTestWpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
        }

        private void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            /*e.Handled = true;
            MessageBox.Show(e.Exception.Message);*/
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            /*e.Handled = true;
            MessageBox.Show(e.Exception.Message);*/
        }
    }
}
