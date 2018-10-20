#include<iostream>
#include<opencv2/opencv.hpp>
#include<opencv2/core/core.hpp>
#include<opencv2/highgui/highgui.hpp>
#include<opencv2/imgproc/imgproc.hpp>
#include<opencv2/imgproc/types_c.h>
#include<vector>
#include<sys/timeb.h>
#include<math.h>
using namespace std;
using namespace cv;

struct point
{
	int x;
	int y;
};

int main()
{

	struct timeb startTime , endTime;
    ftime(&startTime);

	Mat im = imread("E:\\samples\\0502\\yintong\\10_add\\processed.bmp",0);
	int x1=1200,y1=1525,x2=3000,y2=1800;

	int midx = (x1+x2)/2;
	
	int boardx1 = midx - 500;
	int boardx2 = midx + 500;
	int gridx = 3,thry = 10;
	int nsizex = (int)((boardx2-boardx1)/gridx);

	int grad = 10;

	vector<point> linepoint;
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
		if(0!= midpos)
		{
		
			struct point curpoint;
			curpoint.x=x;
			curpoint.y=midpos;
			linepoint.push_back(curpoint);
		
		}
	
	}

	//********将GearTop内的点进行最小二乘拟合*********//	
	
	//求A,B,C,D
	double A=0,B=0,C=0,D=0;
	int PointSize = linepoint.size();
	for(int i=0;i<PointSize;i++)
	{
		A += linepoint[i].x * linepoint[i].x;
		B += linepoint[i].x;
		C += linepoint[i].x * linepoint[i].y;
		D += linepoint[i].y;
	}

	//求a，b
	double a = (C*PointSize-B*D)/(A*PointSize-B*B);
	double b = (A*D-C*B)/(A*PointSize-B*B);
	
	//起始结束点

	double Startx=0,Endx=0,Middlex=0;


	//求startx
	int curmidposleft = 0;
	bool findleft = false;
	for(int x=midx;x>x1;x-=3)
	{
		int midpos = 0;
		bool bmidfind = false;
		for(int y=2500;y>y1;y--)
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
		if(midpos != 0)
		{
			if(!findleft)
			{
				curmidposleft = midpos;
				findleft = true;
			}

			if(abs(curmidposleft - midpos) > grad)
			{
				Startx = x;
				break;
			}

			curmidposleft = midpos;

		//line(im,Point(x,midpos-20),Point(x,midpos+20),Scalar(0,0,255),1,8);
			
		}
	}

	//求endx
	int curmidposright = 0;
	bool findright = false;
	for(int x=midx;x<x2;x+=3)
	{
		int midpos = 0;
		bool bmidfind = false;
		for(int y=2500;y>y1;y--)
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
		
		if(!findright)
		{
			curmidposright = midpos;
			findright = true;
		}

		if(abs(midpos - curmidposright) > grad)
		{
			Endx = x;
			break;
		}
		curmidposright = midpos;

		//line(im,Point(x,midpos-20),Point(x,midpos+20),Scalar(0,0,255),1,8);

	}



	Middlex = (Startx + Endx)/2;
	

	////起始结束点
	double Starty = Startx*a+b, Endy = Endx*a+b,Middley = Middlex*a+b;

	//roi
	/*line(im,Point(x1,y1),Point(x2,y1),Scalar(0,0,255),1,8);
	line(im,Point(x1,y2),Point(x2,y2),Scalar(0,0,255),1,8);
	line(im,Point(x1,y1),Point(x1,y2),Scalar(0,0,255),1,8);
	line(im,Point(x2,y1),Point(x2,y2),Scalar(0,0,255),1,8);*/

	/*line(im,Point(midx,50),Point(midx,2500),Scalar(0,0,255),1,8);
	line(im,Point(boardx1,50),Point(boardx1,2500),Scalar(0,0,255),1,8);				
	line(im,Point(boardx2,50),Point(boardx2,2500),Scalar(0,0,255),1,8);		*/		
					
	line(im,Point(Startx,Starty),Point(Endx,Endy),Scalar(0,0,255),1,8);				

	line(im,Point(Startx,0),Point(Startx,y2),Scalar(0,0,255),1,8);				
	line(im,Point(Endx,0),Point(Endx,y2),Scalar(0,0,255),1,8);
	line(im,Point(Middlex,1200),Point(Middlex,y2),Scalar(0,0,255),1,8);

    ftime(&endTime);

	cout<<Startx<<" "<<Starty<<endl;
	cout<<Endx<<" "<<Endy<<endl;
	cout<<Middlex<<" "<<Middley<<endl;


	imwrite("C:\\Users\\Administrator\\Desktop\\666.bmp",im);
	cout << "耗时"<<(endTime.time-startTime.time)*1000 + (endTime.millitm - startTime.millitm) << "ms"<< endl;
	system("pause");
	
	return 0;
}