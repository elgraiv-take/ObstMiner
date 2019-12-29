using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;
using Elgraiv.ObstMiner;

namespace ObstMinerGui
{
    class MainWindowViewModel:ViewModelBase
    {

        public ICommand LockingProcessCommand { get; }
        public ICommand UpdateTableCommand { get; }

        private string _fileName;

        public ObservableCollection<ProcessInfo> Processes { get; }
        public ObservableCollection<string> Ports { get; }
        public string CheckingFileName
        {
            get => _fileName;
            set => SetProperty(ref _fileName, value);
        }

        public MainWindowViewModel()
        {
            LockingProcessCommand = new DelegateCommand(LockingProcess);
            UpdateTableCommand = new DelegateCommand(UpdateTable);
            Processes = new ObservableCollection<ProcessInfo>();
            Ports = new ObservableCollection<string>();
        }

        private void UpdateTable()
        {
            var checker = new PortChecker();
            checker.UpdateTable();

            var ports = checker.UsedPorts;
            foreach(var port in ports)
            {
                try
                {
                    var name = Process.GetProcessById(port.ProcessId).ProcessName;
                    Ports.Add($":{port.Port.ToString().PadRight(5)} - {name}");
                }
                catch (ArgumentException)
                {
                    continue;
                }
            }


        }

        private void LockingProcess()
        {
            Processes.Clear();
            using (var checker=new FileChecker())
            {
                var result = checker.Setup(_fileName);
                if (!result)
                {
                    return;
                }
                var processes = checker.GetLockingProcesses();

                foreach(var process in processes)
                {
                    Processes.Add(process);
                }

            }
        }
    }
}
