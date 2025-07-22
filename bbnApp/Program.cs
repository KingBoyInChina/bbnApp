// See https://aka.ms/new-console-template for more information
using bbnApp;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Timers;
using static bbnApp.Share.ModbusHelper;


string fileinfo = string.Empty;
while (true)
{
    // 显示菜单
    Console.WriteLine("请选择一个选项：");
    Console.WriteLine("1. 加载iconfot.json生成资源文件内容 1");
    Console.WriteLine("2. SM2解密 2");
    Console.WriteLine("3. SM2加密 3");
    Console.WriteLine("4. AES加密 4");
    Console.WriteLine("5. AES解密 5");
    Console.WriteLine("6. RSA解密 6");
    Console.WriteLine("7. RSA加密 7");
    Console.WriteLine("8. 混合解密 8");
    Console.WriteLine("9. 混合加密 9");
    Console.WriteLine("10. 读取光照强度[TAS-WSGZ]");
    Console.WriteLine("11. 读取温度[TAS-WSGZ]");
    Console.WriteLine("12. 读取湿度[TAS-WSGZ]");
    Console.WriteLine("13. 读取温湿度光照[TAS-WSGZ]");
    Console.WriteLine("99. 退出");

    // 获取用户选择
    Console.Write("请输入选项编号：");
    string choice = Console.ReadLine();
    // 处理用户选择
    switch (choice)
    {
        case "1":
            ResourcesParse.ResourcesJsonParse(Path.Combine("json", "iconfont.json"));
            break;
        case "2":
            Console.Write("请输入SM2解密内容：");
            fileinfo = Console.ReadLine();
            KeyAction.Sm2Decrypt(fileinfo);
            break;
        case "3":
            Console.Write("请输入SM2加密内容：");
            fileinfo = Console.ReadLine();
            KeyAction.Sm2Encrypt(fileinfo);
            break;

        case "4":
            Console.Write("请输入AES加密内容：");
            fileinfo = Console.ReadLine();
            KeyAction.AESEncrypt(fileinfo);
            break;
        case "5":
            Console.Write("请输入AES解密内容：");
            fileinfo = Console.ReadLine();
            KeyAction.AESDecrypt(fileinfo);
            break;

        case "6":
            Console.Write("请输入RSA解密内容：");
            fileinfo = Console.ReadLine();
            KeyAction.RSADecrypt(fileinfo);
            break;
        case "7":
            Console.Write("请输入RSA加密内容：");
            fileinfo = Console.ReadLine();
            KeyAction.RSAEncrypt(fileinfo);
            break;
        case "8":
            Console.Write("请输入混合解密内容：");
            fileinfo = Console.ReadLine();
            KeyAction.MixDecrypt(fileinfo);
            break;
        case "9":
            Console.Write("请输入混合加密内容：");
            fileinfo = Console.ReadLine();
            KeyAction.MixEncrypt(fileinfo);
            break;
        case "10":
            Console.Write("请输入设备地址：");
            fileinfo = Console.ReadLine();
            using (var modbus = new bbnApp.Share.ModbusHelper(fileinfo, ModbusTransportType.SerialRtu))
            {
                ushort[] values = modbus.ReadHoldingRegisters(slaveId: 1, startAddress: 7, numRegisters: 1);
                Console.WriteLine($"光照强度: {Convert.ToDecimal(values[0]*0.1)} lux");
                Console.ReadKey();
            }
            return;
        case "11":
            Console.Write("请输入设备地址：");
            fileinfo = Console.ReadLine();
            using (var modbus = new bbnApp.Share.ModbusHelper(fileinfo, ModbusTransportType.SerialRtu))
            {
                ushort[] values = modbus.ReadHoldingRegisters(slaveId: 1, startAddress: 1, numRegisters: 1);
                Console.WriteLine($"温度: {Convert.ToDecimal(values[0] * 0.1)} ℃");
                Console.ReadKey();
            }
            return;
        case "12":
            Console.Write("请输入设备地址：");
            fileinfo = Console.ReadLine();
            using (var modbus = new bbnApp.Share.ModbusHelper(fileinfo, ModbusTransportType.SerialRtu))
            {
                ushort[] values = modbus.ReadHoldingRegisters(slaveId: 1, startAddress: 0, numRegisters: 1);
                Console.WriteLine($"湿度: {Convert.ToDecimal(values[0] * 0.1)} %");
                Console.ReadKey();
            }
            //TCP模式
            //using (var modbus = new ModbusUniversalHelper("192.168.1.100"))
            //{
            //    var result = await modbus.ReadHoldingRegistersAsync(1, 0, 1); // 异步读取
            //    Console.WriteLine($"TCP异步读取: {result[0]}");
            //}

            return;
        case "13":
            Console.Write("请输入设备地址：");
            fileinfo = Console.ReadLine();
            System.Timers.Timer _timer = new System.Timers.Timer();
            _timer.Interval = 60000; // 设置定时器间隔为60秒
            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();
            Console.ReadKey();
            return;
        case "99":
            // 退出程序
            Console.WriteLine("程序已退出。");
            return;

        default:
            // 处理无效输入
            Console.WriteLine("无效的选项，请重新输入。");
            break;
    }

    Console.WriteLine(); // 打印空行分隔每次循环
}

void _timer_Elapsed(object? sender, ElapsedEventArgs e)
{
    using (var modbus = new bbnApp.Share.ModbusHelper(fileinfo, ModbusTransportType.SerialRtu))
    {
        StringBuilder value = new StringBuilder();
        ushort[] lightvalues = modbus.ReadHoldingRegisters(slaveId: 1, startAddress: 7, numRegisters: 1);
        value.Append($"光照强度：{Convert.ToDecimal(lightvalues[0] * 0.1)} lunx，");

        ushort[] wsvalues = modbus.ReadHoldingRegisters(slaveId: 1, startAddress: 0, numRegisters: 2);
        value.Append($"湿度：{Convert.ToDecimal(wsvalues[0] * 0.1)} %，温度：{Convert.ToDecimal(wsvalues[1] * 0.1)}℃");

        Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")}：{value.ToString()}");
    }
}