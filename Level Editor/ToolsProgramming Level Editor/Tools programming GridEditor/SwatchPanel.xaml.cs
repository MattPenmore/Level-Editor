//MHP 10/01/2019
//SwatchPanel definition for Level Editor Assignment

using System;
using System.Collections.Generic;
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

namespace Tools_programming_GridEditor
{
    /// <summary>
    /// Interaction logic for SwatchPanel.xaml
    /// </summary>
    public partial class SwatchPanel : UserControl
    {
        public ImageSource penImageSource;
        //Border components
        protected int borderWidth = 1;
        protected SolidColorBrush selectedBrush = new SolidColorBrush(Colors.Black);
        protected SolidColorBrush unselectedBrush = new SolidColorBrush(Colors.White);

        //Selected item
        protected Border selectedSwatch = null;

        //Events to fire
        public delegate void SelectedItemDelegate(int selectedIndex, ImageSource source);
        public event SelectedItemDelegate SelectedItem;

        //Public properties, but private setters
        public int SpriteWidth { get; private set; } = 32;
        public int SpriteHeight { get; private set; } = 32;

        public SwatchPanel()
        {
            InitializeComponent();

            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
                return;

            //For now using fixed tilesheet, tbd - allow user to select their tilesheet
            AddTileSheet("images/Sprites/terrain_atlas.png", SpriteWidth, SpriteHeight);
        }
        // Create image from a filepath
        public void AddSprite(string localFilePath, int X, int Y)
        {
            var image = new Image() { Source = new BitmapImage(new Uri("pack://application:,,,/" + localFilePath)) };
            image.Stretch = Stretch.None;
            AddSprite(image, X, Y);
        }

        //Adds image to image editor ( called on load)
        public void AddSprite(Image image, int X, int Y)
        {
            ImageSource swatchImage = (image as Image).Source;

            if (image != null)
            {
                MainWindow.AppWindow.SetImageGrid(X / SpriteWidth, Y / SpriteHeight, swatchImage);
            }
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var border = sender as Border;
            if (selectedSwatch != null)
                selectedSwatch.BorderBrush = unselectedBrush;

            selectedSwatch = border;
            border.BorderBrush = selectedBrush;
            int selectedIndex = stackPanel.Children.IndexOf(border);

            if (selectedSwatch != null)
            {
                penImageSource = (border.Child as Image).Source;
            }
        }

        //Split the tilesheet into width by height tiles
        public void AddTileSheet(string localTileSheetPath, int width, int height)
        {
            var imageSouce = new BitmapImage(new Uri("pack://application:,,,/" + localTileSheetPath));
            for (int y = 0; y < imageSouce.PixelHeight; y += height)
            {
                for (int x = 0; x < imageSouce.PixelWidth; x += width)
                {
                    var croppedBitmap = new CroppedBitmap(imageSouce, new Int32Rect(x, y, width, height));
                    AddSprite(new Image() { Source = croppedBitmap }, x, y);
                }
            }
        }
    }
}
