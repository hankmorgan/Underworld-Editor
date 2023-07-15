using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace UnderworldEditor
{
    public class Repacker
    {

        byte[] Ark = new byte[30000];


        public int DataCompression(ref byte[] inputData, ref byte[] outputData, ref int ReadPtr, int argC = 0x7E08, int argA = 0x8DD0, int arg4 = 0 )
        {
            var InputDataPtr = 0;
            //int arg4 = -1;
            int WriteAddressArg6 = 0;
            //int argA = 0;

            int Var14_16 = 0;
            int Var12 = 0; //WriteAddress
            char[] var28Array = new char[12];
            Int16 Var28 = 0;
            //Int16 WritePtr;
            byte VarE;
            byte VarB;
            Int16 VarA;
            Int16 Var8;
            Int16 Var6;
            Int16 Var4;
            Int16 CurrentByte_Var2 = 0;
            Int16 si;
            Int16 di;

            Var12 = WriteAddressArg6;

            if (arg4 != 0xFFFF)
            {
                Var14_16 += argC;
            }
            else
            {
                Var14_16 += argA;
            }


            //127:7C3
            Util.StoreInt16(Ark, 1, 0);
            Util.StoreInt16(Ark, 3, 0);
            Util.StoreInt16(Ark, 5, 0);
            Util.StoreInt16(Ark, 7, 0);
            Util.StoreInt16(Ark, 9, 0);
            Util.StoreInt16(Ark, 0xB, 0);
            Util.StoreInt16(Ark, 0xD, 1);

            Util.StoreInt16(Ark, 0x722B, 0xFFFF);

            Util.StoreInt16(Ark, 0x722D, 0);
            //ArkOperations[0x722D] = 0;
            //ArkOperations[0x722E] = 0;

            si = 0x1001;
        //jump 127:82F

        //127:82F
        ovr127_82F:
            if (si <= 0x1100)
            {
                //goto ovr127_81D;
                //127:81D
                //ovr127_81D:
                Util.StoreInt16(Ark, 0x3027 + si * 2, 0x1000);
                //ArkOperations[0x3027 + si * 2] = 0x10;
                // ArkOperations[0x3028 + si * 2] = 0x00;
                si++;
                goto ovr127_82F;
            }
            else
            {
                si = 0;
                goto ovr127_84B;
            }

        //ovr127_84B: 
        ovr127_84B:
            if (si < 0x1000)
            {
                //ovr127_839:
                Util.StoreInt16(Ark, 0x5229 + si * 2, 0x1000);
                //ArkOperations[0x5229 + si * 2] = 0x10;
                //ArkOperations[0x5230 + si * 2] = 0x00;
                si++;
                goto ovr127_84B;
            }
            else
            {
                VarB = 1;
                VarA = (Int16)(0);
                di = 0;
                Var6 = (Int16)(0xFEE);
                si = di;
                goto ovr127_874;
            }

        ovr127_874:
            //ovr127_874
            if (si < (int)Var6)
            {
                //goto ovr127_86a;
                //ovr127_86a:
                Ark[si + 0x12] = 0x20;
                si++;
                goto ovr127_874;
            }
            else
            {
                Var4 = (Int16)0;
                goto ovr127_894;
            }

        ovr127_894:
            if (Var4 >= 0x12)
            {
                goto ovr127_8BE;
            }
            else
            {
                if ((int)argC == 0)
                {
                    CurrentByte_Var2 = -1; //(Int16)0xFFFF;
                }
                else
                {
                    argC--;
                    VarE = inputData[ReadPtr];
                    CurrentByte_Var2 = (Int16)inputData[ReadPtr];
                    ReadPtr++;
                }

                if (CurrentByte_Var2 != -1) //0xFFFF)
                {
                    goto ovr127_880;
                ovr127_880:
                    Ark[(int)Var6 + (int)Var4 + 0x12] = (byte)CurrentByte_Var2;
                    goto ovr127_894;
                }
                else
                {
                    goto ovr127_8BE;
                }
            }

        ovr127_8BE://TODO
            cwd(Var4, out char tmpAX, out char tmpDX);
            //To confirm. is this 0,1,2,3 or 1,2,3,4
            Util.StoreInt16(Ark, 1, tmpAX);
            //ArkOperations[1] = (byte)(tmpAX & 0xF);//store ax of word
            //ArkOperations[2] = (byte)((tmpAX >> 8) & 0xF);
            Util.StoreInt16(Ark, 1, tmpDX);
            //ArkOperations[3] = (byte)(tmpDX);//store sign of word
            //ArkOperations[4] = (byte)((byte)(tmpDX >> 8) & 0xF);
            if (Var4 != 0)
            {
                goto ovr127_8D5;
            }
            else
            {
                goto ovr127_C0B;
            }


        ovr127_8D5:
            si = 1;
            goto ovr127_8E7;

        ovr127_8E7:
            if (si <= 0x12)
            {
                SubFunction127_0(Var6);
                goto ovr127_8F5;
            }
            else
            {
                SubFunction127_0((short)(Var6 - si));
                goto ovr127_8E7;
            }

        ovr127_8F5:
            if (Ark[0x10] <= Var4)
            {
                goto ovr127_909;
            }
            else
            {
                Ark[0x10] = (byte)Var4;
                goto ovr127_909;
            }

        ovr127_909:
            {
                byte al;
                if (Ark[0x10] > 2)
                {
                    goto ovr127_92D;
                ovr127_92D:
                    Ark[VarA + Var28] = Ark[0xE];
                    VarA++;
                    var a = Util.getAt(Ark, 0xE, 16);
                    a = (a >> 4) & 0xF0;
                    var d = Ark[0x10] - 3;
                    al = (byte)(a | d);
                }
                else
                {
                    goto ovr127_914;
                ovr127_914:
                    Util.StoreInt16(Ark, 0x10, 1);
                    Var28 = (short)(Var28 | VarB);
                    al = Ark[Var6 + 0x12];
                    goto ovr127_958;
                }

            ovr127_958:
                var28Array[Var28 + VarA] = (char)al;
                VarA++;
                VarB = (byte)(VarB << 1);

                if (VarB == 0)
                {
                    goto ovr127_972;
                ovr127_972:
                    si = 0;
                    goto ovr127_9E9;
                }
                else
                {
                    goto ovr127_A0C;
                }
            }

        ovr127_9E9:
            if (si < VarA)
            {
                goto ovr127_976;
            ovr127_976:
                if (Var12 >= Var14_16)
                {
                    // goto ovr127_992;
                    //File Write With Params
                    // ovr127_992:

                }
                else
                {
                    goto ovr127_97E;
                ovr127_97E:
                    Var12 = Var28 + si;
                    Var12++;
                    goto ovr127_9E8;

                }
            }
            else
            {
                goto ovr127_9EE;
            ovr127_9EE:
                //Unfinished here. Figure out ADC
                {
                    var x = Util.getAt(Ark, 5, 32);
                    x = x + VarA;
                    Util.StoreInt32(Ark, 5, x);
                    Var28 = 0;
                    VarB = 1;
                    VarA = 1;
                    goto ovr127_A0C;
                }
            }


        ovr127_9E8:
            si++;
            goto ovr127_9E9;


        ovr127_A0C:
            Var8 = Ark[0x10];
            si = 0;
            goto ovr127_A57;


        ovr127_A32:
            Ark[0x1012 + di] = (byte)CurrentByte_Var2;
            goto ovr127_A3B;


        ovr127_A3B:
            {
                di = (short)((di + 1) & (0xFFF));
                Var6 = (short)((Var6 + 1) & (0xFFF));
                SubFunction127_0(Var6);
                si++;
                goto ovr127_A57;
            }

        ovr127_A57:
            if (si >= Var8)
            {
                goto ovr127_A80;
            }
            else
            {
                if (argC == 0)
                {
                    goto ovr127_A75;
                ovr127_A75:
                    CurrentByte_Var2 = -1;
                    goto ovr127_A78;
                }
                else
                {
                    argC--;
                    CurrentByte_Var2 = inputData[InputDataPtr++];
                    goto ovr127_A78;
                }
            }




        ovr127_A78:
            if (CurrentByte_Var2 != -1)
            {
                goto ovr127_A1B;
            ovr127_A1B:
                SubFunction127_23C(di);
                Ark[di + 0x12] = (byte)CurrentByte_Var2;
                if (di >= 0x11)
                {
                    goto ovr127_A3B;
                }
                else
                {
                    goto ovr127_A32;
                }

            }
            else
            {
                goto ovr127_A80;

            }

        ovr127_A80:
            {//another adc to check
                var x = Util.getAt(Ark, 1, 32);
                x = x + si;
                Util.StoreInt32(Ark, 1, x);
                if (Ark[3] < Ark[0xB])
                {
                    goto ovr127_AE1;
                }
                else
                {
                    if (Ark[3] > Ark[0xB])
                    {
                        goto ovr127_AA9;
                    }
                    else
                    {//[3]=[0xB]
                        goto ovr127_AA3;
                    }
                }
            }

        ovr127_AA3:
            if (Ark[1] <= Ark[9])
            {
                goto ovr127_AE1;
            }
            else
            {
                goto ovr127_AA9;
            }

        ovr127_AA9:
            {
                var x = Util.getAt(Ark, 9, 32);
                x = x + 0x400;
                Util.StoreInt32(Ark, 9, x);
                goto ovr127_AE1;
            }

        ovr127_ABA:
            {
                SubFunction127_23C(di);
                di = (short)((di + 1) & 0xFFF);
                Var6 = (short)((Var6 + 1) & 0xFFF);
                Var4--;
                if (Var4 == 0)
                {
                    goto ovr127_AE1;
                }
                else
                {
                    SubFunction127_0(Var6);
                    goto ovr127_AE1;
                }
            }


        ovr127_AE1:
            {
                si++;
                if (si - 1 < Var8)
                {
                    goto ovr127_ABA;
                }
                else
                {
                    goto ovr127_AE9;
                }
            }


        ovr127_AE9:
            {
                if (Var4 <= 0)
                {
                    goto ovr127_AF2;
                }
                else
                {
                    goto ovr127_8F5;
                }
            }


        ovr127_AF2:
            {
                if (VarA > 1)
                {
                    goto ovr127_AFB;
                }
                else
                {
                    goto ovr127_B87;
                }
            }

        ovr127_AFB:
            {
                si = 0;
                goto ovr127_B72;
            }

        ovr127_B72:
            {
                if (si < VarA)
                {
                    //    goto ovr127_AFF;
                    //ovr127_AFF:
                    //    if (Var12>=Var14_16)//?
                    //    {
                    //        goto ovr127_B1B;
                    //    }
                    //else
                    //    {
                    //    goto ovr127_AFF;

                    //    }
                    outputData[Var12] = (byte)var28Array[si];
                    si++;
                    goto ovr127_B72;

                }
                else
                {
                    var x = Util.getAt(Ark, 5, 32);
                    x = x + VarA;
                    Util.StoreInt32(Ark, 5, 32);
                    goto ovr127_B87;
                }
            }

        ovr127_B87:
            {
                if (arg4 == 0xFFFF)
                {
                    goto ovr127_BD1;
                }
                else
                {
                    //File Write to handle.
                    //.....//
                    Var12 = WriteAddressArg6;
                    goto ovr127_BD1;
                }
            }

        ovr127_BD1:
            {
                if (Ark[3] < Ark[7])
                {
                    goto ovr127_C00;
                }
                else
                {
                    goto ovr127_BE3;
                }
            }

        ovr127_BE3:
            {
                if (Ark[3] > Ark[7])
                {
                    goto ovr127_BEB;
                }
                else
                {
                    goto ovr127_BE5;
                ovr127_BE5:
                    if (Ark[1] <= Ark[5])
                    {
                        goto ovr127_C00;
                    }
                    else
                    {
                        goto ovr127_BEB;
                    }
                }
            }

        ovr127_BEB:
            return Ark[5];

        ovr127_C00:
            goto ovr127_C0B;

        ovr127_C0B:
            return 0;
        }





        void SubFunction127_0(Int16 arg0)
        {
            int var6;
            Int16 var2;
            Int16 cx;
            Int16 di;
            Int16 si;

            cx = arg0;
            var2 = 1;

            var6 = 0x12 + cx;

            di = (short)(Ark[var6] + 0x1001);

            Util.StoreInt16(Ark, 0x1025 + cx * 2, 0x1000);
            Util.StoreInt16(Ark, 0x3027 + cx * 2, 0x1000);
            Util.StoreInt16(Ark, 0x10, 0);

            goto ovr127_5A;

        ovr127_5A:
            if (var2 < 0)
            {
                goto ovr127_A5;
            ovr127_A5:
                if (Util.getAt(Ark, 0x1025 + di * 2, 16) == 0x4096)
                {
                    goto ovr127_C9;
                }
                else
                {
                    goto ovr127_B8;
                }

            }
            else
            {
                goto ovr127_60;
            ovr127_60:
                //
                if (Util.getAt(Ark, 0x3027 + di * 2, 16) == 0x1000)
                {
                    goto ovr127_84;
                }
                else
                {
                    goto ovr127_73;
                }
            }

        ovr127_73:
            di = Ark[0x3027 + di * 2];
            goto ovr127_EA;

        ovr127_84:
            Util.StoreInt16(Ark, 0x3027 + di * 2, cx);
            Util.StoreInt16(Ark, 0x5229 + cx * 2, di);
            goto ovr127_238;

        ovr127_B8:
            di = Ark[0x1025 + di * 2];
            goto ovr127_EA;

        ovr127_C9:
            Util.StoreInt16(Ark, 0x1025 + di * 2, cx);
            Util.StoreInt16(Ark, 0x5229 + cx * 2, di);
            goto ovr127_238;

        ovr127_EA:
            si = 1;
            goto ovr127_10D;

        ovr_127_10C:
            si++;
            goto ovr127_10D;

        ovr127_10D:
            if (si < 0x12)
            {
                goto ovr127_EF;
            ovr127_EF:
                var al = Ark[var6 + si];
                var dl = Ark[di + si + 0x12];
                var2 = (short)(al - dl);
                if (var2 != 0)
                {
                    goto ovr127_112;
                }
                else
                {
                    goto ovr_127_10C;
                }
            }
            else
            {
                goto ovr127_112;
            }


        ovr127_112:
            if (Ark[0x10] < si)
            {
                goto ovr127_11F;
            }
            else
            {
                goto ovr127_5A;
            }


        ovr127_11F:
            Ark[0xE] = (byte)di;
            Ark[0x10] = (byte)si;
            if (si >= 0x12)
            {
                goto ovr127_135;
            }
            else
            {
                goto ovr127_5A;
            }

        ovr127_135:
            {
                // ovr127_14E:
                Ark[0x5229 + cx * 2] = Ark[0x5229 + di * 2];
                //ovr127_16C:
                Ark[0x1025 + cx * 2] = Ark[0x1025 + di * 2];
                //ovr127_18A:
                Ark[0x3027 + cx * 2] = Ark[0x3027 + di * 2];
                //ovr127_1A8:   
                Ark[0x5229 + cx * 2] = Ark[0x1025 + di * 2];
                //ovr127_1C6
                Ark[0x5229 + cx * 2] = Ark[0x3027 + di * 2];

                //ovr127_1cf
                // var ax = di << 1;
                var ax = (Ark[0x5229 + di * 2]) << 1;
                //ax = ax << 1;
                if (Ark[0x3027 + ax] != di)
                {
                    goto ovr127_209;
                }
                else
                {
                    goto ovr127_1E9;
                }
            }

        ovr127_1E9:
            {
                var ax = Ark[0x5229 + di * 2];
                ax = (byte)(ax << 1);
                Ark[0x3027 + ax] = (byte)cx;
                goto ovr127_227;
            }

        ovr127_209:
            {
                var ax = Ark[0x5229 + di * 2];
                ax = (byte)(ax << 1);
                Ark[0x1025 + ax] = (byte)cx;
                goto ovr127_227;
            }

        ovr127_227:
            Util.StoreInt16(Ark, 0x5229 + di * 2, 0x1000);
            goto ovr127_238;

        ovr127_238:
            return;

        }

        void SubFunction127_23C(Int16 arg0)
        {
            var di = arg0;
            Int16 si = 0;
            if (Util.getAt(Ark, 0x5229 + di * 2, 16) != 0x1000)
            {
                goto ovr127_25A;
            }
            else
            {
                goto ovr127_43C;
            }

        ovr127_25A:
            if (Util.getAt(Ark, 0x3027 + di * 2, 16) != 0x1000)
            {
                goto ovr127_27F;
            }
            else
            {
                si = Ark[0x1025 + di * 2];
                goto ovr127_3B5;
            }

        ovr127_27F:
            if (Util.getAt(Ark, 0x1025 + di * 2, 16) != 0x1000)
            {
                goto ovr127_2A4;
            }
            else
            {
                si = Ark[0x3027 + di * 2];
                goto ovr127_3B5;
            }

        ovr127_2A4:
            si = Ark[0x1204 + di * 2];
            if (Util.getAt(Ark, 0x3027 + si * 2, 16) != 0x1000)
            {
                goto ovr127_2C9;
            }
            else
            {
                goto ovr127_37B;
            }

        ovr127_2C9:
            si = Ark[0x3027 + si * 2];
            if (Util.getAt(Ark, 0x3027 + si * 2, 16) != 0x1000)
            {
                goto ovr127_2C9;
            }
            else
            {
                ////
                ///
                var ax = Ark[0x1025 + si * 2];
                var dx = Ark[0x5229 + si * 2];
                //ovr127_311:
                Ark[0x3027 + dx * 2] = ax;

                ax = Ark[0x5229 + si * 2];
                dx = Ark[0x1025 + si * 2];
                //ovr127_33C
                Ark[0x5229 + dx * 2] = ax;

                //ovr127_354:
                Ark[0x1025 + si * 2] = Ark[0x1025 + di * 2];

                //ovr127_369
                ax = Ark[1025 + di * 2]; ;
                Ark[0x5229 + ax * 2] = (byte)si;
                goto ovr127_37B;
            }

        ovr127_37B:
            {
                Ark[0x3026 + si * 2] = Ark[0x3027 + di * 2];
                var ax = Ark[0x3027 + di * 2];
                Ark[0x5229 + ax * 2] = (byte)si;
                goto ovr127_3B5;
            }

        ovr127_3B5:
            {
                //ovr127_3CE:
                Ark[0x5229 + si * 2] = Ark[0x5229 + di * 2];
                var ax = Ark[0x5229 + di * 2];
                if (Ark[0x3027 + ax * 2] != di)
                {
                    goto ovr127_40F;
                }
                else
                {
                    ax = Ark[0x5229 + di * 2];
                    Ark[0x3027 + ax * 2] = (byte)si;
                    goto ovr127_42B;
                }
            }

        ovr127_40F:
            {
                var ax = Ark[0x5229 + di * 2];
                Ark[0x1025 + ax * 2] = (byte)si;
                goto ovr127_42B;
            }

        ovr127_42B:
            Util.StoreInt16(Ark, 0x5229 + di * 2, 0x1000);
            goto ovr127_43C;

        ovr127_43C:
            return;
        }


        void cwd(Int16 ax, out char outAX, out char outDX)
        {
            if (ax < 0)
            {
                outDX = (char)0xFFFF;
            }
            else
            {
                outDX = (char)0;
            }
            outAX = (char)ax;
        }

    }
}