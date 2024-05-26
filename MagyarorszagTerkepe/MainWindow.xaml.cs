using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MagyarorszagTerkepe
{
    public partial class MainWindow : Window
    {
        double[] x_ek = new double[3137];
        double[] y_ok = new double[3137];
        List<(int, int)> utHalozat = new List<(int, int)>();

        public MainWindow()
        {
            InitializeComponent();
            StreamReader file = new StreamReader("helysegek_koordinatai.csv");
            int db = 0;
            file.ReadLine();
            while (!file.EndOfStream)
            {
                string sor = file.ReadLine();
                string[] reszek = sor.Split(';'); //név;x;y
                string[] fpszp = reszek[1].Split(new char[] { ':', '.' });
                double x = int.Parse(fpszp[0]);
                x += (double.Parse(fpszp[1]) / 60);
                x += (double.Parse(fpszp[2]) / 6000);
                x_ek[db] = x;
                fpszp = reszek[2].Split(new char[] { ':', '.' });
                double y = int.Parse(fpszp[0]);
                y += (double.Parse(fpszp[1]) / 60);
                y += (double.Parse(fpszp[2]) / 6000);
                y_ok[db] = y;
                db++;
            }
            file.Close();

            for (int i = 0; i < db; i++)
            {
                List<(int, double)> tavolsagok = new List<(int, double)>();

                for (int j = 0; j < db; j++)
                {
                    if (i != j)
                    {
                        double tavolsag = Math.Sqrt(Math.Pow(x_ek[i] - x_ek[j], 2) + Math.Pow(y_ok[i] - y_ok[j], 2));
                        tavolsagok.Add((j, tavolsag));
                    }
                }

                tavolsagok.Sort((a, b) => a.Item2.CompareTo(b.Item2));

                for (int k = 0; k < 4; k++)
                {
                    utHalozat.Add((i, tavolsagok[k].Item1));
                }
            }
        }

        private void Terkep(object sender, MouseButtonEventArgs e)
        {
            Rajzolas();
        }

        private void Vonal_MouseUp(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show($"{((Rectangle)sender).Margin}");
        }

        private void Atmeretezes(object sender, SizeChangedEventArgs e)
        {
            Rajzolas();
        }

        void Rajzolas()
        {
            Rajzlap.Children.Clear();
            Rajzlap.Width = this.ActualWidth;
            Rajzlap.Height = this.ActualHeight - 20;
            double xmeret = Rajzlap.Width / 7;
            double ymeret = Rajzlap.Height / 3;

            for (int i = 0; i < x_ek.Length; i++)
            {
                Rectangle vonal = new Rectangle
                {
                    Width = 7,
                    Height = 7,
                    Margin = new Thickness((x_ek[i] - 16) * xmeret + 3, (48.7 - y_ok[i]) * ymeret, 0, 0),
                    Fill = GetColorByLatitude(y_ok[i])
                };
                vonal.MouseUp += Vonal_MouseUp;
                Rajzlap.Children.Add(vonal);
            }

            foreach (var ut in utHalozat)
            {
                int index1 = ut.Item1;
                int index2 = ut.Item2;

                Line line = new Line
                {
                    X1 = (x_ek[index1] - 16) * xmeret + 3 + 3.5,
                    Y1 = (48.7 - y_ok[index1]) * ymeret + 3.5,
                    X2 = (x_ek[index2] - 16) * xmeret + 3 + 3.5,
                    Y2 = (48.7 - y_ok[index2]) * ymeret + 3.5,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                };
                Rajzlap.Children.Add(line);
            }
        }

        private Brush GetColorByLatitude(double latitude)
        {
            if (latitude > 47.5)
            {
                return Brushes.Red;
            }
            else if (latitude < 46.3)
            {
                return Brushes.Green;
            }
            else
            {
                return Brushes.White;
            }
        }
    }
}
