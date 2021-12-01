using System.ComponentModel;

namespace ProjectGames {
    partial class ManDonTGetAngry {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            this.grbSpeler1 = new System.Windows.Forms.GroupBox();
            this.SuspendLayout();
            // 
            // grbSpeler1
            // 
            this.grbSpeler1.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.grbSpeler1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.grbSpeler1.Location = new System.Drawing.Point(676, 12);
            this.grbSpeler1.Name = "grbSpeler1";
            this.grbSpeler1.Size = new System.Drawing.Size(112, 111);
            this.grbSpeler1.TabIndex = 0;
            this.grbSpeler1.TabStop = false;
            // 
            // Don_t_get_angry
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.grbSpeler1);
            this.Name = "ManDonTGetAngry";
            this.Text = "Don_t_get_angry";
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.GroupBox grbSpeler1;

        private System.Windows.Forms.GroupBox grb;

        private System.Windows.Forms.GroupBox gpb;

        #endregion
    }
}