using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFMultiThreadingTemplate
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private object _itemsLock;
        public ObservableCollection<TabItem> Tabs { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            Tabs = new ObservableCollection<TabItem>();
            _itemsLock = new object();
            BindingOperations.EnableCollectionSynchronization(Tabs, _itemsLock);

            var addCollection = Task.Run(() =>
            {
                Thread.Sleep(5000);

                lock (_itemsLock)
                {
                    Tabs.Add(new TabItem { Header = "One", Content = "One's content", Result = "test" });
                    Tabs.Add(new TabItem { Header = "Two", Content = "Two's content", Result = "test" });
                }

                Thread.Sleep(2000);

                lock (_itemsLock)
                {
                    foreach (var i in Tabs)
                    {
                        i.Result = "Hello World";
                    }
                }

                App.Current.Dispatcher.BeginInvoke(new Action(() => {
                    Tabtab.Items.Refresh();
                }));
                
                Thread.Sleep(2000);

                lock (_itemsLock)
                {
                    Tabs.Add(new TabItem { Header = "Three", Content = "Three's content", Result = "test" });
                }
            });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string count = Tabs.Count().ToString();
            Tabs.Add(new TabItem { Header = count, Content = $"{count} content", Result = "test" });
        }
    }

    public sealed class TabItem
    {
        public string? Header { get; set; }
        public string? Content { get; set; }
        public string? Result { get; set; }
    }
}
