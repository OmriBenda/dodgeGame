using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodge_game
{
    public class GamePiece
    {
       public double _x, _y, _height, _width;
        public GamePiece(double x, double y, double height, double width )
        {
            this._x = x;
            this._y = y;
            this._height = height;
            this._width = width;
        }

    }

}
