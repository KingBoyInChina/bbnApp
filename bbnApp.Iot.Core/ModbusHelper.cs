using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using nanoFramework.Hardware.Esp32;
using nanoFramework.Iot.Device.Modbus;

/// <summary>
/// Modbus双协议帮助类（支持RTU/TCP）
/// </summary>
public class ModbusHelper : IDisposable
{
    private ModbusDevice _modbusDevice;
    private readonly ModbusType _modbusType;
    private readonly object _lock = new object();

    /// <summary>
    /// Modbus协议类型
    /// </summary>
    public enum ModbusType
    {
        Rtu,
        Tcp
    }

    /// <summary>
    /// 初始化Modbus帮助类
    /// </summary>
    /// <param name="type">协议类型</param>
    /// <param name="rtuPort">串口名（仅RTU需要）</param>
    /// <param name="tcpIp">IP地址（仅TCP需要）</param>
    /// <param name="tcpPort">TCP端口（默认502）</param>
    /// <param name="dirPin">方向控制引脚（RTU必需）</param>
    public ModbusHelper(
        ModbusType type,
        string rtuPort = "COM1",
        string tcpIp = "192.168.1.100",
        int tcpPort = 502,
        int dirPin = -1)
    {
        _modbusType = type;

        lock (_lock)
        {
            switch (type)
            {
                case ModbusType.Rtu:
                    var dirGpio = dirPin >= 0 ? new GpioController().OpenPin(dirPin, PinMode.Output) : null;
                    _modbusDevice = new ModbusRtuDevice(
                        portName: rtuPort,
                        baudRate: 9600,
                        parity: Parity.None,
                        dataBits: 8,
                        stopBits: StopBits.One,
                        driverEnablePin: dirGpio);
                    break;

                case ModbusType.Tcp:
                    var ipEndpoint = new IPEndPoint(IPAddress.Parse(tcpIp), tcpPort);
                    _modbusDevice = new ModbusTcpDevice(ipEndpoint);
                    break;

                default:
                    throw new ArgumentException("Unsupported Modbus type");
            }
        }
    }

    /// <summary>
    /// 读取保持寄存器
    /// </summary>
    public ushort[] ReadHoldingRegisters(byte slaveAddress, ushort startAddress, ushort count)
    {
        lock (_lock)
        {
            try
            {
                return _modbusDevice.ReadHoldingRegisters(slaveAddress, startAddress, count);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Modbus {_modbusType}] 读取失败: {ex.Message}");
                throw new ModbusException($"Read failed: {ex.Message}", ex);
            }
        }
    }

    /// <summary>
    /// 写入单个寄存器
    /// </summary>
    public void WriteSingleRegister(byte slaveAddress, ushort address, ushort value)
    {
        lock (_lock)
        {
            try
            {
                _modbusDevice.WriteSingleRegister(slaveAddress, address, value);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Modbus {_modbusType}] 写入失败: {ex.Message}");
                throw new ModbusException($"Write failed: {ex.Message}", ex);
            }
        }
    }

    /// <summary>
    /// 读取线圈状态
    /// </summary>
    public bool[] ReadCoils(byte slaveAddress, ushort startAddress, ushort count)
    {
        lock (_lock)
        {
            try
            {
                return _modbusDevice.ReadCoils(slaveAddress, startAddress, count);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Modbus {_modbusType}] 读取线圈失败: {ex.Message}");
                throw new ModbusException($"Read coils failed: {ex.Message}", ex);
            }
        }
    }

    public void Dispose()
    {
        _modbusDevice?.Dispose();
    }
}

/// <summary>
/// Modbus自定义异常
/// </summary>
public class ModbusException : Exception
{
    public ModbusException(string message, Exception innerException)
        : base(message, innerException) { }
}
