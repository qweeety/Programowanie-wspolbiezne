using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Data
{
    public interface IBall : INotifyPropertyChanged
    {
        double X { get; }
        double Y { get; }
        double Radius { get; }
        void Move(double x, double y);
    }

    internal class Ball : IBall
    {
        private double _x;
        private double _y;
        public double Radius { get; }

        public Ball(double x, double y, double radius)
        {
            _x = x;
            _y = y;
            Radius = radius;
        }

        public double X { get => _x; }
        public double Y { get => _y; }

        public void Move(double x, double y)
        {
            _x = x;
            _y = y;
            OnPropertyChanged(nameof(X));
            OnPropertyChanged(nameof(Y));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}