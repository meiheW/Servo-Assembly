# -*- coding: utf-8 -*-
"""
Created on Thu Dec 03 18:41:33 2015

@author: K
"""

import cv2
import numpy as np
import matplotlib.pyplot as plt
from scipy.optimize import leastsq
import math

ptstart1=(650,1350)
ptend1=  (3350,1350)  
ptstart2=(650,1500)
ptend2=  (3350,1500)   
gridx = 20

'''
def func(x,p):
    
    #print A, k, theta
    return A*np.sin(2*np.pi*k*x+theta)
def residuals(p, y, x):
    return y-func(x, p)
    

x = np.linspace(0, -2*np.pi, 100)
A, k, theta = 10, 0.34, np.pi/6 #真实数据函数参数
y0 = func(x, [A, k, theta])
y1 = y0 + 2*np.random.randn(len(x))

p0 = [7, 0.2, 0] #函数拟合参数
plsq = leastsq(residuals, p0, args=(y1, x))
'''

def polyfit(x, y, degree):
    results = {}
    coeffs = np.polyfit(x, y, degree)
    results['polynomial'] = coeffs.tolist()
    # r-squared
    p = np.poly1d(coeffs)
    # fit values, and mean
    yhat = p(x)                         # or [p(z) for z in x]
    ybar = np.sum(y)/len(y)          # or sum(y)/len(y)
    ssreg = np.sum((yhat-ybar)**2)   # or sum([ (yihat - ybar)**2 for yihat in yhat])
    sstot = np.sum((y - ybar)**2)    # or sum([ (yi - ybar)**2 for yi in y])
    results['determination'] = ssreg / sstot #准确率
    return results,coeffs

def findGear(pointlist, stvalue):    
    n=pointlist.shape[0]
    thr=45 #灰度45，从亮变到暗 ，然后在从暗的变到量的
    bfirst=False
    nfirst=0
    nsencond=0
    nlist=[]
    for i in range(1,n):
        #白变黑
        if pointlist[i] < thr and pointlist[i-1] > thr:
            if not bfirst :
                bfirst = True
                nfirst = i
        #黑变白        
        if pointlist[i] > thr and pointlist[i-1] < thr:
            if bfirst:
                bfirst = False
                nsencond=i
                #宽度滤波
                if nsencond - nfirst > 15 and nsencond - nfirst < 40:                    
                    nlist.append(nfirst+stvalue)
                    nlist.append(nsencond+stvalue)
    return nlist       

    
def getGearSum(imgray,im,imrect, pydownsize):  
    
    gridy = 5
    x1   = imrect[0]/4
    y1   = imrect[1]/4
    x2   = imrect[2]/4
    y2   = imrect[3]/4       
    nsizey=(y2-y1)/gridy    
    nlistx=[]
    nmax=0
    ypos=0
    colortaby=np.zeros(( x2-x1,nsizey),dtype=np.ubyte)
    for i in range(0,nsizey):
        tmp = i*gridy+y1
        #cv2.line(im,(x1,tmp),( x2,tmp),(0,255,0),1)
        colortaby[:,i] = imgray[tmp,x1:x2] 
        mylist = findGear(colortaby[:,i],x1)
        n=len(mylist)
        #print n
        if nmax < n:
            nmax=n
            ypos= i*gridy+y1
            nlistx=mylist   
 
    #print nmax
    #计算有多少个峰值 
    #n = len(nlistx)
    #for i in nlistx[:]:
    #     cv2.line(im,(i,ypos+20),( i,ypos-20),(0,0,255),1)
   
    #for i in range(0,nsizey):
    #    plt.plot(colortaby[:,i])    
    #plt.savefig("test3.png", dpi=750) 

    #cv2.imwrite('geara.jpg',im)    
    return ypos,  nlistx
    
def curveknee(pts):
    #找中位置
    midpos=0
    bmidfind= False
    #bkneefind = False
    kneepos=0
    n = len(pts)
    for i in range(1,n):
        if pts[i] < 120 and pts[i-1] > 120:
            midpos=i
            bmidfind = True
        if pts[i]<6 and pts[i-1] > 6:
            if bmidfind:
                kneepos=i
                #bkneefind = True
                bmidfind = False

    return midpos,kneepos,bmidfind

def getmeanHight(l):
    maxv=max(l)
    minv=min(l)
    xx=[0 for x in range(0, maxv-minv+1)]
    for x in l[:]:
        xx[x-minv]= xx[x-minv] + 1            
    #print xx    
    #print xx.index(max(xx))+minv
    return xx.index(max(xx))+minv
    
def getmeanHight2(l):
    maxv=max(l)
    minv=min(l)
    
    xx=[0 for x in range(0, maxv-minv+1)]
    for i in range(0,maxv-minv):
        for v in l[:]:
            if abs(i+minv-v)<2:
                xx[i]=xx[i]+1
                
    return xx.index(max(xx))+minv
 

def getmeanBottom2(l):
    maxv=max(l)
    minv=min(l)
    
    xx=[0 for x in range(0, maxv-minv+1)]
    for i in range(0,maxv-minv):
        for v in l[:]:
            if abs(i+minv-v)<2:
                xx[i]=xx[i]+1
                
    return xx.index(max(xx))+minv   
                
def getgearHight(imgray,x1,x2,ypos, imgshow,myn):
    
    y1=ypos-40
    y2=ypos+40
    gridx=3
    w = x2 -x1
    n = w/gridx+1
    colortab=np.zeros((y2-y1,n),dtype=np.ubyte)
    
    #plt.figure()  
    nmaxpos=2000 
    midlist=[]     
    for i in range(0,n):
        tmp = i*gridx+x1               
        #cv2.line(imgshow,(tmp,y1),(tmp,y2),(0,255,0),1)
        colortab[:,i] = imgray[y1:y2,tmp]
        mid, knee,bfind = curveknee(colortab[:,i])
        if nmaxpos>mid and bfind:
            nmaxpos = mid
        midlist.append(mid)
        
        
        #xxx = imgray[y1:y2,tmp]        
        #drawx=[ x for x in range(0,y2-y1)]  
        #result,coeffs = polyfit(drawx, xxx[:], 2)  
        #if result["determination"] < 1:
        #p = np.poly1d(coeffs)
        
        #drawy=p(drawx)
        #plt.plot(drawy)
        
        #print result["determination"]
    val = getmeanHight(midlist)  
    ret = val+y1     
    addr = midlist.index(val)*gridx+x1  
    
    #ret = getmeanHight(midlist)+y1   
    #cv2.line(imgshow,(x1,ret),(x2,ret),(0,0,255),1)    
    

    '''
    plt.figure()
    
    for i in range(0,n):
        plt.plot(colortab[:,i])
    plt.savefig("bxx"+str(myn)+".png", dpi=750) 
    '''
    return   ret,addr  

def getgearBottom(imgray,x1,x2,ypos, imgshow,myn):
    
    y1=ypos-40
    y2=ypos+40
    gridx=10
    w = x2-x1
    n = w/gridx+1
    colortab=np.zeros((y2-y1,n),dtype=np.ubyte)
    
    #plt.figure()  
    nmaxpos=0 
    midlist=[]     
    for i in range(0,n):
        tmp = i*gridx+x1               
        #cv2.line(imgshow,(tmp,y1),(tmp,y2),(0,255,0),1)
        colortab[:,i] = imgray[y1:y2,tmp]
        mid, knee,bfind = curveknee(colortab[:,i])
        if nmaxpos<mid and bfind:
            nmaxpos = mid
        midlist.append(mid)
        
        
        #xxx = imgray[y1:y2,tmp]        
        #drawx=[ x for x in range(0,y2-y1)]  
        #result,coeffs = polyfit(drawx, xxx[:], 2)  
        #if result["determination"] < 1:
        #p = np.poly1d(coeffs)
        
        #drawy=p(drawx)
        #plt.plot(drawy)
        
        #print result["determination"]
    #val = getmeanHight2(midlist)
    #addr = midlist.index(val)*gridx+x1
    #ret = val+y1,addr  
        
    val = getmeanHight(midlist)  
    ret = val+y1     
    addr = midlist.index(val)*gridx+x1         
    #cv2.line(imgshow,(x1,ret),(x2,ret),(0,0,255),1)     

    '''
    plt.figure()    
    for i in range(0,n):
        plt.plot(colortab[:,i])
    plt.savefig("bxx"+str(myn)+".png", dpi=750) 
    '''
    return   ret,addr       

if __name__ == '__main__': 

    #cv2.namedWindow('1',0) 
    geartop=[]#所有图片齿的最大的集合
    gearbottom=[]#所有图片齿的最小的集合
    
    allgeartop=[]#所有图片齿的最大的集合

    allgearbottom=[]#所有图片齿的最大的集合

    
    for iPos in range( 1, 31):        
        strim= str(iPos)+'.jpg'
        im=cv2.imread(strim)   
        if im  == None:
            continue
        imgray=cv2.cvtColor(im,cv2.COLOR_BGR2GRAY)     
        h,w = im.shape[:2]    
        imgraysmall=cv2.pyrDown(imgray)
        imgraytiny=cv2.pyrDown(imgraysmall)     
        imsmall=cv2.pyrDown(im)
        imtiny=cv2.pyrDown(imsmall)     
        ypos,nlistx = getGearSum(imgraytiny,imtiny,(650,1350,3350,1500),2)
        
        #cv2.imwrite("cxx"+str(iPos)+".jpg",imtiny)
        
        ypos *=2    
        rlistx=[x*2 for x in nlistx[:]]       
        #for i in rlistx[:]:
        #     cv2.line(imsmall,(i,ypos+20),( i,ypos-20),(0,0,255),1)    
        #cv2.imwrite('a.jpg',imsmall)    
        gearsum = len(rlistx)/2
    
        nminpos=10000
        minposlist=[]#一副图片里所有齿顶位置的集合
        minaddrlist=[]   
        for i in range(0,gearsum):
            h,addr = getgearHight(imgraysmall,rlistx[i*2],rlistx[i*2+1],ypos, imsmall,i)
            minposlist.append((addr,h))
            if nminpos > h:
                nminpos=h

        geartop.append(nminpos)
        allgeartop.append(minposlist)
        #cv2.imwrite("bxx"+str(iPos)+".jpg",imsmall)
        
        nmaxpos=0 
        maxposlist=[] #一副图片里所有齿底位置的集合      
        for i in range(0,gearsum-1):
            h,addr = getgearBottom(imgraysmall,rlistx[i*2+1],rlistx[i*2+2],ypos, imsmall,i)
            maxposlist.append((addr,h))
            if nmaxpos < h:
                nmaxpos=h  
        
        gearbottom.append(nmaxpos)
        allgearbottom.append(maxposlist)
        #print    "maxpos", maxposlist  
        #print    "maxaddr", maxaddrlist 
        #print    "minpos", minposlist  
        #print    "minaddr", minaddrlist 
        
        #做拟合
        #a= np.polyfit(addrlist,maxposlist,1 )
        #equ = np.poly1d(a)
        #print equ         
        #cv2.line(imsmall,(650/2,int(equ(650/2))),(3350/2,int(equ(3350/2))),(0,255,0),1)
        #cv2.line(imsmall,(650/2,nmaxpos),(3350/2,nmaxpos),(0,0,255),1)
        #cv2.imwrite("axx"+str(iPos)+".jpg",imsmall)
        
        #cv2.line(im,(650,nmaxpos*2),(3350,nmaxpos*2),(0,0,255),1)
        #cv2.line(im,(650,nminpos*2),(3350,nminpos*2),(0,0,255),1) 
        #cv2.imwrite("axx"+str(iPos)+".jpg",im)

    #print geartop
    #print gearbottom
    
    #print max(geartop),min(geartop)
    #print max(gearbottom),min(gearbottom)
    pre=str(allgeartop)
    pre=pre.replace("[[(","(")
    pre=pre.replace("[(","(")
    pre=pre.replace("],","\n")
    pre=pre.replace("]]","")    
    pre=pre.replace("(","")
    pre=pre.replace(")","")
    f=open("a.txt","w")
    f.write(pre)
    f.close()
    
    #0，800，1200，0
    headmax=99999
    headaddr=0
    midmax=99999
    midaddr=0
    tailmax=99999
    tailaddr=0
    
    for myval in allgeartop[:]:
        for addr,topmax in myval[:]:
            if headmax > topmax and addr <800:
                headmax=topmax
                headadd=addr
            elif tailmax > topmax and addr >1200:
                tailmax=topmax
                tailaddr=addr
            elif midmax > topmax and addr <=1200 and addr >=800:
                midmax = topmax
                midaddr=addr
    addrlist=[headadd,midaddr,tailaddr]
    maxposlist=[headmax,midmax,tailmax]            
    a= np.polyfit(addrlist,maxposlist,1 )
    equ = np.poly1d(a)
    myangle=math.atan(a[0])*180.0/math.pi
    print myangle
                
    for iPos in range( 1, 31):        
        strim= str(iPos)+'.jpg'
        im=cv2.imread(strim)
        imsmall=cv2.pyrDown(im)
        if im  == None:
            continue
        cv2.line(imsmall,(650/2,int(equ(650/2))),(3350/2,int(equ(3350/2))),(255,0,0),1)
        #cv2.line(imsmall,(650/2,headmax),(3350/2,headmax),(0,255,0),1)
        #cv2.line(imsmall,(650/2,midmax),(3350/2,midmax),(0,0,255),1) 
        #cv2.line(imsmall,(650/2,tailmax),(3350/2,tailmax),(255,0,255),1)  
        cv2.imwrite("./a/xaxx"+str(iPos)+".jpg",imsmall)       
    '''   
    realpos=min(geartop)*2
    realbottom=max(gearbottom)*2
    
    for iPos in range( 1, 31):        
        strim= str(iPos)+'.jpg'
        im=cv2.imread(strim)
        if im  == None:
            continue
        cv2.line(im,(650,realpos),(3350,realpos),(0,0,255),1)
        cv2.line(im,(650,realbottom),(3350,realbottom),(0,0,255),1)  
        cv2.imwrite("./a/xaxx"+str(iPos)+".jpg",im)
'''
    #cv2.imshow('1',im)
    #cv2.waitKey(0)
    #cv2.destroyAllWindows()
    