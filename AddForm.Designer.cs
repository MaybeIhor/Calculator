namespace Calculus
{
    partial class addForm
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
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.xLabel = new System.Windows.Forms.ToolStripLabel();
            this.functionBox = new System.Windows.Forms.ToolStripTextBox();
            this.colorLabel = new System.Windows.Forms.ToolStripLabel();
            this.colorBox = new System.Windows.Forms.ToolStripTextBox();
            this.okButton = new System.Windows.Forms.ToolStripButton();
            this.cancelButton = new System.Windows.Forms.ToolStripButton();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.toolStrip.AutoSize = false;
            this.toolStrip.BackColor = System.Drawing.Color.Transparent;
            this.toolStrip.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.toolStrip.CanOverflow = false;
            this.toolStrip.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStrip.GripMargin = new System.Windows.Forms.Padding(0);
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.xLabel,
            this.functionBox,
            this.colorLabel,
            this.colorBox,
            this.okButton,
            this.cancelButton});
            this.toolStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Padding = new System.Windows.Forms.Padding(0);
            this.toolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.toolStrip.ShowItemToolTips = false;
            this.toolStrip.Size = new System.Drawing.Size(164, 91);
            this.toolStrip.TabIndex = 3;
            // 
            // xLabel
            // 
            this.xLabel.AutoSize = false;
            this.xLabel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.xLabel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.xLabel.Margin = new System.Windows.Forms.Padding(5, 5, 5, 0);
            this.xLabel.Name = "xLabel";
            this.xLabel.Size = new System.Drawing.Size(23, 23);
            this.xLabel.Text = "x=";
            this.xLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // functionBox
            // 
            this.functionBox.AutoSize = false;
            this.functionBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.functionBox.CharacterCasing = System.Windows.Forms.CharacterCasing.Lower;
            this.functionBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.functionBox.Margin = new System.Windows.Forms.Padding(1, 6, 5, 0);
            this.functionBox.Name = "functionBox";
            this.functionBox.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.functionBox.Size = new System.Drawing.Size(125, 21);
            // 
            // colorLabel
            // 
            this.colorLabel.AutoSize = false;
            this.colorLabel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.colorLabel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.None;
            this.colorLabel.Margin = new System.Windows.Forms.Padding(5, 5, 5, 0);
            this.colorLabel.Name = "colorLabel";
            this.colorLabel.Size = new System.Drawing.Size(23, 23);
            // 
            // colorBox
            // 
            this.colorBox.AutoSize = false;
            this.colorBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.colorBox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.colorBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.colorBox.Margin = new System.Windows.Forms.Padding(1, 6, 5, 0);
            this.colorBox.Name = "colorBox";
            this.colorBox.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.colorBox.Size = new System.Drawing.Size(125, 21);
            this.colorBox.Text = "#3A90DB";
            this.colorBox.TextChanged += new System.EventHandler(this.ColorBox_TextChanged);
            // 
            // okButton
            // 
            this.okButton.AutoSize = false;
            this.okButton.AutoToolTip = false;
            this.okButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.okButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.okButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.okButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.okButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.okButton.Margin = new System.Windows.Forms.Padding(1, 5, 0, 1);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(81, 29);
            this.okButton.Text = "OK";
            this.okButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.AutoSize = false;
            this.cancelButton.AutoToolTip = false;
            this.cancelButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.cancelButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.cancelButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.cancelButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cancelButton.Margin = new System.Windows.Forms.Padding(0, 5, 0, 1);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(81, 29);
            this.cancelButton.Text = "Cancel";
            this.cancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // addForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(164, 91);
            this.Controls.Add(this.toolStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "addForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add";
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripTextBox functionBox;
        private System.Windows.Forms.ToolStripButton okButton;
        private System.Windows.Forms.ToolStripButton cancelButton;
        private System.Windows.Forms.ToolStripTextBox colorBox;
        private System.Windows.Forms.ToolStripLabel xLabel;
        private System.Windows.Forms.ToolStripLabel colorLabel;
    }
}