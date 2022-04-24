using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace Toastmappers
{
  class MembersViewModel : PropertyChangedBase
  {

    private string _home = string.Empty;
    public MembersViewModel(string location)
    {
      _home = location;
    }

    // the only members list
    //private List<MemberViewModel> _members = new List<MemberViewModel>();
    private ObservableCollection<MemberViewModel> _pastMembers = new ObservableCollection<MemberViewModel>();

    private ObservableCollection<MemberViewModel> _members = new ObservableCollection<MemberViewModel>();
    public ObservableCollection<MemberViewModel> Members
    {
      get
      { return _members; }

      set {; }
    }

    public ObservableCollection<MemberViewModel> PastMembers
    { get { return _pastMembers; } }

    private ICommand _saveMembersCommand;
    public ICommand SaveMembersCmd
    {
      get { return _saveMembersCommand ?? (_saveMembersCommand = new RelayCommand(() => SaveMembers(), () => true)); }
    }

    public void SaveMembers()
    {
      Save();
    }
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
      if (!Directory.Exists(_home + "\\Data"))
        Directory.CreateDirectory(_home + "\\Data");

      if (!File.Exists(_home + "\\Data\\MembersStatus.json"))
      {
        FileStream fs;
        fs = File.Create(_home + "\\Data\\MembersStatusjson");
        fs.Close();
      }

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

      if (!File.Exists(_home + "\\Data\\PastMembers.json"))
      {
        FileStream fs;
        fs = File.Create(_home + "\\Data\\PastMembers");
        fs.Close();
      }

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

    }
    private ICommand _newMemberCmd;
    public ICommand NewMemberCmd
    {
      get
      {
        return _newMemberCmd ?? (_newMemberCmd = new RelayCommand(() => NewMember(), () => true));
      }
    }

    public void NewMember()
    {
      NewMemberViewModel newMember = new NewMemberViewModel();
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
      n.MeetingsOut = new List<System.DateTime>();
      n.Mentors = new ObservableCollection<string>();

      MemberViewModel newvm = new MemberViewModel(n);
      _members.Add(newvm);
      //Save();

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

      using (StreamWriter strmWriter = new StreamWriter(_home + "\\Data\\PastMembers.json"))
      {
        // write out all objects(members)
        string member = string.Empty;
        //List<MemberModel> SortedList = _members.OrderBy(o => o.Name).ToList();
        List<MemberViewModel> pastmembers = new List<MemberViewModel>(_pastMembers);
        pastmembers.Sort();
        //_pastMembers.ToList().Sort((x, y) => x.Name.CompareTo(y.Name));
        _pastMembers.Clear();
        _pastMembers = new ObservableCollection<MemberViewModel>(pastmembers);

        foreach (var m in _pastMembers)
        {
          SaveMemberInfo(m);
          member = m.Member.Serialize(m.Member);
          //member = m.Serialize(m);
          strmWriter.WriteLine(member);
        }
      }

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
