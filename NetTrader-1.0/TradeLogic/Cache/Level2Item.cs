using Interfaces;
using Interfaces.Additional_functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeLogic.Table;

namespace TradeLogic.Cache
{
    public class Level2Item : ITableItem<ColumnParams>
    {
        #region Properties
        public const int COL_APPLICATION_DATE = 0;
        public const int COL_ORDER_ID = 1;
        public const int COL_OUTCOME_SUMM = 2;
        public const int COL_INCOME_SUMM = 3;
        public const int COL_STRAIGHT_CROSS_RATE = 4;
        public const int COL_REVERSE_CROSS_RATE = 5;
        #endregion
        public int ColumnsCount => _params.Count;

        IOrder _order;
        public Level2Item(IOrder order)
        {
            _order = order;
        }


        public ColumnParams GetParams(int i)
        {
            var par = _params[i];
            par.Visible = true;
            return par;
        }

        public object GetValue(int index)
        {
            switch (index)
            {
                case COL_APPLICATION_DATE:
                    return _order.LastUpdateDate;

                case COL_ORDER_ID:
                    return _order.OrderId;

                case COL_OUTCOME_SUMM:                   
                    return _order.AmountOut;

                case COL_INCOME_SUMM:
                    return _order.AmountIn;

                case COL_STRAIGHT_CROSS_RATE:
                    return _order.StraitCrossrate;

                case COL_REVERSE_CROSS_RATE:
                    return _order.ReverseCrossrate;

                default:
                    return "N/A";
            }
        }

        public string GetStringValue(int index)
        {
            var val = GetValue(index);

            switch (index)
            {
                case COL_APPLICATION_DATE:
                    var date = _order.LastUpdateDate;
                    string res = date.ToString("MM.dd.yyyy H:mm:ss");
                    return res;            

                case COL_OUTCOME_SUMM:
                    return Helper.FormatValue(_order.AmountOut, _order.Instrument.Currency1);

                case COL_INCOME_SUMM:
                    return Helper.FormatValue(_order.AmountIn, _order.Instrument.Currency2);

                default:
                    return val != null ? val.ToString() : "N/A";
            }
        }

        private List<ColumnParams> _params = new List<ColumnParams>()
        {
             new ColumnParams("Level2Panel.ApplicationDate", COL_APPLICATION_DATE, Visibility.Visible, IsReadOnly.True),
             new ColumnParams("Level2Panel.OrderId", COL_ORDER_ID, Visibility.Visible, IsReadOnly.True),
             new ColumnParams("Level2Panel.OutcomeSumm", COL_OUTCOME_SUMM, Visibility.Visible, IsReadOnly.True),
             new ColumnParams("Level2Panel.IncomeSumm", COL_INCOME_SUMM, Visibility.Visible, IsReadOnly.True),
             new ColumnParams("Level2Panel.StraightCrossRate", COL_STRAIGHT_CROSS_RATE, Visibility.Visible, IsReadOnly.True),
             new ColumnParams("Level2Panel.ReverseCrossRate", COL_REVERSE_CROSS_RATE, Visibility.Visible, IsReadOnly.True),
        };
    }
}
