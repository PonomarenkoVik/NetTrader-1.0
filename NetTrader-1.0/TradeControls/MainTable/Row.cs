using Interfaces;
using System.Collections.Generic;
using TradeLogic.Table;

namespace TradeControls.MainTable
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