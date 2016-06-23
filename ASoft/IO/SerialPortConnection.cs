using System; 
using System.Collections.Generic; 
using System.Text; 
using System.IO.Ports;
using System.Linq;
using System.Globalization;
namespace ASoft.IO
{
    public class SerialPortConnection
    {
          SerialPort _serialPort = null;
        private bool dataReceivedFlag = false;
        public SerialPortConnection(string comPortName) 
        { 
            _serialPort = new SerialPort(comPortName);
            _serialPort.BaudRate = 9600;
            _serialPort.Parity = Parity.None;
            _serialPort.DataBits = 8;
            _serialPort.StopBits = StopBits.One;
            _serialPort.Handshake = Handshake.None;
            _serialPort.RtsEnable = true;
            _serialPort.ReadTimeout = 2000;
            setSerialPort(); 
        }

        public void setSerialPort(string comPortName, int baudRate, int dataBits, int stopBits) 
        { 
            if (_serialPort.IsOpen) 
                _serialPort.Close();  
            _serialPort.PortName = comPortName; 
            _serialPort.BaudRate = baudRate; 
            _serialPort.Parity = Parity.None; 
            _serialPort.DataBits = dataBits; 
            _serialPort.StopBits = (StopBits)stopBits;  
            _serialPort.ReadTimeout = 3000;  
            setSerialPort(); 
        }

        public void Open() {
            _serialPort.Open();
        }
        public void Close()
        {
            _serialPort.Close();
        }
        void setSerialPort() 
        { 
            if (_serialPort != null) 
            { 
                //设置触发DataReceived事件的字节数为1 
                _serialPort.ReceivedBytesThreshold = 1;
                //接收到一个字节时，也会触发DataReceived事件 _serialPort_DataReceived 
                _serialPort.DataReceived += new SerialDataReceivedEventHandler(_serialPort_DataReceived); 
                //接收数据出错,触发事件 
                //_serialPort.ErrorReceived += new SerialErrorReceivedEventHandler(_serialPort_ErrorReceived);  
            } 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sendData"></param>
        /// <param name="overTime"></param>
        /// <param name="endByte"></param>
        /// <returns></returns>
        public byte[] SendCommand(byte[] sendData,  int overTime, int length)
        {
            _serialPort.Write(sendData, 0, sendData.Length);
            byte[] receivedData = null;
            int num = 0;
            while (receivedData == null && num++ < overTime) {
                if (dataReceivedFlag && receivedData == null && _serialPort.BytesToRead >= length)
                {
                    receivedData = new byte[_serialPort.BytesToRead];
                    _serialPort.Read(receivedData, 0, _serialPort.BytesToRead); //读取数据 
                    _serialPort.DiscardInBuffer();
                }
                System.Threading.Thread.Sleep(10);
            }  
            return receivedData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (_serialPort.IsOpen)     //此处可能没有必要判断是否打开串口，但为了严谨性，我还是加上了
            {
                dataReceivedFlag = true;
            }
            else
            {
                throw new Exception( _serialPort.PortName+ "串口未打开");
            }
        }
    }
}
