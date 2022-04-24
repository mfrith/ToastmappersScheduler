using System.Windows;

namespace Toastmappers
{
  /// <summary>
  /// Interaction logic for NewMemberView.xaml
  /// </summary>
  public partial class NewMemberView : Window
    {
        public NewMemberView()
        {
            InitializeComponent();
        }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      DialogResult = true;
      Close();
    }

    private void Button_Click_1(object sender, RoutedEventArgs e)
    {
      //
      Close();
    }
  }
}
