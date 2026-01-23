using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
  /// Interaction logic for NewMeetingView.xaml
  /// </summary>
  public partial class NewMeetingView : Window
  {
    public NewMeetingView()
    {
      InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      DialogResult = false;
      Close();
    }

    private void Button_Click_1(object sender, RoutedEventArgs e)
    {
      DialogResult = true;
      Close();
    }

    private bool _inDropDownClosed = false;
    private void MemberInfoComboBox_DropDownClosed(object sender, EventArgs e)
    {
      MemberViewModel member = (MemberViewModel)MemberInfoComboBox.SelectedItem;
      if (member == null)
        return;
      _inDropDownClosed = true;
      //MeetingsOutCalendar.SelectedDates.Clear();
      List<DateTime> dates = member.MeetingsOut;
      //if (dates != null)
      //{
      //  foreach (DateTime day in dates)
      //    MeetingsOutCalendar.SelectedDates.Add(day);
      //}
      _inDropDownClosed = false;
    }

    private void MeetingRoles_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      MeetingRoleList.ItemsSource = null;
      if (MeetingRoles.SelectedItem == null)
        return;

      string role = MeetingRoles.SelectedItem?.ToString();
      MeetingEditViewModel a = (MeetingEditViewModel)this.DataContext;
      DateTime dayOfMeeting = DateTime.ParseExact(a.DayOfMeeting, "MM-dd-yyyy", System.Globalization.CultureInfo.InvariantCulture);
      //List<string>? r = null;
      ObservableCollection<MemberViewModel> tempMembers = new ObservableCollection<MemberViewModel>(a.Members);
      var iterationMembers = a.Members.Where(it => it.MeetingsOut.Contains(dayOfMeeting)).ToList();
      foreach (var im in iterationMembers)
        tempMembers.Remove(im);
      MeetingRoleList.ItemsSource = tempMembers.OrderBy(it => it.GetType().GetProperty(role).GetValue(it)).Select(x => x.Name).ToList();
    }
  }
}
