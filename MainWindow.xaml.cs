using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace GameFly
{
    public partial class MainWindow : Window
    {
        public class FlyObj
        {
            private double start_speed;
            private double angle;
            private double X0 = 0;
            private double Y0 = 0;
            private double Y = 0;
            private double X = 0;
            private static double time = 0;
            private const double G = 9.80665;
            public FlyObj(double angle = 0, double speed = 0)
            {
                this.X0 = 0;
                this.Y0 = 0;
                this.angle = angle;
                this.start_speed = speed;
            }

            public void Set_Angle(double a) { this.angle = a; }
            public void Set_Speed(double s) { this.start_speed = s; }
            public double Get_Time() { return Math.Round(time,4); }
            public void GetParam()
            {
                string Angl = @"C:\Users\Максим\source\repos\GameFly\Properties\Angle.txt";
                string S_s = @"C:\Users\Максим\source\repos\GameFly\Properties\Speed.txt";

                using (StreamReader sr = new StreamReader(Angl))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                        this.angle = Convert.ToDouble(line);
                }
                using (StreamReader sr = new StreamReader(S_s))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                        this.start_speed = Convert.ToDouble(line);
                }

            }

            public void Position()
            {
                GetParam();
                using (StreamWriter sr = new StreamWriter("C:\\Users\\Максим\\source\\repos\\GameFly\\Properties\\Position.txt"))
                {
                    time = start_speed * 2 * Math.Sin(angle * Math.PI / 180) / G;
                    string str = (this.X + "\n" + this.Y);
                    sr.WriteLine(str);
                    for (double t = 0.1; Y >= 0; t += 0.1)
                    {
                        this.X = Math.Round(start_speed * t * Math.Cos(angle * Math.PI / 180), 4);
                        this.Y = Math.Round((start_speed * t * Math.Sin(angle * Math.PI / 180)) - (G * t * t * 0.50), 4);
                        if (this.Y <= 0) break;
                        str = (this.X + "\n" + this.Y);
                        sr.WriteLine(str);
                    }
                    this.X = Math.Round(start_speed * time * Math.Cos(angle * Math.PI / 180), 4);
                    this.Y = 0;
                    str = (this.X + "\n" + this.Y);
                    sr.Write(str);
                }
            }

            public void Coordinates(List<double> X, List<double> Y)
            {
                string path = @"C:\Users\Максим\source\repos\GameFly\Properties\Position.txt";
                string[] readtext = File.ReadAllLines(path);
                for (int i = 0; i != readtext.Length; i++)
                {
                    if (i % 2 == 0) X.Add((Convert.ToDouble(readtext[i])));
                    else Y.Add((Convert.ToDouble(readtext[i])));
                }
            }
        }


        RadialGradientBrush brush;
        [STAThread]
        public static void Main()
        {
            Application app = new Application();
            app.Run(new MainWindow());
        }
        public MainWindow()
        {
            FlyObj fly = new FlyObj();
            fly.Position();
        }

        

        private void Fly_Click(object sender, RoutedEventArgs e)
        {
            double num;
            List<double> X= new List<double>();
            List<double> Y = new List<double>();
            if (double.TryParse(TextSpeed.Text, out num))
            {
                using (StreamWriter sr = new StreamWriter("C:\\Users\\Максим\\source\\repos\\GameFly\\Properties\\Speed.txt"))
                {
                    sr.WriteLine(num);
                }
            }

            double num2;
            if (double.TryParse(TextAngl.Text, out num2))
            {
                using (StreamWriter sr = new StreamWriter("C:\\Users\\Максим\\source\\repos\\GameFly\\Properties\\Angle.txt"))
                {
                    sr.WriteLine(num2);
                }
            }
            FlyObj flyObj = new FlyObj();
            flyObj.Position();
        }

        private void Draw_Click(object sender, RoutedEventArgs e)
        {
            Line A= new Line();
            Line O = new Line();
            ICanFly.Children.Add(A);
            ICanFly.Children.Add(O);
            A.Margin = new Thickness(25,ICanFly.Height - 20, 0, 25);
            O.Margin = new Thickness(27, ICanFly.Height - 20, 0, 25);
            A.StrokeThickness = 5;
            O.StrokeThickness = 5;
            A.X2 = ICanFly.Width - 25 ;
            O.Y2 = -ICanFly.Height + 61;
            A.Stroke = Brushes.Red;
            O.Stroke = Brushes.Red;
            List<double> X = new List<double>();
            List<double> Y = new List<double>();
            FlyObj temp = new FlyObj();
            temp.Coordinates(X, Y);
            for(int i = 0; i < Y.Count; i ++)
            {
                var c = new Ellipse();
                double left = 25, top = ICanFly.Height - 25, right = 0, bottom = 25;
                c.Width = 8;
                c.Height = 8;
                if (i == Y.Count / 2)
                {
                    c.Fill = Brushes.CadetBlue;
                    c.Stroke = Brushes.Red;
                    Line projection = new Line();
                    ICanFly.Children.Add(projection);
                    projection.Margin = new Thickness(left + X[i] + 2, top - Y[i] + 2, right, bottom);
                    projection.Stroke = Brushes.CadetBlue;
                    projection.StrokeThickness = 2;
                    DoubleCollection collection = new DoubleCollection();
                    collection.Add(0.5);
                    projection.StrokeDashArray = collection;
                    projection.Y2 = Y[i];
                }
                else
                {
                    c.Fill = Brushes.Violet;
                    c.Stroke = Brushes.Violet;
                }
                ICanFly.Children.Add(c);
                c.Margin = new Thickness(left + X[i], top - Y[i], right, bottom);
            }
            Data.Text = "Максимальная высота = " + Convert.ToString(Y[(Y.Count) / 2]) + '\n' +
                            "Время полета = " + temp.Get_Time();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TextAngl.Clear();
            TextSpeed.Clear();
        }
    }
}
