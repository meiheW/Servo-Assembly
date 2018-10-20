using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;

namespace test
{
    public partial class Form1 : Form
    {

        private SerialPort mCom_ = new SerialPort();

        public Form1()
        {
            InitializeComponent();      
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (mCom_.IsOpen) return;
            try
            {
                mCom_.PortName = "COM6";
                mCom_.BaudRate=9600;
                mCom_.Parity = Parity.None;
                mCom_.DataBits=8;
                mCom_.StopBits=StopBits.Two;
                mCom_.RtsEnable = true;
                mCom_.DtrEnable = true;
                mCom_.ReadTimeout = 1000;
                mCom_.ReceivedBytesThreshold = 1;
                mCom_.Open();
                mCom_.DataReceived +=  new SerialDataReceivedEventHandler(serialPort_DataReceived);  // 响应函数
            }
            catch (Exception except )
            {

                MessageBox.Show(except.ToString());
            }
        }

        private void sendCom_data( string str)
        {
            try
            {
                if (mCom_.IsOpen)
                {
                    mCom_.Write(str);
                }
            }
            catch (Exception except)
            {

                MessageBox.Show(except.ToString());
            }

        }
        
        private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //button3.PerformClick();
            // 判断缓存中是否有数据
            int readsize = mCom_.BytesToRead;
            if (readsize > 0)
            {
                // 读数据
                byte[] recvbyte = new byte[readsize];
                mCom_.Read(recvbyte, 0, readsize);
                String strrecv = Encoding.ASCII.GetString(recvbyte);
                //String strrecv =mCom_.ReadExisting();
                //String strrecv =  mCom_.ReadLine();               
     
                //if (strrecv.IndexOf("LV") == 0)
                //{
                //    MessageBox.Show("start!");
                //}
                //if (strrecv.Contains("01"))
                //{
                    MessageBox.Show("OK!");
                
                //}

            }


        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (mCom_.IsOpen)
            {
                mCom_.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            sendCom_data(textBox1.Text);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int x1=88;
            int x2=1;
            int x3=-1;
            int x4=888;

            int x1o=(x1>=0)?0:1;
            int x2o=(x2>=0)?0:1;
            int x3o=(x3>=0)?0:1;

           String s= String.Format("0C{0:d1}{1:d4}{2:d1}{3:d4}{4:d1}{5:d4}{6:d4}\r",
                x1o,Math.Abs( x1),
                x2o,Math.Abs(x2),
                x3o,Math.Abs(x3),
                Math.Abs(x4) );

           sendCom_data(s);
        }


        //SerialPort.DiscardInBuffer
        //SerialPort.DiscardOutBuffer


    }
}
