﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;

namespace UnderworldEditor
{
    public class GRLoader : ArtLoader
    {
        const int repeat_record_start = 0;
        const int repeat_record = 1;
        const int run_record = 2;

        private string AuxPalPath = "\\DATA\\ALLPALS.DAT";
        bool useOverrideAuxPalIndex = false;
        int OverrideAuxPalIndex = 0;

        public string FileName;
        private bool ImageFileDataLoaded;
        int NoOfImages;

        public GRLoader(string filename)
        {
            useOverrideAuxPalIndex = false;
            OverrideAuxPalIndex = 0;
            PaletteNo = 0;
            FileName = filename;
            LoadImageFile();
        }

        public bool LoadImageFile()
        {
            if (!Util.ReadStreamFile(FileName, out ImageFileData))
            {
                return false;
            }
            else
            {
                NoOfImages = (int)Util.getAt(ImageFileData, 1, 16);
                ImageCache = new BitmapUW[NoOfImages];
                ImageFileDataLoaded = true;
                return true;
            }
        }

        public override BitmapUW LoadImageAt(int index)
        {
            return LoadImageAt(index, true);
        }

        public override BitmapUW LoadImageAt(int index, bool Alpha)
        {
            if (ImageFileDataLoaded == false)
            {
                if (!LoadImageFile())
                {
                    return base.LoadImageAt(index);
                }
            }
            else
            {
                if (ImageCache[index] != null)
                {
                    return ImageCache[index];
                }
            }


            long imageOffset = Util.getAt(ImageFileData, (index * 4) + 3, 32);
            if (imageOffset >= ImageFileData.GetUpperBound(0))
            {//Image out of range
                return base.LoadImageAt(index);
            }


            switch (Util.getAt(ImageFileData, imageOffset, 8))//File type
            {
                case 0x4://8 bit uncompressed
                    {
                        int BitMapWidth = (int)Util.getAt(ImageFileData, imageOffset + 1, 8);
                        int BitMapHeight = (int)Util.getAt(ImageFileData, imageOffset + 2, 8);
                        imageOffset = imageOffset + 5;
                        ImageCache[index] = Image(this, ImageFileData, imageOffset, index, BitMapWidth, BitMapHeight, "name_goes_here", PaletteLoader.Palettes[PaletteNo], Alpha, BitmapUW.ImageTypes.EightBitUncompressed);
                        return ImageCache[index];
                    }
                case 0x8://4 bit run-length
                    {
                        byte[] imgNibbles;
                        int auxPalIndex;
                       
                        int BitMapWidth = (int)Util.getAt(ImageFileData, imageOffset + 1, 8);
                        int BitMapHeight = (int)Util.getAt(ImageFileData, imageOffset + 2, 8);
                        if (!useOverrideAuxPalIndex)
                        {
                            auxPalIndex = (int)Util.getAt(ImageFileData, imageOffset + 3, 8);
                        }
                        else
                        {
                            auxPalIndex = OverrideAuxPalIndex;
                        }
                        int datalen = (int)Util.getAt(ImageFileData, imageOffset + 4, 16);
                        imgNibbles = new byte[Math.Max(BitMapWidth * BitMapHeight * 2, (datalen + 5) * 2)];
                        imageOffset = imageOffset + 6;  //Start of raw data.
                        copyNibbles(ImageFileData, ref imgNibbles, datalen, imageOffset);
                         int[] aux = PaletteLoader.LoadAuxilaryPalIndices(main.basepath + AuxPalPath, auxPalIndex);
                        byte[] RawImg = DecodeRLEBitmap(imgNibbles, datalen, BitMapWidth, BitMapHeight, 4);
                        byte[] OutputImg = ApplyAuxPal(RawImg, aux);
                        ImageCache[index] = Image(this, OutputImg, 0, index, BitMapWidth, BitMapHeight, "name_goes_here", PaletteLoader.Palettes[PaletteNo], Alpha, BitmapUW.ImageTypes.FourBitRunLength);
                        ImageCache[index].UncompressedData = RawImg;
                        ImageCache[index].SetAuxPalRef(aux);
                        ImageCache[index].AuxPalNo = auxPalIndex;
                        return ImageCache[index];
                    }
                case 0xA://4 bit uncompressed
                    {
                        byte[] imgNibbles;
                        int auxPalIndex;
                        int datalen;
                        int BitMapWidth = (int)Util.getAt(ImageFileData, imageOffset + 1, 8);
                        int BitMapHeight = (int)Util.getAt(ImageFileData, imageOffset + 2, 8);
                        if (!useOverrideAuxPalIndex)
                        {
                            auxPalIndex = (int)Util.getAt(ImageFileData, imageOffset + 3, 8);
                        }
                        else
                        {
                            auxPalIndex = OverrideAuxPalIndex;
                        }
                        datalen = (int)Util.getAt(ImageFileData, imageOffset + 4, 16);
                        imgNibbles = new byte[Math.Max(BitMapWidth * BitMapHeight * 2, (5 + datalen) * 2)];
                        imageOffset = imageOffset + 6;  //Start of raw data.
                        copyNibbles(ImageFileData, ref imgNibbles, datalen, imageOffset);
                        //Palette auxpal = PaletteLoader.LoadAuxilaryPal(main.basepath + AuxPalPath, PaletteLoader.Palettes[PaletteNo], auxPalIndex);
                        int[] aux = PaletteLoader.LoadAuxilaryPalIndices(main.basepath + AuxPalPath, auxPalIndex);
                        byte[] OutputImg = ApplyAuxPal(imgNibbles, aux);
                        ImageCache[index] = Image(this, OutputImg, 0, index, BitMapWidth, BitMapHeight, "name_goes_here", PaletteLoader.Palettes[PaletteNo], Alpha, BitmapUW.ImageTypes.FourBitUncompress);
                        ImageCache[index].UncompressedData = OutputImg;
                        ImageCache[index].SetAuxPalRef(aux);
                        ImageCache[index].AuxPalNo = auxPalIndex;
                        ImageCache[index].FileOffset = imageOffset;
                        return ImageCache[index];
                    }
                //break;
                default:
                    {
                        int BitMapWidth = (int)Util.getAt(ImageFileData, imageOffset + 1, 8);
                        int BitMapHeight = (int)Util.getAt(ImageFileData, imageOffset + 2, 8);
                        if (FileName.ToUpper().EndsWith("PANELS.GR"))
                        {//Check to see if the file is panels.gr
                            if (index >= 4) { return base.LoadImageAt(0); } //new Bitmap(2, 2);
                            BitMapWidth = 83;  //getValAtAddress(textureFile, textureOffset + 1, 8);
                            BitMapHeight = 114; // getValAtAddress(textureFile, textureOffset + 2, 8);
                            if (main.curgame == main.GAME_UW2)
                            {
                                BitMapWidth = 79;
                                BitMapHeight = 112;
                            }
                            imageOffset = Util.getAt(ImageFileData, (index * 4) + 3, 32);
                            ImageCache[index] = Image(this, ImageFileData, imageOffset, index, BitMapWidth, BitMapHeight, "name_goes_here", PaletteLoader.Palettes[PaletteNo], Alpha, BitmapUW.ImageTypes.EightBitUncompressed);
                            return ImageCache[index];
                        }
                        break;
                    }
            }

            return base.LoadImageAt(0);
        }

        /// <summary>
        /// Applys an auxillary palette to raw image data.
        /// </summary>
        /// <param name="Img"></param>
        /// <param name="auxpal"></param>
        /// <returns></returns>
        byte[] ApplyAuxPal(byte[] Img, int[] auxpal)
        {
            byte[] output = new byte[Img.GetUpperBound(0) + 1];
            for (int i = 0; i <= Img.GetUpperBound(0); i++)
            {
                output[i] = (byte)auxpal[Img[i]];
            }
            return output;
        }

        /// <summary>
        /// Copies the nibbles.
        /// </summary>
        /// <param name="InputData">Input data.</param>
        /// <param name="OutputData">Output data.</param>
        /// <param name="NoOfNibbles">No of nibbles.</param>
        /// <param name="add_ptr">Add ptr.</param>
        /// This code from underworld adventures
        protected void copyNibbles(byte[] InputData, ref byte[] OutputData, int NoOfNibbles, long add_ptr)
        {
            //Split the data up into it's nibbles.
            int i = 0;
            NoOfNibbles = NoOfNibbles * 2;
            while (NoOfNibbles > 1)
            {
                if (add_ptr <= InputData.GetUpperBound(0))
                {
                    OutputData[i] = (byte)((Util.getAt(InputData, add_ptr, 8) >> 4) & 0x0F);        //High nibble
                    OutputData[i + 1] = (byte)((Util.getAt(InputData, add_ptr, 8)) & 0xf);  //Low nibble							
                }
                i = i + 2;
                add_ptr++;
                NoOfNibbles = NoOfNibbles - 2;
            }
            if (NoOfNibbles == 1)
            {   //Odd nibble out.
                OutputData[i] = (byte)((Util.getAt(InputData, add_ptr, 8) >> 4) & 0x0F);
            }
        }


        /// <summary>
        /// Decodes the RLE bitmap.
        /// </summary>
        /// <returns>The RLE bitmap.</returns>
        /// <param name="imageData">Image data.</param>
        /// <param name="datalen">Datalen.</param>
        /// <param name="imageWidth">Image width.</param>
        /// <param name="imageHeight">Image height.</param>
        /// <param name="BitSize">Bit size.</param>
        /// This code from underworld adventures
        byte[] DecodeRLEBitmap(byte[] imageData, int datalen, int imageWidth, int imageHeight, int BitSize)
        //, palette *auxpal, int index, int BitSize, char OutFileName[255])
        {
            byte[] outputImg = new byte[imageWidth * imageHeight];
            int state = 0;
            int curr_pxl = 0;
            int count = 0;
            int repeatcount = 0;
            byte nibble;

            int add_ptr = 0;

            while ((curr_pxl < imageWidth * imageHeight) || (add_ptr <= datalen))
            {
                switch (state)
                {
                    case repeat_record_start:
                        {
                            count = getcount(imageData, ref add_ptr, BitSize);
                            //main.instance.TxtDebug.Text += "\nStart:" + count;
                            if (count == 1)
                            {
                                state = run_record;
                            }
                            else if (count == 2)
                            {
                                repeatcount = getcount(imageData, ref add_ptr, BitSize) - 1;
                               // main.instance.TxtDebug.Text += "\n\tRepeat count Start: " + repeatcount;
                                state = repeat_record_start;
                            }
                            else
                            {
                                state = repeat_record;
                            }
                            break;
                        }
                    case repeat_record:
                        {
                            nibble = getNibble(imageData, ref add_ptr);
                            //for count times copy the palette data to the image at the output pointer
                            if (imageWidth * imageHeight - curr_pxl < count)
                            {
                                count = imageWidth * imageHeight - curr_pxl;
                            }
                            //main.instance.TxtDebug.Text += "\nRepeat:" + count;
                            for (int i = 0; i < count; i++)
                            {
                               // main.instance.TxtDebug.Text += "\n\tRepeat Pixel: " + (int)nibble;
                                outputImg[curr_pxl++] = nibble;
                            }
                            if (repeatcount == 0)
                            {
                                state = run_record;
                            }
                            else
                            {
                                state = repeat_record_start;
                                repeatcount--;
                            }
                            break;
                        }


                    case run_record: //runrecord
                        {
                            count = getcount(imageData, ref add_ptr, BitSize);
                            if (imageWidth * imageHeight - curr_pxl < count)
                            {
                                count = imageWidth * imageHeight - curr_pxl;
                            }
                            //main.instance.TxtDebug.Text += "\nRun:" + count;
                            for (int i = 0; i < count; i++)
                            {
                                //get nibble for the palette;
                                nibble = getNibble(imageData, ref add_ptr);
                                //main.instance.TxtDebug.Text += "\n\tCopy Pixel: " + (int)nibble;
                                outputImg[curr_pxl++] = nibble;
                            }
                            state = repeat_record_start;
                            break;
                        }
                }
            }
            return outputImg;
        }

        public byte[] EncodeRLEBitMap(byte[] img)
        {
            List<byte> data = new List<byte>();
            //main.instance.TxtDebug.Text += "\n\n\nEncoding";
           int i = 0;
            int state = repeat_record;
            int FoundNoOfRepeats = 0;
            if (getCopyCount(img,0)<3)
            {//image begins with a run record
                state = run_record;
                
            }
            while ( i<= img.GetUpperBound(0))
            {
                //read char.
                int copycount = getCopyCount(img, i);
                int curchar = (int)img[i];
               
                switch (copycount)
                {
                    case 1://
                    case 2://Char only appears 2 times. This is a run record until the next repeat.
                        if (state==run_record)
                        {//previous was already a run record. Insert count to signify this.
                           // main.instance.TxtDebug.Text += "\nRun Count 1";
                            data.Add((byte)1);//flag the data begins with a run record.
                        }
                        state = run_record;
                        int nextrepeat = FindNextRepeat(img, i);
                        if (nextrepeat>0)
                        {
                            copycount = nextrepeat-i;
                            CreateCount(data, copycount);
                            //main.instance.TxtDebug.Text += "\nRun " + copycount + " times\n";
                            for (int x=i; x< nextrepeat && x<=img.GetUpperBound(0);x++)
                            {
                                data.Add(img[x]);
                                //main.instance.TxtDebug.Text += "\n\t Running " + (int)img[x];
                            }
                        }
                        else
                        {//Run till end of file
                            copycount = img.GetUpperBound(0)+1 - i;
                            CreateCount(data, copycount);
                            //main.instance.TxtDebug.Text += "\nRun " + copycount + " times (til end)";
                            for (int x = i; x <= img.GetUpperBound(0); x++)
                            {
                                data.Add(img[x]);
                                //main.instance.TxtDebug.Text += "\n\t Running " + (int)img[x];
                            }
                        }
                        break;
                    default://Char appears 3 or more times. This is a repeat record
                        {
                            // if (state==repeat_record && i>0 )
                            //{//this is a repeating repeat record and not at the start of the data.
                            //data.Add((byte)2);//flag the data begins with a repeat record.
                            //main.instance.TxtDebug.Text += "\nRepeat Count (2)";
                            //}
                            // Find if the data following this repeat is also a repeating record. Do so until eof or not a repeat.
                            if (FoundNoOfRepeats < 1)
                            {
                                FoundNoOfRepeats = FindNoOfRepeats(img, i);
                                if (FoundNoOfRepeats > 1)
                                {
                                    data.Add((byte)2);//flag the data begins with a repeat record.
                                    //data.Add((byte)(FoundNoOfRepeats));
                                    CreateCount(data, FoundNoOfRepeats);
                                   // main.instance.TxtDebug.Text += "\nRepeat Count (2)";
                                }
                            }
                            FoundNoOfRepeats--;
                            state = repeat_record;

                            //main.instance.TxtDebug.Text += "\nRepeat " + curchar + " " + copycount + " times";
                            CreateCount(data, copycount);
                            data.Add((byte)curchar);
                           // for (int x = 0; x < copycount && x <= img.GetUpperBound(0); x++)
                           // {
                               // main.instance.TxtDebug.Text += "\n\t Repeating " + curchar;
                          //  }
                            break;
                        }
                }

                i = i + copycount; 
            }
            return DataToNibbles( data.ToArray() );
        }
        
        /// <summary>
        /// Turns a count into a valid list of 4 bit words
        /// </summary>
        /// <param name="data"></param>
        /// <param name="count"></param>
        void CreateCount(List<byte> data, int count)
        {
            //TODO.
           // 1 word count : [w0]     0-15                     , with(w0) != 0
      //2 word count :   0  [w1] [w2]  0-255              , with(w1 << 4 | w2) != 0
     // 3 word count :   0    0    0  [w3] [w4] [w5]    0-4095
            if (count < 16)
            {
                data.Add((byte)count);
            }
            else if(count<255)
            {
                data.Add((byte)0);
                data.Add((byte)((count & 0xf0) >>4));
                data.Add((byte)(count & 0x0f));
            }
            else //count <4095
            {
                data.Add((byte)0);
                data.Add((byte)0);
                data.Add((byte)0);
                data.Add((byte)((count & 0xf00) >> 8));
                data.Add((byte)((count & 0xf0) >> 4));
                data.Add((byte)(count & 0x0f));
            }
        }

        /// <summary>
        /// Gets how many instances of the specified character are in a row.
        /// </summary>
        /// <param name="val"></param>
        /// <param name="img"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        int getCopyCount(byte[] img, int start)
        {
            if (start > img.GetUpperBound(0)) { return 0; }
            int val = img[start];
            int copycount = 1;
            for (int i=start+1; i<=img.GetUpperBound(0);i++)
            {
                if (val != img[i])
                {
                    return copycount;
                }
                else
                {
                    copycount++;
                }
            }
            return copycount;
        }

        int FindNextRepeat(byte[] img, int start)
        {
            for (int i=start; i<=img.GetUpperBound(0);i++)
            {
                if (getCopyCount(img, i)>=3)
                {
                    return i;
                }
            }
            return -1;//no more repeats. Img is run record til eof.
        }

        int FindNoOfRepeats(byte[] img, int start)
        {
            int nextcopylength = getCopyCount(img, start);
            int count = 0;
            while(nextcopylength>=3)
            {
                start = start + nextcopylength;
                count++;
                nextcopylength = getCopyCount(img, start);
            }
            return count;
        }

        /// <summary>
        /// Getcount the specified nibbles, addr_ptr and size.
        /// </summary>
        /// <param name="nibbles">Nibbles.</param>
        /// <param name="addr_ptr">Address ptr.</param>
        /// <param name="size">Size.</param>
        /// This code from underworld adventures
        int getcount(byte[] nibbles, ref int addr_ptr, int size)
        {
            int n1;
            int n2;
            int n3;
            int count = 0;

            n1 = getNibble(nibbles, ref addr_ptr);
            count = n1;

            if (count == 0)
            {
                n1 = getNibble(nibbles, ref addr_ptr);
                n2 = getNibble(nibbles, ref addr_ptr);
                count = (n1 << size) | n2;
            }
            if (count == 0)
            {
                n1 = getNibble(nibbles, ref addr_ptr);
                n2 = getNibble(nibbles, ref addr_ptr);
                n3 = getNibble(nibbles, ref addr_ptr);
                count = (((n1 << size) | n2) << size) | n3;
            }
            return count;
        }

        /// <summary>
        /// Gets the nibble.
        /// </summary>
        /// <returns>The nibble.</returns>
        /// <param name="nibbles">Nibbles.</param>
        /// <param name="addr_ptr">Address ptr.</param>
        /// This code from underworld adventures
        byte getNibble(byte[] nibbles, ref int addr_ptr)
        {
            var n1 = nibbles[addr_ptr];
            addr_ptr = addr_ptr + 1;
            return n1;
        }


        /// <summary>
        /// Converts an eight bit stream of image data into a 4 bit one in nibbles.
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static byte[] DataToNibbles(byte[] data)
        {
            int datasize = data.GetUpperBound(0) + 1;
            int imgsize = datasize / 2 + datasize % 2;
            byte[] nibbles = new byte[imgsize];
            int counter = 0;
            for (int i=0; i<= data.GetUpperBound(0); i++ )
            {
                int curbyte = data[i];
                if (i % 2 ==1)
                {//odd nibble
                    nibbles[counter] = (byte)((nibbles[counter]) | (byte)(curbyte & 0xf));
                    counter++;
                }
                else
                {//even nibble
                    nibbles[counter] = (byte)((curbyte << 4) & 0xf0);
                }
            }           
            return nibbles;
        }

        public void Save4Bit()
        {
            return; // and this does not work either!!!
            //Copy unmodified files.
            byte[] output = new byte[128000];
            //Allocate space for file header
            int curFileOffset = 1 + 2 + (NoOfImages * 4);
            int FileOffsetPtr = 0;
            output[0] = ImageFileData[0];//type
            output[1] = ImageFileData[1];//no of images
            output[2] = ImageFileData[2];
            for (int i = 0; i < NoOfImages; i++)
            {
                if (ImageCache[i]==null)
                {
                    LoadImageAt(i);
                }
            }
            for (int i=0; i<NoOfImages;i++)
            {
                Util.StoreInt32(output, 3 + FileOffsetPtr, curFileOffset);
                FileOffsetPtr += 4;
                {//This is my new image. encode and store
                    byte[] newdata = EncodeRLEBitMap(ImageCache[i].UncompressedData);
                    output[curFileOffset++] = (byte)8;
                    output[curFileOffset++] = (byte)ImageCache[i].image.Width;
                    output[curFileOffset++] = (byte)ImageCache[i].image.Height;
                    output[curFileOffset++] = (byte)ImageCache[i].AuxPalNo;
                    Util.StoreInt16(output, curFileOffset, newdata.GetUpperBound(0) + 1);
                    curFileOffset += 2;
                    for (int j=0;j<=newdata.GetUpperBound(0);j++)
                    {
                        output[curFileOffset++] = newdata[j];
                    }
                }
            }

            byte[] final = new byte[curFileOffset + 1];
            for (int i=0; i<=final.GetUpperBound(0);i++)
            {
                final[i] = output[i];
            }
            Util.WriteStreamFile(FileName, final);
           
        }

        public void Convert()
        {
            //THIS DOES NOT WORK. Underworld will crash when file is converted. Exporter can open correctly!
            return;
            int[] FileSizes = new int[NoOfImages];
            //Get size of file data required.
            int FileSizeNeeded = 3 + (NoOfImages+1) * 4 ;
            //Converts the current 4 bit .gr file into an 8 bit uncompressed one. 
            //Load all images in files. This will give the uncompressed data in 8 bit format
            for (int i=0;i<=ImageCache.GetUpperBound(0);i++)
            {
                if (ImageCache[i]==null)
                {
                    LoadImageAt(i);
                }
                if (ImageCache[i]!=null)
                {
                    FileSizes[i] = (ImageCache[i].image.Height * ImageCache[i].image.Width) + 5;
                }
                else
                {
                    FileSizes[i] = 4 + 5;
                }
                
                FileSizeNeeded += FileSizes[i];
            }

            byte[] Output = new byte[FileSizeNeeded];
            //Write file header
            Output[0] = ImageFileData[0];//type
            Output[1] = ImageFileData[1];//no of images
            Output[2] = ImageFileData[2];
            int addptr = 3;
            int fileOffset = 3 + (NoOfImages+1) * 4;            
            for (int i=0; i<NoOfImages;i++)
            {
                Util.StoreInt32(Output, addptr, fileOffset);
                fileOffset += FileSizes[i];
                addptr += 4;
            }
            addptr = 3 + (NoOfImages + 1) * 4; 
            //Now write files  
            for (int i = 0; i < NoOfImages; i++)
            {
                Output[addptr++] = (byte)0x4;//Imagetype
                Output[addptr++] = (byte)ImageCache[i].image.Width;
                Output[addptr++] = (byte)ImageCache[i].image.Height;                
                Util.StoreInt16(Output, addptr, ImageCache[i].UncompressedData.GetUpperBound(0)+1);
                addptr += 2;
                //      Image Data
                for (int j=0;j<=ImageCache[i].UncompressedData.GetUpperBound(0);j++)
                {
                    Output[addptr++] = ImageCache[i].UncompressedData[j];
                }
            }

            //Save the file
            Util.WriteStreamFile(FileName, Output);          

        }

    }
}