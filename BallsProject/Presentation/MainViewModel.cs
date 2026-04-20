using System.Collections.ObjectModel;
using System.Windows.Input;
using Logic;
using Data;

namespace Presentation.ViewModel
{
    public class MainViewModel : System.ComponentModel.INotifyPropertyChanged
    {
        private readonly LogicAbstractApi _logic;
        public ObservableCollection<IBall> Balls { get; } = new();
        public ICommand StartCommand { get; }

        public MainViewModel()
        {
            _logic = LogicAbstractApi.CreateApi();
            StartCommand = new RelayCommand(() => {
                _logic.CreateBalls(5);
                Balls.Clear();
                foreach (var b in _logic.GetBalls()) Balls.Add(b);
                _logic.Start();
            });
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
    }

    public class RelayCommand : ICommand
    {
        private readonly System.Action _execute;
        public RelayCommand(System.Action execute) => _execute = execute;
        public bool CanExecute(object parameter) => true;
        public void Execute(object parameter) => _execute();
        public event System.EventHandler CanExecuteChanged;
    }
}