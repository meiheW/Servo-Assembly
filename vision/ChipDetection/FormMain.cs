using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.IO.Ports;
using Microsoft.Win32;
using MiscFunctions;
using DALSA.SaperaLT.SapClassBasic;
using Timer = System.Timers.Timer;
using System.Diagnostics;


namespace ChipDetection
{
  
    public partial class FormChipDetection : Form
    {
        private UserAuthority mUserAuthority;
        private Configuration mConfiguration;          
        private ChipDetection mChipDetection;
        private Timer mInitializationTimer;
        private DateTime mStartTime;  
        private bool mIsResetConfiguration;
        private Bitmap m_BitMap = null;
        private bool m_grabImages;
        private AutoResetEvent m_grabThreadExited;
        private BackgroundWorker m_grabThread;
        //相机
        private SapAcqDevice m_AcqDevice = null;
        private SapBuffer m_Buffers = null;
        private SapBuffer m_CurBuffers = null;
        private SapTransfer m_Xfer = null;
        private SapLocation m_loc = null;
        private IntPtr BufAddress;
        private IntPtr curBufAddress;

        private int nSmallerCof = 20;
        private int nshowImageW = 0;
        private int nshowImageH = 0;
        //通讯           
        private SerialPort mCom_;
        String strprev = " ";     // 保存上一次数据
        bool snapped = false;
        bool grabbed = false;
       
 
        public FormChipDetection(bool isResetConfiguration)
        {

            mIsResetConfiguration = isResetConfiguration;
            InitializeComponent();

            Parameter.GetInstance().LoadOptionFile();
            mConfiguration = new Configuration(isResetConfiguration);

            if (isResetConfiguration)
            {
                UserConfiguration.GetInstance().CreateDefualtUserConfigurationFile();
            }
            else
            {
                UserConfiguration.GetInstance().LoadUserConfigurationFile();
            }
            mChipDetection = new ChipDetection();

            Parameter.GetInstance().WidthRatio = ((double)Parameter.GetInstance().WidthPixels) / picImage.Width;
            Parameter.GetInstance().HeightRatio = ((double)Parameter.GetInstance().HeightPixels) / picImage.Height;

            lblMark.BackColor = Color.Transparent;
            Form.CheckForIllegalCrossThreadCalls = false;   
            
            picImage.Paint += new PaintEventHandler(PicImage_Paint);

            InitializeTimers();
            //UserLogin(false);
                              
            m_grabThreadExited = new AutoResetEvent(false);
            CAM_connect();

            mCom_ = new SerialPort();
            mCom_Open();

            ResetTestStatus();
          
            lblStatusIndicator.Text = "初始化完成";

        }

        private void mInitializationTimer_Elapsed(object sender, EventArgs e)
        {
            mInitializationTimer.Enabled = false;
            //WaitDialogForm waitDialog = new WaitDialogForm(string.Empty, "初始化设备...", new Size(150, 100));

            try
            {
                Application.DoEvents();
                string errorString = string.Empty;
                bool isAllSuccess = true;
                // waitDialog.Caption = "检查系统配置...";
                try
                {
                    if (mConfiguration.CheckSystem)
                    {
                        int returnValue = CheckSystem();
                        //returnValue = 0;
                        Application.DoEvents();
                        Thread.Sleep(1000);
                        if (returnValue != 0)
                        {
                            MessageBox.Show("致命错误，系统无法启动! " + returnValue.ToString());
                            Application.Exit();
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.ToString() + "\r\n 致命错误，系统无法启动!  0xFF");
                    Application.Exit();
                }
                Application.DoEvents();
                // waitDialog.Caption = "初始化相机...";         

                try
                {
                    Application.DoEvents();
                    Thread.Sleep(500);
                }
                catch (System.Exception ex)
                {
                    isAllSuccess = false;
                    //waitDialog.Caption += "失败" + "\r\n" + ex.Message;
                    errorString += "初始化相机失败!" + "\r\n";
                    Application.DoEvents();
                    Thread.Sleep(2000);
                }
                Application.DoEvents();
                // waitDialog.Caption = "初始化采集卡...";
                try
                {
                    Application.DoEvents();
                    Thread.Sleep(500);
                    //string returnString = EM9636B.GetInstance().Connect();
                    //if (!string.IsNullOrEmpty(returnString))
                    //{
                    //    MessageBox.Show("连接数据采集卡失败！" + returnString);
                    //    return;
                    //}
                    //returnString = SetMotorStop();
                    //if (!string.IsNullOrEmpty(returnString))
                    //{
                    //    MessageBox.Show("连接数据采集卡失败！" + returnString);
                    //    return;
                    //}
                }
                catch (System.Exception ex)
                {
                    System.Console.Out.WriteLine(ex.Message);
                    isAllSuccess = false;
                    //  waitDialog.Caption += "初始化采集卡失败!" + "\r\n";
                    Application.DoEvents();
                    Thread.Sleep(2000);
                }
                Application.DoEvents();
                // waitDialog.Caption = "读取用户权限...";
                try
                {
                    //mUserConfiguration = UserConfiguration.CreateInstance(false);

                    // waitDialog.Caption += "";
                    Application.DoEvents();
                    Thread.Sleep(500);
                }
                catch (System.Exception ex)
                {
                    isAllSuccess = false;
                    //   waitDialog.Caption += "失败!" + "\r\n" + ex.Message;
                    errorString += "读取用户权限失败！" + "\r\n";
                    Application.DoEvents();
                    Thread.Sleep(500);

                }
                if (isAllSuccess)
                {
                    //   waitDialog.Caption = "启动初始化成功！";
                    Application.DoEvents();
                    Thread.Sleep(500);
                     }
                else
                {
                    //RefreshTestStatus(TestStatus.Error);
                    //btnTestControl.Enabled = false;
                    MessageBox.Show(errorString, "错误");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Application.Exit();
            }
            finally
            {
                //  waitDialog.Close();
            }

        }

        private void mCom_Open()
        {
            if (mCom_.IsOpen) return;
            
            try
            {
                mCom_.PortName = "COM6";
                mCom_.BaudRate = 9600;
                mCom_.Parity = Parity.None;
                mCom_.DataBits = 8;
                mCom_.StopBits = StopBits.Two;
                mCom_.RtsEnable = true;
                mCom_.DtrEnable = true;
                mCom_.WriteTimeout = 1000;
                mCom_.Open();
                mCom_.DataReceived += new SerialDataReceivedEventHandler(serialPort_DataReceived);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                LogHelper.WriteLog(typeof(FormChipDetection), ex);    
            }

        }

        private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {

            int readsize = mCom_.BytesToRead;
            if (readsize > 0)
            {
                byte[] recvbyte = new byte[readsize];
                mCom_.Read(recvbyte, 0, readsize);
                String strrecv = Encoding.ASCII.GetString(recvbyte);
             
                if (strrecv != strprev )
                {
                    if (strrecv.Contains("01"))
                    {
                        mChipDetection.ResetDectionStatus();
                        Snap();
                        snapped = true;
                        grabbed = false;
                        sendCom_data("0A\r");
                    }
                    else if (strrecv.Contains("02") && snapped && !grabbed)
                    {
                        SeekBound();
                        grabbed = true;
                        sendCom_data("0B\r");
                        Grab();
                    }
                    else if (strrecv.Contains("03") && snapped && grabbed)
                    {
                        snapped = false;
                        snapped = false;
                        Freeze();
                        Thread.Sleep(50);
                        Send_Offset();
                    }
                    strprev = strrecv;
                }

            }

        }

        private void Send_Offset()
        {
            try
            {
               
                int x1 = (int)mChipDetection.OffsetX;
                int x2 = (int)mChipDetection.OffsetL;
                int x3 = (int)mChipDetection.OffsetR;
                int x4 = (int)mChipDetection.OffsetY;

                int x1o = (x1 >= 0) ? 0 : 1;
                int x2o = (x2 >= 0) ? 0 : 1;
                int x3o = (x3 >= 0) ? 0 : 1;
                
                String s = String.Format("0C{0:d1}{1:d4}{2:d1}{3:d4}{4:d1}{5:d4}{6:d4}\r",
                    x1o, Math.Abs(x1),
                    x2o, Math.Abs(x2),
                    x3o, Math.Abs(x3),
                    Math.Abs(x4));

                sendCom_data(s);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                LogHelper.WriteLog(typeof(FormChipDetection), ex);
            }

        }

        private void sendCom_data(string str)
        {
            try
            {
                if (mCom_.IsOpen)
                {
                    mCom_.Write(str);
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
                LogHelper.WriteLog(typeof(FormChipDetection), ex);
            }
        }
        
        private void Snap()
        {
            try
            {

                m_Xfer.Snap();
                Thread.Sleep(50);
                m_Buffers.Save("C:\\Users\\Administrator\\Desktop\\yinpian.bmp", "-format bmp");

                m_Buffers.GetAddress(out BufAddress);
                mChipDetection.ProcessYinPian(BufAddress, (UInt32)m_Buffers.Width, (UInt32)m_Buffers.Height);

                //存图看效果
                mChipDetection.Drawline(BufAddress, (UInt32)m_Buffers.Width, (UInt32)m_Buffers.Height, mChipDetection.SheetLeftX, mChipDetection.SheetRightX,
                mChipDetection.SheetLeftY, mChipDetection.SheetRightY);

                m_Buffers.Save("C:\\Users\\Administrator\\Desktop\\yinpian_lined.bmp", "-format bmp");
                
                //界面显示
                lbl1Val.Text = ((int)mChipDetection.SheetLeftX).ToString();
                lbl2Val.Text = ((int)mChipDetection.SheetLeftY).ToString();
                lbl3Val.Text = ((int)mChipDetection.SheetRightX).ToString();
                lbl4Val.Text = ((int)mChipDetection.SheetRightY).ToString();

            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
                LogHelper.WriteLog(typeof(FormChipDetection), ex);
            }
        }

        private void SeekBound()
        {
            try
            {

                m_Xfer.Snap();
                Thread.Sleep(50);
                m_Buffers.Save("C:\\Users\\Administrator\\Desktop\\yintong.bmp", "-format bmp");

                m_Buffers.GetAddress(out BufAddress);
                mChipDetection.ProcessFirstImage(BufAddress, (UInt32)m_Buffers.Width, (UInt32)m_Buffers.Height);

                //存图看效果
                //mChipDetection.Drawline(BufAddress, (UInt32)m_Buffers.Width, (UInt32)m_Buffers.Height, mChipDetection.LeftBoundX, mChipDetection.LeftBoundX, 1000, 2000 );
                //mChipDetection.Drawline(BufAddress, (UInt32)m_Buffers.Width, (UInt32)m_Buffers.Height, mChipDetection.RightBoundX, mChipDetection.RightBoundX, 1000, 2000);

                //m_Buffers.Save("C:\\Users\\Administrator\\Desktop\\yintong_lined.bmp", "-format bmp");

                lblStatusIndicator.Text = "寻找边界";
                //界面显示
                lbl1Val.Text = ((int)mChipDetection.LeftBoundX).ToString();
                lbl2Val.Text = ((int)mChipDetection.RightBoundX).ToString();
                lbl3Val.Text = "0";
                lbl4Val.Text = "0";

            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
                LogHelper.WriteLog(typeof(FormChipDetection), ex);
            }
        
        }

        private void Grab()
        {
            //生成一幅空图像
            if (m_CurBuffers == null)
            {
                m_CurBuffers = new SapBuffer(1, m_Buffers.Width, m_Buffers.Height, SapFormat.Mono8, SapBuffer.MemoryType.Default);
                m_CurBuffers.Create();
                m_CurBuffers.Clear();
                m_CurBuffers.GetAddress(out curBufAddress);
            }
            else
            {
                m_CurBuffers.Clear();
            }

            /*
            if (m_BitMap == null)
            {
                nshowImageW = (int)m_CurBuffers.Width / nSmallerCof;
                nshowImageH = (int)m_CurBuffers.Height / nSmallerCof;

                m_BitMap = new Bitmap(nshowImageW, nshowImageH,
                    (System.Drawing.Imaging.PixelFormat)System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            }
            */
            
            try
            {
                CAM_StartGrab();

                lblStatusIndicator.Text = "正在采图";
                lblStatusIndicator.BackColor = Color.Green;

            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString()+"采图出错了");
                LogHelper.WriteLog(typeof(FormChipDetection), ex);
            }
 
        }

        private void Freeze()
        {
            try
            {
                m_grabImages = false;
                m_Xfer.Freeze();
                Thread.Sleep(50);

                lblStatusIndicator.Text = "处理图像";
                //反转图像
                mChipDetection.InvertImage(curBufAddress, (UInt32)m_CurBuffers.Width, (UInt32)m_CurBuffers.Height);
                Thread.Sleep(50);

                m_CurBuffers.Save("C:\\Users\\Administrator\\Desktop\\processed.bmp", "-format bmp");
                Thread.Sleep(50);

                //处理图像
                mChipDetection.ProcessImage(curBufAddress, (UInt32)m_CurBuffers.Width, (UInt32)m_CurBuffers.Height);

                //划线测试
                mChipDetection.Drawline(curBufAddress, (UInt32)m_CurBuffers.Width, (UInt32)m_CurBuffers.Height, mChipDetection.TubeLeftX, mChipDetection.TubeRightX, mChipDetection.TubeLeftY, mChipDetection.TubeRightY);
                mChipDetection.Drawline(curBufAddress, (UInt32)m_CurBuffers.Width, (UInt32)m_CurBuffers.Height, mChipDetection.TubeMiddleX, mChipDetection.TubeMiddleX, 0, 3000);
                mChipDetection.Drawline(curBufAddress, (UInt32)m_CurBuffers.Width, (UInt32)m_CurBuffers.Height, 1250, 3000, 1525, 1700);
                mChipDetection.Drawline(curBufAddress, (UInt32)m_CurBuffers.Width, (UInt32)m_CurBuffers.Height, 1250, 3000, 1525, 1525);
                mChipDetection.Drawline(curBufAddress, (UInt32)m_CurBuffers.Width, (UInt32)m_CurBuffers.Height, 3000, 3000, 1525, 1700);
                Thread.Sleep(50);

                m_CurBuffers.Save("C:\\Users\\Administrator\\Desktop\\drawlined.bmp", "-format bmp");

                //显示数据
                lbl1Val.Text = ((int)mChipDetection.TubeLeftX).ToString();
                lbl2Val.Text = ((int)mChipDetection.TubeLeftY).ToString();
                lbl3Val.Text = ((int)mChipDetection.TubeRightX).ToString();
                lbl4Val.Text = ((int)mChipDetection.TubeRightY).ToString();

                Thread.Sleep(50);
                //计算
                mChipDetection.Calculate();

                lblStatusIndicator.Text = "计算完成";
                //显示数据
                lbl1Val.Text = ((int)mChipDetection.OffsetX).ToString();
                lbl2Val.Text = ((int)mChipDetection.OffsetY).ToString();
                lbl3Val.Text = ((int)mChipDetection.OffsetL).ToString();
                lbl4Val.Text = ((int)mChipDetection.OffsetR).ToString();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
                LogHelper.WriteLog(typeof(FormChipDetection), ex);
            }
        }

        public bool CAM_connect()
        {
            try
            {
                int ServerCount = SapManager.GetServerCount();//num of servers
                int ServerIndex = ServerCount-1;
                string ServerName = SapManager.GetServerName(ServerIndex);
                int CameraCount = SapManager.GetResourceCount(ServerName, SapManager.ResourceType.AcqDevice);
                int ResourceIndex = CameraCount - 1;
                m_loc = new SapLocation(ServerName, ResourceIndex);
                
                //ConfigFile
                string ConfigPath = "C:\\Program Files\\Teledyne DALSA\\Sapera\\camFiles\\User\\";
                string[] ccffiles = Directory.GetFiles(ConfigPath, "*.ccf");
                int configFileCount = ccffiles.Length;
                string ConfigFileName = ccffiles[0];

                if (SapManager.GetResourceCount(ServerName, SapManager.ResourceType.AcqDevice) > 0)
                {
                    m_AcqDevice = new SapAcqDevice(m_loc, ConfigFileName);
                    m_Buffers = new SapBuffer(1, m_AcqDevice, SapBuffer.MemoryType.ScatterGather);
                    m_Xfer = new SapAcqDeviceToBuf(m_AcqDevice, m_Buffers);
                    
                }

                m_AcqDevice.Create();
                m_Buffers.Create();
                m_Xfer.Create();
               
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(typeof(FormChipDetection), ex);
                MessageBox.Show("相机连接失败!");
                return false;
            }
            return true;
        }

        private void CAM_StartGrab()
        {
            m_Xfer.Grab();
            m_grabImages = true;
            m_grabThread = new BackgroundWorker();
            m_grabThread.ProgressChanged += new ProgressChangedEventHandler(UpdateUI);
            m_grabThread.DoWork += new DoWorkEventHandler(GrabLoop);
            m_grabThread.WorkerReportsProgress = true;
            m_grabThread.RunWorkerAsync();
            
        }

        private void GrabLoop(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            //延迟一下，
            Thread.Sleep(50);

            while (m_grabImages)
            {
                lock (this)
                {
                    
                    m_Buffers.GetAddress(out BufAddress);
                    cvProcess(BufAddress,m_Buffers.Width,m_Buffers.Height);
                }
                worker.ReportProgress(0);
                Thread.Sleep(1);
            }
            m_grabThreadExited.Set();
        }

        public string CAM_StopGrab()
        {
             m_grabImages = false;
            try
            {
                m_Xfer.Freeze();
                
                if (m_Xfer != null)
                {
                    m_Xfer.Destroy();
                    m_Xfer.Dispose();
                }

                if (m_AcqDevice != null)
                {
                    m_AcqDevice.Destroy();
                    m_AcqDevice.Dispose();
                }

                if (m_Buffers != null)
                {
                    m_Buffers.Destroy();
                    m_Buffers.Dispose();
                }

      
                m_loc.Dispose();  
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to stop camera: " + ex.Message);
                return string.Empty;
            }
           

            return string.Empty;
        }
       
        public void CAM_Close()
        {
            try
            {
                m_Xfer.Freeze();

                if (m_Xfer != null)
                {
                    m_Xfer.Destroy();
                    m_Xfer.Dispose();
                }

                if (m_AcqDevice != null)
                {
                    m_AcqDevice.Destroy();
                    m_AcqDevice.Dispose();
                }

                if (m_Buffers != null)
                {
                    m_Buffers.Destroy();
                    m_Buffers.Dispose();
                }


                m_loc.Dispose();
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(typeof(FormChipDetection), ex);                
                MessageBox.Show("Failed to stop camera: " + ex.Message);
            }

        }

        private void cvProcess(IntPtr dataBufAddress, int width, int height)
        {
            unsafe
            {

                //处理音筒
                //mChipDetection.ProcessImage(dataBufAddress, (UInt32)width, (UInt32)height);
                //处理音片
                //mChipDetection.ProcessYinPian(dataBufAddress, (UInt32)buffer.Width, (UInt32)buffer.Height);
                //图像相加
                mChipDetection.AddImage(dataBufAddress, curBufAddress, (UInt32)width, (UInt32)height);
                //图像反转
                //mChipDetection.InvertImage(dataBufAddress, (UInt32)width, (UInt32)height);
                //m_Buffers.Save("C:\\Users\\Administrator\\Desktop\\yt_0422_11_invert.bmp", "-format bmp");

            }
        }
  
        private void UpdateUI(object sender, ProgressChangedEventArgs e)
        {

            if (m_BitMap != null)
            {

                System.Drawing.Imaging.BitmapData d = m_BitMap.LockBits(
                new Rectangle(0, 0, m_BitMap.Width, m_BitMap.Height),
                System.Drawing.Imaging.ImageLockMode.ReadWrite,
                System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                unsafe
                {

                    m_Buffers.GetAddress(out BufAddress);

                    ChipDetection.getimage(BufAddress, d.Scan0, m_Buffers.Width, m_Buffers.Height, m_BitMap.Width, m_BitMap.Height, d.Stride);

                    m_BitMap.UnlockBits(d);
                }

            }

            picImage.Image = m_BitMap;

            picImage.Invalidate(); 
            ChipDetection_RefreshDisplay();

        }
             
        private void UserLogin(bool isReset)
        {
            FormUser formUser = new FormUser(isReset);
            if (formUser.ShowDialog() == DialogResult.OK)
            {
                mUserAuthority = formUser.GetUserAuthority();
                RefreshUserAuthorityMenu();
            }
        }
       
        private void RefreshUserAuthorityMenu()
        {
            if (mUserAuthority == null)
            {
                return;
            }
            if (mUserAuthority.UserType == UserType.Administrator)
            {
                mnuUserManager.Enabled = true;
                grpCalibration.Enabled = true;
                btnTest.Enabled = true;
            }
            else
            {
                mnuUserManager.Enabled = false;
                grpCalibration.Enabled = false;
                btnTest.Enabled = false;


            }
            this.Text = "芯片检测(" + mConfiguration.Version + ")---" + "当前用户: " + mUserAuthority.Name
                            + "(" + (mUserAuthority.UserType.Equals(UserType.Administrator) ? "管理员" : "操作员") + ")";
        }

        private void ResetTestStatus()
        {
            lblCodeValue.Text = "未知";
            
            lbl1Val.Text = "0";
            lbl2Val.Text = "0";
            lbl3Val.Text = "0";
            lbl4Val.Text = "0";


            btnStart.Enabled = true;
            btnStop.Enabled = true;
            btnTest.Enabled = true;

            btnGrab.Enabled = true;
            btnSaveCalibration.Enabled = true;
            btnStopGrab.Enabled = true;

            btnSnap.Enabled = true;
            btnStopMotor.Enabled = true;


        }

        private int CheckSystem()
        {
            //计算机名
            string cName = Environment.GetEnvironmentVariable("ComputerName");
            //if (!cName.Equals("ADMIN-PC"))
            if (!cName.Equals(mConfiguration.PCName))
            {
                return 1;
            }
            //运行时间
            try
            {
                if (mConfiguration.MaxRunningDays < 1000)
                {
                    RegistryKey registryKey = Registry.CurrentUser.CreateSubKey("SOFTWARE\\ChipDetection");
                    string authorityString = registryKey.GetValue("Authority", string.Empty).ToString();
                    if (string.IsNullOrEmpty(authorityString))
                    {
                        registryKey.SetValue("Authority", DateTime.Now);
                    }
                    DateTime installDateTime = Convert.ToDateTime(registryKey.GetValue("Authority", string.Empty));
                    int interval = (int)DateTime.Now.Subtract(installDateTime).TotalDays;
                    if (interval < 0 || interval > mConfiguration.MaxRunningDays)
                    {
                        return interval;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Console.Out.WriteLine(ex.Message);
                return 2;
            }
            return 0;
        }

        private void InitializeTimers()
        {
            mInitializationTimer = new Timer();
            mInitializationTimer.Interval = 20;
            mInitializationTimer.AutoReset = false;
            mInitializationTimer.Enabled = false;
            mInitializationTimer.SynchronizingObject = this;
            mInitializationTimer.Elapsed += new System.Timers.ElapsedEventHandler(mInitializationTimer_Elapsed);

        }


        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                string picpath = "E:\\samples\\0502\\yintong\\02\\70.bmp";
                SapBuffer picbuffer = null;
                picbuffer = new SapBuffer(picpath, SapBuffer.MemoryType.Default);
                picbuffer.Create();
                picbuffer.Load(picpath, -1);

                //IntPtr BufAddress;
                picbuffer.GetAddress(out BufAddress);

                mChipDetection.ProcessFirstImage(BufAddress, (UInt32)picbuffer.Width, (UInt32)picbuffer.Height);

                lbl1Val.Text = ((int)mChipDetection.LeftBoundX).ToString();
                lbl2Val.Text = ((int)mChipDetection.RightBoundX).ToString();
                lbl3Val.Text = "0";
                lbl4Val.Text = "0";

                lblStatusIndicator.Text = "正在检测";
                lblStatusIndicator.BackColor = Color.Green;
             }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            try
            {
                m_Xfer.Snap();
                Thread.Sleep(50);
                m_Buffers.Save("E:\\samples\\0502\\yinpian\\08.bmp", "-format bmp");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                LogHelper.WriteLog(typeof(FormChipDetection), ex);

            }
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            try
            {
                //测试音片
                string picpath = "C:\\Users\\Administrator\\Desktop\\yinpian.bmp";
                SapBuffer picbuffer = null;
                picbuffer = new SapBuffer(picpath, SapBuffer.MemoryType.Default);
                picbuffer.Create();
                picbuffer.Load(picpath, -1);

                //IntPtr BufAddress;
                picbuffer.GetAddress(out BufAddress);

                mChipDetection.ProcessYinPian(BufAddress, (UInt32)picbuffer.Width, (UInt32)picbuffer.Height);
                mChipDetection.Drawline(BufAddress, (UInt32)picbuffer.Width, (UInt32)picbuffer.Height, mChipDetection.SheetLeftX, mChipDetection.SheetRightX,
                                                 mChipDetection.SheetLeftY, mChipDetection.SheetRightY);
                mChipDetection.Drawline(BufAddress, (UInt32)picbuffer.Width, (UInt32)picbuffer.Height, mChipDetection.TubeLeftX, mChipDetection.TubeRightX,
                                                  mChipDetection.TubeLeftY, mChipDetection.TubeRightY);
                
                picbuffer.Save("C:\\Users\\Administrator\\Desktop\\yp_lined.bmp", "-format bmp");

                //界面显示
                lbl1Val.Text = ((int)mChipDetection.SheetLeftX).ToString();
                lbl2Val.Text = ((int)mChipDetection.SheetLeftY).ToString();
                lbl3Val.Text = ((int)mChipDetection.SheetRightX).ToString();
                lbl4Val.Text = ((int)mChipDetection.SheetRightY).ToString();

            }
            catch (Exception ex)
            {
                MessageBox.Show("音片处理出现异常" + ex.ToString());
                LogHelper.WriteLog(typeof(FormChipDetection), ex);

            }
        }

        private void FormChipDetection_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                //关闭串口
                if (mCom_.IsOpen)
                {
                    mCom_.Close();
                }
                //关闭相机
                CAM_Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                LogHelper.WriteLog(typeof(FormChipDetection), ex);
            }

        }

        //保存测试结果文件
        private void SaveResultToExcelFile()
        {


        }
        
        

        //button1，Excel测试按键
        private void Mytest1(object sender, EventArgs e)
        {
            try
            {
                //测试标定
                //mChipDetection.CalibTest();
                //测试转换
                //mChipDetection.ConvertTest();   
                //测试计算结果
                //mChipDetection.Calculate();

                
                //处理音筒图片
                string picpath = "E:\\samples\\0502\\yintong\\02_add\\02_1.bmp";
                SapBuffer picbuffer = null;
                picbuffer = new SapBuffer(picpath, SapBuffer.MemoryType.Default);
                picbuffer.Create();
                picbuffer.Load(picpath, -1);

                IntPtr BufAddress;
                picbuffer.GetAddress(out BufAddress);

                mChipDetection.ProcessImage(BufAddress, (UInt32)picbuffer.Width, (UInt32)picbuffer.Height);
                
                //划线
                mChipDetection.Drawline(BufAddress, (UInt32)picbuffer.Width, (UInt32)picbuffer.Height, mChipDetection.TubeLeftX, mChipDetection.TubeRightX,
                mChipDetection.TubeLeftY, mChipDetection.TubeRightY);

                mChipDetection.Drawline(BufAddress, (UInt32)picbuffer.Width, (UInt32)picbuffer.Height, mChipDetection.TubeMiddleX, mChipDetection.TubeMiddleX, 0, 3000);

                mChipDetection.Drawline(BufAddress, (UInt32)picbuffer.Width, (UInt32)picbuffer.Height, 1250, 3000, 1525, 1700);
                mChipDetection.Drawline(BufAddress, (UInt32)picbuffer.Width, (UInt32)picbuffer.Height, 1250, 3000, 1525, 1525);
                mChipDetection.Drawline(BufAddress, (UInt32)picbuffer.Width, (UInt32)picbuffer.Height, 3000, 3000, 1525, 1700);


                picbuffer.Save("C:\\Users\\Administrator\\Desktop\\result.bmp", "-format bmp");

                //界面显示
                lbl1Val.Text = ((int)mChipDetection.TubeLeftX).ToString();
                lbl2Val.Text = ((int)mChipDetection.TubeLeftY).ToString();
                lbl3Val.Text = ((int)mChipDetection.TubeRightX).ToString();
                lbl4Val.Text = ((int)mChipDetection.TubeRightY).ToString();
              
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                LogHelper.WriteLog(typeof(FormChipDetection), ex);

            }
        }

        /// <summary>
        /// 窗口刷新核心
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PicImage_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            //if (3 == videoMode) 
            //{   
            //    //模板测试
            //    SolidBrush drawBrush = new SolidBrush(Color.Red);
            //    Font MyFont1 = new Font("宋体", 25, FontStyle.Bold);
            //    //模板阈值信息显示
            //    //g.DrawString(mChipDetection.chipThreVal.ToString(), MyFont1, drawBrush, 10, 10);
            //    //g.DrawString(mChipDetection.chipThreVal.ToString(), MyFont1, drawBrush, 10, 60);
            //}

            //if (1 == videoMode || 3 == videoMode)
            //{
            //    //绘制标定框
            //    mCurrentChipTemplate.Draw(g);
            //}
            //else if(2 == videoMode){
            //    //mChipDetection.Draw(g);
            //}
            
        }
       

        private void SaveCurrentImage()
        {
            //Bitmap bmp = (Bitmap)picImage.Image;
            //string fileName = mChipDetection.SummaryCount + "_" + mChipDetection.FailureCount + "_" + MiscFunction.GetInstance().TimeToFileFormatString(DateTime.Now, "_") + ".jpg";
            //bmp.Save(MiscFunction.GetInstance().GetScreenShotPath() + fileName, System.Drawing.Imaging.ImageFormat.Bmp);
        }


        private void chkCalibration_Click(object sender, EventArgs e)
        {
            grpParameter.Enabled = chkCalibration.Checked;
        }


        //拍照并处理音片
        private void Snap_Click(object sender, EventArgs e)
        {
            try
            {
                m_Xfer.Snap();
                Thread.Sleep(50);
                IntPtr dataBufAddress;
                m_Buffers.GetAddress(out dataBufAddress);
                mChipDetection.ProcessYinPian(dataBufAddress, (UInt32)m_Buffers.Width, (UInt32)m_Buffers.Height);

                m_Buffers.Save("C:\\Users\\Administrator\\Desktop\\yp_0517ori.bmp", "-format bmp");

                // m_Buffers
                mChipDetection.Drawline(dataBufAddress, (UInt32)m_Buffers.Width, (UInt32)m_Buffers.Height, mChipDetection.SheetLeftX, mChipDetection.SheetRightX,
                mChipDetection.SheetLeftY, mChipDetection.SheetRightY);

                m_Buffers.Save("C:\\Users\\Administrator\\Desktop\\yp_0517.bmp", "-format bmp");

                lbl1Val.Text = ((int)mChipDetection.SheetLeftX).ToString();
                lbl2Val.Text = ((int)mChipDetection.SheetLeftY).ToString();
                lbl3Val.Text = ((int)mChipDetection.SheetRightX).ToString();
                lbl4Val.Text = ((int)mChipDetection.SheetRightY).ToString();


                LogHelper.WriteLog(typeof(FormChipDetection), ((mChipDetection.SheetRightY - mChipDetection.SheetLeftY)/(mChipDetection.SheetRightX - mChipDetection.SheetLeftX)).ToString() +"  "+mChipDetection.SheetMiddleX.ToString()+"  "+mChipDetection.SheetMiddleY.ToString());


                sendCom_data("0A\r");

                lblStatusIndicator.Text = "正在采图";
                lblStatusIndicator.BackColor = Color.Green;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "错误");
                LogHelper.WriteLog(typeof(FormChipDetection), ex);

            }
        }

        //拍音筒
        private void Grab_Click(object sender, EventArgs e)
        {
       
            //生成并清除图像数据
            if (m_CurBuffers == null)
            {
                m_CurBuffers = new SapBuffer(1, m_Buffers.Width, m_Buffers.Height, SapFormat.Mono8, SapBuffer.MemoryType.Default);
                m_CurBuffers.Create();
                m_CurBuffers.Clear();
                m_CurBuffers.GetAddress(out curBufAddress);
            }
            else
            {
                m_CurBuffers.Clear();
            }

            if (m_BitMap == null)
            {
                nshowImageW = (int)m_CurBuffers.Width / nSmallerCof;
                nshowImageH = (int)m_CurBuffers.Height / nSmallerCof;

                m_BitMap = new Bitmap(nshowImageW, nshowImageH,
                    (System.Drawing.Imaging.PixelFormat)System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            }

            try
            {
                CAM_StartGrab();

	            lblStatusIndicator.Text = "正在采图";
	            lblStatusIndicator.BackColor = Color.Green;
	           
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                LogHelper.WriteLog(typeof(FormChipDetection), ex);

            }
        }

        //停止抓取并处理
        private void Freeze_Click(object sender, EventArgs e)
        {
            try
            {
                m_grabImages = false;
                m_Xfer.Freeze();

                m_CurBuffers.Save("C:\\Users\\Administrator\\Desktop\\when_freeze.bmp", "-format bmp");

                lblStatusIndicator.Text = "处理图像";

                Thread.Sleep(10);
                mChipDetection.InvertImage(curBufAddress, (UInt32)m_CurBuffers.Width, (UInt32)m_CurBuffers.Height);
                Thread.Sleep(10);
                mChipDetection.ProcessImage(curBufAddress, (UInt32)m_CurBuffers.Width, (UInt32)m_CurBuffers.Height);
                Thread.Sleep(10);
                //Drawline
                mChipDetection.Drawline(curBufAddress, (UInt32)m_CurBuffers.Width, (UInt32)m_CurBuffers.Height, mChipDetection.TubeLeftX, mChipDetection.TubeRightX,
                    mChipDetection.TubeLeftY, mChipDetection.TubeRightY);

                mChipDetection.Drawline(BufAddress, (UInt32)m_CurBuffers.Width, (UInt32)m_CurBuffers.Height, mChipDetection.TubeMiddleX, mChipDetection.TubeMiddleX, 0, 3000);

                mChipDetection.Drawline(BufAddress, (UInt32)m_CurBuffers.Width, (UInt32)m_CurBuffers.Height, 1250, 2850, 1525, 1700);
                mChipDetection.Drawline(BufAddress, (UInt32)m_CurBuffers.Width, (UInt32)m_CurBuffers.Height, 1250, 2850, 1525, 1525);
                mChipDetection.Drawline(BufAddress, (UInt32)m_CurBuffers.Width, (UInt32)m_CurBuffers.Height, 2850, 2850, 1525, 1700);

                m_CurBuffers.Save("C:\\Users\\Administrator\\Desktop\\result.bmp", "-format bmp");


                lbl1Val.Text = mChipDetection.TubeLeftX.ToString();
                lbl2Val.Text = mChipDetection.TubeLeftY.ToString();
                lbl3Val.Text = mChipDetection.TubeRightX.ToString();
                lbl4Val.Text = mChipDetection.TubeRightY.ToString();

                LogHelper.WriteLog(typeof(FormChipDetection), ((mChipDetection.TubeRightY - mChipDetection.TubeLeftY) / (mChipDetection.TubeRightX - mChipDetection.TubeLeftX)).ToString() + "  " + mChipDetection.TubeMiddleX.ToString() + "  " + mChipDetection.TubeMiddleY.ToString());


                lblStatusIndicator.Text = "正在计算";
                Thread.Sleep(50);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                LogHelper.WriteLog(typeof(FormChipDetection), ex);

            }
          

        }

        private void Calculate_Click(object sender, EventArgs e)
        {
            try
            {
                mChipDetection.Calculate();

                lbl1Val.Text = mChipDetection.OffsetX.ToString();
                lbl2Val.Text = mChipDetection.OffsetY.ToString();
                lbl3Val.Text = mChipDetection.OffsetL.ToString();
                lbl4Val.Text = mChipDetection.OffsetR.ToString();



                lblStatusIndicator.Text = "计算结果";
                lblStatusIndicator.BackColor = Color.Red;

                Send_Offset();

	            
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                LogHelper.WriteLog(typeof(FormChipDetection), ex);

            }
        }

        private void mnuUserLogin_Click(object sender, EventArgs e)
        {
            try
            {
                UserLogin(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "错误");
            }
        }


        private void btnStopMotor_Click(object sender, EventArgs e)
        {
            try
            {
                mStartTime = DateTime.Now;

                m_Xfer.Grab();
                m_grabImages = true;
                Thread.Sleep(20);
                for (int i = 1; i < 350; i++)
                {
                    if (m_grabImages == true)
                        m_Buffers.Save("E:\\samples\\0502\\yintong\\10\\" + i.ToString() + ".bmp", "-format bmp");
                    Thread.Sleep(10);
                }

                DateTime mEndTime = DateTime.Now;


                DateTime curtime = System.DateTime.Now;
                TimeSpan ts = TimeSpan.FromSeconds((int)curtime.Subtract(mStartTime).TotalSeconds);
                lbl4Val.Text = TimeSpan.Parse(ts.ToString()).ToString();

                m_Xfer.Freeze();
                m_grabImages = false;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "错误");
            }
        }

        private void mnuUserManager_Click(object sender, EventArgs e)
        {
            try
            {
                FormUserAuthority formUserAuthority = new FormUserAuthority();
                if (formUserAuthority.ShowDialog() == DialogResult.OK)
                {
                    ;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "错误");
            }
        }
        
        private void picImage_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void picImage_MouseMove(object sender, MouseEventArgs e)
        {
            lblDebug.Text = string.Empty;
            lblDebug.Text += e.X.ToString() + "      " + e.Y.ToString();
        }

        private void videorunToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CAM_StartGrab();
        }
        
        private void videostopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CAM_StopGrab();
        }

        //监测结果
        private void ChipDetection_RefreshDisplay()
        {
            ////////////////////////////////////显示统计结果//////////////////////////
           

            //DateTime curtime = System.DateTime.Now;
            //TimeSpan ts = TimeSpan.FromSeconds((int)curtime.Subtract(mStartTime).TotalSeconds);
            //lbl4Val.Text = TimeSpan.Parse(ts.ToString()).ToString();
      
            pnlResult.Update();
        }

        private void Mytest(object sender, EventArgs e)
        {
       
            //*********************************
            //**********反转图像测试***********
            //*********************************

            //********测试1********(halcon转C是否成功)
            //mChipDetection.InvertTest();
            

            //********测试2********(是否能传回图像)
            //string picpath = "E:\\samples\\samples0422\\yt01\\yt_0422_11.bmp";
            //SapBuffer picbuffer = null;
            //picbuffer = new SapBuffer(picpath, SapBuffer.MemoryType.Default);
            //picbuffer.Create();
            //picbuffer.Load(picpath, -1);
            //IntPtr BufAddress;
            //picbuffer.GetAddress(out BufAddress);
         
            //mChipDetection.InvertImage(BufAddress, (UInt32)picbuffer.Width, (UInt32)picbuffer.Height);
            //picbuffer.Save("C:\\Users\\Administrator\\Desktop\\yt_0422_11_invert.bmp", "-format bmp");
            

            //********测试3********(图片反转相加)
            //string picpath = "E:\\samples\\samples0422\\yt01\\2.bmp";
            //SapBuffer picbuffer = null;
            //picbuffer = new SapBuffer(picpath, SapBuffer.MemoryType.Default);
            //picbuffer.Create();
            //picbuffer.Load(picpath, -1);
            //IntPtr BufAddress;
            //picbuffer.GetAddress(out BufAddress);

            //string curpicpath = "E:\\samples\\samples0422\\yt01\\1.bmp";
            //SapBuffer curpicbuffer = null;
            //curpicbuffer = new SapBuffer(curpicpath, SapBuffer.MemoryType.Default);
            //curpicbuffer.Create();
            //curpicbuffer.Load(curpicpath, -1);
            //IntPtr curBufAddress;
            //curpicbuffer.GetAddress(out curBufAddress);

            //picbuffer.Save("C:\\Users\\Administrator\\Desktop\\csharppic.bmp", "-format bmp");
            //curpicbuffer.Save("C:\\Users\\Administrator\\Desktop\\csharpcurpic.bmp", "-format bmp");
            //int i =0;
            //while(i<1000)
            //{
            //    i++;
            //    mChipDetection.AddImage(BufAddress, curBufAddress, (UInt32)curpicbuffer.Width, (UInt32)curpicbuffer.Height);
            //}
            //curpicbuffer.Save("C:\\Users\\Administrator\\Desktop\\test.bmp", "-format bmp");


            //********测试4********（多幅图片相加并取回）
            //string curpicpath = "E:\\samples\\samples0422\\yt01\\dark.bmp";
            //SapBuffer curpicbuffer = null;
            //curpicbuffer = new SapBuffer(curpicpath, SapBuffer.MemoryType.Default);
            //curpicbuffer.Create();
            //curpicbuffer.Load(curpicpath, -1);

            //IntPtr curBufAddress;
            //curpicbuffer.GetAddress(out curBufAddress);

            //for (int i = 1; i <= 330; i++)
            //{
            //    string picpath = "E:\\samples\\0502\\yintong\\09\\" + i.ToString() + ".bmp";
            //    SapBuffer picbuffer = null;
            //    picbuffer = new SapBuffer(picpath, SapBuffer.MemoryType.Default);
            //    picbuffer.Create();
            //    picbuffer.Load(picpath, -1);

            //    IntPtr BufAddress;
            //    picbuffer.GetAddress(out BufAddress);
            //    //将图片反转并且相加
            //    mChipDetection.AddImage(BufAddress, curBufAddress, (UInt32)curpicbuffer.Width, (UInt32)curpicbuffer.Height);
            //    picbuffer.Destroy();
            //    picbuffer.Dispose();
            //}

            //curpicbuffer.Save("C:\\Users\\Administrator\\Desktop\\091d.bmp", "-format bmp");

            ////图像再反转
            //mChipDetection.InvertImage(curBufAddress, (UInt32)curpicbuffer.Width, (UInt32)curpicbuffer.Height);

            //curpicbuffer.Save("C:\\Users\\Administrator\\Desktop\\09.bmp", "-format bmp");


            //********测试5********（测一张叠加后的图片是否能得到两点坐标）
            string picpath = "E:\\samples\\0502\\yintong\\06_add\\06_8.bmp";
            SapBuffer picbuffer = null;
            picbuffer = new SapBuffer(picpath, SapBuffer.MemoryType.Default);
            picbuffer.Create();
            picbuffer.Load(picpath, -1);

            IntPtr BufAddress;
            picbuffer.GetAddress(out BufAddress);
            mChipDetection.ProcessImage(BufAddress, (UInt32)picbuffer.Width, (UInt32)picbuffer.Height);
            mChipDetection.Drawline(BufAddress, (UInt32)picbuffer.Width, (UInt32)picbuffer.Height, mChipDetection.TubeLeftX, mChipDetection.TubeRightX, mChipDetection.TubeLeftY, mChipDetection.TubeRightY);
            mChipDetection.Drawline(BufAddress, (UInt32)picbuffer.Width, (UInt32)picbuffer.Height, mChipDetection.TubeMiddleX, mChipDetection.TubeMiddleX, 0, 3000);

            picbuffer.Save("C:\\Users\\Administrator\\Desktop\\01_1.bmp", "-format bmp");
            lbl1Val.Text = mChipDetection.TubeLeftX.ToString();
            lbl2Val.Text = mChipDetection.TubeLeftY.ToString();
            lbl3Val.Text = mChipDetection.TubeRightX.ToString();
            lbl4Val.Text = mChipDetection.TubeRightY.ToString();

            //******************************//
            //***********整体测试***********//
            //******************************//

            //先读入目标图片curpicbuffer
            //string curpicpath = "E:\\samples\\samples0422\\yt01\\dark.bmp";
            //SapBuffer curpicbuffer = null;
            //curpicbuffer = new SapBuffer(curpicpath, SapBuffer.MemoryType.Default);
            //curpicbuffer.Create();
            //curpicbuffer.Load(curpicpath, -1);

            //IntPtr curBufAddress;
            //curpicbuffer.GetAddress(out curBufAddress);//获得一个指针
            ////捕捉到的图片反转后相加
            //for (int i = 10; i <= 100; i++)
            //{
            //    string picpath = "C:\\Users\\Administrator\\Desktop\\yt0428\\08\\" + i.ToString() + ".bmp";
            //    SapBuffer picbuffer = null;
            //    picbuffer = new SapBuffer(picpath, SapBuffer.MemoryType.Default);
            //    picbuffer.Create();
            //    picbuffer.Load(picpath, -1);

            //    IntPtr BufAddress;
            //    picbuffer.GetAddress(out BufAddress);
            //    //将图片反转并且相加
            //    mChipDetection.AddImage(BufAddress, curBufAddress, (UInt32)curpicbuffer.Width, (UInt32)curpicbuffer.Height);
            //    picbuffer.Destroy();
            //    picbuffer.Dispose();
            //}

            ////图像再反转
            //mChipDetection.InvertImage(curBufAddress, (UInt32)curpicbuffer.Width, (UInt32)curpicbuffer.Height);
            //curpicbuffer.Save("C:\\Users\\Administrator\\Desktop\\processed08.bmp", "-format bmp");            

            ////处理图像获得点的坐标
            //mChipDetection.ProcessImage(curBufAddress, (UInt32)curpicbuffer.Width, (UInt32)curpicbuffer.Height);
            
            ////划线
            //mChipDetection.Drawline(curBufAddress, (UInt32)curpicbuffer.Width, (UInt32)curpicbuffer.Height, mChipDetection.TubeLeftX, mChipDetection.TubeRightX, mChipDetection.TubeLeftY, mChipDetection.TubeRightY);
            //curpicbuffer.Save("C:\\Users\\Administrator\\Desktop\\drawlined08.bmp", "-format bmp");
            
            //lbl1Val.Text = ((int)mChipDetection.TubeLeftX).ToString();
            //lbl2Val.Text = ((int)mChipDetection.TubeLeftY).ToString();
            //lbl3Val.Text = ((int)mChipDetection.TubeRightX).ToString();
            //lbl4Val.Text = ((int)mChipDetection.TubeRightY).ToString();
        }

        ////模板特征提取测试
        private void templateTest()
        {
           
        }



    }

}
