using Toastmappers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Toastmappers
{
  public class MeetingTemplateSelector : DataTemplateSelector
  {
    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
      FrameworkElement element = container as FrameworkElement;
      
      if (item != null)
      {

        var mtg = item as EditMeetingViewModel;
        if (mtg.MeetingType == 1)
          return element.FindResource("RegularMeetingTemplate") as DataTemplate;
        else if (mtg.MeetingType == 2)
          return element.FindResource("RegularMeetingTemplate2") as DataTemplate;

      }
      return base.SelectTemplate(item, container);
    }
  }
  public class MeetingsResolutionViewModel : ViewModelBase
  {
    private EditMeetingViewModel _currentMeetingToResolveVM;
    private List<MeetingModelBase> _meetingsToResolve;
    private ObservableCollection<MemberViewModel> _members;
    private int _meetingCount = 0;
    private int _currentMeetingIndex = -1;
    private MeetingsViewModel _meetingsVM;
    public MeetingsResolutionViewModel()
    {

    }
    public MeetingsResolutionViewModel(List<MeetingModelBase> meetingsToResolve, ObservableCollection<MemberViewModel> members, MeetingsViewModel meetingsVM)
    {
      _meetingsToResolve = meetingsToResolve;
      _meetingCount = _meetingsToResolve.Count;
      _meetingsVM = meetingsVM;
      _members = members;
    }

    public void Resolve()
    {
      MeetingsResolutionView view = new MeetingsResolutionView();
      //MeetingResolutionView view = new MeetingResolutionView();
      CurrentMeetingToResolve = new EditMeetingViewModel(_meetingsToResolve[0], _members);
      _currentMeetingIndex++;
      view.DataContext = this;
      view.ShowDialog();
    }

    public EditMeetingViewModel CurrentMeetingToResolve
    {
      get { return _currentMeetingToResolveVM; }
      set { _currentMeetingToResolveVM = value; }
    }

    private ICommand _nextMeetingToResolveCommand;
    public ICommand NextMeetingToResolveCmd
    {
      get
      {
        return _nextMeetingToResolveCommand ?? (_nextMeetingToResolveCommand = new RelayCommand(() => NextMeetingToResolve(), () => CanExecuteNextMeetingToResolve));
      }
    }

    public void NextMeetingToResolve()
    {
      _currentMeetingIndex++;
      //
      _currentMeetingToResolveVM = new EditMeetingViewModel(_meetingsToResolve[_currentMeetingIndex], _members);
      NotifyPropertyChanged(() => CurrentMeetingToResolve);
    }

    public bool CanExecuteNextMeetingToResolve
    {
      get
      {
        // check if executing is allowed, i.e., validate, check if a process is running, etc. 
        // has the meeting happened yet?
        int nextMeeting = _currentMeetingIndex + 1;
        if (nextMeeting + 1 > _meetingCount)
          return false;
        else
          return true; ;
      }
    }

    private ICommand _prevMeetingToResolveCommand;
    public ICommand PreviousMeetingToResolveCmd
    {
      get
      {
        return _prevMeetingToResolveCommand ?? (_prevMeetingToResolveCommand = new RelayCommand(() => PreviousMeetingToResolve(), () => CanExecutePreviousMeetingToResolve));
      }
    }

    public void LoadMembers()
    {

    }
    public void PreviousMeetingToResolve()
    {
      _currentMeetingIndex--;
      // need to reset member list and remove already attended members from the list
      LoadMembers();
      MeetingModelBase mtg = _meetingsToResolve[_currentMeetingIndex];
      var r = mtg.Attendees;
      foreach (var name in r)
        _members.Remove(_members.Single(iterator => iterator.Name == name));
      _currentMeetingToResolveVM = new EditMeetingViewModel(_meetingsToResolve[_currentMeetingIndex], _members);
      NotifyPropertyChanged(() => CurrentMeetingToResolve);
    }

    public bool CanExecutePreviousMeetingToResolve
    {
      get
      {
        // check if executing is allowed, i.e., validate, check if a process is running, etc. 
        int previousMeeting = _currentMeetingIndex - 1;
        if (previousMeeting < 0)
          return false;
        else
          return true; ;
      }
    }

    private ICommand _saveMeetingCommand;
    public ICommand SaveMeetingCmd
    {
      get
      {
        return _saveMeetingCommand ?? (_saveMeetingCommand = new RelayCommand(() => SaveMeeting(), () => CanExecuteSaveMeeting));
      }
    }

    public void SaveMeeting()
    {
      // save all? or just mark as resolved and save when we close?
      //_currentMeetingToResolveVM
      _currentMeetingToResolveVM.Save();
      _meetingsVM.Save();
      //var t = _currentMeetingToResolveVM.Resolved;
    }

    public bool CanExecuteSaveMeeting
    {
      get
      {
          return true; ;
      }
    }
  }
}
