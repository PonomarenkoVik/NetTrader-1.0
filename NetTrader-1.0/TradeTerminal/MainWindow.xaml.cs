using Interfaces;
using Interfaces.Messages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using TradeLogic;
using TradeLogic.Cache;
using TradeLogic.Table;

namespace TradeTerminal
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IProperties, IDisposable
    {
        private ITradeLogic _core;
        const string CORE_PROPERTIES = "coreProperties";
        public string PropertyId => "mainProperties";

        public Interfaces.Properties Properties
        {
            get
            {
                var props = new Interfaces.Properties(PropertyId);
                if (_core != null)
                {
                    props.InsideProperties.Add(CORE_PROPERTIES, _core.Properties);
                }
                return props;
            }
            set
            {
                if (value == null || value.Id != PropertyId)
                    return;

                if (value.InsideProperties.TryGetValue(CORE_PROPERTIES, out Interfaces.Properties corePr) && _core != null)
                    _core.Properties = corePr;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            _core = new TradeCore();
            Properties = PropertyHelper.ReadProperties();

            var vendor = _core.GetVendors().FirstOrDefault().Value;
            vendor.LoadedEvent += Start;
            Grid1.Items.Clear();
            Grid1.IsReadOnly = true;
            if (vendor.CacheIsLoaded)
                Start();
        }

        private void Start()
        {
            var vendor = _core.GetVendors().FirstOrDefault().Value;
            var instruments = vendor.GetAllInstruments();
            var instr1 = instruments["WMR/WMZ"];
            var instr2 = instruments["WMZ/WMR"];
            vendor.Subscribe(new Subscription(instr1, SubscriptionType.TradingNormal));
            vendor.Subscribe(new Subscription(instr2, SubscriptionType.TradingNormal));
            vendor.OnNewQuoteEvent += OnQuote;
           
        }

        private void OnQuote(Quote3Message msg)
        {
            Grid1.TableItems = msg.Orders.Select((o) => (ITableItem<ColumnParams>)(new Level2Item(o))).ToList();
        }

        public void Set(List<ITableItem<ColumnParams>> items)
        {
            var it = items.First();
            DataTable table = new DataTable();
            Grid1.ColumnWidth = DataGridLength.SizeToHeader;
            //Grid1.Wi
            table.Columns.Add(new DataColumn("Number"));
            for (int i = 0; i < it.ColumnsCount; i++)
            {
                var par = it.GetParams(i);
                if (!par.Visible)
                    continue;

                DataColumn col = new DataColumn(par.HeaderLocalized);
               
                col.ReadOnly = par.IsReadOnly;
                table.Columns.Add(col);
            }

            int number = 1;

            foreach (var item in items)
            {
                var row = table.NewRow();
                row["Number"] = number;
                for (int i = 0; i < it.ColumnsCount; i++)
                {
                    var par = item.GetParams(i);
                    if (!par.Visible)
                        continue;

                    row[par.HeaderLocalized] = item.GetStringValue(i);
                }
                number++;
                table.Rows.Add(row);
                //dynamic row = new ExpandoObject();
                //var rowToAdd = (IDictionary<string, object>)row;
                //for (int i = 0; i < _item.ColumnsCount; i++)
                //{
                //    //row.Item = item;

                //    foreach (var c in Columns)
                //    {
                //        var col = c as DataGridTextColumnAdvanced;
                //        DataGridCell cell = new DataGridCell();
                //        cell.IsEditing = col.IsReadOnly;
                //        cell.Content = item.GetStringValue(col.ColumnParams.Index);
                //        rowToAdd[col.ColumnParams.HeaderLocalized] = cell.Content.ToString();

                //    }
                //}
                //Items.Add(row);
            }
            Grid1.ItemsSource = table.AsDataView();
        }



        protected override void OnClosing(CancelEventArgs e)
        {
            PropertyHelper.SaveProperties(Properties);
            base.OnClosing(e);
            Dispose();
        }



        public void Dispose()
        {
            Grid1?.Dispose();
        }
    }
}
