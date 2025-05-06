// See https://aka.ms/new-console-template for more information
using bbnApp;
using System.ComponentModel;

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
    Console.WriteLine("10. 生成结构体 10");
    Console.WriteLine("99. 退出");

    // 获取用户选择
    Console.Write("请输入选项编号：");
    string choice = Console.ReadLine();
    string fileinfo = string.Empty;
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
            Console.Write("请输入数据表名称,例如bbn.area：");
            
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


