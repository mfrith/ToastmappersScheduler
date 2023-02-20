using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Toastmappers
{

  public class MeetingsResolutionViewModel : ViewModelBase
  {
    private MeetingEditViewModel _currentMeetingToResolveVM;
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
      CurrentMeetingToResolve = new MeetingEditViewModel(_meetingsToResolve[0], _members);
      _currentMeetingIndex++;
      view.DataContext = this;
      view.ShowDialog();
    }

    public MeetingEditViewModel CurrentMeetingToResolve
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
      _currentMeetingToResolveVM = new MeetingEditViewModel(_meetingsToResolve[_currentMeetingIndex], _members);
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
      _currentMeetingToResolveVM = new MeetingEditViewModel(_meetingsToResolve[_currentMeetingIndex], _members);
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

      // update member status from resolved meeting
      // find each member's status and see if this the newest date for that role
      var date = DateTime.ParseExact(_currentMeetingToResolveVM.DayOfMeeting, "mm-dd-yyyy", System.Globalization.CultureInfo.InvariantCulture);
      //var mtg = DateTime.ParseExact(it.DayOfMeeting, "MM-dd-yyyy", System.Globalization.CultureInfo.InvariantCulture).CompareTo(today) > 0).FirstOrDefault();

      // for each meeting role, check if that's newer than the current member status
      var tm = _currentMeetingToResolveVM.Toastmaster;
      var member = _members.Where(it => it.Name == tm).First();
      var newDate = member.Toastmaster.AddMinutes(2); ;
      var bNewerDate = date.CompareTo(member.Toastmaster.AddMinutes(2)) > 0;
      if (member != null && bNewerDate)
      {
        member.Toastmaster = date;
      }

      var spkr1 = _currentMeetingToResolveVM.Speaker1;
      member = _members.Where(it => it.Name == spkr1).First();
      newDate = member.Speaker.AddMinutes(2); ;
      bNewerDate = date.CompareTo(member.Speaker.AddMinutes(2)) > 0;
      if (member != null && bNewerDate)
      {
        member.Speaker = date;
      }

      var spkr2 = _currentMeetingToResolveVM.Speaker2;
      member = _members.Where(it => it.Name == spkr2).First();
      newDate = member.Speaker.AddMinutes(2); ;
      bNewerDate = date.CompareTo(member.Speaker.AddMinutes(2)) > 0;
      if (member != null && bNewerDate)
      {
        member.Speaker = date;
      }

      var eval1 = _currentMeetingToResolveVM.Evaluator1;
      member = _members.Where(it => it.Name == eval1).First();
      newDate = member.Evaluator.AddMinutes(2); ;
      bNewerDate = date.CompareTo(member.Evaluator.AddMinutes(2)) > 0;
      if (member != null && bNewerDate)
      {
        member.Evaluator = date;
      }

      var eval2 = _currentMeetingToResolveVM.Evaluator2;
      member = _members.Where(it => it.Name == eval2).First();
      newDate = member.Evaluator.AddMinutes(2); ;
      bNewerDate = date.CompareTo(member.Evaluator.AddMinutes(2)) > 0;
      if (member != null && bNewerDate)
      {
        member.Evaluator = date;
      }

      var GE = _currentMeetingToResolveVM.GeneralEvaluator;
      member = _members.Where(it => it.Name == GE).First();
      newDate = member.GeneralEvaluator.AddMinutes(2); ;
      bNewerDate = date.CompareTo(member.GeneralEvaluator.AddMinutes(2)) > 0;
      if (member != null && bNewerDate)
      {
        member.GeneralEvaluator = date;
      }

      var TT = _currentMeetingToResolveVM.TableTopics;
      member = _members.Where(it => it.Name == TT).First();
      newDate = member.TT.AddMinutes(2); ;
      bNewerDate = date.CompareTo(member.TT.AddMinutes(2)) > 0;
      if (member != null && bNewerDate)
      {
        member.TT = date;
      }

      var gram = _currentMeetingToResolveVM.Grammarian;
      member = _members.Where(it => it.Name == gram).First();
      newDate = member.Gram.AddMinutes(2); ;
      bNewerDate = date.CompareTo(member.Gram.AddMinutes(2)) > 0;
      if (member != null && bNewerDate)
      {
        member.Gram = date;
      }

      var timer = _currentMeetingToResolveVM.Timer;
      member = _members.Where(it => it.Name == timer).First();
      newDate = member.Timer.AddMinutes(2); ;
      bNewerDate = date.CompareTo(member.Timer.AddMinutes(2)) > 0;
      if (member != null && bNewerDate)
      {
        member.Timer = date;
      }

      var ah  = _currentMeetingToResolveVM.AhCounter;
      member = _members.Where(it => it.Name == eval1).First();
      newDate = member.Ah.AddMinutes(2); ;
      bNewerDate = date.CompareTo(member.Ah.AddMinutes(2)) > 0;
      if (member != null && bNewerDate)
      {
        member.Ah = date;
      }

      var quiz = _currentMeetingToResolveVM.QuizMaster;
      if (!string.IsNullOrEmpty(quiz))
      {
        member = _members.Where(it => it.Name == quiz).First();
        newDate = member.Quiz.AddMinutes(2); ;
        bNewerDate = date.CompareTo(member.Quiz.AddMinutes(2)) > 0;
        if (member != null && bNewerDate)
        {
          member.Quiz = date;
        }
      }

      var video = _currentMeetingToResolveVM.Video;
      if (!string.IsNullOrEmpty(video))
      {
        member = _members.Where(it => it.Name == video).First();
        newDate = member.Video.AddMinutes(2); ;
        bNewerDate = date.CompareTo(member.Video.AddMinutes(2)) > 0;
        if (member != null && bNewerDate)
        {
          member.Video = date;
        }
      }

      var hotseat = _currentMeetingToResolveVM.HotSeat;
      if (!string.IsNullOrEmpty(hotseat))
      {
        member = _members.Where(it => it.Name == hotseat).First();
        newDate = member.HotSeat.AddMinutes(2); ;
        bNewerDate = date.CompareTo(member.HotSeat.AddMinutes(2)) > 0;
        if (member != null && bNewerDate)
        {
          member.HotSeat = date;
        }
      }

      var t = (MainViewModel)Application.Current.MainWindow.DataContext;
      MembersViewModel members = (MembersViewModel)t.Tabs[1];
      members.SaveMembers();
      
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
