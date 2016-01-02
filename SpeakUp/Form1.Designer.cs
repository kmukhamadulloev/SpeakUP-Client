namespace SpeakUp
{
    partial class Form1
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.errPanel = new System.Windows.Forms.Panel();
            this.errBtnReload = new System.Windows.Forms.Button();
            this.errLabelStatus = new System.Windows.Forms.Label();
            this.errLabelTitle = new System.Windows.Forms.Label();
            this.errPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // errPanel
            // 
            this.errPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.errPanel.Controls.Add(this.errBtnReload);
            this.errPanel.Controls.Add(this.errLabelStatus);
            this.errPanel.Controls.Add(this.errLabelTitle);
            this.errPanel.Location = new System.Drawing.Point(121, 100);
            this.errPanel.Name = "errPanel";
            this.errPanel.Size = new System.Drawing.Size(518, 95);
            this.errPanel.TabIndex = 0;
            this.errPanel.Visible = false;
            // 
            // errBtnReload
            // 
            this.errBtnReload.Dock = System.Windows.Forms.DockStyle.Top;
            this.errBtnReload.Location = new System.Drawing.Point(0, 66);
            this.errBtnReload.Margin = new System.Windows.Forms.Padding(10);
            this.errBtnReload.Name = "errBtnReload";
            this.errBtnReload.Size = new System.Drawing.Size(516, 23);
            this.errBtnReload.TabIndex = 2;
            this.errBtnReload.Text = "Try Again";
            this.errBtnReload.UseVisualStyleBackColor = true;
            this.errBtnReload.Click += new System.EventHandler(this.errBtnReload_Click);
            // 
            // errLabelStatus
            // 
            this.errLabelStatus.Dock = System.Windows.Forms.DockStyle.Top;
            this.errLabelStatus.Location = new System.Drawing.Point(0, 27);
            this.errLabelStatus.Margin = new System.Windows.Forms.Padding(5, 15, 5, 5);
            this.errLabelStatus.Name = "errLabelStatus";
            this.errLabelStatus.Size = new System.Drawing.Size(516, 39);
            this.errLabelStatus.TabIndex = 1;
            this.errLabelStatus.Text = "Connection error: N/A";
            this.errLabelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // errLabelTitle
            // 
            this.errLabelTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.errLabelTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.errLabelTitle.Location = new System.Drawing.Point(0, 0);
            this.errLabelTitle.Margin = new System.Windows.Forms.Padding(5);
            this.errLabelTitle.Name = "errLabelTitle";
            this.errLabelTitle.Size = new System.Drawing.Size(516, 27);
            this.errLabelTitle.TabIndex = 0;
            this.errLabelTitle.Text = "SpeakUP";
            this.errLabelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 442);
            this.Controls.Add(this.errPanel);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(800, 480);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SpeakUP";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResizeEnd += new System.EventHandler(this.Form1_ResizeEnd);
            this.LocationChanged += new System.EventHandler(this.Form1_LocationChanged);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.errPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel errPanel;
        private System.Windows.Forms.Button errBtnReload;
        private System.Windows.Forms.Label errLabelStatus;
        private System.Windows.Forms.Label errLabelTitle;

    }
}

