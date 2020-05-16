using Interfaces;
using System.Collections.Generic;

namespace TradeLogic.Table
{
    public class Row
    {
        private List<Cell> _cells;
        public ITableItem<ColumnParams> Item { get; private set; }
        public int CellsCount => _cells.Count;
        public Row(ITableItem<ColumnParams> item)
        {
            Item = item;
        }
    }
}