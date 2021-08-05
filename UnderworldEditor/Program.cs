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
           // PathFinding.pathfind();
            //return;
           // string offsets="";
           //if( Util.ReadStreamFile("C:\\Games\\UWPSX\\avatar_", out byte[] Buffer))
           //     {
           //     for (int i =0; i< 231; i++)
           //         {
           //         var data = Util.getValAtAddress(Buffer, i*4, 32);
           //         offsets += i + "," + data +"\n";
           //         //Console.WriteLine(i + "=" + data.ToString("X")  + "len");

           //          }
           //     System.IO.File.WriteAllText("C:\\Games\\UWPSX\\avatar.txt", offsets);
           //     }



            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new main());
        }
    }
}
