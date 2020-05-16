using Interfaces;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using TradeLogic.Table;

namespace TradeControls.MainTable
{
    /// <summary>
    /// Interaction logic for MainTable.xaml
    /// </summary>
    public partial class MainTable : DataGrid, IDisposable
    {
        private List<Row> _rows;
        private List<Column> _columns;
        private ITableItem<ColumnParams> _item;


        public MainTable()
        {
            InitializeComponent();
        }

        public void Populate(ITableItem<ColumnParams> item)
        {
            _item = item;
            InitColumns();
            InitContextMenu();
        }

        private void InitContextMenu()
        {
            ContextMenu = new ContextMenu();
            foreach (var col in Columns)
            {
                MenuItem item = new MenuItem();
                item.Header = col.Header;
                item.Tag = col;
                item.IsCheckable = true;
                item.IsChecked = col.Visibility == Visibility.Visible;
                item.Click += ContextMenu_Item_Click;
                ContextMenu.Items.Add(item);
            }
        }

        private void InitColumns()
        {
            _columns = new List<Column>();
            for (int i = 0; i < _item.ColumnsCount; i++)
            {
                var par = _item.GetParams(i);
                if (!par.Visible)
                    continue;

                DataGridTextColumnAdvanced gridColumn = new DataGridTextColumnAdvanced(par);
                             
                //col.SetValue(DataGridTextColumn.ToolTip)
                Columns.Add(gridColumn);
            }
        }

        private void PopulateRows(List<ITableItem<ColumnParams>> items)
        {
            Items.Clear();
            foreach (var item in items)
            {
                dynamic row = new ExpandoObject();
                var rowToAdd = (IDictionary<string, object>)row;
                for (int i = 0; i < _item.ColumnsCount; i++)
                {
                    //row.Item = item;
                   
                    foreach (var c in Columns)
                    {
                        var col = c as DataGridTextColumnAdvanced;
                        DataGridCell cell = new DataGridCell();
                        cell.IsEditing = col.IsReadOnly;
                        cell.Content = item.GetStringValue(col.ColumnParams.Index);
                        rowToAdd[col.ColumnParams.HeaderLocalized] = cell.Content.ToString();

                    }
                }
                Items.Add(row);
            }             
        }

      

        public void SetRows(List<ITableItem<ColumnParams>> items) => PopulateRows(items);


        bool _disposed = false;
        public void Dispose()
        {
            if (!_disposed)
            {
                foreach (var item in ContextMenu.Items)
                {
                    var menuItem = item as MenuItem;
                    menuItem.Click -= ContextMenu_Item_Click;
                    menuItem.Tag = null;
                }
            }
            _disposed = true;
        }

        protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseRightButtonDown(e);
            
        }


       
        public void ShowContextMenu()
        {
          
        }

        private void ContextMenu_Item_Click(object sender, RoutedEventArgs e)
        {
            var item = sender as MenuItem;
            var col = item.Tag as DataGridColumn;
            if (item.IsChecked != (col.Visibility == Visibility.Visible))
            {
                col.Visibility = item.IsChecked ? Visibility.Visible : Visibility.Hidden;
            }
        }
    }
}
