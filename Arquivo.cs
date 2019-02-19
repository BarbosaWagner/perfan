using System;
using System.IO;
using System.Windows.Forms;

namespace Perfan
{
    public static class Arquivo
    {
        public static int GravarLog(string log, string diretorio)
        {
            try
            {
                // "M/dd/yyyy hh:mm"
                using (var sw = new StreamWriter(diretorio))
                {
                    sw.Write(log);
                    sw.Close();
                }
                return 0;
            }
            catch
            {
                return -1;
            }
            
        }
    }
}
