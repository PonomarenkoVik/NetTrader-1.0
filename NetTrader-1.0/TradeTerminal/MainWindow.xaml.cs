using Interfaces;
using Interfaces.Messages;
using System;
using System.Collections.Generic;
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

namespace TradeTerminal
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ITradeLogic _core;
        private IVendor _vendor;
        public MainWindow()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            _core = new TradeCore();
            _vendor = _core.GetVendors().FirstOrDefault().Value;
            _vendor.LoadedEvent += Start;
        }

        private void Start()
        {
            var instruments = _vendor.GetAllInstruments();
            var instr1 = instruments["WMZ/WMR"];
            var instr2 = instruments["WMR/WMZ"];
            _vendor.Subscribe(new Subscription(instr1, SubscriptionType.TradingNormal));
            _vendor.Subscribe(new Subscription(instr2, SubscriptionType.TradingNormal));
            _vendor.OnNewQuoteEvent += OnQuote;
           
        }

        private void OnQuote(Quote3Message obj)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new Action(
                    delegate 
                    {
                        if (obj.InstrumentName == "WMZ/WMR")
                        {
                            Grid1.ItemsSource = obj.Orders;
                        }

                        if (obj.InstrumentName == "WMR/WMZ")
                        {
                            Grid2.ItemsSource = obj.Orders;
                        }

                    }));
            }
            else
                Grid1.ItemsSource = obj.Orders;
        }
    }
}
