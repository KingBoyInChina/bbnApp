
using System;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using NModbus;
using NModbus.Device;
using NModbus.IO;
using NModbus.Serial;
using NModbus.Utility;

namespace bbnApp.Share
{
    /// <summary>
    /// Modbus通用帮助类（NModbus 3.0.81）
    /// 支持：RTU/ASCII/TCP/UDP
    /// </summary>
    public class ModbusHelper : IDisposable
    {
        private IModbusMaster _master;
        private SerialPort _serialPort;
        private TcpClient _tcpClient;
        private UdpClient _udpClient;
        private readonly ModbusTransportType _transportType;
        private readonly object _lock = new object();

        public enum ModbusTransportType
        {
            SerialRtu,
            SerialAscii,
            Tcp,
            Udp
        }

        #region 构造函数（多模式初始化）
        /// <summary>
        /// 串口模式（RTU/ASCII）
        /// </summary>
        public ModbusHelper(
            string portName,
            ModbusTransportType type = ModbusTransportType.SerialRtu,
            int baudRate = 9600,
            Parity parity = Parity.None,
            int dataBits = 8,
            StopBits stopBits = StopBits.One,
            int timeout = 1000)
        {
            _transportType = type;
            _serialPort = new SerialPort(portName, baudRate, parity, dataBits, stopBits)
            {
                ReadTimeout = timeout,
                WriteTimeout = timeout
            };
            _serialPort.Open();


            // 创建串口传输对象（直接使用Modbus.Serial.SerialPortAdapter）
            var transport = new SerialPortAdapter(_serialPort);
            var factory = new ModbusFactory();

            // 明确指定传输协议（RTU或ASCII）
            if (type == ModbusTransportType.SerialRtu)
            {
                _master = factory.CreateRtuMaster(transport);
            }
            else
            {
                _master = factory.CreateAsciiMaster(transport);
            }
        }

        /// <summary>
        /// TCP模式
        /// </summary>
        public ModbusHelper(string ip, int port = 502, int timeout = 1000)
        {
            _transportType = ModbusTransportType.Tcp;
            _tcpClient = new TcpClient();
            _tcpClient.Connect(ip, port);
            _tcpClient.ReceiveTimeout = timeout;

            var factory = new ModbusFactory();
            _master = factory.CreateMaster(_tcpClient);
        }

        /// <summary>
        /// UDP模式
        /// </summary>
        public ModbusHelper(string ip, int localPort, int remotePort, int timeout = 1000)
        {
            _transportType = ModbusTransportType.Udp;
            _udpClient = new UdpClient(localPort);
            _udpClient.Connect(ip, remotePort);
            _udpClient.Client.ReceiveTimeout = timeout;

            var factory = new ModbusFactory();
            _master = factory.CreateMaster(_udpClient);
        }
        #endregion

        #region 同步读写方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="slaveId"></param>
        /// <param name="startAddress"></param>
        /// <param name="numRegisters"></param>
        /// <returns></returns>
        /// <exception cref="ModbusException"></exception>
        public ushort[] ReadHoldingRegisters(byte slaveId, ushort startAddress, ushort numRegisters)
        {
            lock (_lock)
            {
                try
                {
                    return _master.ReadHoldingRegisters(slaveId, startAddress, numRegisters);
                }
                catch (Exception ex)
                {
                    throw new ModbusException($"读取保持寄存器失败 (ID:{slaveId}, Addr:{startAddress})", ex);
                }
            }
        }
        /// <summary>
        /// 同步读写
        /// </summary>
        /// <param name="slaveId"></param>
        /// <param name="registerAddress"></param>
        /// <param name="value"></param>
        /// <exception cref="ModbusException"></exception>
        public void WriteSingleRegister(byte slaveId, ushort registerAddress, ushort value)
        {
            lock (_lock)
            {
                try
                {
                    _master.WriteSingleRegister(slaveId, registerAddress, value);
                }
                catch (Exception ex)
                {
                    throw new ModbusException($"写入寄存器失败 (ID:{slaveId}, Addr:{registerAddress})", ex);
                }
            }
        }
        #endregion

        #region 异步读写方法

        /// <summary>
        /// 生成带CRC校验的Modbus RTU命令字符串
        /// </summary>
        /// <param name="slaveAddress">从站地址(1-247)</param>
        /// <param name="functionCode">功能码(如0x03读保持寄存器)</param>
        /// <param name="startAddress">寄存器起始地址</param>
        /// <param name="numberOfRegisters">寄存器数量</param>
        /// <returns>十六进制格式的命令字符串(空格分隔)</returns>
        public static string GenerateModbusCommand(byte slaveAddress, byte functionCode, ushort startAddress, ushort numberOfRegisters)
        {
            // 创建基本命令(不包含CRC)
            byte[] command = new byte[6];
            command[0] = slaveAddress;                  // 从站地址
            command[1] = functionCode;                  // 功能码
            command[2] = (byte)(startAddress >> 8);     // 起始地址高字节
            command[3] = (byte)(startAddress & 0xFF);   // 起始地址低字节
            command[4] = (byte)(numberOfRegisters >> 8);    // 寄存器数量高字节
            command[5] = (byte)(numberOfRegisters & 0xFF);  // 寄存器数量低字节

            // 计算CRC校验
            byte[] crc = ModbusUtility.CalculateCrc(command);

            // 合并命令和CRC
            byte[] fullCommand = command.Concat(crc).ToArray();

            // 转换为十六进制字符串(大写，空格分隔)
            return BitConverter.ToString(fullCommand).Replace("-", " ");
        }
        /// <summary>
        /// 异步读写
        /// </summary>
        /// <param name="slaveId"></param>
        /// <param name="startAddress"></param>
        /// <param name="numRegisters"></param>
        /// <returns></returns>
        /// <exception cref="ModbusException"></exception>
        public async Task<ushort[]> ReadHoldingRegistersAsync(byte slaveId, ushort startAddress, ushort numRegisters)
        {
            try
            {
                return await Task.Run(() => _master.ReadHoldingRegisters(slaveId, startAddress, numRegisters));
            }
            catch (Exception ex)
            {
                throw new ModbusException($"异步读取保持寄存器失败 (ID:{slaveId}, Addr:{startAddress})", ex);
            }
        }

        public async Task WriteSingleRegisterAsync(byte slaveId, ushort registerAddress, ushort value)
        {
            try
            {
                await Task.Run(() => _master.WriteSingleRegister(slaveId, registerAddress, value));
            }
            catch (Exception ex)
            {
                throw new ModbusException($"异步写入寄存器失败 (ID:{slaveId}, Addr:{registerAddress})", ex);
            }
        }
        #endregion

        #region 资源释放
        private bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _master?.Dispose();
                    _serialPort?.Dispose();
                    _tcpClient?.Dispose();
                    _udpClient?.Dispose();
                }
                _disposed = true;
            }
        }
        #endregion
    }

    public class ModbusException : Exception
    {
        public ModbusException(string message, Exception inner) : base(message, inner) { }
    }
}
