﻿using System;
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
        private float g_Scale = 3;
        private MapPanel mapPanel = null;
        private GameMap gameMap = null;


        private bool _showDebugLines = false;
        private bool drawModeActive = false;
        
        public string DebugInfo { get; set; }
        public string CurrentLayer { get; set; }
        public string CurrentTile { get; set; }
        public string ScrollOffset { get; set; }
        public string CurrentMapCoords { get; set; }


        

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        public MainWindow()
        {
            InitializeComponent();
            gameMap = new GameMap(10, 10, 3);
            
            MapPanel.OnCellSelected += MapPanel_OnCellSelected;
            
            CurrentLayer = "0";
            CurrentTile = "grass";
            ScrollOffset = "0/0";
            CurrentMapCoords = "0/0";
            lblMapPositionSelected.Text = "0/0";

            TileListPanel tileListPanel = new TileListPanel(TileListView);
            gameMap.FillLayer(tileListPanel.FindTileByName("Grass"), 0);

            // Test
            gameMap.SetTileForLayer(tileListPanel.FindTileByName("Water"), 5, 5, 0);
            // Test end

            mapPanel = new MapPanel(g_Scale, gameMap, tileListPanel.LayerTiles);
            MapCanvas.Children.Add(mapPanel);
            TileListView.SelectionChanged += TileListView_SelectionChanged;

        }

        private void TileListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
                mapPanel.ActiveTileDescription = (TileDescription)e.AddedItems[0];
            else
                mapPanel.ActiveTileDescription = null;
        }

        private void MapPanel_OnCellSelected(Vector selectedCell)
        {
            lblMapPositionSelected.Text = selectedCell.X + "/" + selectedCell.Y;
        }

        private void MenuItemNewMap_Click(object sender, RoutedEventArgs e)
        {

        }
        
        private void DrawMode_Click(object sender, RoutedEventArgs e)
        {
            drawModeActive = !drawModeActive;
            mapPanel.DrawModeActive = drawModeActive;
        }

        private void ZoomPlus_Click(object sender, RoutedEventArgs e)
        {
            g_Scale++;
            mapPanel.DrawScale = g_Scale;
            mapPanel.ShowDebugLines = _showDebugLines;
            mapPanel.RenderContent();
            DebugInfo = "Set scale to " + g_Scale;
            this.NotifyPropertyChanged("DebugInfo");
            
        }

        private void ZoomMinus_Click(object sender, RoutedEventArgs e)
        {
            g_Scale--;
            mapPanel.DrawScale = g_Scale;
            mapPanel.ShowDebugLines = _showDebugLines;
            mapPanel.RenderContent();
            DebugInfo = "Set scale to " + g_Scale;
            this.NotifyPropertyChanged("DebugInfo");

        }

        private void ShowDebugLines_Click(object sender, RoutedEventArgs e)
        {
            _showDebugLines = !_showDebugLines;
            mapPanel.ShowDebugLines = _showDebugLines;
            mapPanel.RenderContent();
        }

        private void MapCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            mapPanel.MapMouseHandler(sender, e);
        }

        private void MapAreaGrid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            mapPanel.MapMouseHandler(sender, e);
        }
    }
}
