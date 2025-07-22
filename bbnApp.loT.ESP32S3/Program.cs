using System;
using System.Device.Gpio;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading;
using nanoFramework.Hardware.Esp32;
using System.Text;

namespace bbnApp.loT.ESP32C3
{
    public class Program
    {
        /// <summary>
        /// 指定串口
        /// </summary>
        private static SerialPort uart;
        /// <summary>
        /// 
        /// </summary>
        private static GpioController gpio;
        /// <summary>
        /// TXD
        /// </summary>
        private const int TXDPin = 17;
        /// <summary>
        /// 方向控制引脚
        /// </summary>
        private const int RXDirPin = 16;
        /// <summary>
        /// 方向控制引脚
        /// </summary>
        private const int Rs485DirPin = 8;
        /// <summary>
        /// 扇热风扇
        /// </summary>
        private static int djPin = 33;
        /// <summary>
        /// 日志
        /// </summary>
        private static int logPin = 15;
        /// <summary>
        /// 缓冲区
        /// </summary>
        private static byte[] byteBuffer = new byte[256];
        private static int bufferIndex = 0;

        public static void Main()
        {
            gpio = new GpioController();

            gpio.OpenPin(logPin, PinMode.Output);
            gpio.OpenPin(djPin, PinMode.Output);
            //高电平日志输出
            gpio.Write(logPin, PinValue.High);
            //扇热风扇
            gpio.Write(djPin, PinValue.Low);
            State();
            // 初始化方向控制引脚
            var dirPin = gpio.OpenPin(Rs485DirPin, PinMode.Output);
            try
            {
                // 配置 UART2 的引脚
                Configuration.SetPinFunction(TXDPin, DeviceFunction.COM2_TX);
                Configuration.SetPinFunction(RXDirPin, DeviceFunction.COM2_RX);

                // RTU模式（接MAX485模块）
                var modbusRtu = new ModbusHelper(
                    ModbusHelper.ModbusType.Rtu,
                    rtuPort: "COM2",
                    dirPin: Rs485DirPin);


                var data = modbusRtu.ReadHoldingRegisters(
                slaveAddress: 0x01,
                startAddress: 0x0000,
                count: 2);

                float temperature = data[0] / 10.0f;
                float humidity = data[1] / 10.0f;
                // 使用 COM2（对应 UART2）
                //uart = new SerialPort("COM2", 9600, Parity.None, 8, StopBits.One);
                //uart.DataReceived += Uart_DataReceived;
                //uart.Open();

                State();
            }
            catch
            {
                State(2);
                Debug.WriteLine("COM failed.");
            }


            // 主线程保持运行
            Thread.Sleep(Timeout.Infinite);

        }

        private static void Uart_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                // 非阻塞读取
                int bytesToRead = Math.Min(uart.BytesToRead, byteBuffer.Length - bufferIndex);
                if (bytesToRead == 0) return;

                int bytesRead = uart.Read(byteBuffer, bufferIndex, bytesToRead);
                bufferIndex += bytesRead;

                // 检测 \r\n 结尾
                if (bufferIndex >= 2 &&
                    byteBuffer[bufferIndex - 2] == 0x0D &&
                    byteBuffer[bufferIndex - 1] == 0x0A)
                {
                    string completeMessage = Encoding.UTF8.GetString(byteBuffer, 0, bufferIndex - 2);
                    Debug.WriteLine($"完整数据帧: {completeMessage}");
                    bufferIndex = 0;
                    string code = completeMessage.Substring(6, completeMessage.Length - 6);
                    if (code == "000000")
                    {
                        gpio.Write(djPin, PinValue.High);
                    }
                    else if (code == "000001")
                    {
                        gpio.Write(djPin, PinValue.Low);
                    }
                    else if (code == "000002")
                    {
                        //读取传感器数据
                        
                    }
                }
                State();
            }
            catch (TimeoutException)
            {
                State(1);
                Debug.WriteLine("警告：读取超时");
            }
        }
        /// <summary>
        /// 这里用3色Led表示状态
        /// </summary>
        /// <param name="type"></param>
        private static void State(byte type = 0)
        {
            if (type == 2)
            {
                
            }
            else if (type == 1)
            {
               
            }
            else
            {
                
            }
        }
    }
}
