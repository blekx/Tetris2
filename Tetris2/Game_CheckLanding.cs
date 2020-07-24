using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris2
{
    public partial class Game
    {
        /// <summary> Corrects if Falling blocks already landed. </summary>
        private void CheckLanding(List<Block> blocks)
        {
            bool checkAgain = true;
            while (checkAgain)
            {
                checkAgain = false;
                List<Block> backupList = new List<Block>(blocks);
                List<Block> blocksToStop = new List<Block>();
                foreach (Block b in backupList)
                {
                    if (!IsSpace(b, D4.O))
                    {
                        checkAgain = true;
                        blocks.Remove(b);
                        blocksToStop.Add(b);
                        //nomore_fallingBlocks.Add(b);
                        if (b == ab.block)
                        {
                            abJustLanded_ThrowNew = true;
                            ab.vX = 0;
                            gameGrid.Children.Remove(ab.ghostBlock.Canvas);
                        }
                    }
                    // while (!IsSpace(b, D4.O) && IsSpace(b, D4.T))
                    while (!IsSpace(b, D4.O))
                    {//bug situation, shoots the block up away
                        b.CoordinatesY++;
                    }
                    ProjectIntoBoolField(b);
                }
                StopBlocks(blocksToStop);
                foreach (Block b in blocksToStop) fallingBlocks.Remove(b);
            }
            //nevim
            //            foreach (Block b in nomore_fallingBlocks) fallingBlocks.Remove(b);
        }

    }
}
