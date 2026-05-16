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

namespace PermDynamics.Pages
{
    public partial class Main : Page
    {
        public MainWindow mainWindow;

        public Main(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
        }

        private void OpenPageChart(object sender, RoutedEventArgs e)
        {
            double value;
            bool isValid = double.TryParse(tb_value.Text, out value);

            if (!isValid || value <= 0)
            {
                MessageBox.Show("Введите корректное положительное число!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            mainWindow.pointsInfo.Clear();
            mainWindow.pointsInfo.Add(new Classes.PointInfo(value));
            mainWindow.OpenPages(MainWindow.pages.chart);
        }
    }
}