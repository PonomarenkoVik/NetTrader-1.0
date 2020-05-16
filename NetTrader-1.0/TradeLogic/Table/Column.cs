namespace TradeLogic.Table
{
    public class Column
    {
        public int VisibilityIndex { get; set; }
        public int Index { get; private set; }
        public Column(int i)
        {
            VisibilityIndex = Index = i;
        }

        public string Header { get; set; }
        public bool Visible { get; set; }
        public string ToolTip { get; set; }

        public object Tag { get; set; }
    }
}