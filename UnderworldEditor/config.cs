using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnderworldEditor
{
    namespace Underworld
    {
        public class uwsettings
        {
            public string pathuw1 { get; set; }
            public string pathuw2 { get; set; }
            public string gametoload { get; set; }
            public int level { get; set; }
            public int lightlevel { get; set; }
            public string levarkfolder { get; set; }
            public string shader { get; set; }
            public static uwsettings instance;
            
        } //end class
    }//end namespace
}
