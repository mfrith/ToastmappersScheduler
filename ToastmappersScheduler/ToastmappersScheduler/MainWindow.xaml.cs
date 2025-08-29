using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;


namespace Toastmappers
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private static MainViewModel _mainViewModel;
    private string _mode;

    //public MainWindow()
    //{ }
    public MainWindow(/*string mode*/)
    {
      //_mode = mode;
      InitializeComponent();
      //String[] args = App.mode;
      //var arr = args[0];
      _mainViewModel = new MainViewModel(/*_mode*/);
      DataContext = _mainViewModel;
      MainViewModel a = (MainViewModel)this.DataContext;
      _mainViewModel.CheckDataFiles();
      MeetingsViewModel b = (MeetingsViewModel)_mainViewModel.Tabs[2];
      //ObservableCollection<MeetingModelBase> meetings = b.Meetings;
      //DateTime lastMeeting = DateTime.ParseExact(meetings[0].DayOfMeeting, "MM-dd-yyyy", System.Globalization.CultureInfo.InvariantCulture);
      //bool resolved = "1" == meetings[0].Resolved;
      //if (!resolved)
      //{
      //  // pop up view
      //  //MeetingViewModel currentMeeting = new MeetingViewModel(meetings[0]);
      //  meetings[0].Resolved = "1";
      //}
      //load data files?
      // does it exist? if not create it. if it does, load it.
      //var now = DateTime.Today.ToShortDateString();
      //DateTime now2 = DateTime.Today.Date;
      // any meetings missed
    }

    private void GenerateMeeting_Click(object sender, RoutedEventArgs e)
    {
      MeetingView mv = new MeetingView();

      //mv.ShowDialog();
      // when is the next meeting?
      DateTime now = DateTime.Today.Date;
      MainViewModel a = (MainViewModel)this.DataContext;
      MeetingsViewModel b = (MeetingsViewModel)a.Tabs[2];
      ObservableCollection<MeetingModelBase> meetings = b.Meetings;
      DateTime lastMeeting = DateTime.ParseExact(meetings[0].DayOfMeeting, "MM-dd-yyyy", System.Globalization.CultureInfo.InvariantCulture);

      DayOfWeek dow = lastMeeting.DayOfWeek;
      if (dow == DayOfWeek.Friday) // last friday of the month?
      {
        // next meeting is Wednesday, mostly
        int month = lastMeeting.Month;
        int year = lastMeeting.Year;

        DateTime nextMonth = new DateTime(year, ++month, 1);
      }
      var t = this.Members;
      var Members = (MembersViewModel)a.Tabs[1];
      //List<MemberModel> members = Members.MemberList;
      ObservableCollection<MemberViewModel> members = Members.Members;
      DateTime dayofmeeting = now;
      var s1 = members.OrderBy(m => m.Speaker).First();
      members.Remove(s1);
      s1.Speaker = dayofmeeting;
      var s2 = members.OrderBy(m => m.Speaker).First();
      members.Remove(s2);
      s2.Speaker = dayofmeeting;

    }
    List<DateTime> GetMonthlyMeetings(DateTime startDate)
    {
      // assume startDate is a wednesday
      DateTime firstWednesday = startDate;
      DateTime secondWednesday = startDate.AddDays(7);
      DateTime thirdWednesday = startDate.AddDays(14);
      DateTime fourthWednesday = startDate.AddDays(21);
      DateTime fifthWednesday = startDate.AddDays(28);
      var daysinmonth = DateTime.DaysInMonth(startDate.Year, startDate.Month);
      DateTime lastDayOfMonth = new DateTime(2019, startDate.Month, daysinmonth);

      DateTime g = lastDayOfMonth;
      while (g.DayOfWeek != DayOfWeek.Friday)
      {
        g = g.AddDays(-1);
      }
      DateTime fridayMeeting = g;
      List<DateTime> meetings = new List<DateTime>();

      var month = startDate.Month;
      // handle months with holidays differently - November, December, January, July, etc
      if (month == 11) //november
      {
        // account for Thanksgiving
      }

      if (month == 12) //december
      {
        // account for Christmas
        meetings.Add(firstWednesday);
        meetings.Add(secondWednesday);
        meetings.Add(thirdWednesday);
        return meetings;
      }

      meetings.Add(firstWednesday);
      meetings.Add(secondWednesday);
      meetings.Add(thirdWednesday);
      if (fridayMeeting > thirdWednesday && fridayMeeting < fourthWednesday)
      {
        meetings.Add(fridayMeeting);
        meetings.Add(fourthWednesday);
      }
      else if (fridayMeeting > fourthWednesday && fridayMeeting < fifthWednesday)
      {
        meetings.Add(fourthWednesday);
        meetings.Add(fridayMeeting);
        if (fifthWednesday <= lastDayOfMonth)
          meetings.Add(fifthWednesday);
      }
      else if (fridayMeeting > fourthWednesday && fridayMeeting > fifthWednesday)
      {
        meetings.Add(fourthWednesday);
        meetings.Add(fifthWednesday);
        meetings.Add(fridayMeeting);
      }
      return meetings;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {

      //_mainViewModel.CheckDataFiles();
      MeetingsViewModel b = (MeetingsViewModel)_mainViewModel.Tabs[2];
      ObservableCollection<MeetingModelBase> meetings = b.Meetings;

      MembersViewModel c = (MembersViewModel)_mainViewModel.Tabs[1];
      var f = meetings.Where(it => it.Resolved == false).ToList();

      List<MeetingModelBase> meetingsToResolve = new List<MeetingModelBase>();

      foreach (MeetingModelBase mtg in f)
      {

        DateTime today = DateTime.Today.Date;
        var t = DateTime.Now;
        var mtgDate = DateTime.Parse(mtg.DayOfMeeting);
        var y = mtgDate.AddHours(13); // meeting time is 12:00 AM and we add 13 hours to get past 1:00 PM
        int rel = DateTime.Compare(y, t);
        if (rel < 0)
          meetingsToResolve.Add(mtg);
      }

      if (meetingsToResolve.Count > 0)
      {

        //bool resolve = today >= DateTime.Parse(meetingsToResolve[0].DayOfMeeting);

        MessageBoxResult result = MessageBox.Show("Do you want to resolve past meetings?", "", MessageBoxButton.YesNo);
        if (result == MessageBoxResult.Yes)
        {
          //foreach (var meeting in meetingsToResolve)
          //{
          //  //var meetingsR = meetingsToResolve.OrderBy
          //MeetingResolutionViewModel mtg2resolve = new MeetingResolutionViewModel(meeting, c.Members);
          MeetingsResolutionViewModel mrvm = new MeetingsResolutionViewModel(meetingsToResolve, c.Members, c.PastMembers, b);
          mrvm.Resolve();
          //MeetingResolutionView view = new MeetingResolutionView();
          //view.DataContext = mtg2resolve;
          //view.ShowDialog();
          //NewMemberDialog dialog = new NewMemberDialog();
          //bool? result = dialog.ShowDialog();
          //int t = 0;
          //if (result == true)
          //  t = 1;
          //else
          //  t = 2;wpf 
          //}
        }
      }
    }

    private bool _inDropDownClosed = false;
    private void Calendar_SelectedDatesChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
      var dates = MeetingsOutCalendar.SelectedDates;
      MemberViewModel member = (MemberViewModel)MemberInfoComboBox.SelectedItem;
      if (member == null)
        return;

      if (_inDropDownClosed)
        return;

      member.MeetingsOut.Clear();
      foreach (DateTime day in dates)
      {
        if (!member.MeetingsOut.Contains(day))
          member.MeetingsOut.Add(day);
      }
    }

    private void ComboBox_DropDownClosed(object sender, EventArgs e)
    {
      MemberViewModel member = (MemberViewModel)MemberInfoComboBox.SelectedItem;
      if (member == null)
        return;
      _inDropDownClosed = true;
      MeetingsOutCalendar.SelectedDates.Clear();
      List<DateTime> dates = member.MeetingsOut;
      if (dates != null)
      {
        foreach (DateTime day in dates)
          MeetingsOutCalendar.SelectedDates.Add(day);
      }
      _inDropDownClosed = false;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      MemberViewModel member = (MemberViewModel)MemberInfoComboBox.SelectedItem;
      if (member == null)
        return;
      _inDropDownClosed = true;
      MeetingsOutCalendar.SelectedDates.Clear();
      member.MeetingsOut.Clear();
      _inDropDownClosed = false;
    }

    private void ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
      MeetingRoleList.ItemsSource = null;
      if (MeetingRoles.SelectedItem == null)
        return;

      string role = MeetingRoles.SelectedItem?.ToString();
      MainViewModel a = (MainViewModel)this.DataContext;

      MembersViewModel c = (MembersViewModel)a.Tabs[1];
      List<string>? r = null;

      MeetingRoleList.ItemsSource = c.Members.OrderBy(it => it.GetType().GetProperty(role).GetValue(it)).Select(x => x.Name).ToList();
    }

    private void MentorsCombo_DropDownClosed(object sender, EventArgs e)
    {
      MemberViewModel member = (MemberViewModel)MemberInfoComboBox.SelectedItem;
      if (member == null)
        return;

      var t = sender as ComboBox;
      if (t == null || t.SelectedItem == null)
        return;

      member.Mentors.Add(t.Text);
    }

    private ICommand _backupCommand;
    public ICommand BackupCmd
    {
      get { return _backupCommand ?? (_backupCommand = new RelayCommand(() => Backup(), () => true)); }
    }

    public void Backup()
    {
      MainViewModel a = (MainViewModel)this.DataContext;
      a.Backup();
    }

    private void MeetingsOutCalendar_PreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
      if (Mouse.Captured is CalendarItem)
      {
        Mouse.Capture(null);
      }
    }

    private void Button_ClearAll(object sender, RoutedEventArgs e)
    {
      //MemberViewModel member = (MemberViewModel)MemberInfoComboBox.SelectedItem;
      //if (member == null)
      //  return;
      //_inDropDownClosed = true;
      //MeetingsOutCalendar.SelectedDates.Clear();
      //member.MeetingsOut.Clear();
      //_inDropDownClosed = false;
      MainViewModel a = (MainViewModel)this.DataContext;
      MembersViewModel c = (MembersViewModel)a.Tabs[1];
      foreach (MemberViewModel m in c.Members)
      {
        m.MeetingsOut.Clear();
      }
    }

    private void TTContestants_ComboBox_DropDownClosed(object sender, EventArgs e)
    {

        }
    }

}
