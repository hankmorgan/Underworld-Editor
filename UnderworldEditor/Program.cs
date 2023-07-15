using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UnderworldEditor
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //var repack = new Repacker();
            //int readptr = 0;
            //byte[] i = new byte[0x1000]; byte[] o=new byte[0x1000];
            //repack.DataCompression(ref i, ref o,ref readptr);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new main());
        }
    }
}
