using TradeLogic.Localization;

namespace TradeLogic.Table
{
    internal enum Visibility { Visible, Hidden }
    internal enum IsReadOnly { True, False }


    public class ColumnParams
    {
        private Visibility _defaultVisibility;
        private IsReadOnly _isReadOnly;

        public string LocalizationKey { get; private set; }
        public int Index { get; private set; }
        public bool IsDefaultVisible
        {
            get => _defaultVisibility == Visibility.Visible;
            set => _defaultVisibility = value ? Visibility.Visible : Visibility.Hidden;
        }
        public bool Visible { get; set; }
       
        public string HeaderLocalized => Resources.GetResources(LocalizationKey);

        public string ToolTip => Resources.GetResources($"{LocalizationKey}.descr");

        public bool IsReadOnly
        {
            get => _isReadOnly == Table.IsReadOnly.True;
            set => _isReadOnly = value ? Table.IsReadOnly.True : Table.IsReadOnly.False;
        }

        internal ColumnParams(string localizationKey, int index, Visibility isDefaultVisible, IsReadOnly isReadOnly)
        {
            this.LocalizationKey = localizationKey;
            this.Index = index;
            this._defaultVisibility = isDefaultVisible;
            this._isReadOnly = isReadOnly;
        }
    }
}