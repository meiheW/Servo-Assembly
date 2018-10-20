# -*- coding: utf-8 -*-
"""
Created on Tue Dec 08 14:20:01 2015

@author: K
"""

from PyQt5.QtWidgets import (QMainWindow,QDockWidget,QListWidget,QGraphicsView,
                        QSizePolicy,QToolBox,QGridLayout,QWidget,QHBoxLayout,QVBoxLayout,
                        QToolButton,QButtonGroup,QLabel,QLineEdit,QListView,QAction,
                        QMessageBox,QScrollArea,QPushButton,QGroupBox,QTextEdit,QTabWidget,
                        QFrame,QGraphicsPixmapItem,QApplication,QSystemTrayIcon,QMenu,
                        QFileDialog,QDialog)
from PyQt5.QtCore import (Qt,QThread,pyqtSignal,QSize,QTimer,QFileInfo)
from PyQt5.QtGui import QIcon,QFont,QPixmap,QImage,QPalette,QBrush,QColor,QPainter
import cv2
from  ImageProc import processOneImg,processImg,processShowImg
import math

class TestUI(QDialog):
    def __init__(self,parent=None):
        super(TestUI,self).__init__(parent)
        
        self.btnOpen=QPushButton("打开图片")
        self.btnGroupOpen=QPushButton("打开路径")
        self.labResult = QLineEdit()
        self.labAngle = QLineEdit()
        self.btnPrevImage=QPushButton("上一个")
        self.btnNextImage=QPushButton("下一个")
        self.btnClose=QPushButton("关闭")
        
        self.btnscale=QPushButton("自适应")        
        self.btnimgorisize=QPushButton("原始尺寸")
        
        self.nImagePos=0
        self.Equa=None
        self.angle=0
        self.imagePath=None
        
        self.imgWidget=QLabel()
        self.filename='C:\\Temp\\testgray.jpg'
        self.img = QImage()        
        self.img.load(self.filename)        
        self.imgWidget.setPixmap(QPixmap.fromImage(self.img))
        self.imgWidget.resize(self.img.size())
        self.scrollArea = QScrollArea(); 
        self.scrollArea.setWidget(self.imgWidget);        
        self.labResult.setText("0")
        self.labAngle.setText("0")
        

        grid = QGridLayout()       
        grid.addWidget(self.btnOpen, 0, 0)
        grid.addWidget(self.btnGroupOpen, 1, 0)
        grid.addWidget(self.labResult, 2, 0)
        grid.addWidget(self.btnPrevImage, 3, 0)
        grid.addWidget(self.btnNextImage, 4, 0)
        grid.addWidget(self.labAngle, 5, 0)
        grid.addWidget( self.btnscale, 6, 0)
        grid.addWidget( self.btnimgorisize, 7, 0)
        grid.addWidget( self.btnClose, 8, 0)
        
        
        
       
        grid.addWidget(self.scrollArea,0,1,12,1)
        
        grid.setColumnStretch(0,1)
        grid.setColumnStretch(1,20)

        self.setLayout(grid)
        
        self.imgWidget.setBackgroundRole(QPalette.Base)
        self.imgWidget.setSizePolicy(QSizePolicy.Ignored, QSizePolicy.Ignored)   
        self.imgWidget.setScaledContents(True)
        
        #self.btnClose.clicked.connect(self.close())
        #self.connect(self.btnClose, SIGNAL('clicked()'), SLOT('close()'))
        
        self.btnOpen.clicked.connect(self.on_btnOpen_clicked)
        self.btnGroupOpen.clicked.connect(self.on_btnGroupOpen_clicked)
        self.btnPrevImage.clicked.connect(self.on_btnPrev_clicked)
        self.btnNextImage.clicked.connect(self.on_btnNext_clicked)
        self.btnClose.clicked.connect( self.close)
        self.btnscale.clicked.connect( self.onscale)
        self.btnimgorisize.clicked.connect( self.onnormalsize)
        
        #self.connect( self.btnOpen, SIGNAL( 'clicked()' ), self.on_btnOpen_clicked )
        #self.connect( self.btnGroupOpen, SIGNAL( 'clicked()' ), self.on_btnGroupOpen_clicked )  
        #self.connect( self.btnPrevImage, SIGNAL( 'clicked()' ), self.on_btnPrev_clicked ) 
        #self.connect( self.btnNextImage, SIGNAL( 'clicked()' ), self.on_btnNext_clicked ) 
        #self.resize(1600,768)
        self.showMaximized()        
        self.setWindowTitle('图像处理')
        
        self.myscale=1.0
    def onscale(self):
        self.myscale=0.8
        self.scale_image()
    def onnormalsize(self):
        self.myscale=1.0
        self.scale_image()
    def on_btnPrev_clicked(self):
        self.nImagePos = self.nImagePos -1
        if self.nImagePos < 0:
            self.nImagePos=30   
        
        self.labResult.setText(str(self.nImagePos)) 
        self.show_CurImage()         
    def on_btnNext_clicked(self):
        self.nImagePos = self.nImagePos +1
        if self.nImagePos >30:
            self.nImagePos=0
            
        self.labResult.setText(str(self.nImagePos)) 
        self.show_CurImage()       
    def on_btnGroupOpen_clicked(self):      
        dlg = QFileDialog(self)      
        filename = dlg.getOpenFileName()  
        #self.filename = QString(dlg.getOpenFileName()).toUtf8()
        #self.filename = unicode(dlg.getOpenFileName().toUtf8(),'utf8', 'ignore')
        fi = QFileInfo(filename[0])
        

        self.imagePath=fi.absolutePath()
        
        #self.imagePath=unicode(fi.absolutePath().toUtf8(),'utf8', 'ignore')
        
        self.Equa = processImg(self.imagePath)        
        self.nImagePos=25        
        myangle=math.atan(self.Equa[0])*180.0/math.pi         
        self.labAngle.setText(str(myangle))
        self.labResult.setText(str(self.nImagePos)) 
        self.show_CurImage()
        
    def scale_image(self):
        self.imgWidget.resize(self.myscale* self.imgWidget.pixmap().size())
        self.imgWidget.update()
        
        
    def show_CurImage(self):
        strimg = processShowImg(self.imagePath,self.nImagePos,self.Equa )
        if strimg != None:
            self.img.load(strimg)        
            self.imgWidget.setPixmap(QPixmap.fromImage(self.img))
            self.scale_image()
            #self.imgWidget.resize(self.img.size())
        
    def on_btnOpen_clicked(self):
        #QMessageBox.information( self, "Pyqt", "information" )       
        dlg = QFileDialog(self)        
        strfile = dlg.getOpenFileName()    
        self.filename = strfile
        
        #unicode(QString(strfile).toUtf8(),'utf8', 'ignore')
        
        #self.filename = unicode(dlg.getOpenFileName().toUtf8(),'utf8', 'ignore')

        #print self.filename
        from os.path import isfile   
        x= self.filename[0]
        
        if isfile(x):
           
           retimg,a,b = processOneImg(x,True)
           #print(retimg)
           self.img.load(retimg)        
           self.imgWidget.setPixmap(QPixmap.fromImage(self.img))
           self.scale_image()
           #self.imgWidget.resize(self.img.size())
           #self.imgWidget.adjustSize()
           
           #self.imgWidget.resize(0.6* self.imgWidget.pixmap().size())

    def ShowMat(self, matImage):
        if matImage.type() == cv2.CV_8UC1:            
            self.img = QImage(QSize(matImage.cols, matImage.rows),QImage.Format_Mono )
            #ptr = matImage.bits()     
        elif matImage.type() == cv2.CV_8UC3:
            self.img = QImage(QSize(matImage.cols, matImage.rows),QImage.Format_RGB888 )        
        self.imgWidget.setPixmap(QPixmap.fromImage(self.img))
        #self.imgWidget.resize(self.img.size())
        
        self.imgWidget.adjustSize()
        

if __name__=='__main__':
    import sys    
    app = QApplication(sys.argv)
    mainform = TestUI()
    mainform.show()
    app.exec_()
    #sys.exit()