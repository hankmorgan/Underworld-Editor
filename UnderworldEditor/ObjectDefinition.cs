using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnderworldEditor
{

    public class ByteFormat
    {
        public string DataName { get; set; }
        public short DataOffset { get; set; }
        public short DataSize { get; set; }
        public short BitMask
        {
            get 
            {
            if (DataSize==1) 
                {
                    return 1; 
                }
                else 
                { 
                    return (short)((1 << DataSize) - 1);
                }
            }
        }
        public string Description { get; set; }
    }

    public class ObjectDefinition
    {
        public static int NoOfStaticObjectValues = 0;
        public static int NoOfNPCObjectValues = 0;
        public static int NoOfMobileObjectValues = 0;

        //List for common object properties 8x2 byte
        public List<ObjectDefinitionProperties> StaticObjectDefinition { get; set; }

        //List for the NPC version of the mobile objects
        public List<ObjectDefinitionProperties> NPCObjectDefinition { get; set; }

        //List for the projectile object version of the mobile objects
        public List<ObjectDefinitionProperties> MobileObjectDefinition { get; set; }

        public bool SanityCheck()
        {
            if (StaticObjectDefinition.Count != 4)
            {
                return false;
            }
            foreach(var sdef in StaticObjectDefinition) 
            {                
                if (!sdef.SanityCheck(sdef.ByteSize * 8))
                {
                    return false;
                }
            }


            if (NPCObjectDefinition.Count != 15)
            {
                return false;
            }
            foreach (var sdef in NPCObjectDefinition)
            {
                if (!sdef.SanityCheck(sdef.ByteSize * 8))
                {
                    return false;
                }
            }

            if (MobileObjectDefinition.Count != 15)
            {
                return false;
            }
            foreach (var sdef in MobileObjectDefinition)
            {
                if (!sdef.SanityCheck(sdef.ByteSize * 8))
                {
                    return false;
                }
            }

            return true;
        }
    }

    public class ObjectDefinitionProperties
    {
        public int ByteOffset { get; set; }
        public int ByteSize { get; set; }
        public List<ByteFormat> ByteFormat { get; set; }

        public bool SanityCheck(int targetsize)
        {
            int definedlength = 0;
            int expectedOffset = 0;
            foreach (var byt in ByteFormat)
            {               
                definedlength += byt.DataSize;
                if (expectedOffset != byt.DataOffset)
                {
                    Console.WriteLine("Expected Offset incorrect in byte");
                    return false;
                }
                expectedOffset = byt.DataOffset + byt.DataSize ;
            }
            if (definedlength!=targetsize)
            {
                Console.WriteLine("No of bits defined does not match size of structure!");
                return false;
            }
            return true;
        }

    }
}
