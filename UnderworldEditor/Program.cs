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
           // thatOneFunction();


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new main());
        }

        static void thatOneFunction()
        {
            int ax = 0x1f80;
            int dx = 0x786;
            int cx = 0;
            int bx = 0xA;
            int si = 0x29;
            int swap;

            //xchg ax,si
            swap = ax;
            ax = si;
            si = swap;
            
            //xchg ax, dx
            swap = ax;
            ax = dx;
            dx = swap;
            //test ax,ax
            if(ax>0)
            {
                //mul bx
                ax = dx * bx;
            }
            //jcxz 
            if(cx>0)
            {
                //cxch ax,cx
                swap = ax;
                ax = cx;
                cx = swap;
                //mul si
                ax = ax * si;
                //add ax,cx
                ax = ax + cx;
            }
            //xchg ax,si
            swap = ax;
            ax = si;
            si = swap;
            //mul bx
            ax = ax * bx;
            ax = ax & 0xffff;
            Console.WriteLine("ax=" + ax.ToString("x"));
            //add dx, si
            dx = dx + si;
            Console.WriteLine("dx=" + dx.ToString("x"));
        }
    }
}
