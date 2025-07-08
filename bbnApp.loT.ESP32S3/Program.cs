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
        /// ָ������
        /// </summary>
        private static SerialPort uart;
        /// <summary>
        /// 
        /// </summary>
        private static GpioController gpio;
        /// <summary>
        /// ������
        /// ���� PWM ���ź�Ƶ��
        /// </summary>
        private static PwmChannel pwmRed, pwmGreen, pwmBlue;
        /// <summary>
        /// ���ȷ���
        /// </summary>
        private static int djPin = 33;
        /// <summary>
        /// ��־
        /// </summary>
        private static int logPin = 15;
        /// <summary>
        /// ������
        /// </summary>
        private static byte[] byteBuffer = new byte[256];
        private static int bufferIndex = 0;

        public static void Main()
        {
            gpio = new GpioController();

            // ���� GPIO ���Ź���
            Configuration.SetPinFunction(25, DeviceFunction.PWM1); // R (GPIO25)
            Configuration.SetPinFunction(26, DeviceFunction.PWM2); // G (GPIO26)
            Configuration.SetPinFunction(27, DeviceFunction.PWM3); // B (GPIO27)
            // ��ʼ�� PWM ͨ����Ƶ�� 1000Hz��
            pwmRed = PwmChannel.CreateFromPin(25, 1000, 0);
            pwmGreen = PwmChannel.CreateFromPin(26, 1000, 0);
            pwmBlue = PwmChannel.CreateFromPin(27, 1000, 0);

            gpio.OpenPin(logPin, PinMode.Output);
            gpio.OpenPin(djPin, PinMode.Output);
            //�ߵ�ƽ��־���
            gpio.Write(logPin, PinValue.High);
            //���ȷ���
            gpio.Write(djPin, PinValue.Low);
            State();

            try
            {
                // ���� UART2 �����ţ�GPIO17=TX, GPI16=RX��
                Configuration.SetPinFunction(17, DeviceFunction.COM2_TX);
                Configuration.SetPinFunction(16, DeviceFunction.COM2_RX);
                // ʹ�� COM2����Ӧ UART2��
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


            // ���̱߳�������
            Thread.Sleep(Timeout.Infinite);

        }

        private static void Uart_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                // ��������ȡ
                int bytesToRead = Math.Min(uart.BytesToRead, byteBuffer.Length - bufferIndex);
                if (bytesToRead == 0) return;

                int bytesRead = uart.Read(byteBuffer, bufferIndex, bytesToRead);
                bufferIndex += bytesRead;

                // ��� \r\n ��β
                if (bufferIndex >= 2 &&
                    byteBuffer[bufferIndex - 2] == 0x0D &&
                    byteBuffer[bufferIndex - 1] == 0x0A)
                {
                    string completeMessage = Encoding.UTF8.GetString(byteBuffer, 0, bufferIndex - 2);
                    Debug.WriteLine($"��������֡: {completeMessage}");
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
                Debug.WriteLine("���棺��ȡ��ʱ");
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
        /// ��ɫö��
        /// </summary>
        private enum Color { Red, Orange, Green }
        /// <summary>
        /// ������ɫ
        /// </summary>
        /// <param name="color"></param>
        private static void SetColor(Color color)
        {
            // �ر�����ͨ��
            pwmRed.DutyCycle = 0;
            pwmGreen.DutyCycle = 0;
            pwmBlue.DutyCycle = 0;

            // ����״̬������Ӧ��ɫ
            switch (color)
            {
                case Color.Green:
                    pwmGreen.DutyCycle = 1.0; // ����
                    break;
                case Color.Orange:
                    pwmRed.DutyCycle = 1.0;   // �� + ������ = ��
                    pwmGreen.DutyCycle = 0.2;
                    break;
                case Color.Red:
                    pwmRed.DutyCycle = 1.0;   // ����
                    break;
            }
        }
    }
}
