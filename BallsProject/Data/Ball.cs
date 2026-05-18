using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;

namespace Data
{
    public interface IBall : INotifyPropertyChanged
    {
        double X { get; }
        double Y { get; }
        double Radius { get; }
        double Diameter { get; }
        double Mass { get; }      // ДОБАВИЛИ МАССУ
        double Vx { get; set; }   // Вектор скорости X
        double Vy { get; set; }   // Вектор скорости Y
        void Move();              // Теперь двигается сам на основе Vx и Vy
    }

    internal class Ball : IBall
    {
        private double _x;
        private double _y;
        public double Radius { get; }
        public double Diameter => Radius * 2;
        public double Mass { get; }
        public double Vx { get; set; }
        public double Vy { get; set; }

        public Ball(double x, double y, double radius, double mass, double vx, double vy)
        {
            _x = x;
            _y = y;
            Radius = radius;
            Mass = mass;
            Vx = vx;
            Vy = vy;
        }

        public double X { get => _x; }
        public double Y { get => _y; }

        public void Move()
        {
            _x += Vx;
            _y += Vy;
            OnPropertyChanged(nameof(X));
            OnPropertyChanged(nameof(Y));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}