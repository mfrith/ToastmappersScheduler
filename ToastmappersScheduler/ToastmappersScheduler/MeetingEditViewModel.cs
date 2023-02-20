using Toastmappers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Toastmappers
{
  public class MeetingEditViewModel : ViewModelBase
  {
    List<string> regularTemplate = new List<string>(new string[] {"DayOfMeeting","Toastmaster","Speaker 1","Speaker 2","General Evaluator",
                                                                  "Evaluator 1", "Evaluator 2", "Table Topics", "Ah Counter",
                                                                  "Timer", "Grammarian", "Quiz Master", "Video", "Hot Seat" });

    List<string> regularTemplateOutput = new List<string>(new string[] {"DayOfMeeting","Toastmaster","Speaker1","Speaker2","GeneralEvaluator",
                                                                  "Evaluator1", "Evaluator2", "TableTopics", "AhCounter",
                                                                  "Timer", "Grammarian", "QuizMaster", "Video", "HotSeat" });
    List<string> threeSpeakerTemplate = new List<string>(new string[] {"Toastmaster","Speaker 1","Speaker 2", "Speaker 3", "General Evaluator",
                                                                  "Evaluator 1", "Evaluator 2", "Evaluator 3", "Ah Counter",
                                                                  "Timer", "Grammarian", "Quiz Master", "Video", "Hot Seat" });

    List<string> speakathonTemplate = new List<string>(new string[] {"Toastmaster","Speaker 1","Speaker 2", "Speaker 3", "Speaker 4",
                                                                  "Speaker 5", "General Evaluator", "Evaluator 1", "Evaluator 2", "Evaluator 3",
                                                                  "Evaluator 4", "Evaluator 5", "Ah Counter",
                                                                  "Timer", "Grammarian", "Quiz Master", "Video", "Hot Seat" });
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

    public string DayOfMeeting
    {
      get { return _dayOfMeeting; } 
      set { _dayOfMeeting = value; }
    }
    public string Toastmaster
    {
      get { return _meeting.Toastmaster; }
      set { _meeting.Toastmaster = value; }
    }
    public string Speaker1 { get; set; }
    public string Speaker2 { get; set; }
    public string Speaker3 { get; set; }
    public string Speaker4 { get; set; }
    public string Speaker5 { get; set; }
    public string GeneralEvaluator { get; set; }
    public string Evaluator1 { get; set; }
    public string Evaluator2 { get; set; }
    public string Evaluator3 { get; set; }
    public string Evaluator4 { get; set; }
    public string Evaluator5 { get; set; }
    public string AhCounter { get; set; }
    public string Grammarian { get; set; }
    public string Timer { get; set; }
    public string QuizMaster { get; set; }
    public string Video { get; set; }
    public string HotSeat { get; set; }
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
    public string TableTopics { get; set; }

    public string Month { get; set; }
    public string Year { get; set; }
    public string WOTD { get; set; }
    public string Theme { get; set; }
    public string TTWinner { get; set; }
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
        ObservableCollection<string> newList = new ObservableCollection<string>(a);
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
        ObservableCollection<string> newList = new ObservableCollection<string>(a);
        newList.Add("");
        return newList;
      }
    }

    private ObservableCollection<MemberViewModel> _attendeemembers;
    public ObservableCollection<string> AttendeesMembersList
    {
      get
      {
        var a = _attendeemembers.Select(iterator => iterator.Name).ToList();
        ObservableCollection<string> newList = new ObservableCollection<string>(a);
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

    private MeetingModelBase _meeting;
    private bool _bResolve;
    public MeetingEditViewModel(MeetingModelBase mtgToEdit, ObservableCollection<MemberViewModel> members, bool bResolve = true)
    {
      _meeting = mtgToEdit;
      _members = members;
      _attendeemembers = new ObservableCollection<MemberViewModel>(members);
      _ttcontestantmembers = new ObservableCollection<MemberViewModel>(members);
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

    public void Save()
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
