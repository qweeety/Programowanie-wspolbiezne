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
        public ICommand StopCommand { get; } // ДОБАВИЛИ КНОПКУ СТОП

        public MainViewModel()
        {
            _logic = LogicAbstractApi.CreateApi();

            StartCommand = new RelayCommand(() => {
                _logic.CreateBalls(7); // Создаем 7 шаров
                Balls.Clear();
                foreach (var b in _logic.GetBalls()) Balls.Add(b);
                _logic.Start(); // Запускаем многопоточность
            });

            StopCommand = new RelayCommand(() => {
                _logic.Stop(); // Глушим потоки
            });
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
    }

    public class RelayCommand : ICommand
    {
        private readonly System.Action _execute;
        public RelayCommand(System.Action execute) => _execute = execute;
        public bool CanExecute(object? parameter) => true; // тут ?
        public void Execute(object? parameter) => _execute(); // и тут ?
        public event System.EventHandler? CanExecuteChanged; // и тут ?
    }
}