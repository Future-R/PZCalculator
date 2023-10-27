using System;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

public static class Program
{
    public static void Main()
    {
        var processor = new JsonFileProcessor();
        Console.Title = "Paratranz真实进度统计V1.0.1";
        Console.WriteLine("请拖入要统计的目录（会搜索所有子目录）：");
        string input = Console.ReadLine();
        List<JObject> fileContents = processor.ProcessFiles(input);
        var test = 统计完成度(fileContents);
        Console.WriteLine("统计完成！按任意键退出！");
        Console.ReadLine();
    }

    public static double 统计完成度(List<JObject> objs)
    {
        int 已翻译字数 = 0;
        int 未翻译字数 = 0;
        foreach (var obj in objs)
        {
            // 0是未翻译
            if (obj["stage"].ToString() == "0")
            {
                未翻译字数 += obj["original"].ToString().Length;
            }
            else
            {
                已翻译字数 += obj["original"].ToString().Length;
            }
        }
        int 总计 = 已翻译字数 + 未翻译字数;
        Console.WriteLine();
        Console.WriteLine($"已翻译{已翻译字数}");
        Console.WriteLine($"未翻译{未翻译字数}");
        Console.WriteLine($"总　计{总计}");
        
        double 完成度 = (Convert.ToDouble(已翻译字数) / Convert.ToDouble(总计));
        for (double i = 0.1d; i < 1.02d; i += 0.02d)
        {
            if (完成度 > i)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write("◆");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("◇");
            }
        }
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine();
        Console.WriteLine($"完成度{Math.Round(完成度 * 100d, 2)}%");
        Console.WriteLine();
        return 完成度;
    }
}
public class JsonFileProcessor
{
    public List<JObject> ProcessFiles(string directoryPath)
    {
        // 获取目录中所有JSON文件
        var jsonFiles = Directory.GetFiles(directoryPath, "*.json", SearchOption.AllDirectories);

        // 用于存储JSON数组对象的列表
        List<JObject> jsonObjects = new List<JObject>();

        // 遍历所有JSON文件
        foreach (var jsonFile in jsonFiles)
        {
            // 读取JSON文件内容
            string jsonContent = File.ReadAllText(jsonFile);

            // 将JSON内容解析为JObject数组
            JArray jsonArray = JArray.Parse(jsonContent);

            // 将每个JObject添加到列表中
            jsonObjects.AddRange(jsonArray.ToObject<List<JObject>>());
        }

        return jsonObjects;
    }
}