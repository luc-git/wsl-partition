using System;
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
using System.Management;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection.Metadata;
using System.Windows.Interop;
using System.Reflection;
using System.Threading;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;
using System.Collections.Concurrent;
using System.Runtime.ConstrainedExecution;
using System.Text.RegularExpressions;

namespace wsl_partition
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int WM_DEVICECHANGE = 0x0219;
        private const int DBT_DEVNODES_CHANGED = 0x0007;
        List<Person> items;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            GetList();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            HwndSource? source = PresentationSource.FromVisual(this) as HwndSource;
            source.AddHook(WndProc);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {

            if (msg == WM_DEVICECHANGE)
            {
                if (wParam.ToInt32() == DBT_DEVNODES_CHANGED)
                {
                    GetList();
                }
            }

            return IntPtr.Zero;
        }

        private void GetList()
        {
            using (Runspace runspace = RunspaceFactory.CreateRunspace())
            {
                items = new List<Person>();
                items.Clear();
                list.ItemsSource = items;
                runspace.Open();
                Pipeline pipeline = runspace.CreatePipeline();
                pipeline.Commands.AddScript("Set-ExecutionPolicy -Scope Process -ExecutionPolicy ByPass");
                pipeline.Commands.AddScript("get-partition");
                foreach (PSObject result in pipeline.Invoke())
                {
                    try
                    {
                        if (Convert.ToChar(result.Members["DriveLetter"].Value) == '\0')
                        {
                            if (result.Members["MbrType"].Value == null)
                            {
                                items.Add(new Person()
                                {
                                    FileSystem = GptType.Gpt[Regex.Replace(Convert.ToString(result.Members["GptType"].Value), "[{}]", string.Empty)],
                                    DiskNumber = Convert.ToUInt32(result.Members["DiskNumber"].Value),
                                    PartitionNumber = Convert.ToUInt32(result.Members["PartitionNumber"].Value)
                                });
                            }
                            else
                            {
                                items.Add(new Person()
                                {
                                    FileSystem = MbrType.Mbr[string.Format("0x{0}", Convert.ToString(Convert.ToInt32(result.Members["MbrType"].Value), 16))],
                                    DiskNumber = Convert.ToUInt32(result.Members["DiskNumber"].Value),
                                    PartitionNumber = Convert.ToUInt32(result.Members["PartitionNumber"].Value)
                                });
                            }
                        }
                    }
                    catch (KeyNotFoundException)
                    {

                    }
                }
                runspace.Close();
            }
        }

        class Person
        {
            public UInt32 DiskNumber { get; set; }
            public UInt32 PartitionNumber { get; set; }

            public string? FileSystem { get; set; }

            public string? PartitionSize { get; set; }

            public string? Mounted { get; set; }
        }

        private void Mount_Click(object sender, RoutedEventArgs e)
        {
            Person selectedItem = (Person)list.SelectedItem;
            if (selectedItem != null)
            {
                uint disknumber = selectedItem.DiskNumber;
                uint partitionnumber = selectedItem.PartitionNumber;
                string filesystem = selectedItem.FileSystem;
                if (LaunchCommand("wsl.exe", "--mount \\\\.\\PHYSICALDRIVE" + disknumber + " --partition " + partitionnumber + " -t " + filesystem, true) == String.Empty)
                    return;
                string path = LaunchCommand("wsl.exe", "wslpath -w /mnt/wsl", true);
                LaunchCommand("explorer.exe", path, false);
            }

        }

        private string LaunchCommand(string filename, string args, bool nowindow)
        {
            Process command = new Process();
            command.StartInfo.FileName = filename;
            command.StartInfo.CreateNoWindow = nowindow;
            command.StartInfo.Arguments = args;
            command.StartInfo.RedirectStandardOutput = true;
            command.Start();
            command.WaitForExitAsync();
            string output = command.StandardOutput.ReadToEnd().Replace("\0", string.Empty);
            if (command.ExitCode < 0)
            {
                MessageBox.Show(output, "error", MessageBoxButton.OK, MessageBoxImage.Error);
                return String.Empty;
            }
            return output;
        }

        private void Unmount_Click(object sender, RoutedEventArgs e)
        {
            Person selectedItem = (Person)list.SelectedItem;
            if (selectedItem != null)
            {
                uint disknumber = selectedItem.DiskNumber;
                string output = LaunchCommand("wsl.exe", "--unmount \\\\.\\PHYSICALDRIVE" + disknumber, true);
            }
        }
    }
}