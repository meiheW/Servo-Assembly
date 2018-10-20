using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

//using MiscFunctions;
using Timer = System.Timers.Timer;

namespace ChipDetection
{

    public partial class FormFunctionTest : Form
    {

     

        private Timer mTimer;
        private byte mDOStatus;
        private byte mDIStatus;
        private Label[] mDILabels;
        private CheckBox[] mDOChecks;

        private byte mIO1_8Status;
        private byte mIO9_16Status;

        private Label[] mDIOLabels;
        private CheckBox[] mDIOChecks;

        private DAQEvent mEvent;
        private byte mDAQIndex;
     

        public enum DAQEvent
        {
            NA,
            SetDO,
            SetDIO,
            SetPWMValue,
            SetMotorStart,
            SetMotorStop,
            SetPWMEnable,
            SetPWMDisable,
        }
 

        private void RefreshDIOStatus()
        {
            for (int i = 0; i < mDIOChecks.Length; i++)
            {
                mDIOChecks[i].Enabled = rdbIOOut.Checked;
            }
        }

        private void InitializeDODispaly()
        {
            byte index = 0x01;
            byte status = mDOStatus;
            for (int i = 0; i < mDOChecks.Length; i++)
            {
                if ((status & index) == index)
                {
                    mDOChecks[i].Checked = true;
                }
                else
                {
                    mDOChecks[i].Checked = false;
                }
                index = (byte)(index << 1);
                mDOChecks[i].Click += new EventHandler(chkDO_Click);
                mDOChecks[i].Tag = i;
            }
        }

        private void InitializeDIODispaly()
        {
            byte index = 0x01;
            byte status = mIO1_8Status;
            for (int i = 0; i < mDIOChecks.Length; i++)
            {
                if ((status & index) == index)
                {
                    mDIOChecks[i].Checked = true;
                }
                else
                {
                    mDIOChecks[i].Checked = false;
                }
                index = (byte)(index << 1);
                mDIOChecks[i].Click += new EventHandler(chkDIO_Click);
                mDIOChecks[i].Tag = i;
            }
        }

        private void InitializeTimers()
        {
            mTimer = new Timer();
            mTimer.Interval = 1000;
            mTimer.AutoReset = false;
            mTimer.Enabled = false;
            mTimer.SynchronizingObject = this;
            mTimer.Elapsed += new System.Timers.ElapsedEventHandler(mTimer_Elapsed);
        }

    


        private void chkDO_Click(object sender, EventArgs e)
        {
            try
            {
                CheckBox check = (CheckBox) sender;
                mDAQIndex = Convert.ToByte(check.Tag);
                mEvent=DAQEvent.SetDO;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void chkDIO_Click(object sender, EventArgs e)
        {
            try
            {
                CheckBox check = (CheckBox) sender;
                mDAQIndex = Convert.ToByte(check.Tag);
                mEvent = DAQEvent.SetDIO;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
   

        private void btnCameraStart_Click(object sender, EventArgs e)
        {
            try
            {
                //string returnString = mHIKCamera.Initialize();
                //if (!string.IsNullOrEmpty(returnString) && !returnString.Equals("Exist"))
                //{
                //    MessageBox.Show("相机初始化失败！" + returnString);
                //    return;
                //}
                //returnString = mHIKCamera.SetTriggerMode();
                //if (!string.IsNullOrEmpty(returnString))
                //{
                //    MessageBox.Show("设置相机触发模式！" + returnString);
                //    return;
                //}
                
                //returnString = mHIKCamera.StartGrabImage(pictureBoxTest.Handle, pictureBoxTest);
                //if (!string.IsNullOrEmpty(returnString))
                //{
                //    MessageBox.Show("启动图像获取功能失败！" + returnString);
                //    return;
                //}
                btnCameraStart.Enabled = false;
                btnCameraStop.Enabled = true;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void FormFunctionTest_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                //if (mHIKCamera != null)
                //{
                //    mHIKCamera.Close();
                //}
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());

            }
        }

        private void btnCameraStop_Click(object sender, EventArgs e)
        {
            try
            {
                btnCameraStart.Enabled = true;
                btnCameraStop.Enabled = false;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());

            }
        }
        private void chkPWM_Click(object sender, EventArgs e)
        {
            if (chkPWM.Checked)
            {
                mEvent = DAQEvent.SetPWMEnable;
            }
            else
            {
                mEvent = DAQEvent.SetPWMDisable;
            }

        }

        private void btnMotorStart_Click(object sender, EventArgs e)
        {
            mEvent = DAQEvent.SetMotorStart;
        }

        private void btnMotorStop_Click(object sender, EventArgs e)
        {
            mEvent = DAQEvent.SetMotorStop;
        }
    }
}
