using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

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

            // ExtractSpellTable("C:\\Games\\UW2IDA\\uw2\\uw2.exe", 0x66490, "C:\\Games\\UW2IDA\\uw2\\spells.txt");
           // ExtractSpellTable("C:\\Games\\UW1\\game\\UW\\uw.exe", 0x59EF0, "C:\\Games\\UW1\\game\\UW\\spells.txt");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new main());
        }

        static void ExtractSpellTable(string exePath, long tableOffset, string outFile)
        {
            if (Util.ReadStreamFile(exePath, out byte[] Buffer))
            {
                File.Delete(outFile);
                long addr = tableOffset;
                string output="Index,SpellClass,Runes,SpellSubClass,CalculatedEffect\n";
                for (int spell =0; spell<=0x45;spell++)
                {
                    var spellclass = (Util.getValAtAddress(Buffer, addr, 8) & 0xF8)>>3;
                    var runes = Util.getValAtAddress(Buffer, addr + 1, 16);
                    var spellsubclass = Util.getValAtAddress(Buffer, addr + 3, 8);
                    output = output + spell +"," +spellclass + "," + runes + "," + spellsubclass +"," + (spell+256)+ "\n";
                    addr += 4;
                }
                File.WriteAllText(outFile, output);
            }
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
