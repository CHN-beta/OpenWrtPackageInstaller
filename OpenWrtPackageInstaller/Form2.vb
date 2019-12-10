Public Class Form2
    Private Sub Form2_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub Form2_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        My.Forms.Form1.ShowDialog()
    End Sub

    Private Sub Label1_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub Label4_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub TextBox2_TextChanged(sender As Object, e As EventArgs)

    End Sub

    Private Sub Label2_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        OpenFileDialog1.ShowDialog()
        TextBox5.Text = OpenFileDialog1.FileName
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click

        Button2.Enabled = False
        TextBox6.Text += vbCrLf + "-------------" + vbCrLf

        Dim con = New Renci.SshNet.ConnectionInfo(TextBox1.Text, CInt(TextBox2.Text), TextBox3.Text, New Renci.SshNet.PasswordAuthenticationMethod(TextBox3.Text, TextBox4.Text))
        Dim cli = New Renci.SshNet.SshClient(con)

        Try
            cli.Connect()
        Catch ex As System.Net.Sockets.SocketException
            MessageBox.Show("路由器地址或者 SSH 端口输入错误")
            TextBox6.Text += ex.Message + vbCrLf
        Catch ex As Renci.SshNet.Common.SshAuthenticationException
            MessageBox.Show("用户名或者密码错误")
            TextBox6.Text += ex.Message + vbCrLf
        Catch ex As Exception
            MessageBox.Show("与路由器建立连接的过程中出错，但我也不知道错在哪里，你自己看看日志找找罢。")
            TextBox6.Text += ex.Message + vbCrLf
        End Try

        If cli.IsConnected Then
            Dim cli_scp = New Renci.SshNet.ScpClient(con)
            cli_scp.Connect()
            cli_scp.Upload(New IO.FileInfo(TextBox5.Text), "/tmp/test.ipk")
            cli_scp.Disconnect()
            Dim cmd = cli.RunCommand("opkg install /tmp/test.ipk")
            Dim rst = cmd.Result, err = cmd.Error
            TextBox6.Text += rst + vbCrLf + err + vbCrLf
            If rst.Contains("up to date") Then
                MessageBox.Show("已经安装好了，不要重复安装了。")
            ElseIf rst.Contains("Installing") And rst.Contains("Configuring") Then
                MessageBox.Show("当当当当，安装成功。")
            ElseIf err.Contains("incompatible with the architectures configured") Then
                MessageBox.Show("路由器型号与你选择的软件包不对应。")
            ElseIf err.Contains("kernel") Then
                MessageBox.Show("说了多少遍了，装内核模块要和内核版本严格对应，就是不听！你退群吧。")
            Else
                MessageBox.Show("安装软件包的过程中出错，但我也不知道错在哪里，你自己看看日志找找罢。")
            End If
        End If

        Button2.Enabled = True
    End Sub

    Private Sub TextBox6_TextChanged(sender As Object, e As EventArgs) Handles TextBox6.TextChanged

    End Sub
End Class