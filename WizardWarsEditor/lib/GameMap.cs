﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardWarsEditor.lib
{
    public class GameMap
    {
        List<TileDescription[,]> layers;
        public int Width { get; set; }
        public int Height { get; set; }
        
        public GameMap(int mapWidth, int mapHeight, int numberOfLayers)
        {
            layers = new List<TileDescription[,]>();

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
            return layers[layerIndex][x,y];
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