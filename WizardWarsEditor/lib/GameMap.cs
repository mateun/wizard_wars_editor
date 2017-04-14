using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardWarsEditor.lib
{
    [Serializable]
    public class GameMap
    {
       
        // TODO multidimensional arrays are not supported,
        // so we can not make a simple xml serialization.
        List<TileDescription[,]> layers;
        public int Width { get; set; }
        public int Height { get; set; }

        public int NumberOfLayers
        {
            get { return numberOfLayers;  }
            set { numberOfLayers = value;  }
        }
        private int numberOfLayers;

        public GameMap() :this(32, 32, 3)
        {
            
        }
        
        public GameMap(int mapWidth, int mapHeight, int numberOfLayersParam)
        {
            layers = new List<TileDescription[,]>();
            numberOfLayers = numberOfLayersParam;

            Width = mapWidth;
            Height = mapHeight;

            for (int i = 0; i < numberOfLayers; i++)
            {
                TileDescription[,] layer = new TileDescription[mapWidth, mapHeight];
                layers.Add(layer);
            }
            
        }
        

        public void SetTileForLayer(TileDescription td, int x, int y, int layerIndex)
        {
            layers[layerIndex][x, y] = td;
        }

        public TileDescription Tile(int layerIndex, int x, int y)
        {
            if (x < Width && y < Height && y >= 0 && x >= 0)
                return layers[layerIndex][x, y];
            else
                return null;
        }

        public void FillLayer(TileDescription td, int layerIndex)
        {
            for (int x = 0; x < Width; x++ )
            {
                for (int y = 0; y < Height; y++)
                {
                    layers[layerIndex][x, y] = td;
                }
            }
        }
    }
}
