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
using Microsoft.Win32;
using MiscFunctions;
using FlyCapture2Managed;
using DALSA.SaperaLT.SapClassBasic;
using Timer = System.Timers.Timer;
using System.Diagnostics;


namespace ChipDetection
{
  
    public partial class FormChipDetection : Form
    {
    
        private UserAuthority mUserAuthority;
        private Configuration mConfiguration;           
        private AccessExcel mAccessExcel;
        private ChipDetection mChipDetection;
        private Timer mInitializationTimer;
        private DateTime mStartTime;  
        private string mExcelModalFileName;
        private string mDestinationExcelFile;
        private bool mIsResetConfiguration;
        private string mOperationID;
        private string mDeviceName;
        private ManagedImage m_rawImage;
        private ManagedImage m_processedImage;
        private SapAcqDevice AcqDevice = null;
        private SapBuffer Buffers = null;
        private SapTransfer Xfer = null;
        private SapLocation loc = null; 
        private bool m_grabImages;
        private AutoResetEvent m_grabThreadExited;
        private BackgroundWorker m_grabThread;
        private ManagedCameraBase m_camera = null;
        private  int videoMode = 0; //0 camera test //1 标定 //2 芯片检测 //3 整体联动


        public FormChipDetection(bool isResetConfiguration)
        {

            mIsResetConfiguration = isResetConfiguration;
            InitializeComponent();//控件及文本

            Parameter.GetInstance().LoadOptionFile();//参数option.ini（改参数在这里）
            mConfiguration = new Configuration(isResetConfiguration);//配置

            if (isResetConfiguration)//用户配置
            {
                UserConfiguration.GetInstance().CreateDefualtUserConfigurationFile();
            }
            else
            {
                UserConfiguration.GetInstance().LoadUserConfigurationFile();
            }
            mChipDetection = new ChipDetection();//新的ChipDetection


            lblMark.BackColor = Color.Transparent;//是那个上面的3999
            Form.CheckForIllegalCrossThreadCalls = false;
            
            //显示窗口刷新
            picImage.Paint += new PaintEventHandler(PicImage_Paint);//函数PicImage_Paint
            //mExcelModalFileName = MiscFunction.GetInstance().GetAssemblyPath() + Parameter.GetInstance().ExcelFileName + ".xlsx";//EXCEL名称

            InitializeTimers();//定时器
            UserLogin(false);//用户登录

           
            m_grabThreadExited = new AutoResetEvent(false);
            CAM_connect();//相机连接
         
            ResetTestStatus();//界面参数全都初始化

        }


        public bool CAM_connect()
        {
            try
            {
                //server名称
                int ServerCount = SapManager.GetServerCount();
                int ServerIndex = ServerCount - 1;
                string ServerName = SapManager.GetServerName(ServerIndex);
                //设备Index
                int CameraCount = SapManager.GetResourceCount(ServerName,SapManager.ResourceType.AcqDevice);
                int ResourceIndex = CameraCount - 1;
                //if (CameraCount == 0) MessageBox.Show("没有相机。");

                loc = new SapLocation(ServerName,ResourceIndex);

                //配置文件
                string ConfigFileName;
                string ConfigPath =  Environment.GetEnvironmentVariable("SAPERADIR")+"\\camFiles\\User\\";
                string[] ccffiles = Directory.GetFiles(ConfigPath,"*.ccf");
                int configFileCount = ccffiles.Length;
                ConfigFileName = ccffiles[0];
                //创建对象
                if(SapManager.GetResourceCount(ServerName,SapManager.ResourceType.AcqDevice)>0)
                {
                    AcqDevice = new SapAcqDevice(loc,ConfigFileName);
                    Buffers   = new SapBuffer(1,AcqDevice,SapBuffer.MemoryType.ScatterGather);
                    Xfer      = new SapAcqDeviceToBuf(AcqDevice,Buffers);	
                    if(!AcqDevice.Create()){return false;}
                }

                if(!Buffers.Create()){return false;}
                if(!Xfer.Create()){return false;}


                ////ManagedBusManager busMgr = new ManagedBusManager();
                ////uint numCameras = busMgr.GetNumOfCameras();
                ////ManagedPGRGuid guid = busMgr.GetCameraFromIndex(0);
                ////m_camera = new ManagedCamera();
                ////m_camera.Connect(guid);
                //m_camera.IsConnected()
                //CameraInfo camInfo = m_camera.GetCameraInfo();
            }
            catch (FC2Exception ex)//要修改
            {
                MessageBox.Show("相机连接失败。");
                MessageBox.Show("Failed to load form successfully: " + ex.Message);
                return false;
            }
            return true;
        }

        private void CAM_StartGrab()//抓取图像
        {
            Xfer.Grab();
            //m_camera.StartCapture();//待修改
            m_grabImages = true;          //相机正在抓取图像的标志
            m_grabThread = new BackgroundWorker();
            m_grabThread.ProgressChanged += new ProgressChangedEventHandler(UpdateUI);
            m_grabThread.DoWork += new DoWorkEventHandler(GrabLoop);
            m_grabThread.WorkerReportsProgress = true;
            m_grabThread.RunWorkerAsync();             
        }

        private void GrabLoop(object sender, DoWorkEventArgs e)//循环抓取图像并处理
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            while (m_grabImages)
            {
                try
                {
                    //m_camera.RetrieveBuffer(m_rawImage);
                }
                catch (FC2Exception ex)
                {
                    Debug.WriteLine("Error: " + ex.Message);
                    continue;
                }

                lock (this)
                {
                    //cvProcess(m_rawImage);
                    //m_rawImage.Convert(FlyCapture2Managed.PixelFormat.PixelFormatBgr, m_processedImage);  
                    
                    //Buffers.Save("E:\\pic\\1.bmp", "-format bmp");
                    cvProcess(Buffers);
                    ////Buffers.ColorConvert
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
                //m_camera.StopCapture();
                Xfer.Freeze();
                if (Xfer != null)
                {
                    Xfer.Destroy();
                    Xfer.Dispose();
                }

                if (AcqDevice != null)
                {
                    AcqDevice.Destroy();
                    AcqDevice.Dispose();
                }

                if (Buffers != null)
                {
                    Buffers.Destroy();
                    Buffers.Dispose();
                }
                
                loc.Dispose();  
            
            }
            catch (FC2Exception ex)
            {
                //MessageBox.Show("Failed to stop camera: " + ex.Message);
                return string.Empty;
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Camera is null");
            }

            return string.Empty;
        }
       
        public void CAM_Close()
        {
            try
            {
                Xfer.Freeze();
                if (Xfer != null)
                {
                    Xfer.Destroy();
                    Xfer.Dispose();
                }

                if (AcqDevice != null)
                {
                    AcqDevice.Destroy();
                    AcqDevice.Dispose();
                }

                if (Buffers != null)
                {
                    Buffers.Destroy();
                    Buffers.Dispose();
                }

                loc.Dispose();  
                //m_camera.Disconnect();
            }
            catch (FC2Exception ex)
            {
                // Nothing to do here
            }
            catch (NullReferenceException ex)
            {
                // Nothing to do here
            }
        }

        private void cvProcess(SapBuffer buffer)//修改
        {
           
                unsafe
                {

                    IntPtr dataBufAddress;
                    
                    buffer.GetAddress(out dataBufAddress);

                    //IntPtr imgdata = new IntPtr(img.data);
                    if (2 == videoMode)
                        mChipDetection.ProcessImage(dataBufAddress, (UInt32)buffer.Width, (UInt32)buffer.Height);
                    //img.data
                }

        }

        private void UpdateUI(object sender, ProgressChangedEventArgs e)
        {       
               
            ////picImage.Image = m_processedImage.bitmap;
            //if (2 == videoMode)
            //{
            //    //绘图
            //    Graphics g = picImage.CreateGraphics();
            //    mChipDetection.Draw(g);//在窗口中绘制信息，0，1，阈值
            //}
            picImage.Invalidate();
            ChipDetection_RefreshDisplay();//显示统计结果

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
                    // waitDialog.Caption += "失败" + "\r\n" + ex.Message;
                    errorString += "初始化相机失败!" + "\r\n";
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
            lblshowcalib.Text = "0";
            lblwordx.Text = "0";
            lblwordy.Text = "0";
            lblDetectionTime.Text = "00:00:00";
       
            btnStart.Enabled = true;
            btnStop.Enabled = false;
            btnTest.Enabled = true;

            btnStartCalibration.Enabled = true;
            btnSaveCalibration.Enabled = false;
            btnStopCalibration.Enabled = false;

            btnStartMotor.Enabled = true;
            btnStopMotor.Enabled = false;
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
            catch (System.Exception ex)
            {
                System.Console.Out.WriteLine(ex.Message);
                return 2;
            }
            return 0;
        }

        private void InitializeTimers()
        {
            mInitializationTimer = new Timer();//在应用程序中生成定期事件
            mInitializationTimer.Interval = 20;
            mInitializationTimer.AutoReset = false;
            mInitializationTimer.Enabled = false;
            mInitializationTimer.SynchronizingObject = this;
            mInitializationTimer.Elapsed += new System.Timers.ElapsedEventHandler(mInitializationTimer_Elapsed);

        }

        //启动按键，开始作业
        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                bool isGetCode = false;
	            FormCode formCode = new FormCode();//FormCode用于获取应用中的信息
                if (formCode.ShowDialog() == DialogResult.OK)
                {
                    isGetCode = true;
                }
                if (!isGetCode && !chkCalibration.Checked)
                {
                    return;
                }
                
                if (Parameter.GetInstance().IsSaveExcel && !chkCalibration.Checked)
                {
                    //if (!File.Exists(mExcelModalFileName))
                    //{
                    //    MessageBox.Show("无法找到Map模板文件！" + mExcelModalFileName);
                    //    return;
                    //}
                    //Application.DoEvents();
                    //mAccessExcel = AccessExcel.CreateInstance();
                    //mDestinationExcelFile = MiscFunction.GetInstance().GetAssemblyPath() + Parameter.GetInstance().ReportDirectory + "\\" + "temp.xlsx";
                    //try
                    //{
                    //    File.Copy(mExcelModalFileName, mDestinationExcelFile, true);
                    //}
                    //catch
                    //{
                    //    MessageBox.Show("Excel模板复制失败");
                    //    return;
                    //}
                    //Application.DoEvents();
                    //string strinfo = mAccessExcel.OpenExcelFile(mDestinationExcelFile);
                    //if (!string.IsNullOrEmpty(strinfo))
                    //{
                    //    MessageBox.Show("Excel打开失败");
                    //    return;
                    //}
                }
                
               // Graphics g = picImage.CreateGraphics();
               // g.Clear(Color.White);
                
                ResetTestStatus();//界面上控件的初始值
             
                //检测结果重置
                mChipDetection.ResetDectionStatus();
        
                
                lblCodeValue.Text = formCode.GetCodeString();
                mOperationID = formCode.GetOperationIDString();
                mDeviceName = formCode.GetDeviceNameString();

                videoMode = 2;
                CAM_StartGrab();


                btnStop.Enabled = true;
                btnTest.Enabled = false;
                btnStart.Enabled = false;
                grpMotor.Enabled = false;
                mnuMain.Enabled = false;

                btnStartCalibration.Enabled = false;
                btnSaveCalibration.Enabled = false;
                btnStopCalibration.Enabled = false;
        
                lblStatusIndicator.Text = "正在检测";
                lblStatusIndicator.BackColor = Color.Green;
                mStartTime = DateTime.Now;//用于计时
             }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }


        private void FormChipDetection_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (mAccessExcel!=null)
                {
                    mAccessExcel.Close();
                }
                Parameter.GetInstance().SaveOptionFile();

                CAM_Close();
                
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());

            }
        }

        //保存测试结果文件
        private void SaveResultToExcelFile()
        {


            ///mAccessExcel.AddChipStatus();

            //mAccessExcel.SaveDetectionResult(  lblCodeValue.Text, mChipDetection.PassCount,mChipDetection.FailureCount,
            //                                                        mChipDetection.SummaryCount, mDeviceName, mOperationID);

            //mAccessExcel.SaveExcelFile(mDestinationExcelFile);


            //string createTime = MiscFunction.GetInstance().TimeToFileFormatString(DateTime.Now, "");
            //mAccessExcel.Close();

            //Application.DoEvents();
            //Thread.Sleep(500);

            //FileInfo destinationExcelfileInfo = new FileInfo(mDestinationExcelFile);
            //string finalExcelFile = MiscFunction.GetInstance().GetAssemblyPath() + Parameter.GetInstance().ReportDirectory + "\\" + lblCodeValue.Text + ".xlsx";

            //if (File.Exists(finalExcelFile))
            //{
            //    FileInfo finalExcelFileInfo = new FileInfo(finalExcelFile);
            //    string fileName = Path.GetFileNameWithoutExtension(finalExcelFile);
            //    string newFileName = MiscFunction.GetInstance().GetAssemblyPath() + Parameter.GetInstance().ReportDirectory + "\\" + fileName + "_" + createTime + ".xlsx";
            //    //File.Copy(finalExcelFile, newFileName, true);
            //    finalExcelFileInfo.MoveTo(newFileName);
            //    if (Parameter.GetInstance().IsSaveRemoteFile)
            //    {
            //        string remoteNewFile = @Parameter.GetInstance().RemoteFilePath + fileName + "_" + createTime + ".xlsx";
            //        File.Copy(newFileName, remoteNewFile, true);
            //    }
            //}
            //destinationExcelfileInfo.MoveTo(finalExcelFile);
            //if (Parameter.GetInstance().IsSaveRemoteFile)
            //{
            //    string remoteFile = @Parameter.GetInstance().RemoteFilePath + lblCodeValue.Text + ".xlsx";
            //    File.Copy(finalExcelFile, remoteFile, true);
            //}

        }
        //停止按键，停止作业
        private void btnStop_Click(object sender, EventArgs e)
        {
            try
            {
                
                
                if ((!chkCalibration.Checked && Parameter.GetInstance().IsControlMotor))
                {
                      
                }
        
                CAM_StopGrab();

                if (Parameter.GetInstance().IsSaveExcel && !chkCalibration.Checked)
                {
                    try
                    {
                        mAccessExcel.SetExcelVisible();
                        
                        //mAccessExcel.AddChipStatus(i, mChipDetection.mresultlist[i].status);//excel保存数据

                    }
                    catch {
                        MessageBox.Show("Excel关闭失败,请手动关闭。");
                    }

                }

                btnStart.Enabled = true;
                btnTest.Enabled = true;
                btnStop.Enabled = false;
                grpMotor.Enabled = true;
                mnuMain.Enabled = true;
                btnStartCalibration.Enabled = true;
                btnSaveCalibration.Enabled = false;
                btnStopCalibration.Enabled = false;

                lblStatusIndicator.Text = "停止运行";
                lblStatusIndicator.BackColor = Color.Red;
            }
            catch (System.Exception ex)
            {
                //MessageBox.Show(ex.ToString());
            }
        }

      
        //button1，Excel测试按键
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                //Excel测试
                mAccessExcel = AccessExcel.CreateInstance();
                mDestinationExcelFile = MiscFunction.GetInstance().GetAssemblyPath() + Parameter.GetInstance().ReportDirectory + "\\" + "temp.xlsx";
                File.Copy(mExcelModalFileName, mDestinationExcelFile, true);
                Application.DoEvents();
                string ret = mAccessExcel.OpenExcelFile(mDestinationExcelFile);

                mAccessExcel.SetExcelVisible();
                for (int i = 0; i < 1500; i++)
                {
                    mAccessExcel.AddChipStatus(i, 1);
                }
                SaveResultToExcelFile();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
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
        //启动标定
        private void btnStartCalibration_Click(object sender, EventArgs e)
        {
            
            videoMode = 1;
            try
            {
                //CAM_StartGrab();
                //***********界面显示
                string filename = "C:/Users/Administrator/Desktop/4_2/calibration-4-1/scratch_perspective";
                Double result = mChipDetection.Calibration(filename);
                lblshowcalib.Text = result.ToString();

                //*************这一段用以测试Buffer
                //string filename = "E:\\pic\\mono8.bmp";
                //SapBuffer testbuffer = null;
                //testbuffer = new SapBuffer(filename, SapBuffer.MemoryType.Default);
                //testbuffer.Create();
                //testbuffer.Load(filename, -1);

                //IntPtr dataBufAddress;
                //testbuffer.GetAddress(out dataBufAddress);

                //mChipDetection.BufferTest(dataBufAddress, testbuffer.Width, testbuffer.Height);

                //*************这一段测试opencv
                //mChipDetection.Processtest();

                lblCodeValue.Text = "标定模式";
                btnStop.Enabled = false;
                btnTest.Enabled = true;
                btnStart.Enabled = false;
                grpMotor.Enabled = false;
                mnuMain.Enabled = false;
                btnStartCalibration.Enabled = false;
                btnSaveCalibration.Enabled = true;
                btnStopCalibration.Enabled = true;
                //lblStatusIndicator.Text = "正在标定";
                lblStatusIndicator.Text = "标定成功";
                lblStatusIndicator.BackColor = Color.Green;

                //Bitmap modelPic = new Bitmap(mCurrentChipTemplate.PhotoFile);
	            //picImage.Image = modelPic;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnStopCalibration_Click(object sender, EventArgs e)
        {

            //CAM_StopGrab();
            btnStart.Enabled = true;
            btnTest.Enabled = true;
            btnStop.Enabled = false;
            btnStartCalibration.Enabled = true;
            btnSaveCalibration.Enabled = false;
            btnStopCalibration.Enabled = false;
            grpMotor.Enabled = true;
            mnuMain.Enabled = true;
            lblStatusIndicator.Text = "停止运行";
            lblStatusIndicator.BackColor = Color.Red;
        }

        private void btnSaveCalibration_Click(object sender, EventArgs e)//保存标定结果，mCurrentChipTemplate，templateFile，bmp
        {
            try
            {
	           
	            
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnStartMotor_Click(object sender, EventArgs e)
        {
            try
            {
                //相机画面显示
                videoMode = 0;
                //CAM_StartGrab();

                //电机运行标志
                btnStartMotor.Enabled = false;
                btnStopMotor.Enabled = true;

                btnStop.Enabled = false;
                btnTest.Enabled = false;
                btnStart.Enabled = false;
                mnuMain.Enabled = false;

                btnStartCalibration.Enabled = false;
                btnSaveCalibration.Enabled = false;
                btnStopCalibration.Enabled = false;
   
                lblStatusIndicator.Text = "正在处理";
                lblStatusIndicator.BackColor = Color.Green;
               
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString(), "错误");
            }
        }

        private void btnStopMotor_Click(object sender, EventArgs e)
        {
            try
            {

                CAM_StopGrab();

	            btnStart.Enabled = true;
	            btnTest.Enabled = true;
	            btnStop.Enabled = false;
	            mnuMain.Enabled = true;
	
	            btnStartCalibration.Enabled = true;
	            btnSaveCalibration.Enabled = false;
	            btnStopCalibration.Enabled = false;
	
	            btnStartMotor.Enabled = true;
	            btnStopMotor.Enabled = false;
	
	            lblStatusIndicator.Text = "停止运行";
	            lblStatusIndicator.BackColor = Color.Red;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString(), "错误");
            }
        }
        
        private void mnuUserLogin_Click(object sender, EventArgs e)
        {
            try
            {
                UserLogin(true);
            }
            catch (System.Exception ex)
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
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString(), "错误");
            }
        }
       
        private void picImage_MouseDown(object sender, MouseEventArgs e)
        {
            //if (1 == videoMode)
            //{

            //    mCurrentChipTemplate.MouseDown(e);
            //}

        }

        private void picImage_MouseMove(object sender, MouseEventArgs e)
        {
            //if (1 == videoMode)
            //{

            //    mCurrentChipTemplate.MouseMove(e);
            //    picImage.Invalidate();
            //}

            lblDebug.Text = string.Empty;
            lblDebug.Text += e.X.ToString() + "      " + e.Y.ToString();
        }

        private void picImage_MouseUp(object sender, MouseEventArgs e)
        {
            //if (1 == videoMode)
            //{
            //    mCurrentChipTemplate.MouseUp(e);
            //}

        }

        private void picImage_Click(object sender, EventArgs e)
        {

        }
        
        private void videorunToolStripMenuItem_Click(object sender, EventArgs e)
        {
            videoMode = 2;
            CAM_StartGrab();
        }
        
        private void videostopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CAM_StopGrab();
        }

        //监测结果坐标位置及运行时间
        private void ChipDetection_RefreshDisplay()
        {

            ////////////////////////////////////显示统计结果//////////////////////////
            lblwordx.Text = mChipDetection.realworldx.ToString();
            lblwordy.Text = mChipDetection.realworldy.ToString();
            lblshowcalib.Text = mChipDetection.gearnum.ToString();
            //lblSummary.Text = mChipDetection.SummaryCount.ToString();
            //lblCalibrationStatus.Text = mTestLineRate.ToString("0.000") + "\r\n" + mCurrentMotorRate.ToString("0.00");

            //if (2 == videoMode)
            //{

            //    if (mChipDetection.Detectfinished())
            //    {
            //        //结束测试
            //        btnStop.PerformClick();//电机停止就在这里
            //    }
            //    DateTime curtime = System.DateTime.Now;
            //    TimeSpan ts = TimeSpan.FromSeconds((int)curtime.Subtract(mStartTime).TotalSeconds);
            //    lblDetectionTime.Text = TimeSpan.Parse(ts.ToString()).ToString();
                
            //}
            DateTime curtime = System.DateTime.Now;
            TimeSpan ts = TimeSpan.FromSeconds((int)curtime.Subtract(mStartTime).TotalSeconds);
            lblDetectionTime.Text = TimeSpan.Parse(ts.ToString()).ToString();
            pnlResult.Update();
        }


        private void button2_Click(object sender, EventArgs e)
        {
            videoMode = 3;
            templateTest();
        }

        ////模板特征提取测试
        private void templateTest()
        {
            //string templateDirectoryName = MiscFunction.GetInstance().GetAssemblyPath();
            //if (!templateDirectoryName.Equals("(默认模板)"))
            //{
            //    string templateFileName = templateDirectoryName + "template.xml";
            //    if (File.Exists(templateFileName))
            //    {
            //        mCurrentChipTemplate = (ChipTemplate)MiscFunction.GetInstance().DeSerializeXMLFile(templateFileName, typeof(ChipTemplate));
            //        Bitmap modelPic = new Bitmap(mCurrentChipTemplate.PhotoFile);
            //        picImage.Image = modelPic;

            //        mChipDetection.CreateChipTemplate(mCurrentChipTemplate);
            //        picImage.Invalidate();
            //    }
            //    else {
            //        MessageBox.Show("无法找到模板文件");
            //        return;
            //    }
            //}
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            //************整个的处理******************//
            string picpath = "E:\\pic\\7.jpg";
            SapBuffer picbuffer = null;
            picbuffer = new SapBuffer(picpath, SapBuffer.MemoryType.Default);
            picbuffer.Create();
            picbuffer.Load(picpath, -1);

            IntPtr BufAddress;
            picbuffer.GetAddress(out BufAddress);
            mChipDetection.ProcessImage(BufAddress, (uint)picbuffer.Width, (uint)picbuffer.Height);

            lblwordx.Text = mChipDetection.realworldx.ToString();
            lblwordy.Text = mChipDetection.realworldy.ToString();
        }



    }


}
