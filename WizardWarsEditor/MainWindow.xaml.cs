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

    
    public class MapPanel : FrameworkElement
    {
        private VisualCollection _visuals;
        public float DrawScale { get; set; }

        float h = 4;
        float r = 0.866025f;
        float rr = 6.92820323f;
        float s = 8;


        public MapPanel() : base()
        {

            
            DrawingVisual mapBackground = new DrawingVisual();

            _visuals = new VisualCollection(this);
            _visuals.Add(mapBackground);
            RenderContent(3);

            this.MouseUp += new MouseButtonEventHandler(MapMouseHandler);
            
        }

        void MapMouseHandler(object sender, MouseButtonEventArgs e)
        {
            Point pos = e.GetPosition(this);

            int mapX = 0;
            int mapY = 0;

            int sectX = (int) (pos.X / (2*rr  * DrawScale ));
            int sectY = (int) (pos.Y / ((h + s) * DrawScale));

            int sectPixelX = (int) (pos.X % (2 * rr * DrawScale));
            int sectPixelY = (int)(pos.Y % ((h + s) * DrawScale));

            System.Console.WriteLine("Mouse pos: " + sectX + "/" + sectY + " px/py: " + sectPixelX + "/" + sectPixelY);

            if ((sectY & 1) == 0)
            {
                float m = (h / rr);
                System.Console.WriteLine("SectionType A with m: " + m);
                float mAtX = sectPixelX * m;
                System.Console.WriteLine("m at localX: " + mAtX);
                float refYLeft = (h * DrawScale) - mAtX;
                float refYRight = -(h * DrawScale) + mAtX;
                System.Console.WriteLine("refY at localX: " + refYLeft);
                mapX = sectX;
                mapY = sectY;
                if (sectPixelX <= (rr * DrawScale) && sectPixelY < refYLeft)
                {
                    // Left upper
                    mapX = sectX - 1;
                    mapY = sectY - 1;
                } else if (sectPixelX > (rr * DrawScale) && sectPixelY < refYRight)
                {
                    mapX = sectX;
                    mapY = sectY - 1;
                }


            } else
            {
                mapX = sectX;
                mapY = sectY;
                System.Console.WriteLine("SectionType B");
                if (sectPixelX <= (rr * DrawScale))
                {
                    float mLUpper = h / rr;
                    float mLUpperAtX = sectPixelX * mLUpper;
                    System.Console.WriteLine("left side. mAtX: " + mLUpperAtX);
                    if (sectPixelY < mLUpperAtX)
                    {
                        mapY = sectY - 1;
                        mapX = sectX;
                    } else
                    {
                        mapY = sectY;
                        mapX = sectX - 1;
                    }
                } else
                {
                    float mRUpper = h / rr;
                    float mRUpperAtX = sectPixelX * mRUpper;
                    float refYRight = (h*DrawScale)-  (mRUpperAtX - (h * DrawScale));
                    System.Console.WriteLine("right side. mAtX: " + refYRight);
                    if (sectPixelY < refYRight)
                    {
                        mapX = mapX;
                        mapY = mapY - 1;
                    }
                }
            }

            System.Console.WriteLine("MapX/Y: " + mapX + "/" + mapY);
        }

        public void RenderContent(float scale)
        {
            DrawingVisual mapBackground = (DrawingVisual)_visuals[0];
            DrawingContext drawingContext = mapBackground.RenderOpen();
            mapBackground.Offset = new Vector(0, 0);
            
            drawingContext.PushClip(new RectangleGeometry(new Rect(00, 00, 850, 700)));

            BitmapImage image = new BitmapImage(new Uri(Environment.CurrentDirectory + "/assets/images/grass16x16.png"));

            

            for (int row = 0; row < 42; row++)
            {
                int v = row & 1;

                for (int i = 0; i < 50; i += 1)
                {
                    Rect rect = new Rect();
                    rect.X = i * 2 * scale * rr + (row & 1) * rr * scale;
                    rect.Width = 16 * scale;
                    rect.Y = row * (h + s) * scale;
                    rect.Height = 16 * scale;

                    drawingContext.DrawImage(image, rect);
                }

            }

            // Render section lines
            Pen linePen = new Pen(Brushes.LightSteelBlue, 2);
            for (int i = 0; i < 100; i++)
            {
                drawingContext.DrawLine(linePen, new Point((i * scale * rr * 2) + 1.5, 0), new Point((i * scale * rr *2)+1.5, 700));
                drawingContext.DrawLine(linePen, new Point(0, i * (h + s) * scale), new Point(850, i * (h+s) * scale));
            }

            drawingContext.Pop();
            drawingContext.Close();
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
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private MapPanel mapHost = new MapPanel();
        private float g_Scale = 3;
        
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
            mapHost.RenderContent(g_Scale);
            DebugInfo = "Set scale to " + g_Scale;
            this.NotifyPropertyChanged("DebugInfo");
            
        }

        private void ZoomMinus_Click(object sender, RoutedEventArgs e)
        {
            g_Scale--;
            mapHost.DrawScale = g_Scale;
            mapHost.RenderContent(g_Scale);
            DebugInfo = "Set scale to " + g_Scale;
            this.NotifyPropertyChanged("DebugInfo");

        }
    }
}
