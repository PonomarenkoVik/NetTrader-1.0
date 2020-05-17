using Interfaces;
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
using System.Windows.Threading;

namespace TradeControls.MainTable
{

    public enum ColumnTableAutoWidthMode { ByHeaders, ByTable, None }

    /// <summary>
    /// Interaction logic for UserControl.xaml
    /// </summary>
    public partial class MainTableAdvanced<T> : UserControl
    {
        private double _rowsHeight = 5;
        public const double ROWS_HEIGHT_MIN = 5;
        public const double ROWS_HEIGHT_MAX = 50;

        //public const int RENDER_INTERVAL = 200;
        //private DispatcherTimer _timer;

        public MainTableAdvanced()
        {
            InitializeComponent();
            //_timer = new DispatcherTimer(DispatcherPriority.Render) { Interval = new TimeSpan(TimeSpan.TicksPerMillisecond * RENDER_INTERVAL) };

        }

        public ColumnTableAutoWidthMode ColumnTableAutoWidthMode { get; set; } = ColumnTableAutoWidthMode.None;
        public List<ITableItem<T>> ItemsSource { get; set; }
        public List<Column> Columns { get; private set; } = new List<Column>();
        public List<Row> Rows { get; private set; } = new List<Row>();

        public double RowsHeight
        {
            get => _rowsHeight; 
            set
            {
                if (value < ROWS_HEIGHT_MIN)
                {
                    _rowsHeight = ROWS_HEIGHT_MIN;
                }
                else if (value > ROWS_HEIGHT_MAX)
                {
                    _rowsHeight = ROWS_HEIGHT_MAX;
                }
                else
                    _rowsHeight = value;
            }
        }
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            DrawColumns(drawingContext);
            DrawRows(drawingContext);
        }

        private void DrawRows(DrawingContext drawingContext)
        {
            throw new NotImplementedException();
        }

        private void DrawColumns(DrawingContext drawingContext)
        {
            if (double.IsNaN(this.ActualWidth) || Columns.Count == 0)
                return;

            switch (ColumnTableAutoWidthMode)
            {
                case ColumnTableAutoWidthMode.ByHeaders:
                    DrawColumnsNoneMode(drawingContext);
                    break;
                case ColumnTableAutoWidthMode.ByTable:
                    DrawColumnsByHeadersMode(drawingContext);
                    break;
                case ColumnTableAutoWidthMode.None:
                default:
                    DrawColumnsByTableMode(drawingContext);
                    break;
            }

        }

        private void DrawColumnsByTableMode(DrawingContext drawingContext)
        {
            double width = this.ActualWidth - Padding.Left - Padding.Right;
            double columnWidth = width / Columns.Count;
            double y = Padding.Left;
            double x = Padding.Top;
            foreach (var column in Columns)
            {
                //DrawCellBackGround( x, y, columnWidth, RowsHeight);
                DrawCellBorder();
                DrawCellContent();
            }
        }

        private void DrawCellBorder()
        {
            throw new NotImplementedException();
        }

        private void DrawCellContent()
        {
            throw new NotImplementedException();
        }

        private void DrawCellBackGround()
        {
            throw new NotImplementedException();
        }

        private void DrawColumnsByHeadersMode(DrawingContext drawingContext)
        {
            throw new NotImplementedException();
        }

        private void DrawColumnsNoneMode(DrawingContext drawingContext)
        {
            throw new NotImplementedException();
        }
    }
}
