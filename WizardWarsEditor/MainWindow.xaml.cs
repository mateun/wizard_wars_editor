using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using WizardWarsEditor.lib;

namespace WizardWarsEditor
{

    
   
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private MapPanel mapHost = new MapPanel();
        private float g_Scale = 3;
        private bool _showDebugLines = false;
        
        public string DebugInfo { get; set; }
        public string CurrentLayer { get; set; }
        public string CurrentTile { get; set; }
        public string ScrollOffset { get; set; }
        public string CurrentMapCoords { get; set; }


        System.Collections.ObjectModel.ObservableCollection<lib.TileDescription> LayerTiles { get { return _layerTiles; }}
        System.Collections.ObjectModel.ObservableCollection<lib.TileDescription> _layerTiles = new System.Collections.ObjectModel.ObservableCollection<lib.TileDescription>();

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        public MainWindow()
        {
            InitializeComponent();
            mapHost.DrawScale = g_Scale;
            this.DataContext = this;
            CurrentLayer = "0";
            CurrentTile = "grass";
            ScrollOffset = "0/0";
            CurrentMapCoords = "0/0";
            lblMapPositionSelected.Text = "0/0";
            
            LayerTiles.Add(new TileDescription()
            {
                TileName = "Grass",
                TileImageSource = Environment.CurrentDirectory + "/assets/images/grass16x16.png"
            });

            LayerTiles.Add(new TileDescription()
            {
                TileName = "Sand",
                TileImageSource = Environment.CurrentDirectory + "/assets/images/sand32x32.png"
            });

            LayerTiles.Add(new TileDescription()
            {
                TileName = "Water",
                TileImageSource = Environment.CurrentDirectory + "/assets/images/water32x32.png"
            });
            TileListView.ItemsSource = LayerTiles;
            
          
            MapCanvas.Children.Add(mapHost);

        }

        private void MenuItemNewMap_Click(object sender, RoutedEventArgs e)
        {

        }
        

        private void ZoomPlus_Click(object sender, RoutedEventArgs e)
        {
            g_Scale++;
            mapHost.DrawScale = g_Scale;
            mapHost.RenderContent(g_Scale, _showDebugLines);
            DebugInfo = "Set scale to " + g_Scale;
            this.NotifyPropertyChanged("DebugInfo");
            
        }

        private void ZoomMinus_Click(object sender, RoutedEventArgs e)
        {
            g_Scale--;
            mapHost.DrawScale = g_Scale;
            mapHost.RenderContent(g_Scale, _showDebugLines);
            DebugInfo = "Set scale to " + g_Scale;
            this.NotifyPropertyChanged("DebugInfo");

        }

        private void ShowDebugLines_Click(object sender, RoutedEventArgs e)
        {
            _showDebugLines = !_showDebugLines;
            mapHost.RenderContent(g_Scale, _showDebugLines);
        }
    }
}
