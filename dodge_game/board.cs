using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodge_game
{
    class Board
    {
        public Player jerry;
        public Enemy[] toms;
        public Cheese[] cheese;
        double _boardWidth;
        double _boardHeigth;
        Random r = new Random();
        double tomSize = 30;
        double jerrySize = 40;
        double cheeseSize = 30;
        public Board(double boardWidth, double boardHeigth)
        {
            this._boardWidth = boardWidth;
            this._boardHeigth = boardHeigth;
            
            jerry = new Player(boardWidth / 2 - (jerrySize / 2), boardHeigth / 2 - (jerrySize/2), jerrySize, jerrySize);
            toms = new Enemy[10];
            cheese = new Cheese[15];

            for (int i = 0; i < 10; i++)
            {
                toms[i] = new Enemy(r.Next((int)boardWidth - 30), r.Next((int)boardHeigth - 30), tomSize, tomSize);
            }
            for (int i = 0; i < 15; i++)
            {
                cheese[i] = new Cheese(r.Next((int)boardWidth - 30), r.Next((int)boardHeigth - 30), cheeseSize, cheeseSize);
            }

        }
    }
}
