namespace Calculus
{
    partial class plotForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(plotForm));
            this.plotBox = new Calculus.PlotBox();
            this.SuspendLayout();
            // 
            // plotBox
            // 
            this.plotBox.BackColor = System.Drawing.Color.White;
            this.plotBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plotBox.Location = new System.Drawing.Point(0, 0);
            this.plotBox.Name = "plotBox";
            this.plotBox.Size = new System.Drawing.Size(400, 400);
            this.plotBox.TabIndex = 0;
            // 
            // plotForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 400);
            this.Controls.Add(this.plotBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(416, 439);
            this.Name = "plotForm";
            this.Text = "Plot";
            this.ResumeLayout(false);

        }

        #endregion

        private PlotBox plotBox;
    }
}