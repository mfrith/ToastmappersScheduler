using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Toastmappers
{
  public class NewMemberViewModel(List<string> members) : ViewModelBase
  {
    private List<string> _members = members;

    public bool ShowDialog()
    {
      NewMemberView view = new NewMemberView();
      view.Owner = System.Windows.Application.Current.MainWindow;
      view.DataContext = this;
      bool ret = false;

      bool? success = view.ShowDialog();
      if (success == true)
      {
        ret = true;
      }

      return ret;
    }

    private string? _name;
    public string Name
    { 
      get => _name;
      set => SetProperty(ref _name, value, () => Name);
    }

    private bool _iscurrent;
    public bool IsCurrent
    { get => _iscurrent; set => SetProperty(ref _iscurrent, value, () => IsCurrent);

    }

    private bool _canBeToastmaster;
    public bool CanBeToastmaster
    { get => _canBeToastmaster; set => SetProperty(ref _canBeToastmaster, value, () => CanBeToastmaster);
    }

    private bool _canBeEvaluator;
    public bool CanBeEvaluator
    { get => _canBeEvaluator; set => SetProperty(ref _canBeEvaluator, value, () => CanBeEvaluator);
    }

    private int _id;
    public int MemberID
    { get => _id; set => SetProperty(ref _id, value, () => MemberID);
    }

    private bool _hasBeenOfficer;
    public bool HasBeenOfficer
    { get => _hasBeenOfficer; set => SetProperty(ref _hasBeenOfficer, value, () => HasBeenOfficer);
    }


    private ObservableCollection<string> _mentors = [];
    public ObservableCollection<string> Mentors
    {
      get => _mentors;
      set => SetProperty(ref _mentors, value, () => Mentors);

    }

    public List<string> Members
    { get => _members; set => SetProperty(ref _members, value, () => Members);

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
      Mentors.Remove(tr);
    }
  }
}
