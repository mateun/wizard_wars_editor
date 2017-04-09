using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WizardWarsEditor.lib
{
    public enum ScrollInput
    {
        LEFT, 
        RIGHT, 
        UP, 
        DOWN
    }

    public delegate void MouseMapSelectHandler(Vector selectedCell);
    public class MapPanel : FrameworkElement
    {
        public static event MouseMapSelectHandler OnCellSelected;
        public float DrawScale { get; set; }
        public bool ShowDebugLines { get; set; }
        public bool DrawModeActive { get; set; }
        public LayerDescription ActiveLayer { get; set; }
        public TileDescription ActiveTileDescription { get; set; }

        VisualCollection _visuals;
        float h = 4;
        float rr = 6.92820323f;
        float s = 8;

        int cellCursorX = 0;
        int cellCursorY = 0;
        int drawCursorX = 0;
        int drawCursorY = 0;
        GameMap gameMap;
        Collection<TileDescription> tileDescriptions = null;
        Dictionary<string, BitmapImage> imageDict = null;
        int cellScrollOffsetX = 0;
        int cellScrollOffsetY = 0;

        /// <summary>
        /// This class represents the map area where the user manipulates the game map. 
        /// He can add/update/remove tiles on the map, 
        /// change the size of the map,
        /// draw debug lines.
        /// This panel receives its "model" in the constructor. 
        /// When being redrawn, it simply draws the current state of the model. 
        /// 
        /// </summary>
        /// <param name="scale">The inital drawing scale of the map</param>
        /// <param name="gameMapParam">The underlying model, holding the state of the currently worked on map. The thing that is serialized out to disk when saving.</param>
        /// <param name="tileDescriptionsParam">A complete list of all available tiles. Useful to preload images etc. </param>
        public MapPanel(float scale, GameMap gameMapParam, Collection<TileDescription> tileDescriptionsParam) : base()
        {
            gameMap = gameMapParam;
            tileDescriptions = tileDescriptionsParam;
            DrawScale = scale;

            DrawingVisual mapBackground = new DrawingVisual();
            imageDict = new Dictionary<string, BitmapImage>();
            LoadImagesToDict(tileDescriptions);

            _visuals = new VisualCollection(this);
            _visuals.Add(mapBackground);
            RenderContent();

            this.MouseUp += new MouseButtonEventHandler(MapMouseHandler);
        }

        /// <summary>
        /// Preload so we can need not load from disk later when the images are rendered.
        /// </summary>
        /// <param name="tiles"></param>
        void LoadImagesToDict(Collection<TileDescription> tiles)
        {
            foreach(TileDescription td in tiles)
            {
                BitmapImage image = new BitmapImage(new Uri(td.TileImageSource));
                imageDict[td.TileName] = image;
            }
        }

        public void ReceiveScrollInput(ScrollInput input)
        {
            switch (input)
            {
                case (ScrollInput.LEFT): cellScrollOffsetX--; break;
                case (ScrollInput.RIGHT): cellScrollOffsetX++; break;
                case (ScrollInput.UP): cellScrollOffsetY++;break;
                case (ScrollInput.DOWN): cellScrollOffsetY--; break;

            }
        }

        public void RenderContent()
        {
            DrawingVisual mapBackground = (DrawingVisual)_visuals[0];
            DrawingContext drawingContext = mapBackground.RenderOpen();
            
            drawingContext.PushClip(new RectangleGeometry(new Rect(00, 00, 850, 700)));

            BitmapImage image = new BitmapImage(new Uri(Environment.CurrentDirectory + "/assets/images/grass16x16.png"));

            // The idea here is to render only a specific window of the game map, not more than 20x20 cells.
            
            for (int l = 0; l < gameMap.NumberOfLayers(); l++)
            {
                for (int row = 0; row < 18; row++)
                {
                    for (int i = 0; i < 19; i += 1)
                    {
                        int cellX = i + cellScrollOffsetX;
                        int cellY = row + cellScrollOffsetY;
                        TileDescription tileDescription = gameMap.Tile(l, cellX, cellY);
                        if (tileDescription == null)
                            continue;
                        Rect rect = new Rect()
                        {
                            X = i * 2 * DrawScale * rr + (cellY & 1) * rr * DrawScale,
                            Width = 16 * DrawScale,
                            Y = row * (h + s) * DrawScale,
                            Height = 16 * DrawScale
                        };
                        drawingContext.DrawImage(imageDict[tileDescription.TileName], rect);
                    }
                }
            }
            

            if (ShowDebugLines)
            {
                Pen linePen = new Pen(Brushes.LightSteelBlue, 2);
                for (int i = 0; i < 100; i++)
                {
                    drawingContext.DrawLine(linePen, new Point((i * DrawScale * rr * 2) + 1.5, 0), new Point((i * DrawScale * rr * 2) + 1.5, 700));
                    drawingContext.DrawLine(linePen, new Point(0, i * (h + s) * DrawScale), new Point(850, i * (h + s) * DrawScale));
                }
            }

            // Draw the cursor
            int rowInfo = drawCursorY + cellScrollOffsetY;
            int cursorPixelX = (int) ((drawCursorX) * 2 * DrawScale * rr + ((rowInfo) & 1) * rr * DrawScale);
            int cursorPixelY = (int)((drawCursorY) * (h + s) * DrawScale);
            System.Console.WriteLine("cursorPixel: " + cursorPixelX + "/" + cursorPixelY);
            Rect rectCursor = new Rect()
            {
                X = cursorPixelX,
                Y = cursorPixelY,
                Width = 16 * DrawScale,
                Height = 16 * DrawScale
            };
            BitmapImage imageCursor = new BitmapImage(new Uri(Environment.CurrentDirectory + "/assets/images/cellCursorRed16x16.png"));
            drawingContext.DrawImage(imageCursor, rectCursor);

            drawingContext.Pop();
            drawingContext.Close();
        }

        /// <summary>
        /// Here is all the ugly screen to hexgrid mapping!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MapMouseHandler(object sender, MouseButtonEventArgs e)
        {
            Point pos = e.GetPosition(this);
            
            int sectX = (int)(pos.X / (2 * rr * DrawScale));
            int sectY = (int)(pos.Y / ((h + s) * DrawScale));
            sectX += cellScrollOffsetX;
            sectY += cellScrollOffsetY;

            int sectPixelX = (int)(pos.X % (2 * rr * DrawScale));
            int sectPixelY = (int)(pos.Y % ((h + s) * DrawScale));
            
            if ((sectY & 1) == 0)
            {
                float m = (h / rr);
                
                float mAtX = sectPixelX * m;
                
                float refYLeft = (h * DrawScale) - mAtX;
                float refYRight = -(h * DrawScale) + mAtX;
                
                cellCursorX = sectX;
                cellCursorY = sectY;
                if (sectPixelX <= (rr * DrawScale) && sectPixelY < refYLeft)
                {
                    // Left upper
                    cellCursorX = sectX - 1;
                    cellCursorY = sectY - 1;
                }
                else if (sectPixelX > (rr * DrawScale) && sectPixelY < refYRight)
                {
                    cellCursorX = sectX;
                    cellCursorY = sectY - 1;
                }

            }
            else
            {
                cellCursorX = sectX;
                cellCursorY = sectY;
                
                if (sectPixelX <= (rr * DrawScale))
                {
                    float mLUpper = h / rr;
                    float mLUpperAtX = sectPixelX * mLUpper;
                
                    if (sectPixelY < mLUpperAtX)
                    {
                        cellCursorY = sectY - 1;
                        cellCursorX = sectX;
                    }
                    else
                    {
                        cellCursorY = sectY;
                        cellCursorX = sectX - 1;
                    }
                }
                else
                {
                    float mRUpper = h / rr;
                    float mRUpperAtX = sectPixelX * mRUpper;
                    float refYRight = (h * DrawScale) - (mRUpperAtX - (h * DrawScale));
                    System.Console.WriteLine("right side. mAtX: " + refYRight);
                    if (sectPixelY < refYRight)
                    {
                        cellCursorX = sectX;
                        cellCursorY = cellCursorY - 1;
                    }
                }
            }

            // Apply the scrolling offset
            drawCursorX = cellCursorX - cellScrollOffsetX;
            drawCursorY = cellCursorY - cellScrollOffsetY;

            // Normalize cell selection 
            cellCursorX = Math.Max(0, cellCursorX);
            cellCursorY = Math.Max(0, cellCursorY);
            cellCursorX = Math.Min(gameMap.Width - 1, cellCursorX);
            cellCursorY = Math.Min(gameMap.Height - 1, cellCursorY);

            // Inform any event listeners of our cell selection
            OnCellSelected(new Vector(cellCursorX, cellCursorY));

            // update the gameMap model object and set the currently active tile 
            // to the correct place.
            if (DrawModeActive && ActiveLayer != null)
            {
                gameMap.SetTileForLayer(ActiveTileDescription, cellCursorX, cellCursorY, ActiveLayer.Number);
            }

            // Trigger redraw with potentially changed cursor
            // (we could check if it is really changed and only redraw if it has...)
            RenderContent();
        }

        protected override int VisualChildrenCount
        {
            get { return _visuals.Count; }
        }

        // Provide a required override for the GetVisualChild method.
        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= _visuals.Count)
            {
                throw new ArgumentOutOfRangeException();
            }
            return _visuals[index];
        }
    }
}
