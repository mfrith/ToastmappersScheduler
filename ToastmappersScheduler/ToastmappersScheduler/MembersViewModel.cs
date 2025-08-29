using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Metrics;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Toastmappers;

namespace Toastmappers
{
  class MembersViewModel(string location, MainViewModel mainViewModel) : PropertyChangedBase
  {
    private MainViewModel _mainViewModel = mainViewModel;
    private string _home = location;

    // the only members list
    //private List<MemberViewModel> _members = new List<MemberViewModel>();
    private ObservableCollection<MemberViewModel> _pastMembers = new ObservableCollection<MemberViewModel>();

    private ObservableCollection<MemberViewModel> _members = new ObservableCollection<MemberViewModel>();
    public ObservableCollection<MemberViewModel> Members
    {
      get => _members;

      set {; }
    }

    
    public ObservableCollection<MemberViewModel> PastMembers
    { get => _pastMembers; }

    private ICommand _saveMembersCommand;
    public ICommand SaveMembersCmd
    {
      get { return _saveMembersCommand ?? (_saveMembersCommand = new RelayCommand(() => SaveMembers(), () => true)); }
    }

    public void SaveMembers()
    {
      Save();
    }

    public void Reload()
    {
      List<MemberModel> members = new List<MemberModel>();
      using (StreamReader strmReader = new StreamReader(_home + "\\Data\\MembersStatus.json"))
      {
        var t = new List<MemberModel>();
        string member;
        while ((member = strmReader.ReadLine()) != null)
        {
          MemberModel bah = new MemberModel();
          var a = bah.Deserialize(member);
          if (a != null)
            t.Add(a);
        }

        members = new List<MemberModel>(t);

      }

      List<MemberViewModel> mvm = new List<MemberViewModel>();
      foreach (var m in members)
      {
        mvm.Add(new MemberViewModel(m));
      }

      mvm.Sort();
      _members = new ObservableCollection<MemberViewModel>(mvm);

      NotifyPropertyChanged(() => Members);

      List<MemberModel> pastMembers = new List<MemberModel>();
      using (StreamReader strmReader = new StreamReader(_home + "\\Data\\PastMembers.json"))
      {
        var pt = new List<MemberModel>();
        string member;
        while ((member = strmReader.ReadLine()) != null)
        {
          MemberModel bah = new MemberModel();
          var a = bah.Deserialize(member);
          if (a != null)
            pt.Add(a);
        }

        pastMembers = new List<MemberModel>(pt);

      }

      List<MemberViewModel> pmvm = new List<MemberViewModel>();
      foreach (var m in pastMembers)
      {
        pmvm.Add(new MemberViewModel(m));
      }

      _pastMembers = new ObservableCollection<MemberViewModel>(pmvm);
      NotifyPropertyChanged(() => PastMembers);

    }

    //private ICommand _backupCommand;
    //public ICommand BackupCmd
    //{
    //  get { return _backupCommand ?? (_backupCommand = new RelayCommand(() => Backup(), () => true)); }
    //}

    //public void Backup()
    //{
    //  _mainViewModel.Backup();
    //}

    //public bool CanEditMember
    //{
    //  get { return string.IsNullOrEmpty(MemberToEdit) ? false : true; }
    //}


    //public MemberModel MemberToEdit
    //{
    //  get
    //  { return _currentMemberVM; }
    //  set
    //  {

    //            _currentMemberVM = value;
    //  }
    //}
    //private ICommand _editMemberCommand;
    //public ICommand EditMemberCmd
    //{
    //  get
    //  {
    //    return _editMemberCommand ?? (_editMemberCommand = new RelayCommand(() => EditMember(), () => CanEditMember));
    //  }
    //}

    //private MemberViewModel _currentMemberVM = null;
    //public void EditMember()
    //{
    //  // show properties we want to show
    //  string t = MemberToEdit;
    //  _currentMemberEdit = _members.Where(it => it.Name == t).First();
    //  NotifyPropertyChanged(() => MemberSet);

    //  _currentMemberVM = new MemberViewModel(_currentMemberEdit);

    //  NotifyPropertyChanged(() => CurrentMemberEdit);
    //}

    private readonly List<string> _meetingRoles = new List<string>(new string[] {"Toastmaster","Speaker","GeneralEvaluator",
                                                                  "Evaluator", "TT", "Ah",
                                                                  "Timer", "Gram", "Quiz", "Video", "HotSeat" });
    public List<string> Roles
    {
      get { return _meetingRoles; }
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

    private ICommand _deleteMentorCommand;
    public ICommand DeleteMentorCmd
    {
      get { return _deleteMentorCommand ??= new RelayCommand((p) => DeleteMentor(p), () => true); }
    }

    private void DeleteMentor(object item)
    {
      // need to create item or pass name to delete here
      // which list are we in?
      var tr = item.ToString();
      if (string.IsNullOrEmpty(tr))
        return;
      _mme.Mentors.Remove(tr);
    }
    private ICommand _setLastRoleCmd;
    public ICommand SetLastRoleCmd
    {
      get
      {
        return _setLastRoleCmd ?? (_setLastRoleCmd = new RelayCommand<object>((param) => SetLastRole(param), (param) => true));
      }
    }
    public void SetLastRole(object param)
    {
      var role = param.ToString();
      if (role == null)
        return;

      //role = "Speaker2";
      var name = _mme.Name;
      MeetingsViewModel meetingsVM = (MeetingsViewModel)_mainViewModel.Tabs[2];
      var meetings = meetingsVM.Meetings;
      List<MeetingModelBase> a;
      if (role == "Speaker")
      {
        var ones = meetings.Where(it => it != null && it.MeetingType == 1 && it.GetType() != null && it.GetType().GetProperty("Speaker1") != null &&
                           it.GetType().GetProperty("Speaker1").GetValue(it).ToString() == name).ToList();
        var twos = meetings.Where(it => it != null && it.MeetingType == 1 && it.GetType() != null && it.GetType().GetProperty("Speaker2") != null &&
                  it.GetType().GetProperty("Speaker2").GetValue(it).ToString() == name).ToList();

        a = ones.Concat(twos).ToList();
        //        List<MemberViewModel> SortedList = _members.OrderBy(o => o.Name).ToList();

      }
      else if (role == "Evaluator")
      {
        var ones = meetings.Where(it => it != null && it.MeetingType == 1 && it.GetType() != null && it.GetType().GetProperty("Evaluator1") != null &&
                            it.GetType().GetProperty("Evaluator1").GetValue(it).ToString() == name).ToList();
        var twos = meetings.Where(it => it != null && it.MeetingType == 1 && it.GetType() != null && it.GetType().GetProperty("Evaluator2") != null &&
                  it.GetType().GetProperty("Evaluator2").GetValue(it).ToString() == name).ToList();
        a = ones.Concat(twos).ToList();
      }
      else
      {
        //a = meetings.Where(it => it != null && it.MeetingType == 1 && it.GetType() != null && it.GetType().GetProperty(role) != null &&
        //                   it.GetType().GetProperty(role).GetValue(it).ToString() == name).ToList();
        a = meetings.Where(it => it != null && it.MeetingType == 1 && it.GetType() != null && it.GetType().GetProperty(role) != null &&
                   it.GetType().GetProperty(role).GetValue(it) != null && it.GetType().GetProperty(role).GetValue(it).ToString() == name).ToList();
      }

      MeetingModelBase mtg = null;
      DateTime date;
      if (a.Count < 1)
      {
        // assume the last date is 01/01/0001
        date = new DateTime(0001, 01, 01);
        //_mme.GetType().GetProperty(role).SetValue(_mme, date, null);
      }
      else
      {
        a = a.OrderBy(m => m.ID).ToList();
        int mtgCount = 2;
        mtg = a.OrderBy(it => it.ID).Last();
        DateTime now = DateTime.Now;
        date = DateTime.ParseExact(mtg.DayOfMeeting, "MM-dd-yyyy", System.Globalization.CultureInfo.InvariantCulture);
        while (date.CompareTo(now) > 0)
        {
          mtg = a[a.Count - mtgCount];
          date = DateTime.ParseExact(mtg.DayOfMeeting, "MM-dd-yyyy", System.Globalization.CultureInfo.InvariantCulture);
          mtgCount++;
        }
        //_mme.GetType().GetProperty(role).SetValue(_mme, date, null);
      }
      //var meeting = meetings.Where(it => it.Toastmaster == name).ToList();
      //var date = meetingstocheck.OrderBy();
      //MeetingRoleList.ItemsSource = c.Members.OrderBy(it => it.GetType()GetType().GetProperty(role).GetValue(it)).Select(x => x.Name).ToList();
      // = new meeting.DayOfMeeting);
      //_mme.Toastmaster = DateTime.ParseExact(mtg.DayOfMeeting, "MM-dd-yyyy", System.Globalization.CultureInfo.InvariantCulture);
      //_mme.Toastmaster = new DateTime(1963, 08, 14);
      //var a = param.GetType().GetProperty(name).GetValue(par, null);

      switch (role)
      {
        case "Toastmaster":
          _mme.Toastmaster = date;
          NotifyPropertyChanged(() => SetMemberRoleStatus.Toastmaster);
          break;

        case "Speaker":
          _mme.Speaker = date;
          NotifyPropertyChanged(() => SetMemberRoleStatus.Speaker);
          break;

        case "GeneralEvaluator":
          _mme.GeneralEvaluator = date;
          NotifyPropertyChanged(() => SetMemberRoleStatus.GeneralEvaluator);
          break;

        case "Evaluator":
          _mme.Evaluator = date;
          NotifyPropertyChanged(() => SetMemberRoleStatus.Evaluator);
          break;

        case "TableTopics":
          _mme.TT = date;
          NotifyPropertyChanged(() => SetMemberRoleStatus.TT);
          break;

        case "AhCounter":
          _mme.Ah = date;
          NotifyPropertyChanged(() => SetMemberRoleStatus.Ah);
          break;

        case "Timer":
          _mme.Timer = date;
          NotifyPropertyChanged(() => SetMemberRoleStatus.Timer);
          break;

        case "Grammarian":
          _mme.Gram = date;
          NotifyPropertyChanged(() => SetMemberRoleStatus.Gram);
          break;

        case "QuizMaster":
          _mme.Quiz = date;
          NotifyPropertyChanged(() => SetMemberRoleStatus.Quiz);
          break;

        case "Video":
          _mme.Video = date;
          NotifyPropertyChanged(() => SetMemberRoleStatus.Video);
          break;

        case "HotSeat":
          _mme.HotSeat = date;
          NotifyPropertyChanged(() => SetMemberRoleStatus.HotSeat);
          break;

        default:
          break;
      }
    }
    // public 
    //public MemberViewModel CurrentMemberEdit
    //{
    //  get { return _currentMemberVM; }
    //  //set { }
    //}
    //private MemberViewModel _currentMemberEdit;

    public bool MemberSet
    {
      get { return _mme == null ? false : true; }
    }
    //public List<MemberModel> MemberList
    //{
    //  get { return _memberList; }
    //}
    // move these to class view model loading?
    public void Load()
    {
      // needs to be in some initialization call
      //if (!Directory.Exists(_home + "\\Data"))
      //  Directory.CreateDirectory(_home + "\\Data");

      //if (!File.Exists(_home + "\\Data\\MembersStatus.json"))
      //{
      //  FileStream fs;
      //  fs = File.Create(_home + "\\Data\\MembersStatusjson");
      //  fs.Close();
      //}

      //if (!File.Exists(_home + "\\Data\\PastMembers.json"))
      //{
      //  FileStream fs;
      //  fs = File.Create(_home + "\\Data\\PastMembers");
      //  fs.Close();
      //}

      //List<MemberModel> members = new List<MemberModel>();
      //using (StreamReader strmReader = new StreamReader(_home + "\\Data\\MembersStatus.json"))
      //{
      //  var t = new List<MemberModel>();
      //  string member;
      //  while ((member = strmReader.ReadLine()) != null)
      //  {
      //    MemberModel bah = new MemberModel();
      //    var a = bah.Deserialize(member);
      //    if (a != null)
      //      t.Add(a);
      //  }

      //  members = new List<MemberModel>(t);

      //}

      //List<MemberViewModel> mvm = new List<MemberViewModel>();
      //foreach (var m in members)
      //{
      //  mvm.Add(new MemberViewModel(m));
      //}

      //mvm.Sort();
      //_members = new ObservableCollection<MemberViewModel>(mvm);

      //List<MemberModel> pastMembers = new List<MemberModel>();
      //using (StreamReader strmReader = new StreamReader(_home + "\\Data\\PastMembers.json"))
      //{
      //  var pt = new List<MemberModel>();
      //  string member;
      //  while ((member = strmReader.ReadLine()) != null)
      //  {
      //    MemberModel bah = new MemberModel();
      //    var a = bah.Deserialize(member);
      //    if (a != null)
      //      pt.Add(a);
      //  }

      //  pastMembers = new List<MemberModel>(pt);

      //}

      //List<MemberViewModel> pmvm = new List<MemberViewModel>();
      //foreach (var m in pastMembers)
      //{
      //  pmvm.Add(new MemberViewModel(m));
      //}

      //_pastMembers = new ObservableCollection<MemberViewModel>(pmvm);
      Reload();
    }
    private ICommand _newMemberCmd;
    public ICommand NewMemberCmd
    {
      get
      {
        return _newMemberCmd ?? (_newMemberCmd = new RelayCommand(() => NewMember(), () => true));
      }
    }

    public List<string> MemberNames
    {
      get => MembersNames();

    }
    public List<string> MembersNames()
    {
      List<string> names = [];
      foreach (var member in _members)
      {
        names.Add(member.Name);
      }
      return names;
    }

    public void NewMember()
    {

      NewMemberViewModel newMember = new NewMemberViewModel(MembersNames());
      newMember.IsCurrent = true;
      bool success = newMember.ShowDialog();
      if (!success)
        return;

      MemberModel n = new MemberModel();
      n.CanBeEvaluator = newMember.CanBeEvaluator;
      n.CanBeToastmaster = newMember.CanBeToastmaster;
      n.Name = newMember.Name;
      n.MemberID = newMember.MemberID;
      n.HasBeenOfficer = newMember.HasBeenOfficer;
      n.IsCurrentMember = newMember.IsCurrent;
      n.Mentors = newMember.Mentors;
      n.MeetingsOut = new List<System.DateTime>();

      MemberViewModel newvm = new MemberViewModel(n);
      _members.Add(newvm);
      Save();
      NotifyPropertyChanged(() => Members);

    }

    private ICommand _moveToPastMembersCmd;

    public ICommand MoveToPastMembersCmd
    {
      get
      {
        return _moveToPastMembersCmd ?? (_moveToPastMembersCmd = new RelayCommand(() => MoveToPastMembers(), () => true));
      }
    }

    public void MoveToPastMembers()
    {
      var a = SetMemberRoleStatus;
      Members.Remove(a);
      PastMembers.Add(a);
      Save();
      NotifyPropertyChanged(() => Members);
      NotifyPropertyChanged(() => PastMembers);
    }

    private void Save()
    {

      using (StreamWriter strmWriter = new StreamWriter(_home + "\\Data\\MembersStatus.json"))
      {
        // write out all objects(members)
        string member = string.Empty;
        List<MemberViewModel> SortedList = _members.OrderBy(o => o.Name).ToList();
        //_members.Clear();
        //_members = new ObservableCollection<MemberViewModel>(SortedList);

        //var sortList = new List<MemberViewModel>(_members);
        //sortList.Sort();
        //_members.ToList().Sort((x, y) => x.Name.CompareTo(y.Name));
        foreach (var m in _members)
        {
          // need logic to set vm properties into the model before saving, and then need to have the vm not have a reference to the model in the first place
          SaveMemberInfo(m);
          member = m.Member.Serialize(m.Member);
          //member = m.Serialize(m);
          strmWriter.WriteLine(member);
        }
      }

      using (StreamWriter strmWriter = new StreamWriter(_home + "\\Data\\PastMembers.json"))
      {
        // write out all objects(members)
        string member = string.Empty;
        //List<MemberModel> SortedList = _members.OrderBy(o => o.Name).ToList();
        List<MemberViewModel> pastmembers = new List<MemberViewModel>(_pastMembers);
        pastmembers.Sort();
        //_pastMembers.ToList().Sort((x, y) => x.Name.CompareTo(y.Name));
        //_pastMembers.Clear();
        //_pastMembers = new ObservableCollection<MemberViewModel>(pastmembers);

        foreach (var m in _pastMembers)
        {
          SaveMemberInfo(m);
          member = m.Member.Serialize(m.Member);
          //member = m.Serialize(m);
          strmWriter.WriteLine(member);
        }
      }

      //Reload();
      // update vms to latest list
      NotifyPropertyChanged(() => Members);
      NotifyPropertyChanged(() => PastMembers);


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
  }
}
