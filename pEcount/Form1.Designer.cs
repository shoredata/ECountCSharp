namespace pEcount
{
  partial class Form1
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
      this.components = new System.ComponentModel.Container();
      this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
      this.btnGetVersion = new System.Windows.Forms.Button();
      this.listBox1 = new System.Windows.Forms.ListBox();
      this.btnGetStatus = new System.Windows.Forms.Button();
      this.btnPassThroughPrint = new System.Windows.Forms.Button();
      this.btnListCOMPorts = new System.Windows.Forms.Button();
      this.btnSetPreset = new System.Windows.Forms.Button();
      this.txtPreset = new System.Windows.Forms.TextBox();
      this.btnReset = new System.Windows.Forms.Button();
      this.btnMonitor = new System.Windows.Forms.Button();
      this.btnToggleValves = new System.Windows.Forms.Button();
      this.btnEndAndPrint = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // serialPort1
      // 
      this.serialPort1.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort1_DataReceived);
      // 
      // btnGetVersion
      // 
      this.btnGetVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.btnGetVersion.Location = new System.Drawing.Point(13, 402);
      this.btnGetVersion.Name = "btnGetVersion";
      this.btnGetVersion.Size = new System.Drawing.Size(75, 23);
      this.btnGetVersion.TabIndex = 0;
      this.btnGetVersion.Text = "Get Version";
      this.btnGetVersion.UseVisualStyleBackColor = true;
      this.btnGetVersion.Click += new System.EventHandler(this.btnGetVersion_Click);
      // 
      // listBox1
      // 
      this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.listBox1.Font = new System.Drawing.Font("Consolas", 8F);
      this.listBox1.FormattingEnabled = true;
      this.listBox1.Location = new System.Drawing.Point(13, 9);
      this.listBox1.Name = "listBox1";
      this.listBox1.Size = new System.Drawing.Size(576, 381);
      this.listBox1.TabIndex = 1;
      // 
      // btnGetStatus
      // 
      this.btnGetStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.btnGetStatus.Location = new System.Drawing.Point(94, 402);
      this.btnGetStatus.Name = "btnGetStatus";
      this.btnGetStatus.Size = new System.Drawing.Size(75, 23);
      this.btnGetStatus.TabIndex = 2;
      this.btnGetStatus.Text = "Get Status";
      this.btnGetStatus.UseVisualStyleBackColor = true;
      this.btnGetStatus.Click += new System.EventHandler(this.btnGetStatus_Click);
      // 
      // btnPassThroughPrint
      // 
      this.btnPassThroughPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.btnPassThroughPrint.Location = new System.Drawing.Point(175, 402);
      this.btnPassThroughPrint.Name = "btnPassThroughPrint";
      this.btnPassThroughPrint.Size = new System.Drawing.Size(117, 23);
      this.btnPassThroughPrint.TabIndex = 3;
      this.btnPassThroughPrint.Text = "Pass-Through Print";
      this.btnPassThroughPrint.UseVisualStyleBackColor = true;
      this.btnPassThroughPrint.Click += new System.EventHandler(this.btnPassThroughPrint_Click);
      // 
      // btnListCOMPorts
      // 
      this.btnListCOMPorts.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnListCOMPorts.Location = new System.Drawing.Point(493, 402);
      this.btnListCOMPorts.Name = "btnListCOMPorts";
      this.btnListCOMPorts.Size = new System.Drawing.Size(96, 23);
      this.btnListCOMPorts.TabIndex = 4;
      this.btnListCOMPorts.Text = "List COM Ports";
      this.btnListCOMPorts.UseVisualStyleBackColor = true;
      this.btnListCOMPorts.Click += new System.EventHandler(this.btnListCOMPorts_Click);
      // 
      // btnSetPreset
      // 
      this.btnSetPreset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.btnSetPreset.Location = new System.Drawing.Point(13, 431);
      this.btnSetPreset.Name = "btnSetPreset";
      this.btnSetPreset.Size = new System.Drawing.Size(75, 23);
      this.btnSetPreset.TabIndex = 5;
      this.btnSetPreset.Text = "Set Preset";
      this.btnSetPreset.UseVisualStyleBackColor = true;
      this.btnSetPreset.Click += new System.EventHandler(this.btnSetPreset_Click);
      // 
      // txtPreset
      // 
      this.txtPreset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.txtPreset.Location = new System.Drawing.Point(94, 433);
      this.txtPreset.Name = "txtPreset";
      this.txtPreset.Size = new System.Drawing.Size(75, 20);
      this.txtPreset.TabIndex = 6;
      // 
      // btnReset
      // 
      this.btnReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.btnReset.Location = new System.Drawing.Point(175, 431);
      this.btnReset.Name = "btnReset";
      this.btnReset.Size = new System.Drawing.Size(69, 23);
      this.btnReset.TabIndex = 7;
      this.btnReset.Text = "Reset";
      this.btnReset.UseVisualStyleBackColor = true;
      this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
      // 
      // btnMonitor
      // 
      this.btnMonitor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.btnMonitor.Location = new System.Drawing.Point(250, 431);
      this.btnMonitor.Name = "btnMonitor";
      this.btnMonitor.Size = new System.Drawing.Size(92, 23);
      this.btnMonitor.TabIndex = 8;
      this.btnMonitor.Text = "Monitor Status";
      this.btnMonitor.UseVisualStyleBackColor = true;
      this.btnMonitor.Click += new System.EventHandler(this.btnMonitor_Click);
      // 
      // btnToggleValves
      // 
      this.btnToggleValves.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnToggleValves.Location = new System.Drawing.Point(348, 431);
      this.btnToggleValves.Name = "btnToggleValves";
      this.btnToggleValves.Size = new System.Drawing.Size(92, 23);
      this.btnToggleValves.TabIndex = 9;
      this.btnToggleValves.Text = "Toggle Valves";
      this.btnToggleValves.UseVisualStyleBackColor = true;
      this.btnToggleValves.Click += new System.EventHandler(this.btnToggleValves_Click);
      // 
      // btnEndAndPrint
      // 
      this.btnEndAndPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnEndAndPrint.Location = new System.Drawing.Point(446, 431);
      this.btnEndAndPrint.Name = "btnEndAndPrint";
      this.btnEndAndPrint.Size = new System.Drawing.Size(143, 23);
      this.btnEndAndPrint.TabIndex = 10;
      this.btnEndAndPrint.Text = "End && Print Delivery";
      this.btnEndAndPrint.UseVisualStyleBackColor = true;
      this.btnEndAndPrint.Click += new System.EventHandler(this.btnEndAndPrint_Click);
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(601, 472);
      this.Controls.Add(this.btnEndAndPrint);
      this.Controls.Add(this.btnToggleValves);
      this.Controls.Add(this.btnMonitor);
      this.Controls.Add(this.btnReset);
      this.Controls.Add(this.txtPreset);
      this.Controls.Add(this.btnSetPreset);
      this.Controls.Add(this.btnListCOMPorts);
      this.Controls.Add(this.btnPassThroughPrint);
      this.Controls.Add(this.btnGetStatus);
      this.Controls.Add(this.listBox1);
      this.Controls.Add(this.btnGetVersion);
      this.MinimumSize = new System.Drawing.Size(617, 264);
      this.Name = "Form1";
      this.Text = "MID:COM E:Count Serial Interface Sample Code";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.IO.Ports.SerialPort serialPort1;
    private System.Windows.Forms.Button btnGetVersion;
    private System.Windows.Forms.ListBox listBox1;
    private System.Windows.Forms.Button btnGetStatus;
    private System.Windows.Forms.Button btnPassThroughPrint;
    private System.Windows.Forms.Button btnListCOMPorts;
    private System.Windows.Forms.Button btnSetPreset;
    private System.Windows.Forms.TextBox txtPreset;
    private System.Windows.Forms.Button btnReset;
    private System.Windows.Forms.Button btnMonitor;
    private System.Windows.Forms.Button btnToggleValves;
    private System.Windows.Forms.Button btnEndAndPrint;
  }
}

