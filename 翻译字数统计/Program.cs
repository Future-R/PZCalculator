using System;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Text;
using System.Linq;

public static class Program
{
    public static List<JObject> 已翻译词条 = new List<JObject>();
    public static List<JObject> 未翻译词条 = new List<JObject>();
    public static List<string> 漏网之鱼 = new List<string>();
    public static void Main()
    {
        Console.OutputEncoding = Encoding.Unicode;
        var processor = new JsonFileProcessor();
        Console.Title = "Paratranz真实进度统计V1.1.0";
        Console.WriteLine("请拖入要统计的目录（会搜索所有子目录）：");
        string input = Console.ReadLine();
        processor.ProcessFiles(input);
        统计完成度();
        Console.WriteLine("统计完成！按任意键枚举漏网之鱼！");
        Console.ReadLine();
        枚举漏网之鱼();
        Console.ReadLine();
    }

    public static void 枚举漏网之鱼()
    {
        Console.WriteLine($"未翻译词条{未翻译词条.Count}条");
        List<string> 未翻译原文 = 未翻译词条.Select(x => x["original"].ToString().Trim()).Distinct().ToList();
        Console.WriteLine($"已翻译词条{已翻译词条.Count}条");
        List<string> 已翻译原文 = 已翻译词条.Select(x => x["original"].ToString().Trim()).Distinct().ToList();
        double 进度格 = (double)未翻译词条.Count / 100d;
        double 临时进度格 = 进度格;
        string 鱼 = "";
        List<string> 鱼群 = new List<string>();
        for (int i = 0; i < 未翻译原文.Count; i++)
        {
            鱼 = 已翻译原文.Find(x => x == 未翻译原文[i]);
            if (!String.IsNullOrEmpty(鱼))
            {
                鱼群.Add(鱼);
                鱼 = null;
            }
            if (i > 临时进度格)
            {
                Console.Write("♥");
                临时进度格 += 进度格;
            }
        }
        foreach (var item in 鱼群)
        {
            Console.WriteLine(item);
            Console.WriteLine("按任意键继续");
            Console.ReadKey();
        }
    }

    public static double 统计完成度()
    {
        int 已翻译字数 = 0;
        int 未翻译字数 = 0;
        已翻译词条.ForEach(词条 => 已翻译字数 += 词条["original"].ToString().Length);
        未翻译词条.ForEach(词条 => 未翻译字数 += 词条["original"].ToString().Length);

        int 总计 = 已翻译字数 + 未翻译字数;
        Console.WriteLine();
        Console.WriteLine($"已翻译{已翻译字数}");
        Console.WriteLine($"未翻译{未翻译字数}");
        Console.WriteLine($"总　计{总计}");

        double 完成度 = (Convert.ToDouble(已翻译字数) / Convert.ToDouble(总计));
        for (double i = 0.02d; i < 1.02d; i += 0.02d)
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
    public void ProcessFiles(string directoryPath)
    {
        // 获取目录中所有JSON文件
        var jsonFiles = Directory.GetFiles(directoryPath, "*.json", SearchOption.AllDirectories);

        // 遍历所有JSON文件
        foreach (var jsonFile in jsonFiles)
        {
            string jsonContent = File.ReadAllText(jsonFile);
            JArray jsonArray = JArray.Parse(jsonContent);

            // 将每个JObject添加到列表中
            foreach (var jobj in jsonArray.ToObject<List<JObject>>())
            {
                switch (jobj["stage"].ToString())
                {
                    // 0是未翻译
                    case "0":
                        Program.未翻译词条.Add(jobj);
                        break;
                    default:
                        Program.已翻译词条.Add(jobj);
                        break;
                }
            }
        }
    }
}