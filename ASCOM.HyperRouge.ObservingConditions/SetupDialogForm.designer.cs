namespace ASCOM.HyperRouge {
   partial class SetupDialogForm {
      /// <summary>
      /// Required designer variable.
      /// </summary>
      private System.ComponentModel.IContainer components = null;

      /// <summary>
      /// Clean up any resources being used.
      /// </summary>
      /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
      protected override void Dispose(bool disposing) {
         if (disposing && (components != null)) {
            components.Dispose();
         }
         base.Dispose(disposing);
      }

      #region Windows Form Designer generated code

      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent() {
         this.cmdOK = new System.Windows.Forms.Button();
         this.cmdCancel = new System.Windows.Forms.Button();
         this.picASCOM = new System.Windows.Forms.PictureBox();
         this.label2 = new System.Windows.Forms.Label();
         this.chkTrace = new System.Windows.Forms.CheckBox();
         this.txtDataUrl = new System.Windows.Forms.TextBox();
         ((System.ComponentModel.ISupportInitialize)(this.picASCOM)).BeginInit();
         this.SuspendLayout();
         // 
         // cmdOK
         // 
         this.cmdOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK;
         this.cmdOK.Location = new System.Drawing.Point(518, 239);
         this.cmdOK.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
         this.cmdOK.Name = "cmdOK";
         this.cmdOK.Size = new System.Drawing.Size(157, 57);
         this.cmdOK.TabIndex = 0;
         this.cmdOK.Text = "OK";
         this.cmdOK.UseVisualStyleBackColor = true;
         this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
         // 
         // cmdCancel
         // 
         this.cmdCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.cmdCancel.Location = new System.Drawing.Point(710, 237);
         this.cmdCancel.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
         this.cmdCancel.Name = "cmdCancel";
         this.cmdCancel.Size = new System.Drawing.Size(157, 60);
         this.cmdCancel.TabIndex = 1;
         this.cmdCancel.Text = "Cancel";
         this.cmdCancel.UseVisualStyleBackColor = true;
         this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
         // 
         // picASCOM
         // 
         this.picASCOM.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.picASCOM.Cursor = System.Windows.Forms.Cursors.Hand;
         this.picASCOM.Image = global::ASCOM.HyperRouge.Properties.Resources.ASCOM;
         this.picASCOM.Location = new System.Drawing.Point(825, 16);
         this.picASCOM.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
         this.picASCOM.Name = "picASCOM";
         this.picASCOM.Size = new System.Drawing.Size(48, 56);
         this.picASCOM.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
         this.picASCOM.TabIndex = 3;
         this.picASCOM.TabStop = false;
         this.picASCOM.Click += new System.EventHandler(this.BrowseToAscom);
         this.picASCOM.DoubleClick += new System.EventHandler(this.BrowseToAscom);
         // 
         // label2
         // 
         this.label2.AutoSize = true;
         this.label2.Location = new System.Drawing.Point(32, 119);
         this.label2.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
         this.label2.Name = "label2";
         this.label2.Size = new System.Drawing.Size(625, 32);
         this.label2.TabIndex = 5;
         this.label2.Text = "Saisir URL Api des données de la caméra AllSky";
         // 
         // chkTrace
         // 
         this.chkTrace.AutoSize = true;
         this.chkTrace.Location = new System.Drawing.Point(38, 250);
         this.chkTrace.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
         this.chkTrace.Name = "chkTrace";
         this.chkTrace.Size = new System.Drawing.Size(163, 36);
         this.chkTrace.TabIndex = 6;
         this.chkTrace.Text = "Trace on";
         this.chkTrace.UseVisualStyleBackColor = true;
         // 
         // txtDataUrl
         // 
         this.txtDataUrl.Location = new System.Drawing.Point(38, 182);
         this.txtDataUrl.Name = "txtDataUrl";
         this.txtDataUrl.Size = new System.Drawing.Size(868, 38);
         this.txtDataUrl.TabIndex = 8;
         // 
         // SetupDialogForm
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(971, 417);
         this.Controls.Add(this.txtDataUrl);
         this.Controls.Add(this.chkTrace);
         this.Controls.Add(this.label2);
         this.Controls.Add(this.picASCOM);
         this.Controls.Add(this.cmdCancel);
         this.Controls.Add(this.cmdOK);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "SetupDialogForm";
         this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
         this.Text = "HyperRouge Setup";
         ((System.ComponentModel.ISupportInitialize)(this.picASCOM)).EndInit();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.Button cmdOK;
      private System.Windows.Forms.Button cmdCancel;
      private System.Windows.Forms.PictureBox picASCOM;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.CheckBox chkTrace;
      private System.Windows.Forms.TextBox txtDataUrl;
   }
}