using Toastmappers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Toastmappers
{
  public class ReportsViewModel
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
      members.Remove("Lisa Winn");
      members.Remove("Mani Vijayakumar");
      members.Remove("");

      HashSet<string> outcome = new HashSet<string>(members);
      IEnumerable<string> missed = members.Except(octoberAttendees);

    }

  }
}
