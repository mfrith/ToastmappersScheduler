using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Toastmappers
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  /// 

  public partial class App : Application
  {
    public static string[] mode = [];
    private void Application_Startup(object sender, StartupEventArgs e)
    {
      //MainWindow wnd = null;
      //string[] modes = e.Args;
      //if (modes.Length == 0)
      //  wnd = new MainWindow("");
      //else
      //  wnd = new MainWindow(e.Args[0]);


      //wnd.Show();
    }
  }
}
