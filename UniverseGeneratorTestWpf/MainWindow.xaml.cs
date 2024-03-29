﻿using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UniverseGeneratorTestWpf.ViewModels;
using UniverseGeneratorTestWpf.Views.Components;

namespace UniverseGeneratorTestWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        ZoomableCanvas MyCanvas;
        Point LastMousePosition;
        private void ZoomableCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            // Store the canvas in a local variable since x:Name doesn't work.
            MyCanvas = (ZoomableCanvas)sender;
        }

        private void ZoomableCanvas_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            var position = e.GetPosition(MyListBox);
            if (e.LeftButton == MouseButtonState.Pressed
                && !(e.OriginalSource is Thumb)) // Don't block the scrollbars.
            {
                //CaptureMouse();
                MyCanvas.Offset -= position - LastMousePosition;
                //e.Handled = true;
            }
            else
            {
                //ReleaseMouseCapture();
            }
            LastMousePosition = position;
        }

        private void ZoomableCanvas_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var x = Math.Pow(2, e.Delta / 3.0 / Mouse.MouseWheelDeltaForOneLine);
            MyCanvas.Scale *= x;

            // Adjust the offset to make the point under the mouse stay still.
            var position = (Vector)e.GetPosition(MyListBox);
            MyCanvas.Offset = (Point)((Vector)
                (MyCanvas.Offset + position) * x - position);

            e.Handled = true;
        }

        private void ZoomableCanvas_KeyDown(object sender, KeyEventArgs e)
        {
            /*MainViewModel mvm = (MainViewModel)DataContext;
            if (mvm.SelectedSector != null)
            {
                if (e.Key == Key.Up)
                {
                    if (mvm.SelectedSector.Node.NorthGate == null)
                        e.Handled = true;
                    if (mvm.SelectedSector.Node.NorthGate != null && !mvm.SelectedSector.NorthGate.IsRevealed)
                    {
                        mvm.SelectedSector.Node.NorthGate.IsRevealed = true;
                        e.Handled = true;
                    }
                }
                if (e.Key == Key.Down)
                {
                    if (mvm.SelectedSector.SouthGate == null)
                        e.Handled = true;
                    if (mvm.SelectedSector.SouthGate != null && !mvm.SelectedSector.SouthGate.IsRevealed)
                    {
                        mvm.SelectedSector.SouthGate.IsRevealed = true;
                        e.Handled = true;
                    }
                }
                if (e.Key == Key.Left)
                {
                    if (mvm.SelectedSector.WestGate == null)
                        e.Handled = true;
                    if (mvm.SelectedSector.WestGate != null && !mvm.SelectedSector.WestGate.IsRevealed)
                    {
                        mvm.SelectedSector.WestGate.IsRevealed = true;
                        e.Handled = true;
                    }
                }
                if (e.Key == Key.Right)
                {
                    if (mvm.SelectedSector.EastGate == null)
                        e.Handled = true;
                    if (mvm.SelectedSector.EastGate != null && !mvm.SelectedSector.EastGate.IsRevealed)
                    {
                        mvm.SelectedSector.EastGate.IsRevealed = true;
                        e.Handled = true;
                    }
                }
            }*/
        }
    }
}
