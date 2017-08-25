using System;
using System.Threading;
using System.Windows.Forms;

namespace VNJIngressos
{
  internal static class Program
  {
    [STAThread]
    private static void Main()
    {
      bool createdNew;
      Mutex mutex = new Mutex(true, "VouNoJogoAppIngressosMonitor", out createdNew);
      if (!createdNew)
      {
        int num = (int) MessageBox.Show("Outra Instancia deste aplicativo já está em execução.");
      }
      else
      {
        // comentários adicionado
        //
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run((Form) new frmPrincipal());
        GC.KeepAlive((object) mutex);
      }
    }
  }
}
