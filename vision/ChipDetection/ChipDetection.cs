using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Diagnostics;

namespace ChipDetection
{

    public class ChipDetection
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct Result_stru
        {
            public int x;
            public int y;
            public int status;
        };


        [DllImport(@"myprocess.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall, EntryPoint = "gray2rbg")]
        private extern static void gray2rbg(IntPtr src, IntPtr des, int srcw, int srch, int desw, int desh, int desstride);


        [DllImport(@"myprocess.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall, EntryPoint = "process")]
         private extern static void process(IntPtr pdata, UInt32 w, UInt32 h, int leftboundx, int rightboundx, ref Double leftx, ref Double lefty, ref Double rightx, ref Double righty,
                                                                                            ref Double middlex, ref Double middley);

        [DllImport(@"myprocess.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall, EntryPoint = "processfirstimage")]
        private extern static void processfirstimage(IntPtr pdata, UInt32 w, UInt32 h, ref int leftboundx, ref int rightboundx);

        [DllImport(@"myprocess.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall, EntryPoint = "processyinpian")]
        private extern static void processyinpian(IntPtr pdata, UInt32 w, UInt32 h, ref Double leftx, ref Double lefty,
                                            ref Double rightx, ref Double righty, ref Double middlex, ref Double middley);

        [DllImport(@"myprocess.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall, EntryPoint = "addimage")]
        private extern static void addimage(IntPtr pdata, IntPtr curpdata, UInt32 w, UInt32 h);

        [DllImport(@"myprocess.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall, EntryPoint = "invertimage")]
        private extern static void invertimage(IntPtr pdata, UInt32 w, UInt32 h);


        [DllImport(@"myprocess.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall, EntryPoint = "calibtest")]
        private extern static void calibtest();        
        
        [DllImport(@"myprocess.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall, EntryPoint = "convertcoordinate")]
        private extern static void convertcoordinate(Double imagex,Double imagey,ref Double worldx,ref Double worldy); 
  

        [DllImport(@"myprocess.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall, EntryPoint = "drawline")]
        private extern static void drawline(IntPtr pdata, UInt32 w, UInt32 h, Double x1,Double x2,Double y1,Double y2); 

        [DllImport(@"myprocess.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall, EntryPoint = "test")]
        private extern static int test();


        public Double TubeLeftX
        {
            get;
            set;
        }

        public Double TubeLeftY
        {
            get;
            set;
        }

        public Double TubeRightX
        {
            get;
            set;
        }

        public Double TubeRightY
        {
            get;
            set;
        }

        public Double TubeMiddleX
        {
            get;
            set;
        }

        public Double TubeMiddleY
        {
            get;
            set;
        }

        public int LeftBoundX
        {
            get;
            set;
        }

        public int RightBoundX
        {
            get;
            set;
        }
        
        public Double SheetLeftX
        {
            get;
            set;
        }

        public Double SheetLeftY
        {
            get;
            set;
        }

        public Double SheetRightX
        {
            get;
            set;
        }

        public Double SheetRightY
        {
            get;
            set;
        }

        public Double SheetMiddleX
        {
            get;
            set;
        }

        public Double SheetMiddleY
        {
            get;
            set;
        }

        public Double OffsetX
        {
            set;
            get;
        }

        public Double OffsetY
        {
            set;
            get;
        }

        public Double OffsetL
        {
            set;
            get;
        }

        public Double OffsetR
        {
            set;
            get;
        }

        public Double Xconf
        {
            set;
            get;
        }

        public Double Yconf
        {
            set;
            get;
        }

        public ChipDetection()
        {
            Xconf = 11.48;
            Yconf = 11.48;

            ResetDectionStatus();
            
        }

        public void ResetDectionStatus()
        { 
            /***检测结果****/

            //音筒
            TubeLeftX = 0;
            TubeLeftY = 0;
            TubeRightX = 0;
            TubeRightY = 0;
            TubeMiddleX = 0;
            TubeMiddleY = 0;
            //左右边界
            LeftBoundX = 0;
            RightBoundX = 0;
            //音片
            SheetLeftX = 0;
            SheetLeftY = 0;
            SheetRightX = 0;
            SheetRightY = 0;
            SheetMiddleX = 0;
            SheetMiddleY = 0;
            //偏移量
            OffsetX = 0;
            OffsetY = 0;
            OffsetL = 0;
            OffsetR = 0;
            
        }
        
        //标定测试
        public void CalibTest()
        {
            calibtest();
        }
       
        //坐标转换测试
        public void ConvertTest()
        {
    
            Double wx = 0, wy = 0;

            convertcoordinate(222, 232, ref wx, ref wy);

            SheetMiddleX = wx;
            SheetMiddleY = wy;
         
        }
        
        //坐标转换
        public void ConvertCoordinate()
        { 
            Double wx = 0,wy = 0;
            //音筒
            convertcoordinate(TubeLeftX, TubeLeftY, ref wx, ref wy);
            TubeLeftX = wx;
            TubeLeftY = wy;
            convertcoordinate(TubeRightX, TubeRightY, ref wx, ref wy);
            TubeRightX = wx;
            TubeRightY = wy;
            convertcoordinate(TubeMiddleX, TubeMiddleY, ref wx, ref wy);
            TubeMiddleX = wx;
            TubeMiddleY = wy;
            //音片
            convertcoordinate(SheetLeftX,SheetLeftY , ref wx, ref wy);
            SheetLeftX = wx;          
            SheetLeftY = wy;
            convertcoordinate(SheetRightX,SheetRightY, ref wx, ref wy);
            SheetRightX = wx;
            SheetRightY = wy;
            convertcoordinate(SheetMiddleX,SheetMiddleY, ref wx, ref wy);
            SheetMiddleX = wx;
            SheetMiddleY = wy;


        }
        
        //计算结果
        public void Calculate1()
        { 
            //坐标转换后执行
            OffsetX = SheetMiddleX - TubeMiddleX;
            OffsetY = TubeMiddleY - SheetMiddleY;   //>0

            double a = (SheetRightY - SheetLeftY) / (SheetRightX - SheetLeftX);
            double b = SheetRightY - a * SheetRightX;

            OffsetR = (TubeRightY - (a * (TubeRightX + OffsetX) + b)) - OffsetY;
            OffsetL = (TubeLeftY - (a * (TubeLeftX + OffsetX) + b)) - OffsetY;

        }
        
        //计算1
        public void Calculate()
        {
            //音筒往音片靠
            //OffsetX = SheetMiddleX - TubeMiddleX;
            //OffsetY = TubeMiddleY - SheetMiddleY;   //>0
            //double a = (SheetRightY - SheetLeftY) / (SheetRightX - SheetLeftX);
            //double b = SheetRightY - a * SheetRightX;
            //OffsetR = (TubeRightY - (a * (TubeRightX + OffsetX) + b)) - OffsetY;
            ////OffsetL = (TubeLeftY - (a * (TubeLeftX + OffsetX) + b)) - OffsetY;
            //OffsetL = -OffsetR;

            ////连线
            //double Distancepp = Math.Sqrt(Math.Pow((SheetMiddleY - TubeMiddleY), 2) + Math.Pow((SheetMiddleX - TubeMiddleX), 2));
            //double MiddleDeg = Math.Acos((TubeMiddleX - SheetMiddleX) / Distancepp);
            //double PtDeg = MiddleDeg - Math.Atan((SheetRightY - SheetLeftY) / (SheetRightX - SheetLeftX));
            //OffsetY = Distancepp * Math.Sin(PtDeg);
            //OffsetX = Distancepp * Math.Cos(PtDeg);


            //以音片为坐标系
            double YpDeg = Math.Atan((SheetRightY - SheetLeftY) / (SheetRightX - SheetLeftX));
            double YtDeg = Math.Atan((TubeRightY - TubeLeftY)/(TubeRightX - TubeLeftX));
            double Deg = YtDeg - YpDeg;

            double DistanceX = TubeMiddleX - SheetMiddleX;
            double DistanceY = TubeMiddleY - SheetMiddleY;   //>0


            OffsetX = DistanceX * Math.Cos(YpDeg) + DistanceY * Math.Sin(YpDeg);
            OffsetY = DistanceY * Math.Cos(YpDeg) - DistanceX * Math.Sin(YpDeg);

            //以前是直接减的
            //OffsetX = SheetMiddleX - TubeMiddleX;

            //OffsetR = 670 / Math.Cos(YtDeg) * Math.Sin(YtDeg - YpDeg) * 24;
            //OffsetL = - OffsetR / 2;

            double r = 670 / Math.Cos(YtDeg) * Math.Sin(Deg);//atttention
            double l = -r;

            //放大系数12
            OffsetR = r * 12;
            OffsetL = l * 12;

            //图像尺寸转物理尺寸
            OffsetX = (OffsetX * Xconf / 10);
            OffsetY = (OffsetY * Yconf / 10);
            OffsetR = (OffsetR * Yconf / 10);
            OffsetL = (OffsetL * Yconf / 10);

            //考虑机械转动使音筒中心偏转
            OffsetX += 600 * Math.Sin(Deg);

            //音片夹持机构与X轴运动方向相反
            OffsetX = -OffsetX;
        
            //防止撞击
            //if (OffsetY > 700 || OffsetY < 500) OffsetY = 650;
            //if (Math.Abs(OffsetX) >200) OffsetX = 5;
            //if (Math.Abs(OffsetR) > 300) OffsetR = OffsetL = 1;

        }

        //反转图像
        public void InvertImage(IntPtr pData, UInt32 w, UInt32 h)
        {
            try
            {
                unsafe
                {
                    DateTime beforProc = System.DateTime.Now;
                    invertimage(pData, w, h);
                    DateTime afterProc = System.DateTime.Now;
                    TimeSpan ts = afterProc.Subtract(beforProc);
                    Debug.WriteLine("反转图像-算法耗时{0}ms", ts.TotalMilliseconds);
                }
            }
            catch (Exception ex)
            {
                System.Console.Out.WriteLine(ex.Message);
            }
        
        }
        
        //反转相加
        public void AddImage(IntPtr pData, IntPtr CurpData, UInt32 w, UInt32 h)
        {
            try 
            {
                DateTime beforProc = System.DateTime.Now;
                addimage(pData, CurpData, w, h);
                //结束时间
                DateTime afterProc = System.DateTime.Now;
                TimeSpan ts = afterProc.Subtract(beforProc);
                Debug.WriteLine("反转叠加-算法耗时{0}ms", ts.TotalMilliseconds);
                
            }
            catch (Exception ex)
            {
                System.Console.Out.WriteLine(ex.Message);
            }
 
        
        }

        static public void getimage(IntPtr src, IntPtr des, int srcw, int srch, int desw, int desh,int desstride)
        {
            gray2rbg(src,des,srcw,srch,desw,desh,desstride);
        }
        
        //处理第一张图片为了得到左右边界位置
        public void ProcessFirstImage(IntPtr pData, UInt32 w, UInt32 h)
        {
            DateTime beforProc = System.DateTime.Now;
            try 
            {
                int leftx = 0, rightx = 0;
                processfirstimage(pData, w, h, ref leftx, ref rightx);

                LeftBoundX = leftx;
                RightBoundX = rightx;
            }
            catch (Exception ex)
            {
                System.Console.Out.WriteLine(ex.Message);
            }

            DateTime afterProc = System.DateTime.Now;
            TimeSpan ts = afterProc.Subtract(beforProc);
            Debug.WriteLine("处理一张-算法耗时{0}ms", ts.TotalMilliseconds);
        }


        //处理音筒
        public void ProcessImage(IntPtr pData, UInt32 w, UInt32 h)
        {
            // 开始时间
            DateTime beforProc = System.DateTime.Now;
            try
            {
                Double tubeleftx = 0, tuberightx = 0;
                Double tubelefty = 0, tuberighty = 0;
                Double tubemiddlex = 0, tubemiddley = 0;

                process(pData, w, h, LeftBoundX, RightBoundX, ref tubeleftx, ref tubelefty, ref tuberightx, ref tuberighty, ref tubemiddlex, ref tubemiddley);

                TubeLeftX = tubeleftx;  TubeLeftY = tubelefty;
                TubeRightX = tuberightx;  TubeRightY = tuberighty;
                TubeMiddleX = tubemiddlex;  TubeMiddleY = tubemiddley;
              
            }
            catch (Exception ex)
            {
                System.Console.Out.WriteLine(ex.Message);
            }

            //结束时间
            DateTime afterProc = System.DateTime.Now;
            TimeSpan ts = afterProc.Subtract(beforProc);
            Debug.WriteLine("处理音筒-算法耗时{0}ms", ts.TotalMilliseconds);
        }
        
        //处理音片
        public void ProcessYinPian(IntPtr pData, UInt32 w, UInt32 h)
        {
            // 开始时间
            DateTime beforProc = System.DateTime.Now;
            try
            {
                Double leftx = 0, lefty = 0, rightx = 0, righty = 0, middlex = 0, middley = 0;
                processyinpian(pData, w, h, ref leftx, ref lefty,ref rightx, ref righty,ref middlex, ref middley);

                SheetLeftX = leftx; SheetLeftY = lefty;
                SheetRightX = rightx; SheetRightY =  righty;
                SheetMiddleX = middlex; SheetMiddleY = middley;
               
            }
            catch (Exception ex)
            {

                System.Console.Out.WriteLine(ex.Message);
            }

            //结束时间
            DateTime afterProc = System.DateTime.Now;
            TimeSpan ts = afterProc.Subtract(beforProc);
            Debug.WriteLine("处理音片-算法耗时{0}ms", ts.TotalMilliseconds);
        }

        //测试划线
        public void Drawline(IntPtr pData, UInt32 w, UInt32 h, Double x1, Double x2, Double y1, Double y2)
        {
            try 
            {
                drawline(pData, w, h, x1, x2, y1, y2);
            }
            catch (Exception ex)
            {
                System.Console.Out.WriteLine(ex.Message);
            }
            
            
        }
    

    }
}
