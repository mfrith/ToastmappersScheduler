using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Toastmappers
{
  public class MemberViewModel : ViewModelBase, IEquatable<MemberViewModel>, IComparable<MemberViewModel>
  {
    private MemberModel _member;
    private bool _modified;


    public MemberViewModel(MemberModel member)
    {
      _member = member;
      _name = member.Name;
      _id = member.MemberID;
      _iscurrent = member.IsCurrentMember;
      _canBeEvaluator = member.CanBeEvaluator;
      _canBeToastmaster = member.CanBeToastmaster;
      _hasBeenOfficer = member.HasBeenOfficer;
      _mentors = member.Mentors;
      _id = member.MemberID;
      _toastmaster = member.Toastmaster;
      _speaker = member.Speaker;
      _eval = member.Evaluator;
      _ge = member.GeneralEvaluator;
      _gram = member.Gram;
      _ah = member.Ah;
      _timer = member.Timer;
      _hotSeat = member.HotSeat;
      _video = member.Video;
      _tt = member.TT;
      _quiz = member.Quiz;
      _meetingsOut = member.MeetingsOut;
    }

    private bool _hasBeenOfficer;
    public bool HasBeenOfficer
    {
      get { return _hasBeenOfficer; }
      set { SetProperty(ref _hasBeenOfficer, value, () => HasBeenOfficer); }
    }

    public MemberModel Member
    {
      get { return _member; }
    }
    private string _name;
    public string Name
    {
      get { return _name; }
      set { SetProperty(ref _name, value, () => Name); }
    }

    private bool _iscurrent;
    public bool IsCurrent
    {
      get { return _iscurrent; }
      set { SetProperty(ref _iscurrent, value, () => IsCurrent); }

    }

    private bool _canBeToastmaster;
    public bool CanBeToastmaster
    {
      get { return _canBeToastmaster; }
      set { SetProperty(ref _canBeToastmaster, value, () => CanBeToastmaster); }
    }

    private bool _canBeEvaluator;
    public bool CanBeEvaluator
    {
      get { return _canBeEvaluator; }
      set { SetProperty(ref _canBeEvaluator, value, () => CanBeEvaluator); }
    }

    private int _id;
    public int ID
    {
      get { return _id; }
      set { SetProperty(ref _id, value, () => ID); }
    }
    private ObservableCollection<string> _mentors;
    public ObservableCollection<string> Mentors
    {
      get { return _mentors; }
      set { SetProperty(ref _mentors, value, () => Mentors); }
    }

    private DateTime _toastmaster;
    public DateTime Toastmaster
    {
      get { return _toastmaster; }
      set { SetProperty(ref _toastmaster, value, () => Toastmaster); }
    }

    private DateTime _speaker;
    public DateTime Speaker
    {
      get { return _speaker; }
      set { SetProperty(ref _speaker, value, () => Speaker); }
    }

    private DateTime _ge;
    public DateTime GeneralEvaluator
    {
      get { return _ge; }
      set { SetProperty(ref _ge, value, () => GeneralEvaluator); }
    }

    private DateTime _eval;
    public DateTime Evaluator
    {
      get { return _eval; }
      set { SetProperty(ref _eval, value, () => Evaluator); }
    }

    private DateTime _tt;
    public DateTime TT
    {
      get { return _tt; }
      set { SetProperty(ref _tt, value, () => TT); }
    }

    private DateTime _ah;
    public DateTime Ah
    {
      get { return _ah; }
      set { SetProperty(ref _ah, value, () => Ah); }
    }

    private DateTime _gram;
    public DateTime Gram
    {
      get { return _gram; }
      set { SetProperty(ref _gram, value, () => Gram); }
    }

    private DateTime _timer;
    public DateTime Timer
    {
      get { return _timer; }
      set { SetProperty(ref _timer, value, () => Timer); }
    }

    private DateTime _quiz;
    public DateTime Quiz
    {
      get { return _quiz; }
      set { SetProperty(ref _quiz, value, () => Quiz); }
    }

    private DateTime _video;
    public DateTime Video
    {
      get { return _video; }
      set { SetProperty(ref _video, value, () => Video); }
    }

    private DateTime _hotSeat;
    public DateTime HotSeat
    {
      get { return _hotSeat; }
      set { SetProperty(ref _hotSeat, value, () => HotSeat); }
    }

    private List<DateTime> _meetingsOut;
    public List<DateTime> MeetingsOut
    {
      get { return _meetingsOut; }
      set { SetProperty(ref _meetingsOut, value, () => MeetingsOut); }
    }

    private DateTime _selectedDate;
    public DateTime SelectedDate
    {
      get
      {
        return _selectedDate;
      }
      set
      {
        _selectedDate = value;
      }
    }
    public bool Equals(MemberViewModel other)
    {
      if (other == null) return false;
      return (this.Name.Equals(other.Name));
    }

    public int CompareTo(MemberViewModel other)
    {
      if (other == null)
        return 1;
      else
        return this.Name.CompareTo(other.Name);
    }

  }
}
