using System;
using System.Device.Gpio;
using System.Device.Pwm;
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
        /// 呼吸灯
        /// 定义 PWM 引脚和频率
        /// </summary>
        private static PwmChannel pwmRed, pwmGreen, pwmBlue;
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

            // 配置 GPIO 引脚功能
            Configuration.SetPinFunction(25, DeviceFunction.PWM1); // R (GPIO25)
            Configuration.SetPinFunction(26, DeviceFunction.PWM2); // G (GPIO26)
            Configuration.SetPinFunction(27, DeviceFunction.PWM3); // B (GPIO27)
            // 初始化 PWM 通道（频率 1000Hz）
            pwmRed = PwmChannel.CreateFromPin(25, 1000, 0);
            pwmGreen = PwmChannel.CreateFromPin(26, 1000, 0);
            pwmBlue = PwmChannel.CreateFromPin(27, 1000, 0);

            gpio.OpenPin(logPin, PinMode.Output);
            gpio.OpenPin(djPin, PinMode.Output);
            //高电平日志输出
            gpio.Write(logPin, PinValue.High);
            //扇热风扇
            gpio.Write(djPin, PinValue.Low);
            State();

            try
            {
                // 配置 UART2 的引脚（GPIO17=TX, GPI16=RX）
                Configuration.SetPinFunction(17, DeviceFunction.COM2_TX);
                Configuration.SetPinFunction(16, DeviceFunction.COM2_RX);
                // 使用 COM2（对应 UART2）
                uart = new SerialPort("COM2", 9600, Parity.None, 8, StopBits.One);
                uart.DataReceived += Uart_DataReceived;
                uart.Open();

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
        /// 
        /// </summary>
        /// <param name="type"></param>
        private static void State(byte type = 0)
        {
            if (type == 2)
            {
                SetColor(Color.Red);
            }
            else if (type == 1)
            {
                SetColor(Color.Orange);
            }
            else
            {
                SetColor(Color.Green);
            }
        }
        /// <summary>
        /// 颜色枚举
        /// </summary>
        private enum Color { Red, Orange, Green }
        /// <summary>
        /// 设置颜色
        /// </summary>
        /// <param name="color"></param>
        private static void SetColor(Color color)
        {
            // 关闭所有通道
            pwmRed.DutyCycle = 0;
            pwmGreen.DutyCycle = 0;
            pwmBlue.DutyCycle = 0;

            // 根据状态点亮对应颜色
            switch (color)
            {
                case Color.Green:
                    pwmGreen.DutyCycle = 1.0; // 纯绿
                    break;
                case Color.Orange:
                    pwmRed.DutyCycle = 1.0;   // 红 + 少量绿 = 橙
                    pwmGreen.DutyCycle = 0.2;
                    break;
                case Color.Red:
                    pwmRed.DutyCycle = 1.0;   // 纯红
                    break;
            }
        }
    }
}
