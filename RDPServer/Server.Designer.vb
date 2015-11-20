<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Server
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
        Me.cmdListen = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'cmdListen
        '
        Me.cmdListen.Location = New System.Drawing.Point(12, 12)
        Me.cmdListen.Name = "cmdListen"
        Me.cmdListen.Size = New System.Drawing.Size(210, 24)
        Me.cmdListen.TabIndex = 0
        Me.cmdListen.Text = "Listen"
        Me.cmdListen.UseVisualStyleBackColor = True
        '
        'Server
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(234, 48)
        Me.Controls.Add(Me.cmdListen)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Name = "Server"
        Me.Text = "Server"
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents cmdListen As Button
End Class
