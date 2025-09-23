
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
        var mtg = item as MeetingEditViewModel;
        if (mtg.MeetingType == 1)
          return element.FindResource("RegularMeetingTemplate") as DataTemplate;
        else if (mtg.MeetingType == 2)
          return element.FindResource("RegularMeetingTemplate2") as DataTemplate;
      }
      return base.SelectTemplate(item, container);
    }
  }
  public class MeetingsViewModel : PropertyChangedBase
  {
    private ObservableCollection<MeetingModelBase> _meetings;// = new ObservableCollection<MeetingModel>();
    private ObservableCollection<MemberViewModel> _members = new ObservableCollection<MemberViewModel>();
    private int _currentMeetingIndex = -1;

    List<string> regularTemplate = new List<string>(new string[] {"Toastmaster","Speaker 1","Speaker 2","General Evaluator",
                                                                  "Evaluator 1", "Evaluator 2", "Table Topics", "Ah Counter",
                                                                  "Timer", "Grammarian", "Quiz Master", "Video", "Hot Seat" });

    List<string> threeSpeakerTemplate = new List<string>(new string[] {"Toastmaster","Speaker 1","Speaker 2", "Speaker 3", "General Evaluator",
                                                                  "Evaluator 1", "Evaluator 2", "Evaluator 3", "Ah Counter",
                                                                  "Timer", "Grammarian", "Quiz Master", "Video", "Hot Seat" });

    List<string> speakathonTemplate = new List<string>(new string[] {"Toastmaster","Speaker 1","Speaker 2", "Speaker 3", "Speaker 4",
                                                                  "Speaker 5", "General Evaluator", "Evaluator 1", "Evaluator 2", "Evaluator 3",
                                                                  "Evaluator 4", "Evaluator 5", "Ah Counter",
                                                                  "Timer", "Grammarian", "Quiz Master", "Video", "Hot Seat" });

    //private static readonly KeyValuePair<string, string>[] meetingTemplates =
    //{
    //    new KeyValuePair<string, string>("regularTemplate","Regular Meeting"),
    //    new KeyValuePair<string, string>("threeSpeakerTemplate", "Three Speaker Meeting"),
    //    new KeyValuePair<string, string>("speakathonTemplate", "Speakathon")
    //};
    //public KeyValuePair<string, string>[] MeetingTemplates
    //{
    //  get
    //  {
    //    return meetingTemplates;
    //  }
    //}
    private DateTime _meetingDate = DateTime.Now;
    public DateTime MeetingDate
    {
      get { return _meetingDate; }
      set { _meetingDate = value; }
    }

    private bool _generateForMonth = true;
    public bool GenerateForMonth
    {
      get { return _generateForMonth; }
      set
      {
        _generateForMonth = value;
        NotifyPropertyChanged(() => GenerateForMonth);
        NotifyPropertyChanged(() => IsGenerateForMonth);
      }
    }

    private bool _generateForFriday = true;
    public bool GenerateForFriday
    {
      get { return _generateForFriday; }
      set
      {
        _generateForFriday = value;
        NotifyPropertyChanged(() => GenerateForFriday);
        //NotifyPropertyChanged(() => IsGenerateForMonth);
      }
    }
    //ItemsSource="{Binding Months}" SelectedIndex="0" SelectedItem="{Binding MonthToGenerate}"/>
    private string _monthToGenerateFor;// = "January";

    private List<string> _months = new List<string>(new string[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" });

    public string MonthToGenerateFor
    {
      get
      {
        if (string.IsNullOrEmpty(_monthToGenerateFor))
        {
          var dtNow = DateTime.Now;
          var monthNow = dtNow.Month;
          if (monthNow == 12) monthNow = 0;
          _monthToGenerateFor = _months[monthNow];
        }
        return _monthToGenerateFor;
      }
      set { _monthToGenerateFor = value; }
    }
    public List<string> Months
    {
      get { return _months; }
      set {; }
    }
    public bool IsGenerateForMonth
    {
      get { return !_generateForMonth; }
      set {; }
    }
    private List<string> _meetingTemplates = new List<string>(new string[] { "Regular Meeting", "Three Speaker Meeting", "Speakathon" });
    public List<string> MeetingTemplates
    {
      get
      {
        return _meetingTemplates;
      }
    }

    public List<string> CurrentMeetingTemplate
    {
      get { return regularTemplate; }
    }
    private string _meetingTemplate = "Regular Meeting";
    public string MeetingTemplate
    {
      get
      {
        return _meetingTemplate;
      }

      set
      {
        _meetingTemplate = value;
      }
    }

    //          <ComboBox DockPanel.Dock="Left" ItemsSource="{Binding MeetingTemplateEnumValues}" SelectedIndex="0" SelectedItem="{Binding MeetingTemplateEnum}"/>

    //public MeetingTypeEnum
    public MeetingsViewModel()
    {

    }

    //public bool CanExecute
    //{
    //  get { return this.canExecute; }
    //  set
    //  {
    //    if (this.canExecute == value)
    //    { return; }
    //    this.canExecute = value;
    //  }
    //}MM

    //private ICommand _generateMeeting;
    //public ICommand GenerateMeetingCmd
    //{
    //  get
    //  {
    //    if (_generateMeeting == null)
    //      _generateMeeting = new RelayCommand(GenerateMeeting);
    //    //_generateMeeting = new RelayCommand(GenerateMeeting, param => this.canExecute);
    //    return _generateMeeting;
    //  }
    //}

    //private bool canExecute = true;

    private ICommand _generateCommand;
    public ICommand GenerateMeetingCmd
    {
      get
      {
        return _generateCommand ?? (_generateCommand = new RelayCommand(() => GenerateMeeting(), () => CanExecuteGenerate));
      }
    }

    public bool CanExecuteGenerate
    {
      get
      {
        // check if executing is allowed, i.e., validate, check if a process is running, etc. 
        return true; ;
      }
    }

    private ICommand _resetCommand;
    public ICommand ResetCmd
    {
      get
      {
        return _resetCommand ?? (_resetCommand = new RelayCommand(() => ResetMeeting(), () => CanExecuteReset));
      }
    }

    public bool CanExecuteReset
    {
      get
      {
        // check if executing is allowed, i.e., validate, check if a process is running, etc. 
        return true; ;
      }
    }

    private ICommand _okCmd;
    public ICommand OKCmd
    {
      get
      {
        return _okCmd ?? (_okCmd = new RelayCommand(() => OK(), () => CanExecuteOK));
      }
    }

    public bool CanExecuteOK
    {
      get
      {
        // check if executing is allowed, i.e., validate, check if a process is running, etc. 
        return true; ;
      }
    }

    private ICommand _printCommand;
    public ICommand PrintMeetingsCmd
    {
      get
      {
        return _printCommand ?? (_printCommand = new RelayCommand(() => PrintMeetings(), () => true));
      }
    }

    public static Object GetListValue(Object obj, string name)
    {
      return obj.GetType().GetProperty(name).GetValue(obj, null);

    }

    List<string> regularTemplateOutput = new(new string[] {"DayOfMeeting","Toastmaster","Speaker1","Speaker2","GeneralEvaluator",
                                                                  "Evaluator1", "Evaluator2", "TableTopics", "AhCounter",
                                                                  "Timer", "Grammarian", "QuizMaster", "Video", "HotSeat" });

    public void PrintMeetings()
    {
      var one = _meetings[_meetings.Count - 1];
      var two = _meetings[_meetings.Count - 2];
      var three = _meetings[_meetings.Count - 3];
      var four = _meetings[_meetings.Count - 4];
      List<MeetingModelRegular> list = [];
      list.Add(four as MeetingModelRegular);
      list.Add(three as MeetingModelRegular);
      list.Add(two as MeetingModelRegular);
      list.Add(one as MeetingModelRegular);

      // show meetings in dialog for review;
      // need better file name to designate where temporally ->  "Aug 1 - Aug 31.csv" type of name?
      string fileName = _home + "\\Agendas\\MeetingsPerMonth" + "Aug" + "2025" + ".csv";
      if (File.Exists(fileName))
      {
        File.Delete(fileName);
      }

      using (StreamWriter file = new StreamWriter(fileName))
      {
        foreach (var role in regularTemplateOutput)
        {
          string row;
          if (list.Count() == 5)
            row = role + "," + GetListValue(list[0], role) + "," + GetListValue(list[1], role) + "," + GetListValue(list[2], role) + "," + GetListValue(list[3], role) + "," + GetListValue(list[4], role);
          else if (list.Count() == 4)
            row = role + "," + GetListValue(list[0], role) + "," + GetListValue(list[1], role) + "," + GetListValue(list[2], role) + "," + GetListValue(list[3], role);
          else
            row = role + "," + GetListValue(list[0], role) + "," + GetListValue(list[1], role) + "," + GetListValue(list[2], role);
          file.WriteLine(row);
        }

      }
      
    }

    public void OK()
    {
      //push values into meeting model and save
      //var t = _meetings.Count();
      //_newMeeting.Save(_listofmeetingids.Max() + 1);
      //var json =  JsonSerializer.ToString<MeetingModel>(_newMeeting);
      // update member info for last time being role
      Save();
    }

    public void ResetMeeting()
    {
      CurrentMeeting = null;
      _generateButtonEnabled = true;
      _roleListVisible = false;
      List<MemberViewModel> temp = _members.ToList();
      //_temporarymemberList = new List<MemberModel>(temp);
      NotifyPropertyChanged(() => RoleListVisible);
      NotifyPropertyChanged(() => GenerateButtonEnabled);
      NotifyPropertyChanged(() => ResetButtonEnabled);
    }

    private MeetingEditViewModel _currentMeeting;
    public MeetingEditViewModel CurrentMeeting
    {
      get { return _currentMeeting; }
      set { SetProperty(ref _currentMeeting, value, () => CurrentMeeting); }
    }

    private ICommand _nextMeetingCommand;
    public ICommand NextMeetingCmd
    {
      get
      {
        return _nextMeetingCommand ?? (_nextMeetingCommand = new RelayCommand(() => NextMeeting(), () => CanExecuteNextMeeting));
      }
    }

    public void NextMeeting()
    {
      _currentMeetingIndex++;
      //
      _currentMeeting = new MeetingEditViewModel(_meetings[_currentMeetingIndex], _members);
      NotifyPropertyChanged(() => CurrentMeeting);
    }

    public bool CanExecuteNextMeeting
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

    public bool CanExecutePreviousMeeting
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
    private ICommand _prevMeetingCommand;
    public ICommand PreviousMeetingCmd
    {
      get
      {
        return _prevMeetingCommand ?? (_prevMeetingCommand = new RelayCommand(() => PreviousMeeting(), () => CanExecutePreviousMeeting));
      }
    }

    public void PreviousMeeting()
    {
      _currentMeetingIndex--;
      // need to reset member list and remove already attended members from the list
      //LoadMembers();
      MeetingModelBase mtg = _meetings[_currentMeetingIndex];
      var r = mtg.Attendees;
      //foreach (var name in r)
      //  _members.Remove(_members.Single(iterator => iterator.Name == name));
      _currentMeeting = new MeetingEditViewModel(_meetings[_currentMeetingIndex], _members);
      NotifyPropertyChanged(() => CurrentMeeting);
    }

    private MeetingModelRegularVM _newMeeting;
    public void GenerateMeeting()
    {
      //_meetingDate
      var a = _meetingTemplate;
      var b = _meetingDate;

      // scenario 1 - generate a meeting for a specific day
      //var meeting1 = GenerateForDay();

      // this won't work the way I want it to.
      //MeetingModelBase c = new MeetingModelBase(MeetingTemplate, MeetingDate);
      // need to create class depending on template specified

      var notCurrentMembers = new ObservableCollection<MemberViewModel>(_members.Where(it => it.IsCurrent == false));
      // use the below for meeting generation
      var temporarymemberList = new ObservableCollection<MemberViewModel>(_members.Where(it => it.IsCurrent == true));
      List<string> membs = temporarymemberList.Select(it => it.Name).ToList();

      List<MeetingModelRegular> list = null;
      if (GenerateForMonth)
      {
        // generate one meeting at a time and show on pane
        // write them out when okayed, then show the next one
        _newMeeting = new MeetingModelRegularVM(temporarymemberList, _home);
        _newMeeting.Month = MonthToGenerateFor;
        var yearGen = (string month) =>
        {
          var now = DateTime.Now;
          var year = now.Year;
          if (month == "January")
            year++;

          return year.ToString();
        };

        _newMeeting.Year = yearGen(MonthToGenerateFor);
        var w = _meetings.Last();
        list = _newMeeting.GenerateForMonth(GenerateForFriday, _meetings[_meetings.Count() - 1].ID + 1, DateTime.ParseExact(_meetings.Last().DayOfMeeting, "MM-dd-yyyy", System.Globalization.CultureInfo.InvariantCulture));

        using (StreamWriter strmWriter = new StreamWriter(_home + "\\Data\\MembersStatus.json"))
        {
          // write out all objects(members)
          string member = string.Empty;
          //List<MemberModel> SortedList = _members.OrderBy(o => o.Name).ToList();
          _members.ToList().Sort((x, y) => x.Name.CompareTo(y.Name));
          foreach (var m in _members)
          {
            // need logic to set vm properties into the model before saving, and then need to have the vm not have a reference to the model in the first place
            SaveMemberInfo(m);
            member = m.Member.Serialize(m.Member);
            //member = m.Serialize(m);
            strmWriter.WriteLine(member);
          }
        }

        //IsoDateTimeConverter timeFormat = new IsoDateTimeConverter();
        //timeFormat.DateTimeFormat = "yyyy-MM-dd";
        //File.WriteAllText(_home + "\\Data\\MembersStatus.json", JsonConvert.SerializeObject(_members, timeFormat));

        foreach (var m in list)
        {
          _meetings.Add(m);
        }

        Save();

        //string fileName = _home + "\\Data\\Meetings" + MonthToGenerateFor + DateTime.Now.Year.ToString() + ".json";
        //if (!File.Exists(fileName))
        //{
        //  FileStream fs;
        //  fs = File.Create(fileName);
        //  fs.Close();
        //}
        //// following is for dev purposes only
        ////int vers = 1;
        ////string meetingFile = _home + "\\Data\\meetings.json";
        ////if (File.Exists (_home + ))
        //using (StreamWriter strmWriter = new StreamWriter(fileName))
        //{
        //  // write out all objects(members)
        //  strmWriter.AutoFlush = true;
        //  string meeting = string.Empty;
        //  foreach (var m in list)
        //  {
        //    meeting = (m as MeetingModelBase).Serialize(m);
        //    strmWriter.WriteLine(meeting);
        //  }
        //}
      }
      else
      {
        //_newMeeting = new MeetingModelRegularVM(MeetingDate.ToString("MM-dd-yyyy", CultureInfo.InvariantCulture), MeetingTemplate, temporarymemberList);
        //MeetingModelRegularVM newMeetingVM1 = new MeetingModelRegularVM(MeetingDate, MeetingTemplate, temporarymemberList);

        MeetingModelRegular newMeeting = new MeetingModelRegular(MeetingDate, MeetingTemplate, temporarymemberList);
        // if (MeetingTemplate == "Regular Meeting")
        //   newMeeting = new MeetingModelRegularVM();

        newMeeting.Generate();
        // generate should be on the model, not the vm
        //_newMeeting.Generate();
        //_newMeeting.ToTempMeeting();
        var newMeetingVM = new MeetingEditViewModel(newMeeting, temporarymemberList, false);
        _generateButtonEnabled = false;
        _roleListVisible = true;
        NotifyPropertyChanged(() => RoleListVisible);
        NotifyPropertyChanged(() => GenerateButtonEnabled);
        NotifyPropertyChanged(() => ResetButtonEnabled);
        NewMeetingView view = new()
        { DataContext = newMeetingVM };

        bool? bsuccess = view.ShowDialog();
        if (bsuccess == true)
        {
          newMeeting.ID = _meetings.Last().ID + 1;
          // add meeting to list
          //_meetings.Add(newMeeting);
          // probably need a check for meeting date and current date to make sure we're in the future.
          // Should do this in the editing vm not after we've closed the dialog
          //DateTime dateTime = DateTime.Now;
          //DateTime dayOfMeeting = DateTime.ParseExact(newMeetingVM.DayOfMeeting, "MM-dd-yyyy", System.Globalization.CultureInfo.InvariantCulture);

          // save roles out for members
          //var t = newMeetingVM.Toastmaster;

          Meetings.Add(newMeeting);
          //newMeeting.
          //Save();
          // update member status
          //newMeetingVM.Toastmaster;
          NotifyPropertyChanged(() => Meetings);
          _meetingCount = Meetings.Count();
          UpdateMemberStatus(newMeeting);
          //Save();
        }
      }
      NotifyPropertyChanged(() => Meetings);
      _generateButtonEnabled = true;
      NotifyPropertyChanged(() => GenerateButtonEnabled);
     
      return;
    }

    private void UpdateMemberStatus(MeetingModelBase meeting)
    {
      // this needs to be dependent on the template for the meeting

      DateTime dayOfMeeting = DateTime.ParseExact(meeting.DayOfMeeting, "MM-dd-yyyy", System.Globalization.CultureInfo.InvariantCulture);
      var type = meeting.MeetingType;
      MemberViewModel? member = null;

      var tm = meeting.Toastmaster;
      if (tm != null && !string.IsNullOrEmpty(tm))
      {
        member = _members.Where(it => it.Name == tm).FirstOrDefault();
        if (member != null)
          member.Toastmaster = dayOfMeeting;
      }

      var spkr1 = meeting.Speaker1;
      if (!string.IsNullOrEmpty(spkr1))
      {
        member = _members.Where(it => it.Name == spkr1).FirstOrDefault();
        if (member != null)
          member.Speaker = dayOfMeeting;
      }

      var spkr2 = meeting.Speaker2;
      if (!string.IsNullOrEmpty(spkr2))
      {
        member = _members.Where(it => it.Name == spkr2).FirstOrDefault();
        if (member != null)
          member.Speaker = dayOfMeeting;
      }

      var eval1 = meeting.Evaluator1;
      if (!string.IsNullOrEmpty(eval1))
      {
        member = _members.Where(it => it.Name == eval1).FirstOrDefault();
        if (member != null)
          member.Evaluator = dayOfMeeting;
      }

      var eval2 = meeting.Evaluator2;
      if (!string.IsNullOrEmpty(eval2))
      {
        member = _members.Where(it => it.Name == eval2).FirstOrDefault();
        if (member != null)
          member.Evaluator = dayOfMeeting;
      }

      var GE = meeting.GeneralEvaluator;
      if (!string.IsNullOrEmpty(GE))
      {
        member = _members.Where(it => it.Name == GE).FirstOrDefault();
        if (member != null)
          member.GeneralEvaluator = dayOfMeeting;
      }

      var TT = (meeting as MeetingModelRegular).TableTopics;
      if (!string.IsNullOrEmpty(TT))
      {
        member = _members.Where(it => it.Name == TT).FirstOrDefault();
        if (member != null)
          member.TT = dayOfMeeting;
      }

      var gram = meeting.Grammarian;
      if (!string.IsNullOrEmpty(gram))
      {
        member = _members.Where(it => it.Name == gram).FirstOrDefault();
        if (member != null)
          member.Gram = dayOfMeeting;
      }


      var timer = meeting.Timer;
      if (!string.IsNullOrEmpty(timer))
      {
        member = _members.Where(it => it.Name == timer).FirstOrDefault();
        if (member != null)
          member.Timer = dayOfMeeting;
      }

      var ah = meeting.AhCounter;
      if (!string.IsNullOrEmpty(ah))
      {
        member = _members.Where(it => it.Name == ah).FirstOrDefault();
        if (member != null)
          member.Ah = dayOfMeeting;
      }

      var quiz = meeting.QuizMaster;
      if (!string.IsNullOrEmpty(quiz))
      {
        member = _members.Where(it => it.Name == quiz).FirstOrDefault();
        if (member != null)
          member.Quiz = dayOfMeeting;
      }

      var video = meeting.Video;
      if (!string.IsNullOrEmpty(video))
      {
        member = _members.Where(it => it.Name == video).FirstOrDefault();
        if (member != null)
          member.Video = dayOfMeeting;
      }

      var hotseat = meeting.HotSeat;
      if (!string.IsNullOrEmpty(hotseat))
      {
        member = _members.Where(it => it.Name == hotseat).FirstOrDefault();
        if (member != null)
          member.HotSeat = dayOfMeeting;
      }

      //m = _members.First(it => it.Name == meeting.Speaker1);
      //if (m != null) { m.Speaker = dayOfMeeting; }
      //m = _members.First(it => it.Name == meeting.Speaker2);
      //if (m != null) { m.Speaker = dayOfMeeting; }
      //m = _members.First(it => it.Name == meeting.GeneralEvaluator);
      //if (m != null) { m.GeneralEvaluator = dayOfMeeting; }
      //m = _members.First(it => it.Name == meeting.Evaluator1);
      //if (m != null) { m.Evaluator = dayOfMeeting; }
      //m = _members.First(it => it.Name == meeting.Evaluator2);
      //if (m != null) { m.Evaluator = dayOfMeeting; }
      //m = _members.First(it => it.Name == meeting.Grammarian);
      //if (m != null) { m.Gram = dayOfMeeting; }
      //m = _members.First(it => it.Name == meeting.AhCounter);/
      //if (m != null) { m.Ah = dayOfMeeting; }
      //m = _members.First(it => it.Name == meeting.Timer);
      //if (m != null) { m.Timer = dayOfMeeting; }
      //m = _members.First(it => it.Name == meeting.HotSeat);
      //if (m != null) { m.HotSeat = dayOfMeeting; }
      //m = _members.First(it => it.Name == meeting.Video);
      //if (m != null) { m.Video = dayOfMeeting; }
      //m = _members.First(it => it.Name == (meeting as MeetingModelRegular).TableTopics);
      //if (m != null) { m.TT = dayOfMeeting; }
      //m = _members.First(it => it.Name == meeting.QuizMaster);
      //if (m != null) { m.Quiz = dayOfMeeting; }

    }
    private void SaveMemberInfo(MemberViewModel m)
    {
      m.Member.Name = m.Name;
      m.Member.MemberID = m.ID;
      m.Member.IsCurrentMember = m.IsCurrent;
      m.Member.CanBeEvaluator = m.CanBeEvaluator;
      m.Member.CanBeToastmaster = m.CanBeToastmaster;
      m.Member.HasBeenOfficer = m.HasBeenOfficer;
      m.Member.Mentors = m.Mentors;
      m.Member.Toastmaster = m.Toastmaster;
      m.Member.Speaker = m.Speaker;
      m.Member.Evaluator = m.Evaluator;
      m.Member.GeneralEvaluator = m.GeneralEvaluator;
      m.Member.Gram = m.Gram;
      m.Member.Ah = m.Ah;
      m.Member.Timer = m.Timer;
      m.Member.HotSeat = m.HotSeat;
      m.Member.Video = m.Video;
      m.Member.TT = m.TT;
      m.Member.Quiz = m.Quiz;
      m.Member.MeetingsOut = m.MeetingsOut;

    }
    public MeetingModelBase GenerateForDay()
    {
      MeetingModelRegular theMeeting = new MeetingModelRegular();
      //theMeeting.Generate(MeetingDate);
      return theMeeting as MeetingModelBase;
    }
    private string _home = string.Empty;
    public MeetingsViewModel(ObservableCollection<MemberViewModel> members, string location)
    {
      _members = members;
      _home = location;

    }

    private bool _showMeeting;
    public bool ShowMeeting
    {
      get { return _showMeeting; }
      set { _showMeeting = value; }
    }
    public ObservableCollection<MeetingModelBase> Meetings
    {
      get { return _meetings; }
    }

    private List<int> _listofmeetingids = new List<int>();
    private ObservableCollection<MeetingModelRegular> _meetingsRegular = null;
    public void Load()
    {
      Reload();
    }

    public void Reload()
    {
      //List<MeetingModel> theList = new List<MeetingModel>();
      //using (StreamReader strmReader = new StreamReader("C:\\Users\\mike\\Documents\\TI\\Meetings.dat"))//, FileMode.Open, FileAccess.Read))
      //{
      //  //StreamReader strmReader = new StreamReader(fileStream);
      //  string firstLine = strmReader.ReadLine();
      //  string line;
      //  char[] delims = new char[] { ',' };
      //  while ((line = strmReader.ReadLine()) != null)
      //  {
      //    string[] pole = line.Split(delims, StringSplitOptions.None);
      //    MeetingModel rcd = new MeetingModel(pole, ref _members);

      //    theList.Add(rcd);
      //    _listofmeetingids.Add(Int32.Parse(pole[0]));
      //  }
      //}

      List<MeetingModelBase> theList = new List<MeetingModelBase>();

      // next 3 lines for reading and writing
      // if no list create one?
      if (!Directory.Exists(_home + "\\Data"))
        Directory.CreateDirectory(_home + "\\Data");

      FileStream fs = null;
      if (!File.Exists(_home + "\\Data\\meetings.json"))
      {
        fs = File.Create(_home + "\\Data\\meetings.json");
        fs.Close();
      }

      //string json = File.ReadAllText(_home + "\\Data\\meetings.json");
      //var meetingList = JsonConvert.DeserializeObject<List<MeetingModelRegular>>(json);
      //if (meetingList == null)
      //  _meetings = new ObservableCollection<MeetingModelRegular>();
      //else
      //  _meetings = new ObservableCollection<MeetingModelRegular>(meetingList);


      string json = File.ReadAllText(_home + "\\Data\\meetings5.json");
      //string json2 = File.ReadAllText(_home + "\\Data\\meetingsTest.json");

      //var meetingList = JsonConvert.DeserializeObject<List<MeetingModelRegular>>(json);
      //_meetings = new ObservableCollection<MeetingModelBase>(meetingList);

      //using (StreamWriter writer = new StreamWriter(_home + "\\Data\\MeetingsTest.json"))
      //{
      //  foreach (MeetingModelRegular mtg in _meetings)
      //  {
      //    string mtgjson = mtg.Serialize();
      //    writer.WriteLine(mtgjson);
      //  }
      //}

      //IsoDateTimeConverter timeFormat = new IsoDateTimeConverter();
      //timeFormat.DateTimeFormat = "yyyy-MM-dd";

      // following is for dev purposes only
      //int vers = 1;
      //string meetingFile = _home + "\\Data\\meetings.json";
      //if (File.Exists (_home + ))
      //File.WriteAllText(_home + "\\Data\\meetings5.json", JsonConvert.SerializeObject(meetingList, timeFormat));

      //File.WriteAllText("myobjects.json", JsonConvert.SerializeObject(playerList));

      using (StreamReader strmReader = new StreamReader(_home + "\\Data\\meetingsTest.json"))
      {
        var t = new List<MeetingModelBase>();
        string meeting;
        while ((meeting = strmReader.ReadLine()) != null)
        {
          MeetingModelBase bah = new MeetingModelBase();
          var a = bah.Deserialize(meeting);
          if (a != null)
          {
            if (a.MeetingType == 1)
            {
              if ((a as MeetingModelRegular).TTContestants == null)
              {
                (a as MeetingModelRegular).TTContestants = new ObservableCollection<string>();
                (a as MeetingModelRegular).TTWinner = "";
              }
            }
            t.Add(a);
          }
          else
          {
            // what is a?
            var r = t.Count;
          }
        }

        //t.Reverse();
        _meetings = new ObservableCollection<MeetingModelBase>(t);

      }

      //File.WriteAllText("C:\\Users\\mike\\Documents\\TI\\Data\\Meetings5.json", JsonConvert.SerializeObject(_meetings));
      //using (StreamWriter strmWriter = new StreamWriter("C:\\Users\\mike\\Documents\\TI\\Meetings5.json", true))
      //{
      //  //theList.Sort((x, y) => DateTime.Compare(x.DayOfMeeting, y.DayOfMeeting));
      //  //theList.Reverse();
      //  _meetings = new ObservableCollection<MeetingModel>(theList);
      //  _meetingsRegular = new ObservableCollection<MeetingModelRegular>();
      //  foreach (var meeting in _meetings)
      //  {

      //    MeetingModelRegular mreg = new MeetingModelRegular();
      //    mreg.AhCounter = meeting.Ah?.Name;
      //    //mreg.Attendees = meeting.Attendees?.Select(it => it == ).ToList();
      //    mreg.DayOfMeeting = meeting.DayOfMeeting.ToString("MM-dd-yyyy", CultureInfo.InvariantCulture);
      //    mreg.Evaluator1 = meeting.Evaluator1?.Name;
      //    mreg.Evaluator2 = meeting.Evaluator2?.Name;
      //    mreg.GeneralEvaluator = meeting.GeneralEvaluator?.Name;
      //    mreg.Grammarian = meeting.Gram?.Name;
      //    mreg.HotSeat = meeting.HotSeat?.Name;
      //    mreg.MeetingType = meeting.MeetingType.ToString();
      //    mreg.QuizMaster = meeting.Quiz?.Name;
      //    mreg.Resolved = meeting.Resolved == true ? "1" : "0";
      //    mreg.Speaker1 = meeting.Speaker1?.Name;
      //    mreg.Speaker2 = meeting.Speaker2?.Name;
      //    mreg.TableTopics = meeting.TT?.Name;
      //    mreg.Timer = meeting.Timer?.Name;
      //    mreg.Toastmaster = meeting.Toastmaster?.Name;
      //    //mreg.TTContestants = meeting.TTContestants?.ToString();
      //    mreg.TTWinner = meeting.TTWinner?.Name;
      //    mreg.Video = meeting.Video?.Name;
      //    mreg.ID = meeting.ID.ToString();
      //    _meetingsRegular.Add(mreg);
      //    //var json = JsonSerializer.ToString<MeetingModelRegular>(mreg);
      //    var t = mreg.Serialize(mreg);
      //    strmWriter.WriteLine(t);
      //  }
      //  //WriteMeetingToFile("C:\\Users\\mike\\Documents\\TI\\Meetings4.txt", _meetingsRegular[50]);


      //  strmWriter.Close();

      //}

      // set CurrentMeeting to last resolved meeting, or meetings that are in the future.
      DateTime today = DateTime.Today.Date;
      var mtg = _meetings.Where(it => DateTime.ParseExact(it.DayOfMeeting, "MM-dd-yyyy", System.Globalization.CultureInfo.InvariantCulture).CompareTo(today) > 0).FirstOrDefault();
      if (mtg == null)
        mtg = _meetings.Last();
      else
        CurrentMeeting = new MeetingEditViewModel(mtg, _members);

      //var t = DateTime.Now;
      //var mtgDate = DateTime.Parse(mtg.DayOfMeeting);
      //var y = mtgDate.AddHours(13);
      //int rel = DateTime.Compare(y, t);
      //if (rel < 0)
      //  meetingsToResolve.Add(mtg);

      //CurrentMeeting = new MeetingEditViewModel(_meetings[mtg.ID], _members);
      _currentMeetingIndex = mtg.ID - 1;
      _meetingCount = _meetings.Count;
    }

    //private void WriteMeetingToFile(string path, MeetingModelRegular meeting)
    //{
    //  System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
    //  using (FileStream stream = new FileStream(path, FileMode.Append, FileAccess.Write))
    //  {
    //    formatter.Serialize(stream, meeting);
    //  }
    //}
    public void Save(bool fromResolve = false)
    {

      //_currentMeeting.Sync();
      //string json = File.ReadAllText(_home + "\\Data\\meetings5.json");
      if (File.Exists("C:\\Users\\mike\\Documents\\TI\\Meetings.json"))
      {
        //File.Delete("C:\\Users\\mike\\Documents\\TI\\MembersStatus.json");

      }
      using (StreamWriter strmWriter = new StreamWriter(_home + "\\Data\\meetingstest.json"))
      {
        // write out all objects(members)
        strmWriter.AutoFlush = true;
        string meeting = string.Empty;
        foreach (var m in _meetings)
        {
          meeting = (m as MeetingModelBase).Serialize(m);
          strmWriter.WriteLine(meeting);
        }
      }

      if (fromResolve)
        return;

      var date = DateTime.ParseExact(_currentMeeting.DayOfMeeting, "MM-dd-yyyy", System.Globalization.CultureInfo.InvariantCulture);

      // for each meeting role, check if that's newer than the current member status
      var tm = _currentMeeting.Toastmaster;
      MemberViewModel? member = null;
      DateTime newDate;
      bool bNewerDate;

      if (tm != null && !string.IsNullOrEmpty(tm))
      {
        // member = _members.Where(it => it.Name == spkr1).FirstOrDefault();
        member = _members.Where(it => it.Name == tm).FirstOrDefault();
        if (member != null)
        {

          //newDate = member.Toastmaster.AddMinutes(2);
          bNewerDate = date.CompareTo(member.Toastmaster.AddMinutes(2)) > 0;
          if (!string.IsNullOrEmpty(tm))
          {
            if (bNewerDate)
            {
              member.Toastmaster = date;
            }
          }
        }
      }
      var spkr1 = _currentMeeting.Speaker1;
      if (!string.IsNullOrEmpty(spkr1))
      {
        member = _members.Where(it => it.Name == spkr1).FirstOrDefault();
        if (member != null)
        {
          newDate = member.Speaker.AddMinutes(2);
          bNewerDate = date.CompareTo(member.Speaker.AddMinutes(2)) > 0;
          if (bNewerDate)
            member.Speaker = date;
        }
      }

      var spkr2 = _currentMeeting.Speaker2;
      if (!string.IsNullOrEmpty(spkr2))
      {
        member = _members.Where(it => it.Name == spkr2).FirstOrDefault();
        if (member != null)
        {
          newDate = member.Speaker.AddMinutes(2);
          bNewerDate = date.CompareTo(member.Speaker.AddMinutes(2)) > 0;
          if (bNewerDate)
          {
            member.Speaker = date;
          }
        }
      }

      var eval1 = _currentMeeting.Evaluator1;
      if (!string.IsNullOrEmpty(eval1))
      {
        member = _members.Where(it => it.Name == eval1).FirstOrDefault();
        if (member != null)
        {
          newDate = member.Evaluator.AddMinutes(2);
          bNewerDate = date.CompareTo(member.Evaluator.AddMinutes(2)) > 0;
          if (bNewerDate)
          {
            member.Evaluator = date;
          }
        }
      }


      var eval2 = _currentMeeting.Evaluator2;
      if (!string.IsNullOrEmpty(eval2))
      {
        member = _members.Where(it => it.Name == eval2).FirstOrDefault();
        if (member != null)
        {
          newDate = member.Evaluator.AddMinutes(2);
          bNewerDate = date.CompareTo(member.Evaluator.AddMinutes(2)) > 0;
          if (member != null && bNewerDate)
          {
            member.Evaluator = date;
          }
        }
      }

      var GE = _currentMeeting.GeneralEvaluator;
      if (!string.IsNullOrEmpty(GE))
      {
        member = _members.Where(it => it.Name == GE).FirstOrDefault();
        if (member != null)
        {
          newDate = member.GeneralEvaluator.AddMinutes(2);
          bNewerDate = date.CompareTo(member.GeneralEvaluator.AddMinutes(2)) > 0;
          if (member != null && bNewerDate)
          {
            member.GeneralEvaluator = date;
          }
        }
      }

      var TT = _currentMeeting.TableTopics;
      if (!string.IsNullOrEmpty(TT))
      {
        member = _members.Where(it => it.Name == TT).FirstOrDefault();
        if (member != null)
        {
          newDate = member.TT.AddMinutes(2);
          bNewerDate = date.CompareTo(member.TT.AddMinutes(2)) > 0;
          if (member != null && bNewerDate)
          {
            member.TT = date;
          }
        }
      }

      var gram = _currentMeeting.Grammarian;
      if (!string.IsNullOrEmpty(gram))
      {
        member = _members.Where(it => it.Name == gram).FirstOrDefault();
        if (member != null)
        {
          newDate = member.Gram.AddMinutes(2);
          bNewerDate = date.CompareTo(member.Gram.AddMinutes(2)) > 0;
          if (member != null && bNewerDate)
          {
            member.Gram = date;
          }
        }
      }


      var timer = _currentMeeting.Timer;
      if (!string.IsNullOrEmpty(timer))
      {
        member = _members.Where(it => it.Name == timer).FirstOrDefault();
        if (member != null)
        {
          newDate = member.Timer.AddMinutes(2);
          bNewerDate = date.CompareTo(member.Timer.AddMinutes(2)) > 0;
          if (member != null && bNewerDate)
          {
            member.Timer = date;
          }
        }
      }

      var ah = _currentMeeting.AhCounter;
      if (!string.IsNullOrEmpty(ah))
      {
        member = _members.Where(it => it.Name == ah).FirstOrDefault();
        if (member != null)
        {
          newDate = member.Ah.AddMinutes(2);
          bNewerDate = date.CompareTo(member.Ah.AddMinutes(2)) > 0;
          if (member != null && bNewerDate)
          {
            member.Ah = date;
          }
        }
      }

      var quiz = _currentMeeting.QuizMaster;
      if (!string.IsNullOrEmpty(quiz))
      {
        member = _members.Where(it => it.Name == quiz).FirstOrDefault();
        if (member != null)
        {
          newDate = member.Quiz.AddMinutes(2);
          bNewerDate = date.CompareTo(member.Quiz.AddMinutes(2)) > 0;
          if (member != null && bNewerDate)
          {
            member.Quiz = date;
          }
        }
      }

      var video = _currentMeeting.Video;
      if (!string.IsNullOrEmpty(video))
      {
        member = _members.Where(it => it.Name == video).FirstOrDefault();
        if (member != null)
        {
          newDate = member.Video.AddMinutes(2);
          bNewerDate = date.CompareTo(member.Video.AddMinutes(2)) > 0;
          if (member != null && bNewerDate)
          {
            member.Video = date;
          }
        }
      }

      var hotseat = _currentMeeting.HotSeat;
      if (!string.IsNullOrEmpty(hotseat))
      {
        member = _members.Where(it => it.Name == hotseat).FirstOrDefault();
        if (member != null)
        {
          newDate = member.HotSeat.AddMinutes(2);
          bNewerDate = date.CompareTo(member.HotSeat.AddMinutes(2)) > 0;
          if (member != null && bNewerDate)
          {
            member.HotSeat = date;
          }
        }
      }

      var t = (MainViewModel)Application.Current.MainWindow.DataContext;
      MembersViewModel members = (MembersViewModel)t.Tabs[1];
      members.SaveMembers();

    }

    private bool _generateButtonVisibility = false;
    public bool GenerateButtonVisibility
    {
      get { return _generateButtonVisibility; }
      set { SetProperty(ref _generateButtonVisibility, value, () => GenerateButtonVisibility); }
    }

    private bool _generateButtonEnabled = true;
    public bool GenerateButtonEnabled
    {
      get { return _generateButtonEnabled; }
      set { SetProperty(ref _generateButtonEnabled, value, () => GenerateButtonEnabled); }
    }

    public bool ResetButtonEnabled
    {
      get
      {
        bool enabled = !_generateButtonEnabled;
        return enabled;
      }
    }

    private bool _roleListVisible = false;
    private int _meetingCount;

    public bool RoleListVisible
    {
      get { return _roleListVisible; }
      set { SetProperty(ref _roleListVisible, value, () => RoleListVisible); }
    }

  }
}
