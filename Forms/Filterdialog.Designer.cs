namespace IV_Play
{
  partial class FilterDialog
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FilterDialog));
      this._txtFilter = new System.Windows.Forms.TextBox();
      this._btnOK = new System.Windows.Forms.Button();
      this.SuspendLayout();
      //
      // _txtFilter
      //
      this._txtFilter.Location = new System.Drawing.Point(92, 40);
      this._txtFilter.Name = "_txtFilter";
      this._txtFilter.Size = new System.Drawing.Size(214, 20);
      this._txtFilter.TabIndex = 0;
      this._txtFilter.Text = "Enter Game / Year / Driver / Manufacturer";
      //
      // _btnOK
      //
      this._btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
      this._btnOK.Location = new System.Drawing.Point(231, 114);
      this._btnOK.Name = "_btnOK";
      this._btnOK.Size = new System.Drawing.Size(75, 23);
      this._btnOK.TabIndex = 1;
      this._btnOK.Text = "OK";
      this._btnOK.UseVisualStyleBackColor = true;
      this._btnOK.Click += new System.EventHandler(this._btnOK_Click);
      //
      // FilterDialog
      //
      this.AcceptButton = this._btnOK;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackgroundImage = global::IV_Play.Properties.Resources.Text_Filter_Background;
      this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
      this.ClientSize = new System.Drawing.Size(318, 149);
      this.Controls.Add(this._btnOK);
      this.Controls.Add(this._txtFilter);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.KeyPreview = true;
      this.Name = "FilterDialog";
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Text Filter";
      this.TopMost = true;
      this.Load += new System.EventHandler(this.FilterDialog_Load);
      this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FilterDialog_KeyDown);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox _txtFilter;
    private System.Windows.Forms.Button _btnOK;
  }
}
