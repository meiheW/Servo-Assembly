using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;
using Application = System.Windows.Forms.Application;

namespace ChipDetection
{
    public class AccessExcel
    {
        private static AccessExcel mSingletonAccessExcel;
        private const int SW_NORMAL = 1;
        private ApplicationClass mExcel;
        private Workbook mWorkbook;
        private Worksheet mWorksheet;         
        private int startCellRow;
        private int startCellCol;
        public AccessExcel()
        {
            mExcel = new ApplicationClass();
            mExcel.DisplayAlerts = false;
            mExcel.AlertBeforeOverwriting = false;
        }
        public static AccessExcel CreateInstance()
        {
            if ( null == mSingletonAccessExcel)
            {
                mSingletonAccessExcel = new AccessExcel();
            }
            return mSingletonAccessExcel;
        }
        public string OpenExcelFile(string fileName)
        {
            try
            {
	            mWorkbook = mExcel.Workbooks.Open(fileName, 
                                                         0, false, 5, System.Reflection.Missing.Value,
	                                                     System.Reflection.Missing.Value, false,
	                                                     System.Reflection.Missing.Value,
	                                                     System.Reflection.Missing.Value, true, false,
	                                                     System.Reflection.Missing.Value, false, false, false);
	            mExcel.Visible = false;
                startCellRow = Parameter.GetInstance().StartCellRow;
                startCellCol = Parameter.GetInstance().StartCellCol;
                mWorksheet = (Worksheet)mWorkbook.Worksheets[1];
	            return string.Empty;
            }
            catch (System.Exception ex)
            {
                return ex.ToString();
            }
        }

        public void SetExcelVisible() {
            mExcel.Visible = true;
        }

        public void SaveDetectionResult(string code,int passCount,int failCount,int summaryCount,string deviceName,string operationID)
        {

            double passRate = 0;

            if (summaryCount > 0)
            {
               passRate=(double)passCount / summaryCount;
            }
    
            mWorksheet.Cells[7, 3] = summaryCount.ToString();
            mWorksheet.Cells[8, 3] = passCount.ToString();
            mWorksheet.Cells[9, 3] = passRate.ToString();
            mWorksheet.Cells[10, 3] = failCount.ToString();
            mWorksheet.Cells[4, 3] = code;
            mWorksheet.Cells[3, 3] = deviceName;
            mWorksheet.Cells[5, 3] = operationID;

        }
        public void AddChipStatus(int chipIndex, int isPass)
        {
            int myrow=0;
            int mycol=0;
            int myspace=0;
            string mylabel = "";
            if (1 == isPass)
            {
                mylabel = Parameter.GetInstance().PassLabel;

            }
            else if (0 == isPass)
            {
                mylabel = Parameter.GetInstance().FailLabel;
            }    

            myrow = chipIndex / Parameter.GetInstance().RowCells;
            mycol = chipIndex % Parameter.GetInstance().RowCells;

            myspace = (mycol / Parameter.GetInstance().SpaceInterval) * Parameter.GetInstance().SpaceLength;

            if (mycol == 0)
            {
                mWorksheet.Cells[myrow + startCellRow, startCellCol - 3] = chipIndex.ToString();
                mWorksheet.Cells[myrow + startCellRow, startCellCol - 1] = "|";
            }
            mWorksheet.get_Range(mWorksheet.Cells[myrow + startCellRow, startCellCol + mycol + myspace ],
                                 mWorksheet.Cells[myrow + startCellRow, startCellCol + mycol + myspace]).Interior.ColorIndex = isPass == 1 ? 0 : 8;
            mWorksheet.Cells[myrow + startCellRow, startCellCol + mycol + myspace] = mylabel;  

        }

        public string SaveExcelFile(string fileName)
        {
            try
            {
                mWorkbook.SaveAs(fileName, System.Reflection.Missing.Value, System.Reflection.Missing.Value,
                    System.Reflection.Missing.Value, System.Reflection.Missing.Value,
                    System.Reflection.Missing.Value, XlSaveAsAccessMode.xlNoChange,
                    System.Reflection.Missing.Value, System.Reflection.Missing.Value,
                    System.Reflection.Missing.Value, System.Reflection.Missing.Value,
                    System.Reflection.Missing.Value);
                return string.Empty;
            }
            catch (System.Exception ex)
            {
                return ex.ToString();
            }
        }

        public bool Close()
        {
            if (mExcel != null)
            {
                mExcel.Quit();
            }
            return true;
        }
         
    }
}
