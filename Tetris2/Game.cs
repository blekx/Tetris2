using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Tetris2
{
    public class Game
    {
        public Image Image;
        private WriteableBitmap bitmap;
        public int DimensionX { get; private set; }
        public int DimensionY { get; private set; }
        public double Gravity { get; private set; }
        /// <summary>
        /// [0,0] = Left, Bottom
        /// </summary>
        private bool[,] boolField;
        private List<Block> AllFieldBlocks = new List<Block>();
        private List<Block> FallingBlocks = new List<Block>();

        //public Game(WriteableBitmap bitmap, int dimensionX, int dimensionY, double gravity)
        public Game(Image image, int dimensionX, int dimensionY, double gravity)
        {
            //this.bitmap = bitmap;
            Image = image;
            DimensionX = dimensionX;
            DimensionY = dimensionY;
            Gravity = gravity;
            boolField = new bool[DimensionX, DimensionY * 2];
        }
        //public Game(WriteableBitmap bitmap)
        //    : this(bitmap, 10, 20, 1.5) { }
        //public Game(Image image)
        //            : this((WriteableBitmap)image.Source, 10, 20, 1.5) { }
        public Game(Image image)
               : this(image, 10, 20, 1.5) { }
        //public Game(WriteableBitmap image)
          //          : this((Image)image.Source, 10, 20, 1.5) { }

        public void Update()
        {
            //CheckBlocks();
            //CountFallingBlocks();
            Redraw();
        }

        private void Redraw()
        {
            throw new NotImplementedException();
        }

        private void CountFallingBlocks()
        {
            throw new NotImplementedException();
        }

        private void CheckBlocks()
        {
            throw new NotImplementedException();
        }

        public void HelloBlock()
        {
            Block b = BlockGenerator.NewBlock();
            AllFieldBlocks.Add(b);
        }
        public string ShowHelloBlock()
        {
            AllFieldBlocks.Clear();
            HelloBlock();
            return AllFieldBlocks[0].ToString();
        }

        public Block GiveHelloBlock()
        {
            return AllFieldBlocks[0];
        }
    }
}
