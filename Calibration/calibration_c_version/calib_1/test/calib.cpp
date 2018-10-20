#include<HalconC.h>
#include<iostream>
using namespace std;

int main()
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
  Hobject  ho_Image, ho_CurImage, ho_Contours;

  /* Local control variables */
  Htuple  hv_Image_X1, hv_Image_Y1, hv_Width, hv_Height;
  Htuple  hv_CalibDataID, hv_StartCamPar, hv_CaltabName, hv_ImageNum;
  Htuple  hv_I, hv_RCoord, hv_CCoord, hv_Index, hv_StartPose;
  Htuple  hv_Error, hv_CamParam, hv_PoseCalib, hv_CamParIn;
  Htuple  hv_PoseNewOrigin, hv_World_X1, hv_World_Y1, hv_Pic_Y1;
  Htuple  hv_Pic_X1, hv_Wor_X1, hv_Wor_Y1;

  /* Initialize iconic variables */
  gen_empty_obj(&ho_Image);
  gen_empty_obj(&ho_CurImage);
  gen_empty_obj(&ho_Contours);

  /* Initialize control variables */
  create_tuple(&hv_Image_X1,0);
  create_tuple(&hv_Image_Y1,0);
  create_tuple(&hv_Width,0);
  create_tuple(&hv_Height,0);
  create_tuple(&hv_CalibDataID,0);
  create_tuple(&hv_StartCamPar,0);
  create_tuple(&hv_CaltabName,0);
  create_tuple(&hv_ImageNum,0);
  create_tuple(&hv_I,0);
  create_tuple(&hv_RCoord,0);
  create_tuple(&hv_CCoord,0);
  create_tuple(&hv_Index,0);
  create_tuple(&hv_StartPose,0);
  create_tuple(&hv_Error,0);
  create_tuple(&hv_CamParam,0);
  create_tuple(&hv_PoseCalib,0);
  create_tuple(&hv_CamParIn,0);
  create_tuple(&hv_PoseNewOrigin,0);
  create_tuple(&hv_World_X1,0);
  create_tuple(&hv_World_Y1,0);
  create_tuple(&hv_Pic_Y1,0);
  create_tuple(&hv_Pic_X1,0);
  create_tuple(&hv_Wor_X1,0);
  create_tuple(&hv_Wor_Y1,0);

  /****************************************************/
  /******************   Begin procedure   *************/
  /****************************************************/

  /*Image_X1 := 295,Image_Y1 := 132*/
  
  /*Image_X1 := 295*/
  reuse_tuple_i(&hv_Image_X1,295);

  /*Image_Y1 := 132*/
  reuse_tuple_i(&hv_Image_Y1,132);


  /***读取图像，获取尺寸****/
  /*read_image (Image, 'C:/Users/Administrator/Desktop/calibration/scratch_perspective.png')*/
  clear_obj(ho_Image);
  /***/read_image(&ho_Image, "C:/Users/Administrator/Desktop/calibration/scratch_perspective.png");

  /*get_image_size (Image, Width, Height)*/
  destroy_tuple(hv_Width);
  destroy_tuple(hv_Height);
  /***/T_get_image_size(ho_Image, &hv_Width, &hv_Height);


  /***设置标定数据****/
  /*create_calib_data ('calibration_object', 1, 1, CalibDataID)*/
  create_tuple_s(&TTemp[SP++],"calibration_object");
  create_tuple_i(&TTemp[SP++],1);
  create_tuple_i(&TTemp[SP++],1);
  destroy_tuple(hv_CalibDataID);
  /***/T_create_calib_data(TTemp[SP-3], TTemp[SP-2], TTemp[SP-1], &hv_CalibDataID);
  destroy_tuple(TTemp[--SP]);
  destroy_tuple(TTemp[--SP]);
  destroy_tuple(TTemp[--SP]);


  /***相机内参和标定板****/
  /*StartCamPar := [0.012,0,0.0000055,0.0000055,0.5*Width,0.5*Height, Width, Height]*/
  create_tuple(&TTemp[SP++],4);
  set_d(TTemp[SP-1],0.012  ,0);
  set_i(TTemp[SP-1],0  ,1);
  set_d(TTemp[SP-1],0.0000055  ,2);
  set_d(TTemp[SP-1],0.0000055  ,3);
  create_tuple_d(&TTemp[SP++],0.5);
  T_tuple_mult(TTemp[SP-1],hv_Width,&TTemp[SP]);
  destroy_tuple(TTemp[SP-1]);
  TTemp[SP-1]=TTemp[SP];
  T_tuple_concat(TTemp[SP-2],TTemp[SP-1],&TTemp[SP]);
  destroy_tuple(TTemp[SP-2]);
  destroy_tuple(TTemp[SP-1]);
  TTemp[SP-2]=TTemp[SP];
  SP=SP-1;
  create_tuple_d(&TTemp[SP++],0.5);
  T_tuple_mult(TTemp[SP-1],hv_Height,&TTemp[SP]);
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

  /*set_calib_data_cam_param (CalibDataID, 0, 'area_scan_division', StartCamPar)*/
  create_tuple_i(&TTemp[SP++],0);
  create_tuple_s(&TTemp[SP++],"area_scan_division");
  /***/T_set_calib_data_cam_param(hv_CalibDataID, TTemp[SP-2], TTemp[SP-1], hv_StartCamPar);
  destroy_tuple(TTemp[--SP]);
  destroy_tuple(TTemp[--SP]);

  /*CaltabName := 'caltab_30mm.descr'*/
  reuse_tuple_s(&hv_CaltabName,"caltab_30mm.descr");
  /*set_calib_data_calib_object (CalibDataID, 0, CaltabName)*/
  create_tuple_i(&TTemp[SP++],0);
  /***/T_set_calib_data_calib_object(hv_CalibDataID, TTemp[SP-1], hv_CaltabName);
  destroy_tuple(TTemp[--SP]);


  /***加载图像，标定数据****/
  /*ImageNum := 12*/
  reuse_tuple_i(&hv_ImageNum,12);

  /*========== for I := 1 to ImageNum by 1 ==========*/
  copy_tuple(hv_ImageNum,&TTemp[SP++]);
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

    /*read_image (CurImage, 'C:/Users/Administrator/Desktop/calibration/scratch_calib_'+I$'02d')*/
    create_tuple_s(&TTemp[SP++],"C:/Users/Administrator/Desktop/calibration/scratch_calib_");
    create_tuple_s(&TTemp[SP++],"02d");
    T_tuple_string(hv_I,TTemp[SP-1],&TTemp[SP]);
    destroy_tuple(TTemp[SP-1]);
    TTemp[SP-1]=TTemp[SP];
    T_tuple_add(TTemp[SP-2],TTemp[SP-1],&TTemp[SP]);
    destroy_tuple(TTemp[SP-2]);
    destroy_tuple(TTemp[SP-1]);
    TTemp[SP-2]=TTemp[SP];
    SP=SP-1;
    clear_obj(ho_CurImage);
    /***/T_read_image(&ho_CurImage, TTemp[SP-1]);
    destroy_tuple(TTemp[--SP]);

    /*find_calib_object (CurImage, CalibDataID, 0, 0, I, [], [])*/
    create_tuple_i(&TTemp[SP++],0);
    create_tuple_i(&TTemp[SP++],0);
    create_tuple(&TTemp[SP++],0);
    create_tuple(&TTemp[SP++],0);
    /***/T_find_calib_object(ho_CurImage, hv_CalibDataID, TTemp[SP-4], TTemp[SP-3], 
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

    /*get_calib_data_observ_points (CalibDataID, 0, 0, I, RCoord, CCoord, Index, StartPose)*/
    create_tuple_i(&TTemp[SP++],0);
    create_tuple_i(&TTemp[SP++],0);
    destroy_tuple(hv_RCoord);
    destroy_tuple(hv_CCoord);
    destroy_tuple(hv_Index);
    destroy_tuple(hv_StartPose);
    /***/T_get_calib_data_observ_points(hv_CalibDataID, TTemp[SP-2], TTemp[SP-1], 
        hv_I, &hv_RCoord, &hv_CCoord, &hv_Index, &hv_StartPose);
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


  /***确定相机参数****/
  /*calibrate_cameras (CalibDataID, Error)*/
  destroy_tuple(hv_Error);
  /***/T_calibrate_cameras(hv_CalibDataID, &hv_Error);

  /*get_calib_data (CalibDataID, 'camera', 0, 'params', CamParam)*/
  create_tuple_s(&TTemp[SP++],"camera");
  create_tuple_i(&TTemp[SP++],0);
  create_tuple_s(&TTemp[SP++],"params");
  destroy_tuple(hv_CamParam);
  /***/T_get_calib_data(hv_CalibDataID, TTemp[SP-3], TTemp[SP-2], TTemp[SP-1], &hv_CamParam);
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


  /*** 内参赋值语句CamParIn := CamParam****/
  /*CamParIn := CamParam*/
  destroy_tuple(hv_CamParIn);
  copy_tuple(hv_CamParam,&hv_CamParIn);


  /***外参调整****/
  /*set_origin_pose (PoseCalib, 0, 0, 0.001, PoseNewOrigin)*/
  create_tuple_i(&TTemp[SP++],0);
  create_tuple_i(&TTemp[SP++],0);
  create_tuple_d(&TTemp[SP++],0.001);
  destroy_tuple(hv_PoseNewOrigin);
  /***/T_set_origin_pose(hv_PoseCalib, TTemp[SP-3], TTemp[SP-2], TTemp[SP-1], &hv_PoseNewOrigin);
  destroy_tuple(TTemp[--SP]);
  destroy_tuple(TTemp[--SP]);
  destroy_tuple(TTemp[--SP]);


  /***坐标转换****/
  /*image_points_to_world_plane (CamParam, PoseNewOrigin, Image_Y1, Image_X1, 'm', World_X1, World_Y1)*/
  create_tuple_s(&TTemp[SP++],"m");
  destroy_tuple(hv_World_X1);
  destroy_tuple(hv_World_Y1);
  /***/T_image_points_to_world_plane(hv_CamParam, hv_PoseNewOrigin, hv_Image_Y1, 
      hv_Image_X1, TTemp[SP-1], &hv_World_X1, &hv_World_Y1);
  destroy_tuple(TTemp[--SP]);


  /***类型转换****/
  /*tuple_int (Image_Y1, Pic_Y1)*/
  destroy_tuple(hv_Pic_Y1);
  /***/T_tuple_int(hv_Image_Y1, &hv_Pic_Y1);

  /*tuple_int (Image_X1, Pic_X1)*/
  destroy_tuple(hv_Pic_X1);
  /***/T_tuple_int(hv_Image_X1, &hv_Pic_X1);

  /*tuple_real (World_X1, Wor_X1)*/
  destroy_tuple(hv_Wor_X1);
  /***/T_tuple_real(hv_World_X1, &hv_Wor_X1);

  /*tuple_real (World_Y1, Wor_Y1)*/
  destroy_tuple(hv_Wor_Y1);
  /***/T_tuple_real(hv_World_Y1, &hv_Wor_Y1);


  int IY1,IX1;
  float WY1,WX1;
  IY1=hv_Pic_Y1.val.l;
  IX1=hv_Pic_X1.val.l;
  WY1=hv_Wor_X1.val.f;
  WX1=hv_Wor_Y1.val.f;


  cout<<IY1<<" "<<IX1<<" "<<WY1<<" "<<WX1<<" "<<endl;
  
  system("pause");

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
  clear_obj(ho_CurImage);
  clear_obj(ho_Contours);

  /* Clear local control variables */
  destroy_tuple(hv_Image_X1);
  destroy_tuple(hv_Image_Y1);
  destroy_tuple(hv_Width);
  destroy_tuple(hv_Height);
  destroy_tuple(hv_CalibDataID);
  destroy_tuple(hv_StartCamPar);
  destroy_tuple(hv_CaltabName);
  destroy_tuple(hv_ImageNum);
  destroy_tuple(hv_I);
  destroy_tuple(hv_RCoord);
  destroy_tuple(hv_CCoord);
  destroy_tuple(hv_Index);
  destroy_tuple(hv_StartPose);
  destroy_tuple(hv_Error);
  destroy_tuple(hv_CamParam);
  destroy_tuple(hv_PoseCalib);
  destroy_tuple(hv_CamParIn);
  destroy_tuple(hv_PoseNewOrigin);
  destroy_tuple(hv_World_X1);
  destroy_tuple(hv_World_Y1);
  destroy_tuple(hv_Pic_Y1);
  destroy_tuple(hv_Pic_X1);
  destroy_tuple(hv_Wor_X1);
  destroy_tuple(hv_Wor_Y1);





}