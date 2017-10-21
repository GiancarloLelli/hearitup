using Windows.UI.Xaml.Controls;

namespace HIU.Models.UI
{
    public class HamburgerMenuItem
    {
        public HamburgerMenuItem()
        {
            Visible = true;
        }

        public string Label { get; set; }
        public SymbolIcon Gliph { get; set; }
        public bool Visible { get; set; }
    }
}