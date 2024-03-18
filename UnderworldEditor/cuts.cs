﻿using System;
using System.Collections.Generic;

//Based on file research from https://github.com/jastadj/uwgodot2/tree/main/docs


namespace UnderworldEditor
{
    public class cuts
    {
        public List<CutSceneCommand> commands = new List<CutSceneCommand>();

        public static List<CutSceneCommand> LoadCutsceneData(byte[] data)
        {
            var output = new List<CutSceneCommand>();
            int addr_ptr = 0;

            while (addr_ptr < data.Length)
            {
                if(addr_ptr+4>data.Length)
                {
                    return output;
                }
                
                var cmd = new CutSceneCommand();
                cmd.offset = addr_ptr;
                cmd.frame = (int)Util.getAt(data, addr_ptr, 16);
                cmd.functionNo = (int)Util.getAt(data, addr_ptr + 2, 16);
                int argCount = CutSceneCommand.GetArgumentCount(cmd.functionNo);
                cmd.functionParams = new List<int>();
                addr_ptr += 4;
                for (int i = 0; i < argCount; i++)
                {
                    if(addr_ptr<data.Length)
                    {
                        cmd.functionParams.Add((int)Util.getAt(data, addr_ptr, 16));
                    }                    
                    addr_ptr += 2;
                }                   
                output.Add(cmd);
            }
            return output;
        }
    }

    public class CutSceneCommand
    {
        public int offset;
        public int frame;
        public int functionNo;
        public List<int> functionParams;

        static string[] FunctionNames = {
            "show-text",
            "set-flag",
            "no-op",
            "pause",
            "to-frame",
            "5????",
            "end-cutsc",
            "rep-seg",
            "open-file",
            "fade-out",
            "fade-in",
            "11????",
            "12????",
            "text-play",
            "Wait-Seconds",
            "klang",
            "16????",
            "17????",
            "no-op2",
            "19????",
            "20????",
            "21????",
            "22????",
            "23????",
            "24????",
            "Music",
            "no-op3",
            "27????"
            };

        static string[] FunctionDescriptions =
        {
            "displays text arg[1] with color arg[0]",
            "sets some flag to 0",
            "no-op, arguments are ignored",
            "pauses for arg[0] / 2 seconds",
            "plays up to frame arg[0]",
            "unknown, set frame for static cutscene?",
            "ends cutscene",
            "repeat segment arg[0] times",
            "opens a new file csXXX.nYY, with XXX from arg[0] and YY from arg[1]; XXX and YY are formatted octal values",
            "fades out at rate arg[0] (higher is faster)",
            "fades in at rate arg[0] (higher is faster)",
            "11 unknown",
            "12 unknown",
            "13 displays text arg[1] with color arg[0] and plays audio arg[2]",
            "14 Wait arg[1] seconds",
            "plays 'klang' sound",
            "16 unknown, not used in uw2",
            "17 unknown, not used in uw2",
            "does nothing, not used in uw2",
            "19 unknown",
            "20 unknown",
            "21 unknown",
            "22 unknown",
            "23 unknown",
            "24 unknown, usually at the start of cutscene",
            "Plays Music theme given by arg[0]",
            "does nothing, not used in uw2",
            "27 unknown"
        };
        public string FunctionName
        {
            get
            {
                if (functionNo > FunctionNames.GetUpperBound(0)) { return "OUTOFBOUNDS No " + functionNo + " "; }
                return FunctionNames[functionNo];
            }
        }
        public string FunctionDesc
        {
            get
            {
                if (functionNo > FunctionDescriptions.GetUpperBound(0)) { return "OUTOFBOUNDS"; }
                return FunctionDescriptions[functionNo];
            }
        }
        public string display
        {
            get
            {

                var funcname = FunctionName;
                if (functionParams == null)
                { return funcname + "()"; }
                var paramslist = String.Join(",", functionParams);
                return funcname +"(" + paramslist+ ")";
            }
        }
        public static int GetArgumentCount(int cmd)
        {
            switch (cmd)
            {
                case 0: return 2;//displays text arg[1] with color arg[0]
                case 1: return 0;//sets some flag to 0
                case 2: return 2;//no-op, arguments are ignored
                case 3: return 1;//pauses for arg[0] / 2 seconds
                case 4: return 2;//plays up to frame arg[0]
                case 5: //unknown, set frame for static cutscene? (0 params in UW2, 1 in UW1)
                    if (main.curgame == main.GAME_UW2)
                    {
                        return 0;
                    }
                    else
                    {
                        return 1;
                    }  
                case 6: return 0;//ends cutscene
                case 7: return 1;//repeat segment arg[0] times
                case 8: return 2;//opens a new file csXXX.nYY, with XXX from arg[0] and YY from arg[1]; XXX and YY are formatted octal values
                case 9: return 1;//fades out at rate arg[0] (higher is faster)
                case 10: return 1;//fades in at rate arg[0] (higher is faster)
                case 11: return 1;//unknown
                case 12: return 1;//unknown
                case 13: return 3;//displays text arg[1] with color arg[0] and plays audio arg[2]
                case 14: return 2;//wait arg[1] seconds
                case 15: return 0;//plays 'klang' sound (UW1 only)
                //case 16: return *;//unknown, not used in uw2 but looks like some sort of copy function.
                //case 17: return *;//unknown, not used in uw2
                case 18: return 4;//does nothing, not used in uw2
                case 19: return 3;//unknown
                case 20: return 3;//unknown
                case 21: return 2;//unknown
                case 22: return 3;//unknown
                case 23: return 3;//unknown
                case 24: return 1;//unknown, usually at the start of cutscene
                case 25: return 1;//music
                case 26: return 1;//does nothing, not used in uw2
                case 27: return 1;//unknown
            }

            return 0;


        }

    }
}
