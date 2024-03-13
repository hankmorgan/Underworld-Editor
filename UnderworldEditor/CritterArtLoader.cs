using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Drawing;

namespace UnderworldEditor
{
    class CritterArtLoader
    {       
        public static void LoadCritterArt(int game, TreeNode critnode, Palette pal)
        {
            if (game == 1)
            {
                ReadUw1AssocFile(Path.Combine(main.basepath, "CRIT", "ASSOC.ANM"), pal, critnode);
            }
            if (game == 2)
            {
                ReadUW2AssocFile(critnode);
            }
        }



        static void ReadUW2AssocFile(TreeNode critnode)
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
                    int FileID = (int)Util.getAt(assoc, AssocAddressPtr++, 8);
                    int auxPal = (int)Util.getAt(assoc, AssocAddressPtr++, 8);
                    if (FileID != 255)
                    {
                        var critname = objects.ObjectName(ass + 64, 2);
                        TreeNode assocNode = critnode.Nodes.Add(critname + "(" + ass + ") File " + FileID);
                        CritterArt.critterArt[ass] = new CritterArt(ass, FileID,PaletteLoader.Palettes[0], auxPal, assoc, pgmp, cran, assocNode);
                    }
                }
            }
        }


        static void ReadUw1AssocFile(string assocpath, Palette pal, TreeNode critnode)
        {
            long AssocAddressPtr = 256;
            if (Util.ReadStreamFile(assocpath, out byte[] assoc))
            {
                for (int ass = 0; ass <= 63; ass++)//Ignore npc 63 which has no animations
                {
                    int FileID = (int)Util.getAt(assoc, AssocAddressPtr++, 8);
                    int auxPal = (int)Util.getAt(assoc, AssocAddressPtr++, 8);
                    var critname = objects.ObjectName(ass+64,1);
                    TreeNode assocNode = critnode.Nodes.Add(critname + "("+ass+") File "+ FileID);

                    CritterArt.critterArt[ass] = new CritterArt(ass, FileID, pal, auxPal, assocNode);
                }
            }
        }
    }


    class CritterArt:ArtLoader
    {
        public static bool Loaded = false;
        public static CritterArt[] critterArt = new CritterArt[64];

        public BitmapUW[] animSprites = new BitmapUW[256];

        public Dictionary<string, CritterAnimation> Animations = new Dictionary<string, CritterAnimation>();

        public CritterArt(int CritterNo, int fileXX, Palette paletteToUse, int AuxPalNo, TreeNode assocNode)
        {
            string critterIDO = Util.DecimalToOct(fileXX.ToString());
            byte[] FilePage0;
            byte[] FilePage1;

            int StartingSpriteIndex = 0;
            for (int fileYY = 0; fileYY < 2; fileYY++)
            {
                //load in both page files.
                if (fileYY == 0)
                {//CR{CRITTER file ID in octal}PAGE.N{Page}
                    var toLoad = Path.Combine(main.basepath, "CRIT", "CR" + critterIDO + "PAGE.N0" + fileYY);
                    Util.ReadStreamFile(toLoad, out FilePage0);
                    StartingSpriteIndex = ReadPageFileUW1(CritterNo,FilePage0, fileXX, fileYY, StartingSpriteIndex, AuxPalNo, assocNode);
                }
                else
                {
                    var toLoad = Path.Combine(main.basepath, "CRIT", "CR" + critterIDO + "PAGE.N0" + fileYY);
                    Util.ReadStreamFile(toLoad, out FilePage1);
                    ReadPageFileUW1(CritterNo,FilePage1, fileXX, fileYY, StartingSpriteIndex, AuxPalNo, assocNode);
                }
            }
            Loaded = true;
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
        public CritterArt(int CritterNo, int file_id, Palette paletteToUse, int AuxPalNo, byte[] assocData, byte[] PGMP, byte[] cran, TreeNode assocNode)
        {
            int ExtractPageNo = 0;
            string critterIDO = Util.DecimalToOct(file_id.ToString());
            int spriteIndex = 0;
            for (int i = 0; i < 8; i++)
            {
                if ((int)Util.getAt(PGMP, file_id * 8 + i, 8) != 255)//Checks if a critter exists at this index in the page file.
                {
                    string ExtractPageNoOctal = Util.DecimalToOct(ExtractPageNo.ToString());
                    string fileCrit = Path.Combine(main.basepath, "CRIT", "CR" + critterIDO + "." + ExtractPageNoOctal);  // BasePath + sep + "CRIT" + sep + "CR" + critterIDO + "." + ExtractPageNoO;
                    spriteIndex = ReadPageFileUW2(CritterNo, assocData, AuxPalNo, fileCrit,spriteIndex, paletteToUse);
                    ExtractPageNo++;
                }
            }

            
            int cranAdd = (file_id * 512); //address lookup into cr.an

            for (int Animation = 0; Animation < 8; Animation++)//The animation slots
            {
                for (int Angle = 0; Angle < 8; Angle++)//Each animation has 8 possible angles.
                {
                    string newAnimName  = GetUW2AnimName(Animation, Angle);
                    int[] newIndices = new int[8];                  
                    TreeNode AnimationSet = assocNode.Nodes.Add(newAnimName);
                    int NoOfValidEntries = (int)Util.getAt(cran, cranAdd + (Animation * 64) + (Angle * 8) + (7), 8);//Get how many valid frames are in the animation
                    for (int FrameNo = 0; FrameNo < 8; FrameNo++)
                    {
                        int currFrame = (int)Util.getAt(cran, cranAdd + (Animation * 64) + (Angle * 8) + (FrameNo), 8);
                        if (FrameNo < NoOfValidEntries)
                        {
                            newIndices[FrameNo] = currFrame;
                            var ImageNode = AnimationSet.Nodes.Add($"{currFrame}");
                            ImageNode.Tag = "CRITTER:," + CritterNo + "," + (currFrame).ToString();
                        }
                        else
                        {
                            newIndices[FrameNo] = -1;
                        }
                    }

                    var newAnim = new CritterAnimation(newAnimName,newIndices);      

                    Animations.Add(newAnimName, newAnim);
                }
            }
            Loaded = true;
        }

        private string GetUW2AnimName(int animation, int angle)
        {
            /*
                  x*512 : start character x animation definition [C]
                Each chunk has 8 subchunks of 64 bytes. A subchunk [SC] describes the
                animation frames to take for a certain action. The actions are
                  [C]+0000 : [SC] standing 0
                  [C]+0040 : [SC] walking  1
                  [C]+0080 : [SC] in combat 2
                  [C]+00c0 : [SC] attack 3
                  [C]+0100 : [SC] attack 4
                  [C]+0140 : [SC] attack 5
                  [C]+0180 : [SC] attack 6
                  [C]+01c0 : [SC] dying 7
                */
            string output=$"UNKNOWNANIM_{animation}";
            switch(animation)
            {
                case 0:
                    output = "idle_";break;
                case 1:
                    output = "walking_"; break;
                case 2:
                    output = "idle_combat_"; break;
                case 3:
                    output = "attack_bash_"; break;
                case 4:
                    output = "attack_slash_"; break;
                case 5:
                    output = "attack_stab_"; break;
                case 6:
                    output = "attack_secondary_"; break;
                case 7:
                    output = "death_"; break;

            }

            //[SC] +0000 : [AS] rear  0
            //[SC] + 0008 : [AS] rear right 1
            //[SC]+0010 : [AS] right  2
            //[SC] + 0018 : [AS] front right 3
            //[SC]+0020 : [AS] front  4
            //[SC] + 0028 : [AS] front left 5
            //[SC]+0030 : [AS] left  6
            //[SC] + 0038 : [AS] rear left 7

            switch (angle)
            {
                case 0:
                     output += "rear"; break;
                case 1:
                    output += "rear_right"; break;
                case 2:
                    output += "right"; break;
                case 3:
                    output += "front_right"; break;
                   case 4:
                    output += "front"; break;
                case 5:
                    output += "front_left"; break;
                case 6:
                    output += "left"; break;
                case 7:
                    output += "rear_left"; break;
            }

            return output;
        }

        public static string GetUW1AnimName(int animNo)
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
                    return "attack_stab";
                //case 0x4:
                //    return "attack_unk4";
                case 0x5:
                    return "attack_secondary";
                //case 0x6:
                //    return "attack_unk6"; //does not exist
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
                    return "ethereal_anim_80";
                case 0x51:
                    return "ethereal_anim_81";
                case 0x52:
                    return "ethereal_anim_82";
                case 0x53:
                    return "ethereal_anim_83";
                case 0x54:
                    return "ethereal_anim_84";
                case 0x55:
                    return "ethereal_anim_85";
                case 0x56:
                    return "ethereal_anim_86";
                case 0x57:
                    return "ethereal_anim_87";
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

        /// <summary>
        /// Reads in the page files for UW1. adds them to a tree and store the animation sequences
        /// </summary>
        /// <param name="CritterNo"></param>
        /// <param name="PageFile"></param>
        /// <param name="XX"></param>
        /// <param name="YY"></param>
        /// <param name="spriteIndex"></param>
        /// <param name="AuxPalNo"></param>
        /// <param name="assocNode"></param>
        /// <returns></returns>
        private int ReadPageFileUW1(int CritterNo, byte[] PageFile, int XX, int YY, int spriteIndex, int AuxPalNo, TreeNode assocNode)
        {
            int addptr = 0;
            int slotbase = (int)Util.getAt(PageFile, addptr++, 8);
            int NoOfSlots = (int)Util.getAt(PageFile, addptr++, 8);
            int[] SlotIndices = new int[NoOfSlots];
            int spriteCounter = 0;
            int k = 0;
            //string XXo = Util.DecimalToOct(XX.ToString());
            //string YYo = Util.DecimalToOct(YY.ToString());
            for (int i = 0; i < NoOfSlots; i++)
            {
                int val = (int)Util.getAt(PageFile, addptr++, 8);
                if (val != 255)
                {
                    SlotIndices[k++] = i;
                }
            }
            int NoOfSegs = (int)Util.getAt(PageFile, addptr++, 8);
            for (int i = 0; i < NoOfSegs; i++)
            {
                string AnimName = GetUW1AnimName(slotbase + SlotIndices[i]);
                //int index = slotbase + SlotIndices[i]; //TranslateAnimToIndex(slotbase + SlotIndices[i]);

                TreeNode AnimationSet = assocNode.Nodes.Add($"{AnimName} {slotbase + SlotIndices[i]}");
                //
                int ValidCount = 0;
                int[] newIndices = new int[8];
                for (int j = 0; j < 8; j++)
                {                    
                    int val = (int)Util.getAt(PageFile, addptr++, 8);
                    if (val != 255)
                    {                  
                        newIndices[j] = (val + spriteIndex);
                        var ImageNode = AnimationSet.Nodes.Add($"{(val + spriteIndex)}");
                        ImageNode.Tag = "CRITTER:," + CritterNo + "," + (val + spriteIndex).ToString();
                        ValidCount++;
                    }
                    else
                    {
                        newIndices[j] = -1;
                    }
                }
                var newanim = new CritterAnimation(AnimName, newIndices);
                Animations.Add(AnimName, newanim);
            }

            //Read in the palette
            int NoOfPals = (int)Util.getAt(PageFile, addptr, 8);//Will skip ahead this far.
            addptr++;
            byte[] auxPalVal = new byte[32];
            for (int i = 0; i < 32; i++)
            {
                auxPalVal[i] = (byte)Util.getAt(PageFile, (addptr) + (AuxPalNo * 32) + i, 8);
            }

            //Skip past the palettes
            addptr += NoOfPals * 32;
            int NoOfFrames = (int)Util.getAt(PageFile, addptr, 8);
            addptr += 2;
            int addptr_start = addptr;//Bookmark my position
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
                        int frameOffset = (int)Util.getAt(PageFile, addptr + (i * 2), 16);
                        int BitMapWidth = (int)Util.getAt(PageFile, frameOffset + 0, 8);
                        int BitMapHeight = (int)Util.getAt(PageFile, frameOffset + 1, 8);
                        int hotspotx = (int)Util.getAt(PageFile, frameOffset + 2, 8);
                        int hotspoty = (int)Util.getAt(PageFile, frameOffset + 3, 8);
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
                        int frameOffset = (int)Util.getAt(PageFile, addptr + (i * 2), 16);
                        int BitMapWidth = (int)Util.getAt(PageFile, frameOffset + 0, 8);
                        int BitMapHeight = (int)Util.getAt(PageFile, frameOffset + 1, 8);
                        int hotspotx = (int)Util.getAt(PageFile, frameOffset + 2, 8);
                        int hotspoty = (int)Util.getAt(PageFile, frameOffset + 3, 8);
                        int compression = (int)Util.getAt(PageFile, frameOffset + 4, 8);
                        int datalen = (int)Util.getAt(PageFile, frameOffset + 5, 16);

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
                        BitmapUW imgData = ArtLoader.Image(this, outputImg, 0, 0, BitMapWidth, BitMapHeight, "name_goes_here", PaletteLoader.GreyScale, true, BitmapUW.ImageTypes.EightBitUncompressed);
                        this.animSprites[spriteIndex + i] = imgData;
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
        /// <param name="critterinfo"></param>
        /// <param name="spriteIndex"></param>
        /// <param name="paletteToUse"></param>
        /// <returns></returns>
        int ReadPageFileUW2(int CritterNo, byte[] assocFile, int AuxPalNo, string fileCrit, int spriteIndex, Palette paletteToUse)
        {
            //AuxPalNo = 3; auxpal 3 for stone strike
            //Local auxilary palette
            Palette pal = paletteToUse;
            int AddressPointer;

            Util.ReadStreamFile(fileCrit, out byte[] critterFile);

            AddressPointer = 0;

            byte[] auxPalVal = new byte[32];
            for (int j = 0; j < 32; j++)
            {
                auxPalVal[j] = (byte)Util.getAt(critterFile, (AddressPointer) + (AuxPalNo * 32) + j, 8);
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
                        int frameOffset = (int)Util.getAt(critterFile, index, 16);
                        if (frameOffset != 0)
                        {
                            int BitMapWidth = (int)Util.getAt(critterFile, frameOffset + 0, 8);
                            int BitMapHeight = (int)Util.getAt(critterFile, frameOffset + 1, 8);
                            int hotspotx = (int)Util.getAt(critterFile, frameOffset + 2, 8);
                            int hotspoty = (int)Util.getAt(critterFile, frameOffset + 3, 8);
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
                        int frameOffset = (int)Util.getAt(critterFile, index, 16);
                        if (frameOffset != 0)
                        {
                            int BitMapWidth = (int)Util.getAt(critterFile, frameOffset + 0, 8);
                            int BitMapHeight = (int)Util.getAt(critterFile, frameOffset + 1, 8);
                            int hotspotx = (int)Util.getAt(critterFile, frameOffset + 2, 8);
                            int hotspoty = (int)Util.getAt(critterFile, frameOffset + 3, 8);
                            int compression = (int)Util.getAt(critterFile, frameOffset + 4, 8);
                            int datalen = (int)Util.getAt(critterFile, frameOffset + 5, 16);
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
                               this.animSprites[spriteIndex++] = imgData;
                            }
                        }//end extract frameoffset
                    }//End for loop extract
                }//End extract images

            }
            //Debug.Log(fileCrit + " returning  "  + spriteIndex);
            return spriteIndex;
        }




        /// <summary>
        /// Class for storing info about the animation sequence.
        /// </summary>
        public class CritterAnimation
        {
            public string animName;
            public int[] animSequence = { -1,-1,-1,-1,-1,-1,-1,-1};
            public int[] animIndices = { -1, -1, -1, -1, -1, -1, -1, -1 }; //indices for the bitmap images?

            public CritterAnimation(string _animName, int[] _indices)
            {
                animName = _animName;
                animIndices = _indices;
            }
        }    
       
    }
}
