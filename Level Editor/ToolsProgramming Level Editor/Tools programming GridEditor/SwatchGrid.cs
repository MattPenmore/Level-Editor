//MHP 10/01/2019
//imageGrid definition for Level Editor Assignment
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
    public class SwatchGrid : Grid
    {
        //Create a border to highlight the image the mouse is over
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

        //2d Array holding swatch images for picking
        private Image[,] swatchImages;

        //Size of the 2d array
        public int swatchWidth = 32;
        public int swatchHeight = 32;

        public SwatchGrid()
        {
            swatchImages = new Image[swatchWidth, swatchHeight];

            //Loop through the 2d array and set the position of each image fill with the currently null images
            for (int x = 0; x < swatchWidth; x++)
            {
                for (int y = 0; y < swatchHeight; y++)
                {
                    Image image = new Image();
                    SetColumn(image, x);
                    SetRow(image, y);

                    //Make the image a child of the grid
                    Children.Add(image);

                    //Placing image in array at position x,y
                    swatchImages[x, y] = image;

                    //Add mousedown handler for this image
                    image.MouseDown += OnMouseDown;
                }
            }
        }

        //When an image is clicked make it the image placed when the pen is used
        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            Image image = sender as Image;

            MainWindow.AppWindow.penImageSource = (image as Image).Source;
        }

        //Make the image at chosen positon the chosen image
        public void SetTile(int gridX, int gridY, ImageSource Picture)
        {
            swatchImages[gridX, gridY].Source = Picture;
        }
    }
}
