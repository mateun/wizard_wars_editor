using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WizardWarsEditor.lib
{
    public class TileListPanel
    {
        public System.Collections.ObjectModel.ObservableCollection<lib.TileDescription> LayerTiles { get { return _layerTiles; } }
        System.Collections.ObjectModel.ObservableCollection<lib.TileDescription> _layerTiles = new System.Collections.ObjectModel.ObservableCollection<lib.TileDescription>();

        public TileListPanel(ListView lv)
        {
            LayerTiles.Add(new TileDescription()
            {
                TileName = "Grass",
                TileImageSource = Environment.CurrentDirectory + "/assets/images/grass16x16.png"
            });

            LayerTiles.Add(new TileDescription()
            {
                TileName = "Sand",
                TileImageSource = Environment.CurrentDirectory + "/assets/images/sand16x16.png"
            });

            LayerTiles.Add(new TileDescription()
            {
                TileName = "Water",
                TileImageSource = Environment.CurrentDirectory + "/assets/images/water16x16.png"
            });
            lv.ItemsSource = LayerTiles;
        }

        /// <summary>
        /// Returns the tileDescription with the given name, 
        /// or null, if none can be found.
        /// </summary>
        /// <param name="name">The tileDescription to search for</param>
        /// <returns></returns>
        public TileDescription FindTileByName(string name)
        {
            foreach (TileDescription td in LayerTiles)
            {
                if (td.TileName.Equals(name)) return td;
            }

            return null;
        }
    }
}
