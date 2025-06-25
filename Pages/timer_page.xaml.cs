using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
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
            dispatcherTimer.Interval = TimeSpan.FromMicroseconds(500);

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
            Debug.WriteLine(App.ramCounter.NextValue());
        }
    }

    //Õº±Ì¿‡

    public partial class SystemInfoViewModel
    {
        private readonly Random _random = new();

        // We use the ObservableCollection class to let the chart know 
        // when a new item is added or removed from the chart. 
        public ObservableCollection<ISeries> Series { get; set; }

        // The ObservablePoints property is an ObservableCollection of ObservableValue 
        // it means that the chart is listening for changes in this collection 
        // and also for changes in the properties of each element in the collection 
        public ObservableCollection<ObservableValue> ObservableValues { get; set; }

        public SystemInfoViewModel()
        {
            ObservableValues = [
            ];

            Series = [
                new LineSeries<ObservableValue>(ObservableValues)
            ];
        }

        [RelayCommand]
        public void AddItem()
        {
            var randomValue = _random.Next(1, 10);

            // the new value is added to the collection 
            // the chart is listening, and will update and animate the change 

            ObservableValues.Add(new() { Value = randomValue });
        }

        [RelayCommand]
        public void RemoveItem()
        {
            if (ObservableValues.Count == 0) return;

            // the last value is removed from the collection 
            // the chart is listening, and will update and animate the change 

            ObservableValues.RemoveAt(0);
        }

        [RelayCommand]
        public void UpdateItem()
        {
            var randomValue = _random.Next(1, 10);
            var lastItem = ObservableValues[ObservableValues.Count - 1];

            // becase lastItem is an ObservableObject and implements INotifyPropertyChanged 
            // the chart is listening for changes in the Value property 
            // and will update and animate the change 

            lastItem.Value = randomValue;
        }

        [RelayCommand]
        public void ReplaceItem()
        {
            var randomValue = _random.Next(1, 10);
            var randomIndex = _random.Next(0, ObservableValues.Count - 1);

            // replacing and item also triggers the chart to update and animate the change 

            ObservableValues[randomIndex] = new(randomValue);
        }

        [RelayCommand]
        public void AddSeries()
        {
            var values = Enumerable.Range(0, 3)
                .Select(_ => _random.Next(0, 10))
                .ToArray();

            // a new line series is added to the chart 

            Series.Add(new LineSeries<int>(values));
        }

        [RelayCommand]
        public void RemoveSeries()
        {
            if (Series.Count == 1) return;

            // the last series is removed from the chart 

            Series.RemoveAt(Series.Count - 1);
        }
    }

}
