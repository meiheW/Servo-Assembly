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
#include<math.h>
#include<stdlib.h>
using namespace std;
using namespace cv;


MYPROCESS_API void _stdcall drawline(void* pdata,  unsigned int w,  unsigned int h, double x1,double x2,double y1,double y2)
{
	Mat im(h,w,CV_8UC1);
	memcpy(im.data,pdata,h*w);

	line(im,Point(x1,y1),Point(x2,y2),Scalar(0,0,255),1,8);//划线
	
	imwrite("1.jpg",im);
	memcpy(pdata,im.data,h*w);

}


MYPROCESS_API void _stdcall gray2rbg( void *src, void *des, int srcw, int srch, int desw, int desh,int desstride )
{

	int stepw = srcw/ desw;
	int stepH = srch/desh;

	char* psrc=(char*)src;
	char* pdes = (char*)des;

	for( int i=0;i<desh;i++)
	{

		for( int j=0;j<desw;j++)
		{
			*(pdes+3*j)=*(pdes+3*j+1)=*(pdes+3*j+2)=*(psrc +j*stepw);
		}

		pdes += desstride;
		psrc += srcw*stepH;
	}

}


MYPROCESS_API void _stdcall process(void* pdata, unsigned int w, unsigned int h, int leftboundx, int rightboundx, double* leftx,double* lefty, double* rightx, double* righty, double* middlex, double* middley)
{
	
	Mat im(h,w,CV_8UC1);
	memcpy(im.data,pdata,h*w);
	//ROI   //2850
	//int x1=1300,y1=1525,x2=2900,y2=1700;
	int x1=1250,y1=1525,x2=3000,y2=1800;

	int gridy=5, thr=20;		   
	int nsizey =(int)((y2-y1)/gridy); 
	//求取nlist和ypos
	int nmax=0;
	int ypos=0;
	vector<int> nlist;
	
 	for(int i = 0;i < nsizey ;i++)//以等步长取的每一行
	{
		int y = y1 + i * gridy;

		vector<int> intersec;
		bool bfirst=false;
		int nfirst=0;
		int nsecond=0;
		
		for(int x=1+x1 ;x<x2 ;x++)//遍历行上面的每一点
		{
			int tmpgray = im.at<uchar>(y,x);//当前点与前一点的灰度值
			int bfrgray = im.at<uchar>(y,x-1);

			//找齿两边的边界点
			if(tmpgray<thr && bfrgray>=thr)//进入
			{
				if(!bfirst)
				{
					bfirst = true;
					nfirst = x;
				}
			}
			if(tmpgray>=thr && bfrgray<thr)//离开
			{
				if(bfirst)
				{
					bfirst = false;
					nsecond = x;
					if(nsecond-nfirst>=40 && nsecond-nfirst<65)
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
	

	//如果沒有就返回
	if(nlist.empty()) return;


	//得到了nlist和ypos

	int n = nlist.size();
	int gearnum =(int)(n/2);  //齿数

	/////////////////////////
	//找左右边界
	//左右边界点集合
	vector<int> leftlist;
	vector<int> rightlist;
	for(int y = ypos -50;y<ypos+50;y+=3)
	{
		
		bool bleftfirst =false;
		int rightx=0;
		for(int x = x1;x<x2;x++)
		{
			int tmpgray = im.at<uchar>(y,x);//当前点与前一点的灰度值
			int bfrgray = im.at<uchar>(y,x-1);
	
		//找每一行的左边界点
			if(tmpgray>=thr && bfrgray<thr)
			{
				if(!bleftfirst)
				{
					leftlist.push_back(x);
					bleftfirst = true;
				}
			}
			//找每一行的右边界点
			if(tmpgray<thr && bfrgray>=thr)
			{
				rightx = x;
			}
		}		

		if(rightx!=0)
		rightlist.push_back(rightx);
	}



	////////////////////////////

	int yup = ypos-40 , ydown = ypos+40;
	int locx1=0,locx2=0;            //最高齿的平均位置  
	
	int gridx = 3,thry=20;
	vector<point> GearTop;
	for(int i=0;i<gearnum;i++)
	{	
		int x1 = nlist[2*i],x2 = nlist[2*i+1];
		
		int nsizex = (int)((x2-x1)/gridx);
		//存放一个齿内的所有midpos
		vector<int> midlist;
		for(int j=0;j<nsizex;j++)	
		{
			int x = x1 + j*gridx;
			int midpos = 0;
			bool bmidfind = false;
			for(int y=ydown;y>yup;y--)
			{
				int tmpgray = im.at<uchar>(y,x);
				int bfrgray = im.at<uchar>(y+1,x);
				if(tmpgray>=thry && bfrgray<thry)
				{
					if(!bmidfind)	
					{	
						midpos = y;
						bmidfind = true;
					}
				}
			}
			if(0!= midpos)
			midlist.push_back(midpos);
	        
		}

		//求平均高度
		if(midlist.size()>6)
		{
			int meanheight = 0;
			int midsize = midlist.size();
			int compareindex = 0;
			for(int i =0;i<midsize;i++)
			{
				int tempmid = midlist[i];
				int tempindex = 0;
				for(int j =0;j<midsize;j++)
				{
					if(abs(tempmid - midlist[j])<2)  tempindex++;
				}
				if(tempindex>compareindex)
				{
					compareindex = tempindex;
					meanheight = midlist[i];
				}
			}
			struct point curpoint;
			curpoint.x=(x1+x2)/2;
			curpoint.y=meanheight;
			GearTop.push_back(curpoint);
		}
	
	
	}

	//*************对左右边界求平均***********//
	
	//左平均
	int meanleftx = 0;
	int leftnum = leftlist.size();
	int compleftindex = 0;
	for(int i=0;i<leftnum;i++)
	{
		int curindex = 0;
        int curleftx = leftlist[i];
		for(int j=0;j<leftnum;j++)
		{					
			if(abs(curleftx - leftlist[j])<2) curindex++; 
		}
		if(curindex>compleftindex)
		{
			compleftindex = curindex;
			meanleftx = curleftx; 
		}
	}

	//右平均
	int meanrightx = 0;
	int rightnum = rightlist.size();
	int comprightindex = 0;
	for(int i=0;i<rightnum;i++)
	{
		int curindex = 0;
        int currightx = rightlist[i];
		for(int j=0;j<rightnum;j++)
		{					
			if(abs(currightx - rightlist[j])<2) curindex++; 
		}
		if(curindex>comprightindex)
		{
			comprightindex = curindex;
			meanrightx = currightx; 
		}
	}

	//2017/7/10
	//如果叠加后的边界与第一张的出入较大，那么就用第一张的代替它
	if(abs(meanleftx-leftboundx)>=20) meanleftx = leftboundx;
	if(abs(meanrightx-rightboundx)>=20) meanrightx = rightboundx;


	//********将GearTop内的点进行最小二乘拟合*********//	
	
	//求A,B,C,D
	double A=0,B=0,C=0,D=0;
	int TopSize = GearTop.size();
	for(int i=0;i<TopSize;i++)
	{
		A += GearTop[i].x * GearTop[i].x;
		B += GearTop[i].x;
		C += GearTop[i].x * GearTop[i].y;
		D += GearTop[i].y;
	}

	//求a，b
	double a = (C*TopSize-B*D)/(A*TopSize-B*B);
	double b = (A*D-C*B)/(A*TopSize-B*B);
	
	//起始结束点
	//起始结束点
	double Startx=0,Endx=0,Middlex=0;
	bool getmiddleline = false;
	
	if(TopSize == 18)
	{
		getmiddleline = true;
		Startx = GearTop[0].x;
		Endx = GearTop[TopSize-1].x;
		Middlex = (Startx + Endx)/2;
	}		
	
	if(TopSize != 18)
	{																	
		int locleftx = meanleftx - 20;	
		int locrightx = meanrightx - 25; 
			
		for(int i = 1; i < 10; i++)
		{	
			locleftx  += 78 ;
			locrightx -= 78 ;
			bool bleft = false;
			bool bright = false;

			for(int j = 0;j < TopSize;j++)
			{
				if(abs(locleftx - GearTop[j].x)<20)
				{
					locleftx = GearTop[j].x;
					bleft = true;
					break;
				}
			
			}
	        for(int j = 0;j < TopSize;j++)
			{
				if(abs(locrightx - GearTop[j].x)<20)
				{
					locrightx = GearTop[j].x;
					bright = true;
					break;
				}
			}
			
			if(bleft && bright)
			{

				getmiddleline = true;
				Middlex = (locleftx + locrightx)/2;
				Startx = Middlex - 670;
				Endx = Middlex + 670;
				break;
			
			}
			
		}
		
	}	
	
	if(getmiddleline == false)
	{
		Startx = meanleftx + 65;
		Endx = meanrightx - 105;
		Middlex = (Startx + Endx)/2;
	}

	
	////起始结束点
	double Starty = Startx*a+b, Endy = Endx*a+b,Middley = Middlex*a+b;

	//赋值
	*leftx = Startx;
	*lefty = Starty;
	*rightx = Endx;
	*righty = Endy;
	*middlex = Middlex;
	*middley = Middley;

}


MYPROCESS_API void _stdcall processfirstimage(void* pdata, unsigned int w, unsigned int h, int* leftboundx,int* rightboundx)
{
	
	Mat im(h,w,CV_8UC1);
	memcpy(im.data,pdata,h*w);

	int x1=1250,y1=1525,x2=3000,y2=1800;

	int midx = (x1+x2)/2;
	
	int boardx1 = midx - 500;
	int boardx2 = midx + 500;
	int gridx = 3,thry = 120;
	int thr = 120;
	int nsizex = (int)((boardx2-boardx1)/gridx);

	//获取齿底位置
	vector<int> nlist;
	int ypos = 0;
	for(int i=0;i<nsizex;i++)
	{
		int x = boardx1 + i * gridx;

		int midpos = 0;
		bool bmidfind = false;
		for(int y=y2;y>y1;y--)
		{
			int tmpgray = im.at<uchar>(y,x);
			int bfrgray = im.at<uchar>(y+1,x);
			if(tmpgray>=thry && bfrgray<thry)
			{
				if(!bmidfind)	
				{	
					midpos = y;
					bmidfind = true;
				}
			}
		}
		if(midpos > ypos) ypos = midpos;
	
	}

	/////////////////////////
	//找左右边界
	//左右边界点集合
	vector<int> leftlist;
	vector<int> rightlist;
	for(int y = ypos - 80;y < ypos;y+=3)
	{
		bool bleftfirst =false;
		int rightx=0;
		for(int x = x1;x<x2;x++)
		{
			int tmpgray = im.at<uchar>(y,x);//当前点与前一点的灰度值
			int bfrgray = im.at<uchar>(y,x-1);
	
		//找每一行的左边界点
			if(tmpgray>=thr && bfrgray<thr)
			{
				if(!bleftfirst)
				{
					leftlist.push_back(x);
					bleftfirst = true;
				}
			}
			//找每一行的右边界点
			if(tmpgray<thr && bfrgray>=thr)
			{
				rightx = x;
			}
		}		

		if(rightx!=0)
		rightlist.push_back(rightx);
	}
	
	//*************对左右边界求平均***********//
	
	//左平均
	int meanleftx = 0;
	int leftnum = leftlist.size();
	int compleftindex = 0;
	for(int i=0;i<leftnum;i++)
	{
		int curindex = 0;
        int curleftx = leftlist[i];
		for(int j=0;j<leftnum;j++)
		{					
			if(abs(curleftx - leftlist[j])<2) curindex++; 
		}
		if(curindex>compleftindex)
		{
			compleftindex = curindex;
			meanleftx = curleftx; 
		}
	}

	//右平均
	int meanrightx = 0;
	int rightnum = rightlist.size();
	int comprightindex = 0;
	for(int i=0;i<rightnum;i++)
	{
		int curindex = 0;
        int currightx = rightlist[i];
		for(int j=0;j<rightnum;j++)
		{					
			if(abs(currightx - rightlist[j])<2) curindex++; 
		}
		if(curindex>comprightindex)
		{
			comprightindex = curindex;
			meanrightx = currightx; 
		}
	}

	//赋值
	*leftboundx = meanleftx;
	*rightboundx = meanrightx;

}


MYPROCESS_API void _stdcall processyinpian(void* pdata, unsigned int w, unsigned int h,  double* leftx, double* lefty,double* rightx, double* righty, double* middlex, double* middley)
{
	 
	Mat im(h,w,CV_8UC1);
	memcpy(im.data,pdata,h*w);

	int x1=800,y1=800,x2=2800,y2=1200;
	
	int gridy=5, thrx=60;		   
	int nsizey =(y2-y1)/gridy; 
	//求取nlist和ypos
	int nmax=0;
	int ypos=0;
	vector<int> nlist;
	for(int i = 0;i < nsizey ;i++)
	{
		int y = y2 - i * gridy;

		vector<int> intersec;
		bool bfirst=false;
		int nfirst=0;
		int nsecond=0;
		
		for(int x=1+x1 ;x<x2 ;x++)
		{
			int tmpgray = im.at<uchar>(y,x);
			int bfrgray = im.at<uchar>(y,x-1);
		
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
					if(nsecond-nfirst>=45 && nsecond-nfirst<85)
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
	
	//如果沒有就返回
	if(nlist.empty()) return;


	int n = nlist.size();
	int gearnum =(int)(n/2);  //齿数
	int yup = ypos , ydown = ypos+200;       //ydown参数要注意
	int locx1=0,locx2=0;
	
	int gridx = 3,thry=120;
	vector<point> GearTop;

	for(int i=0;i<gearnum;i++)
	{	
		int x1 = nlist[2*i],x2 = nlist[2*i+1];
		int nsizex = (int)((x2-x1)/gridx);

		//存放一个齿内的所有midpos
		vector<int> midlist;
		for(int j=1;j<nsizex-1;j++)
		{
			int x = x1 + j * gridx;
			int midpos = 0;
			bool midfind = false;
			for(int y=yup;y<ydown;y++)
			{
				int tmpgray = im.at<uchar>(y,x);
				int bfrgray = im.at<uchar>(y-1,x);
				if(tmpgray>=thry && bfrgray<thry)
				{
					if(!midfind)
					    {
							midpos = y;
							midfind = true;
						}
				}
			}
			if (midpos != 0)
			{
				midlist.push_back(midpos);
				line(im,Point(x,midpos),Point(x+1,midpos),Scalar(0,0,255),1,8);
			}
		}

		//求平均高度
		int meanheight = 0;
		int midsize = midlist.size();
		int compareindex = 0;
		for(int i =0;i<midsize;i++)
		{
			int tempmid = midlist[i];
			int tempindex = 0;
		    for(int j =0;j<midsize;j++)
			{
				if(abs(tempmid - midlist[j])<2)  tempindex++;
			}
			if(tempindex>compareindex)
			{
				compareindex = tempindex;
				meanheight = midlist[i];
			}
		}

		struct point curpoint;
		curpoint.x=(x1+x2)/2;
		curpoint.y=meanheight;
		GearTop.push_back(curpoint);

	}


	//*******将GearTop内的点进行最小二乘拟合********//	
	
	//求A,B,C,D
	double A=0,B=0,C=0,D=0;
	int TopSize = GearTop.size();
	for(int i=0;i<TopSize;i++)
	{
		A += GearTop[i].x * GearTop[i].x;
		B += GearTop[i].x;
		C += GearTop[i].x * GearTop[i].y;
		D += GearTop[i].y;
	}

	//求a，b
	double a = (C*TopSize-B*D)/(A*TopSize-B*B);
	double b = (A*D-C*B)/(A*TopSize-B*B);
	
	//起始结束点
	double Startx = GearTop[0].x,Endx = GearTop[TopSize-1].x;
	double Starty = Startx*a+b,Endy = Endx*a+b;

	//赋值
	*leftx = Startx;
	*lefty = Starty;
	*rightx = Endx;
	*righty = Endy;
	*middlex = (Startx+Endx)/2;
	*middley = (Starty+Endy)/2;


}


MYPROCESS_API void _stdcall addimage(void* pdata,void* curpdata, unsigned int w, unsigned int h)
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
  Hobject  ho_CurImage, ho_Image, ho_InvertedImage;

  /* Local control variables */
  Htuple  hv_Pointer, hv_Type, hv_Width, hv_Height;
  Htuple  hv_PointerInt;

  /* Initialize iconic variables */
  gen_empty_obj(&ho_CurImage);
  gen_empty_obj(&ho_Image);
  gen_empty_obj(&ho_InvertedImage);

  /* Initialize control variables */
  create_tuple(&hv_Pointer,0);
  create_tuple(&hv_Type,0);
  create_tuple(&hv_Width,0);
  create_tuple(&hv_Height,0);
  create_tuple(&hv_PointerInt,0);

  /****************************************************/
  /******************   Begin procedure   *************/
  /****************************************************/

  /*Hobject Image;*/
  /*gen_image1(&Image,"byte",w,h,(Hlong)pdata);*/
  /*copy_obj(Image,&ho_Image,1,-1);*/
  /*clear_obj(Image);*/

  /*read_image (CurImage, 'E:/samples/samples0422/yt01/1')*/
  clear_obj(ho_CurImage);
  //***/read_image(&ho_CurImage, "E:/samples/samples0422/yt01/1");
  gen_image1(&ho_CurImage,"byte",w,h,(Hlong)curpdata);
  
  /*read_image (Image, 'E:/samples/samples0422/yt01/2')*/
  clear_obj(ho_Image);
  //***/read_image(&ho_Image, "E:/samples/samples0422/yt01/2");
  gen_image1(&ho_Image,"byte",w,h,(Hlong)pdata);

  /*invert_image (Image, InvertedImage)*/
  clear_obj(ho_InvertedImage);
  /***/invert_image(ho_Image, &ho_InvertedImage);

  /*add_image (InvertedImage, CurImage, CurImage, 1, 0)*/
  /***/add_image(ho_InvertedImage, ho_CurImage, &(OTemp[SPO]), 1, 0);
  SPO++;
  clear_obj(ho_CurImage);
  ho_CurImage = OTemp[--SPO];


  /*convert_image_type (CurImage, CurImage, 'byte')*/
  /***/convert_image_type(ho_CurImage, &(OTemp[SPO]), "byte");
  SPO++;
  clear_obj(ho_CurImage);
  ho_CurImage = OTemp[--SPO];

  /*get_image_pointer1 (CurImage, Pointer, Type, Width, Height)*/
  destroy_tuple(hv_Pointer);
  destroy_tuple(hv_Type);
  destroy_tuple(hv_Width);
  destroy_tuple(hv_Height);
  /***/T_get_image_pointer1(ho_CurImage, &hv_Pointer, &hv_Type, &hv_Width, &hv_Height);

  /*tuple_int (Pointer, PointInt)*/
  destroy_tuple(hv_PointerInt);
  /***/T_tuple_int(hv_Pointer, &hv_PointerInt);

   memcpy(curpdata, (unsigned char *)hv_PointerInt.val.l,h*w);


  /*write_image (CurImage, 'bmp', 0, 'E:/samples/samples0422/yt01/3')*/
  //***/write_image(ho_CurImage, "bmp", 0, "E:/samples/samples0422/yt01/3");



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
  clear_obj(ho_CurImage);
  clear_obj(ho_Image);
  clear_obj(ho_InvertedImage);

  /* Clear local control variables */
  destroy_tuple(hv_Pointer);
  destroy_tuple(hv_Type);
  destroy_tuple(hv_Width);
  destroy_tuple(hv_Height);
  destroy_tuple(hv_PointerInt);
	
	
	
	///* Stack for temporary tuples */
 // Htuple   TTemp[100];
 // int      SP=0;
 // /* Stack for temporary objects */
 // Hobject  OTemp[100] = {0};
 // int      SPO=0;
 // /* Stack for temporary tuple vectors */
 // Hvector  TVTemp[100] = {0};
 // int      SPTV=0;
 // /* Stack for temporary object vectors */
 // Hvector  OVTemp[100] = {0};
 // int      SPOV=0;

 // /* Local iconic variables */
 // Hobject  ho_Image, ho_CurImage, ho_InvertedImage;

 // /* Initialize iconic variables */
 // gen_empty_obj(&ho_Image);
 // gen_empty_obj(&ho_CurImage);
 // gen_empty_obj(&ho_InvertedImage);

 // /****************************************************/
 // /******************   Begin procedure   *************/
 // /****************************************************/

 // Hobject Image;
 // gen_image1(&Image,"byte",w,h,(Hlong)pdata);
 // //write_image(Image, "bmp", 0, "C:/Users/Administrator/Desktop/cjjpic.bmp");
 // Hobject CurImage;
 // gen_image1(&CurImage,"byte",w,h,(Hlong)curpdata);
 // //write_image(CurImage, "bmp", 0, "C:/Users/Administrator/Desktop/cjjcurpic.bmp");

 // /*read_image (Image, 'E:/samples_0422/yt01/yt_0422_97')*/
 // clear_obj(ho_Image);
 // copy_obj(Image,&ho_Image,1,-1);
 // //***/read_image(&ho_Image, "E:/samples_0422/yt01/yt_0422_97");

 // /*read_image (CurImage, 'E:/samples_0422/yt01/yt_0422_98')*/
 // clear_obj(ho_CurImage);
 // copy_obj(CurImage,&ho_CurImage,1,-1);
 // //***/read_image(&ho_CurImage, "E:/samples_0422/yt01/yt_0422_98");

 // /*invert_image (Image, InvertedImage)*/
 // clear_obj(ho_InvertedImage);
 // /***/invert_image(ho_Image, &ho_InvertedImage);

 // /*add_image (InvertedImage, CurImage, CurImage, 1, 0)*/
 // /***/add_image(ho_InvertedImage, ho_CurImage, &(OTemp[SPO]), 1, 0);
 // SPO++;
 // clear_obj(ho_CurImage);
 // ho_CurImage = OTemp[--SPO];
 // //write_image(ho_CurImage, "bmp", 0, "C:/Users/Administrator/Desktop/cjjaddedpic.bmp");
 // 
 // 
 // Hlong width,height;
 // width=0;
 // height=0;
 // char type[256];
 // convert_image_type(ho_CurImage,&ho_CurImage,"byte");
 // unsigned char* ptr;
 // get_image_pointer1(ho_CurImage,(Hlong*)&ptr,type,&width,&height);
 // memcpy(curpdata,ptr,h*w);



 // /****************************************************/
 // /******************     End procedure   *************/
 // /****************************************************/

 // /* Clear temporary tuple stack */
 // while (SP > 0)
 //   destroy_tuple(TTemp[--SP]);
 // /* Clear temporary object stack */
 // while (SPO > 0)
 //   clear_obj(OTemp[--SPO]);
 // /* Clear temporary tuple vectors stack*/
 // while (SPTV > 0)
 //   V_destroy_vector(TVTemp[--SPTV]);
 // /* Clear temporary object vectors stack */
 // while (SPOV > 0)
 //   V_destroy_vector(OVTemp[--SPOV]);
 // /* Clear local iconic variables */
 // clear_obj(Image);
 // clear_obj(CurImage);
 // clear_obj(ho_Image);
 // clear_obj(ho_CurImage);
 // clear_obj(ho_InvertedImage);

}


MYPROCESS_API void _stdcall invertimage(void* pdata, unsigned int w, unsigned int h)
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
	  Hobject  ho_Image, ho_ImageInvert;

	  /* Local control variables */
	  Htuple  hv_Pointer, hv_Type, hv_Width, hv_Height;
	  Htuple  hv_PointerInt;

	  /* Initialize iconic variables */
	  gen_empty_obj(&ho_Image);
	  gen_empty_obj(&ho_ImageInvert);

	  /* Initialize control variables */
	  create_tuple(&hv_Pointer,0);
	  create_tuple(&hv_Type,0);
	  create_tuple(&hv_Width,0);
	  create_tuple(&hv_Height,0);
	  create_tuple(&hv_PointerInt,0);

	  /****************************************************/
	  /******************   Begin procedure   *************/
	  /****************************************************/

	  /*read_image (Image, 'E:/samples/samples0422/yt01/yt_0422_01.bmp')*/
	  clear_obj(ho_Image);
	  // Generate same image with Halcon DataType
      gen_image1(&ho_Image,"byte",w,h,(Hlong)pdata);
	  //copy_obj(Image,&ho_Image,1,-1);

	  //***/read_image(&ho_Image, "E:/samples/samples0422/yt01/yt_0422_01.bmp");


	  /*invert_image (Image, ImageInvert)*/
	  clear_obj(ho_ImageInvert);
	  /***/invert_image(ho_Image, &ho_ImageInvert);
	  //write_image(ho_ImageInvert, "bmp", 0, "C:/Users/Administrator/Desktop/ho_ImageInvert.bmp");

	  /*convert_image_type (ImageInvert, ImageInvert, 'byte')*/
	  /***/convert_image_type(ho_ImageInvert, &(OTemp[SPO]), "byte");
	  SPO++;
	  clear_obj(ho_ImageInvert);
	  ho_ImageInvert = OTemp[--SPO];

	  /*get_image_pointer1 (ImageInvert, Pointer, Type, Width, Height)*/
	  destroy_tuple(hv_Pointer);
	  destroy_tuple(hv_Type);
	  destroy_tuple(hv_Width);
	  destroy_tuple(hv_Height);
	  /***/T_get_image_pointer1(ho_ImageInvert, &hv_Pointer, &hv_Type, &hv_Width, &hv_Height);

	  /*tuple_int (Pointer, PointerInt)*/
	  destroy_tuple(hv_PointerInt);
	  /***/T_tuple_int(hv_Pointer, &hv_PointerInt);
	  memcpy(pdata, (unsigned char *)hv_PointerInt.val.l,h*w);

	  /*write_image (ImageInvert, 'bmp', 0, 'C:/Users/Administrator/Desktop/test_invert.bmp')*/
	  //***/write_image(ho_ImageInvert, "bmp", 0, "C:/Users/Administrator/Desktop/test_invert.bmp");


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
	  clear_obj(ho_ImageInvert);

	  /* Clear local control variables */
	  destroy_tuple(hv_Pointer);
	  destroy_tuple(hv_Type);
	  destroy_tuple(hv_Width);
	  destroy_tuple(hv_Height);
	  destroy_tuple(hv_PointerInt);

}


MYPROCESS_API void _stdcall calibtest()
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
  /*filename := './calibration-4-1/scratch_perspective'*/
  reuse_tuple_s(&hv_filename,"E:\\标定\\4_2\\calibration-4-1\\scratch_perspective");
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

    /*read_image (ImageCalib, './calibration-4-1/temp/scratch_calib_'+I$'02d')*/
    create_tuple_s(&TTemp[SP++],"E:\\标定\\4_2\\calibration-4-1\\temp\\scratch_calib_");
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

  /*write_cam_par (CamPara, 'campar.dat')*/
  create_tuple_s(&TTemp[SP++],"campar.dat");
  /***/T_write_cam_par(hv_CamPara, TTemp[SP-1]);
  destroy_tuple(TTemp[--SP]);

  /*write_pose (PoseCalib, 'campose.dat')*/
  create_tuple_s(&TTemp[SP++],"campose.dat");
  /***/T_write_pose(hv_PoseCalib, TTemp[SP-1]);
  destroy_tuple(TTemp[--SP]);




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


MYPROCESS_API void _stdcall convertcoordinate(double imagex,double imagey,double* worldx,double* worldy)
{
	/* Stack for temporary tuples */
  Htuple   TTemp[100];
  int      SP=0;
  /* Stack for temporary tuple vectors */
  Hvector  TVTemp[100] = {0};
  int      SPTV=0;

  /* Local iconic variables */

  /* Local control variables */
  Htuple  hv_CameraParam, hv_Pose, hv_Image_x, hv_Image_y;
  Htuple  hv_X, hv_Y, hv_WorldX, hv_WorldY;

  /* Initialize control variables */
  create_tuple(&hv_CameraParam,0);
  create_tuple(&hv_Pose,0);
  create_tuple(&hv_Image_x,0);
  create_tuple(&hv_Image_y,0);
  create_tuple(&hv_X,0);
  create_tuple(&hv_Y,0);
  create_tuple(&hv_WorldX,0);
  create_tuple(&hv_WorldY,0);

  /****************************************************/
  /******************   Begin procedure   *************/
  /****************************************************/



  /*read_cam_par ('campar.dat', CameraParam)*/
  create_tuple_s(&TTemp[SP++],"campar.dat");
  destroy_tuple(hv_CameraParam);
  /***/T_read_cam_par(TTemp[SP-1], &hv_CameraParam);
  destroy_tuple(TTemp[--SP]);

  /*read_pose ('campose.dat', Pose)*/
  create_tuple_s(&TTemp[SP++],"campose.dat");
  destroy_tuple(hv_Pose);
  /***/T_read_pose(TTemp[SP-1], &hv_Pose);
  destroy_tuple(TTemp[--SP]);

  /*Image_x := 783*/
  reuse_tuple_i(&hv_Image_x,imagex);

  /*Image_y := 349*/
  reuse_tuple_i(&hv_Image_y,imagey);

  /***坐标转换****/
  /*image_points_to_world_plane (CameraParam, Pose, Image_y, Image_x, 'm', X, Y)*/
  create_tuple_s(&TTemp[SP++],"mm");
  destroy_tuple(hv_X);
  destroy_tuple(hv_Y);
  /***/T_image_points_to_world_plane(hv_CameraParam, hv_Pose, hv_Image_y, hv_Image_x, 
      TTemp[SP-1], &hv_X, &hv_Y);
  destroy_tuple(TTemp[--SP]);


  /***类型转换****/
  /*tuple_real (X, WorldX)*/
  destroy_tuple(hv_WorldX);
  /***/T_tuple_real(hv_X, &hv_WorldX);

  /*tuple_real (Y, WorldY)*/
  destroy_tuple(hv_WorldY);
  /***/T_tuple_real(hv_Y, &hv_WorldY);


  *worldx = hv_WorldX.val.f;
  *worldy = hv_WorldY.val.f;


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
  destroy_tuple(hv_CameraParam);
  destroy_tuple(hv_Pose);
  destroy_tuple(hv_Image_x);
  destroy_tuple(hv_Image_y);
  destroy_tuple(hv_X);
  destroy_tuple(hv_Y);
  destroy_tuple(hv_WorldX);
  destroy_tuple(hv_WorldY);

}


MYPROCESS_API int __stdcall test()
{
	return 0;
}