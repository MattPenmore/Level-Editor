//MHP 10/01/2019
//Main Tilegrid definition for Level Editor Assignment
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Tools_programming_GridEditor
{
    public class TileGrid : Grid
    {
        public Image[,] images;

        public int GridWidth = 31;
        public int GridHeight = 19;

        public TileGrid()
        {
            images = new Image[GridWidth, GridHeight];

            for (int x = 0; x < GridWidth; x++)
            {
                for (int y = 0; y < GridHeight; y++)
                {
                    Image image = new Image();

                    SetColumn(image, x);
                    SetRow(image, y);

                    Children.Add(image);

                    images[x, y] = image;
                }
            }
        }

        //Make the image at chosen poisiton the chosen image
        public void SetTile(int gridX, int gridY, ImageSource Picture)
        {
            images[gridX, gridY].Source = Picture;
        }
    }
}
