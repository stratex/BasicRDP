<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Client
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.picDesktop = New System.Windows.Forms.PictureBox()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.cmdSendCommand = New System.Windows.Forms.Button()
        Me.txtCMD = New System.Windows.Forms.TextBox()
        Me.txtIP = New System.Windows.Forms.TextBox()
        Me.cmdConnect = New System.Windows.Forms.Button()
        CType(Me.picDesktop, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'picDesktop
        '
        Me.picDesktop.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.picDesktop.Location = New System.Drawing.Point(12, 68)
        Me.picDesktop.Name = "picDesktop"
        Me.picDesktop.Size = New System.Drawing.Size(1002, 493)
        Me.picDesktop.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.picDesktop.TabIndex = 0
        Me.picDesktop.TabStop = False
        '
        'GroupBox1
        '
        Me.GroupBox1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBox1.Controls.Add(Me.cmdSendCommand)
        Me.GroupBox1.Controls.Add(Me.txtCMD)
        Me.GroupBox1.Controls.Add(Me.txtIP)
        Me.GroupBox1.Controls.Add(Me.cmdConnect)
        Me.GroupBox1.Location = New System.Drawing.Point(12, 12)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(1002, 50)
        Me.GroupBox1.TabIndex = 1
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Control"
        '
        'cmdSendCommand
        '
        Me.cmdSendCommand.Enabled = False
        Me.cmdSendCommand.Location = New System.Drawing.Point(553, 22)
        Me.cmdSendCommand.Name = "cmdSendCommand"
        Me.cmdSendCommand.Size = New System.Drawing.Size(116, 23)
        Me.cmdSendCommand.TabIndex = 3
        Me.cmdSendCommand.Text = "Send Command"
        Me.cmdSendCommand.UseVisualStyleBackColor = True
        '
        'txtCMD
        '
        Me.txtCMD.Enabled = False
        Me.txtCMD.Location = New System.Drawing.Point(321, 24)
        Me.txtCMD.Name = "txtCMD"
        Me.txtCMD.Size = New System.Drawing.Size(226, 20)
        Me.txtCMD.TabIndex = 2
        '
        'txtIP
        '
        Me.txtIP.Location = New System.Drawing.Point(6, 24)
        Me.txtIP.Name = "txtIP"
        Me.txtIP.Size = New System.Drawing.Size(196, 20)
        Me.txtIP.TabIndex = 1
        Me.txtIP.Text = "127.0.0.1"
        '
        'cmdConnect
        '
        Me.cmdConnect.Location = New System.Drawing.Point(208, 22)
        Me.cmdConnect.Name = "cmdConnect"
        Me.cmdConnect.Size = New System.Drawing.Size(107, 23)
        Me.cmdConnect.TabIndex = 0
        Me.cmdConnect.Text = "Connect"
        Me.cmdConnect.UseVisualStyleBackColor = True
        '
        'Client
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1026, 573)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.picDesktop)
        Me.Name = "Client"
        Me.Text = "Client"
        CType(Me.picDesktop, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents cmdSendCommand As Button
    Friend WithEvents txtCMD As TextBox
    Friend WithEvents txtIP As TextBox
    Friend WithEvents cmdConnect As Button
    Friend WithEvents picDesktop As PictureBox
End Class
