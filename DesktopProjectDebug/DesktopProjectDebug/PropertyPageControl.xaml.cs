using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ElementHost = System.Windows.Forms.Integration.ElementHost;

namespace DesktopProjectDebug
{
    /// <summary>
    /// Interaction logic for PropertyPageControl.xaml
    /// </summary>
    public partial class PropertyPageControl : UserControl
    {
        public UIElement LastElement { get; set; }

        public UIElement FirstElement { get; set; }

        public PropertyPageControl()
        {
            InitializeComponent();

            // Find the stack panel children for lookup later. Finding the elements in the stack
            // panel is more robust to future change in case somebody adds another radio button
            // Note: System.Linq doesn't work on a UIElementCollection
            foreach (UIElement child in rootPanel.Children)
            {
                if (FirstElement == null)
                {
                    FirstElement = child;
                }
                LastElement = child;
            }
        }
    }
}
