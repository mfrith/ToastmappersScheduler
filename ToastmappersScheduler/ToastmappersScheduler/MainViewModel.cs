using Toastmappers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.IO.Compression;
using System.Windows.Input;

namespace Toastmappers
{
  public class InverseBoolConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return !(bool)value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotSupportedException();
    }
  }
  public class MainViewModel
  {
    ObservableCollection<object> _tabs;
    string _home;
    public MainViewModel()
    {
      _tabs = new ObservableCollection<object>();
      var t = Assembly.GetExecutingAssembly().Location;
      var e = Path.GetDirectoryName(t);

      _home = Path.GetDirectoryName(e);
      
      GeneralViewModel generalVM = new GeneralViewModel(_home);
      MembersViewModel membersVM = new MembersViewModel(_home, this);
      membersVM.Load();
      MeetingsViewModel meetingsVM = new MeetingsViewModel(membersVM.Members, _home);
      meetingsVM.Load();

      ReportsViewModel reportsVM = new ReportsViewModel(meetingsVM.Meetings.ToList(), membersVM.Members, _home);

      _tabs.Add(generalVM);
      _tabs.Add(membersVM);
      _tabs.Add(meetingsVM);
      _tabs.Add(reportsVM);

      // resolve outstanding meetings
      //ObservableCollection<MeetingModelRegular> meetings = meetingsVM.Meetings;
      //List<MeetingModelRegular> meetingsToResolve = new List<MeetingModelRegular>();
      //foreach (MeetingModelRegular mtg in meetings)
      //{
      //  var mtgDate = DateTime.Parse(mtg.DayOfMeeting);
      //  if ((mtg.Resolved == "0" || mtg.Resolved == null))// && (mtgDate
      //    meetingsToResolve.Add(mtg);
      //}
      //if (meetingsToResolve.Count > 0)
      //{
      //  MessageBoxResult result = MessageBox.Show("Do you want to resolve past meetings?", "", MessageBoxButton.YesNo);
      //  if (result == MessageBoxResult.Yes)
      //  {
      //    DateTime today = DateTime.Today.Date;

      //    foreach (var meeting in meetingsToResolve)
      //    {
      //      //var meetingsR = meetingsToResolve.OrderBy
      //      MeetingResolutionViewModel mtg2resolve = new MeetingResolutionViewModel(meeting, membersVM.Members);

      //      MeetingResolutionView view = new MeetingResolutionView();
      //      view.DataContext = mtg2resolve;
      //      view.ShowDialog();
      //      //NewMemberDialog dialog = new NewMemberDialog();
      //      //bool? result = dialog.ShowDialog();
      //      //int t = 0;
      //      //if (result == true)
      //      //  t = 1;
      //      //else
      //      //  t = 2;wpf 
      //    }
      //  }
      //}

    }
    public void CheckDataFiles()
    {

    }
    public ObservableCollection<object> Tabs { get { return _tabs; } }

    private ICommand _backupCommand;
    public ICommand BackupCmd
    {
      get { return _backupCommand ?? (_backupCommand = new RelayCommand(() => Backup(), () => true)); }
    }

    public void Backup()
    {
      DateTime dt = DateTime.Now;
      var a = dt.ToString("yyyy-MM-dd-HH-mm-ss");
      var path = _home + "\\Backup";
      var zipname = Path.ChangeExtension(Path.Combine(path, a), "zip");
      ZipFile.CreateFromDirectory(Path.Combine(_home, "Data"), zipname);
    }
  }
}
