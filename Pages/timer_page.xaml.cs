using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using SkiaSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace L2dToolkit_Beta.Pages
{

    public class DataOverload 
    {
        public enum TIMER_STATUS
        {
            INITIAL_STATE,
            TIMING_STATE,
            STOP_STATE
        };

        public static DateTimeOffset startTime;
        public static DateTimeOffset lastTime;
        public static DateTimeOffset stopTime;
        public static TIMER_STATUS timerState;
        public static TimeSpan overloadSpan; 

        public DataOverload() 
        {
            TIMER_STATUS timerState = TIMER_STATUS.INITIAL_STATE;
        }

    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class timer_page : Page
    {
        DispatcherTimer dispatcherTimer;

        public timer_page()
        {
            this.InitializeComponent();

            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = TimeSpan.FromMicroseconds(1000);

            switch (DataOverload.timerState)
            {
                case DataOverload.TIMER_STATUS.INITIAL_STATE:
                    break;

                case DataOverload.TIMER_STATUS.TIMING_STATE:
                    dispatcherTimer.Start();
                    break;

                case DataOverload.TIMER_STATUS.STOP_STATE:
                    total_duration.Text = DataOverload.overloadSpan.ToString(@"hh\:mm\:ss");
                    total_duration_progressring.Value = DataOverload.overloadSpan.Seconds % 60;
                    dispatcherTimer.Stop();
                    break;

            }
        }

        public void Click_StartTiming_Button(object sender, RoutedEventArgs e) 
        {
            switch (DataOverload.timerState) 
            {
                case DataOverload.TIMER_STATUS.INITIAL_STATE:
                    DataOverload.timerState = DataOverload.TIMER_STATUS.TIMING_STATE;
                    DataOverload.startTime = DateTimeOffset.Now;
                    DataOverload.lastTime = DataOverload.startTime;
                    dispatcherTimer.Start();
                    break;

                case DataOverload.TIMER_STATUS.TIMING_STATE:
                    DataOverload.timerState = DataOverload.TIMER_STATUS.STOP_STATE;
                    DataOverload.stopTime = DateTimeOffset.Now;
                    dispatcherTimer.Stop();
                    break;

                case DataOverload.TIMER_STATUS.STOP_STATE:
                    DataOverload.timerState = DataOverload.TIMER_STATUS.TIMING_STATE;
                    DateTimeOffset time = DateTimeOffset.Now;
                    TimeSpan span = time - DataOverload.stopTime;
                    DataOverload.startTime = DataOverload.startTime + span;
                    dispatcherTimer.Start();
                    break;
            }
        }

        private void dispatcherTimer_Tick(object sender, object e)
        {
            DateTimeOffset time = DateTimeOffset.Now;
            TimeSpan span = time - DataOverload.lastTime;
            DataOverload.lastTime = time;

            span = DataOverload.lastTime - DataOverload.startTime;
            total_duration.Text = span.ToString(@"hh\:mm\:ss");
            total_duration_progressring.Value = span.Seconds % 60;
            DataOverload.overloadSpan = span;
            Get_System_Info();
        }

        public void Get_System_Info()
        {
            //Debug.WriteLine($"CPU: {cpuCounter.NextValue():F2}%");
            //Debug.WriteLine($"Mem: {ramCounter.NextValue():F2}%");
            Debug.WriteLine(App.cpuCounter.NextValue());
            SystemInfoViewModel.AddRamItem(App.ramCounter.NextValue());
        }
    }

    //Õº±Ì¿‡

    public partial class SystemInfoViewModel
    {
        public ObservableCollection<ISeries> Series { get; set; }

        static private ObservableCollection<ObservableValue> ramObservableValues;
        public SystemInfoViewModel()
        {
            ramObservableValues = new ObservableCollection<ObservableValue> { };
            Series = new ObservableCollection<ISeries>
            {
                new LineSeries<ObservableValue>
                {
                    Values = ramObservableValues,
                    Fill = new SolidColorPaint(SKColors.CornflowerBlue.WithAlpha(90)),
                    Stroke = null,
                    GeometryFill = null,
                    GeometryStroke = null
                }

            };
        }

        public Axis[] YAxes { get; set; } = new Axis[]
        {
            new Axis
            {
                MaxLimit = 100,
                MinLimit = 0,
                CrosshairSnapEnabled = true
            }
        };

        public Axis[] XAxes { get; set; } = new Axis[]
        {
            new Axis
            {
                IsVisible = false
            }
        };

        [RelayCommand]
        static public void AddRamItem(float value)
        {
            ramObservableValues.Add(new(value));
            if (ramObservableValues.Count > 100) 
            {
                ramObservableValues.RemoveAt(0);
            }
        }

    }

}
