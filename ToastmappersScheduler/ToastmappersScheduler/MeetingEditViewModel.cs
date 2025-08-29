using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Windows.Input;
using System.Windows.Input;
using System;

namespace Toastmappers
{
  public class MeetingEditViewModel : ViewModelBase
  {
    readonly List<string> regularTemplate = new(["DayOfMeeting","Toastmaster","Speaker 1","Speaker 2","General Evaluator",
                                                                  "Evaluator 1", "Evaluator 2", "Table Topics", "Ah Counter",
                                                                  "Timer", "Grammarian", "Quiz Master", "Video", "Hot Seat" ]);

    readonly List<string> regularTemplateOutput = new(["DayOfMeeting","Toastmaster","Speaker1","Speaker2","GeneralEvaluator",
                                                                  "Evaluator1", "Evaluator2", "TableTopics", "AhCounter",
                                                                  "Timer", "Grammarian", "QuizMaster", "Video", "HotSeat" ]);
    readonly List<string> threeSpeakerTemplate = new(["Toastmaster","Speaker 1","Speaker 2", "Speaker 3", "General Evaluator",
                                                                  "Evaluator 1", "Evaluator 2", "Evaluator 3", "Ah Counter",
                                                                  "Timer", "Grammarian", "Quiz Master", "Video", "Hot Seat" ]);

    readonly List<string> speakathonTemplate = new(["Toastmaster","Speaker 1","Speaker 2", "Speaker 3", "Speaker 4",
                                                                  "Speaker 5", "General Evaluator", "Evaluator 1", "Evaluator 2", "Evaluator 3",
                                                                  "Evaluator 4", "Evaluator 5", "Ah Counter",
                                                                  "Timer", "Grammarian", "Quiz Master", "Video", "Hot Seat" ]);
    #region privates
    private static readonly KeyValuePair<string, string>[] meetingTemplates =
    {
        new KeyValuePair<string, string>("regularTemplate","Regular Meeting"),
        new KeyValuePair<string, string>("threeSpeakerTemplate", "Three Speaker Meeting"),
        new KeyValuePair<string, string>("speakathonTemplate", "Speakathon")
    };
    private string _template = string.Empty;
    private MeetingModelBase meetingModel;
    //private DateTime DayOfMeeting { get; set; }
    //private MemberModel Toastmaster { get; set; }
    //private MemberModel Speaker1 { get; set; }
    //private MemberModel Speaker2 { get; set; }
    //private MemberModel GeneralEvaluator { get; set; }
    //private MemberModel Evaluator1 { get; set; }
    //private MemberModel Evaluator2 { get; set; }
    //private MemberModel TT { get; set; }
    //private MemberModel Ah { get; set; }
    //private MemberModel Gram { get; set; }
    //private MemberModel Timer { get; set; }
    //private MemberModel Quiz { get; set; }
    //private MemberModel Video { get; set; }
    //private MemberModel HotSeat { get; set; }
    //private List<int> Attendees { get; set; }
    //private MemberModel TTWinner { get; set; }
    //private List<int> TTContestants { get; set; }
    public ObservableCollection<MemberViewModel> _members;

    #endregion
    public int MeetingType { get; set; }
    public string ID { get; set; }
    private string _toastmaster;

    private string _dayOfMeeting = "";
    private void setBorderFlag(string role)
    {
    }
    public string DayOfMeeting
    {
      get { return _dayOfMeeting; } 
      set { _dayOfMeeting = value; }
    }
    public string Toastmaster
    {
      get { return _meeting.Toastmaster; }
      set { _meeting.Toastmaster = value; if (CheckDuplicateRole(value)) setBorderFlag("Toastmaster"); }
    }

    public string Speaker1
    {
      get { return _meeting.Speaker1; }
      set { _meeting.Speaker1 = value; }
    }

    public string Speaker2
    {
      get { return _meeting.Speaker2; }
      set { _meeting.Speaker2 = value; }
    }
    public string Speaker3 { get; set; }
    public string Speaker4 { get; set; }
    public string Speaker5 { get; set; }
    public string GeneralEvaluator
    {
      get { return _meeting.GeneralEvaluator; }
      set { _meeting.GeneralEvaluator = value; }
    }
    public string Evaluator1
    {
      get { return _meeting.Evaluator1; }
      set { _meeting.Evaluator1 = value; }
    }
    public string Evaluator2
    {
      get { return _meeting.Evaluator2; }
      set { _meeting.Evaluator2 = value; }
    }
    public string Evaluator3 { get; set; }
    public string Evaluator4 { get; set; }
    public string Evaluator5 { get; set; }
    public string AhCounter
    {
      get { return _meeting.AhCounter; }
      set { _meeting.AhCounter = value; }
    }

    public string Grammarian
    {
      get { return _meeting.Grammarian; }
      set { _meeting.Grammarian = value; }
    }

    public string Timer
    {
      get { return _meeting.Timer; }
      set { _meeting.Timer = value; }
    }

    public string QuizMaster
    {
      get { return _meeting.QuizMaster; }
      set { _meeting.QuizMaster = value; }
    }

    public string Video
    {
      get { return _meeting.Video; }
      set { _meeting.Video = value; }
    }

    public string HotSeat
    {
      get { return _meeting.HotSeat; }
      set { _meeting.HotSeat = value; }
    }
    private bool CheckDuplicateRole(string name)
    {
      
      return _namesWithRole.Contains(name);
    }

    private ObservableCollection<string> _attendees;

    public ObservableCollection<string> Attendees
    {
      get { return _attendees; }
      set
      {
        SetProperty(ref _attendees, value, () => Attendees);
        NotifyPropertyChanged(() => MembersList);
      }
    }
    public bool Resolved { get; set; }
    public string TableTopics
    {
      get { return (_meeting as MeetingModelRegular).TableTopics; }
      set { (_meeting as MeetingModelRegular).TableTopics = value; }
    }
    public string Month { get; set; }
    public string Year { get; set; }
    public string WOTD { get; set; }
    public string Theme { get; set; }

    private string _TTWinner = string.Empty;

    public string TTWinner
    {
      get { return _TTWinner; }
      set { _TTWinner = value; }
    }
    private ObservableCollection<string> _ttcontestants;
    public ObservableCollection<string> TTContestants
    {
      get { return _ttcontestants; }
      set
      {
        _ttcontestants = value;
        NotifyPropertyChanged(() => TTContestants);
      }
    }
    public void AddTTContestant(string name)
    {
      TTContestants.Add(name);
      NotifyPropertyChanged(() => TTContestants);

      //if (string.IsNullOrEmpty(_ttcontestantmembers.FirstOrDefault(it => it.Name == name).Name.ToString())

       // _ttcontestantmembers.Remove(_ttcontestantmembers.Single(it => it.Name == name));
      NotifyPropertyChanged(() => TTContestantMembersList);

    }
    
    private ObservableCollection<MemberViewModel> _ttcontestantmembers;
    public ObservableCollection<string> TTContestantMembersList
    {
      get
      {
        var a = _ttcontestantmembers.Select(iterator => iterator.Name).ToList();
        ObservableCollection<string> newList = new(a);
        return newList;
      }
    } 

    private ObservableCollection<string> _guests;
    public ObservableCollection<string> Guests
    {
      get { return _guests; }
      set
      {
        _guests = value;
        NotifyPropertyChanged(() => Guests);
      }
    }

    public void AddGuest(string name)
    {
      Guests.Add(name);
      NotifyPropertyChanged(() => Guests);

    }

    private ICommand _deleteGuestCommand;
    public ICommand DeleteGuestCmd
    {
      get { return _deleteGuestCommand ??= new RelayCommand((p) => DeleteGuest(p), () => true); }
    }

    private void DeleteGuest(object item)
    {
      // need to create item or pass name to delete here
      // which list are we in?
      var tr = item.ToString();
      if (string.IsNullOrEmpty(tr))
        return;
      Guests.Remove(tr);
    }

    private ICommand _deleteAttendeeCommand;
    public ICommand DeleteAttendeeCmd
    {
      get { return _deleteAttendeeCommand ??= new RelayCommand((p) => DeleteAttendee(p), () => true); }
    }

    private void DeleteAttendee(object item)
    {
      // need to create item or pass name to delete here
      // which list are we in?
      var tr = item.ToString();
      if (string.IsNullOrEmpty(tr))
        return;
      Attendees.Remove(tr);
    }

    private ICommand _deleteContestantCommand;
    public ICommand DeleteContestantCmd
    {
      get { return _deleteContestantCommand ??= new RelayCommand((p) => DeleteContestant(p), () => true); }
    }

    private void DeleteContestant(object item)
    {
      // need to create item or pass name to delete here
      // which list are we in?
      var tr = item.ToString();
      if (string.IsNullOrEmpty(tr))
        return;
      TTContestants.Remove(tr);
    }
    public MeetingEditViewModel()
    { }

    private List<string> _newMeeting;
    public MeetingEditViewModel(List<string> newMeeting)
    {

    }
    public List<string> SpeakerEvaluatorLessList
    {
      get
      {

        return new List<string>();
      }
    }

    public ObservableCollection<string> MembersList
    {
      get
      {
        var a = _members.Select(iterator => iterator.Name).ToList();
        ObservableCollection<string> newList = new(a)
        {
          ""
        };
        return newList;
      }
    }

    private ObservableCollection<MemberViewModel> _attendeemembers;
    public ObservableCollection<string> AttendeesMembersList
    {
      get
      {
        var a = _attendeemembers.Select(iterator => iterator.Name).ToList();
        ObservableCollection<string> newList = new(a);
        return newList;
      }
    }

    public void AddAttendee(string name)
    {
      Attendees.Add(name);
      _attendeemembers.Remove(_attendeemembers.Single(it => it.Name == name));
      NotifyPropertyChanged(() => Attendees); 
      NotifyPropertyChanged(() => AttendeesMembersList);

    }
    private List<string> _namesWithRole = new List<string>();

    private MeetingModelBase _meeting;
    private bool _bResolve;
    public MeetingEditViewModel(MeetingModelBase mtgToEdit, ObservableCollection<MemberViewModel> members, bool bResolve = true)
    {
      _members = members;
      //var iterationMembers = members.Where(it => it.MeetingsOut.Contains(DateTime.ParseExact(mtgToEdit.DayOfMeeting, "MM-dd-yyyy", System.Globalization.CultureInfo.InvariantCulture))).ToList();
      //foreach (var im in iterationMembers)
      //  _members.Remove(im);
      
      _meeting = mtgToEdit;

      _attendeemembers = new ObservableCollection<MemberViewModel>(_members);
      _ttcontestantmembers = new ObservableCollection<MemberViewModel>(_members);
      Toastmaster = _meeting.Toastmaster;
      Speaker1 = _meeting.Speaker1;
      Speaker2 = _meeting.Speaker2;
      if (mtgToEdit.MeetingType == 2)
        Speaker3 = "Some speaker";
      GeneralEvaluator = _meeting.GeneralEvaluator;
      Evaluator1 = _meeting.Evaluator1;
      Evaluator2 = _meeting.Evaluator2;
      if (mtgToEdit.MeetingType == 1)
      {
        TableTopics = (_meeting as MeetingModelRegular).TableTopics;
        TTWinner = (_meeting as MeetingModelRegular).TTWinner;
        TTContestants = (_meeting as MeetingModelRegular).TTContestants;
      }
      AhCounter = _meeting.AhCounter;
      Grammarian = _meeting.Grammarian;
      Timer = _meeting.Timer;
      QuizMaster = _meeting.QuizMaster;
      Video = _meeting.Video;
      HotSeat = _meeting.HotSeat;
      WOTD = _meeting.WOTD;
      Theme = _meeting.Theme;
      DayOfMeeting = _meeting.DayOfMeeting;
      MeetingType = _meeting.MeetingType;
      Guests = _meeting.Guests;
      Attendees = _meeting.Attendees;
      ID = _meeting.ID.ToString();
      _bResolve = bResolve;

      _namesWithRole.Add(Toastmaster);
      _namesWithRole.Add(Speaker1);
      _namesWithRole.Add(Speaker2);
      _namesWithRole.Add(GeneralEvaluator);
      _namesWithRole.Add(Evaluator1);
      _namesWithRole.Add(Evaluator2);
      if (mtgToEdit.MeetingType == 1)
        _namesWithRole.Add(TableTopics);
      _namesWithRole.Add(HotSeat);
      _namesWithRole.Add(Grammarian);
      _namesWithRole.Add(AhCounter);
      _namesWithRole.Add(QuizMaster);
      _namesWithRole.Add(Timer);
      _namesWithRole.Add(Video);

    }

    public void Reset()
    {

    }

    //public List<string> ToList()
    //{
    //  List<string> list = new List<string>();
    //  list.Add(Toastmaster);
    //  list.Add(Speaker1);
    //  list.Add(Speaker2);
    //  list.Add(GeneralEvaluator);
    //  list.Add(Evaluator1);
    //  list.Add(Evaluator2);
    //  list.Add(TableTopics);
    //  list.Add(AhCounter);
    //  list.Add(Timer);
    //  list.Add(Grammarian);
    //  list.Add(QuizMaster);
    //  list.Add(Video);
    //  list.Add(HotSeat);
    //  return list;
    //}

    public void Sync()
    {
      //System.IO.FileStream fileStream = new FileStream("C:\\Users\\mike\\Documents\\TI\\Meetings.dat", FileMode.Append, FileAccess.Write);
      //StreamWriter strmWriter = new StreamWriter(fileStream);
      //strmWriter.Write(this.ToFile());
      _meeting.Toastmaster = Toastmaster;
      _meeting.Speaker1 = Speaker1;
      _meeting.Speaker2 = Speaker2;
      _meeting.GeneralEvaluator = GeneralEvaluator;
      _meeting.Evaluator1 = Evaluator1;
      _meeting.Evaluator2 = Evaluator2;
      (_meeting as MeetingModelRegular).TableTopics = TableTopics;
      (_meeting as MeetingModelRegular).TTContestants = TTContestants;
      (_meeting as MeetingModelRegular).TTWinner = TTWinner;
      _meeting.Grammarian = Grammarian;
      _meeting.AhCounter = AhCounter;
      _meeting.Timer = Timer;
      _meeting.QuizMaster = QuizMaster;
      _meeting.Video = Video;
      _meeting.HotSeat = HotSeat;
      _meeting.Attendees = Attendees;
      _meeting.Guests = Guests;
      _meeting.Theme = Theme;
      _meeting.WOTD = WOTD;
      _meeting.Resolved = Resolved;

    }

  }
}
