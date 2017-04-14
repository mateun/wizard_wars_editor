using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace WizardWarsEditor.lib
{
    public class GameMap
    {
       
        // TODO multidimensional arrays are not supported,
        // so we can not make a simple xml serialization.
        List<TileDescription[,]> layers;
        public int Width { get; set; }
        public int Height { get; set; }

        public String ToJSONRepresentation()
        {
            StringBuilder sb = new StringBuilder();
            JsonWriter jw = new JsonTextWriter(new StringWriter(sb));

            jw.Formatting = Formatting.Indented;
            jw.WriteStartObject();
            jw.WritePropertyName("width");
            jw.WriteValue(this.Width);
            jw.WritePropertyName("height");
            jw.WriteValue(this.Height);
            jw.WritePropertyName("numberOfLayers");
            jw.WriteValue(numberOfLayers);
            jw.WritePropertyName("layers");
            jw.WriteStartArray();
            
            for (int l = 0; l < numberOfLayers; l++)
            {

                jw.WriteStartObject();
                jw.WritePropertyName("layer");
                jw.WriteValue(l);
                jw.WritePropertyName("tiles");
                jw.WriteStartArray();
                for (int x = 0; x < Width; x++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        jw.WriteStartObject();
                        jw.WritePropertyName("x");
                        jw.WriteValue(x);
                        jw.WritePropertyName("y");
                        jw.WriteValue(y);
                        jw.WritePropertyName("tileName");
                        jw.WriteValue(layers[0][x, y].TileName);
                        jw.WriteEndObject();

                    }
                }
                jw.WriteEndArray();
                jw.WriteEndObject();

            }

            
            jw.WriteEndObject();

            return sb.ToString();
        }

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
