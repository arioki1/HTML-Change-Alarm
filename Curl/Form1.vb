









Imports System.Net
Imports System.Text.RegularExpressions
Imports System.Runtime.CompilerServices
Imports WMPLib 'Media Player in virtual form
Imports System.IO
Imports System.Text
Imports System.Media


Module Extensions
    <Extension()>
    Public Function RemoveHtmlTags(value As String) As String
        Return Regex.Replace(value, "<(?:[^>=]|='[^']*'|=""[^""]*""|=[^'""][^\s>]*)*>", "")
    End Function
End Module

Public Class Form1
    Dim Player As WindowsMediaPlayer = New WindowsMediaPlayer
    Dim no As Integer = 0
    Dim btn As Integer = 0
    Dim m_Interval As Integer
    Dim change As Integer = 0

    Function getUrl(url) As String
        Dim respon As String = ""
        Dim client As WebClient = New WebClient()
        respon = client.DownloadString(url).ToString
        If (no = 0) Then
            no = 1
        Else
            no = no + 1
        End If

        RichTextBox4.Text = respon
        Label10.Text = no
        regexUrl(respon)
        Return respon
    End Function
    Sub startBtn()
        If (btn = 0) Then
            Button1.Text = "STOP"
            btn = 1
            If (ComboBox1.SelectedItem = "POST") Then
                postData(TextBox1.Text.ToString, TextBox2.Text)
                'getUrl(TextBox1.Text.ToString)

            Else
                getUrl(TextBox1.Text.ToString)
            End If

            Timer1.Enabled = True
                Timer1.Interval = 1000
                m_Interval = TextBox3.Text
            Else
                Button1.Text = "START"
            btn = 0
            Timer1.Enabled = False
            Label8.Text = "Stoped"
        End If
    End Sub

    Function regexUrl(html) As String
        Dim respon As String = ""
        If (RichTextBox1.Text.Length > 0) Then
            Dim expr As String = RichTextBox1.Text
            Dim mc As MatchCollection = Regex.Matches(html, expr)
            Dim m As Match
            Dim a As String = "", c As String = ""
            For Each m In mc
                c = m.ToString
                a = a & " " & c & vbNewLine
            Next m
            RichTextBox2.Text = a.RemoveHtmlTags()
            compareText(a.RemoveHtmlTags())
        Else
            RichTextBox2.Text = "Stoped"
            respon = "Stoped"
        End If
        Return respon
    End Function
    Sub compareText(text)
        If (RichTextBox2.Text.Length <= 0) Then
            RichTextBox2.Text = text
        ElseIf (RichTextBox3.Text.Length <= 0) Then
            RichTextBox3.Text = text
        ElseIf (Not (RichTextBox2.Text = RichTextBox3.Text)) Then
            RichTextBox3.Text = text
            change = change + 1
            Label14.Text = change.ToString
            If (CheckBox1.Checked) Then
                Alert()
            End If
        End If
    End Sub
    Sub Alert()
        PlayBackgroundSoundFile()
        Show_Balloon()
        Dim msg As Integer = MessageBox.Show("Changes detected", "Alert!", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk)
        If (msg = DialogResult.OK Or msg = DialogResult.Ignore Or
            msg = DialogResult.Cancel Or msg = DialogResult.Abort) Then
            Player.controls.stop()
        End If
    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        'PlayBackgroundSoundFile()
        startBtn()
        ' postData(TextBox1.Text.ToString, TextBox2.Text)
    End Sub
    Sub PlayBackgroundSoundFile()
        Dim tempDir As String = Path.GetTempPath()

        Player.URL = "alert.mp3"
        Player.controls.play()



    End Sub
    Private Sub Show_Balloon()
        NotifyIcon1.BalloonTipTitle = "Alert!!"
        NotifyIcon1.BalloonTipText = "Changes detected"
        NotifyIcon1.BalloonTipIcon = ToolTipIcon.Info

        NotifyIcon1.Visible = True
        NotifyIcon1.ShowBalloonTip(3000)
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        m_Interval -= 1
        Label8.Text = "Waiting " & System.TimeSpan.FromSeconds(m_Interval).ToString.Substring(6) & " sec"
        If m_Interval = 0 Then
            Me.Cursor = Cursors.WaitCursor
            getUrl(TextBox1.Text.ToString)
            Timer1.Enabled = False
            m_Interval = TextBox3.Text
            Timer1.Enabled = True
            Me.Cursor = Cursors.Default
        Else
            Label8.Text = "Waiting " & System.TimeSpan.FromSeconds(m_Interval).ToString.Substring(6) & " sec"

        End If
    End Sub
    Private Sub Form1_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
        Me.Hide()
        NotifyIcon1.Visible = True
        NotifyIcon1.Icon = SystemIcons.Application
        NotifyIcon1.BalloonTipIcon = ToolTipIcon.Info
        NotifyIcon1.BalloonTipText = "Aplikasi berjalan di Tray."
        NotifyIcon1.BalloonTipTitle = "System Tray"
        NotifyIcon1.ShowBalloonTip(5000)
    End Sub

    Private Sub NotifyIcon1_MouseDown(sender As Object, e As MouseEventArgs) Handles NotifyIcon1.MouseDown
        If e.Button = MouseButtons.Right Then
            ContextMenuStrip1.Show(Cursor.Position)
        Else
            Me.Show()
            NotifyIcon1.Visible = False
        End If

    End Sub

    Private Sub ContextMenuStrip1_Opening(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles ContextMenuStrip1.Opening
        Me.Show()
    End Sub


    Private Sub postData(ByVal url As String, ByVal formData As String)

        Dim postData As String = formData
        Dim tempCookies As New CookieContainer
        Dim encoding As New UTF8Encoding
        Dim byteData As Byte() = encoding.GetBytes(postData)
        Dim logincookie As CookieContainer

        Dim postReq As HttpWebRequest = DirectCast(WebRequest.Create(url), HttpWebRequest)
        postReq.Method = "POST"
        postReq.KeepAlive = True
        postReq.CookieContainer = tempCookies
        postReq.ContentType = "application/x-www-form-urlencoded"
        postReq.Referer = url
        postReq.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; ru; rv:1.9.2.3) Gecko/20100401 Firefox/4.0 (.NET CLR 3.5.30729)"
        postReq.ContentLength = byteData.Length

        Dim postreqstream As Stream = postReq.GetRequestStream()
        postreqstream.Write(byteData, 0, byteData.Length)
        postreqstream.Close()
        Dim postresponse As HttpWebResponse

        postresponse = DirectCast(postReq.GetResponse(), HttpWebResponse)
        tempCookies.Add(postresponse.Cookies)
        logincookie = tempCookies
        Dim postreqreader As New StreamReader(postresponse.GetResponseStream())

        Dim thepage As String = postreqreader.ReadToEnd


        Dim respon As String = thepage
        RichTextBox4.Text = respon
        Label10.Text = no
        regexUrl(respon)


    End Sub

    Private Sub Label4_Click(sender As Object, e As EventArgs) Handles Label4.Click

    End Sub
End Class