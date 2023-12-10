using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Security.Policy;

namespace UnderworldEditor
{
    public class Util
    {

        public enum ContentTypes
        {
            TileMap,
            TextureMap,
            AutoMap,
            AutoMapNotes,
            Conversation,
            ImageTR,
            ImageGR,
            Text,
            DataTable,
            AnimationOverlay,
            Unknown
        }

        /// <summary>
        /// UW block structure for .ark files.
        /// </summary>
        public struct UWBlock
        {
            public byte[] Data;
            public long Address; //The file address of the block

            public long DataLen;
            //UW2 specific
            public int CompressionFlag;
            public long ReservedSpace;

            public ContentTypes ContentType; 
        };


        /// <summary>
        /// Reads the file into the file buffer
        /// </summary>
        /// <returns><c>true</c>, if stream file was  read, <c>false</c> otherwise.</returns>
        /// <param name="Path">Path.</param>
        /// <param name="buffer">Buffer.</param>
        public static bool ReadStreamFile(String Path, out byte[] buffer)
        {
            if (!File.Exists(Path))
            {           
                
                Debug.WriteLine("Util.ReadStreamFile : File not found : " + Path);
                buffer = null;
                return false;
            }

            buffer=System.IO.File.ReadAllBytes(Path);
            return true;
            //Stream fs = File.Open(Path,FileMode.Open,FileAccess.ReadWrite);
            //if (fs.CanRead)
            //{
                
            //    buffer = new byte[fs.Length];
            //    for (int i = 0; i < fs.Length; i++)
            //    {
            //          buffer[i] = (byte)fs.ReadByte();
            //    }
            //    fs.Close();
            //    return true;
            //}
            //else
            //{
            //    fs.Close();
            //    buffer = new byte[0];
            //    return false;
            //}
        }
        
        public static void WriteStreamFile(String Path, byte[] buffer)
        {
            byte[] bytes = new byte[buffer.GetUpperBound(0)+1];
            for (int i=0; i<=buffer.GetUpperBound(0);i++)
            {
                bytes[i] = (byte)buffer[i];
            }
            File.WriteAllBytes(Path, bytes);
        }


        /// <summary>
        /// Gets the value at the specified address in the file buffer and performs any necessary -endian conversions
        /// </summary>
        /// <returns>The value at address.</returns>
        /// <param name="buffer">Buffer.</param>
        /// <param name="Address">Address.</param>
        /// <param name="size">Size of the data in bits</param>
        public static long getAt(byte[] buffer, long Address, int size)
        {//Gets contents of bytes the the specific integer address. int(8), int(16), int(32) per uw-formats.txt
            switch (size)
            {
                case 8:
                    { return buffer[Address]; }
                case 16:
                    { return ConvertInt16(buffer[Address], buffer[Address + 1]); }
                case 24:
                    { return ConvertInt24(buffer[Address], buffer[Address + 1], buffer[Address + 2]); }
                case 32:    //may be buggy!
                    { return ConvertInt32(buffer[Address], buffer[Address + 1], buffer[Address + 2], buffer[Address + 3]); }
                default:
                    {
                        return 0;
                    }
            }
        }


        public static long ConvertInt16(byte Byte1, byte Byte2)
        {
            int b1 = (int)Byte1;
            int b2 = (int)Byte2;
            return Byte2 << 8 | Byte1;
        }

        public static long ConvertInt24(byte Byte1, byte Byte2, byte Byte3)
        {
            return Byte3 << 16 | Byte2 << 8 | Byte1;
        }

        public static long ConvertInt32(byte Byte1, byte Byte2, byte Byte3, byte Byte4)
        {
            return Byte4 << 24 | Byte3 << 16 | Byte2 << 8 | Byte1;      //24 was 32
        }


        /// <summary>
        /// Extracts the specified number of bits from the position in the value.
        /// </summary>
        /// <returns>The bits.</returns>
        /// <param name="value">Value.</param>
        /// <param name="From">From.</param>
        /// <param name="Length">Length.</param>
        public static int ExtractBits(int value, int From, int Length)
        {
            int mask = getMask(Length);
            return (value >> From) & (mask);
        }


        /// <summary>
        /// Gets a bit mask of the specified length.
        /// </summary>
        /// <returns>The mask.</returns>
        /// <param name="Length">Length.</param>
        static int getMask(int Length)
        {
            int mask = 0;
            for (int i = 0; i < Length; i++)
            {
                mask = (mask << 1) | 1;
            }
            return mask;
        }


        /// <summary>
        /// Loads the UW block.
        /// </summary>
        /// <returns><c>true</c>, if UW block was loaded, <c>false</c> otherwise.</returns>
        /// <param name="arkData">Ark data.</param>
        /// <param name="blockNo">Block no.</param>
        /// <param name="targetDataLen">Target data length.</param>
        /// <param name="uwb">Uwb.</param>
        public static bool LoadUWBlock(byte[] arkData, int blockNo, long targetDataLen, out UWBlock uwb)
        {
            uwb = new UWBlock();
            int NoOfBlocks = (int)getAt(arkData, 0, 32);
            switch (main.curgame)
            {
                case main.GAME_UW2:
                    {//6 + block *4 + (noOfBlocks*type)
                        uwb.Address = (int)getAt(arkData, 6 + (blockNo * 4), 32);
                        uwb.CompressionFlag = (int)getAt(arkData, 6 + (blockNo * 4) + (NoOfBlocks * 4), 32);
                        uwb.DataLen = getAt(arkData, 6 + (blockNo * 4) + (NoOfBlocks * 8), 32);
                        uwb.ReservedSpace = getAt(arkData, 6 + (blockNo * 4) + (NoOfBlocks * 12), 32);
                        if (uwb.Address != 0)
                        {
                            if (((uwb.CompressionFlag >> 1) & 0x01) == 1)
                            {//data is compressed;
                                uwb.Data = unpackUW2(arkData, uwb.Address, ref uwb.DataLen);
                                return true;
                            }
                            else
                            {
                                uwb.Data = new byte[uwb.DataLen];
                                int b = 0;
                                for (long i = uwb.Address; i < uwb.Address + uwb.DataLen; i++)
                                {//Copy the data to the block.
                                    uwb.Data[b++] = arkData[i];
                                }
                                return true;
                            }
                        }
                        else
                        {
                            uwb.DataLen = 0;
                            return false;
                        }
                    }
                default:
                    {
                        uwb.Address = getAt(arkData, (blockNo * 4) + 2, 32);
                        if (uwb.Address != 0)
                        {
                            uwb.Data = new byte[targetDataLen];
                            uwb.DataLen = targetDataLen;
                            uwb.CompressionFlag = 0;
                            int b = 0;
                            for (long i = uwb.Address; i < uwb.Address + uwb.DataLen; i++)
                            {//Copy the data to the block.
                                uwb.Data[b++] = arkData[i];
                            }
                            return true;
                        }
                        else
                        {
                            uwb.DataLen = 0;
                            return false;
                        }
                    }
            }
        }



        /// <summary>
        /// Loads the UW block.
        /// </summary>
        /// <returns><c>true</c>, if UW block was loaded, <c>false</c> otherwise.</returns>
        /// <param name="arkData">Ark data.</param>
        /// <param name="blockNo">Block no.</param>
        /// <param name="targetDataLen">Target data length.</param>
        /// <param name="uwb">Uwb.</param>
        public static bool CopyUWBlock(byte[] arkData, int blockNo, long targetDataLen, out UWBlock uwb)
        {
            uwb = new UWBlock();
            int NoOfBlocks = (int)getAt(arkData, 0, 32);
            switch (main.curgame)
            {
                case main.GAME_UW2:
                    {//6 + block *4 + (noOfBlocks*type)
                        uwb.Address = (int)getAt(arkData, 6 + (blockNo * 4), 32);
                        uwb.CompressionFlag = (int)getAt(arkData, 6 + (blockNo * 4) + (NoOfBlocks * 4), 32);
                        uwb.DataLen = getAt(arkData, 6 + (blockNo * 4) + (NoOfBlocks * 8), 32);
                        uwb.ReservedSpace = getAt(arkData, 6 + (blockNo * 4) + (NoOfBlocks * 12), 32);
                        if (uwb.Address != 0)
                        {
                            uwb.Data = new byte[uwb.DataLen];
                            int b = 0;
                            for (long i = uwb.Address; i < uwb.Address + uwb.DataLen; i++)
                            {//Copy the data to the block.
                                uwb.Data[b++] = arkData[i];
                            }
                            return true;
                        }
                        else
                        {
                            uwb.Data = new byte[0];
                            uwb.DataLen = 0;
                            return false;
                        }
                    }                    
            }
            return false;
        }


        /// <summary>
        /// Unpacks the Uw2 compressed blocks
        /// </summary>
        /// <returns>The Uw2 uncompressed block.</returns>
        /// <param name="tmp">Tmp.</param>
        /// <param name="address_pointer">Address pointer.</param>
        /// <param name="datalen">Datalen.</param>
        /// Robbed and changed slightly from the Labyrinth of Worlds implementation project.
        ///This decompresses UW2 blocks.
        public static byte[] unpackUW2(byte[] tmp, long address_pointer, ref long datalen)
        {
            //string UnpackLog = "";
            long BlockLen = (int)getAt(tmp, address_pointer, 32);  //lword(base);
            long NoOfSegs = ((BlockLen / 0x1000) + 1) * 0x1000;
            //byte[] buf = new byte[BlockLen+100];
            byte[] buf = new byte[Math.Max(NoOfSegs, BlockLen + 100)];

            long upPtr = 0;
            datalen = 0;
            address_pointer += 4;

            while (upPtr < BlockLen)
            {
                byte bits = (byte)tmp[address_pointer++];
                for (int r = 0; r < 8; r++)
                {
                    if (address_pointer > tmp.GetUpperBound(0))
                    {//No more data!
                        //UnpackLog += "NoMoreSpace\n";
                        return buf;
                    }
                    if ((bits & 1) == 1)
                    {//Transfer
                       // UnpackLog += "Transferring " + tmp[address_pointer] + " to " + upPtr + "\n";
                        buf[upPtr++] = tmp[address_pointer++];
                        datalen = datalen + 1;
                    }
                    else
                    {//copy
                        int o = tmp[address_pointer++];
                        int c = tmp[address_pointer++];

                        //Convert the two bytes into the copy record header format.
                        //0000   Int8   0..7: position, bits 0..7
                        //0001   Int8   0..3: copy count
                        //4..7: position, bits 8..11

                        //The copy count is 4 bits long and an offset of 3 is added to it.The
                        //position has 12 bits(accessing the last 4k bytes) and an offset of 18 is
                        //added.
                     o |= ((c & 0xF0) << 4);
                        c = ((c & 15) + 3);
                        o = (o + 18);

                      //  UnpackLog += "Copy Count " + c + " at offset " + o + "\n";

                        if (o > upPtr)
                        {
                            o -= 0x1000;
                        }

                        while (o < (upPtr - 0x1000))
                        {
                            o += 0x1000;
                        }

                        
                        while (c-- > 0)
                        {
                            if (o < 0)
                            {
                                //int currentsegment = ((datalen/0x1000) + 1) * 0x1000;
                                //buf[upPtr++]= buf[buf.GetUpperBound(0) + o++];//This is probably very very wrong.
                                //buf[upPtr++]= buf[currentsegment + o++];//This is probably very very wrong.
                                //UnpackLog += "Copying " + 0 + " to " + upPtr + " (o<0)\n";
                                buf[upPtr++] = (byte)0;
                                o++;
                            }
                            else
                            {
                                //UnpackLog += "Copying " + buf[o] + " to " + upPtr + " (o = " + o + ")\n";
                                buf[upPtr++] = buf[o++];
                            }
                            datalen++;    // = datalen+1;
                        }
                    }
                    bits >>= 1;
                }
            }
            //System.IO.File.WriteAllText(main.basepath + "\\unpack.log", UnpackLog);
            return buf;
        }


        public static ContentTypes GetUWLevArkContentType(int blockno)
        {
            if (main.curgame==main.GAME_UW1)
            {
                if (blockno <= 8)
                {
                    return ContentTypes.TileMap;
                }
                if (blockno <= 17)
                {
                    return ContentTypes.AnimationOverlay;
                }
                if (blockno <= 28)
                {
                    return ContentTypes.TextureMap;
                }
                if (blockno <= 39)
                {
                    return ContentTypes.AutoMap;
                }
                if (blockno <= 48)
                {
                    return ContentTypes.AutoMapNotes;
                }
            }
            else
            {
                /*
                        0.. 79  level maps
      80..159  texture mappings
     160..239  automap infos
     240..319  map notes
                 */
                if (blockno <= 79)
                {
                    return ContentTypes.TileMap;
                }
                if (blockno <= 159)
                {
                    return ContentTypes.TextureMap;
                }
                if (blockno <= 239)
                {
                    return ContentTypes.AutoMap;
                }
                if (blockno <= 319)
                {
                    return ContentTypes.AutoMapNotes;
                }


            }
 
            return ContentTypes.Unknown;
        }

        public static int UWBlockSizes(int blockno)
        {
            if (blockno <= 8)
            {
                return 0x7c08;
            }
            if (blockno <= 17)
            {
                return 0x0180;
            }
            if (blockno <= 28)
            {
                return 0x7a;
            }
            if (blockno <= 39)
            {
                return 0x1000;
            }
            else
            {
                return 0;
            }
            // if (blockno <= 48)
            //{
            //     return 0;//TODO:This block is of variable size
            //}
        }


        /// <summary>
        /// Writes an int8 to a file
        /// </summary>
        /// <param name="writer">Writer.</param>
        /// <param name="val">Value.</param>
        public static long WriteInt8(BinaryWriter writer, long val)
        {
            byte valOut = (byte)(val & 0xff);
            writer.Write(valOut);

            return 1;
        }


        /// <summary>
        /// Writes an int16 to a file
        /// </summary>
        /// <param name="writer">Writer.</param>
        /// <param name="val">Value.</param>
        public static long WriteInt16(BinaryWriter writer, long val)
        {
            byte valOut = (byte)(val & 0xff);
            writer.Write(valOut);

            valOut = (byte)(val >> 8 & 0xff);
            writer.Write(valOut);

            return 2;
        }

        public static void StoreInt16(byte[] buffer, int address, long val)
        {
            byte valOut = (byte)(val & 0xff);
            buffer[address] = valOut;

            valOut = (byte)(val >> 8 & 0xff);
            buffer[address + 1] = valOut;
        }


        /// <summary>
        /// Writes an int32 to file
        /// </summary>
        /// <param name="writer">Writer.</param>
        /// <param name="val">Value.</param>
        public static long WriteInt32(BinaryWriter writer, long val)
        {
            byte valOut = (byte)(val & 0xff);
            writer.Write(valOut);

            valOut = (byte)(val >> 8 & 0xff);
            writer.Write(valOut);

            valOut = (byte)(val >> 16 & 0xff);
            writer.Write(valOut);

            valOut = (byte)(val >> 24 & 0xff);
            writer.Write(valOut);

            return 4;
        }

        public static void StoreInt32(byte[] buffer, int address, long val)
        {
            byte valOut = (byte)(val & 0xff);
            buffer[address] = valOut;

            valOut = (byte)(val >> 8 & 0xff);
            buffer[address+1] = valOut;

            valOut = (byte)(val >> 16 & 0xff);
            buffer[address+2] = valOut;

            valOut = (byte)(val >> 24 & 0xff);
            buffer[address+3] = valOut;
        }



        public static string DecimalToOct(string data)
        {
            if (data == "0")
            { return "00"; }
            string result = string.Empty;
            int num = int.Parse(data);
            while (num > 0)
            {
                int rem = num % 8;
                num /= 8;
                result = rem.ToString() + result;
            }
            if (result.Length == 1)
            {
                result = "0" + result;
            }
            return result;
        }


    }//end class
}
