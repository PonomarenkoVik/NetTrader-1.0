using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using TradeLogic.Table;

namespace TradeControls.MainTable
{
    public class DataGridTextColumnAdvanced : DataGridTextColumn
    {
        private ColumnParams par;

        public DataGridTextColumnAdvanced(ColumnParams par)
        {
            this.par = par;
            IsReadOnly = par.IsReadOnly;
            Visibility = par.Visible ?  System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
            Header = par.HeaderLocalized;
            ColumnParams = par;
            Binding = new Binding(par.LocalizationKey);
        }

        public ColumnParams ColumnParams { get; private set; }
    }
}
