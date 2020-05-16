using Interfaces;
using Interfaces.Messages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public partial class MainWindow : Window, IProperties
    {
        private ITradeLogic _core;
        private IVendor _vendor;
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
                if (value != null && value.InsideProperties.TryGetValue(CORE_PROPERTIES, out Interfaces.Properties pr) && _core != null)
                {
                    _core.Properties = pr;
                }
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
            _vendor = _core.GetVendors().FirstOrDefault().Value;
            _vendor.LoadedEvent += Start;
            Grid1.Populate(new Level2Item(null));
        }

        private void Start()
        {
            var instruments = _vendor.GetAllInstruments();
            var instr1 = instruments["WMR/WMU"];
            var instr2 = instruments["WMU/WMR"];
            _vendor.Subscribe(new Subscription(instr1, SubscriptionType.QuickTrading));
            _vendor.Subscribe(new Subscription(instr2, SubscriptionType.QuickTrading));
            _vendor.OnNewQuoteEvent += OnQuote;
           
        }

        private void OnQuote(Quote3Message msg)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new Action(
                    delegate 
                    {
                        var items = msg.Orders.Select((o) => (ITableItem<ColumnParams>)(new Level2Item(o))).ToList();
                        Grid1.SetRows(items);

                    }));
            }
            else
                Grid1.ItemsSource = msg.Orders;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            PropertyHelper.SaveProperties(Properties);
            base.OnClosing(e);
        }
    }
}
