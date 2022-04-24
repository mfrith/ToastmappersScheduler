using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Toastmappers
{
  class GeneralViewModel
  {
    private string _home = string.Empty;
    public GeneralViewModel(string location)
    {
      _home = location;
    }

    public void Button_Click(object sender, RoutedEventArgs e)
    {
      //NewMemberDialog dialog = new NewMemberDialog();
      //bool? result = dialog.ShowDialog();
      //int t = 0;
      //if (result == true)
      //  t = 1;
      //else
      //  t = 2;
    }
  }
}
