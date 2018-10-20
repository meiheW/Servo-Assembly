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

	Mat im = imread("E:\\samples\\0502\\yintong\\02\\70.bmp",0);
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





	//roi
	line(im,Point(x1,y1),Point(x2,y1),Scalar(0,0,255),1,8);
	line(im,Point(x1,y2),Point(x2,y2),Scalar(0,0,255),1,8);
	line(im,Point(x1,y1),Point(x1,y2),Scalar(0,0,255),1,8);
	line(im,Point(x2,y1),Point(x2,y2),Scalar(0,0,255),1,8);

	line(im,Point(midx,50),Point(midx,2500),Scalar(0,0,255),1,8);
	line(im,Point(boardx1,50),Point(boardx1,2500),Scalar(0,0,255),1,8);				
	line(im,Point(boardx2,50),Point(boardx2,2500),Scalar(0,0,255),1,8);				
	
	line(im,Point(meanleftx,y1),Point(meanleftx,y2),Scalar(0,0,255),1,8);				
	line(im,Point(meanrightx,y1),Point(meanrightx,y2),Scalar(0,0,255),1,8);				
					
	
    ftime(&endTime);




	imwrite("C:\\Users\\Administrator\\Desktop\\666.bmp",im);
	cout << "meanleftx = " << meanleftx <<" meanrightx = " << meanrightx <<" ypos = " << ypos <<endl; 
	cout << "耗时"<<(endTime.time-startTime.time)*1000 + (endTime.millitm - startTime.millitm) << "ms"<< endl;
	system("pause");
	
	return 0;
}