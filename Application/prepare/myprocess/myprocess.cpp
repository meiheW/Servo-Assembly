// myprocess.cpp : 定义 DLL 应用程序的导出函数。
//
#include "stdafx.h"
#include "myprocess.h"
#include <HalconC.h>
#include<iostream>
#include<opencv2/opencv.hpp>
#include<opencv2/core/core.hpp>
#include<opencv2/highgui/highgui.hpp>
#include<opencv2/imgproc/imgproc.hpp>
#include<opencv2/imgproc/types_c.h>
#include<vector>
using namespace std;
using namespace cv;

Htuple RealPara,RealPose;

void calib(char* filename,Htuple &Para,Htuple &Pose)//要引用的
{
	/* Stack for temporary tuples */
  Htuple   TTemp[100];
  int      SP=0;
  /* Stack for temporary objects */
  Hobject  OTemp[100] = {0};
  int      SPO=0;
  /* Stack for temporary tuple vectors */
  Hvector  TVTemp[100] = {0};
  int      SPTV=0;
  /* Stack for temporary object vectors */
  Hvector  OVTemp[100] = {0};
  int      SPOV=0;

  /* Local iconic variables */
  Hobject  ho_Image, ho_ImageCalib, ho_Contours;

  /* Local control variables */
  Htuple  hv_filename, hv_Width, hv_Height, hv_StartCamPar;
  Htuple  hv_CaltabName, hv_CalibDataID, hv_NumImages, hv_I;
  Htuple  hv_Row, hv_Column, hv_Index, hv_StartPose, hv_Error;
  Htuple  hv_CamPara, hv_PoseCalib;

  /* Initialize iconic variables */
  gen_empty_obj(&ho_Image);
  gen_empty_obj(&ho_ImageCalib);
  gen_empty_obj(&ho_Contours);

  /* Initialize control variables */
  create_tuple(&hv_filename,0);
  create_tuple(&hv_Width,0);
  create_tuple(&hv_Height,0);
  create_tuple(&hv_StartCamPar,0);
  create_tuple(&hv_CaltabName,0);
  create_tuple(&hv_CalibDataID,0);
  create_tuple(&hv_NumImages,0);
  create_tuple(&hv_I,0);
  create_tuple(&hv_Row,0);
  create_tuple(&hv_Column,0);
  create_tuple(&hv_Index,0);
  create_tuple(&hv_StartPose,0);
  create_tuple(&hv_Error,0);
  create_tuple(&hv_CamPara,0);
  create_tuple(&hv_PoseCalib,0);

  /****************************************************/
  /******************   Begin procedure   *************/
  /****************************************************/

  /***读取图像获得宽高*** NOTE：目录*/
  /*filename := 'C:/Users/Administrator/Desktop/4_2/calibration-4-1/scratch_perspective'*/
  reuse_tuple_s(&hv_filename,filename);
  /*read_image (Image, filename)*/
  clear_obj(ho_Image);
  /***/T_read_image(&ho_Image, hv_filename);

  /*get_image_size (Image, Width, Height)*/
  destroy_tuple(hv_Width);
  destroy_tuple(hv_Height);
  /***/T_get_image_size(ho_Image, &hv_Width, &hv_Height);


  /*******标定相机*******  NOTE: 标定类型，相机参数，面阵点阵，标定板规格*/
  /*StartCamPar := [0.012,0,0.0000055,0.0000055,Width/2,Height/2,Width,Height]*/
  create_tuple(&TTemp[SP++],4);
  set_d(TTemp[SP-1],0.012  ,0);
  set_i(TTemp[SP-1],0  ,1);
  set_d(TTemp[SP-1],0.0000055  ,2);
  set_d(TTemp[SP-1],0.0000055  ,3);
  create_tuple_i(&TTemp[SP++],2);
  T_tuple_div(hv_Width,TTemp[SP-1],&TTemp[SP]);
  destroy_tuple(TTemp[SP-1]);
  TTemp[SP-1]=TTemp[SP];
  T_tuple_concat(TTemp[SP-2],TTemp[SP-1],&TTemp[SP]);
  destroy_tuple(TTemp[SP-2]);
  destroy_tuple(TTemp[SP-1]);
  TTemp[SP-2]=TTemp[SP];
  SP=SP-1;
  create_tuple_i(&TTemp[SP++],2);
  T_tuple_div(hv_Height,TTemp[SP-1],&TTemp[SP]);
  destroy_tuple(TTemp[SP-1]);
  TTemp[SP-1]=TTemp[SP];
  T_tuple_concat(TTemp[SP-2],TTemp[SP-1],&TTemp[SP]);
  destroy_tuple(TTemp[SP-2]);
  destroy_tuple(TTemp[SP-1]);
  TTemp[SP-2]=TTemp[SP];
  SP=SP-1;
  T_tuple_concat(TTemp[SP-1],hv_Width,&TTemp[SP]);
  destroy_tuple(TTemp[SP-1]);
  TTemp[SP-1]=TTemp[SP];
  T_tuple_concat(TTemp[SP-1],hv_Height,&TTemp[SP]);
  destroy_tuple(TTemp[SP-1]);
  TTemp[SP-1]=TTemp[SP];
  destroy_tuple(hv_StartCamPar);
  hv_StartCamPar=TTemp[--SP];

  /*CaltabName := 'caltab_30mm.descr'*/
  reuse_tuple_s(&hv_CaltabName,"caltab_30mm.descr");
  /*create_calib_data ('calibration_object', 1, 1, CalibDataID)*/
  create_tuple_s(&TTemp[SP++],"calibration_object");
  create_tuple_i(&TTemp[SP++],1);
  create_tuple_i(&TTemp[SP++],1);
  destroy_tuple(hv_CalibDataID);
  /***/T_create_calib_data(TTemp[SP-3], TTemp[SP-2], TTemp[SP-1], &hv_CalibDataID);
  destroy_tuple(TTemp[--SP]);
  destroy_tuple(TTemp[--SP]);
  destroy_tuple(TTemp[--SP]);

  /*set_calib_data_cam_param (CalibDataID, 0, 'area_scan_division', StartCamPar)*/
  create_tuple_i(&TTemp[SP++],0);
  create_tuple_s(&TTemp[SP++],"area_scan_division");
  /***/T_set_calib_data_cam_param(hv_CalibDataID, TTemp[SP-2], TTemp[SP-1], hv_StartCamPar);
  destroy_tuple(TTemp[--SP]);
  destroy_tuple(TTemp[--SP]);

  /*set_calib_data_calib_object (CalibDataID, 0, CaltabName)*/
  create_tuple_i(&TTemp[SP++],0);
  /***/T_set_calib_data_calib_object(hv_CalibDataID, TTemp[SP-1], hv_CaltabName);
  destroy_tuple(TTemp[--SP]);


  /*读取目录**/
  /*NumImages := 12*/
  reuse_tuple_i(&hv_NumImages,12);

  /*========== for I := 1 to NumImages by 1 ==========*/
  copy_tuple(hv_NumImages,&TTemp[SP++]);
  create_tuple_i(&TTemp[SP++],1);
  create_tuple_i(&TTemp[SP++],1);
  T_tuple_greater(TTemp[SP-1],TTemp[SP-3],&TTemp[SP]);
  SP++;
  T_tuple_equal(TTemp[SP-2],TTemp[SP-4],&TTemp[SP]);
  if(get_i(TTemp[SP],0) ||
     (!((( get_i(TTemp[SP-1],0)) && (get_d(TTemp[SP-3],0)>0)) ||
        ((!get_i(TTemp[SP-1],0)) && (get_d(TTemp[SP-3],0)<0)))))
  {
   destroy_tuple(TTemp[SP--]);
   destroy_tuple(TTemp[SP]);
   T_tuple_sub(TTemp[SP-1],TTemp[SP-2],&TTemp[SP]);
   destroy_tuple(hv_I);
   copy_tuple(TTemp[SP],&hv_I);
   destroy_tuple(TTemp[SP--]);
   destroy_tuple(TTemp[SP]);
   for(;;)
   {
   T_tuple_add(hv_I,TTemp[SP-1],&TTemp[SP]);
   destroy_tuple(hv_I);
   copy_tuple(TTemp[SP],&hv_I);
   destroy_tuple(TTemp[SP]);
   if(get_d(TTemp[SP-1],0)<0)
    T_tuple_less(hv_I,TTemp[SP-2],&TTemp[SP]);
   else
    T_tuple_greater(hv_I,TTemp[SP-2],&TTemp[SP]);
   if(get_i(TTemp[SP],0)) break;
   destroy_tuple(TTemp[SP]);
   /*========== for ==========*/

    /*read_image (ImageCalib, 'C:/Users/Administrator/Desktop/4_2/calibration-4-1/temp/scratch_calib_'+I$'02d')*/
    create_tuple_s(&TTemp[SP++],"C:/Users/Administrator/Desktop/4_2/calibration-4-1/temp/scratch_calib_");
    create_tuple_s(&TTemp[SP++],"02d");
    T_tuple_string(hv_I,TTemp[SP-1],&TTemp[SP]);
    destroy_tuple(TTemp[SP-1]);
    TTemp[SP-1]=TTemp[SP];
    T_tuple_add(TTemp[SP-2],TTemp[SP-1],&TTemp[SP]);
    destroy_tuple(TTemp[SP-2]);
    destroy_tuple(TTemp[SP-1]);
    TTemp[SP-2]=TTemp[SP];
    SP=SP-1;
    clear_obj(ho_ImageCalib);
    /***/T_read_image(&ho_ImageCalib, TTemp[SP-1]);
    destroy_tuple(TTemp[--SP]);


    /*find_calib_object (ImageCalib, CalibDataID, 0, 0, I, [], [])*/
    create_tuple_i(&TTemp[SP++],0);
    create_tuple_i(&TTemp[SP++],0);
    create_tuple(&TTemp[SP++],0);
    create_tuple(&TTemp[SP++],0);
    /***/T_find_calib_object(ho_ImageCalib, hv_CalibDataID, TTemp[SP-4], TTemp[SP-3], 
        hv_I, TTemp[SP-2], TTemp[SP-1]);
    destroy_tuple(TTemp[--SP]);
    destroy_tuple(TTemp[--SP]);
    destroy_tuple(TTemp[--SP]);
    destroy_tuple(TTemp[--SP]);

    /*get_calib_data_observ_contours (Contours, CalibDataID, 'caltab', 0, 0, I)*/
    create_tuple_s(&TTemp[SP++],"caltab");
    create_tuple_i(&TTemp[SP++],0);
    create_tuple_i(&TTemp[SP++],0);
    clear_obj(ho_Contours);
    /***/T_get_calib_data_observ_contours(&ho_Contours, hv_CalibDataID, TTemp[SP-3], 
        TTemp[SP-2], TTemp[SP-1], hv_I);
    destroy_tuple(TTemp[--SP]);
    destroy_tuple(TTemp[--SP]);
    destroy_tuple(TTemp[--SP]);

    /*get_calib_data_observ_points (CalibDataID, 0, 0, I, Row, Column, Index, StartPose)*/
    create_tuple_i(&TTemp[SP++],0);
    create_tuple_i(&TTemp[SP++],0);
    destroy_tuple(hv_Row);
    destroy_tuple(hv_Column);
    destroy_tuple(hv_Index);
    destroy_tuple(hv_StartPose);
    /***/T_get_calib_data_observ_points(hv_CalibDataID, TTemp[SP-2], TTemp[SP-1], 
        hv_I, &hv_Row, &hv_Column, &hv_Index, &hv_StartPose);
    destroy_tuple(TTemp[--SP]);
    destroy_tuple(TTemp[--SP]);

   }
   destroy_tuple(TTemp[SP--]);
   destroy_tuple(TTemp[SP--]);
   destroy_tuple(TTemp[SP]);
  }
  else
  {
   destroy_tuple(TTemp[SP--]);
   destroy_tuple(TTemp[SP--]);
   destroy_tuple(TTemp[SP--]);
   destroy_tuple(TTemp[SP--]);
   destroy_tuple(TTemp[SP]);
  }/*========== end for ========*/


  /*标定相机* NOTE:选择哪一个外参，标定板摆放*/
  /*calibrate_cameras (CalibDataID, Error)*/
  destroy_tuple(hv_Error);
  /***/T_calibrate_cameras(hv_CalibDataID, &hv_Error);

  /*get_calib_data (CalibDataID, 'camera', 0, 'params', CamPara)*/
  create_tuple_s(&TTemp[SP++],"camera");
  create_tuple_i(&TTemp[SP++],0);
  create_tuple_s(&TTemp[SP++],"params");
  destroy_tuple(hv_CamPara);
  /***/T_get_calib_data(hv_CalibDataID, TTemp[SP-3], TTemp[SP-2], TTemp[SP-1], &hv_CamPara);
  destroy_tuple(TTemp[--SP]);
  destroy_tuple(TTemp[--SP]);
  destroy_tuple(TTemp[--SP]);

  /*get_calib_data (CalibDataID, 'calib_obj_pose', [0,1], 'pose', PoseCalib)*/
  create_tuple_s(&TTemp[SP++],"calib_obj_pose");
  create_tuple(&TTemp[SP++],2);
  set_i(TTemp[SP-1],0  ,0);
  set_i(TTemp[SP-1],1  ,1);
  create_tuple_s(&TTemp[SP++],"pose");
  destroy_tuple(hv_PoseCalib);
  /***/T_get_calib_data(hv_CalibDataID, TTemp[SP-3], TTemp[SP-2], TTemp[SP-1], &hv_PoseCalib);
  destroy_tuple(TTemp[--SP]);
  destroy_tuple(TTemp[--SP]);
  destroy_tuple(TTemp[--SP]);

  //参数赋值
  copy_tuple(hv_CamPara,&Para);
  copy_tuple(hv_PoseCalib,&Pose);


  /****************************************************/
  /******************     End procedure   *************/
  /****************************************************/

  /* Clear temporary tuple stack */
  while (SP > 0)
    destroy_tuple(TTemp[--SP]);
  /* Clear temporary object stack */
  while (SPO > 0)
    clear_obj(OTemp[--SPO]);
  /* Clear temporary tuple vectors stack*/
  while (SPTV > 0)
    V_destroy_vector(TVTemp[--SPTV]);
  /* Clear temporary object vectors stack */
  while (SPOV > 0)
    V_destroy_vector(OVTemp[--SPOV]);
  /* Clear local iconic variables */
  clear_obj(ho_Image);
  clear_obj(ho_ImageCalib);
  clear_obj(ho_Contours);

  /* Clear local control variables */
  destroy_tuple(hv_filename);
  destroy_tuple(hv_Width);
  destroy_tuple(hv_Height);
  destroy_tuple(hv_StartCamPar);
  destroy_tuple(hv_CaltabName);
  destroy_tuple(hv_CalibDataID);
  destroy_tuple(hv_NumImages);
  destroy_tuple(hv_I);
  destroy_tuple(hv_Row);
  destroy_tuple(hv_Column);
  destroy_tuple(hv_Index);
  destroy_tuple(hv_StartPose);
  destroy_tuple(hv_Error);
  destroy_tuple(hv_CamPara);
  destroy_tuple(hv_PoseCalib);

}

void convertcoordinate(int ImageX,int ImageY,Htuple CurPara,Htuple CurPose,double *WorldX,double *WorldY)
{
	/* Stack for temporary tuples */
  Htuple   TTemp[100];
  int      SP=0;
  /* Stack for temporary tuple vectors */
  Hvector  TVTemp[100] = {0};
  int      SPTV=0;

  /* Local iconic variables */

  /* Local control variables */
  Htuple  hv_CamPara, hv_IMAGE_X;
  Htuple  hv_IMAGE_Y, hv_PoseCalib, hv_X, hv_Y, hv_WorldX;
  Htuple  hv_WorldY;

  /* Initialize control variables */
  create_tuple(&hv_CamPara,0);
  create_tuple(&hv_IMAGE_X,0);
  create_tuple(&hv_IMAGE_Y,0);
  create_tuple(&hv_PoseCalib,0);
  create_tuple(&hv_X,0);
  create_tuple(&hv_Y,0);
  create_tuple(&hv_WorldX,0);
  create_tuple(&hv_WorldY,0);

  /****************************************************/
  /******************   Begin procedure   *************/
  /****************************************************/

  /***参数赋值****/
  
  /***函数接口****/
  /*50 = x*/
  /*60 = y*/
  /*CamPara := CurPara*/
  /*PseCalib := CurPose*/

  /***参数赋值****/
  /*IMAGE_X := 50*/
  reuse_tuple_i(&hv_IMAGE_X,ImageX);

  /*IMAGE_Y := 60*/
  reuse_tuple_i(&hv_IMAGE_Y,ImageY);

  /*CamPara := Para*/
  destroy_tuple(hv_CamPara);
  copy_tuple(CurPara,&hv_CamPara);

  /*PoseCalib := Pose*/
  destroy_tuple(hv_PoseCalib);
  copy_tuple(CurPose,&hv_PoseCalib);

  /***坐标转换****/
  /*image_points_to_world_plane (CamPara, PoseCalib, IMAGE_Y, IMAGE_X, 'm', X, Y)*/
  create_tuple_s(&TTemp[SP++],"m");
  destroy_tuple(hv_X);
  destroy_tuple(hv_Y);
  /***/T_image_points_to_world_plane(hv_CamPara, hv_PoseCalib, hv_IMAGE_Y, hv_IMAGE_X, 
      TTemp[SP-1], &hv_X, &hv_Y);
  destroy_tuple(TTemp[--SP]);


  /***类型转换****/
  /*tuple_real (X, WorldX)*/
  destroy_tuple(hv_WorldX);
  /***/T_tuple_real(hv_X, &hv_WorldX);

  /*tuple_real (Y, WorldY)*/
  destroy_tuple(hv_WorldY);
  /***/T_tuple_real(hv_Y, &hv_WorldY);


  (*WorldX) = hv_WorldX.val.f;
  (*WorldY) = hv_WorldY.val.f;



  /****************************************************/
  /******************     End procedure   *************/
  /****************************************************/

  /* Clear temporary tuple stack */
  while (SP > 0)
    destroy_tuple(TTemp[--SP]);
  /* Clear temporary tuple vectors stack*/
  while (SPTV > 0)
    V_destroy_vector(TVTemp[--SPTV]);
  /* Clear local control variables */
  destroy_tuple(hv_CamPara);
  destroy_tuple(hv_IMAGE_X);
  destroy_tuple(hv_IMAGE_Y);
  destroy_tuple(hv_PoseCalib);
  destroy_tuple(hv_X);
  destroy_tuple(hv_Y);
  destroy_tuple(hv_WorldX);
  destroy_tuple(hv_WorldY);


}

MYPROCESS_API int _stdcall process(void* pdata,unsigned int w, unsigned int h ,double* worldx,double* worldy)
{
	//图像数据赋值
	Mat imgray(h,w,CV_8UC1);
	memcpy(imgray.data,pdata,h*w);
	
	Mat imgraysmall,imgraytiny;
	pyrDown(imgray, imgraysmall, Size(imgray.cols / 2 , imgray.rows / 2));
	pyrDown(imgraysmall, imgraytiny, Size(imgraysmall.cols / 2 , imgraysmall.rows / 2));
	//ROI
	int x1=200,y1=340,x2=820,y2=370;
	//阈值和分格
	int gridy=5, thrx=45;	
	int nsizey =(int)((y2-y1)/gridy); 
	//齿数、线位、序列
	int nmax=0;
	int ypos=0;
	vector<int> nlist;//nlist为imgraytiny中X的集合

	for(int i = 0;i < nsizey + 1 ; i++)//以等步长取的每一行
	{
		int y = y1 + i * gridy;

		vector<int> intersec;//intersec为每一行交点集合
		bool bfirst=false;
		int nfirst=0;
		int nsecond=0;
		for(int x=1+x1 ;x<x2 ;x++)//遍历行上面的每一点
		{
			int tmpgray = imgraytiny.at<uchar>(y,x);
			int bfrgray = imgraytiny.at<uchar>(y,x-1);
			if(tmpgray<thrx && bfrgray>=thrx)//进入
			{
				if(!bfirst)
				{
					bfirst = true;
					nfirst = x;
				}
			}
			if(tmpgray>=thrx && bfrgray<thrx)//离开
			{
				if(bfirst)
				{
					bfirst = false;
					nsecond = x;
					if(nsecond-nfirst>=15 && nsecond-nfirst<40)
					{
						intersec.push_back(nfirst);
						intersec.push_back(nsecond);
					}
				}
			}
			
		}
		
		int n = intersec.size();
		if(nmax < n)
		{
			nmax = n;
			ypos = y;
			nlist = intersec;
		}
		
	}
	//得到了nlist和ypos

	//放大为imgraysmall
	ypos *= 2;
	int n = nlist.size();
	if(n == 0){return 0;}
	for(int i =0;i<n;i++)
	{	
		nlist[i] *= 2;
	}		
	if(n == 0){return 0;}
	int yup = ypos-40 , ydown = ypos+40;
	int nminheight = 10000;		//最高齿的平均高度
	int Index=0;				//最高齿是第几个
	int locx1=0,locx2=0;
	int gearnum=(int)(n/2);
	int gridx = 3,thry=120;
	for(int i=0;i<gearnum;i++)  
	{	
		int x1 = nlist[2*i],x2 = nlist[2*i+1];
		int nsizex = (int)((x2-x1)/gridx);
		vector<int> midlist;	//存放一个齿内的所有midpos
		
		for(int j=0;j<nsizex+1;j++)	
		{
			int x = x1 + j*gridx;
			int midpos = 0;
			bool bmidfind = false;
			for(int y=yup;y<ydown;y++)
			{
				int tmpgray = imgraysmall.at<uchar>(y,x);
				int bfrgray = imgraysmall.at<uchar>(y-1,x);
				if(tmpgray<=thry && bfrgray>thry)
				{
					if(!bmidfind)	
					{	
						midpos = y;
						bmidfind = true;
					}
				}

			}
			midlist.push_back(midpos);
		}
		//求平均高度
		int meanheight = 0;
		for(int k=0;k < midlist.size();k++)
		{
			meanheight += midlist[k];
		}
		meanheight /= midlist.size(); 
		//最高齿位置信息
		if(meanheight < nminheight)
		{
			Index = i+1;
			nminheight = meanheight;
			locx1=x1;
			locx2=x2;
		}
	}
	
	int img_x = (locx1+locx2)/2;
	int img_y = nminheight;

	img_x /=2;
	img_y /=2;

	//*worldx = 676;
	//*worldy = 767; 


	//坐标转换
	convertcoordinate(img_x,img_y,RealPara,RealPose,worldx,worldy);

	return gearnum;
}

MYPROCESS_API double _stdcall calibration(char* filename)
{
	calib(filename,RealPara,RealPose);
	return 0.0000055;
}

MYPROCESS_API int __stdcall test()
{
	return 1;
};

MYPROCESS_API void _stdcall processtest(double* worldx,double* worldy)
{
	string filename = "C:\\Users\\Administrator\\Desktop\\pic\\7.jpg";
	Mat imgray = imread(filename,0);
	//Mat out;
	//pyrDown(img,out,Size(img.cols/2,img.rows/2));
	Mat imgraysmall,imgraytiny;
	pyrDown(imgray, imgraysmall, Size(imgray.cols / 2 , imgray.rows / 2));
	pyrDown(imgraysmall, imgraytiny, Size(imgraysmall.cols / 2 , imgraysmall.rows / 2));
	//ROI
	int x1=200,y1=340,x2=820,y2=370;
	//阈值和分格
	int gridy=5, thrx=45;	
	int nsizey =(int)((y2-y1)/gridy); 
	//齿数、线位、序列
	int nmax=0;
	int ypos=0;
	vector<int> nlist;//nlist为imgraytiny中X的集合

	for(int i = 0;i < nsizey + 1 ;i++)//以等步长取的每一行
	{
		int y = y1 + i * gridy;

		vector<int> intersec;//intersec为每一行交点集合
		bool bfirst=false;
		int nfirst=0;
		int nsecond=0;
		for(int x=1+x1 ;x<x2 ;x++)//遍历行上面的每一点
		{
			int tmpgray = imgraytiny.at<uchar>(y,x);
			int bfrgray = imgraytiny.at<uchar>(y,x-1);
			if(tmpgray<thrx && bfrgray>=thrx)//进入
			{
				if(!bfirst)
				{
					bfirst = true;
					nfirst = x;
				}
			}
			if(tmpgray>=thrx && bfrgray<thrx)//离开
			{
				if(bfirst)
				{
					bfirst = false;
					nsecond = x;
					if(nsecond-nfirst>=15 && nsecond-nfirst<40)
					{
						intersec.push_back(nfirst);
						intersec.push_back(nsecond);
					}
				}
			}
			
		}
		
		int n = intersec.size();
		if(nmax < n)
		{
			nmax = n;
			ypos = y;
			nlist = intersec;
		}
		
	}
	//得到了nlist和ypos

	//放大为imgraysmall
	ypos *= 2;
	int n = nlist.size();
	for(int i =0;i<n;i++)
	{
		nlist[i] *= 2;
	}

	int yup = ypos-40 , ydown = ypos+40;
	int nminheight = 10000;		//最高齿的平均高度
	int Index=0;				//最高齿是第几个
	int locx1=0,locx2=0;
	int gearnum=(int)(n/2);
	int gridx = 3,thry=120;
	for(int i=0;i<gearnum;i++)  
	{	
		int x1 = nlist[2*i],x2 = nlist[2*i+1];
		int nsizex = (int)((x2-x1)/gridx);
		vector<int> midlist;	//存放一个齿内的所有midpos
		
		for(int j=0;j<nsizex+1;j++)	
		{
			int x = x1 + j*gridx;
			int midpos = 0;
			bool bmidfind = false;
			for(int y=yup;y<ydown;y++)
			{
				int tmpgray = imgraysmall.at<uchar>(y,x);
				int bfrgray = imgraysmall.at<uchar>(y-1,x);
				if(tmpgray<=thry && bfrgray>thry)
				{
					if(!bmidfind)	
					{	
						midpos = y;
						bmidfind = true;
					}
				}

			}
			midlist.push_back(midpos);
		}
		//求平均高度
		int meanheight = 0;
		for(int k=0;k < midlist.size();k++)
		{
			meanheight += midlist[k];
		}
		meanheight /= midlist.size(); 
		//最高齿位置信息
		if(meanheight < nminheight)
		{
			Index = i+1;
			nminheight = meanheight;
			locx1=x1;
			locx2=x2;
		}
	}
	
	int x = (locx1+locx2)/2;
	int y = nminheight;


	*worldx = x;
	*worldy = y;
	
}

MYPROCESS_API void _stdcall buffertt(void* pdata, int w,  int h )
{
	
	Mat img(h,w,CV_8UC1);
	memcpy(img.data,pdata,h*w);

	

	imwrite("mono8419.bmp",img);

}

MYPROCESS_API int __stdcall test(char* imgname, int& c_x, struct Result_stru* v, int maxsize)
{
	for( int i=0;i<maxsize;i++)
	{

		v[i].x=1;
		v[i].y=i;
		v[i].status=i;
	}

	return 100;
}