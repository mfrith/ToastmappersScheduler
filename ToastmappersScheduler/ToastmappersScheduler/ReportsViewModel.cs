using Toastmappers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Toastmappers
{
  public class ReportsViewModel : PropertyChangedBase
  {
    List<MeetingModelBase> _meetings;
    ObservableCollection<MemberViewModel> _members;
    private string _home = string.Empty;
    public ReportsViewModel(List<MeetingModelBase> meetings, ObservableCollection<MemberViewModel> members, string location)
    {
      _home = location;
      _meetings = meetings;
      _members = members;
    }

    public List<MeetingModelBase> Meetings
    {
      get { return _meetings; }
    }

    private ICommand _generateReport;
    public ICommand GenerateReportCmd
    {
      get
      {
        return _generateReport ?? (_generateReport = new RelayCommand(() => GenerateReport(), () => true));
      }
    }
    public bool MemberSet
    {
      get { return _mme == null ? false : true; }
    }

    private MemberViewModel _mme;
    public MemberViewModel SetMemberRoleStatus
    {
      get { return _mme; }
      set
      {
        SetProperty(ref _mme, value, () => SetMemberRoleStatus);
        NotifyPropertyChanged(() => MemberSet);
      }

      //NotifyPropertyChanged(() => ShowMemberRoles);

    }


    public ObservableCollection<MemberViewModel> Members
    {
      get
      { return _members; }

      set {; }
    }
    public void GenerateReport()
    {

      var last5 = _meetings.Skip(5).Take(6);
      HashSet<string> octoberAttendees = new HashSet<string>();
      foreach (var m in last5)
      {
        octoberAttendees.Add(m.AhCounter);
        octoberAttendees.Add(m.Evaluator1);
        octoberAttendees.Add(m.Evaluator2);
        octoberAttendees.Add(m.GeneralEvaluator);
        octoberAttendees.Add(m.Grammarian);
        octoberAttendees.Add(m.HotSeat);
        octoberAttendees.Add(m.QuizMaster);
        octoberAttendees.Add(m.Speaker1);
        octoberAttendees.Add(m.Speaker2);
        octoberAttendees.Add(m.Timer);
        octoberAttendees.Add(m.Toastmaster);
        octoberAttendees.Add(m.Video);
      }

      HashSet<string> members = new HashSet<string>();

      foreach (var mem in _members)
      {
        members.Add(mem.Name);
      }
      members.Remove("");

      HashSet<string> outcome = new HashSet<string>(members);
      IEnumerable<string> missed = members.Except(octoberAttendees);

    }

    public bool Toastmaster
    {
      get; set;
    }
    public bool Speaker { get; set; }
    public bool GeneralEvaluator { get; set; }
    public bool Evaluator { get; set; }
    public bool AhCounter { get; set; }
    public bool Grammarian { get; set; }
    public bool Timer { get; set; }
    public bool QuizMaster { get; set; }
    public bool Video { get; set; }
    public bool HotSeat { get; set; }

    public bool TableTopics { get; set; }

    private ICommand _findPastRoles;
    public ICommand FindPastRolesCmd
    {
      get
      {
        return _findPastRoles ?? (_findPastRoles = new RelayCommand(() => FindPastRoles(), () => true));
      }
    }

    private ObservableCollection<Tuple<string, List<string>>> _findResults = new ObservableCollection<Tuple<string, List<string>>>();

    public ObservableCollection<Tuple<string, List<string>>> FindResults
    {
      get { return _findResults; }
    }
    private List<string> _results;
    public List<string> Results
    {
      get { return _results; } 
    }
    public void FindPastRoles()
    {
      _findResults.Clear();

      if (Toastmaster)
      {
        var result = _meetings.Where(it => it.Toastmaster == _mme.Name).ToList();
        if (result.Count > 0)
        {
          var results = result.Select(it => it.DayOfMeeting).ToList();
          results.Reverse();
          var f = Tuple.Create("Toastmaster", results);
          _findResults.Add(f);
        }

      }

      if (Speaker)
      { 
        var result = _meetings.Where(it => it.Speaker1 == _mme.Name || it.Speaker2 == _mme.Name ).ToList();
        if (result.Count > 0)
        {
          var results = result.Select(it => it.DayOfMeeting).ToList();
          results.Reverse();

          var f = Tuple.Create("Speaker", results);
          _findResults.Add(f);

        }
      }

      if (Evaluator)
      {
        var result = _meetings.Where(it => it.Evaluator1 == _mme.Name || it.Evaluator2 == _mme.Name).ToList();
        if (result.Count > 0)
        {
          var results = result.Select(it => it.DayOfMeeting).ToList();
          results.Reverse();

          var f = Tuple.Create("Evaluator", results);
          _findResults.Add(f);
        }
      }

      if (GeneralEvaluator)
      {
        var result = _meetings.Where(it => it.GeneralEvaluator == _mme.Name).ToList();
        if (result.Count > 0)
        {
          var results = result.Select(it => it.DayOfMeeting).ToList();
          results.Reverse();
          var f = Tuple.Create("GeneralEvaluator", results);
          _findResults.Add(f);
        }

      }

      if (TableTopics)
      {
        var result = _meetings.Where(it => it is MeetingModelRegular).Cast<MeetingModelRegular>().Where(it => it.TableTopics == _mme.Name).ToList();
        if (result.Count > 0)
        {
          var results = result.Select(it => it.DayOfMeeting).ToList();
          results.Reverse();
          var f = Tuple.Create("Table Topics", results);
          _findResults.Add(f);
        }

      }

      if (AhCounter)
      {
        var result = _meetings.Where(it => it.AhCounter == _mme.Name).ToList();
        if (result.Count > 0)
        {
          var results = result.Select(it => it.DayOfMeeting).ToList();
          results.Reverse();
          var f = Tuple.Create("AhCounter", results);
          _findResults.Add(f);
        }

      }

      if (Timer)
      {
        var result = _meetings.Where(it => it.Timer == _mme.Name).ToList();
        if (result.Count > 0)
        {
          var results = result.Select(it => it.DayOfMeeting).ToList();
          results.Reverse();
          var f = Tuple.Create("Timer", results);
          _findResults.Add(f);
        }

      }

      if (Grammarian)
      {
        var result = _meetings.Where(it => it.Grammarian == _mme.Name).ToList();
        if (result.Count > 0)
        {
          var results = result.Select(it => it.DayOfMeeting).ToList();
          results.Reverse();
          var f = Tuple.Create("Grammarian", results);
          _findResults.Add(f);
        }

      }

      if (QuizMaster)
      {
        var result = _meetings.Where(it => it.QuizMaster == _mme.Name).ToList();
        if (result.Count > 0)
        {
          var results = result.Select(it => it.DayOfMeeting).ToList();
          results.Reverse();
          var f = Tuple.Create("Quiz Master", results);
          _findResults.Add(f);
        }

      }

      if (Video)
      {
        var result = _meetings.Where(it => it.Video == _mme.Name).ToList();
        if (result.Count > 0)
        {
          var results = result.Select(it => it.DayOfMeeting).ToList();
          results.Reverse();
          var f = Tuple.Create("Videographer", results);
          _findResults.Add(f);
        }

      }
      if (HotSeat)
      {
        var result = _meetings.Where(it => it.HotSeat == _mme.Name).ToList();
        if (result.Count > 0)
        {
          var results = result.Select(it => it.DayOfMeeting).ToList();
          results.Reverse();
          var f = Tuple.Create("Hotseat", results);
          _findResults.Add(f);
        }

      }

      if (_findResults.Count > 0)
        NotifyPropertyChanged(() => FindResults);

    }
  }
}
