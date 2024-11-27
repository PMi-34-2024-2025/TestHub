using System.Windows.Controls;


namespace PE.DesktopApplication.TestHub.WPF
{
    /// <summary>
    /// Interaction logic for AnswerControl.xaml
    /// </summary>
    public partial class AnswerControl : UserControl
    {
        public AnswerControl()
        {
            InitializeComponent();
        }
        public (string AnswerText, bool IsCorrect) GetAnswerData()
        {
            return (txtAnswer.Text, chkIsCorrect.IsChecked == true);
        }
    }
}
