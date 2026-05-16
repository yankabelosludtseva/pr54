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
using System.Windows.Threading;

namespace PermDynamics.Pages
{
    public partial class Chart : Page
    {
        public MainWindow mainWindow;
        public double actualHeightCanvas = 0;
        public double maxValue = 0;
        double averageValue = 0;
        public DispatcherTimer dispatcherTimer = new DispatcherTimer();
        private Line averageLine = null;

        public Chart(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;

            if (_canvas == null)
            {
                MessageBox.Show("Canvas не найден в XAML!");
                return;
            }

            actualHeightCanvas = _canvas.Height;

            if (actualHeightCanvas <= 0 || double.IsNaN(actualHeightCanvas))
                actualHeightCanvas = 300;

            if (mainWindow.pointsInfo == null || mainWindow.pointsInfo.Count == 0)
            {
                mainWindow.pointsInfo = new List<Classes.PointInfo>();
                mainWindow.pointsInfo.Add(new Classes.PointInfo(100));
            }

            dispatcherTimer.Interval = new TimeSpan(0, 0, 2);
            dispatcherTimer.Tick += CreateNewValue;
            dispatcherTimer.Start();

            CreateChart();
            ColorChart();
        }

        private void CreateNewValue(object sender, EventArgs e)
        {
            Random random = new Random();
            double value = mainWindow.pointsInfo[mainWindow.pointsInfo.Count - 1].value;
            double newValue = value * (random.NextDouble() + 0.5d);
            mainWindow.pointsInfo.Add(new Classes.PointInfo(newValue));
            ControlCreateChart();
        }

        public void CreateChart()
        {
            try
            {
                _canvas.Children.Clear();
                averageLine = null;
                maxValue = 0;

                if (mainWindow.pointsInfo == null || mainWindow.pointsInfo.Count == 0) return;

                for (int i = 0; i < mainWindow.pointsInfo.Count; i++)
                {
                    if (mainWindow.pointsInfo[i].value > maxValue)
                        maxValue = mainWindow.pointsInfo[i].value;
                }

                if (maxValue <= 0 || double.IsNaN(maxValue)) maxValue = 1;
                if (actualHeightCanvas <= 0 || double.IsNaN(actualHeightCanvas)) actualHeightCanvas = 300;

                // Пересчитываем среднее значение
                averageValue = 0;
                for (int i = 0; i < mainWindow.pointsInfo.Count; i++)
                    averageValue += mainWindow.pointsInfo[i].value;
                averageValue /= mainWindow.pointsInfo.Count;

                // Рисуем линии графика
                for (int i = 0; i < mainWindow.pointsInfo.Count; i++)
                {
                    Line line = new Line();
                    line.X1 = i * 20;
                    line.X2 = (i + 1) * 20;

                    double y1 = actualHeightCanvas;
                    double y2 = actualHeightCanvas;

                    if (i == 0)
                    {
                        y1 = actualHeightCanvas;
                    }
                    else
                    {
                        double prevValue = mainWindow.pointsInfo[(i - 1)].value;
                        if (!double.IsNaN(prevValue) && prevValue >= 0)
                        {
                            y1 = actualHeightCanvas - ((prevValue / maxValue) * actualHeightCanvas);
                        }
                    }

                    double currentValue = mainWindow.pointsInfo[i].value;
                    if (!double.IsNaN(currentValue) && currentValue >= 0)
                    {
                        y2 = actualHeightCanvas - ((currentValue / maxValue) * actualHeightCanvas);
                    }

                    if (double.IsNaN(y1) || double.IsInfinity(y1)) y1 = actualHeightCanvas;
                    if (double.IsNaN(y2) || double.IsInfinity(y2)) y2 = actualHeightCanvas;

                    if (y1 < 0) y1 = 0;
                    if (y2 < 0) y2 = 0;
                    if (y1 > actualHeightCanvas) y1 = actualHeightCanvas;
                    if (y2 > actualHeightCanvas) y2 = actualHeightCanvas;

                    line.Y1 = y1;
                    line.Y2 = y2;
                    line.StrokeThickness = 2;
                    line.Stroke = Brushes.Blue;

                    mainWindow.pointsInfo[i].line = line;
                    _canvas.Children.Add(line);
                }

                // Рисуем линию среднего значения
                DrawAverageLine();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка в CreateChart: " + ex.Message);
            }
        }

        public void CreatePoint()
        {
            try
            {
                if (mainWindow.pointsInfo == null || mainWindow.pointsInfo.Count < 2) return;
                if (maxValue <= 0 || double.IsNaN(maxValue)) maxValue = 1;
                if (actualHeightCanvas <= 0 || double.IsNaN(actualHeightCanvas)) actualHeightCanvas = 300;

                // Пересчитываем среднее значение
                averageValue = 0;
                for (int i = 0; i < mainWindow.pointsInfo.Count; i++)
                    averageValue += mainWindow.pointsInfo[i].value;
                averageValue /= mainWindow.pointsInfo.Count;

                Line line = new Line();
                line.X1 = (mainWindow.pointsInfo.Count - 1) * 20;
                line.X2 = mainWindow.pointsInfo.Count * 20;

                double y1 = actualHeightCanvas;
                double y2 = actualHeightCanvas;

                double prevValue = mainWindow.pointsInfo[(mainWindow.pointsInfo.Count - 2)].value;
                if (!double.IsNaN(prevValue) && prevValue >= 0)
                {
                    y1 = actualHeightCanvas - ((prevValue / maxValue) * actualHeightCanvas);
                }

                double currentValue = mainWindow.pointsInfo[(mainWindow.pointsInfo.Count - 1)].value;
                if (!double.IsNaN(currentValue) && currentValue >= 0)
                {
                    y2 = actualHeightCanvas - ((currentValue / maxValue) * actualHeightCanvas);
                }

                if (double.IsNaN(y1) || double.IsInfinity(y1)) y1 = actualHeightCanvas;
                if (double.IsNaN(y2) || double.IsInfinity(y2)) y2 = actualHeightCanvas;

                if (y1 < 0) y1 = 0;
                if (y2 < 0) y2 = 0;
                if (y1 > actualHeightCanvas) y1 = actualHeightCanvas;
                if (y2 > actualHeightCanvas) y2 = actualHeightCanvas;

                line.Y1 = y1;
                line.Y2 = y2;
                line.StrokeThickness = 2;

                mainWindow.pointsInfo[(mainWindow.pointsInfo.Count - 1)].line = line;
                _canvas.Children.Add(line);

                // Обновляем линию среднего значения
                UpdateAverageLine();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка в CreatePoint: " + ex.Message);
            }
        }

        private void DrawAverageLine()
        {
            try
            {
                if (_canvas == null) return;
                if (averageValue <= 0 || maxValue <= 0) return;

                double averageY = actualHeightCanvas - ((averageValue / maxValue) * actualHeightCanvas);

                if (double.IsNaN(averageY) || double.IsInfinity(averageY)) return;
                if (averageY < 0) averageY = 0;
                if (averageY > actualHeightCanvas) averageY = actualHeightCanvas;

                averageLine = new Line();
                averageLine.X1 = 0;
                averageLine.X2 = _canvas.Width > 0 ? _canvas.Width : 800;
                averageLine.Y1 = averageY;
                averageLine.Y2 = averageY;
                averageLine.StrokeThickness = 2;
                averageLine.Stroke = Brushes.Orange;
                averageLine.StrokeDashArray = new DoubleCollection { 5, 5 };

                _canvas.Children.Add(averageLine);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка в DrawAverageLine: " + ex.Message);
            }
        }

        private void UpdateAverageLine()
        {
            try
            {
                if (_canvas == null) return;
                if (averageValue <= 0 || maxValue <= 0) return;

                double averageY = actualHeightCanvas - ((averageValue / maxValue) * actualHeightCanvas);

                if (double.IsNaN(averageY) || double.IsInfinity(averageY)) return;
                if (averageY < 0) averageY = 0;
                if (averageY > actualHeightCanvas) averageY = actualHeightCanvas;

                if (averageLine != null)
                {
                    averageLine.Y1 = averageY;
                    averageLine.Y2 = averageY;
                    averageLine.X2 = _canvas.Width > 0 ? _canvas.Width : 800;
                }
                else
                {
                    DrawAverageLine();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка в UpdateAverageLine: " + ex.Message);
            }
        }

        public void ControlCreateChart()
        {
            if (mainWindow.pointsInfo == null || mainWindow.pointsInfo.Count == 0) return;

            double value = mainWindow.pointsInfo[mainWindow.pointsInfo.Count - 1].value;
            if (value < maxValue && !double.IsNaN(value))
            {
                CreatePoint();
            }
            else
            {
                CreateChart();
            }
            ColorChart();
        }

        public void ColorChart()
        {
            try
            {
                if (mainWindow.pointsInfo == null || mainWindow.pointsInfo.Count == 0) return;

                double currentValue = mainWindow.pointsInfo[mainWindow.pointsInfo.Count - 1].value;

                for (int i = 0; i < mainWindow.pointsInfo.Count; i++)
                {
                    if (mainWindow.pointsInfo[i].line != null)
                    {
                        if (mainWindow.pointsInfo[i].value < averageValue)
                            mainWindow.pointsInfo[i].line.Stroke = Brushes.Red;
                        else
                            mainWindow.pointsInfo[i].line.Stroke = Brushes.Green;
                    }
                }

                if (_canvas != null)
                    _canvas.Width = Math.Max(mainWindow.pointsInfo.Count * 20 + 300, 800);

                if (_scroll != null)
                    _scroll.ScrollToHorizontalOffset(_canvas.Width);

                // Обновляем позицию линии среднего значения при изменении ширины
                if (averageLine != null && _canvas != null)
                {
                    averageLine.X2 = _canvas.Width;
                }

                if (current_value != null)
                    current_value.Content = "Тек. знач: " + Math.Round(currentValue, 2);
                if (average_value != null)
                    average_value.Content = "Сред. знач: " + Math.Round(averageValue, 2);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка в ColorChart: " + ex.Message);
            }
        }

        public void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_canvas != null)
            {
                actualHeightCanvas = _canvas.Height;
                if (actualHeightCanvas <= 0 || double.IsNaN(actualHeightCanvas))
                    actualHeightCanvas = 300;
            }

            if (mainWindow.pointsInfo != null && mainWindow.pointsInfo.Count > 0)
            {
                CreateChart();
                ColorChart();
            }
        }
    }
}