using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Modern_Enigma
{
    /// <summary>
    /// UserControl1.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Prompt : UserControl
    {
        Process process;
        ProcessStartInfo psi;
        List<string> buffer = new List<string>();

        public Prompt()
        {
            InitializeComponent();
            psi = new ProcessStartInfo()
            {
                CreateNoWindow = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                FileName = "cmd.exe",
                UseShellExecute = false,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8
            };
            process = Process.Start(psi);
            process.OutputDataReceived += OutputDataReceived;
            process.ErrorDataReceived += ErrorDataReceived;
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            InitializeConsoleAsync();
        }
        private async void InitializeConsoleAsync()
        {
            await Task.Delay(100);
            ConsoleWrite("cd C:\\Windows\\System32 && chcp 65001");
            ConsoleWrite("cd " + Environment.CurrentDirectory);
            await Task.Delay(200);
            ConsoleWrite("cls");
            await Task.Delay(100);
            Veil.Visibility = Visibility.Collapsed;
        }

        private void UpdateUI()
        {
            while (buffer.Count > 0)
            {
                Output.Text += buffer[0] + Environment.NewLine;
                buffer.RemoveAt(0);
            }
            Output.ScrollToEnd();
        }

        void ConsoleWrite(string s)
        {
            if (s.ToLower() == "cls")
            {
                Output.Text = "";
                return;
            }
            process.StandardInput.WriteLine(s);
        }

        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                EnterCommand();
            }
        }

        private void Enter_Click(object sender, RoutedEventArgs e)
        {
            EnterCommand();
        }

        private void EnterCommand()
        {
            if (!string.IsNullOrEmpty(Input.Text))
            {
                ConsoleWrite(Input.Text);
                Input.Clear();
            }
        }

        private async void OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                buffer.Add(e.Data);
                await Dispatcher.InvokeAsync(UpdateUI);
            }
        }

        void ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                buffer.Add(e.Data);
            }
        }
    }
}
