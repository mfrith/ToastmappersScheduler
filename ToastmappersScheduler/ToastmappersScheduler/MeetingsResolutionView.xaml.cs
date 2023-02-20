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
using System.Windows.Shapes;

namespace Toastmappers
{
  /// <summary>
  /// Interaction logic for MeetingsResolutionView.xaml
  /// </summary>
  public partial class MeetingsResolutionView : Window
  {
    public MeetingsResolutionView()
    {
      InitializeComponent();
    }

    private void ComboBox_DropDownClosed(object sender, EventArgs e)
    {
      var t = sender as ComboBox;
      if (t == null || t.SelectedItem == null)
        return;

      MeetingsResolutionViewModel mtgBeingResolved = (MeetingsResolutionViewModel)this.DataContext;
      var mtg = mtgBeingResolved.CurrentMeetingToResolve;
      string a = t.SelectedItem.ToString();
      mtg.AddAttendee(a);
    }

    private void GuestTextBox_KeyDown(object sender, KeyEventArgs e)
    {
      var t = sender as TextBox;
      if (string.IsNullOrEmpty(t.Text))
        return;

      if (e.Key == Key.Enter)// && t != null && string.IsNullOrEmpty(t.Text))
      {
        MeetingsResolutionViewModel mtgBeingResolved = (MeetingsResolutionViewModel)this.DataContext;
        var mtg = mtgBeingResolved.CurrentMeetingToResolve;
        mtg.AddGuest(t.Text);
        t.Text = "";
      }
    }

    private void TTContestants_ComboBox_DropDownClosed(object sender, EventArgs e)
    {
      var t = sender as ComboBox;
      if (t == null || t.SelectedItem == null)
        return;

      MeetingsResolutionViewModel mtgBeingResolved = (MeetingsResolutionViewModel)this.DataContext;
      var mtg = mtgBeingResolved.CurrentMeetingToResolve;
      string a = t.SelectedItem.ToString();
      mtg.AddTTContestant(a);
    }

        private void ComboBox_DropDownClosed_1(object sender, EventArgs e)
        {

        }

        private void TTWinner_Combobox_DropDownClosed(object sender, EventArgs e)
        {

        }

    private void TTContestants_PreViewKeyDown(object sender, KeyEventArgs e)
    {
      var t = sender as ComboBox;

      if (e.Key == Key.Enter)// && t != null && string.IsNullOrEmpty(t.Text))
      {
        MeetingsResolutionViewModel mtgBeingResolved = (MeetingsResolutionViewModel)this.DataContext;
        var mtg = mtgBeingResolved.CurrentMeetingToResolve;
        mtg.AddTTContestant(t.Text);
        t.Text = "";
      }
    }

    private void TTContestants_ComboBox_KeyDown(object sender, KeyEventArgs e)
    {
      var t = sender as ComboBox;
      if (string.IsNullOrEmpty(t.Text))
        return;
      if (e.Key == Key.Enter)// && t != null && string.IsNullOrEmpty(t.Text))
      {
        MeetingsResolutionViewModel mtgBeingResolved = (MeetingsResolutionViewModel)this.DataContext;
        var mtg = mtgBeingResolved.CurrentMeetingToResolve;
        mtg.AddTTContestant(t.Text);
        t.Text = "";
      }
    }
  }
}
