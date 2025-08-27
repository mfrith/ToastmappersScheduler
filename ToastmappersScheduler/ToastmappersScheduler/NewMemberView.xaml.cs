using System;
using System.Windows;
using System.Windows.Controls;

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
      //NewMemberViewModel a = (NewMemberViewModel)this.DataContext;
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

    private void MentorsCombo_DropDownClosed(object sender, EventArgs e)
    {
      var t = sender as ComboBox;
      if (t == null || t.SelectedItem == null)
        return;

      NewMemberViewModel a = (NewMemberViewModel)this.DataContext;
      a.Mentors.Add(t.Text);
      //newMemberListBox.Items.Add(t.Text);

    }
  }

}
