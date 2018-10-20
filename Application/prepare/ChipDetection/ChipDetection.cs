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
    //public enum DetectionStatus
    //{
    //    NA,
    //    Pass,
    //    Failure,
    //}
    public class ChipDetection
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct Result_stru//
        {                
            public int x;
            public int y;
            public int status;
        };
       
        [DllImport(@"myprocess.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall, EntryPoint = "process")]
        private extern static int process(IntPtr pdata, UInt32 w, UInt32 h, ref double worldx, ref double worldy);

        [DllImport(@"myprocess.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall, EntryPoint = "calibration")]
        private extern static Double calibration(string filename);

        [DllImport(@"myprocess.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall, EntryPoint = "test")]
        private extern static int test();

        [DllImport(@"myprocess.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall, EntryPoint = "processtest")]
        private extern static void processtest(ref double worldx, ref double worldy);

        [DllImport(@"myprocess.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall, EntryPoint = "buffertt")]
        private extern static void buffertt(IntPtr pdata, int w, int h);
        //新增
        public double realworldx
        {
            set;
            get;
        }
        public double realworldy
        {
            set;
            get;
        }
        public int gearnum
        {
            set;
            get;
        }
       
        public ChipDetection()//构造函数
        {
            
            ResetDectionStatus();
          
        }

        public void ResetDectionStatus()
        {
            realworldx = 0;
            realworldy = 0;
            gearnum = 0;
        }

        public int mytest()
        {
            return test();
            //return 1;
        }
        public Double Calibration(string filename)
        {
            try
            {
                return calibration(filename);
            }
            catch (Exception ex)
            {
                System.Console.Out.WriteLine(ex.Message);
                return 12;
            }
           

        }

        //芯片检测运动状态
        //在窗口中绘制信息，0，1，阈值
        public void Draw(Graphics g)
        {
            //Pen pen1 = new Pen(Color.Green, 4);
            //SolidBrush drawBrush = new SolidBrush(Color.Red);
            //SolidBrush drawBrush2 = new SolidBrush(Color.Green);
            //Font MyFont1 = new Font("宋体", 24, FontStyle.Bold);
            //Font MyFont2 = new Font("宋体", 15, FontStyle.Bold);
            //for (int i = 0; i < nsumchip; i++)
            //{
            //    //绘制图像结果
            //    g.DrawString(curresluts[i].status.ToString(), MyFont1, drawBrush, (int)((double)curresluts[i].x / Parameter.GetInstance().WidthRatio - 12),
            //        (int)((double)curresluts[i].y / Parameter.GetInstance().HeightRatio - 12));
            //}
            //g.DrawEllipse(pen1, (int)((double)curChipLocation.X / Parameter.GetInstance().WidthRatio - 12),
            //        (int)((double)curChipLocation.Y / Parameter.GetInstance().HeightRatio - 12), 24, 24);
            ////模板阈值信息显示
            //g.DrawString(chipThreVal.ToString(), MyFont2, drawBrush2, 5, 5);
            //g.DrawString(blackThreVal.ToString(), MyFont2, drawBrush2, 5, 20);

        }

        //获取模板信息//计数
        //测试buffer
        public void BufferTest(IntPtr pData, int w, int h)
        {
            unsafe 
            {

                buffertt(pData, w, h);
                
            }
        
        }
        //测试图像
        public void Processtest()
        {
            // 开始时间
            try
            {
                double realx = 0;
                double realy = 0;

                processtest(ref realx, ref realy);

                realworldx = realx;
                realworldy = realy;

           
            }
            catch (Exception ex)
            {
                System.Console.Out.WriteLine(ex.Message);
                MessageBox.Show(ex.Message);
                realworldx = 555;
                realworldy = 666;
            }
           
        }
        
        //图像处理
        public void ProcessImage(IntPtr pData, UInt32 w, UInt32 h)
        {
            // 开始时间
            DateTime beforProc = System.DateTime.Now;
            try
            {
                
                    double realx = 0;
                    double realy = 0;

                    gearnum = process(pData, w, h, ref realx, ref realy);
                   
                    realworldx = realx;
                    realworldy = realy;
                 
            }
            catch (Exception ex)
            {
                System.Console.Out.WriteLine(ex.Message);
            }

            //结束时间
            DateTime afterProc = System.DateTime.Now;
            TimeSpan ts = afterProc.Subtract(beforProc);
            Debug.WriteLine("算法耗时{0}ms", ts.TotalMilliseconds);//这个是显示在哪里的？
        }
    }
   
}
