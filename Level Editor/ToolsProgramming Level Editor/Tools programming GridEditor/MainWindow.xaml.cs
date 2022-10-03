//MHP 10/01/2019
//MainWindow for Level Editor Assignment
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml;
using System.Xml.Serialization;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;

namespace Tools_programming_GridEditor
{

    public partial class MainWindow : Window
    {
        public static MainWindow AppWindow;

        int imageGridWidth = 16;
        int imageGridHeight = 16;
        int tileGridWidth = 32;
        int tileGridHeight = 32;

        //Set background image for when no specific image is placed
        public Image backgroundImage = new Image() { Source = new BitmapImage(new Uri("pack://application:,,,/" + "Images/Sprites/BackgroundColour.png")) };

        //Image that will be placed by user when pen is used
        public ImageSource penImageSource;

        // Create list to place information to be saved
        List<byte[]> savedList;

        //Border that will highlight image to be selected when mouse is clicked on imageGrid
        Border border = new Border()
        {
            BorderThickness = new Thickness()
            {
                Bottom = 1,
                Left = 1,
                Right = 1,
                Top = 1
            },
            BorderBrush = new SolidColorBrush(Colors.Black)
        };

        public MainWindow()
        {
            AppWindow = this;

            //Set default image placed whne pen is used to the background image
            penImageSource = backgroundImage.Source;

            InitializeComponent();

            //Fill all Tiles with Background Image at start          
            for (int x = 0; x < grid.GridWidth; x++)
            {
                for (int y = 0; y < grid.GridHeight; y++)
                {
                    grid.SetTile(x, y, backgroundImage.Source);
                }
            }

            // Have mouse events cause the corresponding funcion to occur
            grid.MouseDown += OnMouseDown;
            grid.MouseUp += OnMouseUp;
            grid.MouseLeave += OnMouseLeave;
            grid.MouseEnter += OnMouseEnter;
            grid.MouseMove += OnMouseMove;

            imageGrid.MouseEnter += OnMouseEnterImage;
            imageGrid.MouseMove += OnMouseMoveImage;
        }

        bool penIsDown;

        //Find the tilegrid the mouse is over and use the selected tool when mouse is clicked
        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            int x = (int)e.GetPosition(grid).X / tileGridWidth;
            int y = (int)e.GetPosition(grid).Y / tileGridHeight;

            penIsDown = true;

            UseTool(x, y);
        }
        //Find the tilegrid the mouse is over and stop using selected tool when mouse is no longer down
        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            int x = (int)e.GetPosition(grid).X / tileGridWidth;
            int y = (int)e.GetPosition(grid).Y / tileGridHeight;

            penIsDown = false;
        }

        //Set tool to no longer work when mouse leaves the grid
        //This stops errors occuring
        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            int x = (int)e.GetPosition(grid).X / tileGridWidth;
            int y = (int)e.GetPosition(grid).Y / tileGridHeight;

            penIsDown = false;
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {

            int x = (int)e.GetPosition(grid).X / tileGridWidth;
            int y = (int)e.GetPosition(grid).Y / tileGridHeight;

            if (penIsDown == true)
            {
                UseTool(x, y);
            }
        }

        private void OnMouseEnterImage(object sender, MouseEventArgs e)
        {
            int X = (int)e.GetPosition(imageGrid).X / imageGridWidth;
            int Y = (int)e.GetPosition(imageGrid).Y / imageGridHeight;

            imageGrid.Children.Remove(border);
            imageGrid.Children.Add(border);
            border.SetValue(Grid.ColumnProperty, X);
            border.SetValue(Grid.RowProperty, Y);
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            int x = (int)e.GetPosition(grid).X / tileGridWidth;
            int y = (int)e.GetPosition(grid).Y / tileGridHeight;

            if (penIsDown == true)
            {
                UseTool(x, y);
            }
        }

        private void OnMouseMoveImage(object sender, MouseEventArgs e)
        {
            int X = (int)e.GetPosition(imageGrid).X / imageGridWidth;
            int Y = (int)e.GetPosition(imageGrid).Y / imageGridHeight;

            border.SetValue(Grid.ColumnProperty, X);
            border.SetValue(Grid.RowProperty, Y);
        }

        public void SetImageGrid(int gridX, int gridY, ImageSource Picture)
        {
            if (gridX < imageGrid.swatchWidth && gridY < imageGrid.swatchHeight)
            {
                imageGrid.SetTile(gridX, gridY, Picture);
            }
        }

        private void UseTool(int gridX, int gridY)
        {
            if (gridX < grid.GridWidth && gridY < grid.GridHeight)
            {
                switch (selectedTool)
                {
                    case Tool.Pen:
                        //Make penimageSource the image selected in the swatchpanel and place a copy of the image in the grid
                        grid.SetTile(gridX, gridY, penImageSource);
                        break;

                    case Tool.Erase:
                        //Set the image for the selected tile to the background image
                        grid.SetTile(gridX, gridY, backgroundImage.Source);
                        break;
                }
            }
            else
            {
                return;
            }
        }

        public enum Tool
        {
            Pen,
            Erase
        }

        //Set the defualt tool to the pen
        private Tool selectedTool = Tool.Pen;

        //When the file new button is pressed, cycle through all the 
        private void HandleNew(object sender, RoutedEventArgs e)
        {
            for (int x = 0; x < grid.GridWidth; x++)
            {
                for (int y = 0; y < grid.GridHeight; y++)
                {
                    grid.SetTile(x, y, backgroundImage.Source);
                }
            }
        }

        private void HandleClose(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void HandlePen(object sender, RoutedEventArgs e)
        {
            selectedTool = Tool.Pen;
        }

        private void HandleErase(object sender, RoutedEventArgs e)
        {
            selectedTool = Tool.Erase;
        }

        private void HandleImage(object sender, RoutedEventArgs e)
        {
            swatch.AddTileSheet("images/Sprites/terrain_atlas.png", swatch.SpriteWidth, swatch.SpriteHeight);
        }

        private void Render(object sender, RoutedEventArgs e)
        {
            //Poorly grab width/height
            int width = (int)selectedImages.ActualWidth;
            int height = (int)selectedImages.ActualWidth;
            int dpi = 96;

            //Renders to a target bitmap (also a component which is viewable)
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(width, height, dpi, dpi, PixelFormats.Pbgra32);
            renderTargetBitmap.Render(selectedImages);

            //Encodes to a PNG
            PngBitmapEncoder pngImage = new PngBitmapEncoder();
            pngImage.Frames.Add(BitmapFrame.Create(renderTargetBitmap));

            //Writes PNG encoded image to file
            using (System.IO.Stream fileStream = System.IO.File.Create("created.png"))
            {
                pngImage.Save(fileStream);
            }
        }

        private void HandleSave(object sender, RoutedEventArgs e)
        {
            try
            {
                //For easy array serialization place images into a list and use basic list xml serialization
                //It would be more efficient to save a reference to each unique bitmap used in the file and
                //then use a reference to that bitmap for each tile
                //However this would require reworking the way the entire grid works and would
                //not make that large a difference for the current grid size
                //This would definately need to be done if a very large/high definition grid was used
                savedList = new List<byte[]>();

                for (int x = 0; x < grid.GridWidth; x++)
                {
                    for (int y = 0; y < grid.GridHeight; y++)
                    {
                        // Convert each image to a Bitmap so that it can be saved in an xml stream, because Image objects won't serialize properly
                        MemoryStream memStream = new MemoryStream();
                        JpegBitmapEncoder encoder = new JpegBitmapEncoder();

                        if (grid.images[x, y].Source != null)
                        {
                            // convert image to bitap using encoder
                            encoder.Frames.Add(BitmapFrame.Create(grid.images[x, y].Source as BitmapSource));
                            encoder.Save(memStream);
                            memStream.ToArray();
                            savedList.Add(memStream.ToArray());
                        }
                        else
                        {
                            savedList.Add(null);
                        }
                    }
                }

                //Create a new XmlSerializer for the created savedList and write it to a XML File using the stream
                var serializer = new XmlSerializer(typeof(List<byte[]>));
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "XML Files (*.xml)|*.xml";
                if (saveFileDialog.ShowDialog() == true)
                {
                    using (var stream = File.OpenWrite(saveFileDialog.FileName))
                    {
                        serializer.Serialize(stream, savedList);
                    }
                }
            }
            catch
            {
                //Add message box if unable to save file
                MessageBox.Show("Unable to Save");
                return;
            }
        }

        private void HandleOpen(object sender, RoutedEventArgs e)
        {
            savedList = new List<byte[]>();
            var serializer = new XmlSerializer(typeof(List<byte[]>));
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "XML Files (*.xml)|*.xml";
            if (openFileDialog.ShowDialog() == true)
            {
                using (var stream = File.OpenRead(openFileDialog.FileName))
                {
                    //Open selected XML file and use to set the images in grid
                    //If selected file does not work return without doing anything
                    try
                    {
                        var other = (List<byte[]>)(serializer.Deserialize(stream));
                        if (savedList != null)
                            savedList.Clear();

                        savedList.AddRange(other);
                        for (int x = 0; x < grid.GridWidth; x++)
                        {
                            for (int y = 0; y < grid.GridHeight; y++)
                            {
                                //Create new BitmapImage and a new memorystream using the saved databyte
                                //Which are used in the same order they are saved
                                BitmapImage biImg = new BitmapImage();
                                MemoryStream ms = new MemoryStream(savedList[x * grid.GridHeight + y]);

                                //Initialize the BitmapImage, setting the memorystream ms as the stream source
                                biImg.BeginInit();
                                biImg.StreamSource = ms;
                                biImg.EndInit();

                                ImageSource imgSrc = biImg as ImageSource;

                                //return imgSrc;
                                grid.images[x, y].Source = imgSrc;
                            }

                        }
                    }
                    catch
                    {
                        //Add Message Box if unable to open the selected file
                        MessageBox.Show("Unable to Open");
                        return;
                    }
                }
            }
        }
    }
}
