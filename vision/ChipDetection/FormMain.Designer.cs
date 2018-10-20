namespace ChipDetection
{
    partial class FormChipDetection
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.picImage = new System.Windows.Forms.PictureBox();
            this.pnlTop = new System.Windows.Forms.Panel();
            this.lblMark = new System.Windows.Forms.Label();
            this.lblStatusIndicator = new System.Windows.Forms.Label();
            this.lblCodeValue = new System.Windows.Forms.Label();
            this.lblCodeString = new System.Windows.Forms.Label();
            this.pnlRight = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.grpMotor = new System.Windows.Forms.GroupBox();
            this.btnStopMotor = new System.Windows.Forms.Button();
            this.btnSnap = new System.Windows.Forms.Button();
            this.grpCalibration = new System.Windows.Forms.GroupBox();
            this.btnStopGrab = new System.Windows.Forms.Button();
            this.btnSaveCalibration = new System.Windows.Forms.Button();
            this.btnGrab = new System.Windows.Forms.Button();
            this.lblCalibrationStatus = new System.Windows.Forms.Label();
            this.grpParameter = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.numHeightMin = new System.Windows.Forms.NumericUpDown();
            this.numWidthMin = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.lblWidthMax = new System.Windows.Forms.Label();
            this.numHeightMax = new System.Windows.Forms.NumericUpDown();
            this.numWidthMax = new System.Windows.Forms.NumericUpDown();
            this.chkCalibration = new System.Windows.Forms.CheckBox();
            this.lblDebug = new System.Windows.Forms.Label();
            this.pnlControl = new System.Windows.Forms.Panel();
            this.button2 = new System.Windows.Forms.Button();
            this.btnTest = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.pnlResult = new System.Windows.Forms.Panel();
            this.lbl4Val = new System.Windows.Forms.Label();
            this.lbl4TimeString = new System.Windows.Forms.Label();
            this.lbl3Val = new System.Windows.Forms.Label();
            this.lbl3String = new System.Windows.Forms.Label();
            this.lbl2Val = new System.Windows.Forms.Label();
            this.lbl1Val = new System.Windows.Forms.Label();
            this.lbl2String = new System.Windows.Forms.Label();
            this.lbl1String = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.mnuMain = new System.Windows.Forms.MenuStrip();
            this.mnuUser = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuUserManager = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuUserLogin = new System.Windows.Forms.ToolStripMenuItem();
            this.相机测试ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.videorunToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.videostopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.picImage)).BeginInit();
            this.pnlTop.SuspendLayout();
            this.pnlRight.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.grpMotor.SuspendLayout();
            this.grpCalibration.SuspendLayout();
            this.grpParameter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numHeightMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numWidthMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHeightMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numWidthMax)).BeginInit();
            this.pnlControl.SuspendLayout();
            this.pnlResult.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.mnuMain.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // picImage
            // 
            this.picImage.BackColor = System.Drawing.Color.White;
            this.picImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picImage.Location = new System.Drawing.Point(0, 0);
            this.picImage.Margin = new System.Windows.Forms.Padding(4);
            this.picImage.Name = "picImage";
            this.picImage.Size = new System.Drawing.Size(904, 848);
            this.picImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picImage.TabIndex = 0;
            this.picImage.TabStop = false;
            // 
            // pnlTop
            // 
            this.pnlTop.BackColor = System.Drawing.Color.Transparent;
            this.pnlTop.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlTop.Controls.Add(this.lblMark);
            this.pnlTop.Controls.Add(this.lblStatusIndicator);
            this.pnlTop.Controls.Add(this.lblCodeValue);
            this.pnlTop.Controls.Add(this.lblCodeString);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Margin = new System.Windows.Forms.Padding(4);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(1692, 42);
            this.pnlTop.TabIndex = 1;
            // 
            // lblMark
            // 
            this.lblMark.AutoSize = true;
            this.lblMark.Font = new System.Drawing.Font("微软雅黑", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblMark.ForeColor = System.Drawing.Color.Red;
            this.lblMark.Location = new System.Drawing.Point(440, -1);
            this.lblMark.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMark.Name = "lblMark";
            this.lblMark.Size = new System.Drawing.Size(114, 50);
            this.lblMark.TabIndex = 5;
            this.lblMark.Text = "2017";
            // 
            // lblStatusIndicator
            // 
            this.lblStatusIndicator.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblStatusIndicator.BackColor = System.Drawing.Color.Red;
            this.lblStatusIndicator.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblStatusIndicator.ForeColor = System.Drawing.Color.White;
            this.lblStatusIndicator.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblStatusIndicator.Location = new System.Drawing.Point(1557, -1);
            this.lblStatusIndicator.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblStatusIndicator.Name = "lblStatusIndicator";
            this.lblStatusIndicator.Size = new System.Drawing.Size(133, 42);
            this.lblStatusIndicator.TabIndex = 2;
            this.lblStatusIndicator.Text = "停 止 运 行";
            this.lblStatusIndicator.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblCodeValue
            // 
            this.lblCodeValue.AutoSize = true;
            this.lblCodeValue.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblCodeValue.Location = new System.Drawing.Point(132, 11);
            this.lblCodeValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCodeValue.Name = "lblCodeValue";
            this.lblCodeValue.Size = new System.Drawing.Size(173, 27);
            this.lblCodeValue.TabIndex = 1;
            this.lblCodeValue.Text = "FM03603002-08";
            // 
            // lblCodeString
            // 
            this.lblCodeString.AutoSize = true;
            this.lblCodeString.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblCodeString.Location = new System.Drawing.Point(16, 11);
            this.lblCodeString.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCodeString.Name = "lblCodeString";
            this.lblCodeString.Size = new System.Drawing.Size(112, 27);
            this.lblCodeString.TabIndex = 0;
            this.lblCodeString.Text = "产品编码：";
            // 
            // pnlRight
            // 
            this.pnlRight.Controls.Add(this.groupBox1);
            this.pnlRight.Controls.Add(this.pnlControl);
            this.pnlRight.Controls.Add(this.pnlResult);
            this.pnlRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlRight.Location = new System.Drawing.Point(0, 0);
            this.pnlRight.Margin = new System.Windows.Forms.Padding(4);
            this.pnlRight.Name = "pnlRight";
            this.pnlRight.Size = new System.Drawing.Size(783, 848);
            this.pnlRight.TabIndex = 2;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.grpMotor);
            this.groupBox1.Controls.Add(this.grpCalibration);
            this.groupBox1.Controls.Add(this.lblCalibrationStatus);
            this.groupBox1.Controls.Add(this.grpParameter);
            this.groupBox1.Controls.Add(this.chkCalibration);
            this.groupBox1.Controls.Add(this.lblDebug);
            this.groupBox1.Location = new System.Drawing.Point(8, 264);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(469, 509);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            // 
            // grpMotor
            // 
            this.grpMotor.Controls.Add(this.btnStopMotor);
            this.grpMotor.Controls.Add(this.btnSnap);
            this.grpMotor.Location = new System.Drawing.Point(8, 18);
            this.grpMotor.Margin = new System.Windows.Forms.Padding(4);
            this.grpMotor.Name = "grpMotor";
            this.grpMotor.Padding = new System.Windows.Forms.Padding(4);
            this.grpMotor.Size = new System.Drawing.Size(448, 89);
            this.grpMotor.TabIndex = 9;
            this.grpMotor.TabStop = false;
            this.grpMotor.Text = "手动测试";
            // 
            // btnStopMotor
            // 
            this.btnStopMotor.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnStopMotor.Location = new System.Drawing.Point(143, 32);
            this.btnStopMotor.Margin = new System.Windows.Forms.Padding(4);
            this.btnStopMotor.Name = "btnStopMotor";
            this.btnStopMotor.Size = new System.Drawing.Size(116, 36);
            this.btnStopMotor.TabIndex = 13;
            this.btnStopMotor.Text = "采图";
            this.btnStopMotor.UseVisualStyleBackColor = true;
            this.btnStopMotor.Click += new System.EventHandler(this.btnStopMotor_Click);
            // 
            // btnSnap
            // 
            this.btnSnap.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnSnap.Location = new System.Drawing.Point(19, 32);
            this.btnSnap.Margin = new System.Windows.Forms.Padding(4);
            this.btnSnap.Name = "btnSnap";
            this.btnSnap.Size = new System.Drawing.Size(116, 36);
            this.btnSnap.TabIndex = 12;
            this.btnSnap.Text = "拍音片";
            this.btnSnap.UseVisualStyleBackColor = true;
            this.btnSnap.Click += new System.EventHandler(this.Snap_Click);
            // 
            // grpCalibration
            // 
            this.grpCalibration.Controls.Add(this.btnStopGrab);
            this.grpCalibration.Controls.Add(this.btnSaveCalibration);
            this.grpCalibration.Controls.Add(this.btnGrab);
            this.grpCalibration.Location = new System.Drawing.Point(7, 108);
            this.grpCalibration.Margin = new System.Windows.Forms.Padding(4);
            this.grpCalibration.Name = "grpCalibration";
            this.grpCalibration.Padding = new System.Windows.Forms.Padding(4);
            this.grpCalibration.Size = new System.Drawing.Size(449, 74);
            this.grpCalibration.TabIndex = 8;
            this.grpCalibration.TabStop = false;
            this.grpCalibration.Text = "标定";
            // 
            // btnStopGrab
            // 
            this.btnStopGrab.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnStopGrab.Location = new System.Drawing.Point(144, 25);
            this.btnStopGrab.Margin = new System.Windows.Forms.Padding(4);
            this.btnStopGrab.Name = "btnStopGrab";
            this.btnStopGrab.Size = new System.Drawing.Size(116, 36);
            this.btnStopGrab.TabIndex = 11;
            this.btnStopGrab.Text = "停止";
            this.btnStopGrab.UseVisualStyleBackColor = true;
            this.btnStopGrab.Click += new System.EventHandler(this.Freeze_Click);
            // 
            // btnSaveCalibration
            // 
            this.btnSaveCalibration.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnSaveCalibration.Location = new System.Drawing.Point(267, 25);
            this.btnSaveCalibration.Margin = new System.Windows.Forms.Padding(4);
            this.btnSaveCalibration.Name = "btnSaveCalibration";
            this.btnSaveCalibration.Size = new System.Drawing.Size(169, 36);
            this.btnSaveCalibration.TabIndex = 3;
            this.btnSaveCalibration.Text = "计算结果";
            this.btnSaveCalibration.UseVisualStyleBackColor = true;
            this.btnSaveCalibration.Click += new System.EventHandler(this.Calculate_Click);
            // 
            // btnGrab
            // 
            this.btnGrab.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnGrab.Location = new System.Drawing.Point(20, 25);
            this.btnGrab.Margin = new System.Windows.Forms.Padding(4);
            this.btnGrab.Name = "btnGrab";
            this.btnGrab.Size = new System.Drawing.Size(116, 36);
            this.btnGrab.TabIndex = 10;
            this.btnGrab.Text = "拍音筒";
            this.btnGrab.UseVisualStyleBackColor = true;
            this.btnGrab.Click += new System.EventHandler(this.Grab_Click);
            // 
            // lblCalibrationStatus
            // 
            this.lblCalibrationStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblCalibrationStatus.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblCalibrationStatus.Location = new System.Drawing.Point(273, 185);
            this.lblCalibrationStatus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCalibrationStatus.Name = "lblCalibrationStatus";
            this.lblCalibrationStatus.Size = new System.Drawing.Size(183, 312);
            this.lblCalibrationStatus.TabIndex = 6;
            this.lblCalibrationStatus.Text = "L: 300 R:200";
            // 
            // grpParameter
            // 
            this.grpParameter.Controls.Add(this.label3);
            this.grpParameter.Controls.Add(this.label4);
            this.grpParameter.Controls.Add(this.numHeightMin);
            this.grpParameter.Controls.Add(this.numWidthMin);
            this.grpParameter.Controls.Add(this.label2);
            this.grpParameter.Controls.Add(this.lblWidthMax);
            this.grpParameter.Controls.Add(this.numHeightMax);
            this.grpParameter.Controls.Add(this.numWidthMax);
            this.grpParameter.Location = new System.Drawing.Point(11, 58);
            this.grpParameter.Margin = new System.Windows.Forms.Padding(4);
            this.grpParameter.Name = "grpParameter";
            this.grpParameter.Padding = new System.Windows.Forms.Padding(4);
            this.grpParameter.Size = new System.Drawing.Size(451, 21);
            this.grpParameter.TabIndex = 5;
            this.grpParameter.TabStop = false;
            this.grpParameter.Text = "参数";
            this.grpParameter.Visible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(231, 101);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(97, 15);
            this.label3.TabIndex = 7;
            this.label3.Text = "芯片高度下限";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(20, 101);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(97, 15);
            this.label4.TabIndex = 6;
            this.label4.Text = "芯片宽度下限";
            // 
            // numHeightMin
            // 
            this.numHeightMin.Location = new System.Drawing.Point(335, 99);
            this.numHeightMin.Margin = new System.Windows.Forms.Padding(4);
            this.numHeightMin.Maximum = new decimal(new int[] {
            960,
            0,
            0,
            0});
            this.numHeightMin.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numHeightMin.Name = "numHeightMin";
            this.numHeightMin.Size = new System.Drawing.Size(97, 25);
            this.numHeightMin.TabIndex = 5;
            this.numHeightMin.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // numWidthMin
            // 
            this.numWidthMin.Location = new System.Drawing.Point(125, 99);
            this.numWidthMin.Margin = new System.Windows.Forms.Padding(4);
            this.numWidthMin.Maximum = new decimal(new int[] {
            1280,
            0,
            0,
            0});
            this.numWidthMin.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numWidthMin.Name = "numWidthMin";
            this.numWidthMin.Size = new System.Drawing.Size(97, 25);
            this.numWidthMin.TabIndex = 4;
            this.numWidthMin.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(231, 68);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "芯片高度上限";
            // 
            // lblWidthMax
            // 
            this.lblWidthMax.AutoSize = true;
            this.lblWidthMax.Location = new System.Drawing.Point(20, 68);
            this.lblWidthMax.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblWidthMax.Name = "lblWidthMax";
            this.lblWidthMax.Size = new System.Drawing.Size(97, 15);
            this.lblWidthMax.TabIndex = 2;
            this.lblWidthMax.Text = "芯片宽度上限";
            // 
            // numHeightMax
            // 
            this.numHeightMax.Location = new System.Drawing.Point(335, 65);
            this.numHeightMax.Margin = new System.Windows.Forms.Padding(4);
            this.numHeightMax.Maximum = new decimal(new int[] {
            960,
            0,
            0,
            0});
            this.numHeightMax.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numHeightMax.Name = "numHeightMax";
            this.numHeightMax.Size = new System.Drawing.Size(97, 25);
            this.numHeightMax.TabIndex = 1;
            this.numHeightMax.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // numWidthMax
            // 
            this.numWidthMax.Location = new System.Drawing.Point(125, 65);
            this.numWidthMax.Margin = new System.Windows.Forms.Padding(4);
            this.numWidthMax.Maximum = new decimal(new int[] {
            1280,
            0,
            0,
            0});
            this.numWidthMax.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numWidthMax.Name = "numWidthMax";
            this.numWidthMax.Size = new System.Drawing.Size(97, 25);
            this.numWidthMax.TabIndex = 0;
            this.numWidthMax.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // chkCalibration
            // 
            this.chkCalibration.AutoSize = true;
            this.chkCalibration.Location = new System.Drawing.Point(8, 86);
            this.chkCalibration.Margin = new System.Windows.Forms.Padding(4);
            this.chkCalibration.Name = "chkCalibration";
            this.chkCalibration.Size = new System.Drawing.Size(89, 19);
            this.chkCalibration.TabIndex = 4;
            this.chkCalibration.Text = "标定模式";
            this.chkCalibration.UseVisualStyleBackColor = true;
            this.chkCalibration.Visible = false;
            // 
            // lblDebug
            // 
            this.lblDebug.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblDebug.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblDebug.Location = new System.Drawing.Point(7, 189);
            this.lblDebug.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDebug.Name = "lblDebug";
            this.lblDebug.Size = new System.Drawing.Size(245, 308);
            this.lblDebug.TabIndex = 3;
            this.lblDebug.Text = "L: 300 R:200";
            // 
            // pnlControl
            // 
            this.pnlControl.BackColor = System.Drawing.Color.Transparent;
            this.pnlControl.Controls.Add(this.button2);
            this.pnlControl.Controls.Add(this.btnTest);
            this.pnlControl.Controls.Add(this.button1);
            this.pnlControl.Controls.Add(this.btnStop);
            this.pnlControl.Controls.Add(this.btnStart);
            this.pnlControl.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlControl.Location = new System.Drawing.Point(0, 779);
            this.pnlControl.Margin = new System.Windows.Forms.Padding(4);
            this.pnlControl.Name = "pnlControl";
            this.pnlControl.Size = new System.Drawing.Size(783, 69);
            this.pnlControl.TabIndex = 1;
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button2.Location = new System.Drawing.Point(324, 18);
            this.button2.Margin = new System.Windows.Forms.Padding(4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(77, 36);
            this.button2.TabIndex = 3;
            this.button2.Text = "模版";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.Mytest1);
            // 
            // btnTest
            // 
            this.btnTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnTest.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnTest.Location = new System.Drawing.Point(201, 18);
            this.btnTest.Margin = new System.Windows.Forms.Padding(4);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(115, 36);
            this.btnTest.TabIndex = 2;
            this.btnTest.Text = "功能测试";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button1.Location = new System.Drawing.Point(409, 18);
            this.button1.Margin = new System.Windows.Forms.Padding(4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(72, 36);
            this.button1.TabIndex = 2;
            this.button1.Text = "测试";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Mytest);
            // 
            // btnStop
            // 
            this.btnStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnStop.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnStop.Location = new System.Drawing.Point(113, 18);
            this.btnStop.Margin = new System.Windows.Forms.Padding(4);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(80, 36);
            this.btnStop.TabIndex = 1;
            this.btnStop.Text = "停止";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnStart
            // 
            this.btnStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnStart.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnStart.Location = new System.Drawing.Point(23, 18);
            this.btnStart.Margin = new System.Windows.Forms.Padding(4);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(80, 36);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "启动";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // pnlResult
            // 
            this.pnlResult.BackColor = System.Drawing.Color.Transparent;
            this.pnlResult.Controls.Add(this.lbl4Val);
            this.pnlResult.Controls.Add(this.lbl4TimeString);
            this.pnlResult.Controls.Add(this.lbl3Val);
            this.pnlResult.Controls.Add(this.lbl3String);
            this.pnlResult.Controls.Add(this.lbl2Val);
            this.pnlResult.Controls.Add(this.lbl1Val);
            this.pnlResult.Controls.Add(this.lbl2String);
            this.pnlResult.Controls.Add(this.lbl1String);
            this.pnlResult.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlResult.Location = new System.Drawing.Point(0, 0);
            this.pnlResult.Margin = new System.Windows.Forms.Padding(4);
            this.pnlResult.Name = "pnlResult";
            this.pnlResult.Size = new System.Drawing.Size(783, 260);
            this.pnlResult.TabIndex = 0;
            // 
            // lbl4Val
            // 
            this.lbl4Val.AutoSize = true;
            this.lbl4Val.Font = new System.Drawing.Font("微软雅黑", 27.7F);
            this.lbl4Val.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lbl4Val.Location = new System.Drawing.Point(183, 196);
            this.lbl4Val.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl4Val.Name = "lbl4Val";
            this.lbl4Val.Size = new System.Drawing.Size(53, 60);
            this.lbl4Val.TabIndex = 8;
            this.lbl4Val.Text = "0";
            // 
            // lbl4TimeString
            // 
            this.lbl4TimeString.AutoSize = true;
            this.lbl4TimeString.Font = new System.Drawing.Font("微软雅黑", 27.25F, System.Drawing.FontStyle.Bold);
            this.lbl4TimeString.Location = new System.Drawing.Point(28, 198);
            this.lbl4TimeString.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl4TimeString.Name = "lbl4TimeString";
            this.lbl4TimeString.Size = new System.Drawing.Size(66, 60);
            this.lbl4TimeString.TabIndex = 7;
            this.lbl4TimeString.Text = "4:";
            // 
            // lbl3Val
            // 
            this.lbl3Val.AutoSize = true;
            this.lbl3Val.Font = new System.Drawing.Font("微软雅黑", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbl3Val.ForeColor = System.Drawing.Color.Red;
            this.lbl3Val.Location = new System.Drawing.Point(179, 134);
            this.lbl3Val.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl3Val.Name = "lbl3Val";
            this.lbl3Val.Size = new System.Drawing.Size(56, 62);
            this.lbl3Val.TabIndex = 6;
            this.lbl3Val.Text = "1";
            // 
            // lbl3String
            // 
            this.lbl3String.AutoSize = true;
            this.lbl3String.Font = new System.Drawing.Font("微软雅黑", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbl3String.Location = new System.Drawing.Point(28, 135);
            this.lbl3String.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl3String.Name = "lbl3String";
            this.lbl3String.Size = new System.Drawing.Size(69, 62);
            this.lbl3String.TabIndex = 5;
            this.lbl3String.Text = "3:";
            // 
            // lbl2Val
            // 
            this.lbl2Val.AutoSize = true;
            this.lbl2Val.Font = new System.Drawing.Font("微软雅黑", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbl2Val.ForeColor = System.Drawing.Color.LimeGreen;
            this.lbl2Val.Location = new System.Drawing.Point(179, 66);
            this.lbl2Val.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl2Val.Name = "lbl2Val";
            this.lbl2Val.Size = new System.Drawing.Size(143, 62);
            this.lbl2Val.TabIndex = 4;
            this.lbl2Val.Text = "3999";
            // 
            // lbl1Val
            // 
            this.lbl1Val.AutoSize = true;
            this.lbl1Val.Font = new System.Drawing.Font("微软雅黑", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbl1Val.Location = new System.Drawing.Point(179, 4);
            this.lbl1Val.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl1Val.Name = "lbl1Val";
            this.lbl1Val.Size = new System.Drawing.Size(143, 62);
            this.lbl1Val.TabIndex = 3;
            this.lbl1Val.Text = "4000";
            // 
            // lbl2String
            // 
            this.lbl2String.AutoSize = true;
            this.lbl2String.Font = new System.Drawing.Font("微软雅黑", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbl2String.Location = new System.Drawing.Point(27, 66);
            this.lbl2String.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl2String.Name = "lbl2String";
            this.lbl2String.Size = new System.Drawing.Size(69, 62);
            this.lbl2String.TabIndex = 2;
            this.lbl2String.Text = "2:";
            // 
            // lbl1String
            // 
            this.lbl1String.AutoSize = true;
            this.lbl1String.Font = new System.Drawing.Font("微软雅黑", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbl1String.Location = new System.Drawing.Point(28, 4);
            this.lbl1String.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl1String.Name = "lbl1String";
            this.lbl1String.Size = new System.Drawing.Size(69, 62);
            this.lbl1String.TabIndex = 1;
            this.lbl1String.Text = "1:";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 42);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.picImage);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.pnlRight);
            this.splitContainer1.Size = new System.Drawing.Size(1692, 848);
            this.splitContainer1.SplitterDistance = 904;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 3;
            // 
            // mnuMain
            // 
            this.mnuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuUser,
            this.相机测试ToolStripMenuItem});
            this.mnuMain.Location = new System.Drawing.Point(0, 0);
            this.mnuMain.Name = "mnuMain";
            this.mnuMain.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
            this.mnuMain.Size = new System.Drawing.Size(1692, 28);
            this.mnuMain.TabIndex = 4;
            this.mnuMain.Text = "用户权限";
            // 
            // mnuUser
            // 
            this.mnuUser.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuUserManager,
            this.mnuUserLogin});
            this.mnuUser.Name = "mnuUser";
            this.mnuUser.Size = new System.Drawing.Size(81, 24);
            this.mnuUser.Text = "用户权限";
            // 
            // mnuUserManager
            // 
            this.mnuUserManager.Name = "mnuUserManager";
            this.mnuUserManager.Size = new System.Drawing.Size(150, 24);
            this.mnuUserManager.Text = "权限管理...";
            // 
            // mnuUserLogin
            // 
            this.mnuUserLogin.Name = "mnuUserLogin";
            this.mnuUserLogin.Size = new System.Drawing.Size(150, 24);
            this.mnuUserLogin.Text = "用户登录...";
            // 
            // 相机测试ToolStripMenuItem
            // 
            this.相机测试ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.videorunToolStripMenuItem,
            this.videostopToolStripMenuItem});
            this.相机测试ToolStripMenuItem.Name = "相机测试ToolStripMenuItem";
            this.相机测试ToolStripMenuItem.Size = new System.Drawing.Size(81, 24);
            this.相机测试ToolStripMenuItem.Text = "相机测试";
            this.相机测试ToolStripMenuItem.Visible = false;
            // 
            // videorunToolStripMenuItem
            // 
            this.videorunToolStripMenuItem.Name = "videorunToolStripMenuItem";
            this.videorunToolStripMenuItem.Size = new System.Drawing.Size(108, 24);
            this.videorunToolStripMenuItem.Text = "开始";
            // 
            // videostopToolStripMenuItem
            // 
            this.videostopToolStripMenuItem.Name = "videostopToolStripMenuItem";
            this.videostopToolStripMenuItem.Size = new System.Drawing.Size(108, 24);
            this.videostopToolStripMenuItem.Text = "停止";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.splitContainer1);
            this.panel1.Controls.Add(this.pnlTop);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 28);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1692, 890);
            this.panel1.TabIndex = 5;
            // 
            // FormChipDetection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1692, 918);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.mnuMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MainMenuStrip = this.mnuMain;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "FormChipDetection";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "芯片检测";
            ((System.ComponentModel.ISupportInitialize)(this.picImage)).EndInit();
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            this.pnlRight.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.grpMotor.ResumeLayout(false);
            this.grpCalibration.ResumeLayout(false);
            this.grpParameter.ResumeLayout(false);
            this.grpParameter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numHeightMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numWidthMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHeightMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numWidthMax)).EndInit();
            this.pnlControl.ResumeLayout(false);
            this.pnlResult.ResumeLayout(false);
            this.pnlResult.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.mnuMain.ResumeLayout(false);
            this.mnuMain.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picImage;
        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Panel pnlRight;
        private System.Windows.Forms.Panel pnlControl;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Panel pnlResult;
        private System.Windows.Forms.Label lblCodeValue;
        private System.Windows.Forms.Label lblCodeString;
        private System.Windows.Forms.Label lbl2String;
        private System.Windows.Forms.Label lbl1String;
        private System.Windows.Forms.Label lblStatusIndicator;
        private System.Windows.Forms.Label lbl4TimeString;
        private System.Windows.Forms.Label lbl3Val;
        private System.Windows.Forms.Label lbl3String;
        private System.Windows.Forms.Label lbl2Val;
        private System.Windows.Forms.Label lbl1Val;
        private System.Windows.Forms.Label lbl4Val;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label lblMark;
        private System.Windows.Forms.Label lblDebug;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox grpParameter;
        private System.Windows.Forms.Button btnSaveCalibration;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numHeightMin;
        private System.Windows.Forms.NumericUpDown numWidthMin;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblWidthMax;
        private System.Windows.Forms.NumericUpDown numHeightMax;
        private System.Windows.Forms.NumericUpDown numWidthMax;
        private System.Windows.Forms.Label lblCalibrationStatus;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button btnStopGrab;
        private System.Windows.Forms.Button btnGrab;
        private System.Windows.Forms.GroupBox grpCalibration;
        private System.Windows.Forms.CheckBox chkCalibration;
        private System.Windows.Forms.MenuStrip mnuMain;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripMenuItem mnuUser;
        private System.Windows.Forms.ToolStripMenuItem mnuUserManager;
        private System.Windows.Forms.ToolStripMenuItem mnuUserLogin;
        private System.Windows.Forms.GroupBox grpMotor;
        private System.Windows.Forms.Button btnStopMotor;
        private System.Windows.Forms.Button btnSnap;
        private System.Windows.Forms.ToolStripMenuItem 相机测试ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem videorunToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem videostopToolStripMenuItem;
        private System.Windows.Forms.Button button1;
    }
}

