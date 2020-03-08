using BestRunApp.Objetos;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace BestRunApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Objetos.metodosImportacao metImp = new Objetos.metodosImportacao();
            metImp.LoadGrid(null, dtgResultado);

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            Objetos.metodosImportacao metImp = new Objetos.metodosImportacao();

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Arquivo de texto (Log Formula 1) |*.txt";

            if (openFileDialog.ShowDialog() == true)
            {
                StreamReader file = new StreamReader(openFileDialog.FileName);

                metImp.Log("Inicío da importação!");

                //dtgResultado.ItemsSource =;

                metImp.LoadGrid(metImp.ListDadosImportados(file), dtgResultado);

                metImp.Log("Fim da importação!");

                MessageBoxResult result = MessageBox.Show("Arquivo importado com sucesso! \nVerifique o arquivo de log na pasta da aplicação!", "Fim da importação", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("Nenhum arquivo selecionado!", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }
    }
}
