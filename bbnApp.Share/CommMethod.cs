using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Xml;
using NPinyin;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace bbnApp.Share
{
    public static class OSHelper
    {
        public static bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        public static bool IsLinux => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
    }
    /// <summary>
    /// 常用方法
    /// </summary>
    public class CommMethod
    {
        /// <summary>
        /// 非空值过滤,获取指定类型的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T GetValueOrDefault<T>(object value, T defaultValue)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
                return defaultValue;

            try
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return defaultValue;
            }
        }
        /// <summary>
        ///  把字符串按照指定长度分割
        /// </summary>
        /// <param name="txtString">字符串</param>
        /// <param name="charNumber">长度</param>
        /// <returns></returns>
        public static ArrayList GetSeparateSubString(string txtString, int charNumber)
        {
            ArrayList arrlist = new ArrayList();
            string tempStr = txtString;
            for (int i = 0; i < tempStr.Length; i += charNumber)
            {
                if ((tempStr.Length - i) > charNumber)//如果是，就截取
                {
                    arrlist.Add(tempStr.Substring(i, charNumber));
                }
                else
                {
                    arrlist.Add(tempStr.Substring(i));//如果不是，就截取最后剩下的那部分
                }
            }
            return arrlist;
        }
        /// <summary>
        /// 文档类文件转Base64字符串
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static string FileToBase64(string filePath)
        {
            if (File.Exists(filePath))
            {
                FileStream fsForRead = new FileStream(filePath, FileMode.Open);
                string base64Str = "";
                try
                {
                    //读写指针移到距开头10个字节处
                    fsForRead.Seek(0, SeekOrigin.Begin);
                    byte[] bs = new byte[fsForRead.Length];
                    int log = Convert.ToInt32(fsForRead.Length);
                    //从文件中读取10个字节放到数组bs中
                    fsForRead.Read(bs, 0, log);
                    base64Str = Convert.ToBase64String(bs);
                    return base64Str;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    fsForRead.Close();
                }
            }
            else
            {
                throw new Exception(filePath + "文件不存在");
            }
        }
        /// <summary>
        /// base64转文档类文件
        /// </summary>
        /// <param name="base64String"></param>
        /// <param name="folderPath"></param>
        /// <param name="fileName"></param>
        public static void Base64StringToFile(string base64String, string folderPath, string fileName)
        {
            try
            {
                if (!Directory.Exists(Path.Combine(folderPath)))
                {
                    Directory.CreateDirectory(Path.Combine(folderPath));
                }
                if (File.Exists(Path.Combine(folderPath, fileName)))
                {
                    File.Delete(Path.Combine(folderPath, fileName));
                }
                string strbase64 = base64String.Trim().Substring(base64String.IndexOf(",") + 1);   //将‘，’以前的多余字符串删除
                using (MemoryStream stream = new MemoryStream(Convert.FromBase64String(strbase64)))
                {
                    FileStream fs = new FileStream(Path.Combine(folderPath, fileName), FileMode.OpenOrCreate, FileAccess.Write);
                    byte[] b = stream.ToArray();
                    fs.Write(b, 0, b.Length);
                    fs.Close();
                    fs.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 图片文件转Base64字符串
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string ImageToBase64(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    byte[] imageBytes = File.ReadAllBytes(filePath);

                    // 获取图片格式用于 MIME 类型
                    string mimeType = GetMimeType(filePath);

                    // 转换为 Base64 字符串
                    string base64String = Convert.ToBase64String(imageBytes);
                    return base64String;
                }
                else
                {
                    throw new Exception(filePath + "文件不存在");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 图片base64字符串转byte[]
        /// </summary>
        /// <param name="base64info"></param>
        /// <returns></returns>
        public static byte[] Base64ToImageBytes(string base64info)
        {
            byte[] imageBytes = Convert.FromBase64String(base64info);
            return imageBytes;
        }
        /// <summary>
        /// 获取图片类型
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        private static string GetMimeType(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLower();
            switch (extension)
            {
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".png":
                    return "image/png";
                case ".bmp":
                    return "image/bmp";
                case ".gif":
                    return "image/gif";
                default:
                    throw new NotSupportedException("File extension is not supported.");
            }
        }
        /// <summary>
        /// json字符串转解析为XML
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static string ConvertJsonToXml(string jsonString)
        {
            try
            {
                // 将 JSON 字符串转换为 Json 对象
                string info = jsonString;
                if (jsonString.StartsWith("["))
                {
                    info = "{ \"Items\": " + jsonString + " }";
                }
                var jsonObject = JsonConvert.DeserializeObject<JToken>(info);

                // 将 Json 对象转换为 XML
                XmlDocument xmlDoc = JsonConvert.DeserializeXmlNode(jsonObject.ToString(), "Root");

                // 返回格式化的 XML 字符串
                return BeautifyXml(xmlDoc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// xml处理
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        private static string BeautifyXml(XmlDocument doc)
        {
            using (var stringWriter = new System.IO.StringWriter())
            using (var xmlTextWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings { Indent = true }))
            {
                doc.WriteTo(xmlTextWriter);
                xmlTextWriter.Flush();
                return stringWriter.GetStringBuilder().ToString();
            }
        }
        /// <summary>
        /// xml字符串转JSON
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static string ConvertXmlToJson(string xml)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);
                string jsonText = JsonConvert.SerializeXmlNode(doc);
                JObject jsonObject = JObject.Parse(jsonText);
                return jsonObject.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 生成随机数字符串
        /// </summary>
        /// <param name="num">随机整数数区间</param>
        /// <returns></returns>
        public static string GetRandom(int num)
        {
            Random rd = new Random();
            int i = rd.Next(0, num);//生成0到num的整型随机数
            string result = i.ToString();
            return result;
        }
        /// <summary>
        /// 生成随机数字
        /// </summary>
        /// <param name="num">随机整数数区间</param>
        /// <returns></returns>
        public static int GetIntRandom(int num)
        {
            Random rd = new Random();
            int i = rd.Next(0, num);//生成0到num的整型随机数
            return i;
        }
        /// <summary>
        /// 生成随机字符串
        /// </summary>
        /// <returns></returns>
        public static string GetStrRandom(int num)
        {
            string result = string.Empty;
            Random rd = new Random();
            char[] constant =
              {
                '0','1','2','3','4','5','6','7','8','9',
                'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z',
                'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z'
              };

            for (int i = 0; i < num; i++)
            {
                result += constant[rd.Next(62)];
            }
            return result;
        }
        /// <summary>
        /// 日期转换为时间戳
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static Int64 DateTimeToInt(DateTime datetime)
        {
            return (datetime.ToUniversalTime().Ticks - new DateTime(1970, 1, 1).Ticks) / 10000000;
        }
        /// <summary>
        /// 日期是星期几,返回字符 星期x
        /// </summary>
        /// <param name="dt">制定日期</param>
        /// <returns></returns>
        public static string DayOfWeekString(DateTime dt)
        {
            var weekdays = new string[] { "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" };
            return weekdays[(int)dt.DayOfWeek];
        }
        /// <summary>
        /// 日期是星期几,返回数字
        /// </summary>
        /// <param name="rq">制定日期</param>
        /// <returns></returns>
        public static int DayOfWeekInt(DateTime rq)
        {
            string[] Day = new string[] { "0", "1", "2", "3", "4", "5", "6" };
            int week = Convert.ToInt32(Day[Convert.ToInt32(rq.DayOfWeek.ToString("d"))].ToString());
            return week;
        }
        /// <summary>
        /// 获取汉字字母（可包含多个汉字）
        /// </summary>
        /// <param name="strText"></param>
        /// <param name="qp">是否全拼</param>
        /// <returns></returns>
        public static string GetChineseSpell(string chineseText, bool qp)
        {
            //Encoding gb2312 = Encoding.GetEncoding("GB2312");
            //string strQuanpin = Pinyin.ConvertEncoding(chineseText, Encoding.UTF8, gb2312);
            return qp ? Pinyin.GetPinyin(chineseText) : GetPinyinAbbreviation(chineseText);
        }
        /// <summary>
        /// 简拼
        /// </summary>
        /// <param name="chineseText"></param>
        /// <returns></returns>
        private static string GetPinyinAbbreviation(string chineseText)
        {
            string abbreviation = string.Empty;
            foreach (char ch in chineseText)
            {
                string pinyin = Pinyin.GetPinyin(ch);
                if (!string.IsNullOrEmpty(pinyin))
                {
                    abbreviation += pinyin[0]; // 取每个拼音的首字母
                }
            }
            return abbreviation.ToUpper(); // 转为大写
        }
        /// <summary>
        /// 获取公网IP信息
        /// </summary>
        /// <returns></returns>
        public async static Task<IPData> GetIpInfo()
        {
            IPData IpData = new IPData();
            try
            {
                IpData.localip = GetLocalIPAddress();
                string url = "https://qifu.baidu.com/ip/local/geo/v1/district";
                HttpClient client = new HttpClient();
                // 发送 GET 请求
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode(); // 如果请求不成功则抛出异常
                // 读取响应内容
                string responseData = await response.Content.ReadAsStringAsync();
                JObject ObjIpData = JObject.Parse(responseData);
                if (ObjIpData["code"].ToString() == "Success")
                {
                    JObject ObjData = JObject.Parse(ObjIpData["data"].ToString());
                    string ip = ObjIpData["ip"].ToString();
                    IpData.continent = ObjData["continent"].ToString();
                    IpData.country = ObjData["country"].ToString();
                    IpData.zipcode = ObjData["zipcode"].ToString();
                    IpData.owner = ObjData["owner"].ToString();
                    IpData.isp = ObjData["isp"].ToString();
                    IpData.adcode = ObjData["adcode"].ToString();
                    IpData.prov = ObjData["prov"].ToString();
                    IpData.city = ObjData["city"].ToString();
                    IpData.district = ObjData["district"].ToString();
                }
                else
                {
                    //throw new Exception("IP信息获取异常");
                }
            }
            catch (Exception ex)
            {
                //throw ex;
            }
            return IpData;
        }
        /// <summary>
        /// 获取内网IP
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static string GetLocalIPAddress()
        {
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.OperationalStatus == OperationalStatus.Up &&
                    (ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
                     ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211))
                {
                    foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            return ip.Address.ToString();
                        }
                    }
                }
            }
            return "";
        }
        /// <summary>
        /// 获取指定路径下的文件名称
        /// </summary>
        /// <param name="path"></param>
        /// <param name="Extensions">{ "*.jpg", "*.jpeg", "*.png", "*.bmp", "*.gif", "*.tiff" }</param>
        /// <returns></returns>
        public static string[] GetFilesName(string path, string[] Extensions)
        {
            // 定义要搜索的文件扩展名
            string[] FileExtensions = Extensions;

            // 创建一个列表来存储找到的文件
            var Files = new List<string>();

            // 迭代每种扩展名，并获取匹配的文件
            foreach (string extension in FileExtensions)
            {
                Files.AddRange(Directory.GetFiles(path, extension, SearchOption.TopDirectoryOnly));
            }

            return Files.ToArray();
        }
        /// <summary>
        /// 将 DataTable 转换为 IEnumerable<object[]>
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public static IEnumerable<object[]> ConvertDataTableToIEnumerable(DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                // 将 DataRow 转换为 object 数组
                object[] rowArray = row.ItemArray;
                yield return rowArray;
            }
        }
        /// <summary>
        /// 打开文件夹
        /// </summary>
        /// <param name="directoryPath"></param>
        public static void OpenDirectory(string directoryPath)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = directoryPath,
                    UseShellExecute = true,
                    Verb = "open"
                });
            }
            catch (Exception ex)
            {
                throw new Exception("无法打开目录: " + ex.Message);
            }
        }
        /// <summary>
        /// 打开指定文件夹-跨平台
        /// </summary>
        /// <param name="folderPath"></param>
        public static void OpenFolder(string folderPath)
        {
            try
            {
                // 确保路径格式正确
                folderPath = Path.GetFullPath(folderPath);

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    // Windows
                    Process.Start("explorer.exe", folderPath);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    // macOS
                    Process.Start("open", folderPath);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    // Linux
                    Process.Start("xdg-open", folderPath);
                }
                else
                {
                    Console.WriteLine("Unsupported platform!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to open folder: " + ex.Message);
            }
        }
        /// <summary>
        /// 密码正则表达式校验
        /// 8位数，需要有大小写字母，数字，特殊字符
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsPasswordValid(string input)
        {
            // 正则表达式
            string pattern = @"^(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9])(?=.*[!@#$%^&*()_+=\[{\]};:<>|./?,-]).{8,}$";

            // 使用 Regex.IsMatch 进行验证
            return Regex.IsMatch(input, pattern);
        }
        
        /// <summary>
        /// 应用重启,获取当前进程的可执行文件路径
        /// </summary>
        /// <param name="proccessName"></param>
        public static void applicationRestart()
        {
            try
            {
                // 1. 获取当前进程的可执行文件路径
                string executablePath = Environment.ProcessPath!;
                string processName = Process.GetCurrentProcess().ProcessName;

                // 2. 判断是否以 `dotnet` 方式运行（适用于 .NET 控制台应用）
                bool isDotNetProcess = executablePath.EndsWith("dotnet") || executablePath.EndsWith("dotnet.exe");

                // 3. 构造启动命令
                string command = null;
                string args = null;

                if (isDotNetProcess)
                {
                    // 如果是 `dotnet` 启动的，则使用 `dotnet YourApp.dll` 方式
                    command = "dotnet";
                    args = $"{processName}.dll";
                }
                else
                {
                    // 否则直接启动可执行文件
                    command = executablePath;
                    args = "";
                }

                // 4. 根据操作系统选择不同的 Shell 执行方式
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    // Windows: 使用 cmd 延迟 1 秒启动，避免端口占用等问题
                    Process.Start("cmd.exe", $"/c timeout 1 & start \"\" \"{command}\" {args}");
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    // Linux: 使用 bash 延迟 1 秒启动
                    Process.Start("bash", $"-c \"sleep 1 && {command} {args}\"");
                }
                else
                {
                    throw new PlatformNotSupportedException("Unsupported OS");
                }

                // 5. 退出当前进程
                Environment.Exit(0);
            }
            catch (Exception ex)
            {

            }
        }
        #region TAS 传感器通用解析
        /// <summary>
        /// 塔石传感器通用解析(自动识别温湿度或光照)
        /// </summary>
        /// <returns>
        /// 对于温湿度：(状态，消息, 地址，温度，湿度)
        /// 对于光照：(状态，消息, 地址，光照)
        /// </returns>
        public static (bool, string, byte, double, double) ParseTASSensor(byte[] modbusData, bool bcrc = false)
        {
            try
            {
                // 基本校验
                if (modbusData == null || modbusData.Length < 5)
                {
                    return (false, "数据长度不足",byte.MinValue, double.MinValue, double.MinValue);
                }

                if (bcrc && !VerifyModbusCrc(modbusData))
                {
                    return (false, "CRC校验失败",  byte.MinValue, double.MinValue, double.MinValue);
                }

                // 1. 解析基础信息
                byte address = modbusData[0];          // 地址码
                byte functionCode = modbusData[1];     // 功能码
                byte dataLength = modbusData[2];       // 数据字节数

                // 2. 根据数据长度自动识别传感器类型
                if (dataLength == 4) // 温湿度传感器(2个16位寄存器)
                {
                    if (modbusData.Length < 9) // 7数据字节 + 2CRC
                    {
                        return (false, "温湿度数据长度不足", byte.MinValue, double.MinValue, double.MinValue);
                    }

                    // 解析温湿度数据（大端序）
                    ushort humidityRaw = (ushort)((modbusData[3] << 8) | modbusData[4]);
                    ushort temperatureRaw = (ushort)((modbusData[5] << 8) | modbusData[6]);

                    double humidity = humidityRaw / 10.0;
                    double temperature = temperatureRaw / 10.0;

                    return (true, "温湿度数据解析成功", address, temperature, humidity);
                }
                else if (dataLength == 2) // 单独数据采集
                {
                    if (modbusData.Length < 7) // 5数据字节 + 2CRC
                    {
                        return (false, "数据长度不足",  byte.MinValue, double.MinValue, double.MinValue);
                    }

                    // 解析数据（大端序）
                    ushort lightValue = (ushort)((modbusData[3] << 8) | modbusData[4]);
                    double actualLightValue = lightValue;

                    // 返回时湿度位置填double.MinValue表示无效
                    return (true, "数据解析成功", address, actualLightValue, double.MinValue);
                }
                else
                {
                    return (false, $"未知传感器类型(数据长度:{dataLength})",  byte.MinValue, double.MinValue, double.MinValue);
                }
            }
            catch (Exception ex)
            {
                return (false, $"数据解析失败：{ex.Message}",  byte.MinValue, double.MinValue, double.MinValue);
            }
        }
        #endregion
        #region CRC计算
        /// <summary>
        /// 验证Modbus CRC
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool VerifyModbusCrc(byte[] data)
        {
            if (data.Length < 2)
                return false;

            // 计算除最后两个字节(CRC)外的所有数据的CRC
            ushort calculatedCrc = CalculateModbusCrc(data, 0, data.Length - 2);

            // 获取帧中的CRC(小端序)
            ushort receivedCrc = (ushort)((data[data.Length - 1] << 8) | data[data.Length - 2]);

            return calculatedCrc == receivedCrc;
        }
        /// <summary>
        /// Modbus CRC16计算
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static ushort CalculateModbusCrc(byte[] data, int offset, int length)
        {
            ushort crc = 0xFFFF;

            for (int i = offset; i < offset + length; i++)
            {
                crc ^= data[i];
                for (int j = 0; j < 8; j++)
                {
                    bool lsb = (crc & 0x0001) != 0;
                    crc >>= 1;
                    if (lsb)
                        crc ^= 0xA001;
                }
            }

            return crc;
        }
        #endregion
    }
}
