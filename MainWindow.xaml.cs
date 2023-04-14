using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace PTGui_Language_Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DataContext = new MainWindowViewModel();
            InitializeComponent();

            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);

            Copyright.Text = string.Format("© Michael Göricke v{0}.{1}.{2}", fvi.ProductMajorPart, fvi.ProductMinorPart, fvi.ProductBuildPart);
        }
    }
}

