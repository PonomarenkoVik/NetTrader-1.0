using Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
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
using TradeLogic.Cache;
using TradeLogic.Table;

namespace TradeControls.MainTable
{
    /// <summary>
    /// Interaction logic for MainTableView.xaml
    /// </summary>
    public partial class MainTableView : DataGrid, IDisposable
    {
        private List<ITableItem<ColumnParams>> _tableItems;

        private List<Column> _columns;
        private object _sync = new object();
        public MainTableView()
        {
            InitializeComponent();
            ContextMenu = new ContextMenu();
            _columns = new List<Column>();
        }


        public List<ITableItem<ColumnParams>> TableItems
        {

            get => _tableItems;
            set
            {
                _tableItems = value;

                if (_tableItems == null || _tableItems.Count == 0)
                    return;

                PopulateItems();
                if (!this.Dispatcher.CheckAccess())
                    this.Dispatcher.Invoke(() => PopulateContextMenu());
                else
                   PopulateContextMenu();
            }
        }

        private void PopulateContextMenu()
        {
            ContextMenu.Items.Clear();
            foreach (var col in Columns)
            {
                MenuItem menuItem = new MenuItem();
                menuItem.Header = col.Header;
                menuItem.IsCheckable = true;
                menuItem.IsChecked = col.Visibility == Visibility.Visible;
                menuItem.Tag = col;
                menuItem.Click += MenuItem_Click;
                ContextMenu.Items.Add(menuItem);
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            var col = menuItem.Tag as DataGridColumn;
            if (menuItem.IsChecked != (col.Visibility == Visibility.Visible))
            {
                col.Visibility = menuItem.IsChecked ? Visibility.Visible : Visibility.Hidden;
                var colState = _columns.FirstOrDefault((c) => c.Header == col.Header.ToString());
                if (colState != null)
                    colState.Visible = col.Visibility == Visibility.Visible;

            }
        }

        private void PopulateItems()
        {
            var initItem = _tableItems.First();

            if (!this.Dispatcher.CheckAccess())
                this.Dispatcher.Invoke(() => PopulateColumns(_tableItems));
            else
                PopulateColumns(_tableItems);

            DataTable table = new DataTable();
            for (int i = 0; i < initItem.ColumnsCount; i++)
            {
                var par = initItem.GetParams(i);
                if (!par.Visible)
                    continue;

                DataColumn col = new DataColumn(par.HeaderLocalized);
                table.Columns.Add(col);
            }
            foreach (var item in TableItems)
            {
                var row = table.NewRow();
                for (int i = 0; i < initItem.ColumnsCount; i++)
                {
                    var par = initItem.GetParams(i);
                    if (!par.Visible)
                        continue;
                    row[par.HeaderLocalized] = item.GetStringValue(i);
                }
                table.Rows.Add(row);
            }

            if (!this.Dispatcher.CheckAccess())
                this.Dispatcher.Invoke(() => { ItemsSource = table.AsDataView(); SetColumnState(); });
            else
            {
                ItemsSource = table.AsDataView();
                SetColumnState();
            }
        }

        public void SetRowBackColor(Level2Item level2Item, SolidColorBrush color)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(() => SetRowBackColor(level2Item, color));
                return;
            }

            foreach (var it in Items)
            {
                var item = it as DataRowView;
                bool found = true;
                int index = -1;
                for (int i = 0; i < level2Item.ColumnsCount; i++)
                {
                    var par = level2Item.GetParams(i);
                    if (item.Row[par.HeaderLocalized].ToString() != level2Item.GetStringValue(i))
                    {
                        found = false;
                        break;
                    }
                    index = i;
                }

                //if (found)
                //{
                //    var row = (DataGridRow)ItemContainerGenerator.ContainerFromIndex(index);
                //    if (row != null)
                //        row.Background = color;
                //}
            }
        }

        private void SetColumnState()
        {
            foreach (var col in Columns)
            {
                var colState = _columns.FirstOrDefault((c) => c.Header == col.Header.ToString());
                if (colState == null)
                    continue;
                col.Visibility = colState.Visible ? Visibility.Visible : Visibility.Hidden;
                col.DisplayIndex = colState.DisplayIndex;
            }
        }

        private bool CheckColumns(List<ITableItem<ColumnParams>> items)
        {
            var initItem = items.First();

            if (_columns.Count == 0 || _columns.Count != initItem.ColumnsCount)
                return false;

            foreach (var col in Columns)
            {
                bool found = false;
                for (int i = 0; i < initItem.ColumnsCount; i++)
                {
                    var par = initItem.GetParams(i);
                    if (col.Header.ToString() == par.HeaderLocalized)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                    return false;
            }

            return true;
        }

        private void PopulateColumns(List<ITableItem<ColumnParams>> items)
        {
            if (CheckColumns(_tableItems))
                return;

            lock (_columns)
            {
                _columns.Clear();
                var initItem = items.First();
                for (int i = 0; i < initItem.ColumnsCount; i++)
                {
                    var par = initItem.GetParams(i);
                    if (!par.Visible)
                        continue;

                    var col = new Column(i);
                    col.Header = par.HeaderLocalized;
                    col.ToolTip = par.ToolTip;
                    col.Visible = par.IsDefaultVisible;
                    _columns.Add(col);
                }
            }
           
        }

        public void Dispose()
        {
            foreach (var item in ContextMenu.Items)
            {
                var menuItem = item as MenuItem;
                menuItem.Click -= MenuItem_Click;
                menuItem.Tag = null;
            }
        }


        protected override void OnColumnDisplayIndexChanged(DataGridColumnEventArgs e)
        {
            base.OnColumnDisplayIndexChanged(e);
            lock (_columns)
            {
                var colState = _columns.FirstOrDefault((c) => c.Header == e.Column.Header.ToString());
                if (colState != null)
                    colState.DisplayIndex = e.Column.DisplayIndex;
            }        
        }
    }
}
