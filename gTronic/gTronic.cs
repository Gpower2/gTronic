using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;

namespace gTronic
{
    public class gTronic
    {
        #region "PInvokes"

        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, int dwExtraInfo);

        public enum MouseEventFlags
        {
            LEFTDOWN = 0x00000002,
            LEFTUP = 0x00000004,
            MIDDLEDOWN = 0x00000020,
            MIDDLEUP = 0x00000040,
            MOVE = 0x00000001,
            ABSOLUTE = 0x00008000,
            RIGHTDOWN = 0x00000008,
            RIGHTUP = 0x00000010
        }

        #endregion

        public enum BioColors
        {
            Red = 0,
            Blue = 1,
            Green = 2,
            LightBlue = 3,
            Purple = 4,
            Yellow = 5,
            Orange = 6,
            UnknownColor = 7,
            UberBomb = 8
        };

        public class BioBlock
        {
            public int Row { get; set; }
            public int Column { get; set; }
        }

        public class BioMove
        {
            public BioBlock StartBlock { get; set; }
            public BioBlock EndBlock { get; set; }
            public int TotalBlocks { get; set; }
            public int TotalCombos { get; set; }
            public bool HasBomb { get; set; }
            public bool IsUberBomb { get; set; }
            public List<BioBlock> HorizontalBlocks { get; set; }
            public List<BioBlock> VerticalBlocks { get; set; }

            public BioMove()
            {
                HorizontalBlocks = new List<BioBlock>();
                VerticalBlocks = new List<BioBlock>();
            }
        }

        public class BioRGB
        {
            public int R { get; set; }
            public int G { get; set; }
            public int B { get; set; }

            public int R_PLUS_DIFF { get; set; }
            public int G_PLUS_DIFF { get; set; }
            public int B_PLUS_DIFF { get; set; }

            public int R_MINUS_DIFF { get; set; }
            public int G_MINUS_DIFF { get; set; }
            public int B_MINUS_DIFF { get; set; }
        }

        public static int BioWidth = 33;
        public static int BioHeight = 33;
        public static int BioSpace = 0;
        public static int BioWidthOffset = 8;
        public static int BioHeightOffset = 51;
        public static int BioMatrixWidth = 8;
        public static int BioMatrixHeight = 10;
        public static int BioBitmapWidth = 280;
        public static int BioBitmapHeight = 390;

        public static int RGB_DIFF_UBER_BOMB = 10;
        public static int RGB_DIFF = 15;

        public static int DELAY_BETWEEN_CLICK = 0; //in milliseconds
        public static int MAX_DELAY_BETWEEN_MOVES = 0; //in milliseconds
        public static int MIN_DELAY_BETWEEN_MOVES = 0; //in milliseconds
        public static int UNKNOWN_THRESHOLD = 20;
        public static int PLAYED_THRESHOLD = 3;

        public static String gTronicIniFile = "gTronic.ini";
        public static String gTronicLogFile = "gTronic.log";
        public static String gTronicColorFile = "gTronic.col";

        private List<BioRGB> Orange = new List<BioRGB>();
        private List<BioRGB> Purple = new List<BioRGB>();
        private List<BioRGB> LightBlue = new List<BioRGB>();
        private List<BioRGB> Red = new List<BioRGB>();
        private List<BioRGB> Blue = new List<BioRGB>();
        private List<BioRGB> Yellow = new List<BioRGB>();
        private List<BioRGB> Green = new List<BioRGB>();

        private List<BioRGB> OrangeEye = new List<BioRGB>();
        private List<BioRGB> PurpleEye = new List<BioRGB>();
        private List<BioRGB> LightBlueEye = new List<BioRGB>();
        private List<BioRGB> RedEye = new List<BioRGB>();
        private List<BioRGB> BlueEye = new List<BioRGB>();
        private List<BioRGB> YellowEye = new List<BioRGB>();
        private List<BioRGB> GreenEye = new List<BioRGB>();

        private List<BioRGB> OrangeBomb = new List<BioRGB>();
        private List<BioRGB> PurpleBomb = new List<BioRGB>();
        private List<BioRGB> LightBlueBomb = new List<BioRGB>();
        private List<BioRGB> RedBomb = new List<BioRGB>();
        private List<BioRGB> BlueBomb = new List<BioRGB>();
        private List<BioRGB> YellowBomb = new List<BioRGB>();
        private List<BioRGB> GreenBomb = new List<BioRGB>();

        private List<BioRGB> OrangeEyeBomb = new List<BioRGB>();
        private List<BioRGB> PurpleEyeBomb = new List<BioRGB>();
        private List<BioRGB> LightBlueEyeBomb = new List<BioRGB>();
        private List<BioRGB> RedEyeBomb = new List<BioRGB>();
        private List<BioRGB> BlueEyeBomb = new List<BioRGB>();
        private List<BioRGB> YellowEyeBomb = new List<BioRGB>();
        private List<BioRGB> GreenEyeBomb = new List<BioRGB>();

        private List<BioRGB> UberBomb = new List<BioRGB>();

        public BioColors[][] BioMatrix { get; set; }
        public bool[][] BioBombMatrix { get; set; }

        public StreamWriter gLog { get; set; }

        public Image BioBmp { get; set; }

        public int gLocationX { get; set; }
        public int gLocationY { get; set; }

        public int CountUnknown { get; set; }

        public bool IsStarted { get; set; }

        public gTronic()
        {
            ReadIni();
            ReadColors();

            BioMatrix = new BioColors[BioMatrixWidth][];
            BioBombMatrix = new bool[BioMatrixWidth][];

            for (int i = 0; i < BioMatrixWidth; i++)
            {
                BioMatrix[i] = new BioColors[BioMatrixHeight];
                BioBombMatrix[i] = new bool[BioMatrixHeight];
                for (int j = 0; j < BioMatrixHeight; j++)
                {
                    BioBombMatrix[i][j] = false;
                }
            }
        }

        public static Bitmap CaptureScreen(int width, int height, int x, int y)
        {
            Bitmap screenBitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            Graphics.FromImage(screenBitmap).CopyFromScreen(x, y, 0, 0, new Size(width, height), System.Drawing.CopyPixelOperation.SourceCopy);

            return screenBitmap;
        }

        public void PlayMouseMove(BioMove mov, Int32 argBorderWidth, Int32 argTitleBarHeight, Int32 argStartX, Int32 argStartY, Int32 argEndX, Int32 argEndY, bool argFullLog)
        {
            System.Windows.Forms.Cursor.Position = new Point(argStartX, argStartY);
            //Mouse Left Down and Mouse Left Up
            mouse_event((uint)MouseEventFlags.LEFTDOWN, (uint)argStartX, (uint)argStartY, 0, 0);
            mouse_event((uint)MouseEventFlags.LEFTUP, (uint)argStartX, (uint)argStartY, 0, 0);

            //System.Threading.Thread.Sleep(DELAY_BETWEEN_CLICK);

            System.Windows.Forms.Cursor.Position = new Point(argEndX, argEndY);
            mouse_event((uint)MouseEventFlags.LEFTDOWN, (uint)argEndX, (uint)argEndY, 0, 0);
            mouse_event((uint)MouseEventFlags.LEFTUP, (uint)argEndX, (uint)argEndY, 0, 0);

            //Check Full Log
            if (argFullLog)
            {
                gLog.WriteLine("Played Start Block : " + mov.StartBlock.Row + " " + mov.StartBlock.Column + " " + BioMatrix[mov.StartBlock.Row][mov.StartBlock.Column]);
                gLog.WriteLine("Played End Block : " + mov.EndBlock.Row + " " + mov.EndBlock.Column + " " + BioMatrix[mov.EndBlock.Row][mov.EndBlock.Column]);
            }
        }

        public void ReadIni()
        {
            gLocationX = 0;
            gLocationY = 0;
            if (File.Exists(gTronicIniFile))
            {
                using (StreamReader sr = new StreamReader(gTronicIniFile))
                {
                    String Line1 = sr.ReadLine();
                    String Line2 = sr.ReadLine();
                    gLocationX = Convert.ToInt32(Line1.Replace("x=", ""));
                    gLocationY = Convert.ToInt32(Line2.Replace("y=", ""));
                }
            }
        }

        public void ReadColors()
        {
            if (File.Exists(gTronicColorFile))
            {
                using (StreamReader sr = new StreamReader(gTronicColorFile))
                {
                    String tmpString;
                    while ((tmpString = sr.ReadLine()) != null)
                    {
                        String[] elements = tmpString.Split(new String[] { "," }, StringSplitOptions.None);
                        BioRGB tmp = new BioRGB();
                        tmp.R = Convert.ToInt32(elements[1]);
                        tmp.G = Convert.ToInt32(elements[2]);
                        tmp.B = Convert.ToInt32(elements[3]);

                        int diff = RGB_DIFF;
                        switch (elements[0])
                        {
                            case "Orange":
                                Orange.Add(tmp);
                                break;
                            case "Purple":
                                Purple.Add(tmp);
                                break;
                            case "LightBlue":
                                LightBlue.Add(tmp);
                                break;
                            case "Red":
                                Red.Add(tmp);
                                break;
                            case "Blue":
                                Blue.Add(tmp);
                                break;
                            case "Yellow":
                                Yellow.Add(tmp);
                                break;
                            case "Green":
                                Green.Add(tmp);
                                break;
                            case "OrangeEye":
                                OrangeEye.Add(tmp);
                                break;
                            case "PurpleEye":
                                PurpleEye.Add(tmp);
                                break;
                            case "LightBlueEye":
                                LightBlueEye.Add(tmp);
                                break;
                            case "RedEye":
                                RedEye.Add(tmp);
                                break;
                            case "BlueEye":
                                BlueEye.Add(tmp);
                                break;
                            case "YellowEye":
                                YellowEye.Add(tmp);
                                break;
                            case "GreenEye":
                                GreenEye.Add(tmp);
                                break;
                            case "OrangeBomb":
                                OrangeBomb.Add(tmp);
                                break;
                            case "PurpleBomb":
                                PurpleBomb.Add(tmp);
                                break;
                            case "LightBlueBomb":
                                LightBlueBomb.Add(tmp);
                                break;
                            case "RedBomb":
                                RedBomb.Add(tmp);
                                break;
                            case "BlueBomb":
                                BlueBomb.Add(tmp);
                                break;
                            case "YellowBomb":
                                YellowBomb.Add(tmp);
                                break;
                            case "GreenBomb":
                                GreenBomb.Add(tmp);
                                break;
                            case "OrangeEyeBomb":
                                OrangeEyeBomb.Add(tmp);
                                break;
                            case "PurpleEyeBomb":
                                PurpleEyeBomb.Add(tmp);
                                break;
                            case "LightBlueEyeBomb":
                                LightBlueEyeBomb.Add(tmp);
                                break;
                            case "RedEyeBomb":
                                RedEyeBomb.Add(tmp);
                                break;
                            case "BlueEyeBomb":
                                BlueEyeBomb.Add(tmp);
                                break;
                            case "YellowEyeBomb":
                                YellowEyeBomb.Add(tmp);
                                break;
                            case "GreenEyeBomb":
                                GreenEyeBomb.Add(tmp);
                                break;
                            case "UberBomb":
                                diff = RGB_DIFF_UBER_BOMB;
                                UberBomb.Add(tmp);
                                break;
                        }

                        tmp.R_PLUS_DIFF = tmp.R + diff;
                        tmp.G_PLUS_DIFF = tmp.G + diff;
                        tmp.B_PLUS_DIFF = tmp.B + diff;

                        tmp.R_MINUS_DIFF = tmp.R - diff;
                        tmp.G_MINUS_DIFF = tmp.G - diff;
                        tmp.B_MINUS_DIFF = tmp.B - diff;
                    }
                }
            }
        }

        public void FillBioMatrix(bool argFullLog)
        {
            int BioWidthSpace = BioWidth + BioSpace;
            int BioHeightSpace = BioHeight + BioSpace;
            //int count = 0;
            if (IsStarted)
            {
                for (int i = 0; i < BioMatrixWidth; i++)
                {
                    for (int j = 0; j < BioMatrixHeight; j++)
                    {
                        int x = BioWidthOffset + i * BioWidthSpace;
                        int y = BioHeightOffset + j * BioHeightSpace;
                        Bitmap BioTemp = new Bitmap(BioWidth, BioHeight);
                        Graphics g = Graphics.FromImage(BioTemp);
                        g.DrawImage(BioBmp, new Rectangle(0, 0, BioWidth, BioHeight), new Rectangle(x, y, BioWidth, BioHeight), GraphicsUnit.Pixel);
                        //BioMatrix[i][j] = DetectBioColor(BioTemp, i, j, argFullLog);
                        BioMatrix[i][j] = DetectBioColorNew(BioTemp, i, j);
                        if (CountUnknown > UNKNOWN_THRESHOLD)
                        {
                            return;
                        }
                    }
                }
            }
            //Check Full Log
            if (argFullLog)
            {
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < BioMatrixWidth; i++)
                {
                    for (int j = 0; j < BioMatrixHeight; j++)
                    {
                        builder.AppendLine(String.Format("Bioblock : {0},{1} {2}", i, j, BioMatrix[i][j]));
                    }
                }
                gLog.Write(builder.ToString());
            }
        }

        //public BioColors DetectBioColor(Bitmap BioTemp, int x, int y, bool argDebuggingMode)
        //{
        //    if (isStarted)
        //    {
        //        int Rsum = 0;
        //        int Gsum = 0;
        //        int Bsum = 0;

        //        int Ravg = 0;
        //        int Gavg = 0;
        //        int Bavg = 0;

        //        //Find Average Color
        //        int count = 0;
        //        //Normal Biotronics
        //        for (int z = 5; z <= 28; z++)
        //        {
        //            for (int w = 5; w <= 8; w++)
        //            {
        //                bool cond_dark = BioTemp.GetPixel(w, w).R < 50 && BioTemp.GetPixel(w, w).G < 50 && BioTemp.GetPixel(w, w).B < 50;
        //                bool cond_light = BioTemp.GetPixel(w, w).R > 240 && BioTemp.GetPixel(w, w).G > 240 && BioTemp.GetPixel(w, w).B > 240;
        //                if (!cond_dark || !cond_light)
        //                {
        //                    Rsum += BioTemp.GetPixel(z, w).R;
        //                    Gsum += BioTemp.GetPixel(z, w).G;
        //                    Bsum += BioTemp.GetPixel(z, w).B;
        //                    count++;
        //                }
        //            }
        //            for (int w = 24; w <= 28; w++)
        //            {
        //                bool cond_dark = BioTemp.GetPixel(w, w).R < 50 && BioTemp.GetPixel(w, w).G < 50 && BioTemp.GetPixel(w, w).B < 50;
        //                bool cond_light = BioTemp.GetPixel(w, w).R > 240 && BioTemp.GetPixel(w, w).G > 240 && BioTemp.GetPixel(w, w).B > 240;
        //                if (!cond_dark || !cond_light)
        //                {
        //                    Rsum += BioTemp.GetPixel(z, w).R;
        //                    Gsum += BioTemp.GetPixel(z, w).G;
        //                    Bsum += BioTemp.GetPixel(z, w).B;
        //                    count++;
        //                }
        //            }
        //        }
        //        for (int z = 9; z <= 23; z++)
        //        {
        //            for (int w = 5; w <= 8; w++)
        //            {
        //                bool cond_dark = BioTemp.GetPixel(w, w).R < 50 && BioTemp.GetPixel(w, w).G < 50 && BioTemp.GetPixel(w, w).B < 50;
        //                bool cond_light = BioTemp.GetPixel(w, w).R > 240 && BioTemp.GetPixel(w, w).G > 240 && BioTemp.GetPixel(w, w).B > 240;
        //                if (!cond_dark || !cond_light)
        //                {
        //                    Rsum += BioTemp.GetPixel(w, z).R;
        //                    Gsum += BioTemp.GetPixel(w, z).G;
        //                    Bsum += BioTemp.GetPixel(w, z).B;
        //                    count++;
        //                }
        //            }
        //            for (int w = 24; w <= 28; w++)
        //            {
        //                bool cond_dark = BioTemp.GetPixel(w, w).R < 50 && BioTemp.GetPixel(w, w).G < 50 && BioTemp.GetPixel(w, w).B < 50;
        //                bool cond_light = BioTemp.GetPixel(w, w).R > 240 && BioTemp.GetPixel(w, w).G > 240 && BioTemp.GetPixel(w, w).B > 240;
        //                if (!cond_dark || !cond_light)
        //                {
        //                    Rsum += BioTemp.GetPixel(w, z).R;
        //                    Gsum += BioTemp.GetPixel(w, z).G;
        //                    Bsum += BioTemp.GetPixel(w, z).B;
        //                    count++;
        //                }
        //            }
        //        }


        //        Ravg = Convert.ToInt32(Convert.ToDouble(Rsum) / Convert.ToDouble(count));
        //        Gavg = Convert.ToInt32(Convert.ToDouble(Gsum) / Convert.ToDouble(count));
        //        Bavg = Convert.ToInt32(Convert.ToDouble(Bsum) / Convert.ToDouble(count));

        //        //Reference Colors
        //        ////Orange 9C5432 156 84 50
        //        ////Purple 61528E 97 82 142
        //        ////LightBlue 5491A6 84 145 166
        //        ////Red 9B282C 155 40 44
        //        ////Blue 2A5582 42 85 130
        //        ////Yellow 827943 130 121 67
        //        ////Green 516823 81 104 35

        //        int RGBdiff = 20;

        //        int Red_R = 155;
        //        int Red_G = 40;
        //        int Red_B = 44;

        //        int Blue_R = 42;
        //        int Blue_G = 85;
        //        int Blue_B = 130;

        //        int Green_R = 81;
        //        int Green_G = 104;
        //        int Green_B = 35;

        //        int LightBlue_R = 84;
        //        int LightBlue_G = 145;
        //        int LightBlue_B = 166;

        //        int Purple_R = 97;
        //        int Purple_G = 82;
        //        int Purple_B = 142;

        //        int Yellow_R = 130;
        //        int Yellow_G = 121;
        //        int Yellow_B = 67;

        //        int Orange_R = 156;
        //        int Orange_G = 84;
        //        int Orange_B = 50;

        //        bool Red = (Ravg > (Red_R - RGBdiff) && Ravg < (Red_R + RGBdiff)) && (Gavg > (Red_G - RGBdiff) && Gavg < (Red_G + RGBdiff)) && (Bavg > (Red_B - RGBdiff) && Bavg < (Red_B + RGBdiff));
        //        bool Blue = (Ravg > (Blue_R - RGBdiff) && Ravg < (Blue_R + RGBdiff)) && (Gavg > (Blue_G - RGBdiff) && Gavg < (Blue_G + RGBdiff)) && (Bavg > (Blue_B - RGBdiff) && Bavg < (Blue_B + RGBdiff));
        //        bool Green = (Ravg > (Green_R - RGBdiff) && Ravg < (Green_R + RGBdiff)) && (Gavg > (Green_G - RGBdiff) && Gavg < (Green_G + RGBdiff)) && (Bavg > (Green_B - RGBdiff) && Bavg < (Green_B + RGBdiff));
        //        bool LightBlue = (Ravg > (LightBlue_R - RGBdiff) && Ravg < (LightBlue_R + RGBdiff)) && (Gavg > (LightBlue_G - RGBdiff) && Gavg < (LightBlue_G + RGBdiff)) && (Bavg > (LightBlue_B - RGBdiff) && Bavg < (LightBlue_B + RGBdiff));
        //        bool Purple = (Ravg > (Purple_R - RGBdiff) && Ravg < (Purple_R + RGBdiff)) && (Gavg > (Purple_G - RGBdiff) && Gavg < (Purple_G + RGBdiff)) && (Bavg > (Purple_B - RGBdiff) && Bavg < (Purple_B + RGBdiff));
        //        bool Yellow = (Ravg > (Yellow_R - RGBdiff) && Ravg < (Yellow_R + RGBdiff)) && (Gavg > (Yellow_G - RGBdiff) && Gavg < (Yellow_G + RGBdiff)) && (Bavg > (Yellow_B - RGBdiff) && Bavg < (Yellow_B + RGBdiff));
        //        bool Orange = (Ravg > (Orange_R - RGBdiff) && Ravg < (Orange_R + RGBdiff)) && (Gavg > (Orange_G - RGBdiff) && Gavg < (Orange_G + RGBdiff)) && (Bavg > (Orange_B - RGBdiff) && Bavg < (Orange_B + RGBdiff));

        //        ////Green Eye 606D4F 96 109 79
        //        ////Orange Eye 7D5A5B 125 90 91
        //        ////Purple Eye 706C8C 112 108 140
        //        ////LightBlue Eye 677A89 103 122 137
        //        ////Red Eye 92545A 146 84 90
        //        ////Blue Eye 465470 70 84 112
        //        ////Yellow Eye 68674D 104 103 77

        //        int Red_Eye_R = 146;
        //        int Red_Eye_G = 84;
        //        int Red_Eye_B = 90;

        //        int Blue_Eye_R = 70;
        //        int Blue_Eye_G = 84;
        //        int Blue_Eye_B = 112;

        //        int Green_Eye_R = 96;
        //        int Green_Eye_G = 109;
        //        int Green_Eye_B = 79;

        //        int LightBlue_Eye_R = 103;
        //        int LightBlue_Eye_G = 122;
        //        int LightBlue_Eye_B = 137;

        //        int Purple_Eye_R = 112;
        //        int Purple_Eye_G = 108;
        //        int Purple_Eye_B = 140;

        //        int Yellow_Eye_R = 104;
        //        int Yellow_Eye_G = 103;
        //        int Yellow_Eye_B = 77;

        //        int Orange_Eye_R = 125;
        //        int Orange_Eye_G = 90;
        //        int Orange_Eye_B = 91;

        //        int EyeDiff = 5;

        //        bool RedEye = (Ravg > (Red_Eye_R - EyeDiff) && Ravg < (Red_Eye_R + EyeDiff)) && (Gavg > (Red_Eye_G - EyeDiff) && Gavg < (Red_Eye_G + EyeDiff)) && (Bavg > (Red_Eye_B - EyeDiff) && Bavg < (Red_Eye_B + EyeDiff));
        //        bool BlueEye = (Ravg > (Blue_Eye_R - EyeDiff) && Ravg < (Blue_Eye_R + EyeDiff)) && (Gavg > (Blue_Eye_G - EyeDiff) && Gavg < (Blue_Eye_G + EyeDiff)) && (Bavg > (Blue_Eye_B - EyeDiff) && Bavg < (Blue_Eye_B + EyeDiff));
        //        bool GreenEye = (Ravg > (Green_Eye_R - EyeDiff) && Ravg < (Green_Eye_R + EyeDiff)) && (Gavg > (Green_Eye_G - EyeDiff) && Gavg < (Green_Eye_G + EyeDiff)) && (Bavg > (Green_Eye_B - EyeDiff) && Bavg < (Green_Eye_B + EyeDiff));
        //        bool LightBlueEye = (Ravg > (LightBlue_Eye_R - EyeDiff) && Ravg < (LightBlue_Eye_R + EyeDiff)) && (Gavg > (LightBlue_Eye_G - EyeDiff) && Gavg < (LightBlue_Eye_G + EyeDiff)) && (Bavg > (LightBlue_Eye_B - EyeDiff) && Bavg < (LightBlue_Eye_B + EyeDiff));
        //        bool PurpleEye = (Ravg > (Purple_Eye_R - EyeDiff) && Ravg < (Purple_Eye_R + EyeDiff)) && (Gavg > (Purple_Eye_G - EyeDiff) && Gavg < (Purple_Eye_G + EyeDiff)) && (Bavg > (Purple_Eye_B - EyeDiff) && Bavg < (Purple_Eye_B + EyeDiff));
        //        bool YellowEye = (Ravg > (Yellow_Eye_R - EyeDiff) && Ravg < (Yellow_Eye_R + EyeDiff)) && (Gavg > (Yellow_Eye_G - EyeDiff) && Gavg < (Yellow_Eye_G + EyeDiff)) && (Bavg > (Yellow_Eye_B - EyeDiff) && Bavg < (Yellow_Eye_B + EyeDiff));
        //        bool OrangeEye = (Ravg > (Orange_Eye_R - EyeDiff) && Ravg < (Orange_Eye_R + EyeDiff)) && (Gavg > (Orange_Eye_G - EyeDiff) && Gavg < (Orange_Eye_G + EyeDiff)) && (Bavg > (Orange_Eye_B - EyeDiff) && Bavg < (Orange_Eye_B + EyeDiff));


        //        ////Green Bomb "6A8422" 106 132 34
        //        ////Orange Bomb "AD7947" 173 121 71
        //        ////Purple Bomb "60318B" 96 49 139
        //        ////LightBlue Bomb "7EA6B5" 126 166 181
        //        ////Red Bomb "90383D" 144 56 61
        //        ////Blue Bomb "387199" 56 113 153
        //        ////Yellow Bomb "988F45" 152 143 69

        //        int Red_Bomb_R = 144;
        //        int Red_Bomb_G = 56;
        //        int Red_Bomb_B = 61;

        //        int Blue_Bomb_R = 56;
        //        int Blue_Bomb_G = 113;
        //        int Blue_Bomb_B = 153;

        //        int Green_Bomb_R = 106;
        //        int Green_Bomb_G = 132;
        //        int Green_Bomb_B = 34;

        //        int LightBlue_Bomb_R = 83;
        //        int LightBlue_Bomb_G = 148;
        //        int LightBlue_Bomb_B = 166;

        //        int Purple_Bomb_R = 96;
        //        int Purple_Bomb_G = 49;
        //        int Purple_Bomb_B = 139;

        //        int Yellow_Bomb_R = 152;
        //        int Yellow_Bomb_G = 143;
        //        int Yellow_Bomb_B = 69;

        //        int Orange_Bomb_R = 173;
        //        int Orange_Bomb_G = 121;
        //        int Orange_Bomb_B = 71;

        //        bool RedBomb = (Ravg > (Red_Bomb_R - RGBdiff) && Ravg < (Red_Bomb_R + RGBdiff)) && (Gavg > (Red_Bomb_G - RGBdiff) && Gavg < (Red_Bomb_G + RGBdiff)) && (Bavg > (Red_Bomb_B - RGBdiff) && Bavg < (Red_Bomb_B + RGBdiff));
        //        bool BlueBomb = (Ravg > (Blue_Bomb_R - RGBdiff) && Ravg < (Blue_Bomb_R + RGBdiff)) && (Gavg > (Blue_Bomb_G - RGBdiff) && Gavg < (Blue_Bomb_G + RGBdiff)) && (Bavg > (Blue_Bomb_B - RGBdiff) && Bavg < (Blue_Bomb_B + RGBdiff));
        //        bool GreenBomb = (Ravg > (Green_Bomb_R - RGBdiff) && Ravg < (Green_Bomb_R + RGBdiff)) && (Gavg > (Green_Bomb_G - RGBdiff) && Gavg < (Green_Bomb_G + RGBdiff)) && (Bavg > (Green_Bomb_B - RGBdiff) && Bavg < (Green_Bomb_B + RGBdiff));
        //        bool LightBlueBomb = (Ravg > (LightBlue_Bomb_R - RGBdiff) && Ravg < (LightBlue_Bomb_R + RGBdiff)) && (Gavg > (LightBlue_Bomb_G - RGBdiff) && Gavg < (LightBlue_Bomb_G + RGBdiff)) && (Bavg > (LightBlue_Bomb_B - RGBdiff) && Bavg < (LightBlue_Bomb_B + RGBdiff));
        //        bool PurpleBomb = (Ravg > (Purple_Bomb_R - RGBdiff) && Ravg < (Purple_Bomb_R + RGBdiff)) && (Gavg > (Purple_Bomb_G - RGBdiff) && Gavg < (Purple_Bomb_G + RGBdiff)) && (Bavg > (Purple_Bomb_B - RGBdiff) && Bavg < (Purple_Bomb_B + RGBdiff));
        //        bool YellowBomb = (Ravg > (Yellow_Bomb_R - RGBdiff) && Ravg < (Yellow_Bomb_R + RGBdiff)) && (Gavg > (Yellow_Bomb_G - RGBdiff) && Gavg < (Yellow_Bomb_G + RGBdiff)) && (Bavg > (Yellow_Bomb_B - RGBdiff) && Bavg < (Yellow_Bomb_B + RGBdiff));
        //        bool OrangeBomb = (Ravg > (Orange_Bomb_R - RGBdiff) && Ravg < (Orange_Bomb_R + RGBdiff)) && (Gavg > (Orange_Bomb_G - RGBdiff) && Gavg < (Orange_Bomb_G + RGBdiff)) && (Bavg > (Orange_Bomb_B - RGBdiff) && Bavg < (Orange_Bomb_B + RGBdiff));

        //        ////Uber Bomb "9B8A89" 155 138 137
        //        ////Uber Bomb "82777C" 130 119 124
        //        ////928788 146 135 136
        //        ////919698 145 150 152
        //        ////948989 148 137 137
        //        ////928D8C 146 141 140
        //        ////7B737B 123 115 123

        //        int Uber_Bomb_R = 145;
        //        int Uber_Bomb_G = 135;
        //        int Uber_Bomb_B = 135;
        //        int BombDiff = 5;

        //        bool UberBomb = (Ravg > (Uber_Bomb_R - BombDiff) && Ravg < (Uber_Bomb_R + BombDiff)) && (Gavg > (Uber_Bomb_G - BombDiff) && Gavg < (Uber_Bomb_G + BombDiff)) && (Bavg > (Uber_Bomb_B - BombDiff) && Bavg < (Uber_Bomb_B + BombDiff));

        //        if (argDebuggingMode)
        //        {
        //            //Debugging
        //            String Rhex = Ravg.ToString("X");
        //            if (Rhex.Length == 1)
        //            {
        //                Rhex = "0" + Rhex;
        //            }
        //            String Ghex = Gavg.ToString("X");
        //            if (Ghex.Length == 1)
        //            {
        //                Ghex = "0" + Ghex;
        //            }
        //            String Bhex = Bavg.ToString("X");
        //            if (Bhex.Length == 1)
        //            {
        //                Bhex = "0" + Bhex;
        //            }

        //            String hexValue = Rhex + Ghex + Bhex;
        //            BioDebug[x][y] = hexValue;
        //        }

        //        if (Red || RedBomb || RedEye)
        //        {
        //            return BioColors.Red;
        //        }
        //        else if (Blue || BlueBomb || BlueEye)
        //        {
        //            return BioColors.Blue;
        //        }
        //        else if (Yellow || YellowBomb || YellowEye)
        //        {
        //            return BioColors.Yellow;
        //        }
        //        else if (Green || GreenBomb || GreenEye)
        //        {
        //            return BioColors.Green;
        //        }
        //        else if (LightBlue || LightBlueBomb || LightBlueEye)
        //        {
        //            return BioColors.LightBlue;
        //        }
        //        else if (Purple || PurpleBomb || PurpleEye)
        //        {
        //            return BioColors.Purple;
        //        }
        //        else if (Orange || OrangeBomb || OrangeEye)
        //        {
        //            return BioColors.Orange;
        //        }
        //        else if (UberBomb)
        //        {
        //            return BioColors.UberBomb;
        //        }
        //        else
        //        {
        //            return BioColors.UnknownColor;
        //        }
        //    }
        //    else
        //    {
        //        return BioColors.UnknownColor;
        //    }
        //}

        public BioColors DetectBioColorNew(Bitmap BioTemp, int x, int y)
        {
            if (IsStarted)
            {
                int Rsum = 0;
                int Gsum = 0;
                int Bsum = 0;

                int Ravg = 0;
                int Gavg = 0;
                int Bavg = 0;

                //Find Average Color
                int count = 0;

                //UberBomb
                for (int z = 7; z <= 21; z++)
                {
                    for (int w = 11; w <= 23; w++)
                    {

                        bool cond_dark = BioTemp.GetPixel(w, w).R < 50 && BioTemp.GetPixel(w, w).G < 50 && BioTemp.GetPixel(w, w).B < 50;
                        bool cond_light = BioTemp.GetPixel(w, w).R > 240 && BioTemp.GetPixel(w, w).G > 240 && BioTemp.GetPixel(w, w).B > 240;
                        if (!cond_dark || !cond_light)
                        {
                            Rsum += BioTemp.GetPixel(z, w).R;
                            Gsum += BioTemp.GetPixel(z, w).G;
                            Bsum += BioTemp.GetPixel(z, w).B;
                            count++;
                        }
                    }
                }
                Ravg = Convert.ToInt32(Convert.ToDouble(Rsum) / Convert.ToDouble(count));
                Gavg = Convert.ToInt32(Convert.ToDouble(Gsum) / Convert.ToDouble(count));
                Bavg = Convert.ToInt32(Convert.ToDouble(Bsum) / Convert.ToDouble(count));

                bool bUberBomb = false;

                for (int i = 0; i < UberBomb.Count; i++)
                {
                    bUberBomb = bUberBomb || ((Ravg > (UberBomb[i].R_MINUS_DIFF) && Ravg < (UberBomb[i].R_PLUS_DIFF)) && (Gavg > (UberBomb[i].G_MINUS_DIFF) && Gavg < (UberBomb[i].G_PLUS_DIFF)) && (Bavg > (UberBomb[i].B_MINUS_DIFF) && Bavg < (UberBomb[i].B_PLUS_DIFF)));
                }

                if (bUberBomb)
                {
                    return BioColors.UberBomb;
                }

                count = 0;
                Ravg = Gavg = Bavg = Rsum = Gsum = Bsum = 0;

                //Normal Biotronics
                //for (int z = 5; z <= 28; z++)
                //{
                //    for (int w = 5; w <= 8; w++)
                //    {
                //        bool cond_dark = BioTemp.GetPixel(w, w).R < 50 && BioTemp.GetPixel(w, w).G < 50 && BioTemp.GetPixel(w, w).B < 50;
                //        bool cond_light = BioTemp.GetPixel(w, w).R > 240 && BioTemp.GetPixel(w, w).G > 240 && BioTemp.GetPixel(w, w).B > 240;
                //        if (!cond_dark || !cond_light)
                //        {
                //            Rsum += BioTemp.GetPixel(z, w).R;
                //            Gsum += BioTemp.GetPixel(z, w).G;
                //            Bsum += BioTemp.GetPixel(z, w).B;
                //            count++;
                //        }
                //    }
                //    for (int w = 24; w <= 28; w++)
                //    {
                //        bool cond_dark = BioTemp.GetPixel(w, w).R < 50 && BioTemp.GetPixel(w, w).G < 50 && BioTemp.GetPixel(w, w).B < 50;
                //        bool cond_light = BioTemp.GetPixel(w, w).R > 240 && BioTemp.GetPixel(w, w).G > 240 && BioTemp.GetPixel(w, w).B > 240;
                //        if (!cond_dark || !cond_light)
                //        {
                //            Rsum += BioTemp.GetPixel(z, w).R;
                //            Gsum += BioTemp.GetPixel(z, w).G;
                //            Bsum += BioTemp.GetPixel(z, w).B;
                //            count++;
                //        }
                //    }
                //}
                //for (int z = 9; z <= 23; z++)
                //{
                //    for (int w = 5; w <= 8; w++)
                //    {
                //        bool cond_dark = BioTemp.GetPixel(w, w).R < 50 && BioTemp.GetPixel(w, w).G < 50 && BioTemp.GetPixel(w, w).B < 50;
                //        bool cond_light = BioTemp.GetPixel(w, w).R > 240 && BioTemp.GetPixel(w, w).G > 240 && BioTemp.GetPixel(w, w).B > 240;
                //        if (!cond_dark || !cond_light)
                //        {
                //            Rsum += BioTemp.GetPixel(w, z).R;
                //            Gsum += BioTemp.GetPixel(w, z).G;
                //            Bsum += BioTemp.GetPixel(w, z).B;
                //            count++;
                //        }
                //    }
                //    for (int w = 24; w <= 28; w++)
                //    {
                //        bool cond_dark = BioTemp.GetPixel(w, w).R < 50 && BioTemp.GetPixel(w, w).G < 50 && BioTemp.GetPixel(w, w).B < 50;
                //        bool cond_light = BioTemp.GetPixel(w, w).R > 240 && BioTemp.GetPixel(w, w).G > 240 && BioTemp.GetPixel(w, w).B > 240;
                //        if (!cond_dark || !cond_light)
                //        {
                //            Rsum += BioTemp.GetPixel(w, z).R;
                //            Gsum += BioTemp.GetPixel(w, z).G;
                //            Bsum += BioTemp.GetPixel(w, z).B;
                //            count++;
                //        }
                //    }
                //}

                for (int z = 5; z <= 28; z++)
                {
                    for (int w = 5; w <= 8; w++)
                    {
                        bool cond_dark = BioTemp.GetPixel(w, w).R < 50 && BioTemp.GetPixel(w, w).G < 50 && BioTemp.GetPixel(w, w).B < 50;
                        bool cond_light = BioTemp.GetPixel(w, w).R > 240 && BioTemp.GetPixel(w, w).G > 240 && BioTemp.GetPixel(w, w).B > 240;
                        if (!cond_dark || !cond_light)
                        {
                            Rsum += BioTemp.GetPixel(z, w).R;
                            Gsum += BioTemp.GetPixel(z, w).G;
                            Bsum += BioTemp.GetPixel(z, w).B;
                            count++;

                            if (z >= 9 && z <= 23)
                            {
                                Rsum += BioTemp.GetPixel(w, z).R;
                                Gsum += BioTemp.GetPixel(w, z).G;
                                Bsum += BioTemp.GetPixel(w, z).B;
                                count++;
                            }
                        }
                    }
                    for (int w = 24; w <= 28; w++)
                    {
                        bool cond_dark = BioTemp.GetPixel(w, w).R < 50 && BioTemp.GetPixel(w, w).G < 50 && BioTemp.GetPixel(w, w).B < 50;
                        bool cond_light = BioTemp.GetPixel(w, w).R > 240 && BioTemp.GetPixel(w, w).G > 240 && BioTemp.GetPixel(w, w).B > 240;
                        if (!cond_dark || !cond_light)
                        {
                            Rsum += BioTemp.GetPixel(z, w).R;
                            Gsum += BioTemp.GetPixel(z, w).G;
                            Bsum += BioTemp.GetPixel(z, w).B;
                            count++;

                            if (z >= 9 && z <= 23)
                            {
                                Rsum += BioTemp.GetPixel(w, z).R;
                                Gsum += BioTemp.GetPixel(w, z).G;
                                Bsum += BioTemp.GetPixel(w, z).B;
                                count++;
                            }
                        }
                    }
                }

                Ravg = Convert.ToInt32(Convert.ToDouble(Rsum) / Convert.ToDouble(count));
                Gavg = Convert.ToInt32(Convert.ToDouble(Gsum) / Convert.ToDouble(count));
                Bavg = Convert.ToInt32(Convert.ToDouble(Bsum) / Convert.ToDouble(count));

                //New Detection Algorithm
                bool bRed = false;
                bool bBlue = false;
                bool bGreen = false;
                bool bLightBlue = false;
                bool bPurple = false;
                bool bYellow = false;
                bool bOrange = false;

                bool bRedEye = false;
                bool bBlueEye = false;
                bool bGreenEye = false;
                bool bLightBlueEye = false;
                bool bPurpleEye = false;
                bool bYellowEye = false;
                bool bOrangeEye = false;

                bool bRedBomb = false;
                bool bBlueBomb = false;
                bool bGreenBomb = false;
                bool bLightBlueBomb = false;
                bool bPurpleBomb = false;
                bool bYellowBomb = false;
                bool bOrangeBomb = false;

                bool bRedEyeBomb = false;
                bool bBlueEyeBomb = false;
                bool bGreenEyeBomb = false;
                bool bLightBlueEyeBomb = false;
                bool bPurpleEyeBomb = false;
                bool bYellowEyeBomb = false;
                bool bOrangeEyeBomb = false;

                for (int i = 0; i < Red.Count; i++)
                {
                    bRed = bRed || ((Ravg > (Red[i].R_MINUS_DIFF) && Ravg < (Red[i].R_PLUS_DIFF)) && (Gavg > (Red[i].G_MINUS_DIFF) && Gavg < (Red[i].G_PLUS_DIFF)) && (Bavg > (Red[i].B_MINUS_DIFF) && Bavg < (Red[i].B_PLUS_DIFF)));
                }
                for (int i = 0; i < RedEye.Count; i++)
                {
                    bRedEye = bRedEye || ((Ravg > (RedEye[i].R_MINUS_DIFF) && Ravg < (RedEye[i].R_PLUS_DIFF)) && (Gavg > (RedEye[i].G_MINUS_DIFF) && Gavg < (RedEye[i].G_PLUS_DIFF)) && (Bavg > (RedEye[i].B_MINUS_DIFF) && Bavg < (RedEye[i].B_PLUS_DIFF)));
                }
                if (bRed || bRedEye)
                {
                    BioBombMatrix[x][y] = false;
                    return BioColors.Red;
                }

                for (int i = 0; i < RedEyeBomb.Count; i++)
                {
                    bRedEyeBomb = bRedEyeBomb || ((Ravg > (RedEyeBomb[i].R_MINUS_DIFF) && Ravg < (RedEyeBomb[i].R_PLUS_DIFF)) && (Gavg > (RedEyeBomb[i].G_MINUS_DIFF) && Gavg < (RedEyeBomb[i].G_PLUS_DIFF)) && (Bavg > (RedEyeBomb[i].B_MINUS_DIFF) && Bavg < (RedEyeBomb[i].B_PLUS_DIFF)));
                }

                for (int i = 0; i < RedBomb.Count; i++)
                {
                    bRedBomb = bRedBomb || ((Ravg > (RedBomb[i].R_MINUS_DIFF) && Ravg < (RedBomb[i].R_PLUS_DIFF)) && (Gavg > (RedBomb[i].G_MINUS_DIFF) && Gavg < (RedBomb[i].G_PLUS_DIFF)) && (Bavg > (RedBomb[i].B_MINUS_DIFF) && Bavg < (RedBomb[i].B_PLUS_DIFF)));
                }
                if (bRedBomb || bRedEyeBomb)
                {
                    BioBombMatrix[x][y] = true;
                    return BioColors.Red;
                }

                for (int i = 0; i < Blue.Count; i++)
                {
                    bBlue = bBlue || ((Ravg > (Blue[i].R_MINUS_DIFF) && Ravg < (Blue[i].R_PLUS_DIFF)) && (Gavg > (Blue[i].G_MINUS_DIFF) && Gavg < (Blue[i].G_PLUS_DIFF)) && (Bavg > (Blue[i].B_MINUS_DIFF) && Bavg < (Blue[i].B_PLUS_DIFF)));
                }
                for (int i = 0; i < BlueEye.Count; i++)
                {
                    bBlueEye = bBlueEye || ((Ravg > (BlueEye[i].R_MINUS_DIFF) && Ravg < (BlueEye[i].R_PLUS_DIFF)) && (Gavg > (BlueEye[i].G_MINUS_DIFF) && Gavg < (BlueEye[i].G_PLUS_DIFF)) && (Bavg > (BlueEye[i].B_MINUS_DIFF) && Bavg < (BlueEye[i].B_PLUS_DIFF)));
                }
                if (bBlue || bBlueEye)
                {
                    BioBombMatrix[x][y] = false;
                    return BioColors.Blue;
                }

                for (int i = 0; i < BlueBomb.Count; i++)
                {
                    bBlueBomb = bBlueBomb || ((Ravg > (BlueBomb[i].R_MINUS_DIFF) && Ravg < (BlueBomb[i].R_PLUS_DIFF)) && (Gavg > (BlueBomb[i].G_MINUS_DIFF) && Gavg < (BlueBomb[i].G_PLUS_DIFF)) && (Bavg > (BlueBomb[i].B_MINUS_DIFF) && Bavg < (BlueBomb[i].B_PLUS_DIFF)));
                }
                for (int i = 0; i < BlueEyeBomb.Count; i++)
                {
                    bBlueEyeBomb = bBlueEyeBomb || ((Ravg > (BlueEyeBomb[i].R_MINUS_DIFF) && Ravg < (BlueEyeBomb[i].R_PLUS_DIFF)) && (Gavg > (BlueEyeBomb[i].G_MINUS_DIFF) && Gavg < (BlueEyeBomb[i].G_PLUS_DIFF)) && (Bavg > (BlueEyeBomb[i].B_MINUS_DIFF) && Bavg < (BlueEyeBomb[i].B_PLUS_DIFF)));
                }
                if (bBlueBomb || bBlueEyeBomb)
                {
                    BioBombMatrix[x][y] = true;
                    return BioColors.Blue;
                }

                for (int i = 0; i < Green.Count; i++)
                {
                    bGreen = bGreen || ((Ravg > (Green[i].R_MINUS_DIFF) && Ravg < (Green[i].R_PLUS_DIFF)) && (Gavg > (Green[i].G_MINUS_DIFF) && Gavg < (Green[i].G_PLUS_DIFF)) && (Bavg > (Green[i].B_MINUS_DIFF) && Bavg < (Green[i].B_PLUS_DIFF)));
                }
                for (int i = 0; i < GreenEye.Count; i++)
                {
                    bGreenEye = bGreenEye || ((Ravg > (GreenEye[i].R_MINUS_DIFF) && Ravg < (GreenEye[i].R_PLUS_DIFF)) && (Gavg > (GreenEye[i].G_MINUS_DIFF) && Gavg < (GreenEye[i].G_PLUS_DIFF)) && (Bavg > (GreenEye[i].B_MINUS_DIFF) && Bavg < (GreenEye[i].B_PLUS_DIFF)));
                }
                if (bGreen || bGreenEye)
                {
                    BioBombMatrix[x][y] = false;
                    return BioColors.Green;
                }

                for (int i = 0; i < GreenBomb.Count; i++)
                {
                    bGreenBomb = bGreenBomb || ((Ravg > (GreenBomb[i].R_MINUS_DIFF) && Ravg < (GreenBomb[i].R_PLUS_DIFF)) && (Gavg > (GreenBomb[i].G_MINUS_DIFF) && Gavg < (GreenBomb[i].G_PLUS_DIFF)) && (Bavg > (GreenBomb[i].B_MINUS_DIFF) && Bavg < (GreenBomb[i].B_PLUS_DIFF)));
                }
                for (int i = 0; i < GreenEyeBomb.Count; i++)
                {
                    bGreenEyeBomb = bGreenEyeBomb || ((Ravg > (GreenEyeBomb[i].R_MINUS_DIFF) && Ravg < (GreenEyeBomb[i].R_PLUS_DIFF)) && (Gavg > (GreenEyeBomb[i].G_MINUS_DIFF) && Gavg < (GreenEyeBomb[i].G_PLUS_DIFF)) && (Bavg > (GreenEyeBomb[i].B_MINUS_DIFF) && Bavg < (GreenEyeBomb[i].B_PLUS_DIFF)));
                }
                if (bGreenBomb || bGreenEyeBomb)
                {
                    BioBombMatrix[x][y] = true;
                    return BioColors.Green;
                }

                for (int i = 0; i < LightBlue.Count; i++)
                {
                    bLightBlue = bLightBlue || ((Ravg > (LightBlue[i].R_MINUS_DIFF) && Ravg < (LightBlue[i].R_PLUS_DIFF)) && (Gavg > (LightBlue[i].G_MINUS_DIFF) && Gavg < (LightBlue[i].G_PLUS_DIFF)) && (Bavg > (LightBlue[i].B_MINUS_DIFF) && Bavg < (LightBlue[i].B_PLUS_DIFF)));
                }
                for (int i = 0; i < LightBlueEye.Count; i++)
                {
                    bLightBlueEye = bLightBlueEye || ((Ravg > (LightBlueEye[i].R_MINUS_DIFF) && Ravg < (LightBlueEye[i].R_PLUS_DIFF)) && (Gavg > (LightBlueEye[i].G_MINUS_DIFF) && Gavg < (LightBlueEye[i].G_PLUS_DIFF)) && (Bavg > (LightBlueEye[i].B_MINUS_DIFF) && Bavg < (LightBlueEye[i].B_PLUS_DIFF)));
                }
                if (bLightBlue || bLightBlueEye)
                {
                    BioBombMatrix[x][y] = false;
                    return BioColors.LightBlue;
                }

                for (int i = 0; i < LightBlueBomb.Count; i++)
                {
                    bLightBlueBomb = bLightBlueBomb || ((Ravg > (LightBlueBomb[i].R_MINUS_DIFF) && Ravg < (LightBlueBomb[i].R_PLUS_DIFF)) && (Gavg > (LightBlueBomb[i].G_MINUS_DIFF) && Gavg < (LightBlueBomb[i].G_PLUS_DIFF)) && (Bavg > (LightBlueBomb[i].B_MINUS_DIFF) && Bavg < (LightBlueBomb[i].B_PLUS_DIFF)));
                }
                for (int i = 0; i < LightBlueEyeBomb.Count; i++)
                {
                    bLightBlueEyeBomb = bLightBlueEyeBomb || ((Ravg > (LightBlueEyeBomb[i].R_MINUS_DIFF) && Ravg < (LightBlueEyeBomb[i].R_PLUS_DIFF)) && (Gavg > (LightBlueEyeBomb[i].G_MINUS_DIFF) && Gavg < (LightBlueEyeBomb[i].G_PLUS_DIFF)) && (Bavg > (LightBlueEyeBomb[i].B_MINUS_DIFF) && Bavg < (LightBlueEyeBomb[i].B_PLUS_DIFF)));
                }
                if (bLightBlueBomb || bLightBlueEyeBomb)
                {
                    BioBombMatrix[x][y] = true;
                    return BioColors.LightBlue;
                }

                for (int i = 0; i < Purple.Count; i++)
                {
                    bPurple = bPurple || ((Ravg > (Purple[i].R_MINUS_DIFF) && Ravg < (Purple[i].R_PLUS_DIFF)) && (Gavg > (Purple[i].G_MINUS_DIFF) && Gavg < (Purple[i].G_PLUS_DIFF)) && (Bavg > (Purple[i].B_MINUS_DIFF) && Bavg < (Purple[i].B_PLUS_DIFF)));
                }
                for (int i = 0; i < PurpleEye.Count; i++)
                {
                    bPurpleEye = bPurpleEye || ((Ravg > (PurpleEye[i].R_MINUS_DIFF) && Ravg < (PurpleEye[i].R_PLUS_DIFF)) && (Gavg > (PurpleEye[i].G_MINUS_DIFF) && Gavg < (PurpleEye[i].G_PLUS_DIFF)) && (Bavg > (PurpleEye[i].B_MINUS_DIFF) && Bavg < (PurpleEye[i].B_PLUS_DIFF)));
                }
                if (bPurple || bPurpleEye)
                {
                    BioBombMatrix[x][y] = false;
                    return BioColors.Purple;
                }

                for (int i = 0; i < PurpleBomb.Count; i++)
                {
                    bPurpleBomb = bPurpleBomb || ((Ravg > (PurpleBomb[i].R_MINUS_DIFF) && Ravg < (PurpleBomb[i].R_PLUS_DIFF)) && (Gavg > (PurpleBomb[i].G_MINUS_DIFF) && Gavg < (PurpleBomb[i].G_PLUS_DIFF)) && (Bavg > (PurpleBomb[i].B_MINUS_DIFF) && Bavg < (PurpleBomb[i].B_PLUS_DIFF)));
                }
                for (int i = 0; i < PurpleEyeBomb.Count; i++)
                {
                    bPurpleEyeBomb = bPurpleEyeBomb || ((Ravg > (PurpleEyeBomb[i].R_MINUS_DIFF) && Ravg < (PurpleEyeBomb[i].R_PLUS_DIFF)) && (Gavg > (PurpleEyeBomb[i].G_MINUS_DIFF) && Gavg < (PurpleEyeBomb[i].G_PLUS_DIFF)) && (Bavg > (PurpleEyeBomb[i].B_MINUS_DIFF) && Bavg < (PurpleEyeBomb[i].B_PLUS_DIFF)));
                }
                if (bPurpleBomb || bPurpleEyeBomb)
                {
                    BioBombMatrix[x][y] = true;
                    return BioColors.Purple;
                }

                for (int i = 0; i < Yellow.Count; i++)
                {
                    bYellow = bYellow || ((Ravg > (Yellow[i].R_MINUS_DIFF) && Ravg < (Yellow[i].R_PLUS_DIFF)) && (Gavg > (Yellow[i].G_MINUS_DIFF) && Gavg < (Yellow[i].G_PLUS_DIFF)) && (Bavg > (Yellow[i].B_MINUS_DIFF) && Bavg < (Yellow[i].B_PLUS_DIFF)));
                }
                for (int i = 0; i < YellowEye.Count; i++)
                {
                    bYellowEye = bYellowEye || ((Ravg > (YellowEye[i].R_MINUS_DIFF) && Ravg < (YellowEye[i].R_PLUS_DIFF)) && (Gavg > (YellowEye[i].G_MINUS_DIFF) && Gavg < (YellowEye[i].G_PLUS_DIFF)) && (Bavg > (YellowEye[i].B_MINUS_DIFF) && Bavg < (YellowEye[i].B_PLUS_DIFF)));
                }
                if (bYellow || bYellowEye)
                {
                    BioBombMatrix[x][y] = false;
                    return BioColors.Yellow;
                }

                for (int i = 0; i < YellowBomb.Count; i++)
                {
                    bYellowBomb = bYellowBomb || ((Ravg > (YellowBomb[i].R_MINUS_DIFF) && Ravg < (YellowBomb[i].R_PLUS_DIFF)) && (Gavg > (YellowBomb[i].G_MINUS_DIFF) && Gavg < (YellowBomb[i].G_PLUS_DIFF)) && (Bavg > (YellowBomb[i].B_MINUS_DIFF) && Bavg < (YellowBomb[i].B_PLUS_DIFF)));
                }
                for (int i = 0; i < YellowEyeBomb.Count; i++)
                {
                    bYellowEyeBomb = bYellowEyeBomb || ((Ravg > (YellowEyeBomb[i].R_MINUS_DIFF) && Ravg < (YellowEyeBomb[i].R_PLUS_DIFF)) && (Gavg > (YellowEyeBomb[i].G_MINUS_DIFF) && Gavg < (YellowEyeBomb[i].G_PLUS_DIFF)) && (Bavg > (YellowEyeBomb[i].B_MINUS_DIFF) && Bavg < (YellowEyeBomb[i].B_PLUS_DIFF)));
                }
                if (bYellowBomb || bYellowEyeBomb)
                {
                    BioBombMatrix[x][y] = true;
                    return BioColors.Yellow;
                }

                for (int i = 0; i < Orange.Count; i++)
                {
                    bOrange = bOrange || ((Ravg > (Orange[i].R_MINUS_DIFF) && Ravg < (Orange[i].R_PLUS_DIFF)) && (Gavg > (Orange[i].G_MINUS_DIFF) && Gavg < (Orange[i].G_PLUS_DIFF)) && (Bavg > (Orange[i].B_MINUS_DIFF) && Bavg < (Orange[i].B_PLUS_DIFF)));
                }
                for (int i = 0; i < OrangeEye.Count; i++)
                {
                    bOrangeEye = bOrangeEye || ((Ravg > (OrangeEye[i].R_MINUS_DIFF) && Ravg < (OrangeEye[i].R_PLUS_DIFF)) && (Gavg > (OrangeEye[i].G_MINUS_DIFF) && Gavg < (OrangeEye[i].G_PLUS_DIFF)) && (Bavg > (OrangeEye[i].B_MINUS_DIFF) && Bavg < (OrangeEye[i].B_PLUS_DIFF)));
                }
                if (bOrange || bOrangeEye)
                {
                    BioBombMatrix[x][y] = false;
                    return BioColors.Orange;
                }

                for (int i = 0; i < OrangeBomb.Count; i++)
                {
                    bOrangeBomb = bOrangeBomb || ((Ravg > (OrangeBomb[i].R_MINUS_DIFF) && Ravg < (OrangeBomb[i].R_PLUS_DIFF)) && (Gavg > (OrangeBomb[i].G_MINUS_DIFF) && Gavg < (OrangeBomb[i].G_PLUS_DIFF)) && (Bavg > (OrangeBomb[i].B_MINUS_DIFF) && Bavg < (OrangeBomb[i].B_PLUS_DIFF)));
                }
                for (int i = 0; i < OrangeEyeBomb.Count; i++)
                {
                    bOrangeEyeBomb = bOrangeEyeBomb || ((Ravg > (OrangeEyeBomb[i].R_MINUS_DIFF) && Ravg < (OrangeEyeBomb[i].R_PLUS_DIFF)) && (Gavg > (OrangeEyeBomb[i].G_MINUS_DIFF) && Gavg < (OrangeEyeBomb[i].G_PLUS_DIFF)) && (Bavg > (OrangeEyeBomb[i].B_MINUS_DIFF) && Bavg < (OrangeEyeBomb[i].B_PLUS_DIFF)));
                }
                if (bOrangeBomb || bOrangeEyeBomb)
                {
                    BioBombMatrix[x][y] = true;
                    return BioColors.Orange;
                }

                BioBombMatrix[x][y] = false;
                CountUnknown++;
                return BioColors.UnknownColor;
            }
            else
            {
                BioBombMatrix[x][y] = false;
                CountUnknown++;
                return BioColors.UnknownColor;
            }
        }

        public List<BioMove> DetectAvailableMoves(BioColors[][] argBioMatrix, bool argFullLog)
        {
            List<BioMove> BioMoves = new List<BioMove>();
            if (IsStarted)
            {
                for (int column = 0; column < BioMatrixWidth; column++)
                {
                    for (int row = 0; row < BioMatrixHeight; row++)
                    {
                        if (argBioMatrix[column][row] != BioColors.UnknownColor && argBioMatrix[column][row] != BioColors.UberBomb)
                        {
                            BioColors current = argBioMatrix[column][row];
                            bool cond1;
                            bool cond2;
                            bool cond3;
                            bool cond4;
                            //Move 1 - Up
                            if (column > 0) //Check Boundaries
                            {
                                //Check above row - left
                                if (row - 2 >= 0)
                                {
                                    cond1 = argBioMatrix[column - 1][row - 1] == current && argBioMatrix[column - 1][row - 2] == current;
                                }
                                else
                                {
                                    cond1 = false;
                                }
                                //Check above row - right
                                if (row + 2 < BioMatrixHeight)
                                {
                                    cond2 = argBioMatrix[column - 1][row + 1] == current && argBioMatrix[column - 1][row + 2] == current;
                                }
                                else
                                {
                                    cond2 = false;
                                }
                                //Check above row - center
                                if (row - 1 > 0 && row + 1 < BioMatrixHeight)
                                {
                                    cond4 = argBioMatrix[column - 1][row - 1] == current && argBioMatrix[column - 1][row + 1] == current;
                                }
                                else
                                {
                                    cond4 = false;
                                }
                                //Check above column
                                if (column - 3 >= 0)
                                {
                                    cond3 = argBioMatrix[column - 2][row] == current && argBioMatrix[column - 3][row] == current;
                                }
                                else
                                {
                                    cond3 = false;
                                }
                                if (cond1 || cond2 || cond3 || cond4)
                                {
                                    BioMove mov_tmp = new BioMove();

                                    BioBlock block_tmp = new BioBlock();
                                    block_tmp.Row = column;
                                    block_tmp.Column = row;
                                    mov_tmp.StartBlock = block_tmp;

                                    block_tmp = new BioBlock();
                                    block_tmp.Row = column - 1;
                                    block_tmp.Column = row;
                                    mov_tmp.EndBlock = block_tmp;

                                    mov_tmp.IsUberBomb = false;
                                    mov_tmp.HasBomb = BioBombMatrix[column][row];
                                    mov_tmp.TotalBlocks = 1;

                                    if (cond1)//Check above row - left
                                    {
                                        for (int w = row - 1; w >= 0; w--)
                                        {
                                            if (argBioMatrix[column - 1][w] == current)
                                            {
                                                mov_tmp.TotalBlocks++;
                                                if (BioBombMatrix[column - 1][w])
                                                {
                                                    mov_tmp.HasBomb = true;
                                                }
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    if (cond2)//Check above row - right
                                    {
                                        for (int w = row + 1; w < BioMatrixHeight; w++)
                                        {
                                            if (argBioMatrix[column - 1][w] == current)
                                            {
                                                mov_tmp.TotalBlocks++;
                                                if (BioBombMatrix[column - 1][w])
                                                {
                                                    mov_tmp.HasBomb = true;
                                                }
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    if (cond3)//Check above row - center
                                    {
                                        for (int w = column - 2; w >= 0; w--)
                                        {
                                            if (argBioMatrix[w][row] == current)
                                            {
                                                mov_tmp.TotalBlocks++;
                                                if (BioBombMatrix[w][row])
                                                {
                                                    mov_tmp.HasBomb = true;
                                                }
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    if (cond4)//Check above column
                                    {
                                        for (int w = row - 1; w >= 0; w--)
                                        {
                                            if (argBioMatrix[column - 1][w] == current)
                                            {
                                                mov_tmp.TotalBlocks++;
                                                if (BioBombMatrix[column - 1][w])
                                                {
                                                    mov_tmp.HasBomb = true;
                                                }
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }
                                        for (int w = row + 1; w < BioMatrixHeight; w++)
                                        {
                                            if (argBioMatrix[column - 1][w] == current)
                                            {
                                                mov_tmp.TotalBlocks++;
                                                if (BioBombMatrix[column - 1][w])
                                                {
                                                    mov_tmp.HasBomb = true;
                                                }
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    BioMoves.Add(mov_tmp);
                                }
                            }
                            //Move 2 - Down
                            if (column < (BioMatrixWidth - 1)) //Check Boundaries
                            {
                                //Check below row - left
                                if (row - 2 >= 0)
                                {
                                    cond1 = argBioMatrix[column + 1][row - 1] == current && argBioMatrix[column + 1][row - 2] == current;
                                }
                                else
                                {
                                    cond1 = false;
                                }
                                //Check below row - right
                                if (row + 2 < BioMatrixHeight)
                                {
                                    cond2 = argBioMatrix[column + 1][row + 1] == current && argBioMatrix[column + 1][row + 2] == current;
                                }
                                else
                                {
                                    cond2 = false;
                                }
                                //Check below row - center
                                if (row - 1 >= 0 && row + 1 < BioMatrixHeight)
                                {
                                    cond4 = argBioMatrix[column + 1][row - 1] == current && argBioMatrix[column + 1][row + 1] == current;
                                }
                                else
                                {
                                    cond4 = false;
                                }
                                //Check below column
                                if (column + 3 < BioMatrixWidth)
                                {
                                    cond3 = argBioMatrix[column + 2][row] == current && argBioMatrix[column + 3][row] == current;
                                }
                                else
                                {
                                    cond3 = false;
                                }
                                if (cond1 || cond2 || cond3 || cond4)
                                {
                                    BioMove mov_tmp = new BioMove();

                                    BioBlock block_tmp = new BioBlock();
                                    block_tmp.Row = column;
                                    block_tmp.Column = row;
                                    mov_tmp.StartBlock = block_tmp;

                                    block_tmp = new BioBlock();
                                    block_tmp.Row = column + 1;
                                    block_tmp.Column = row;
                                    mov_tmp.EndBlock = block_tmp;

                                    mov_tmp.IsUberBomb = false;
                                    mov_tmp.HasBomb = BioBombMatrix[column][row];
                                    mov_tmp.TotalBlocks = 1;

                                    if (cond1)
                                    {
                                        for (int w = row - 1; w >= 0; w--)
                                        {
                                            if (argBioMatrix[column + 1][w] == current)
                                            {
                                                mov_tmp.TotalBlocks++;
                                                if (BioBombMatrix[column + 1][w])
                                                {
                                                    mov_tmp.HasBomb = true;
                                                }
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    if (cond2)
                                    {
                                        for (int w = row + 1; w < BioMatrixHeight; w++)
                                        {
                                            if (argBioMatrix[column + 1][w] == current)
                                            {
                                                mov_tmp.TotalBlocks++;
                                                if (BioBombMatrix[column + 1][w])
                                                {
                                                    mov_tmp.HasBomb = true;
                                                }
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    if (cond3)
                                    {
                                        for (int w = column + 2; w < BioMatrixWidth; w++)
                                        {
                                            if (argBioMatrix[w][row] == current)
                                            {
                                                mov_tmp.TotalBlocks++;
                                                if (BioBombMatrix[w][row])
                                                {
                                                    mov_tmp.HasBomb = true;
                                                }
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    if (cond4)
                                    {
                                        for (int w = row - 1; w >= 0; w--)
                                        {
                                            if (argBioMatrix[column + 1][w] == current)
                                            {
                                                mov_tmp.TotalBlocks++;
                                                if (BioBombMatrix[column + 1][w])
                                                {
                                                    mov_tmp.HasBomb = true;
                                                }
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }
                                        for (int w = row + 1; w < BioMatrixHeight; w++)
                                        {
                                            if (argBioMatrix[column + 1][w] == current)
                                            {
                                                mov_tmp.TotalBlocks++;
                                                if (BioBombMatrix[column + 1][w])
                                                {
                                                    mov_tmp.HasBomb = true;
                                                }
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    BioMoves.Add(mov_tmp);
                                }
                            }
                            //Move 3 - Left
                            if (row > 0) //Check Boundaries
                            {
                                //Check left column - up
                                if (column - 2 >= 0)
                                {
                                    cond1 = argBioMatrix[column - 1][row - 1] == current && argBioMatrix[column - 2][row - 1] == current;
                                }
                                else
                                {
                                    cond1 = false;
                                }
                                //Check left column - down
                                if (column + 2 < BioMatrixWidth)
                                {
                                    cond2 = argBioMatrix[column + 1][row - 1] == current && argBioMatrix[column + 2][row - 1] == current;
                                }
                                else
                                {
                                    cond2 = false;
                                }
                                //Check left column - center
                                if (column - 1 >= 0 && column + 1 < BioMatrixWidth)
                                {
                                    cond4 = argBioMatrix[column - 1][row - 1] == current && argBioMatrix[column + 1][row - 1] == current;
                                }
                                else
                                {
                                    cond4 = false;
                                }
                                //Check left row
                                if (row - 3 >= 0)
                                {
                                    cond3 = argBioMatrix[column][row - 2] == current && argBioMatrix[column][row - 3] == current;
                                }
                                else
                                {
                                    cond3 = false;
                                }
                                if (cond1 || cond2 || cond3 || cond4)
                                {
                                    BioMove mov_tmp = new BioMove();

                                    BioBlock block_tmp = new BioBlock();
                                    block_tmp.Row = column;
                                    block_tmp.Column = row;
                                    mov_tmp.StartBlock = block_tmp;

                                    block_tmp = new BioBlock();
                                    block_tmp.Row = column;
                                    block_tmp.Column = row - 1;
                                    mov_tmp.EndBlock = block_tmp;

                                    mov_tmp.IsUberBomb = false;
                                    mov_tmp.HasBomb = BioBombMatrix[column][row];
                                    mov_tmp.TotalBlocks = 1;

                                    if (cond1)
                                    {
                                        for (int w = column - 1; w >= 0; w--)
                                        {
                                            if (argBioMatrix[w][row - 1] == current)
                                            {
                                                mov_tmp.TotalBlocks++;
                                                if (BioBombMatrix[w][row - 1])
                                                {
                                                    mov_tmp.HasBomb = true;
                                                }
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    if (cond2)
                                    {
                                        for (int w = column + 1; w < BioMatrixWidth; w++)
                                        {
                                            if (argBioMatrix[w][row - 1] == current)
                                            {
                                                mov_tmp.TotalBlocks++;
                                                if (BioBombMatrix[w][row - 1])
                                                {
                                                    mov_tmp.HasBomb = true;
                                                }
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    if (cond3)
                                    {
                                        for (int w = row - 2; w >= 0; w--)
                                        {
                                            if (argBioMatrix[column][w] == current)
                                            {
                                                mov_tmp.TotalBlocks++;
                                                if (BioBombMatrix[column][w])
                                                {
                                                    mov_tmp.HasBomb = true;
                                                }
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    if (cond4)
                                    {
                                        for (int w = column - 1; w >= 0; w--)
                                        {
                                            if (argBioMatrix[w][row - 1] == current)
                                            {
                                                mov_tmp.TotalBlocks++;
                                                if (BioBombMatrix[w][row - 1])
                                                {
                                                    mov_tmp.HasBomb = true;
                                                }
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }
                                        for (int w = column + 1; w < BioMatrixWidth; w++)
                                        {
                                            if (argBioMatrix[w][row - 1] == current)
                                            {
                                                mov_tmp.TotalBlocks++;
                                                if (BioBombMatrix[w][row - 1])
                                                {
                                                    mov_tmp.HasBomb = true;
                                                }
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }
                                    }

                                    BioMoves.Add(mov_tmp);
                                }
                            }
                            //Move 4 - Right
                            if (row < (BioMatrixHeight - 1)) //Check Boundaries
                            {
                                //Check right column - up
                                if (column - 2 >= 0)
                                {
                                    cond1 = argBioMatrix[column - 1][row + 1] == current && argBioMatrix[column - 2][row + 1] == current;
                                }
                                else
                                {
                                    cond1 = false;
                                }
                                //Check right column - down
                                if (column + 2 < BioMatrixWidth)
                                {
                                    cond2 = argBioMatrix[column + 1][row + 1] == current && argBioMatrix[column + 2][row + 1] == current;
                                }
                                else
                                {
                                    cond2 = false;
                                }
                                //Check right column - center
                                if (column - 1 >= 0 && column + 1 < BioMatrixWidth)
                                {
                                    cond4 = argBioMatrix[column - 1][row + 1] == current && argBioMatrix[column + 1][row + 1] == current;
                                }
                                else
                                {
                                    cond4 = false;
                                }
                                //Check right row
                                if (row + 3 < BioMatrixHeight)
                                {
                                    cond3 = argBioMatrix[column][row + 2] == current && argBioMatrix[column][row + 3] == current;
                                }
                                else
                                {
                                    cond3 = false;
                                }
                                if (cond1 || cond2 || cond3 || cond4)
                                {
                                    BioMove mov_tmp = new BioMove();

                                    BioBlock block_tmp = new BioBlock();
                                    block_tmp.Row = column;
                                    block_tmp.Column = row;
                                    mov_tmp.StartBlock = block_tmp;

                                    block_tmp = new BioBlock();
                                    block_tmp.Row = column;
                                    block_tmp.Column = row + 1;
                                    mov_tmp.EndBlock = block_tmp;

                                    mov_tmp.IsUberBomb = false;
                                    mov_tmp.HasBomb = BioBombMatrix[column][row];
                                    mov_tmp.TotalBlocks = 1;

                                    if (cond1)
                                    {
                                        for (int w = column - 1; w >= 0; w--)
                                        {
                                            if (argBioMatrix[w][row + 1] == current)
                                            {
                                                mov_tmp.TotalBlocks++;
                                                if (BioBombMatrix[w][row + 1])
                                                {
                                                    mov_tmp.HasBomb = true;
                                                }
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    if (cond2)
                                    {
                                        for (int w = column + 1; w < BioMatrixWidth; w++)
                                        {
                                            if (argBioMatrix[w][row + 1] == current)
                                            {
                                                mov_tmp.TotalBlocks++;
                                                if (BioBombMatrix[w][row + 1])
                                                {
                                                    mov_tmp.HasBomb = true;
                                                }
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    if (cond3)
                                    {
                                        for (int w = row + 2; w < BioMatrixHeight; w++)
                                        {
                                            if (argBioMatrix[column][w] == current)
                                            {
                                                mov_tmp.TotalBlocks++;
                                                if (BioBombMatrix[column][w])
                                                {
                                                    mov_tmp.HasBomb = true;
                                                }
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    if (cond4)
                                    {
                                        for (int w = column - 1; w >= 0; w--)
                                        {
                                            if (argBioMatrix[w][row + 1] == current)
                                            {
                                                mov_tmp.TotalBlocks++;
                                                if (BioBombMatrix[w][row + 1])
                                                {
                                                    mov_tmp.HasBomb = true;
                                                }
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }
                                        for (int w = column + 1; w < BioMatrixWidth; w++)
                                        {
                                            if (argBioMatrix[w][row + 1] == current)
                                            {
                                                mov_tmp.TotalBlocks++;
                                                if (BioBombMatrix[w][row + 1])
                                                {
                                                    mov_tmp.HasBomb = true;
                                                }
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }
                                    }

                                    BioMoves.Add(mov_tmp);
                                }
                            }
                        }
                        else if (argBioMatrix[column][row] == BioColors.UberBomb)
                        {
                            int[] mov_all = new int[4];
                            if (column - 1 >= 0)
                            {
                                mov_all[0] = CountKnownColor(argBioMatrix[column - 1][row]);
                            }
                            else
                            {
                                mov_all[0] = 0;
                            }
                            if (column + 1 < BioMatrixWidth - 1)
                            {
                                mov_all[1] = CountKnownColor(argBioMatrix[column + 1][row]);
                            }
                            else
                            {
                                mov_all[1] = 0;
                            }
                            if (row - 1 >= 0)
                            {
                                mov_all[2] = CountKnownColor(argBioMatrix[column][row - 1]);
                            }
                            else
                            {
                                mov_all[2] = 0;
                            }
                            if (row + 1 < BioMatrixHeight - 1)
                            {
                                mov_all[3] = CountKnownColor(argBioMatrix[column][row + 1]);
                            }
                            else
                            {
                                mov_all[3] = 0;
                            }
                            int max_idx = 0;
                            for (int w = 1; w < 4; w++)
                            {
                                if (mov_all[w] > mov_all[max_idx])
                                {
                                    max_idx = w;
                                }
                            }
                            int final_x = 0;
                            int final_y = 0;
                            switch (max_idx)
                            {
                                case 0:
                                    final_x = column - 1;
                                    final_y = row;
                                    break;
                                case 1:
                                    final_x = column + 1;
                                    final_y = row;
                                    break;
                                case 2:
                                    final_x = column;
                                    final_y = row - 1;
                                    break;
                                case 3:
                                    final_x = column;
                                    final_y = row + 1;
                                    break;
                            }

                            BioMove mov_tmp = new BioMove();

                            BioBlock block_tmp = new BioBlock();
                            block_tmp.Row = column;
                            block_tmp.Column = row;
                            mov_tmp.StartBlock = block_tmp;

                            block_tmp = new BioBlock();
                            block_tmp.Row = final_x;
                            block_tmp.Column = final_y;
                            mov_tmp.EndBlock = block_tmp;

                            mov_tmp.IsUberBomb = true;
                            mov_tmp.HasBomb = false;
                            mov_tmp.TotalBlocks = mov_all[max_idx];

                            BioMoves.Add(mov_tmp);
                        }
                    }
                }

                //Check Full Log
                if (argFullLog)
                {
                    StringBuilder builder = new StringBuilder();
                    for (int i = 0; i < BioMoves.Count; i++)
                    {
                        BioMove mov = BioMoves[i];
                        builder.AppendFormat("Detected Start Block : {0} {1} {2}", mov.StartBlock.Row, mov.StartBlock.Column, argBioMatrix[mov.StartBlock.Row][mov.StartBlock.Column]);
                        builder.AppendFormat("Detected End Block : {0} {1} {2}", mov.EndBlock.Row, mov.EndBlock.Column, argBioMatrix[mov.EndBlock.Row][mov.EndBlock.Column]);
                        builder.AppendFormat("Total move blocks : {0}", mov.TotalBlocks);
                    }
                    gLog.WriteLine(builder.ToString());
                }
            }
            return BioMoves;
        }

        public BioMove SelectMove(List<BioMove> argBioMoves)
        {
            List<int> maxCombos = new List<int>();
            List<int> maxBlocks = new List<int>();
            List<int> Combos = new List<int>();
            List<int> Blocks = new List<int>();

            //Prefer Create UberBomb
            if (argBioMoves[0].TotalBlocks >= 5)
            {
                return argBioMoves[0];
            }
            //Prefer UberBomb
            if (argBioMoves[0].IsUberBomb)
            {
                return argBioMoves[0];
            }

            //Find max_combo moves
            int maxCombo = 0;
            argBioMoves[maxCombo].TotalCombos = DetectCombos(argBioMoves[maxCombo]);
            Combos.Add(argBioMoves[maxCombo].TotalCombos);

            //Find the move with max_combos
            for (int i = 1; i < argBioMoves.Count; i++)
            {
                //Prefer Create UberBomb
                if (argBioMoves[i].TotalBlocks >= 5)
                {
                    return argBioMoves[i];
                }
                //Prefer UberBomb
                if (argBioMoves[i].IsUberBomb)
                {
                    return argBioMoves[i];
                }

                argBioMoves[i].TotalCombos = DetectCombos(argBioMoves[i]);
                Combos.Add(argBioMoves[i].TotalCombos);
                
                if (Combos[i] > Combos[maxCombo])
                {
                    maxCombo = i;
                }
            }

            //Find all the moves with max_combos
            for (int i = 0; i < argBioMoves.Count; i++)
            {
                if (Combos[i] == Combos[maxCombo])
                {
                    maxCombos.Add(i);
                }
            }
            int mcombos = Combos[maxCombo];

            int maxBlock = 0;
            for (int i = 0; i < argBioMoves.Count; i++)
            {
                Blocks.Add(argBioMoves[i].TotalBlocks);
            }
            //Prefer Maximum blocks from moves with maximum combos
            //Find the move with max blocks
            for (int i = 1; i < maxCombos.Count; i++)
            {
                Blocks.Add(argBioMoves[maxCombos[i]].TotalBlocks);
                if (Blocks[maxCombos[i]] > Blocks[maxCombos[maxBlock]])
                {
                    maxBlock = i;
                }
            }
            //Find all the moves with max_blocks
            for (int i = 0; i < maxCombos.Count; i++)
            {
                if (Blocks[maxCombos[i]] == Blocks[maxCombos[maxBlock]])
                {
                    maxBlocks.Add(i);
                }
            }
            int maxDown = 0;

            //Prefer Down moves
            for (int i = 1; i < maxBlocks.Count; i++)
            {
                if (argBioMoves[maxCombos[maxBlocks[i]]].StartBlock.Column > argBioMoves[maxCombos[maxBlocks[maxDown]]].StartBlock.Column || 
                    argBioMoves[maxCombos[maxBlocks[i]]].EndBlock.Column > argBioMoves[maxCombos[maxBlocks[maxDown]]].EndBlock.Column)
                {
                    maxDown = i;
                }
            }
            BioMove selMove = argBioMoves[maxCombos[maxBlocks[maxDown]]];

            return selMove;

            //int max_idx = 0;

            //for (int i = 1; i < BioMoves.Count; i++)
            //{
            //    //Prefer UberBomb
            //    if (BioMoves[i].isUberBomb)
            //    {
            //        return BioMoves[i];
            //    }
            //    //Prefer Maximum Combos
            //    if (ComboDetect(BioMoves[i]) > ComboDetect(BioMoves[max_idx]))
            //    {
            //        max_idx = i;
            //    }
            //    else if (ComboDetect(BioMoves[i]) == ComboDetect(BioMoves[max_idx]))
            //    {
            //        if (BioMoves[i].StartBlock.y > BioMoves[max_idx].StartBlock.y || BioMoves[i].EndBlock.y > BioMoves[max_idx].EndBlock.y)
            //        {
            //            max_idx = i;
            //        }
            //    }
            //    ////Prefer Maximum Blocks
            //    //if (BioMoves[i].TotalBlocks > BioMoves[max_idx].TotalBlocks)
            //    //{
            //    //    max_idx = i;
            //    //}
            //    //else if (BioMoves[i].TotalBlocks == BioMoves[max_idx].TotalBlocks)
            //    //{
            //    //    //Prefer Down moves
            //    //    if (BioMoves[i].StartBlock.y > BioMoves[max_idx].StartBlock.y || BioMoves[i].EndBlock.y > BioMoves[max_idx].EndBlock.y)
            //    //    {
            //    //        max_idx = i;
            //    //    }
            //    //}
            //}

            ////Prefer Bombs if not 4 or more total blocks
            ////if (BioMoves[max_idx].TotalBlocks == 3)
            ////{
            ////    max_idx = 0;
            ////    for (int i = 0; i < BioMoves.Count; i++)
            ////    {
            ////        if (BioMoves[i].TotalBlocks == 3)
            ////        {
            ////            if (BioMoves[i].hasBomb)
            ////            {
            ////                max_idx = i;
            ////            }
            ////        }
            ////    }
            ////}

            //return BioMoves[max_idx];
        }

        public int DetectCombos(BioMove mov)
        {
            //Create Virtual BioMatrix
            BioColors[][] BioTempMatrix = new BioColors[BioMatrixWidth][];
            for (int i = 0; i < BioMatrixWidth; i++)
            {
                BioTempMatrix[i] = new BioColors[BioMatrixHeight];
                for (int j = 0; j < BioMatrixHeight; j++)
                {
                    BioTempMatrix[i][j] = BioMatrix[i][j];
                }
            }

            //Make the Virtual Move
            //Phase 1 - Swap BioBlocks
            BioColors temp = BioTempMatrix[mov.StartBlock.Row][mov.StartBlock.Column];
            BioTempMatrix[mov.StartBlock.Row][mov.StartBlock.Column] = BioTempMatrix[mov.EndBlock.Row][mov.EndBlock.Column];
            BioTempMatrix[mov.EndBlock.Row][mov.EndBlock.Column] = temp;

            // Get the move's BioColor
            BioColors movColor = BioMatrix[mov.StartBlock.Row][mov.StartBlock.Column];
            
            //Phase 2 - Eliminate Played BioBlocks
            int lastCol = mov.EndBlock.Row;
            int lastCol2 = mov.EndBlock.Row;
            int countCombos = 0;
            bool foundCombo = false;

            for (int column = 0; column < BioMatrixWidth; column++)
            {
                for (int row = 0; row < BioMatrixHeight; row++)
                {
                    BioColors currentColor = BioTempMatrix[column][row];
                    //Don't Check unknown color bioblocks
                    if (currentColor == BioColors.UnknownColor)
                    {
                        continue;
                    }
                    int count_same_col = 1;
                    //check right
                    for (int w = column + 1; w < BioMatrixWidth; w++)
                    {
                        if (BioTempMatrix[w][row] == currentColor)
                        {
                            count_same_col++;
                        }
                        else
                        {
                            if (count_same_col > 2)
                            {
                                //Found Valid Move, Eliminate BioBlocks
                                for (int w2 = column; w2 < w; w2++)
                                {
                                    if (count_same_col > 3)
                                    {
                                        if (w2 == lastCol || w2 == lastCol2)
                                        {
                                            continue;
                                        }
                                    }
                                    if (row > 0)
                                    {
                                        for (int w3 = row; w3 > 0; w3--)
                                        {
                                            BioTempMatrix[w2][w3] = BioTempMatrix[w2][w3 - 1];
                                            BioTempMatrix[w2][w3 - 1] = BioColors.UnknownColor;
                                        }
                                    }
                                    else
                                    {
                                        BioTempMatrix[w2][0] = BioColors.UnknownColor;
                                    }
                                }
                                foundCombo = true;
                                lastCol = column;
                                lastCol2 = w - 1;
                                countCombos++;
                            }
                            break;
                        }
                    }
                    count_same_col = 1;
                    //check down
                    for (int w = row + 1; w < BioMatrixHeight; w++)
                    {
                        if (BioTempMatrix[column][w] == currentColor)
                        {
                            count_same_col++;
                        }
                        else
                        {
                            if (count_same_col > 2)
                            {
                                int diff = 1;
                                if (count_same_col > 3)
                                {
                                    BioTempMatrix[column][row + count_same_col - 1] = BioTempMatrix[column][row];
                                    diff = 0;
                                }
                                //Found Valid Move, Eliminate BioBlocks
                                int w3 = 0;
                                if ((row + count_same_col - diff) < BioMatrixHeight)
                                {
                                    for (w3 = row + count_same_col - diff; w3 - count_same_col > 0; w3--)
                                    {
                                        BioTempMatrix[column][w3] = BioTempMatrix[column][w3 - count_same_col];
                                        BioTempMatrix[column][w3 - count_same_col] = BioColors.UnknownColor;
                                    }
                                }
                                if (w3 > 0)
                                {
                                    for (int w4 = w3; w4 > 0; w4--)
                                    {
                                        BioTempMatrix[column][w4] = BioTempMatrix[column][w4 - 1];
                                        BioTempMatrix[column][w4 - 1] = BioColors.UnknownColor;
                                    }
                                }
                                foundCombo = true;
                                lastCol = column;
                                lastCol2 = column;
                                countCombos++;
                            }
                            break;
                        }
                    }

                    if (foundCombo)
                    {
                        column = 0;
                        row = 0;
                        foundCombo = false;
                    }
                }
                //if (count_combos > 1)
                //{
                //    break;
                //}
            }
            return (countCombos * countCombos) + DetectAvailableMoves(BioTempMatrix, false).Count;
        }

        public int CountKnownColor(BioColors col)
        {
            int count = 0;
            for (int i = 0; i < BioMatrixWidth; i++)
            {
                for (int j = 0; j < BioMatrixHeight; j++)
                {
                    if (BioMatrix[i][j] == col)
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        public void PreviewBioMatrix(Image argImage)
        {
            if (IsStarted)
            {
                Graphics g1 = Graphics.FromImage(argImage);
                g1.FillRectangle(Brushes.Black, new Rectangle(0, 0, argImage.Width, argImage.Height));
                Brush br = Brushes.Black;
                for (int i = 0; i < BioMatrixWidth; i++)
                {
                    for (int j = 0; j < BioMatrixHeight; j++)
                    {
                        int x = BioWidthOffset + i * (BioWidth + BioSpace);
                        int y = BioHeightOffset + j * (BioHeight + BioSpace);
                        
                        switch (BioMatrix[i][j])
                        {
                            case BioColors.Blue:
                                br = Brushes.Blue;
                                break;
                            case BioColors.Green:
                                br = Brushes.Green;
                                break;
                            case BioColors.LightBlue:
                                br = Brushes.LightBlue;
                                break;
                            case BioColors.Orange:
                                br = Brushes.Orange;
                                break;
                            case BioColors.Purple:
                                br = Brushes.Purple;
                                break;
                            case BioColors.Red:
                                br = Brushes.Red;
                                break;
                            case BioColors.UnknownColor:
                                br = Brushes.Gray;
                                break;
                            case BioColors.Yellow:
                                br = Brushes.Yellow;
                                break;
                            case BioColors.UberBomb:
                                br = Brushes.HotPink;
                                break;
                            default:
                                br = Brushes.Black;
                                break;
                        }
                        g1.FillRectangle(br, new Rectangle(x, y, BioWidth, BioHeight));

                        if (BioBombMatrix[i][j])
                        {
                            g1.FillEllipse(Brushes.Chocolate, new Rectangle(x + BioWidth / 4, y + BioHeight / 4, BioWidth / 2, BioHeight / 2));
                        }

                        g1.DrawString(BioMatrix[i][j].ToString(), new Font(FontFamily.GenericSansSerif, 10), Brushes.Black, new PointF(x, y + 5));
                    }
                }
            }
        }

        public void PreviewMoves(Image argImage, List<BioMove> argBioMoves)
        {
            if (IsStarted)
            {
                for (int i = 0; i < argBioMoves.Count; i++)
                {
                    int start_x = BioWidthOffset + argBioMoves[i].StartBlock.Row * (BioWidth + BioSpace);
                    int start_y = BioHeightOffset + argBioMoves[i].StartBlock.Column * (BioHeight + BioSpace);

                    Graphics g1 = Graphics.FromImage(argImage);

                    g1.DrawRectangle(new Pen(Brushes.White, 3), start_x, start_y, BioWidth, BioHeight);

                    int end_x = BioWidthOffset + argBioMoves[i].EndBlock.Row * (BioWidth + BioSpace);
                    int end_y = BioHeightOffset + argBioMoves[i].EndBlock.Column * (BioHeight + BioSpace);

                    g1.DrawRectangle(new Pen(Brushes.Silver, 3), end_x, end_y, BioWidth, BioHeight);

                    int end_x1 = 0;
                    int end_x2 = 0;
                    int end_y1 = 0;
                    int end_y2 = 0;

                    start_x = start_x + BioWidth / 2;
                    end_x = end_x + BioWidth / 2;

                    start_y = start_y + BioHeight / 2;
                    end_y = end_y + BioHeight / 2;

                    if (start_x > end_x)
                    {
                        end_x1 = end_x + BioWidth / 4;
                        end_x2 = end_x1;
                        end_y1 = end_y + BioHeight / 4;
                        end_y2 = end_y - BioHeight / 4;
                    }
                    else if (start_x < end_x)
                    {
                        end_x1 = end_x - BioWidth / 4;
                        end_x2 = end_x1;
                        end_y1 = end_y + BioHeight / 4;
                        end_y2 = end_y - BioHeight / 4;
                    }

                    if (start_y > end_y)
                    {
                        end_y1 = end_y + BioHeight / 4;
                        end_y2 = end_y1;
                        end_x1 = end_x + BioWidth / 4;
                        end_x2 = end_x - BioWidth / 4;
                    }
                    else if (start_y < end_y)
                    {
                        end_y1 = end_y - BioHeight / 4;
                        end_y2 = end_y1;
                        end_x1 = end_x + BioWidth / 4;
                        end_x2 = end_x - BioWidth / 4;
                    }

                    g1.DrawLine(new Pen(Brushes.Black, 2), new Point(start_x, start_y), new Point(end_x, end_y));
                    g1.DrawLine(new Pen(Brushes.Black, 2), new Point(end_x, end_y), new Point(end_x1, end_y1));
                    g1.DrawLine(new Pen(Brushes.Black, 2), new Point(end_x, end_y), new Point(end_x2, end_y2));

                }
            }
        }

        public void WriteDebugDump(Image argImage)
        {
            StringBuilder builder = new StringBuilder();
            String dateTimeStamp = DateTime.Now.ToString("[yyyy-MM-dd][HH-mm-ss]");
            for (int i = 0; i < BioMatrixWidth; i++)
            {
                for (int j = 0; j < BioMatrixHeight; j++)
                {
                    builder.AppendLine(String.Format("Bioblock : {0},{1} {2}", i, j, BioMatrix[i][j]));
                }
            }

            using (StreamWriter sw = new StreamWriter(String.Format("{0}_gTronicDump.txt", dateTimeStamp), false))
            {
                sw.Write(builder.ToString());
            }
            argImage.Save(String.Format("{0}_gTronicImg.png", dateTimeStamp));
        }

    }
}
