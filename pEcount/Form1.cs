using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace pEcount
{
  public partial class Form1 : Form
  {

    int intlastregisternumericversionnumber = 0;
    int intlastregisterstate = 0;

    byte[] bacomportbuffer = new byte[] { };
    //change this to use a COM port other than COM1
    int intcurrentcomport = 5;

    public Form1()
    {
      InitializeComponent();
      listBox1.Items.Clear();
    }

    public delegate void DataCOMAcquired(object sender);
    private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
    {
      try
      {
        int bytes = serialPort1.BytesToRead;
        byte[] buffer = new byte[bytes];
        serialPort1.Read(buffer, 0, bytes);
        if (bacomportbuffer.Length > 0)
        {
          byte[] tempbuffer1 = new byte[bacomportbuffer.Length];
          bacomportbuffer.CopyTo(tempbuffer1, 0);
          bacomportbuffer = new byte[bytes + bacomportbuffer.Length];
          bacomportbuffer = ByteArrayConcatenate(tempbuffer1, buffer);
        }
        else
        {
          bacomportbuffer = new byte[bytes];
          buffer.CopyTo(bacomportbuffer, 0);
        }
        if (bacomportbuffer.Length > serialPort1.ReadBufferSize)
        {
          bacomportbuffer = new byte[] { };
        }
      }
      //catch (Exception e1)
      catch
      {
        //not thread safe .. 
        //LineOut("serialPort1_DataReceived()::Error Reading Port.InputBuffer", e1);
      }
    }

    public void AddToList(string stringout)
    {
      if (listBox1.Items.Count>1000)
      {
        listBox1.Items.Clear();
      }
      listBox1.Items.Add(stringout);
      listBox1.SelectedIndex = listBox1.Items.Count-1;
      listBox1.Refresh();
    }
    public void LineOut(string stringout)
    {
      System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + " " + stringout);
      AddToList(DateTime.Now.ToString() + " " + stringout);
    }
    public void LineOut(string stringout, Exception e)
    {
      string stringlocal = "";
      if (stringout != null)
      {
        LineOut(stringout);
      }

      if (e.Message != null)
      {
        stringlocal = "ERROR ============================================== ";
        LineOut(stringlocal);
        stringlocal = "Message:  " + e.Message.ToString();
        LineOut(stringlocal);
      }
      if (e.InnerException != null)
      {
        stringlocal = "InnerException:  " + e.InnerException.ToString();
        LineOut(stringlocal);
      }
      if (e.TargetSite != null)
      {
        stringlocal = "TargetSite:  " + e.TargetSite.ToString();
        LineOut(stringlocal);
      }
      if (e.Source != null)
      {
        stringlocal = "Source:  " + e.Source.ToString();
        LineOut(stringlocal);
      }
    }
    public void LineOutHexAndASCII(string stringMessage)
    {
      string sOutA = "  ASC: ";
      string sOutH = "  HEX: ";
      string sOutT = "     : ";
      //string strbinary = stringMessage.Replace("\0", "<NULL>").Replace("\r", "<CR>").Replace("\n", "<LF>");
      bool boolbinaryfound = false;

      foreach (char c in stringMessage)
      {
        int val = (int)c;
        char chr = '*';
        if(val>=32 && val<=127) {
          chr = c;
        }
        int tmp = c;
        sOutA += tmp.ToString("  000") + " ";
        sOutH += "0x" + String.Format("{0:x3}", System.Convert.ToUInt32(tmp.ToString())).ToUpper() + " ";
        sOutT += chr;
      }
      LineOut(sOutA);
      LineOut(sOutH);
      LineOut(sOutT);
      //LineOut("  " + strbinary);
    }

    private bool OpenSerialPort(System.IO.Ports.SerialPort sspport)
    {
      try
      {
        if (sspport.IsOpen)
        {
          sspport.Close();
        }

        sspport.PortName = "COM" + intcurrentcomport.ToString("0");

        sspport.Handshake = System.IO.Ports.Handshake.None;
        sspport.BaudRate = 9600;
        sspport.Parity = System.IO.Ports.Parity.None;
        sspport.DataBits = 8;
        sspport.StopBits = System.IO.Ports.StopBits.One;
        sspport.ReadBufferSize = 8192;
        sspport.WriteBufferSize = 8192;
        sspport.DtrEnable = false; 
        sspport.RtsEnable = false;
        sspport.ReceivedBytesThreshold = 1; 

        if (!sspport.IsOpen)
        {
          sspport.Open();
          sspport.DiscardInBuffer();
        }
      }
      catch (Exception e1)
      {
        string stringexception = "OpenSerialPort(): Error Opening COM Port " + sspport.PortName.ToString();
        LineOut(stringexception, e1);
      }
      return sspport.IsOpen;
    }
    private void CloseSerialPort(System.IO.Ports.SerialPort sspport)
    {
      try
      {
        if (sspport.IsOpen)
        {
          sspport.DiscardInBuffer();
          sspport.DiscardInBuffer();
          if (sspport.BytesToRead > 0)
          {
            string sdump = sspport.ReadExisting(); 
          }
          sspport.Close();
        }
      }
      catch (Exception e1)
      {
        string stringexception = "CloseSerialPort(): Error Closing COM Port " + sspport.PortName.ToString();
        LineOut(stringexception, e1);
      }
    }
    private bool IsCOMPortValid(int intcomport)
    {
      bool boolfound = false;
      foreach (string stringtemplabel in System.IO.Ports.SerialPort.GetPortNames())
      {
        string stringtest = "COM" + intcomport.ToString();
        if (stringtest.CompareTo(stringtemplabel) == 0)
        {
          boolfound = true;
          break;
        }
      }
      return boolfound;
    }

    public string Chr(int intchar)
    {
      int intlocal = intchar;
      if ((intchar < 0) || (intchar > 255))
      {
        LineOut("Chr() Invalid (int): intchar = " + intchar.ToString());
        LineOut("Chr()  Must be between 0 and 255, returning 0.");
        intlocal = 0;
        throw new ArgumentOutOfRangeException("intchar", "Must be between 0 and 255.");
      }
      //#if (WINCE)
      //      //was used until 5/13/2013 in CE5 and Win3264, had problems with X8Pro using it and getting garbage chars
      //      byte[] bytBuffer = new byte[] { (byte)intlocal };
      //      return Encoding.GetEncoding(1252).GetString(bytBuffer, 0, 1);
      //#elif (WIN3264)
      //as it turns out, this is great on pc but bad in CE ..
      byte src = Convert.ToByte(intchar);
      char chrretrn = (System.Text.Encoding.GetEncoding("iso-8859-1").GetChars(new byte[] { src })[0]);
      return Convert.ToString(chrretrn);
      //#endif
    }
    public int Asc(string stringin)
    {
      return (int)stringin[0];
    }
    public int Asc(char charin)
    {
      return (int)charin;
    }

    public string ByteArrayToString(byte[] byteArray)
    {
      string stringreturn = "";
      for (int intidx = 0; intidx < byteArray.Length; intidx++)
      {
        stringreturn += Chr(int.Parse(byteArray[intidx].ToString()));
      }
      return stringreturn;
    }
    public byte[] StringToByteArray(string str)
    {
      byte[] bytearrayreturn = new byte[str.Length];
      for (int intidx = 0; intidx < str.Length; intidx++)
      {
        int intchar = Asc(str.Substring(intidx, 1));
        bytearrayreturn[intidx] = byte.Parse(intchar.ToString());
      }
      return bytearrayreturn;
    }
    public byte[] ByteArrayConcatenate(byte[] a, byte[] b)
    {
      byte[] c = new byte[a.Length + b.Length]; 
      Buffer.BlockCopy(a, 0, c, 0, a.Length);
      Buffer.BlockCopy(b, 0, c, a.Length, b.Length);
      return c;
    }
    public string ByteArrayToHexStringWithPad(byte[] data)
    {
      StringBuilder sb = new StringBuilder(data.Length * 3);
      foreach (byte b in data)
        sb.Append(Convert.ToString(b, 16).PadLeft(2, '0').PadRight(3, ' '));
      return sb.ToString().ToUpper();
    }
    public string ByteArrayToHexStringNoPad(byte[] data)
    {
      StringBuilder sb = new StringBuilder(data.Length * 2);
      foreach (byte b in data)
        sb.Append(Convert.ToString(b, 16).PadLeft(2, '0'));
      return sb.ToString().ToUpper();
    }
    public byte[] ByteArraySubarray(byte[] a, int intstart, int intlength)
    {
      const string stringfunction = "ByteArraySubarray";
      byte[] c = new byte[intlength]; // just one array
      try
      {
        Buffer.BlockCopy(a, intstart, c, 0, intlength);
      }
      catch (Exception e1)
      {
        LineOut(stringfunction + "()::Error", e1);
      }
      return c;
    }

    public static bool IsNumeric(string stringin)
    {
      if (stringin.Length <= 0)
        return false;
      try
      {
#pragma warning disable 168
        double retNum = Double.Parse(Convert.ToString(stringin), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo);
#pragma warning restore 168
        return true;
      }
      catch
      {
        return false;
      }
    }
    public static double ReturnValidDouble(string stringin)
    {
      if (stringin.Length <= 0)
        return 0.0;
      try
      {
        double retNum = Double.Parse(Convert.ToString(stringin), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo);
        return retNum;
      }
      catch
      {
        return 0.0;
      }
    }
    public static int ReturnValidInt(string stringin)
    {
      if (stringin.Length <= 0)
        return 0;
      try
      {
        int retNum = int.Parse(Convert.ToString(stringin), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo);
        return retNum;
      }
      catch
      {
        return 0;
      }
    }

    private void OutputStatusBits(int intbyte, string stringstatusbinary)
    {
      for (int i = 7; i >= 0; i--)
      {
        if (stringstatusbinary[i].CompareTo('1') == 0)
        {
          LineOut(GetDescriptionForStatusBit(intbyte, 7 - i).ToUpper());
        }
      }
    }

    private string GetDescriptionForStatusBit(int intbyte, int intbit)
    {
      if (intbyte == 1)
      {
        switch (intbit)
        {
          case 0:
            return "Bit " + intbit + ": Timeout Elapsed";
          case 1:
            return "Bit " + intbit + ": Print Key Pressed";
          case 2:
            return "Bit " + intbit + ": Preset Pending";
          case 3:
            return "Bit " + intbit + ": Valves Open";
          case 4:
            return "Bit " + intbit + ": Product Flowing";
          case 5:
            return "Bit " + intbit + ": Delivery Active";
          case 6:
            return "Bit " + intbit + ": Ticket Pending";
          case 7:
            return "Bit " + intbit + ": Host Mode Active";
        }
      }
      else if (intbyte == 2)
      {
        switch (intbit)
        {
          case 0:
            return "Bit " + intbit + ": Power Failure";
          case 1:
            return "Bit " + intbit + ": Host Mode Cancelled During Delivery";
        }
      }
      return "";
    }
    public byte GetByteChecksumV1(byte[] sSend)
    {
      byte bSum = 0;
      foreach (byte b in sSend)
      {
        bSum ^= b;
      }
      return bSum;
    }
    public string CurrentVolumeFromHexByteArrayToDecimalString(byte[] sInBytes)
    {
      const string stringfunction = "CurrentVolumeFromHexByteArrayToDecimalString";
      string sReturn = "";
      string sVal1 = "";

      try
      {
        sVal1 = ByteArrayToHexStringNoPad(sInBytes);
        sReturn = sVal1.Substring(0, 6) + "." + sVal1.Substring(6, 2);
      }
      catch (Exception e1)
      {
        LineOut(stringfunction + "()::Error processing hex byte array", e1);
      }
      return sReturn;
    }
    public int GetNumericSoftwareVersion(string stringversion)
    {
      const string stringfunction = "GetNumericSoftwareVersion";
      string stringout = "";
      int intreturn = 0;
      try
      {
        if (stringversion != null)
        {
          foreach (char c in stringversion)
          {
            if (((int)c >= 48) && ((int)c <= 57))
            {
              stringout += c.ToString();
            }
          }
          if (stringout.Length == 0)
          {
            stringout = "0";
          }
        }
        else
        {
          LineOut(stringfunction + "()::stringversion is null, must fix caller..");
        }
        intreturn = ReturnValidInt(stringout);
      }
      catch (Exception e1)
      {
        LineOut(stringfunction + "()::Error", e1);
      }
      return intreturn;
    }

    public void DoVersionCommand()
    {
      //string stringfunction = "DoVersionCommand";

      bool boolcomportopen = false;

      byte[] bytestosend;
      byte[] bytespcmtosend;

      int inthundredths = 0;
      int intthousandths = 0;

      int intpcmcommanddelay = 5;               //milliseconds
      int intmaxcommandtimeoutTenths = 0;   //HUNDREDTHS of seconds
      int inttargetresponsecharcount = 0;
      int intmaxretries = 0;
      int intretrycount = 0;
      int intretrytimeoutmilliseconds = 0;      //milliseconds

      string stringout = "";

      try
      {
        //1. com port
        //2. pcm command
        //3. send command
        //4. get response
        //5. parse response
        //6. close pcm

        LineOut("*** GET E:COUNT VERSION ***");

        LineOut("USING SERIAL PORT COM" + intcurrentcomport);
        if (!IsCOMPortValid(intcurrentcomport))
        {
          stringout = "INVALID COM PORT! CHECK DEVICE MANAGER";
          LineOut(stringout);
          System.Windows.Forms.MessageBox.Show(stringout);
          goto End_VersionCommand;
        }


        //setup com port and open it
        if (boolcomportopen)
        {
          //if the port is open, close it (possibly due to an error)
          LineOut("COM PORT ALREADY OPEN, CLOSING");
          CloseSerialPort(serialPort1);
          boolcomportopen = false;
        }
        LineOut("INITIALIZING COM PORT");
        boolcomportopen = OpenSerialPort(serialPort1);
        if (!boolcomportopen)
        {
          LineOut("ERROR INITIALIZING COM PORT!");
          goto End_VersionCommand;
        }

        //build pcm and command arrays to connect HOST to REGISTER1 (0x1F 0x02) and then send V (0x56)
        LineOut("BUILDING COMMAND BYTEARRAYS");
        bytespcmtosend = StringToByteArray(Chr(31) + Chr(2));  //chr(31) = HEX 0x1F, chr(2) = HEX 0x02 
        bytestosend = StringToByteArray(Chr(86));              //chr(86) = ASCII V

        //V command response looks like this (from ECount Host Interface docs):
        // response: V + data + |
        // data = 15 bytes:
        //
        //    Register Firmware Version XXXXXX (may be spaces)     6 bytes
        //    Firmware Data Block Version (00-99)                  2 bytes
        //    Register Port Number (1/2)                           1 byte
        //    Register Serial Number (000000-999999)               6 bytes
        //    ==============================================================
        //                                             Total      15 bytes

        //set parameters for V comamnd
        LineOut("INITIALIZING COMMAND CONTROL PARAMETERS");
        intmaxcommandtimeoutTenths = 10; //10 hundredths = 1000ms max to wait, this will vary from command to command
        inttargetresponsecharcount = 17;     //V + 15 data chars + |, this will vary from command to command
        intmaxretries = 0;                   //V cmd should not be re-sent if no response (only J should be resent if no response)
        intretrycount = 0;
        intretrytimeoutmilliseconds = 0;     //delay between retries in ms, J command is only cmd retry that is valid, J requires *min* of 250 ms between retries


Retry_VersionCommand:

        //clear RX buffer before sending
        LineOut("CLEARING COM PORT BUFFER");
        bacomportbuffer = new byte[] { };

        //send pcm command bytes
        LineOut("SENDING PCM CONNECT HOST-TO-REG1 COMMAND: ");
        LineOutHexAndASCII(ByteArrayToString(bytespcmtosend));
        serialPort1.Write(bytespcmtosend, 0, bytespcmtosend.GetUpperBound(0) + 1);

        //wait X ms for PCM hardware to complete port switching
        LineOut("WAITING FOR PCM TO SWITCH PORTS ..");
        intthousandths = 0;
        while (intthousandths < intpcmcommanddelay)
        {
          //this is a kludge
          intthousandths++;
          System.Windows.Forms.Application.DoEvents();
          System.Threading.Thread.Sleep(1);  //1 = 1ms
        }

        //send V command bytes
        LineOut("SENDING E:COUNT COMMAND: ");
        LineOutHexAndASCII(ByteArrayToString(bytestosend));
        serialPort1.Write(bytestosend, 0, bytestosend.GetUpperBound(0) + 1);

        //wait for response
        inthundredths = 0;
        while ((inthundredths < intmaxcommandtimeoutTenths) && (bacomportbuffer.Length < inttargetresponsecharcount))
        {
          inthundredths++;
          System.Windows.Forms.Application.DoEvents();
          System.Threading.Thread.Sleep(10);  //10 = 10ms, 10ms is one-hundredth of a second ..
          System.Windows.Forms.Application.DoEvents();
        }

        if (bacomportbuffer.Length < inttargetresponsecharcount)
        {
          LineOut("RESPONSE TIMEOUT EXCEEDED " + intmaxcommandtimeoutTenths + " TENTHS OF A SECOND");
          if (intretrycount < intmaxretries)
          {
            intretrycount++;
            System.Threading.Thread.Sleep(intretrytimeoutmilliseconds);
            LineOut("RETRY # " + intretrycount + " OF " + intmaxretries + ", TOO FEW CHARS: " + bacomportbuffer.Length.ToString());
            LineOut("PLEASE WAIT ..");
            goto Retry_VersionCommand;
          }
          else
          {
            if (intmaxretries == 0)
            {
              LineOut("INVALID RESPONSE, TOO FEW CHARS: " + bacomportbuffer.Length.ToString());
            }
            else
            {
              LineOut("MAX OF " + intmaxretries + " RETRIES REACHED, TOO FEW CHARS: " + bacomportbuffer.Length.ToString());
            }
          }
        }
        else
        {
          string stringresponse = ByteArrayToString(bacomportbuffer);
          LineOut("RX FROM E:COUNT: " + stringresponse.Length + " BYTES");
          LineOutHexAndASCII(stringresponse);

          //NOTE, C# ARRAY INDICES ARE 0-BASED
          LineOut("COMMAND ECHO               (0,1)  ....: " + stringresponse.Substring(0, 1)); 
          LineOut("FIRMWARE VERSION           (1,6)  ....: " + stringresponse.Substring(1,6));
          LineOut("DATABLOCK VERSION          (7,2)  ....: " + stringresponse.Substring(7,2));
          LineOut("REGISTER NUMBER            (9,1)  ....: " + stringresponse.Substring(9,1));
          LineOut("REGISTER SERIAL#           (10,6) ....: " + stringresponse.Substring(10,6));
          LineOut("TERMINATING PIPE CHARACTER (16,1) ....: " + stringresponse.Substring(16,1));

          intlastregisternumericversionnumber = GetNumericSoftwareVersion(stringresponse.Substring(1, 6));

        }


        //build pcm command arrays to disconnect HOST
        bytespcmtosend = StringToByteArray(Chr(255));

        //send pcm command bytes
        LineOut("SENDING PCM DISCONNECT COMMAND: ");
        LineOutHexAndASCII(ByteArrayToString(bytespcmtosend));
        serialPort1.Write(bytespcmtosend, 0, bytespcmtosend.GetUpperBound(0) + 1);


      }
      catch (Exception e1)
      {
        LineOut("EXCEPTION IN DoVersionCommand()", e1);
      }


End_VersionCommand:

      LineOut("PROCESSING COMPLETE");
      if (boolcomportopen)
      {
        LineOut("CLOSING COM PORT");
        CloseSerialPort(serialPort1);
        boolcomportopen = false;
      }
      LineOut("============================");


    }//end- DoVersionCommand()

    public void DoPresetCommand()
    {
      //string stringfunction = "DoPresetCommand";

      bool boolcomportopen = false;

      byte[] bytestosend;
      byte[] bytespcmtosend;

      int inthundredths = 0;
      int intthousandths = 0;

      int intpcmcommanddelay = 5;               //milliseconds
      int intmaxcommandtimeoutTenths = 0;   //HUNDREDTHS of seconds
      int inttargetresponsecharcount = 0;
      int intmaxretries = 0;
      int intretrycount = 0;
      int intretrytimeoutmilliseconds = 0;      //milliseconds

      string stringout = "";

      try {
        //1. com port
        //2. pcm command
        //3. send command
        //4. get response
        //5. parse response
        //6. close pcm

        LineOut("*** SET E:COUNT PRESET ***");

        string strpreset = "A" + "01" + "000000" + "0" + "01";
        if (txtPreset.Text.Length > 0) {
          decimal dpreset = decimal.Parse(txtPreset.Text) * 10;
          if (dpreset > 0.09m && dpreset < 1000000m) {
            strpreset = dpreset.ToString("000000");
            strpreset = "A" + "01" + strpreset + "1" + "01";
            LineOut(" - THIS WILL BE A HOST MODE DELIVERY A PRESET VALUE OF " + txtPreset.Text);
          }
          else {
            LineOut(" - THIS WILL BE A HOST MODE DELIVERY WITH NO PRESET VALUE");
          }        
        }
        else {
          LineOut("Option 1: To skip the preset and do a Host Mode delivery enter preset of 0.");
          LineOut("Option 2: To do a pump + print delivery skip preset and reset the E:Count.");
          goto End_PresetCommand;
        }
        //LineOut(strpreset);

        // Note: By sending the "A" or the "E" command prior to the delivery it automatically puts the E:Count in host mode
        //  which requires the X command be used after the delivery has ended in order to print the delivery ticket

        LineOut("USING SERIAL PORT COM" + intcurrentcomport);
        if (!IsCOMPortValid(intcurrentcomport)) {
          stringout = "INVALID COM PORT! CHECK DEVICE MANAGER";
          LineOut(stringout);
          System.Windows.Forms.MessageBox.Show(stringout);
          goto End_PresetCommand;
        }

        //setup com port and open it
        if (boolcomportopen) {
          //if the port is open, close it (possibly due to an error)
          LineOut("COM PORT ALREADY OPEN, CLOSING");
          CloseSerialPort(serialPort1);
          boolcomportopen = false;
        }
        LineOut("INITIALIZING COM PORT");
        boolcomportopen = OpenSerialPort(serialPort1);
        if (!boolcomportopen) {
          LineOut("ERROR INITIALIZING COM PORT!");
          goto End_PresetCommand;
        }

        //build pcm and command arrays to connect HOST to REGISTER1 (0x1F 0x02) and then send V (0x56)
        LineOut("BUILDING COMMAND BYTEARRAYS");
        bytespcmtosend = StringToByteArray(Chr(31) + Chr(2));  //chr(31) = HEX 0x1F, chr(2) = HEX 0x02 
        bytestosend = StringToByteArray(strpreset);            // Built above

        //A command looks like this (from ECount Host Interface docs):
        // response: A + response + | (Response=0:Invalid PC, Response=1:Valid PC)
        // command = 11 bytes (10 bytes for E command):
        //
        // Product Code                     2 bytes   01 - 99                                      
        // Preset Tenths, Implied Decimal   6 bytes   000000 - 999999
        // Preset Enable                    1 byte    0 - 1(0 = OFF 1 = ON)
        // Byte 10(Ignored)                 1 byte    0(Send ASCII 0)
        // Byte 11(Ignored)                 1 byte    1(Send ASCII 1)

        //set parameters for V comamnd
        LineOut("INITIALIZING COMMAND CONTROL PARAMETERS");
        intmaxcommandtimeoutTenths = 30; //hundredths = N * 100ms max to wait, this will vary from command to command
        inttargetresponsecharcount = 3;      //E + response + |, this will vary from command to command
        intmaxretries = 0;                   //E cmd should not be re-sent if no response (only J should be resent if no response)
        intretrycount = 0;
        intretrytimeoutmilliseconds = 0;     //delay between retries in ms, J command is only cmd retry that is valid, J requires *min* of 250 ms between retries

      //Retry_PresetCommand:

        //clear RX buffer before sending
        LineOut("CLEARING COM PORT BUFFER");
        bacomportbuffer = new byte[] { };

        //send pcm command bytes
        LineOut("SENDING PCM CONNECT HOST-TO-REG1 COMMAND: ");
        LineOutHexAndASCII(ByteArrayToString(bytespcmtosend));
        serialPort1.Write(bytespcmtosend, 0, bytespcmtosend.GetUpperBound(0) + 1);

        //wait X ms for PCM hardware to complete port switching
        LineOut("WAITING FOR PCM TO SWITCH PORTS ..");
        intthousandths = 0;
        while (intthousandths < intpcmcommanddelay) {
          //this is a kludge
          intthousandths++;
          System.Windows.Forms.Application.DoEvents();
          System.Threading.Thread.Sleep(1);  //1 = 1ms
        }

        //send command bytes
        LineOut("SENDING E:COUNT COMMAND: ");
        LineOutHexAndASCII(ByteArrayToString(bytestosend));
        serialPort1.Write(bytestosend, 0, bytestosend.GetUpperBound(0) + 1);

        //wait for response
        inthundredths = 0;
        while ((inthundredths < intmaxcommandtimeoutTenths) && (bacomportbuffer.Length < inttargetresponsecharcount)) {
          inthundredths++;
          System.Windows.Forms.Application.DoEvents();
          System.Threading.Thread.Sleep(10);  //10 = 10ms, 10ms is one-hundredth of a second ..
          System.Windows.Forms.Application.DoEvents();
        }

        if (bacomportbuffer.Length < inttargetresponsecharcount) {
          LineOut("RESPONSE TIMEOUT EXCEEDED " + intmaxcommandtimeoutTenths + " TENTHS OF A SECOND");
          if (intretrycount < intmaxretries) {
            intretrycount++;
            System.Threading.Thread.Sleep(intretrytimeoutmilliseconds);
            LineOut("RETRY # " + intretrycount + " OF " + intmaxretries + ", TOO FEW CHARS: " + bacomportbuffer.Length.ToString());
            LineOut("PLEASE WAIT ..");
            //goto Retry_PresetCommand;
          }
          else {
            if (intmaxretries == 0) {
              LineOut("INVALID RESPONSE, TOO FEW CHARS: " + bacomportbuffer.Length.ToString());
              LineOutHexAndASCII(ByteArrayToString(bacomportbuffer));
            }
            else {
              LineOut("MAX OF " + intmaxretries + " RETRIES REACHED, TOO FEW CHARS: " + bacomportbuffer.Length.ToString());
            }
          }
        }
        else {
          LineOut("TX TO E:COUNT: " + strpreset.Length + " BYTES: " + strpreset);
          LineOut("COMMAND                    (0,1)  ....: " + strpreset.Substring(0, 1));
          LineOut("PRODUCT CODE               (1,2)  ....: " + strpreset.Substring(1, 2));
          LineOut("PRESET (TENTHS NO DECIMAL) (3,6)  ....: " + strpreset.Substring(2, 6));
          LineOut("PRESET ENABLE (0,1)        (9,1)  ....: " + strpreset.Substring(8, 1));
          LineOut("REQUIRED CHARS             (10,2)  ....: " + strpreset.Substring(9, 2));
          LineOut("");

          string stringresponse = ByteArrayToString(bacomportbuffer);
          LineOut("RX FROM E:COUNT: " + stringresponse.Length + " BYTES");
          LineOutHexAndASCII(stringresponse);

          //NOTE, C# ARRAY INDICES ARE 0-BASED
          LineOut("COMMAND ECHO               (0,1)  ....: " + stringresponse.Substring(0, 1));
          LineOut("PRODUCT CODE STATUS        (1,1)  ....: " + stringresponse.Substring(1, 1));
          LineOut("TERMINATING PIPE CHARACTER (2,1)  ....: " + stringresponse.Substring(2, 1));

          if (stringresponse.Substring(1, 1).CompareTo("1")==0) {
            LineOut("*** THE E:COUNT IS IN HOST MOST STATUS AND READY TO BE RESET TO BEGIN A DELVIERY ***");
            LineOut("*** NOTE: THE ~DELIVERY~ LEGEND SHOULD BE VISIBLE ON THE E:COUNT ***");
          }
        }

        //build pcm command arrays to disconnect HOST
        bytespcmtosend = StringToByteArray(Chr(255));

        //send pcm command bytes
        LineOut("SENDING PCM DISCONNECT COMMAND: ");
        LineOutHexAndASCII(ByteArrayToString(bytespcmtosend));
        serialPort1.Write(bytespcmtosend, 0, bytespcmtosend.GetUpperBound(0) + 1);


      }
        catch (Exception e1)
        {
          LineOut("EXCEPTION IN DoPresetCommand()", e1);
      }


    End_PresetCommand:

      LineOut("PROCESSING COMPLETE");
      if (boolcomportopen)
      {
        LineOut("CLOSING COM PORT");
        CloseSerialPort(serialPort1);
        boolcomportopen = false;
      }
      LineOut("============================");

    }//end- DoPresetCommand()



    public void DoResetCommand()
    {
      //string stringfunction = "DoResetCommand";

      bool boolcomportopen = false;

      byte[] bytestosend;
      byte[] bytespcmtosend;

      int inthundredths = 0;
      int intthousandths = 0;

      int intpcmcommanddelay = 5;               //milliseconds
      int intmaxcommandtimeoutTenths = 0;   //HUNDREDTHS of seconds
      int inttargetresponsecharcount = 0;
      int intmaxretries = 0;
      int intretrycount = 0;
      int intretrytimeoutmilliseconds = 0;      //milliseconds

      string stringout = "";

      try {
        //1. com port
        //2. pcm command
        //3. send command
        //4. get response
        //5. parse response
        //6. close pcm

        LineOut("*** SET E:COUNT RESET ***");

        LineOut("USING SERIAL PORT COM" + intcurrentcomport);
        if (!IsCOMPortValid(intcurrentcomport)) {
          stringout = "INVALID COM PORT! CHECK DEVICE MANAGER";
          LineOut(stringout);
          System.Windows.Forms.MessageBox.Show(stringout);
          goto End_ResetCommand;
        }

        //setup com port and open it
        if (boolcomportopen) {
          //if the port is open, close it (possibly due to an error)
          LineOut("COM PORT ALREADY OPEN, CLOSING");
          CloseSerialPort(serialPort1);
          boolcomportopen = false;
        }
        LineOut("INITIALIZING COM PORT");
        boolcomportopen = OpenSerialPort(serialPort1);
        if (!boolcomportopen) {
          LineOut("ERROR INITIALIZING COM PORT!");
          goto End_ResetCommand;
        }


        //Command Sequence:
        //TX: 0x1F 0x02(Connect PCM - Host to PCM - EC1)
        //TX: R
        //RX: R(Verify R is echoed)
        //RX: | (Verify pipe char)
        //TX: 0xFF(Disconnect PCM)
        //N
        string stringtosend = "R";

        //build pcm and command arrays to connect HOST to REGISTER1 (0x1F 0x02) and then send V (0x56)
        LineOut("BUILDING COMMAND BYTEARRAYS");
        bytespcmtosend = StringToByteArray(Chr(31) + Chr(2));  //chr(31) = HEX 0x1F, chr(2) = HEX 0x02 
        bytestosend = StringToByteArray(stringtosend);            // Built above

        //set parameters for V comamnd
        LineOut("INITIALIZING COMMAND CONTROL PARAMETERS");
        intmaxcommandtimeoutTenths = 1000;//10 sec = tenths = N * 100ms max to wait, this will vary from command to command
        inttargetresponsecharcount = 2;      //E + response + |, this will vary from command to command
        intmaxretries = 0;                   //E cmd should not be re-sent if no response (only J should be resent if no response)
        intretrycount = 0;
        intretrytimeoutmilliseconds = 0;     //delay between retries in ms, J command is only cmd retry that is valid, J requires *min* of 250 ms between retries

        //Retry_ResetCommand:

        //clear RX buffer before sending
        LineOut("CLEARING COM PORT BUFFER");
        bacomportbuffer = new byte[] { };

        //send pcm command bytes
        LineOut("SENDING PCM CONNECT HOST-TO-REG1 COMMAND: ");
        LineOutHexAndASCII(ByteArrayToString(bytespcmtosend));
        serialPort1.Write(bytespcmtosend, 0, bytespcmtosend.GetUpperBound(0) + 1);

        //wait X ms for PCM hardware to complete port switching
        LineOut("WAITING FOR PCM TO SWITCH PORTS ..");
        intthousandths = 0;
        while (intthousandths < intpcmcommanddelay) {
          //this is a kludge
          intthousandths++;
          System.Windows.Forms.Application.DoEvents();
          System.Threading.Thread.Sleep(1);  //1 = 1ms
        }

        //send command bytes
        LineOut("SENDING E:COUNT COMMAND: ");
        LineOutHexAndASCII(ByteArrayToString(bytestosend));
        serialPort1.Write(bytestosend, 0, bytestosend.GetUpperBound(0) + 1);

        //wait for response
        inthundredths = 0;
        while ((inthundredths < intmaxcommandtimeoutTenths) && (bacomportbuffer.Length < inttargetresponsecharcount)) {
          inthundredths++;
          System.Windows.Forms.Application.DoEvents();
          System.Threading.Thread.Sleep(10);  //10 = 10ms, 10ms is one-hundredth of a second ..
          System.Windows.Forms.Application.DoEvents();
        }

        if (bacomportbuffer.Length < inttargetresponsecharcount) {
          LineOut("RESPONSE TIMEOUT EXCEEDED " + intmaxcommandtimeoutTenths + " TENTHS OF A SECOND");
          if (intretrycount < intmaxretries) {
            intretrycount++;
            System.Threading.Thread.Sleep(intretrytimeoutmilliseconds);
            LineOut("RETRY # " + intretrycount + " OF " + intmaxretries + ", TOO FEW CHARS: " + bacomportbuffer.Length.ToString());
            LineOut("PLEASE WAIT ..");
            //goto Retry_ResetCommand;
          }
          else {
            if (intmaxretries == 0) {
              LineOut("INVALID RESPONSE, TOO FEW CHARS: " + bacomportbuffer.Length.ToString());
              LineOutHexAndASCII(ByteArrayToString(bacomportbuffer));
            }
            else {
              LineOut("MAX OF " + intmaxretries + " RETRIES REACHED, TOO FEW CHARS: " + bacomportbuffer.Length.ToString());
            }
          }
        }
        else {
          LineOut("TX TO E:COUNT: " + stringtosend.Length + " BYTES: " + stringtosend);

          string stringresponse = ByteArrayToString(bacomportbuffer);
          LineOut("RX FROM E:COUNT: " + stringresponse.Length + " BYTES");
          LineOutHexAndASCII(stringresponse);

          //NOTE, C# ARRAY INDICES ARE 0-BASED
          LineOut("COMMAND ECHO               (0,1)  ....: " + stringresponse.Substring(0, 1));
          LineOut("TERMINATING PIPE CHARACTER (1,1)  ....: " + stringresponse.Substring(1, 1));

        }

        //build pcm command arrays to disconnect HOST
        bytespcmtosend = StringToByteArray(Chr(255));

        //send pcm command bytes
        LineOut("SENDING PCM DISCONNECT COMMAND: ");
        LineOutHexAndASCII(ByteArrayToString(bytespcmtosend));
        serialPort1.Write(bytespcmtosend, 0, bytespcmtosend.GetUpperBound(0) + 1);


      }
      catch (Exception e1) {
        LineOut("EXCEPTION IN DoResetCommand()", e1);
      }


      End_ResetCommand:

      LineOut("PROCESSING COMPLETE");
      if (boolcomportopen) {
        LineOut("CLOSING COM PORT");
        CloseSerialPort(serialPort1);
        boolcomportopen = false;
      }
      LineOut("============================");
    }//end- DoResetCommand()

    public void DoMonitorCommand()
    {

    }//end- DoMonitorCommand()

    public void DoToggleValvesCommand()
    {

    }//end- DoToggleValvesCommand()

    public void DoEndCommands()
    {
      //string stringfunction = "DoEndCommand";

      bool boolcomportopen = false;

      byte[] bytestosend;
      byte[] bytespcmtosend;

      int inthundredths = 0;
      int intthousandths = 0;

      int intpcmcommanddelay = 5;               //milliseconds
      int intmaxcommandtimeoutTenths = 0;       //HUNDREDTHS of seconds
      int inttargetresponsecharcount = 0;
      int intmaxretries = 0;
      int intretrycount = 0;
      int intretrytimeoutmilliseconds = 0;      //milliseconds

      string stringout = "";

      try {
        //1. com port
        //2. pcm command
        //3. send command
        //4. get response
        //5. parse response
        //6. close pcm

        LineOut("*** SET E:COUNT END DELIVERY (N) ***");

        LineOut("USING SERIAL PORT COM" + intcurrentcomport);
        if (!IsCOMPortValid(intcurrentcomport)) {
          stringout = "INVALID COM PORT! CHECK DEVICE MANAGER";
          LineOut(stringout);
          System.Windows.Forms.MessageBox.Show(stringout);
          goto End_EndCommand;
        }

        //setup com port and open it
        if (boolcomportopen) {
          //if the port is open, close it (possibly due to an error)
          LineOut("COM PORT ALREADY OPEN, CLOSING");
          CloseSerialPort(serialPort1);
          boolcomportopen = false;
        }
        LineOut("INITIALIZING COM PORT");
        boolcomportopen = OpenSerialPort(serialPort1);
        if (!boolcomportopen) {
          LineOut("ERROR INITIALIZING COM PORT!");
          goto End_EndCommand;
        }


        //Command Sequence:
        //TX: 0x1F 0x02(Connect PCM - Host to PCM - EC1)
        //TX: N
        //RX: N(Verify N is echoed)
        //RX: | (Verify pipe char)
        //TX: 0xFF(Disconnect PCM)
        //N
        string stringtosend = "N";

        //build pcm and command arrays to connect HOST to REGISTER1 (0x1F 0x02) and then send V (0x56)
        LineOut("BUILDING COMMAND BYTEARRAYS");
        bytespcmtosend = StringToByteArray(Chr(31) + Chr(2));  //chr(31) = HEX 0x1F, chr(2) = HEX 0x02 
        bytestosend = StringToByteArray(stringtosend);            // Built above

        //set parameters for comamnd
        LineOut("INITIALIZING COMMAND CONTROL PARAMETERS");
        intmaxcommandtimeoutTenths = 1000;   //10 sec = tenths = N * 10ms max to wait, this will vary from command to command
        inttargetresponsecharcount = 2;      //E + response + |, this will vary from command to command
        intmaxretries = 0;                   //E cmd should not be re-sent if no response (only J should be resent if no response)
        intretrycount = 0;
        intretrytimeoutmilliseconds = 0;     //delay between retries in ms, J command is only cmd retry that is valid, J requires *min* of 250 ms between retries

        //Retry_EndCommand:

        //clear RX buffer before sending
        LineOut("CLEARING COM PORT BUFFER");
        bacomportbuffer = new byte[] { };

        //send pcm command bytes
        LineOut("SENDING PCM CONNECT HOST-TO-REG1 COMMAND: ");
        LineOutHexAndASCII(ByteArrayToString(bytespcmtosend));
        serialPort1.Write(bytespcmtosend, 0, bytespcmtosend.GetUpperBound(0) + 1);

        //wait X ms for PCM hardware to complete port switching
        LineOut("WAITING FOR PCM TO SWITCH PORTS ..");
        intthousandths = 0;
        while (intthousandths < intpcmcommanddelay) {
          //this is a kludge
          intthousandths++;
          System.Windows.Forms.Application.DoEvents();
          System.Threading.Thread.Sleep(1);  //1 = 1ms
        }

        //send command bytes
        LineOut("SENDING E:COUNT COMMAND: ");
        LineOutHexAndASCII(ByteArrayToString(bytestosend));
        serialPort1.Write(bytestosend, 0, bytestosend.GetUpperBound(0) + 1);

        //wait for response
        inthundredths = 0;
        while ((inthundredths < intmaxcommandtimeoutTenths) && (bacomportbuffer.Length < inttargetresponsecharcount)) {
          inthundredths++;
          System.Windows.Forms.Application.DoEvents();
          System.Threading.Thread.Sleep(10);  //10 = 10ms, 10ms is one-hundredth of a second ..
          System.Windows.Forms.Application.DoEvents();
        }

        if (bacomportbuffer.Length < inttargetresponsecharcount) {
          LineOut("RESPONSE TIMEOUT EXCEEDED " + intmaxcommandtimeoutTenths + " TENTHS OF A SECOND");
          if (intretrycount < intmaxretries) {
            intretrycount++;
            System.Threading.Thread.Sleep(intretrytimeoutmilliseconds);
            LineOut("RETRY # " + intretrycount + " OF " + intmaxretries + ", TOO FEW CHARS: " + bacomportbuffer.Length.ToString());
            LineOut("PLEASE WAIT ..");
            //goto Retry_ResetCommand;
          }
          else {
            if (intmaxretries == 0) {
              LineOut("INVALID RESPONSE, TOO FEW CHARS: " + bacomportbuffer.Length.ToString());
              LineOutHexAndASCII(ByteArrayToString(bacomportbuffer));
            }
            else {
              LineOut("MAX OF " + intmaxretries + " RETRIES REACHED, TOO FEW CHARS: " + bacomportbuffer.Length.ToString());
            }
          }
        }
        else {
          LineOut("TX TO E:COUNT: " + stringtosend.Length + " BYTES: " + stringtosend);

          string stringresponse = ByteArrayToString(bacomportbuffer);
          LineOut("RX FROM E:COUNT: " + stringresponse.Length + " BYTES");
          LineOutHexAndASCII(stringresponse);

          //NOTE, C# ARRAY INDICES ARE 0-BASED
          LineOut("COMMAND ECHO               (0,1)  ....: " + stringresponse.Substring(0, 1));
          LineOut("TERMINATING PIPE CHARACTER (1,1)  ....: " + stringresponse.Substring(1, 1));

        }

        //build pcm command arrays to disconnect HOST
        bytespcmtosend = StringToByteArray(Chr(255));

        //send pcm command bytes
        LineOut("SENDING PCM DISCONNECT COMMAND: ");
        LineOutHexAndASCII(ByteArrayToString(bytespcmtosend));
        serialPort1.Write(bytespcmtosend, 0, bytespcmtosend.GetUpperBound(0) + 1);


      }
      catch (Exception e1) {
        LineOut("EXCEPTION IN DoEndCommand()", e1);
      }


      End_EndCommand:

      LineOut("PROCESSING COMPLETE");
      if (boolcomportopen) {
        LineOut("CLOSING COM PORT");
        CloseSerialPort(serialPort1);
        boolcomportopen = false;
      }
      LineOut("============================");
    }//end- DoEndCommand()

    public void DoPrintCommands()
    {

    }//end- DoPrintCommand()

    public void DoStatusCommand()
    {
      string stringfunction = "DoStatusCommand";

      bool boolcomportopen = false;

      byte[] bytestosend;
      byte[] bytespcmtosend;

      int inthundredths = 0;
      int intthousandths = 0;

      int intpcmcommanddelay = 5;               //milliseconds
      int intmaxcommandtimeoutTenths = 0;   //hundredths of seconds
      int inttargetresponsecharcount = 0;
      int intmaxretries = 0;
      int intretrycount = 0;
      int intretrytimeoutmilliseconds = 0;      //milliseconds

      string stringout = "";

      try
      {
        //1. com port
        //2. pcm command
        //3. send command
        //4. get response
        //5. parse response
        //6. close pcm

        LineOut("*** GET E:COUNT STATUS ***");

        LineOut("USING SERIAL PORT COM" + intcurrentcomport);
        if (!IsCOMPortValid(intcurrentcomport))
        {
          stringout = "INVALID COM PORT! CHECK DEVICE MANAGER";
          LineOut(stringout);
          System.Windows.Forms.MessageBox.Show(stringout);
          goto End_StatusCommand;
        }

        //setup com port and open it
        if (boolcomportopen)
        {
          //if the port is open, close it (possibly due to an error)
          LineOut("COM PORT ALREADY OPEN, CLOSING");
          CloseSerialPort(serialPort1);
          boolcomportopen = false;
        }
        LineOut("INITIALIZING COM PORT");
        boolcomportopen = OpenSerialPort(serialPort1);
        if (!boolcomportopen)
        {
          LineOut("ERROR INITIALIZING COM PORT!");
          goto End_StatusCommand;
        }

        //build pcm and command arrays to connect HOST to REGISTER1 (0x1F 0x02) and then send V (0x56)
        LineOut("BUILDING COMMAND BYTEARRAYS");
        bytespcmtosend = StringToByteArray(Chr(31) + Chr(2));  //chr(31) = HEX 0x1F, chr(2) = HEX 0x02 
        bytestosend = StringToByteArray(Chr(74));              //chr(74) = ASCII J

        //J command response looks like this (from ECount Host Interface docs):
        //=====================================================================
        //  SHHHH  5 Hex bytes (versions prior to E135E)
        //  SHHHHC 6 Hex bytes (versions E135E and later)
        // S: Status - 1 Byte (bit 0 = LSB)
        //  Bit 0= Timeout 0-1 0=No, 1=Yes
        //  Bit 1= Print Key 0-1 0=Not pressed, 1=Pressed
        //  Bit 2= Preset 0-1 0=None or Reached, 1=Set
        //  Bit 3= Valves 0-1 0=Closed, 1=Open
        //  Bit 4= Product Flowing 0-1 0=No, 1=Yes
        //  Bit 5= Delivery Active 0-1 0=Off, 1=On
        //  Bit 6= Ticket Pending 0-1 0=No, 1=Yes
        //  Bit 7= Host Mode Status 0-1 0=No, 1=Yes
        // HHHH: Current Delivery Volume – 4 HEX Bytes
        //  These bytes represent the hex values of the volume to the hundredths of unit volume.
        //  The hex values should be 00-99 for each character.
        //  These characters are frequently unprintable characters and are not viewable inside a terminal emulation program.
        // C: XOR Checksum of SHHHH
        //  C is the XOR Checksum of the previous 5 bytes
        //  Only available in Data Block 05 and later (see the V 'Get Version' command notes regarding the Dat Block)

        //set parameters for V comamnd
        LineOut("INITIALIZING COMMAND CONTROL PARAMETERS");
        intmaxcommandtimeoutTenths = 10;     //10 tenths = 100ms max to wait, this will vary from command to command
        inttargetresponsecharcount = 6;      //6 hex buytes, this will vary from command to command
        intmaxretries = 5;                   //J cmd should be re-sent after 250ms if no response, for max of attempts (only J should be resent, no other commands should be re-sent)
        intretrycount = 0;
        intretrytimeoutmilliseconds = 150;    //delay between retries in ms, J command is only cmd retry that is valid, J requires *min* of 250 ms between attempts (100ms timeout + 150ms delay between retries)

Retry_StatusCommand:

        //clear RX buffer before sending
        LineOut("CLEARING COM PORT BUFFER");
        bacomportbuffer = new byte[] { };

        //send pcm command bytes
        LineOut("SENDING PCM CONNECT HOST-TO-REG1 COMMAND: ");
        LineOutHexAndASCII(ByteArrayToString(bytespcmtosend));
        serialPort1.Write(bytespcmtosend, 0, bytespcmtosend.GetUpperBound(0) + 1);

        //wait X ms for PCM hardware to complete port switching
        LineOut("WAITING FOR PCM TO SWITCH PORTS ..");
        intthousandths = 0;
        while (intthousandths < intpcmcommanddelay)
        {
          //this is a kludge
          intthousandths++;
          System.Windows.Forms.Application.DoEvents();
          System.Threading.Thread.Sleep(1);  //1 = 1ms
        }

        //send V command bytes
        LineOut("SENDING E:COUNT COMMAND: ");
        LineOutHexAndASCII(ByteArrayToString(bytestosend));
        serialPort1.Write(bytestosend, 0, bytestosend.GetUpperBound(0) + 1);

        //wait for response
        inthundredths = 0;
        while ((inthundredths < intmaxcommandtimeoutTenths) && (bacomportbuffer.Length < inttargetresponsecharcount))
        {
          inthundredths++;
          System.Windows.Forms.Application.DoEvents();
          System.Threading.Thread.Sleep(10);  //10 = 10ms, 10ms is one-hundredth of a second ..
          System.Windows.Forms.Application.DoEvents();
        }

        if (bacomportbuffer.Length < inttargetresponsecharcount)
        {
          LineOut("RESPONSE TIMEOUT EXCEEDED " + intmaxcommandtimeoutTenths + " TENTHS OF A SECOND");
          if (intretrycount < intmaxretries)
          {
            intretrycount++;
            System.Threading.Thread.Sleep(intretrytimeoutmilliseconds);
            LineOut("RETRY # " + intretrycount + " OF " + intmaxretries + ", TOO FEW CHARS: " + bacomportbuffer.Length.ToString());
            LineOut("PLEASE WAIT ..");
            goto Retry_StatusCommand;
          }
          else
          {
            if (intmaxretries == 0)
            {
              LineOut("INVALID RESPONSE, TOO FEW CHARS: " + bacomportbuffer.Length.ToString());
            }
            else
            {
              LineOut("MAX OF " + intmaxretries + " RETRIES REACHED, TOO FEW CHARS: " + bacomportbuffer.Length.ToString());
            }
          }
        }
        else
        {
          string stringresponse = ByteArrayToString(bacomportbuffer);
          LineOut("RX FROM E:COUNT: " + bacomportbuffer.Length.ToString() + " BYTES");
          LineOutHexAndASCII(stringresponse);

          //NOTE, C# ARRAY INDICES ARE 0-BASED

          LineOut("STATUS BYTE                (0,1)  ....: " + ByteArrayToHexStringWithPad(ByteArraySubarray(bacomportbuffer, 0, 1)));
          LineOut("HEX VOLUME BYTES           (1,4)  ....: " + ByteArrayToHexStringWithPad(ByteArraySubarray(bacomportbuffer, 1, 4)));
          LineOut("CHECKSUM BYTE              (5,1)  ....: " + ByteArrayToHexStringWithPad(ByteArraySubarray(bacomportbuffer, 5, 1)));

          byte[] bytenochecksum = new byte[5];
          for (int idx = 0; idx < 5; idx++)
          {
            bytenochecksum[idx] = bacomportbuffer[idx];
          }
          byte[] bytesum = new byte[1];
          bytesum[0] = GetByteChecksumV1(bytenochecksum);
          byte[] byteval = new byte[1];
          byteval[0] = bacomportbuffer[5];
          if (bytesum[0] != byteval[0])
          {
            LineOut("CHECKSUM ERROR:");
            LineOut("HEX(XORCHECKSUM_OF_BYTES) = " + ByteArrayToHexStringWithPad(bytesum));
            LineOut("HEX(CHECKSUMBYTE) = " + ByteArrayToHexStringWithPad(byteval));
            goto Disconnect_StatusCommand;
          }

          string strstatusbinary = Convert.ToString(bacomportbuffer[0], 2).PadLeft(8, '0');
          LineOut("STATUS BIT# ....:  76543210");
          LineOut("       VALUE ...:  " + strstatusbinary);
          OutputStatusBits(1, strstatusbinary);

          string stringregisterstatedescription = "";
          int intregisterstate = 0;
          string stringjcommandvolume = "";

          bool booldeliveryactive = false;
          bool boolproductflowing = false;
          bool boolticketpending = false;

          int intbinarystringbyte = 3; //byte 0 on left of the string
          if (strstatusbinary[intbinarystringbyte].CompareTo('1') == 0)
          {
            boolproductflowing = true;
          }
          intbinarystringbyte = 2;
          if (strstatusbinary[intbinarystringbyte].CompareTo('1') == 0)
          {
            booldeliveryactive = true;
          }
          intbinarystringbyte = 1;
          if (strstatusbinary[intbinarystringbyte].CompareTo('1') == 0)
          {
            boolticketpending = true;
          }
          if (!booldeliveryactive)
          {
            if (!boolticketpending)
            {
              intregisterstate = 100;
              stringregisterstatedescription = "NO DELIVERY ACTIVE";
            }
            else
            {
              intregisterstate = 400;
              stringregisterstatedescription = "HOST MODE TICKET PENDING";

              //LOOK AT OTHER VARIABLES HERE
              intbinarystringbyte = 7; 
              if (strstatusbinary[intbinarystringbyte].CompareTo('1') == 0)
              {
                stringregisterstatedescription += ", NO-FLOW TIMEOUT ELAPSED";
              }
              intbinarystringbyte = 6;
              if (strstatusbinary[intbinarystringbyte].CompareTo('1') == 0)
              {
                stringregisterstatedescription += ", <PRINT> KEY PRESSED";
              }

            }
          }
          else
          {
            if (!boolproductflowing)
            {
              intregisterstate = 200;
              stringregisterstatedescription = "DELIVERY ACTIVE, NO PRODUCT FLOW";
            }
            else
            {
              intregisterstate = 300;
              stringregisterstatedescription = "DELIVERY ACTIVE, PRODUCT FLOWING";
            }

            //LOOK AT OTHER VARIABLES HERE
            intbinarystringbyte = 5;
            if (strstatusbinary[intbinarystringbyte].CompareTo('1') == 0)
            {
              stringregisterstatedescription += ", PRESET PENDING";
            }
            intbinarystringbyte = 4;
            if (strstatusbinary[intbinarystringbyte].CompareTo('1') == 0)
            {
              stringregisterstatedescription += ", VALVES OPEN";
            }
            else
            {
              stringregisterstatedescription += ", VALVES CLOSED";
            }
            intbinarystringbyte = 0;
            if (strstatusbinary[intbinarystringbyte].CompareTo('1') == 0)
            {
              stringregisterstatedescription += ", HOST MODE ACTIVE";
            }

          }

          if (intregisterstate > 100)
          {
            //if delivery active or ticket pending then show register volume

            //NOTE: IF PRODUCT IS FLOWING THIS VOLUME WILL BE 'STALE' AS THE CURRENT DELIVERY VOLUME
            // DISPLAYED ON THE ECOUNT WILL BE UPDATED IMMEDIATLEY WHILE PULSES ARE BEING DETECTED, THE RESULT WILL BE 
            // THAT THE VALUE RECIEVED HERE WILL APPEAR TO LAG BEHIND THE ECOUNT DISPLAYED VOLUME. AS SOON
            // AS PRODUCT FLOW STOPS THE VOLUME REFLECTED HERE SHOULD 'CATCH UP' TO THE E:COUNT DISPLAYED VOLUME

            byte[] balocalvolumebytes = new byte[4];
            for (int idz = 0; idz < 4; idz++)
            {
              balocalvolumebytes[idz] = bacomportbuffer[idz + 1];
            }
            string stringvolume = CurrentVolumeFromHexByteArrayToDecimalString(balocalvolumebytes);
            if (!IsNumeric(stringvolume))
            {
              LineOut("INVALID J VALUE-BYTES: VOLUME IS NON-NUMERIC");
            }
            else
            {
              stringjcommandvolume = stringvolume;
            }


          }//end- register state is in delivery or ticket pending

          intlastregisterstate = intregisterstate;

          LineOut("E:COUNT STATE = " + intregisterstate + ": " + stringregisterstatedescription);
          if (stringjcommandvolume.Length > 0)
          {
            LineOut("J COMMAND VOLUME: " + stringjcommandvolume);
          }

        }//end- buffer length is valid for response


Disconnect_StatusCommand:
        
        //build pcm command arrays to disconnect HOST
        bytespcmtosend = StringToByteArray(Chr(255));

        //send pcm command bytes
        LineOut("SENDING PCM DISCONNECT COMMAND: ");
        LineOutHexAndASCII(ByteArrayToString(bytespcmtosend));
        serialPort1.Write(bytespcmtosend, 0, bytespcmtosend.GetUpperBound(0) + 1);


      }
      catch (Exception e1)
      {
        LineOut("EXCEPTION IN DoStatusCommand()", e1);
      }


End_StatusCommand:

      LineOut("PROCESSING COMPLETE");
      if (boolcomportopen)
      {
        LineOut("CLOSING COM PORT");
        CloseSerialPort(serialPort1);
        boolcomportopen = false;
      }
      LineOut("============================");


    }//end- DoStatusCommand()


    public void DoPassThroughPrintCommand()
    {
      //string stringfunction = "DoPassThroughPrintCommand";

      bool boolcomportopen = false;

      byte[] bytestosend;
      byte[] bytespcmtosend;

      int inthundredths = 0;
      int intthousandths = 0;

      int intpcmcommanddelay = 5;              //milliseconds
      int intmaxcommandtimeoutTenths = 0;  //HUNDREDTHS of second
      int inttargetresponsecharcount = 0;
      int intmaxretries = 0;
      int intretrycount = 0;
      int intretrytimeoutmilliseconds = 0;     //milliseconds

      string stringresponse = "";
      string stringout = "";

      try
      {
        //1. com port
        //2. pcm command
        //3. send command
        //4. get response
        //5. parse response
        //6. close pcm

        LineOut("*** PASS THROUGH PRINT TEXT ***");

        LineOut("USING SERIAL PORT COM" + intcurrentcomport);
        if (!IsCOMPortValid(intcurrentcomport))
        {
          stringout = "INVALID COM PORT! CHECK DEVICE MANAGER";
          LineOut(stringout);
          System.Windows.Forms.MessageBox.Show(stringout);
          goto End_DoPassThroughPrintCommand;
        }

        //verify regsiter version 175+
        if (intlastregisternumericversionnumber < 175)
        {
          stringout = "INVALID E:COUNT FIRMWARE VERSION (" + intlastregisternumericversionnumber + ")! CHECK VERSION TO CONTINUE";
          LineOut(stringout);
          System.Windows.Forms.MessageBox.Show(stringout);
          //NOTE: IF THE PROGRAM STOPS HERE YOU NEED TO USE THE 'GET VERSION' COMMAND FIRST
          goto End_DoPassThroughPrintCommand;
        }

        //verify regsiter state is no delivery active, no product flowing, no ticket pending
        if (intlastregisterstate != 100)
        {
          stringout = "INVALID E:COUNT STATUS (" + intlastregisterstate + ")! CHECK STATUS TO CONTINUE";
          LineOut(stringout);
          System.Windows.Forms.MessageBox.Show(stringout);
          //NOTE: IF THE PROGRAM STOPS HERE YOU NEED TO USE THE 'GET STATUS' COMMAND FIRST
          goto End_DoPassThroughPrintCommand;
        }


        //setup com port and open it
        if (boolcomportopen)
        {
          //if the port is open, close it (possibly due to an error)
          LineOut("COM PORT ALREADY OPEN, CLOSING");
          CloseSerialPort(serialPort1);
          boolcomportopen = false;
        }
        LineOut("INITIALIZING COM PORT");
        boolcomportopen = OpenSerialPort(serialPort1);
        if (!boolcomportopen)
        {
          LineOut("ERROR INITIALIZING COM PORT!");
          goto End_DoPassThroughPrintCommand;
        }


        // The Pass-Through Printing Commands (o,q,r) may be used to print
        //  a non-delivery ticket regardless of the type of printer
        //  connected to the E:Count.
        // The pass-through printing commands are only valid when a
        //  delivery is not active and there is no delivery ticket pending.
        //
        // There are 3 Pass-Through Commands:
        //
        // 1. ‘o’ = Start Pass-Through Ticket
        //  Based on the E:Count printer setting the o command will
        //  verify the printer is online, verify the printer has paper,
        //  advance to the next black mark (if enabled), and the print
        //  the logo stored in the printer (if configured). The o
        //  command only needs to be called once per pass-through
        //  ticket.
        //
        // 2. ‘q’ = Print Line on Pass-Through Ticket
        //  Based on the E:Count printer setting the q command takes 25
        //  ASCII text bytes as arguments and will verify the printer
        //  is online, verify the printer has paper, print the 25 ASCII
        //  text bytes, advance the form feed 1 line, and return the
        //  ‘carriage’ to the beginning of the line. The q command may
        //  be called as many times as necessary to add lines to the
        //  pass-through ticket.
        //
        // 3. ‘r’ = Finish Pass-Through Ticket
        //  Based on the E:Count printer setting the r command will
        //  verify the printer is online, verify the printer has paper,
        //  do a full or partial cut (if enabled), advance the
        //  necessary number of linefeed to eject the paper from the
        //  printer (if enabled), and advance to the next black mark
        //  (if enabled). The r command only needs to be called once
        //  per pass-through ticket.


        //step 1. initialize pass-through printing

        //build pcm and command arrays to connect HOST to REGISTER1 (0x1F 0x02) and then send command
        LineOut("BUILDING COMMAND BYTEARRAYS");
        bytespcmtosend = StringToByteArray(Chr(31) + Chr(2));  //chr(31) = HEX 0x1F, chr(2) = HEX 0x02 
        bytestosend = StringToByteArray(Chr(111));             //chr(111) = ASCII o

        //set parameters for o comamnd
        LineOut("INITIALIZING COMMAND CONTROL PARAMETERS");
        intmaxcommandtimeoutTenths = 200;  //100 HUNDREDTHS = 2000ms max to wait, this will vary from command to command
        inttargetresponsecharcount = 3;        //3 bytes, this will vary from command to command
        intmaxretries = 0;                     //o command should not be re-sent
        intretrycount = 0;
        intretrytimeoutmilliseconds = 0;       //delay between retries in ms

        //clear RX buffer before sending
        LineOut("CLEARING COM PORT BUFFER");
        bacomportbuffer = new byte[] { };

        //send pcm command bytes
        LineOut("SENDING PCM CONNECT HOST-TO-REG1 COMMAND: ");
        LineOutHexAndASCII(ByteArrayToString(bytespcmtosend));
        serialPort1.Write(bytespcmtosend, 0, bytespcmtosend.GetUpperBound(0) + 1);

        //wait X ms for PCM hardware to complete port switching
        LineOut("WAITING FOR PCM TO SWITCH PORTS ..");
        intthousandths = 0;
        while (intthousandths < intpcmcommanddelay)
        {
          //this is a kludge
          intthousandths++;
          System.Windows.Forms.Application.DoEvents();
          System.Threading.Thread.Sleep(1);  //1 = 1ms
        }

        //send o command bytes
        LineOut("SENDING E:COUNT COMMAND: ");
        LineOutHexAndASCII(ByteArrayToString(bytestosend));
        serialPort1.Write(bytestosend, 0, bytestosend.GetUpperBound(0) + 1);

        //wait for response
        inthundredths = 0;
        while ((inthundredths < intmaxcommandtimeoutTenths) && (bacomportbuffer.Length < inttargetresponsecharcount))
        {
          inthundredths++;
          System.Windows.Forms.Application.DoEvents();
          System.Threading.Thread.Sleep(10);  //10 = 10ms, 10ms is one-hundredth of a second ..
          System.Windows.Forms.Application.DoEvents();
        }

        if (bacomportbuffer.Length < inttargetresponsecharcount)
        {
          LineOut("INVALID RESPONSE, TOO FEW CHARS: " + bacomportbuffer.Length.ToString());
          if (inthundredths >= intmaxcommandtimeoutTenths)
          {
            LineOut("RESPONSE TIMEOUT EXCEEDED " + intmaxcommandtimeoutTenths + " TENTHS OF A SECOND");
          }
          goto Disconnect_DoPassThroughPrintCommand;
        }
        else
        {
          stringresponse = ByteArrayToString(bacomportbuffer);
          LineOut("RX FROM E:COUNT: " + bacomportbuffer.Length.ToString() + " BYTES");
          LineOutHexAndASCII(stringresponse);

          //Parse o Response:
          //  o + return_value + | return_value Result
          //  ---------------------------
          //  ASCII  0  Paper Error
          //  ASCII  1  Command Successful
          //  ASCII  2  Printer Error
          //  ASCII  3  “NO PRINTER”

          stringresponse = ByteArrayToString(bacomportbuffer).Substring(1, 1);
          if (stringresponse.CompareTo("0") == 0)
          {
            LineOut("RESPONSE = PAPER ERROR, CANNOT PASS-THROUGH PRINT!");
            goto Disconnect_DoPassThroughPrintCommand;
          }
          else if (stringresponse.CompareTo("2") == 0)
          {
            LineOut("RESPONSE = PRINTER ERROR, CANNOT PASS-THROUGH PRINT!");
            goto Disconnect_DoPassThroughPrintCommand;
          }
          else if (stringresponse.CompareTo("3") == 0)
          {
            LineOut("RESPONSE = NO PRINTER CONFIGURED, CANNOT PASS-THROUGH PRINT!");
            goto Disconnect_DoPassThroughPrintCommand;
          }
          else if (stringresponse.CompareTo("1") == 0)
          {
            LineOut("RESPONSE = PRINTER READY");
          }
          else 
          {
            LineOut("UNKNOWN RESPONSE, CANNOT PASS-THROUGH PRINT!");
            goto Disconnect_DoPassThroughPrintCommand;
          }
        }//end- buffer length is valid for response


        //step 2. Send each line of 25 ASCII chars to print (repeat as necessary)
        //TO GET HERE, RESPONSE MUST BE VALID

  //                          1234567890123456789012345
  string stringtexttoprint = "==SAMPLE TICKET FOLLOWS==";
        stringtexttoprint += "                         ";
        stringtexttoprint += "BILL AND BARBERA SMITH   ";
        stringtexttoprint += "12345 MAIN STREET        ";
        stringtexttoprint += "ANYTOWN, USA 12345-9876  ";
        stringtexttoprint += "PHONE 555-555-1212       ";
        stringtexttoprint += "TANK# A123456789         ";
        stringtexttoprint += "                         ";
        stringtexttoprint += "PROPANE GALLONS    1013.5";
        stringtexttoprint += "PRICE/GALLON      $2.3990";
        stringtexttoprint += "EXTENDED        $2,431.39";
        stringtexttoprint += "HAZMAT FEE          $4.56";
        stringtexttoprint += "WILL-CALL CHARGE   $50.00";
        stringtexttoprint += "-------------------------";
        stringtexttoprint += "SUBTOTAL        $2,485.95";
        stringtexttoprint += "SALES TAX @7.0%   $174.02";
        stringtexttoprint += "EXCISE TAX @$0.017 $17.23";
        stringtexttoprint += "=========================";
        stringtexttoprint += "AMOUNT DUE      $2,677.20";
        //stringtexttoprint += "                         ";
        //stringtexttoprint += "IF PAID IN 10 DAYS       ";
        //stringtexttoprint += "YOU MAY DEDUCT $72.94    ";
        //stringtexttoprint += "PAY ONLY $2,604.26       ";
        //stringtexttoprint += "IF PAID BY FEB 11, 2013  ";
        stringtexttoprint += "                         ";
        stringtexttoprint += "THANK YOU FOR ALLOWING US";
        stringtexttoprint += "  TO SERVE YOU TODAY!!   ";

        int intlinecount = stringtexttoprint.Length / 25;
        LineOut("PRINTING " + intlinecount + " LINES OF TEXT");



        //FOR THIS COMAMND, WE'RE ONLY GOING TO CALL THE PCM COMMAND ONCE

        LineOut("BUILDING PCM COMMAND BYTEARRAY");
        bytespcmtosend = StringToByteArray(Chr(31) + Chr(2));  //chr(31) = HEX 0x1F, chr(2) = HEX 0x02 

        //clear RX buffer before sending
        LineOut("CLEARING COM PORT BUFFER");
        bacomportbuffer = new byte[] { };
        
        //send pcm command bytes
        LineOut("SENDING PCM CONNECT HOST-TO-REG1 COMMAND: ");
        LineOutHexAndASCII(ByteArrayToString(bytespcmtosend));
        serialPort1.Write(bytespcmtosend, 0, bytespcmtosend.GetUpperBound(0) + 1);

        //wait X ms for PCM hardware to complete port switching
        LineOut("WAITING FOR PCM TO SWITCH PORTS ..");
        intthousandths = 0;
        while (intthousandths < intpcmcommanddelay)
        {
          //this is a kludge
          intthousandths++;
          System.Windows.Forms.Application.DoEvents();
          System.Threading.Thread.Sleep(1);  //1 = 1ms
        }

        for (int intthisline = 0; intthisline < intlinecount; intthisline++)
        {

          int intonebasedlinenum = intthisline + 1;
          LineOut("PRINTING PASS-THROUGH LINE # " + intonebasedlinenum + " OF " + intlinecount + " LINES OF TEXT");

          //build pcm and command arrays to connect HOST to REGISTER1 (0x1F 0x02) and then send command
          LineOut("BUILDING PASS-THROUGH PRINT LINE COMMAND BYTEARRAY");
          bytestosend = StringToByteArray(Chr(113) + stringtexttoprint.Substring(25 * intthisline, 25));  //chr(113) = ASCII q + 25 bytes to print, per command
                                                                                                        //must be padded, of necessary
          //clear RX buffer before sending
          LineOut("CLEARING COM PORT BUFFER");
          bacomportbuffer = new byte[] { };

          //set parameters for qcomamnd
          LineOut("INITIALIZING COMMAND CONTROL PARAMETERS");
          intmaxcommandtimeoutTenths = 200;  //200 HUNDREDTHS = 2000ms max to wait, this will vary from command to command
          inttargetresponsecharcount = 3;        //3 bytes, this will vary from command to command
          intmaxretries = 0;                     //o command should not be re-sent
          intretrycount = 0;
          intretrytimeoutmilliseconds = 0;       //delay between retries in ms

          //send o command bytes
          LineOut("SENDING E:COUNT COMMAND: ");
          LineOutHexAndASCII(ByteArrayToString(bytestosend));
          serialPort1.Write(bytestosend, 0, bytestosend.GetUpperBound(0) + 1);

          //wait for response
          inthundredths = 0;
          while ((inthundredths < intmaxcommandtimeoutTenths) && (bacomportbuffer.Length < inttargetresponsecharcount))
          {
            inthundredths++;
            System.Windows.Forms.Application.DoEvents();
            System.Threading.Thread.Sleep(10);  //10 = 10ms, 10ms is one-hundredth of a second ..
            System.Windows.Forms.Application.DoEvents();
          }

          if (bacomportbuffer.Length < inttargetresponsecharcount)
          {
            LineOut("INVALID RESPONSE, TOO FEW CHARS: " + bacomportbuffer.Length.ToString());
            if (inthundredths >= intmaxcommandtimeoutTenths)
            {
              LineOut("RESPONSE TIMEOUT EXCEEDED " + intmaxcommandtimeoutTenths + " TENTHS OF A SECOND");
            }
            goto SendFinish_DoPassThroughPrintCommand; //must 'finish' pass-through printing ...
          }
          else
          {
            stringresponse = ByteArrayToString(bacomportbuffer);
            LineOut("RX FROM E:COUNT: " + bacomportbuffer.Length.ToString() + " BYTES");
            LineOutHexAndASCII(stringresponse);

            //Parse o Response:
            //  o + return_value + | return_value Result
            //  ---------------------------
            //  ASCII  0  Paper Error
            //  ASCII  1  Command Successful
            //  ASCII  2  Printer Error
            //  ASCII  3  “NO PRINTER”

            stringresponse = ByteArrayToString(bacomportbuffer).Substring(1, 1);
            if (stringresponse.CompareTo("0") == 0)
            {
              LineOut("RESPONSE = PAPER ERROR, CANNOT PASS-THROUGH PRINT!");
              goto SendFinish_DoPassThroughPrintCommand; //must 'finish' pass-through printing ...
            }
            else if (stringresponse.CompareTo("2") == 0)
            {
              LineOut("RESPONSE = PRINTER ERROR, CANNOT PASS-THROUGH PRINT!");
              goto SendFinish_DoPassThroughPrintCommand; //must 'finish' pass-through printing ...
            }
            else if (stringresponse.CompareTo("3") == 0)
            {
              LineOut("RESPONSE = NO PRINTER CONFIGURED, CANNOT PASS-THROUGH PRINT!");
              goto SendFinish_DoPassThroughPrintCommand; //must 'finish' pass-through printing ...
            }
            else if (stringresponse.CompareTo("1") == 0)
            {
              LineOut("RESPONSE = PRINTER READY");
            }
            else
            {
              LineOut("UNKNOWN RESPONSE, CANNOT PASS-THROUGH PRINT!");
              goto SendFinish_DoPassThroughPrintCommand; //must 'finish' pass-through printing ...
            }
          }//end- buffer length is valid for response


          //NOTE: IT IS IMPORTANT TO SLEEP HERE FOR EACH LINE TO ALLOW THE PRINTER/PCM COMMS TO FINISH!
          // HOW LONG DEPENDS ON THE PRINTER USED, BUT THE PIPE CHARACTER MAY/WILL COME BACK BEFORE THE PRINTER ACTUALLY FINISHES PRINTING
          // DEPENDING ON THE PRINTER BUFFERING HARDWARE
          // WE RECOMMEDN GENERATING A LONG TICKET (SUHC AS THIS ONE) AND TESTING YOUR CODE BY PRINTING
          // THE LONG TICKET REPEATEDLY UNTIL IT ALWAYS PRINTS, THEN AD 2% MORE TIME TO THAT DELAY IN THIS LOOP
          //
          // BLASTER: 150MS = TOO SHORT!, 200 MS = TOO SHORT!, 250MS = OK (10)
          //
          // CITIZEN CTS-651: 100MS=OK, 250MS=OK, 25MS=ok (1)
          //
          for (int intsleep = 0; intsleep < 1; intsleep++)
          {
            System.Windows.Forms.Application.DoEvents();
            System.Threading.Thread.Sleep(25);   //25X1=25ms, 200x10=2000ms=2seconds, etc..
          }
          

        }//end- for each line to print




SendFinish_DoPassThroughPrintCommand:
        //step 3. End pass through printing

        //build pcm and command arrays to connect HOST to REGISTER1 (0x1F 0x02) and then send command
        LineOut("BUILDING COMMAND BYTEARRAYS");
        bytespcmtosend = StringToByteArray(Chr(31) + Chr(2));  //chr(31) = HEX 0x1F, chr(2) = HEX 0x02 
        bytestosend = StringToByteArray(Chr(114));             //chr(114) = ASCII r

        //set parameters for o comamnd
        LineOut("INITIALIZING COMMAND CONTROL PARAMETERS");
        intmaxcommandtimeoutTenths = 600;  //600 HUNDREDTHS = 6000ms max to wait, this will vary from command to command, some printers are slow to finish
        inttargetresponsecharcount = 3;        //3 bytes, this will vary from command to command
        intmaxretries = 0;                     //o command should not be re-sent
        intretrycount = 0;
        intretrytimeoutmilliseconds = 0;       //delay between retries in ms

        //clear RX buffer before sending
        LineOut("CLEARING COM PORT BUFFER");
        bacomportbuffer = new byte[] { };

        //send pcm command bytes
        LineOut("SENDING PCM CONNECT HOST-TO-REG1 COMMAND: ");
        LineOutHexAndASCII(ByteArrayToString(bytespcmtosend));
        serialPort1.Write(bytespcmtosend, 0, bytespcmtosend.GetUpperBound(0) + 1);

        //wait X ms for PCM hardware to complete port switching
        LineOut("WAITING FOR PCM TO SWITCH PORTS ..");
        intthousandths = 0;
        while (intthousandths < intpcmcommanddelay)
        {
          //this is a kludge
          intthousandths++;
          System.Windows.Forms.Application.DoEvents();
          System.Threading.Thread.Sleep(1);  //1 = 1ms
        }

        //send o command bytes
        LineOut("SENDING E:COUNT COMMAND: ");
        LineOutHexAndASCII(ByteArrayToString(bytestosend));
        serialPort1.Write(bytestosend, 0, bytestosend.GetUpperBound(0) + 1);

        //wait for response
        inthundredths = 0;
        while ((inthundredths < intmaxcommandtimeoutTenths) && (bacomportbuffer.Length < inttargetresponsecharcount))
        {
          inthundredths++;
          System.Windows.Forms.Application.DoEvents();
          System.Threading.Thread.Sleep(10);  //10 = 10ms, 10ms is one-hundredth of a second ..
          System.Windows.Forms.Application.DoEvents();
        }

        if (bacomportbuffer.Length < inttargetresponsecharcount)
        {
          LineOut("INVALID RESPONSE, TOO FEW CHARS: " + bacomportbuffer.Length.ToString());
          if (inthundredths >= intmaxcommandtimeoutTenths)
          {
            LineOut("RESPONSE TIMEOUT EXCEEDED " + intmaxcommandtimeoutTenths + " TENTHS OF A SECOND");
          }
          goto Disconnect_DoPassThroughPrintCommand;
        }
        else
        {
          stringresponse = ByteArrayToString(bacomportbuffer);
          LineOut("RX FROM E:COUNT: " + bacomportbuffer.Length.ToString() + " BYTES");
          LineOutHexAndASCII(stringresponse);

          //Parse o Response:
          //  o + return_value + | return_value Result
          //  ---------------------------
          //  ASCII  0  Paper Error
          //  ASCII  1  Command Successful
          //  ASCII  2  Printer Error
          //  ASCII  3  “NO PRINTER”

          stringresponse = ByteArrayToString(bacomportbuffer).Substring(1, 1);
          if (stringresponse.CompareTo("0") == 0)
          {
            LineOut("RESPONSE = PAPER ERROR, CANNOT PASS-THROUGH PRINT!");
            goto Disconnect_DoPassThroughPrintCommand;
          }
          else if (stringresponse.CompareTo("2") == 0)
          {
            LineOut("RESPONSE = PRINTER ERROR, CANNOT PASS-THROUGH PRINT!");
            goto Disconnect_DoPassThroughPrintCommand;
          }
          else if (stringresponse.CompareTo("3") == 0)
          {
            LineOut("RESPONSE = NO PRINTER CONFIGURED, CANNOT PASS-THROUGH PRINT!");
            goto Disconnect_DoPassThroughPrintCommand;
          }
          else if (stringresponse.CompareTo("1") == 0)
          {
            LineOut("RESPONSE = PRINTER READY");
          }
          else
          {
            LineOut("UNKNOWN RESPONSE, CANNOT PASS-THROUGH PRINT!");
            goto Disconnect_DoPassThroughPrintCommand;
          }
        }//end- buffer length is valid for response



Disconnect_DoPassThroughPrintCommand:

        //build pcm command arrays to disconnect HOST
        bytespcmtosend = StringToByteArray(Chr(255));

        //send pcm command bytes
        LineOut("SENDING PCM DISCONNECT COMMAND: ");
        LineOutHexAndASCII(ByteArrayToString(bytespcmtosend));
        serialPort1.Write(bytespcmtosend, 0, bytespcmtosend.GetUpperBound(0) + 1);


      }
      catch (Exception e1)
      {
        LineOut("EXCEPTION IN DoPassThroughPrintCommand()", e1);
      }


End_DoPassThroughPrintCommand:

      LineOut("PROCESSING COMPLETE");
      if (boolcomportopen)
      {
        LineOut("CLOSING COM PORT");
        CloseSerialPort(serialPort1);
        boolcomportopen = false;
      }
      LineOut("============================");


    }//end- DoPassThroughPrintCommand()


    private void EnableControls(bool boolenabled)
    {
      btnGetVersion.Enabled = boolenabled;
      btnGetStatus.Enabled = boolenabled;
      btnPassThroughPrint.Enabled = boolenabled;
      btnListCOMPorts.Enabled = boolenabled;
      btnSetPreset.Enabled = boolenabled;
      txtPreset.Enabled = boolenabled;
      btnReset.Enabled = boolenabled;
      btnMonitor.Enabled = boolenabled;
      btnToggleValves.Enabled = boolenabled;
      btnEnd.Enabled = boolenabled;
      btnPrint.Enabled = boolenabled;
    }

    private void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
      //could check any, button1 is as good as any
      if (!btnGetVersion.Enabled) 
      {
        //the control is not enabled, cancel the form unload since the app is busy
        e.Cancel = true;
      }
    }

    private void btnGetVersion_Click(object sender, EventArgs e)
    {
      EnableControls(false);
      DoVersionCommand();
      EnableControls(true);
    }

    private void btnGetStatus_Click(object sender, EventArgs e)
    {
      EnableControls(false);
      DoStatusCommand();
      EnableControls(true);
    }

    private void btnPassThroughPrint_Click(object sender, EventArgs e)
    {
      EnableControls(false);
      DoPassThroughPrintCommand();
      EnableControls(true);
    }

    private void btnListCOMPorts_Click(object sender, EventArgs e)
    {
      int intserialportcount = 0;
      LineOut("CURRENT COM PORT: " + intcurrentcomport);
      LineOut("VALID COM PORTS:");
      foreach (string stringtemplabel in System.IO.Ports.SerialPort.GetPortNames()) {
        string sel = "";
        if (stringtemplabel.CompareTo("COM" + intcurrentcomport.ToString()) == 0) {
          sel = " **";
        }
        intserialportcount++;
        LineOut(" " + stringtemplabel + sel);
      }
      LineOut(" " + intserialportcount + " COM Ports found.");
      LineOut("============================");
    }

    private void btnSetPreset_Click(object sender, EventArgs e)
    {
      EnableControls(false);
      DoPresetCommand();
      EnableControls(true);
    }

    private void btnReset_Click(object sender, EventArgs e)
    {
      EnableControls(false);
      DoResetCommand();
      EnableControls(true);
    }

    private void btnMonitor_Click(object sender, EventArgs e)
    {
      EnableControls(false);
      DoMonitorCommand();
      EnableControls(true);
    }

    private void btnToggleValves_Click(object sender, EventArgs e)
    {
      EnableControls(false);
      DoToggleValvesCommand();
      EnableControls(true);
    }

    private void btnEnd_Click(object sender, EventArgs e)
    {
      EnableControls(false);
      DoEndCommands();
      EnableControls(true);
    }

    private void btnPrint_Click(object sender, EventArgs e)
    {
      EnableControls(false);
      DoPrintCommands();
      EnableControls(true);
    }
  }
}
