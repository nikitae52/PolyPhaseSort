using System;
using System.IO;
using static System.Net.WebRequestMethods;
class Program
{
    static void Main(string[] args)
    {
        string fileName;
        string tempfile01 = "tempfile01.txt"; // Specify file name or path
        string tempfile02 = "tempfile02.txt"; // Specify file name or path
        string tempfile03 = "tempfile03.txt"; // Specify file name or path

        Console.WriteLine("Enter file name:");
        fileName = Console.ReadLine();
        SplitFile(fileName, tempfile01, tempfile02);
        Console.WriteLine("Масив розбито на файли");
        MergeFiles(tempfile01, tempfile02, tempfile03);
        Console.WriteLine("Відсортовано");



        static void SplitFile(string inputFile, string file1, string file2)
    {
            using (StreamReader reader = new StreamReader(inputFile))
            using (StreamWriter writer1 = new StreamWriter(file1))
            using (StreamWriter writer2 = new StreamWriter(file2))
            {
                int fibprev = 0; int fibcurr = 1;
                bool writeToFirst = true;
                string line;
                int count = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    if (!int.TryParse(line, out int number)) continue; // Пропускаем некорректные строки
                    if (writeToFirst)
                    {
                        writer1.WriteLine(number);
                    }
                    else
                    {
                        writer2.WriteLine(number);
                    }

                    count++;
                    if (count == fibcurr)
                    {
                        count = 0;
                        int temp = fibcurr;
                        fibcurr = fibprev + fibcurr;
                        fibprev = temp;
                        writeToFirst = !writeToFirst;
                    }
                }
            }
    }
        
        static void MergeFiles(string inputFile1, string inputFile2, string OutputFile)
        {
            string EMPTY;
            int f1Count = 0;
            int f2Count = 0;
            string tempFile1 = "tempfile1.txt";
            string tempFile2 = "tempfile2.txt";
            using (StreamReader reader1 = new StreamReader(inputFile1))
            using (StreamReader reader2 = new StreamReader(inputFile2))
            using (StreamWriter writer = new StreamWriter(OutputFile))
            {
                string line1 = reader1.ReadLine();
                string line2 = reader2.ReadLine();

                while (line1 != null && line2 != null)
                {
                    List<int> mergedArray = new List<int>();
                    mergedArray.AddRange(Array.ConvertAll(line1.Split(), int.Parse));
                    f1Count++;
                    mergedArray.AddRange(Array.ConvertAll(line2.Split(), int.Parse));
                    f2Count++;
                    mergedArray.Sort();
                    writer.WriteLine(string.Join(" ", mergedArray));

                    line1 = reader1.ReadLine();
                    line2 = reader2.ReadLine();
                }
                
                if (line1 == null) 
                {   
                    EMPTY = inputFile1;
                                                           
                }
                else if (line2 == null) { EMPTY = inputFile2; }
                else  { EMPTY = "both"; }

            }
                string NEWOUTPUT;
                string NEWINPUT;
                if (EMPTY== "both")
                {
                    System.IO.File.WriteAllText(inputFile1, string.Empty);
                    System.IO.File.WriteAllText(inputFile2, string.Empty);
                    return;

                }
                else if (EMPTY == inputFile1)
                {
                    NEWOUTPUT = inputFile1;
                    NEWINPUT = OutputFile;
                    

                System.IO.File.WriteAllText(inputFile1, string.Empty);
                    using (StreamReader reader = new StreamReader(inputFile2))
                    using (StreamWriter writer = new StreamWriter(tempFile2))
                    {
                        for (int i = 0; i < f2Count; i++)
                        {
                            reader.ReadLine(); // Пропускаем первую строку
                        }
                        
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            writer.WriteLine(line);
                        }
                    }

                // Заменяем старый файл новым
                System.IO.File.Delete(inputFile2);
                System.IO.File.Move(tempFile2, inputFile2);
                MergeFiles(inputFile2, NEWINPUT, NEWOUTPUT);

            }
                else if (EMPTY == inputFile2)
                {
                    NEWOUTPUT = inputFile2;
                    NEWINPUT = OutputFile;
                    
                    System.IO.File.WriteAllText(inputFile2, string.Empty);

                using (StreamReader reader = new StreamReader(inputFile1))
                using (StreamWriter writer = new StreamWriter(tempFile1))
                {
                    for (int i = 0; i < f1Count; i++)
                    {
                        reader.ReadLine(); // Пропускаем первую строку
                    }

                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        writer.WriteLine(line);
                    }
                }

                // Заменяем старый файл новым
                System.IO.File.Delete(inputFile1);
                System.IO.File.Move(tempFile1, inputFile1);
                
                MergeFiles(inputFile1, NEWINPUT, NEWOUTPUT);
            }
        }
            Console.WriteLine("Масив розбито на файли");
    }
}













































/*using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class PolyphaseMergeSort
{
    static List<int> GenerateFibonacciSequence(int n)
    {
        List<int> fib = new List<int> { 1, 1 };
        while (fib.Count < n)
        {
            fib.Add(fib[fib.Count - 1] + fib[fib.Count - 2]);
        }
        return fib;
    }

    static void SplitIntoRuns(string inputPath, string file1Path, string file2Path)
    {
        using (StreamReader reader = new StreamReader(inputPath))
        using (StreamWriter f1 = new StreamWriter(file1Path))
        using (StreamWriter f2 = new StreamWriter(file2Path))
        {
            List<int> tempRun = new List<int>();
            List<List<int>> runs = new List<List<int>>();
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (int.TryParse(line.Trim(), out int num))
                {
                    if (tempRun.Count == 0 || num >= tempRun.Last())
                        tempRun.Add(num);
                    else
                    {
                        runs.Add(new List<int>(tempRun));
                        tempRun.Clear();
                        tempRun.Add(num);
                    }
                }
            }
            if (tempRun.Count > 0) runs.Add(tempRun);

            var fibSequence = GenerateFibonacciSequence(runs.Count);
            int fib1 = fibSequence[fibSequence.Count - 2], fib2 = fibSequence.Last();

            for (int i = 0; i < runs.Count; i++)
            {
                var writer = (i < fib1) ? f1 : f2;
                writer.WriteLine(string.Join(" ", runs[i]));
            }
        }
    }

    static void MergeRuns(string file1Path, string file2Path, string outputPath)
    {
        using (StreamReader f1 = new StreamReader(file1Path))
        using (StreamReader f2 = new StreamReader(file2Path))
        using (StreamWriter output = new StreamWriter(outputPath))
        {
            string line1 = f1.ReadLine(), line2 = f2.ReadLine();
            List<int> list1 = line1?.Split(' ').Select(int.Parse).ToList() ?? new List<int>();
            List<int> list2 = line2?.Split(' ').Select(int.Parse).ToList() ?? new List<int>();

            while (list1.Count > 0 || list2.Count > 0)
            {
                if (list1.Count > 0 && (list2.Count == 0 || list1[0] <= list2[0]))
                {
                    output.WriteLine(list1[0]);
                    list1.RemoveAt(0);
                    if (list1.Count == 0 && !f1.EndOfStream)
                        list1 = f1.ReadLine()?.Split(' ').Select(int.Parse).ToList() ?? new List<int>();
                }
                else if (list2.Count > 0)
                {
                    output.WriteLine(list2[0]);
                    list2.RemoveAt(0);
                    if (list2.Count == 0 && !f2.EndOfStream)
                        list2 = f2.ReadLine()?.Split(' ').Select(int.Parse).ToList() ?? new List<int>();
                }
            }
        }
        File.Delete(file1Path);
        File.Delete(file2Path);
    }

    public static void Sort(string inputPath, string outputPath)
    {
        string file1Path = "file1.txt", file2Path = "file2.txt";
        SplitIntoRuns(inputPath, file1Path, file2Path);
        MergeRuns(file1Path, file2Path, outputPath);
    }
}

class Program
{
    static void Main()
    {
        string inputPath = "input.txt";
        string outputPath = "sorted_output.txt";

        if (!File.Exists(inputPath))
        {
            Console.WriteLine("Error: Input file not found.");
            return;
        }

        PolyphaseMergeSort.Sort(inputPath, outputPath);
        Console.WriteLine("Sorting complete. Check sorted_output.txt");
    }
}*/
