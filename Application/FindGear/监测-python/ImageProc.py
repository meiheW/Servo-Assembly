# -*- coding: utf-8 -*-
"""
Created on Tue Dec 08 16:31:38 2015

@author: K
"""

import cv2
import numpy as np

ptstart1=(650,1350)
ptend1=  (3350,1350)  
ptstart2=(650,1500)
ptend2=  (3350,1500)   

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
    x1   = (int)(imrect[0]/4)
    y1   = (int)(imrect[1]/4)
    x2   = (int)(imrect[2]/4)
    y2   = (int)(imrect[3]/4)      
    nsizey=(int)((y2-y1)/gridy)    
    nlistx=[]
    nmax=0
    ypos=0
    colortaby=np.zeros( (x2-x1,nsizey),dtype=np.ubyte)
    
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
    n = len(nlistx)
    for i in nlistx[:]:
         cv2.line(im,(i,ypos+20),( i,ypos-20),(0,0,255),1)
   
    #for i in range(0,nsizey):
    #    plt.plot(colortaby[:,i])    
    #plt.savefig("test3.png", dpi=750)
    cv2.imwrite('c:/temp/geara.jpg',im)    
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
    n = (int)(w/gridx+1)
    colortab=np.zeros((y2-y1,n),dtype=np.ubyte)    
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
    plt.savefig("bxx"+str(myn)+".png", dpi=750)     '''
    return   ret,addr  

def getgearBottom(imgray,x1,x2,ypos, imgshow,myn):
    
    y1=ypos-40
    y2=ypos+40
    gridx=10
    w = x2-x1
    n = (int)(w/gridx)+1
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

def  processOneImg(strim, bOutput=False):
    
    #strimgfile = strim.decode('utf-8').encode('gbk')
    #strimgfile = strim.encode('gbk')
    #print( strimgfile )
    
    im=cv2.imread(strim ) 
    
    if im  is None:
        return None,None,None
        
    imgray=cv2.cvtColor(im,cv2.COLOR_BGR2GRAY)     
    h,w = im.shape[:2]    
    imgraysmall=cv2.pyrDown(imgray)
    imgraytiny=cv2.pyrDown(imgraysmall)     
    imsmall=cv2.pyrDown(im)
    imtiny=cv2.pyrDown(imsmall)  
    
    ypos,nlistx = getGearSum(imgraytiny,imtiny,(650,1350,3350,1500),2)
    
   
    ypos *=2    
    rlistx=[x*2 for x in nlistx[:]]       
    gearsum = (int)(len(rlistx)/2)

    minposlist=[]    
    nminpos=10000
    for i in range(0,gearsum):
        h,addr = getgearHight(imgraysmall,rlistx[i*2],rlistx[i*2+1],ypos, imsmall,i)
        minposlist.append((addr,h))
        if nminpos > h:
            nminpos=h

    
    nmaxpos=0 
    maxposlist=[]    
    for i in range(0,gearsum-1):
        h,addr = getgearBottom(imgraysmall,rlistx[i*2+1],rlistx[i*2+2],ypos, imsmall,i)
        maxposlist.append((addr,h))
        if nmaxpos < h:
            nmaxpos=h  
    outstr = "c:/temp/1.jpg"
    
    if bOutput:
        cv2.line(imsmall,(int(650/2),nminpos),(int(3350/2),nminpos),(0,255,0),1)
        cv2.line(imsmall,(int(650/2),nmaxpos),(int(3350/2),nmaxpos),(0,255,0),1)
        
        cv2.imwrite(outstr,imsmall)
    
    
    return outstr,minposlist,maxposlist

    
    
    
def  processImg(strpath):
     #print strim
    allgeartop=[]#所有图片齿的最大的集合
    allgearbottom=[]#所有图片齿的最大的集合 
    
    for iPos in range( 0, 31):  
        strim = strpath + '/'+str(iPos)+'.jpg'
        strfile,minposlist,maxposlist = processOneImg(strim,True)
        if minposlist != None:
            allgeartop.append(minposlist)
        if maxposlist != None:
            allgearbottom.append(maxposlist)
    
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
                headaddr=addr
            elif tailmax > topmax and addr >1200:
                tailmax=topmax
                tailaddr=addr
            elif midmax > topmax and addr <=1200 and addr >=800:
                midmax = topmax
                midaddr=addr
    addrlist=[headaddr,midaddr,tailaddr]
    maxposlist=[headmax,midmax,tailmax]            
    a= np.polyfit(addrlist,maxposlist,1 )    
    #myangle=math.atan(a[0])*180.0/math.pi   
    #equ = np.poly1d(a)
    #print myangle,a   
    return a 
    
def  processShowImg(strpath, pos, equa):
    
    #strimgfile = strpath.decode('utf-8').encode('gbk')
    strimgfile = strpath # strpath.encode('gbk')    
    strimgfile = strpath + "/"+str(pos)+".jpg"
    im=cv2.imread(strimgfile) 
    if im  is None:
        return None    
    imsmall=cv2.pyrDown(im)    
    equ = np.poly1d(equa)
    outstr = "c:/temp/2.jpg"
    cv2.line(imsmall,(int(650/2),int(equ(650/2))),(int(3350/2),int(equ(3350/2))),(0,255,0),1)
    cv2.imwrite(outstr,imsmall)
    return  outstr

if __name__ == '__main__': 
    a = processImg('D:/work/cvtest/522/3ok')
    #print (a)
    #print (processShowImg('D:/work/cvtest/5月22日/3号齿位 ok',2,a))
    #print (a)


    