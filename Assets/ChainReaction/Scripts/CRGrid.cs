using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Athena.ChainReaction
{
    public class CRGrid 
    {
        private int _width;
        private int _height;

        public Cell[,] _cells;

        

        public CRGrid(int width,int height)
        {
            _width = width;
            _height = height;

            _cells = new Cell[width, height];
        }

        public List<Cell> GetAdjacentCells(Cell cell)
        {
            List<Cell> adjCells = new List<Cell>();

            //left
            if (cell.idx.Item1 - 1 >= 0)
                adjCells.Add(_cells[cell.idx.Item1 - 1, cell.idx.Item2]);

            //right
            if (cell.idx.Item1 + 1 < _width)
                adjCells.Add(_cells[cell.idx.Item1 + 1, cell.idx.Item2]);

            //up
            if (cell.idx.Item2 - 1 >= 0)
                adjCells.Add(_cells[cell.idx.Item1, cell.idx.Item2 - 1]);

            //down
            if (cell.idx.Item2 + 1 < _height)
                adjCells.Add(_cells[cell.idx.Item1, cell.idx.Item2 + 1]);


            return adjCells;
        }

    }
}
