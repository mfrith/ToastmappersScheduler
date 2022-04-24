using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toastmappers
{
  public class NewMemberViewModel : ViewModelBase
  {
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
    public int MemberID
    {
      get { return _id; }
      set { SetProperty(ref _id, value, () => MemberID); }
    }

    private bool _hasBeenOfficer;
    public bool HasBeenOfficer
    {
      get { return _hasBeenOfficer; }
      set { SetProperty(ref _hasBeenOfficer, value, () => HasBeenOfficer); }
    }
  }
}
