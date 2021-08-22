using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace UnderworldEditor
{
    class CritterArtLoader
    {
        public CritterInfo[] critter =new CritterInfo[64];
        public CritterArtLoader(int game, TreeNode critnode, Palette pal)
        {
            if (game == 1)
            {
                ReadUw1AssocFile( Path.Combine(main.basepath, "CRIT", "ASSOC.ANM"), pal,critnode);
            }
            if(game==2)
            {
                ReadUW2AssocFile(critnode);
            }
        }



        void ReadUW2AssocFile(TreeNode critnode)
        {
            //Load the assoc file
            long AssocAddressPtr = 0;
            if (
                            (Util.ReadStreamFile(Path.Combine(main.basepath, "CRIT", "AS.AN"), out byte[] assoc))
                            && (Util.ReadStreamFile(Path.Combine(main.basepath, "CRIT", "PG.MP"), out byte[] pgmp))
                            && (Util.ReadStreamFile(Path.Combine(main.basepath, "CRIT", "CR.AN"), out byte[] cran))
                    )
            {
                for (int ass = 0; ass <= 63; ass++)
                {
                    int FileID = (int)Util.getValAtAddress(assoc, AssocAddressPtr++, 8);
                    int auxPal = (int)Util.getValAtAddress(assoc, AssocAddressPtr++, 8);
                    if (FileID != 255)
                    {
                        var critname = objects.ObjectName(ass + 64, 2);
                        TreeNode assocNode = critnode.Nodes.Add(critname + "(" + ass + ") File " + FileID);
                        critter[ass] = new CritterInfo(ass, FileID,PaletteLoader.Palettes[0], auxPal, assoc, pgmp, cran, assocNode);
                    }
                }
            }
        }


        private void ReadUw1AssocFile(string assocpath, Palette pal, TreeNode critnode)
        {
            long AssocAddressPtr = 256;
            if (Util.ReadStreamFile(assocpath, out byte[] assoc))
            {
                for (int ass = 0; ass <= 63; ass++)//Ignore npc 63 which has no animations
                {
                    int FileID = (int)Util.getValAtAddress(assoc, AssocAddressPtr++, 8);
                    int auxPal = (int)Util.getValAtAddress(assoc, AssocAddressPtr++, 8);
                    var critname = objects.ObjectName(ass+64,1);
                    TreeNode assocNode = critnode.Nodes.Add(critname + "("+ass+") File "+ FileID);

                    critter[ass] = new CritterInfo(ass, FileID, pal, auxPal, assocNode);
                }
            }
        }
    }


    class CritterInfo:ArtLoader
    {
        readonly byte[] FilePage0;
        readonly byte[] FilePage1;

        public CritterAnimInfo AnimInfo;

        public CritterInfo(int CritterNo, int file_id, Palette paletteToUse, int AuxPalNo, TreeNode assocNode)
        {
            string critterIDO = Util.DecimalToOct(file_id.ToString());
            
            AnimInfo = new CritterAnimInfo();
            int spriteIndex = 0;
            for (int pass = 0; pass < 2; pass++)
            {
                //load in both page files.
                if (pass == 0)
                {//CR{CRITTER file ID in octal}PAGE.N{Page}
                    var toLoad = Path.Combine(main.basepath, "CRIT", "CR" + critterIDO + "PAGE.N0" + pass);
                    Util.ReadStreamFile(toLoad, out FilePage0);
                    spriteIndex = ReadPageFileUW1(CritterNo,FilePage0, file_id, pass, spriteIndex, AuxPalNo, assocNode);
                }
                else
                {
                    var toLoad = Path.Combine(main.basepath, "CRIT", "CR" + critterIDO + "PAGE.N0" + pass);
                    Util.ReadStreamFile(toLoad, out FilePage1);
                    ReadPageFileUW1(CritterNo,FilePage1, file_id, pass, spriteIndex, AuxPalNo, assocNode);
                }
            }
        }

        /// <summary>
        /// Criter animations for UW2
        /// </summary>
        /// <param name="file_id"></param>
        /// <param name="paletteToUse"></param>
        /// <param name="AuxPalNo"></param>
        /// <param name="assocData"></param>
        /// <param name="PGMP"></param>
        /// <param name="cran"></param>
        public CritterInfo(int CritterNo, int file_id, Palette paletteToUse, int AuxPalNo, byte[] assocData, byte[] PGMP, byte[] cran, TreeNode assocNode)
        {
            int ExtractPageNo = 0;
            string critterIDO = Util.DecimalToOct(file_id.ToString());
            AnimInfo = new CritterAnimInfo();
            int spriteIndex = 0;
            for (int i = 0; i < 8; i++)
            {
                if ((int)Util.getValAtAddress(PGMP, file_id * 8 + i, 8) != 255)//Checks if a critter exists at this index in the page file.
                {
                    string ExtractPageNoOctal = Util.DecimalToOct(ExtractPageNo.ToString());
                    string fileCrit = Path.Combine(main.basepath, "CRIT", "CR" + critterIDO + "." + ExtractPageNoOctal);  // BasePath + sep + "CRIT" + sep + "CR" + critterIDO + "." + ExtractPageNoO;
                    spriteIndex = ReadPageFileUW2(CritterNo, assocData, AuxPalNo, fileCrit, AnimInfo, spriteIndex, paletteToUse);
                    ExtractPageNo++;
                }
            }



            /*
      x*512 : start character x animation definition [C]
   Each chunk has 8 subchunks of 64 bytes. A subchunk [SC] describes the
   animation frames to take for a certain action. The actions are
      [C]+0000 : [SC] standing
      [C]+0040 : [SC] walking
      [C]+0080 : [SC] in combat
      [C]+00c0 : [SC] attack
      [C]+0100 : [SC] attack
      [C]+0140 : [SC] attack
      [C]+0180 : [SC] attack
      [C]+01c0 : [SC] dying
            */
            
            int cranAdd = (file_id * 512);
            int cranBase = cranAdd;
            int animationCounter = 0;
            for (int Animation = 0; Animation < 8; Animation++)//The the animation slot
            {
                bool NoAngle = false;// isAnimUnAngled(Animation);
                //int NoOfValid=0;

                for (int Angle = 0; Angle < 8; Angle++)//Each animation has every possible angle.
                {
                    if ((NoAngle == false) || (Angle == 4))
                    {
                        //int UW2animIndex = GetUW2Anim(Animation, Angle);
                        int animIndex = animationCounter;//  Animation + Angle;//GetUW2Anim(Animation, Angle);
                        //int animIndex = TranslateAnimToIndex(UW2animIndex);//Maybe remove this?
                        AnimInfo.animName[animIndex] = animIndex.ToString(); //PrintAnimName(UW2animIndex);
                        TreeNode AnimationSet = assocNode.Nodes.Add(animIndex.ToString());
                        int ValidEntries = (int)Util.getValAtAddress(cran, cranAdd + (Animation * 64) + (Angle * 8) + (7), 8);//Get how many valid frames are in the animation
                        for (int FrameNo = 0; FrameNo < 8; FrameNo++)
                        {
                            int currFrame = (int)Util.getValAtAddress(cran, cranAdd + (Animation * 64) + (Angle * 8) + (FrameNo), 8);
                            if (FrameNo < ValidEntries)
                            {
                                AnimInfo.animIndices[animIndex, FrameNo] = currFrame;
                                var ImageNode = AnimationSet.Nodes.Add("CRITTER:," + CritterNo + "," + (currFrame).ToString());
                                ImageNode.Tag = "CRITTER:," + CritterNo + "," + (currFrame).ToString();
                            }
                            else
                            {
                                AnimInfo.animIndices[animIndex, FrameNo] = -1;
                            }
                        }
                        animationCounter++;
                    }                    
                }
            }
        }


        /// <summary>
        /// Is the animation unangled.
        /// </summary>
        /// <returns><c>true</c>, if animation unangled was ised, <c>false</c> otherwise.</returns>
        /// <param name="animationNo">Animation no.</param>
        bool isAnimUnAngled(int animationNo)
        {
            switch (animationNo)
            {
                case 0x2:
                case 0x3:
                case 0x4:
                case 0x5:
                case 0x6:
                case 0x7:
                case 0xd:
                    return true;
                default:
                    return false;
            }
        }



        public static string PrintAnimName(int animNo)
        {
            switch (animNo)
            {
                case 0x0:
                    return "idle_combat";
                case 0x1:
                    return "attack_bash";
                case 0x2:
                    return "attack_slash";
                case 0x3:
                    return "attack_thrust";
                case 0x4:
                    return "attack_unk4";
                case 0x5:
                    return "attack_secondary";
                case 0x6:
                    return "attack_unk6";
                case 0x7:
                    return "walking_towards";
                case 0xc:
                    return "death";
                case 0xd:
                    return "begin_combat";
                case 0x20:
                    return "idle_rear";
                case 0x21:
                    return "idle_rear_right";
                case 0x22:
                    return "idle_right";
                case 0x23:
                    return "idle_front_right";
                case 0x24:
                    return "idle_front";
                case 0x25:
                    return "idle_front_left";
                case 0x26:
                    return "idle_left";
                case 0x27:
                    return "idle_rear_left";
                case 0x28:
                    return "unknown_anim_40";
                case 0x29:
                    return "unknown_anim_41";
                case 0x2a:
                    return "unknown_anim_42";
                case 0x2b:
                    return "unknown_anim_43";
                case 0x2c:
                    return "unknown_anim_44";
                case 0x2d:
                    return "unknown_anim_45";
                case 0x2e:
                    return "unknown_anim_46";
                case 0x2f:
                    return "unknown_anim_47";
                case 0x50:
                    return "unknown_anim_80";
                case 0x51:
                    return "unknown_anim_81";
                case 0x52:
                    return "unknown_anim_82";
                case 0x53:
                    return "unknown_anim_83";
                case 0x54:
                    return "unknown_anim_84";
                case 0x55:
                    return "unknown_anim_85";
                case 0x56:
                    return "unknown_anim_86";
                case 0x57:
                    return "unknown_anim_87";
                case 0x80:
                    return "walking_rear";
                case 0x81:
                    return "walking_rear_right";
                case 0x82:
                    return "walking_right";
                case 0x83:
                    return "walking_front_right";
                case 0x84:
                    return "walking_front";
                case 0x85:
                    return "walking_front_left";
                case 0x86:
                    return "walking_left";
                case 0x87:
                    return "walking_rear_left";
                default:
                    return "unknown_anim";
            }
        }
        private int ReadPageFileUW1(int CritterNo, byte[] PageFile, int XX, int YY, int spriteIndex, int AuxPalNo, TreeNode assocNode)
        {
            int addptr = 0;
            int slotbase = (int)Util.getValAtAddress(PageFile, addptr++, 8);
            int NoOfSlots = (int)Util.getValAtAddress(PageFile, addptr++, 8);
            int[] SlotIndices = new int[NoOfSlots];
            int spriteCounter = 0;
            int k = 0;
            string XXo = Util.DecimalToOct(XX.ToString());
            string YYo = Util.DecimalToOct(YY.ToString());
            for (int i = 0; i < NoOfSlots; i++)
            {
                int val = (int)Util.getValAtAddress(PageFile, addptr++, 8);
                if (val != 255)
                {
                    SlotIndices[k++] = i;
                }
            }
            int NoOfSegs = (int)Util.getValAtAddress(PageFile, addptr++, 8);
            for (int i = 0; i < NoOfSegs; i++)
            {
                //string[] AnimFiles = new string[8];
                string AnimName = PrintAnimName(slotbase + SlotIndices[i]);
                // string AnimName = slotbase + "_" + SlotIndices[i]; // PrintAnimName(slotbase + SlotIndices[i]);

                int index = slotbase + SlotIndices[i]; //TranslateAnimToIndex(slotbase + SlotIndices[i]);
                AnimInfo.animName[index] = AnimName;
                TreeNode AnimationSet = assocNode.Nodes.Add(AnimName);
                //
                int ValidCount = 0;
                for (int j = 0; j < 8; j++)
                {
                    int val = (int)Util.getValAtAddress(PageFile, addptr++, 8);
                    if (val != 255)
                    {                   //AnimFiles[j] = "CR" + XX.ToString("d2") + "PAGE_N" + YY.ToString("d2") + "_" + AuxPalNo + "_" + val;

                        AnimInfo.animSequence[index, j] = "CR" + XXo + "PAGE_N" + YYo + "_" + AuxPalNo + "_" + (val).ToString("d4");
                        AnimInfo.animIndices[index, j] = (val + spriteIndex);
                        var ImageNode = AnimationSet.Nodes.Add("CRITTER:," + CritterNo + "," + (val + spriteIndex).ToString());
                        ImageNode.Tag = "CRITTER:," + CritterNo + "," + (val + spriteIndex).ToString();
                        ValidCount++;
                    }
                    else
                    {
                        AnimInfo.animIndices[index, j] = -1;
                    }
                }
            }

            //Read in the palette
            int NoOfPals = (int)Util.getValAtAddress(PageFile, addptr, 8);//Will skip ahead this far.
            addptr++;
            byte[] auxPalVal = new byte[32];
            for (int i = 0; i < 32; i++)
            {
                auxPalVal[i] = (byte)Util.getValAtAddress(PageFile, (addptr) + (AuxPalNo * 32) + i, 8);
            }

            //Skip past the palettes
            addptr += NoOfPals * 32;
            int NoOfFrames = (int)Util.getValAtAddress(PageFile, addptr, 8);
            //AnimInfo.animSprites=new Sprite[NoOfFrames];
            addptr += 2;
            int addptr_start = addptr;//Bookmark my positiohn
            int MaxWidth = 0;
            int MaxHeight = 0;
            int MaxHotSpotX = 0;
            int MaxHotSpotY = 0;
            for (int pass = 0; pass <= 1; pass++)
            {
                addptr = addptr_start;
                if (pass == 0)
                {//get the max width and height
                    for (int i = 0; i < NoOfFrames; i++)
                    {
                        int frameOffset = (int)Util.getValAtAddress(PageFile, addptr + (i * 2), 16);
                        int BitMapWidth = (int)Util.getValAtAddress(PageFile, frameOffset + 0, 8);
                        int BitMapHeight = (int)Util.getValAtAddress(PageFile, frameOffset + 1, 8);
                        int hotspotx = (int)Util.getValAtAddress(PageFile, frameOffset + 2, 8);
                        int hotspoty = (int)Util.getValAtAddress(PageFile, frameOffset + 3, 8);
                        if (hotspotx > BitMapWidth)
                        {
                            hotspotx = BitMapWidth;
                        }
                        if (hotspoty > BitMapHeight)
                        {
                            hotspoty = BitMapHeight;
                        }

                        if (BitMapWidth > MaxWidth)
                        {
                            MaxWidth = BitMapWidth;
                        }
                        if (BitMapHeight > MaxHeight)
                        {
                            MaxHeight = BitMapHeight;
                        }

                        if (hotspotx > MaxHotSpotX)
                        {
                            MaxHotSpotX = hotspotx;
                        }
                        if (hotspoty > MaxHotSpotY)
                        {
                            MaxHotSpotY = hotspoty;
                        }
                    }
                }
                else
                {//Extract
                    if (MaxHotSpotX * 2 > MaxWidth)
                    {//Try and center the hot spot in the image.
                        MaxWidth = MaxHotSpotX * 2;
                    }
                    byte[] outputImg;
                    outputImg = new byte[MaxWidth * MaxHeight * 2];
                    for (int i = 0; i < NoOfFrames; i++)
                    {
                        int frameOffset = (int)Util.getValAtAddress(PageFile, addptr + (i * 2), 16);
                        int BitMapWidth = (int)Util.getValAtAddress(PageFile, frameOffset + 0, 8);
                        int BitMapHeight = (int)Util.getValAtAddress(PageFile, frameOffset + 1, 8);
                        int hotspotx = (int)Util.getValAtAddress(PageFile, frameOffset + 2, 8);
                        int hotspoty = (int)Util.getValAtAddress(PageFile, frameOffset + 3, 8);
                        int compression = (int)Util.getValAtAddress(PageFile, frameOffset + 4, 8);
                        int datalen = (int)Util.getValAtAddress(PageFile, frameOffset + 5, 16);

                        //Adjust the hotspots from the biggest point back to the image corners
                        int cornerX; int cornerY;
                        cornerX = MaxHotSpotX - hotspotx;
                        cornerY = MaxHotSpotY - hotspoty;
                        if (cornerX <= 0)
                        {
                            cornerX = 0;
                        }
                        else
                        {
                            cornerX--;
                        }
                        if (cornerY <= 0)
                        {
                            cornerY = 0;
                        }

                        //Extract the image
                        byte[] srcImg;
                        srcImg = new byte[BitMapWidth * BitMapHeight * 2];
                        outputImg = new byte[MaxWidth * MaxHeight * 2];
                        ArtLoader.ua_image_decode_rle(PageFile, srcImg, compression == 6 ? 5 : 4, datalen, BitMapWidth * BitMapHeight, frameOffset + 7, auxPalVal);


                        //*Put the sprite in the a frame of size max width & height
                        cornerY = MaxHeight - cornerY;//y is from the top left corner
                        int ColCounter = 0; int RowCounter = 0;
                        bool ImgStarted = false;
                        for (int y = 0; y < MaxHeight; y++)
                        {
                            for (int x = 0; x < MaxWidth; x++)
                            {
                                if ((cornerX + ColCounter == x) && (MaxHeight - cornerY + RowCounter == y) && (ColCounter < BitMapWidth) && (RowCounter < BitMapHeight))
                                {//the pixel from the source image is here 
                                    ImgStarted = true;
                                    outputImg[x + (y * MaxWidth)] = srcImg[ColCounter + (RowCounter * BitMapWidth)];
                                    ColCounter++;
                                }
                                else
                                {
                                    outputImg[x + (y * MaxWidth)] = 0;//alpha
                                }
                            }
                            if (ImgStarted == true)
                            {//New Row on the src image
                                RowCounter++;
                                ColCounter = 0;
                            }
                        }
                        //Set the heights for output
                        BitMapWidth = MaxWidth;
                        BitMapHeight = MaxHeight;

                        //****************************

                        // BitmapUW imgData = ArtLoader.Image(outputImg, 0, BitMapWidth, BitMapHeight, "namehere", pal, true, true);

                        BitmapUW imgData = ArtLoader.Image(this, outputImg, 0, 0, BitMapWidth, BitMapHeight, "name_goes_here", PaletteLoader.Palettes[0], true, BitmapUW.ImageTypes.EightBitUncompressed);
                        //CropImageData(ref imgData, pal);
                        //ImageCache[spriteIndex + 1] = imgData;
                        AnimInfo.animSprites[spriteIndex + i] = imgData;
                        spriteCounter++;
                    }
                }//endextract
            }
            return spriteCounter;
        }
        /// <summary>
        /// Read critter animation page files for UW2
        /// </summary>
        /// <param name="assocFile"></param>
        /// <param name="AuxPalNo"></param>
        /// <param name="fileCrit"></param>
        /// <param name="critanim"></param>
        /// <param name="spriteIndex"></param>
        /// <param name="paletteToUse"></param>
        /// <returns></returns>
        int ReadPageFileUW2(int CritterNo, byte[] assocFile, int AuxPalNo, string fileCrit, CritterAnimInfo critanim, int spriteIndex, Palette paletteToUse)
        {
            //Debug.Log(fileCrit + " starting at  "  + spriteIndex);
            Palette pal = paletteToUse;
            //byte[] auxpalval=new byte[32];
            //Palette[] auxpal = new Palette[32];
            //int auxPalNo = PaletteNo;
            int AddressPointer;


            //pal = new palette[256];
            //getPalette(PaletteFile, pal, 0);//always palette 0?

            Util.ReadStreamFile(fileCrit, out byte[] critterFile);


            //UW2 uses a different method
            //Starting at offset 0x80
            //fprintf(LOGFILE, "\n\t//%s - palette = %d", fileCrit, auxPalNo);
            //auxPalNo=2;
            AddressPointer = 0;//auxPalNo * 32;

            byte[] auxPalVal = new byte[32];
            for (int j = 0; j < 32; j++)
            {
                auxPalVal[j] = (byte)Util.getValAtAddress(critterFile, (AddressPointer) + (AuxPalNo * 32) + j, 8);
            }

            //int i = 0;
            int MaxWidth = 0;
            int MaxHeight = 0;
            int MaxHotSpotX = 0;
            int MaxHotSpotY = 0;

            for (int pass = 0; pass <= 1; pass++)
            {
                if (pass == 0)
                {//First pass is getting max image sizes
                    for (int index = 128; index < 640; index += 2)
                    {
                        int frameOffset = (int)Util.getValAtAddress(critterFile, index, 16);
                        if (frameOffset != 0)
                        {
                            int BitMapWidth = (int)Util.getValAtAddress(critterFile, frameOffset + 0, 8);
                            int BitMapHeight = (int)Util.getValAtAddress(critterFile, frameOffset + 1, 8);
                            int hotspotx = (int)Util.getValAtAddress(critterFile, frameOffset + 2, 8);
                            int hotspoty = (int)Util.getValAtAddress(critterFile, frameOffset + 3, 8);
                            if (hotspotx > BitMapWidth) { hotspotx = BitMapWidth; }
                            if (hotspoty > BitMapHeight) { hotspoty = BitMapHeight; }
                            if (BitMapWidth > MaxWidth) { MaxWidth = BitMapWidth; }
                            if (BitMapHeight > MaxHeight) { MaxHeight = BitMapHeight; }
                            if (hotspotx > MaxHotSpotX) { MaxHotSpotX = hotspotx; }
                            if (hotspoty > MaxHotSpotY) { MaxHotSpotY = hotspoty; }
                        }//End frameoffsetr first pass
                    }//End for loop first pass

                    switch (fileCrit.Substring(fileCrit.Length - 7, 4).ToUpper())
                    {
                        case "CR02"://Rat. max height is calculated incorrectly
                            MaxHeight = 100;
                            break;
                    }
                }//End first pass
                else
                {//Extract images
                    if (MaxHotSpotX * 2 > MaxWidth)
                    {//Try and center the hot spot in the image.
                        MaxWidth = MaxHotSpotX * 2;
                    }
                    byte[] outputImg;
                    outputImg = new byte[MaxWidth * MaxHeight * 2];
                    for (int index = 128; index < 640; index += 2)
                    {
                        int frameOffset = (int)Util.getValAtAddress(critterFile, index, 16);
                        if (frameOffset != 0)
                        {
                            int BitMapWidth = (int)Util.getValAtAddress(critterFile, frameOffset + 0, 8);
                            int BitMapHeight = (int)Util.getValAtAddress(critterFile, frameOffset + 1, 8);
                            int hotspotx = (int)Util.getValAtAddress(critterFile, frameOffset + 2, 8);
                            int hotspoty = (int)Util.getValAtAddress(critterFile, frameOffset + 3, 8);
                            int compression = (int)Util.getValAtAddress(critterFile, frameOffset + 4, 8);
                            int datalen = (int)Util.getValAtAddress(critterFile, frameOffset + 5, 16);
                            //Adjust the hotspots from the biggest point back to the image corners
                            int cornerX; int cornerY;
                            cornerX = MaxHotSpotX - hotspotx;
                            cornerY = MaxHotSpotY - hotspoty;
                            if (cornerX <= 0) { cornerX = 0; }
                            else { cornerX--; }
                            if (cornerY <= 0) { cornerY = 0; }

                            if (true)
                            {
                                //Merge the image into a new big image at the hotspot coordinates.;
                                byte[] srcImg;

                                srcImg = new byte[BitMapWidth * BitMapHeight * 2];
                                ArtLoader.ua_image_decode_rle(critterFile, srcImg, compression == 6 ? 5 : 4, datalen, BitMapWidth * BitMapHeight, frameOffset + 7, auxPalVal);
                                cornerY = MaxHeight - cornerY;//y is from the top left corner

                                int ColCounter = 0; int RowCounter = 0;
                                bool ImgStarted = false;
                                for (int y = 0; y < MaxHeight; y++)
                                {
                                    for (int x = 0; x < MaxWidth; x++)
                                    {
                                        if ((cornerX + ColCounter == x) && (MaxHeight - cornerY + RowCounter == y) && (ColCounter < BitMapWidth) && (RowCounter < BitMapHeight))
                                        {//the pixel from the source image is here 
                                            ImgStarted = true;
                                            outputImg[x + (y * MaxWidth)] = srcImg[ColCounter + (RowCounter * BitMapWidth)];
                                            ColCounter++;
                                        }
                                        else
                                        {
                                            outputImg[x + (y * MaxWidth)] = 0;//alpha
                                        }
                                    }
                                    if (ImgStarted == true)
                                    {//New Row on the src image
                                        RowCounter++;
                                        ColCounter = 0;
                                    }
                                }
                                //Set the heights for output
                                BitMapWidth = MaxWidth;
                                BitMapHeight = MaxHeight;

                                var imgData = ArtLoader.Image(this,outputImg, 0,0, BitMapWidth, BitMapHeight, "namehere", pal, true,  BitmapUW.ImageTypes.EightBitUncompressed);
                                //CropImageData(ref imgData, pal);
                                AnimInfo.animSprites[spriteIndex++] = imgData; //Sprite.Create(imgData, new Rect(0f, 0f, imgData.width, imgData.height), new Vector2(0.5f, 0.0f));
                            }
                        }//end extrac frameoffset
                    }//End for loop extract
                }//End extract images

            }
            //Debug.Log(fileCrit + " returning  "  + spriteIndex);
            return spriteIndex;


        }




        public class CritterAnimInfo
        {
            public const int NoOfAnims = 200;
            public string[,] animSequence;
            public int[,] animIndices;
            //public ArtLoader.RawImageData[,] animSprites;
            public BitmapUW[] animSprites;
            public string[] animName;

            public CritterAnimInfo()
            {
                animSequence = new string[NoOfAnims, 8];
                animIndices = new int[NoOfAnims, 8];
                animSprites = new BitmapUW[256];
                animName = new string[NoOfAnims];
            }
        }




    }
}
