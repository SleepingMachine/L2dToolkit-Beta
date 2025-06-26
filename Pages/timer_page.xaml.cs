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
using System.Threading;
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
        public static int last_cpu_info;
        public static int last_ram_info;

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
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(1000);

            switch (DataOverload.timerState)
            {
                case DataOverload.TIMER_STATUS.INITIAL_STATE:
                    start_button_text.Text = "������ʼ��ʱ";
                    break;

                case DataOverload.TIMER_STATUS.TIMING_STATE:
                    start_button_text.Text = "������ͣ��ʱ";
                    dispatcherTimer.Start();
                    break;

                case DataOverload.TIMER_STATUS.STOP_STATE:
                    start_button_text.Text = "����������ʱ";
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
                    start_button_text.Text = "������ͣ��ʱ";
                    dispatcherTimer.Start();
                    break;

                case DataOverload.TIMER_STATUS.TIMING_STATE:
                    DataOverload.timerState = DataOverload.TIMER_STATUS.STOP_STATE;
                    DataOverload.stopTime = DateTimeOffset.Now;

                    start_button_text.Text = "����������ʱ";
                    cpu_info_text.Text = "0% CPUռ��";
                    ram_info_text.Text = "0% RAMռ��";
                    SystemInfoViewModel.ClearAllItems();

                    dispatcherTimer.Stop();
                    break;

                case DataOverload.TIMER_STATUS.STOP_STATE:
                    DataOverload.timerState = DataOverload.TIMER_STATUS.TIMING_STATE;
                    DateTimeOffset time = DateTimeOffset.Now;
                    TimeSpan span = time - DataOverload.stopTime;
                    DataOverload.startTime = DataOverload.startTime + span;
                    start_button_text.Text = "������ͣ��ʱ";
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
            //Debug.WriteLine($"Mem: {ramCounter.NextValue():F2}%");
            SystemInfoViewModel.AddRamItem((int)App.ramCounter.NextValue());
            SystemInfoViewModel.AddCpuItem((int)App.cpuCounter.NextValue());
            cpu_info_text.Text = (int)App.cpuCounter.NextValue() + "% CPUռ��";
            ram_info_text.Text = (int)App.ramCounter.NextValue() + "% RAMռ��";
        }
    }

    //ͼ����

    public partial class SystemInfoViewModel
    {
        public ObservableCollection<ISeries> Series { get; set; }

        static private ObservableCollection<ObservableValue> ramObservableValues;
        static private ObservableCollection<ObservableValue> cpuObservableValues;

        public SystemInfoViewModel()
        {
            ramObservableValues = new ObservableCollection<ObservableValue> { };
            cpuObservableValues = new ObservableCollection<ObservableValue> { };

            Series = new ObservableCollection<ISeries>
            {
                new LineSeries<ObservableValue>
                {
                    Values = ramObservableValues,
                    Fill = new SolidColorPaint(SKColors.DarkSeaGreen.WithAlpha(90)),
                    Stroke = null,
                    GeometryFill = null,
                    GeometryStroke = null
                },

                new LineSeries<ObservableValue>
                {
                    Values = cpuObservableValues,
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
        static public void AddRamItem(int value)
        {
            ramObservableValues.Add(new(value));
            if (ramObservableValues.Count > 60) 
            {
                ramObservableValues.RemoveAt(0);
            }
        }

        [RelayCommand]
        static public void AddCpuItem(int value)
        {
            cpuObservableValues.Add(new(value));
            if (cpuObservableValues.Count > 60)
            {
                cpuObservableValues.RemoveAt(0);
            }
        }

        static public void ClearAllItems()
        {
            ramObservableValues.Clear();
            cpuObservableValues.Clear();
        }

    }

}
