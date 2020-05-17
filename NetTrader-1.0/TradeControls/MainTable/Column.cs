namespace TradeControls.MainTable
{
    public class Column
    {
        public int DisplayIndex { get; set; }
        public int Index { get; private set; }
        public Column(int i)
        {
            DisplayIndex = Index = i;
        }

        public string Header { get; set; }
        public bool Visible { get; set; }
        public string ToolTip { get; set; }

        public object Tag { get; set; }
    }
}