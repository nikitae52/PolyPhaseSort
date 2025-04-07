//Поки найближчий варіант
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        string fileName;
        string tempfile01 = "tempfile01.txt"; // Writer 1
        string tempfile02 = "tempfile02.txt"; // Writer 2
        string tempfile03 = "tempfile03.txt"; // Writer 2

        Console.WriteLine("Enter file name:");
        fileName = Console.ReadLine();

        using (StreamReader reader = new StreamReader(fileName))
        using (StreamWriter writer1 = new StreamWriter(tempfile01))
        using (StreamWriter writer2 = new StreamWriter(tempfile02))
        {
            int fib1 = 1, fib2 = 1;
            int activeFib1 = fib1, activeFib2 = fib2;
            int totalWritten1 = 0, totalWritten2 = 0;

            string line = reader.ReadLine();
            if (!int.TryParse(line, out int prevNumb)) return;

            StreamWriter activeWriter = writer1;
            bool writingToFirst = true; // true = writer1, false = writer2

            activeWriter.Write(prevNumb);
            while ((line = reader.ReadLine()) != null)
            {
                if (!int.TryParse(line, out int number)) continue;

                if (number >= prevNumb)
                {
                    activeWriter.Write(" " + number);
                }
                else
                {
                    activeWriter.WriteLine(); // закончился подмассив

                    // учёт подмассивов
                    if (writingToFirst) totalWritten1++;
                    else totalWritten2++;

                    // проверка: достигли ли лимита
                    if (totalWritten1 >= activeFib1 && totalWritten2 >= activeFib2)
                    {
                        int next = fib1 + fib2;
                        fib1 = fib2;
                        fib2 = next;
                        activeFib1 = fib1;
                        activeFib2 = fib2;

                    }

                    // переключаемся на нужный поток
                    if (totalWritten1 < activeFib1)
                    {
                        activeWriter = writer1;
                        writingToFirst = true;
                    }
                    else if (totalWritten2 < activeFib2)
                    {
                        activeWriter = writer2;
                        writingToFirst = false;
                    }

                    // начинаем новый подмассив
                    activeWriter.Write(number);
                }

                prevNumb = number;
            }

            activeWriter.WriteLine();
            if (writingToFirst) totalWritten1++;
            else totalWritten2++;

            // Дописываем недостающие EMPTY
            while (totalWritten1 < activeFib1)
            {
                writer1.WriteLine("E");
                totalWritten1++;
            }

            while (totalWritten2 < activeFib2)
            {
                writer2.WriteLine("E");
                totalWritten2++;
            }

            activeWriter.WriteLine(); // Завершить последний подмассив
        }

        MergeFiles(tempfile01, tempfile02, tempfile03);

        Console.WriteLine("Подмассивы распределены без накопления в памяти.");
    }

    static void MergeFiles(string inputfile1, string inputfile2, string outputfile)
    {
        bool has1, has2;//обьявляем тут чтобы потом анализировать для рекурсии
        using (StreamReader reader1 = new StreamReader(inputfile1))
        using (StreamReader reader2 = new StreamReader(inputfile2))
        using (StreamWriter writer = new StreamWriter(outputfile))
        {
            has1 = TryReadNextNumber(reader1, out int number1, out bool isEmptyMarker1);
            has2 = TryReadNextNumber(reader2, out int number2, out bool isEmptyMarker2);


            while (has1 && has2)
            {


                if (number1 <= number2)
                {
                    
                    if (isEmptyMarker1 && isEmptyMarker2) //если два числа последние в строке
                    {
                        writer.Write(number1 + " ");//вводим меньшеее число з 1 файла
                        writer.Write(number2 + " ");//вводим большее число з 2 файла
                        writer.WriteLine();//переходим на новую строку
                        has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);//ищем следующие числа
                        has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);

                    }

                    else if (isEmptyMarker1)// если число из 1 файла последнее в строке
                    {   
                        writer.Write(number1 + " ");// записываем его
                        while (!isEmptyMarker2)// пишем числа из файла 2 пока строчка не закончиться
                        {
                            writer.Write(number2 + " ");
                            has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                        }
                        writer.Write(number2 + " ");// пишем последнее число з 2 файла
                        writer.WriteLine();
                        has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);//ищем следующие числа
                        has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                    }
                    else if (isEmptyMarker2)//если число из 2 файла последнее в строке
                    {   

                        while (!isEmptyMarker1) //пока не закончится строка в 1 файле
                        {
                            if (number1 <= number2) // если  число из 1 файла меньше, выводим числа из 1 файла
                            {
                                writer.Write(number1 + " ");
                                has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);

                                if (isEmptyMarker1 && number1<=number2)//если последнее число из 1 файла так и осталось меньше
                                {
                                    writer.Write(number1 + " ");
                                    writer.Write(number2 + " ");
                                    writer.WriteLine();
                                }
                                else if (isEmptyMarker1 && number1 > number2)//если последнее число из 1 файла стало больше
                                {
                                    writer.Write(number2 + " ");
                                    writer.Write(number1 + " ");
                                    writer.WriteLine();
                                }
                            }
                            else //пока они не станут больше
                            {
                                writer.Write(number2 + " ");// пишем последнее число з 2 файла
                                while (!isEmptyMarker1)// и до конца строки пишем числа из файла 1
                                {
                                    writer.Write(number1 + " ");
                                    has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                                }
                                writer.Write(number1 + " ");// записываем последнее число з 1 файла которое цикл не покрывает

                            }
                        }
                        writer.WriteLine();//переходим на новую строку
                        has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);//ищем следующие числа
                        has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                    }
                    else//если числа не последние, просто записываем меньшее и запрашиваем новое
                    {
                        writer.Write(number1 + " ");
                        has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                    }



                }
                else// если число з 2 файла меньше
                {
                    
                    if (isEmptyMarker1 && isEmptyMarker2) //если два числа последние в строке
                    {
                        writer.Write(number2 + " ");//вводим меньшее число
                        writer.Write(number1 + " ");// вводим большее число
                        writer.WriteLine();//переходим на новую строку
                        has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);//ищем следующие числа
                        has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                    }

                    else if (isEmptyMarker1)// если число из 1 файла последнее в строке
                    {
                        //writer.Write(number2 + " ");
                        while (!isEmptyMarker2)//пока не закончаться числа во 2 файле 
                        {
                            if (number1 > number2) // если  число из 2 файла меньше, выводим числа из 2 файла
                            {
                                writer.Write(number2 + " ");
                                has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                                if (isEmptyMarker2 && number1 > number2)//на случай если последнее будет меньше
                                {
                                    writer.Write(number2 + " ");
                                    writer.Write(number1 + " ");
                                    //writer.WriteLine();
                                }
                                else if (isEmptyMarker2 && number1 <= number2)//если последнее будет больше
                                {
                                    writer.Write(number1 + " ");
                                    writer.Write(number2 + " ");
                                    //writer.WriteLine();
                                }
                            }
                            else //пока они не станут больше
                            {
                                writer.Write(number1 + " ");// пишем последнее число з 1 файла
                                while (!isEmptyMarker2)// и до конца строки пишем числа из файла 2
                                {
                                    writer.Write(number2 + " ");
                                    has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                                }
                                writer.Write(number2 + " ");
                            }
                        }
                        has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                        has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                        writer.WriteLine();
                    }
                    else if (isEmptyMarker2)//если число из 2 файла последнее в строке
                    {
                        writer.Write(number2 + " ");//пишем его так как оно меньше
                        while (!isEmptyMarker1)//пишем до конца строки чисел из файла 1
                        {
                            writer.Write(number1 + " ");
                            has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                        }
                        writer.Write(number1 + " ");//записуем последнее число з 1 файла
                        writer.WriteLine();
                        has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                        has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                    }

                    else//если числа не последние, просто записываем меньшее и запрашиваем новое
                    {
                        writer.Write(number2 + " ");
                        has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                    }

                        /*
                        if (isEmptyMarker2)
                        {
                            while (isEmptyMarker1)
                            {
                                writer.Write(number1);
                                has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                            }
                            writer.WriteLine();
                        }
                        */
                    
                }
                /*
                                while (has1)
                                {
                                    writer.Write(number1 + " ");
                                    has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                                }

                                while (has2)
                                {
                                    writer.Write(number2 + " ");
                                    has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                                }
                */
            }


        }
        //если один из файлов закончился, то сливаем оставшиеся числа из другого файла
        int count1 = CountLines(outputfile);//считаем количество строк в файле 3
        RemoveFirstNLines(inputfile1, count1);//удаляем обьедененное колличество слов
        RemoveFirstNLines(inputfile2, count1);
        Console.WriteLine(count1);

        if (!has1 && !has2)
        {
            return;//заканчиваем рекурсию
        }
        else if (!has1)// если первым закончился файл 1
            {
                MergeFiles(inputfile2, outputfile, inputfile1);
            }
        else if (!has2)// если первым закончился файл 2
            {
                MergeFiles(inputfile1, outputfile, inputfile2);
            }




        static bool TryReadNextNumber(StreamReader reader, out int result, out bool isEndOfLine)
        {
            result = 0;
            isEndOfLine = false;
            string currentNumber = "";
            int ch;

            while ((ch = reader.Read()) != -1)
            {
                char c = (char)ch;

                if (c == '\n')
                {


                    // Если число накопилось перед \n — сначала его вернём
                    if (currentNumber.Length > 0)
                    {
                        if (int.TryParse(currentNumber, out result))
                        {
                            return true;
                        }
                        else
                        {
                            return false; // ошибка парсинга числа
                        }
                    }

                    // Иначе просто переходим к следующему числу
                    continue;
                }

                if (char.IsWhiteSpace(c))
                {
                    if (currentNumber.Length > 0)
                    {
                        if (int.TryParse(currentNumber, out result))
                        {

                            if (reader.Peek() == '\n'|| reader.Peek() == '\r')
                            {
                                isEndOfLine = true;
                            }
                            return true;
                        }
                        else
                        {
                            return false; // ошибка парсинга числа
                        }
                    }
                    // пробелы до начала числа — игнорируем
                }
                else
                {
                    currentNumber += c;
                }
            }

            // Дошли до конца файла — возможно, есть ещё одно число
            if (currentNumber.Length > 0)
            {
                if (int.TryParse(currentNumber, out result))
                {
                    isEndOfLine = true; // условно считаем, что строка закончилась
                    return true;
                }
            }

            return false; // действительно конец файла
        }

        static int CountLines(string filePath)//считаем количество строк в файле
        {
            int lineCount = 0;
            using (StreamReader reader = new StreamReader(filePath))
            {
                int b;
                while ((b = reader.Read()) != -1)
                {
                    if (b == '\n')
                        lineCount++;
                }
            }
            return lineCount;
        }
        static void RemoveFirstNLines(string filePath, int N)
        {
            string tempFilePath = "TEMPFILE.txt";
            using (var input = File.OpenRead(filePath))
            using (var output = File.Create(tempFilePath))
            {
                int b;
                int newlineCount = 0;
                bool startWriting = false;

                while ((b = input.ReadByte()) != -1)
                {
                    if (!startWriting)
                    {
                        if (b == '\n')
                        {
                            newlineCount++;
                            if (newlineCount >= N)
                            {
                                startWriting = true;
                            }
                        }
                        continue;
                    }

                    output.WriteByte((byte)b);
                }
            }

            // Заменяем оригинальный файл
            File.Delete(filePath);
            File.Move(tempFilePath, filePath);
        }
        }

    }



//Кінець


//Поки найближчий варіант
using System;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        string fileName;
        string tempfile01 = "tempfile01.txt"; // Writer 1
        string tempfile02 = "tempfile02.txt"; // Writer 2
        string tempfile03 = "tempfile03.txt"; // Writer 2

        Console.WriteLine("Enter file name:");
        fileName = Console.ReadLine();

        using (StreamReader reader = new StreamReader(fileName))
        using (StreamWriter writer1 = new StreamWriter(tempfile01))
        using (StreamWriter writer2 = new StreamWriter(tempfile02))
        {
            int fib1 = 1, fib2 = 1;
            int activeFib1 = fib1, activeFib2 = fib2;
            int totalWritten1 = 0, totalWritten2 = 0;

            string line = reader.ReadLine();
            if (!int.TryParse(line, out int prevNumb)) return;

            StreamWriter activeWriter = writer1;
            bool writingToFirst = true; // true = writer1, false = writer2

            activeWriter.Write(prevNumb);
            while ((line = reader.ReadLine()) != null)
            {
                if (!int.TryParse(line, out int number)) continue;

                if (number >= prevNumb)
                {
                    activeWriter.Write(" " + number);
                }
                else
                {
                    activeWriter.WriteLine(); // закончился подмассив

                    // учёт подмассивов
                    if (writingToFirst) totalWritten1++;
                    else totalWritten2++;

                    // проверка: достигли ли лимита
                    if (totalWritten1 >= activeFib1 && totalWritten2 >= activeFib2)
                    {
                        int next = fib1 + fib2;
                        fib1 = fib2;
                        fib2 = next;
                        activeFib1 = fib1;
                        activeFib2 = fib2;

                    }

                    // переключаемся на нужный поток
                    if (totalWritten1 < activeFib1)
                    {
                        activeWriter = writer1;
                        writingToFirst = true;
                    }
                    else if (totalWritten2 < activeFib2)
                    {
                        activeWriter = writer2;
                        writingToFirst = false;
                    }

                    // начинаем новый подмассив
                    activeWriter.Write(number);
                }

                prevNumb = number;
            }

            activeWriter.WriteLine();
            if (writingToFirst) totalWritten1++;
            else totalWritten2++;

            // Дописываем недостающие EMPTY
            while (totalWritten1 < activeFib1)
            {
                writer1.WriteLine("EMPTY");
                totalWritten1++;
            }

            while (totalWritten2 < activeFib2)
            {
                writer2.WriteLine("EMPTY");
                totalWritten2++;
            }

            activeWriter.WriteLine(); // Завершить последний подмассив
        }

        MergeFiles(tempfile01, tempfile02, tempfile03);

        Console.WriteLine("Подмассивы распределены без накопления в памяти.");
    }

    static void MergeFiles(string inputfile1, string inputfile2, string outputfile)
    {
        using (StreamReader reader1 = new StreamReader(inputfile1))
        using (StreamReader reader2 = new StreamReader(inputfile2))
        using (StreamWriter writer = new StreamWriter(outputfile))
        {
            bool has1 = TryReadNextNumber(reader1, out int number1, out bool isEmptyMarker1);
            bool has2 = TryReadNextNumber(reader2, out int number2, out bool isEmptyMarker2);


                while (has1 && has2)
                {


                    if (number1 <= number2)
                    {
                        writer.Write(number1 + " ");
                        has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                        if (isEmptyMarker1 && isEmptyMarker2)
                        {
                            if (number1 <= number2)
                            {
                                writer.Write(number1 + " ");
                                writer.Write(number2 + " ");
                                writer.WriteLine();
                                has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                                has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                            }
                            else
                            {
                                writer.Write(number2 + " ");
                                writer.Write(number1 + " ");
                                writer.WriteLine();
                                has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                                has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                            }
                        }
                        /*
                        if (isEmptyMarker1)
                        {
                            while (isEmptyMarker2)
                            {
                                writer.Write(number2 + " ");
                                has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                            }
                            writer.WriteLine();
                        }
                        */

                    }
                    else
                    {
                        writer.Write(number2 + " ");
                        has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                        if (isEmptyMarker1 && isEmptyMarker2)
                        {
                            if (number1 <= number2)
                            {
                                writer.Write(number1 + " ");
                                writer.Write(number2 + " ");
                                writer.WriteLine();
                                has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                                has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                            }
                            else
                            {
                                writer.Write(number2 + " ");
                                writer.Write(number1 + " ");
                                writer.WriteLine();
                                has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                                has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                            }
                            /*
                            if (isEmptyMarker2)
                            {
                                while (isEmptyMarker1)
                                {
                                    writer.Write(number1);
                                    has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                                }
                                writer.WriteLine();
                            }
                            */
                        }
                    }
    /*
                    while (has1)
                    {
                        writer.Write(number1 + " ");
                        has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                    }

                    while (has2)
                    {
                        writer.Write(number2 + " ");
                        has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                    }
    */
                }
        }


        static bool TryReadNextNumber(StreamReader reader, out int result, out bool isEndOfLine)
        {
            result = 0;
            isEndOfLine = false;
            string currentNumber = "";
            int ch;

            while ((ch = reader.Read()) != -1)
            {
                char c = (char)ch;

                if (c == '\n')
                {


                    // Если число накопилось перед \n — сначала его вернём
                    if (currentNumber.Length > 0)
                    {
                        if (int.TryParse(currentNumber, out result))
                        {
                            return true;
                        }
                        else
                        {
                            return false; // ошибка парсинга числа
                        }
                    }

                    // Иначе просто переходим к следующему числу
                    continue;
                }

                if (char.IsWhiteSpace(c))
                {
                    if (currentNumber.Length > 0)
                    {
                        if (int.TryParse(currentNumber, out result))
                        {

                            if (reader.Peek() == '\n')
                            {
                                isEndOfLine = true;
                            }
                            return true;
                        }
                        else
                        {
                            return false; // ошибка парсинга числа
                        }
                    }
                    // пробелы до начала числа — игнорируем
                }
                else
                {
                    currentNumber += c;
                }
            }

            // Дошли до конца файла — возможно, есть ещё одно число
            if (currentNumber.Length > 0)
            {
                if (int.TryParse(currentNumber, out result))
                {
                    isEndOfLine = true; // условно считаем, что строка закончилась
                    return true;
                }
            }

            return false; // действительно конец файла
        }


    }
}


//Кінець

//Другий найближчий
/*
//Поки найближчий варіант
using System;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        string fileName;
        string tempfile01 = "tempfile01.txt"; // Writer 1
        string tempfile02 = "tempfile02.txt"; // Writer 2
        string tempfile03 = "tempfile03.txt"; // Writer 2

        Console.WriteLine("Enter file name:");
        fileName = Console.ReadLine();

        using (StreamReader reader = new StreamReader(fileName))
        using (StreamWriter writer1 = new StreamWriter(tempfile01))
        using (StreamWriter writer2 = new StreamWriter(tempfile02))
        {
            int fib1 = 1, fib2 = 1;
            int activeFib1 = fib1, activeFib2 = fib2;
            int totalWritten1 = 0, totalWritten2 = 0;

            string line = reader.ReadLine();
            if (!int.TryParse(line, out int prevNumb)) return;

            StreamWriter activeWriter = writer1;
            bool writingToFirst = true; // true = writer1, false = writer2

            activeWriter.Write(prevNumb);
            while ((line = reader.ReadLine()) != null)
            {
                if (!int.TryParse(line, out int number)) continue;

                if (number >= prevNumb)
                {
                    activeWriter.Write(" " + number);
                }
                else
                {
                    activeWriter.WriteLine(); // закончился подмассив

                    // учёт подмассивов
                    if (writingToFirst) totalWritten1++;
                    else totalWritten2++;

                    // проверка: достигли ли лимита
                    if (totalWritten1 >= activeFib1 && totalWritten2 >= activeFib2)
                    {
                        int next = fib1 + fib2;
                        fib1 = fib2;
                        fib2 = next;
                        activeFib1 = fib1;
                        activeFib2 = fib2;

                    }

                    // переключаемся на нужный поток
                    if (totalWritten1 < activeFib1)
                    {
                        activeWriter = writer1;
                        writingToFirst = true;
                    }
                    else if (totalWritten2 < activeFib2)
                    {
                        activeWriter = writer2;
                        writingToFirst = false;
                    }

                    // начинаем новый подмассив
                    activeWriter.Write(number);
                }

                prevNumb = number;
            }

            activeWriter.WriteLine();
            if (writingToFirst) totalWritten1++;
            else totalWritten2++;

            // Дописываем недостающие EMPTY
            while (totalWritten1 < activeFib1)
            {
                writer1.WriteLine("EMPTY");
                totalWritten1++;
            }

            while (totalWritten2 < activeFib2)
            {
                writer2.WriteLine("EMPTY");
                totalWritten2++;
            }

            activeWriter.WriteLine(); // Завершить последний подмассив
        }

        MergeFiles(tempfile01, tempfile02, tempfile03);

        Console.WriteLine("Подмассивы распределены без накопления в памяти.");
    }

    static void MergeFiles(string inputfile1, string inputfile2, string outputfile)
    {
        using (StreamReader reader1 = new StreamReader(inputfile1))
        using (StreamReader reader2 = new StreamReader(inputfile2))
        using (StreamWriter writer = new StreamWriter(outputfile))
        {
            bool has1 = TryReadNextNumber(reader1, out int number1, out bool isEmptyMarker1);
            bool has2 = TryReadNextNumber(reader2, out int number2, out bool isEmptyMarker2);


            while (has1 && has2)
            {


                if (number1 <= number2)
                {
                    
                    if (isEmptyMarker1 && isEmptyMarker2) //если два числа последние в строке
                    {
                            writer.Write(number1 + " ");
                            writer.Write(number2 + " ");
                            writer.WriteLine();
                            has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                            has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);

                    }

                    else if (isEmptyMarker1)// если число из 1 файла последнее в строке
                    {   
                        writer.Write(number1 + " ");
                        while (!isEmptyMarker2)
                        {
                            writer.Write(number2 + " ");
                            has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                        }
                        writer.Write(number2 + " ");
                        writer.WriteLine();
                        has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                        has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                    }
                    else if (isEmptyMarker2)//если число из 2 файла последнее в строке
                    {   

                        while (!isEmptyMarker1)
                        {
                            if (number1 <= number2) // если  число из 1 файла меньше, выводим числа из 1 файла
                            {
                                writer.Write(number1 + " ");
                                has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);

                                if (isEmptyMarker1 && number1<=number2)//на случай если все будут меньше
                                {
                                    writer.Write(number1 + " ");
                                    writer.Write(number2 + " ");
                                    writer.WriteLine();
                                }
                                else if (isEmptyMarker1 && number1 > number2)
                                {
                                    writer.Write(number2 + " ");
                                    writer.Write(number1 + " ");
                                    writer.WriteLine();
                                }
                            }
                            else //пока они не станут больше
                            {
                                writer.Write(number2 + " ");// пишем последнее число з 2 файла
                                while (!isEmptyMarker1)// и до конца строки пишем числа из файла 1
                                {
                                    writer.Write(number1 + " ");
                                    has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                                }

                            }
                        }
                        writer.WriteLine();
                        has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                        has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                    }
                    else
                    {
                        writer.Write(number1 + " ");
                        has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                    }



                }
                else
                {
                    
                    if (isEmptyMarker1 && isEmptyMarker2) //если два числа последние в строке
                    {
                        writer.Write(number2 + " ");
                        writer.Write(number1 + " ");
                        writer.WriteLine();
                        has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                        has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                    }

                    else if (isEmptyMarker1)// если число из 1 файла последнее в строке
                    {
                        //writer.Write(number2 + " ");
                        while (!isEmptyMarker2)
                        {
                            if (number1 > number2) // если  число из 2 файла меньше, выводим числа из 2 файла
                            {
                                writer.Write(number2 + " ");
                                has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                                if (isEmptyMarker2 && number1 > number2)//на случай если все будут меньше
                                {
                                    writer.Write(number2 + " ");
                                    writer.Write(number1 + " ");
                                    //writer.WriteLine();
                                }
                                else if (isEmptyMarker2 && number1 <= number2)
                                {
                                    writer.Write(number1 + " ");
                                    writer.Write(number2 + " ");
                                    //writer.WriteLine();
                                }
                            }
                            else //пока они не станут больше
                            {
                                writer.Write(number1 + " ");// пишем последнее число з 1 файла
                                while (!isEmptyMarker2)// и до конца строки пишем числа из файла 2
                                {
                                    writer.Write(number2 + " ");
                                    has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                                }
                            }
                        }
                        has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                        has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                        writer.WriteLine();
                    }
                    else if (isEmptyMarker2)//если число из 2 файла последнее в строке
                    {
                        writer.Write(number2 + " ");
                        while (!isEmptyMarker1)
                        {
                            writer.Write(number1 + " ");
                            has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                        }
                        writer.Write(number1 + " ");
                        writer.WriteLine();
                        has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                        has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                    }

                    else
                    {
                        writer.Write(number2 + " ");
                        has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                    }

                        /*
                        if (isEmptyMarker2)
                        {
                            while (isEmptyMarker1)
                            {
                                writer.Write(number1);
                                has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                            }
                            writer.WriteLine();
                        }
                        */
                    
                }
                /*
                                while (has1)
                                {
                                    writer.Write(number1 + " ");
                                    has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                                }

                                while (has2)
                                {
                                    writer.Write(number2 + " ");
                                    has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                                }
                */
            }
        }


        static bool TryReadNextNumber(StreamReader reader, out int result, out bool isEndOfLine)
        {
            result = 0;
            isEndOfLine = false;
            string currentNumber = "";
            int ch;

            while ((ch = reader.Read()) != -1)
            {
                char c = (char)ch;

                if (c == '\n')
                {


                    // Если число накопилось перед \n — сначала его вернём
                    if (currentNumber.Length > 0)
                    {
                        if (int.TryParse(currentNumber, out result))
                        {
                            return true;
                        }
                        else
                        {
                            return false; // ошибка парсинга числа
                        }
                    }

                    // Иначе просто переходим к следующему числу
                    continue;
                }

                if (char.IsWhiteSpace(c))
                {
                    if (currentNumber.Length > 0)
                    {
                        if (int.TryParse(currentNumber, out result))
                        {

                            if (reader.Peek() == '\n')
                            {
                                isEndOfLine = true;
                            }
                            return true;
                        }
                        else
                        {
                            return false; // ошибка парсинга числа
                        }
                    }
                    // пробелы до начала числа — игнорируем
                }
                else
                {
                    currentNumber += c;
                }
            }

            // Дошли до конца файла — возможно, есть ещё одно число
            if (currentNumber.Length > 0)
            {
                if (int.TryParse(currentNumber, out result))
                {
                    isEndOfLine = true; // условно считаем, что строка закончилась
                    return true;
                }
            }

            return false; // действительно конец файла
        }


    }
}


//Кінець
*/
/*
﻿using System;
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
/**
        static bool TryReadNextNumber(StreamReader reader, out int result, out bool isEmptyMarker)
        {
            result = 0;
            string number = "";
            isEmptyMarker = false;
            while (!reader.EndOfStream)
            {
                int ch = reader.Read();

                // конец строки = закончить
                if (ch == '\n' || ch == '\r')
                    break;

                if (char.IsWhiteSpace((char)ch))
                {
                    if (number.Length > 0)
                        break; // число закончилось
                    else
                        continue; // пропускаем лишние пробелы
                }

                number += (char)ch;
            }

            if (number == "E")
            {
                isEmptyMarker = true;
                return true;
            }
            if (int.TryParse(number, out result))
            {
                return true;
            }

            return false;
        }

/*
using System;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        string fileName;
        string tempfile02 = "tempfile01.txt"; // Writer 1
        string tempfile03 = "tempfile02.txt"; // Writer 2

        Console.WriteLine("Enter file name:");
        fileName = Console.ReadLine();

        using (StreamReader reader = new StreamReader(fileName))
        using (StreamWriter writer1 = new StreamWriter(tempfile02))
        using (StreamWriter writer2 = new StreamWriter(tempfile03))
        {
            int fib1 = 1, fib2 = 1;
            int activeFib1 = fib1, activeFib2 = fib2;
            int totalWritten1 = 0, totalWritten2 = 0;

            string line = reader.ReadLine();
            if (!int.TryParse(line, out int prevNumb)) return;

            StreamWriter activeWriter = writer1;
            bool writingToFirst = true; // true = writer1, false = writer2

            activeWriter.Write(prevNumb);
            while ((line = reader.ReadLine()) != null)
            {
                if (!int.TryParse(line, out int number)) continue;

                if (number >= prevNumb)
                {
                    activeWriter.Write(" " + number);
                }
                else
                {
                    activeWriter.WriteLine(); // закончился подмассив

                    // учёт подмассивов
                    if (writingToFirst) totalWritten1++;
                    else totalWritten2++;

                    // проверка: достигли ли лимита
                    if (totalWritten1 >= activeFib1 && totalWritten2 >= activeFib2)
                    {
                        int next = fib1 + fib2;
                        fib1 = fib2;
                        fib2 = next;
                        activeFib1 = fib1;
                        activeFib2 = fib2;

                    }

                    // переключаемся на нужный поток
                    if (totalWritten1 < activeFib1)
                    {
                        activeWriter = writer1;
                        writingToFirst = true;
                    }
                    else if (totalWritten2 < activeFib2)
                    {
                        activeWriter = writer2;
                        writingToFirst = false;
                    }

                    // начинаем новый подмассив
                    activeWriter.Write(number);
                }

                prevNumb = number;
            }

            activeWriter.WriteLine();
            if (writingToFirst) totalWritten1++;
            else totalWritten2++;

            // Дописываем недостающие EMPTY
            while (totalWritten1 < activeFib1)
            {
                writer1.WriteLine("EMPTY");
                totalWritten1++;
            }

            while (totalWritten2 < activeFib2)
            {
                writer2.WriteLine("EMPTY");
                totalWritten2++;
            }

            activeWriter.WriteLine(); // Завершить последний подмассив
        }

        MergeFiles()

        Console.WriteLine("Подмассивы распределены без накопления в памяти.");
    }
*/












































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


using System;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        string fileName;
        string tempfile01 = "tempfile01.txt"; // Writer 1
        string tempfile02 = "tempfile02.txt"; // Writer 2
        string tempfile03 = "tempfile03.txt"; // Writer 2

        Console.WriteLine("Enter file name:");
        fileName = Console.ReadLine();

        using (StreamReader reader = new StreamReader(fileName))
        using (StreamWriter writer1 = new StreamWriter(tempfile01))
        using (StreamWriter writer2 = new StreamWriter(tempfile02))
        {
            int fib1 = 1, fib2 = 1;
            int activeFib1 = fib1, activeFib2 = fib2;
            int totalWritten1 = 0, totalWritten2 = 0;

            string line = reader.ReadLine();
            if (!int.TryParse(line, out int prevNumb)) return;

            StreamWriter activeWriter = writer1;
            bool writingToFirst = true; // true = writer1, false = writer2

            activeWriter.Write(prevNumb);
            while ((line = reader.ReadLine()) != null)
            {
                if (!int.TryParse(line, out int number)) continue;

                if (number >= prevNumb)
                {
                    activeWriter.Write(" " + number);
                }
                else
                {
                    activeWriter.WriteLine(); // закончился подмассив

                    // учёт подмассивов
                    if (writingToFirst) totalWritten1++;
                    else totalWritten2++;

                    // проверка: достигли ли лимита
                    if (totalWritten1 >= activeFib1 && totalWritten2 >= activeFib2)
                    {
                        int next = fib1 + fib2;
                        fib1 = fib2;
                        fib2 = next;
                        activeFib1 = fib1;
                        activeFib2 = fib2;

                    }

                    // переключаемся на нужный поток
                    if (totalWritten1 < activeFib1)
                    {
                        activeWriter = writer1;
                        writingToFirst = true;
                    }
                    else if (totalWritten2 < activeFib2)
                    {
                        activeWriter = writer2;
                        writingToFirst = false;
                    }

                    // начинаем новый подмассив
                    activeWriter.Write(number);
                }

                prevNumb = number;
            }

            activeWriter.WriteLine();
            if (writingToFirst) totalWritten1++;
            else totalWritten2++;

            // Дописываем недостающие EMPTY
            while (totalWritten1 < activeFib1)
            {
                writer1.WriteLine("E");
                totalWritten1++;
            }

            while (totalWritten2 < activeFib2)
            {
                writer2.WriteLine("E");
                totalWritten2++;
            }

            activeWriter.WriteLine(); // Завершить последний подмассив
        }

        MergeFiles(tempfile01, tempfile02, tempfile03);

        Console.WriteLine("Подмассивы распределены без накопления в памяти.");
    }

    static void MergeFiles(string inputfile1, string inputfile2, string outputfile)
    {
        using (StreamReader reader1 = new StreamReader(inputfile1))
        using (StreamReader reader2 = new StreamReader(inputfile2))
        using (StreamWriter writer = new StreamWriter(outputfile))
        {
            bool has1 = TryReadNextNumber(reader1, out int number1, out bool isEmptyMarker1);
            bool has2 = TryReadNextNumber(reader2, out int number2, out bool isEmptyMarker2);


                while (has1 && has2)
                {
                    if (number1 == int.MaxValue && number2 == int.MaxValue)
                    {
                        writer.WriteLine("E");
                        has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                        has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                    }

                    if (number1 <= number2)
                    {
                    if (number2 == int.MaxValue)
                    {
                        while (has1)
                        {
                            if (isEmptyMarker1)
                            {

                                if (number1 == int.MaxValue)
                                {
                                    writer.WriteLine("E");
                                }
                                else
                                {
                                    writer.Write(number1 + " ");
                                    writer.WriteLine();
                                    has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                                }
                            }
                            if (has1)
                            {
                                writer.Write(number1 + " ");
                                has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                            }


                        }
                    }
                    else
                    {
                        writer.Write(number1 + " ");
                        has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                    }
                        if (isEmptyMarker1 && isEmptyMarker2)
                        {
                            if (number1 <= number2)
                            {
                                writer.Write(number1 + " ");
                                writer.Write(number2 + " ");
                                writer.WriteLine();
                                has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                                has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                            }
                            else
                            {
                                writer.Write(number2 + " ");
                                writer.Write(number1 + " ");
                                writer.WriteLine();
                                has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                                has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                            }
                        }
                        /*
                        if (isEmptyMarker1 && number1== int.MinValue)
                        {
                            while (isEmptyMarker2)
                            {
                                writer.Write(number2 + " ");
                                has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                            }
                            writer.WriteLine();
                        }
                        */

                    }
                else
                {
                    writer.Write(number2 + " ");
                    if (isEmptyMarker2)
                    {
                        writer.Write(number1 + " ");
                        has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                        while (!isEmptyMarker1)
                        {   
                            writer.Write(number1 + " ");
                            has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                        }
                    }
                    else{has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);}
                        if (isEmptyMarker1 && isEmptyMarker2)
                        {
                            if (number1 <= number2)
                            {
                                writer.Write(number1 + " ");
                                writer.Write(number2 + " ");
                                writer.WriteLine();
                                has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                                has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                            }
                            else
                            {
                                writer.Write(number2 + " ");
                                writer.Write(number1 + " ");
                                writer.WriteLine();
                                has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                                has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                            }
                            /*
                            if (isEmptyMarker2 && number2 == int.MinValue)
                            {
                                while (isEmptyMarker1)
                                {
                                    writer.Write(number1);
                                    has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                                }
                                writer.WriteLine();
                            }
                            */
                            
                        }
                    }
                    /*
                                    while (has1)
                                    {
                                        writer.Write(number1 + " ");
                                        has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                                    }

                                    while (has2)
                                    {
                                        writer.Write(number2 + " ");
                                        has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                                    }
                    */
                }
                /*
                if (has1)
                {
                    while (has1)
                    {
                        if (isEmptyMarker1)
                        {

                            if (number1 == int.MaxValue)
                            {
                                writer.WriteLine("E");
                            }
                            else
                            {
                                writer.Write(number1 + " ");
                                writer.WriteLine();
                                has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                            }
                        }
                        if (number1 == int.MinValue)
                        {
                            break;
                        }

                        writer.Write(number1 + " ");
                        has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                    }
                }
                else if (has2)
                {
                    while (has2)
                    {
                        if (isEmptyMarker2)
                        {
                            if (number2 == int.MaxValue)
                            {
                                writer.WriteLine("E");
                            }

                            else
                            {
                                writer.Write(number2 + " ");
                                writer.WriteLine();
                                has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                            }

                        }
                        if (number2== int.MinValue)
                        {
                            break;
                        }
                        if (number2 != int.MaxValue)
                        {
                            writer.Write(number2 + " ");
                        }
                        has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                    }
                }
            */


            static bool TryReadNextNumber(StreamReader reader, out int result, out bool isEndOfLine)
            {
                result = int.MinValue;
                isEndOfLine = false;
                string currentNumber = "";
                int ch;

                while ((ch = reader.Read()) != -1)
                {
                    char c = (char)ch;

                    if (c == 'E') 
                    {
                        result = int.MaxValue;
                        isEndOfLine = true;
                        return true;
                    }

                    if (c == '\n')
                    {


                        // Если число накопилось перед \n — сначала его вернём
                        if (currentNumber.Length > 0)
                        {
                            if (int.TryParse(currentNumber, out result))
                            {
                                return true;
                            }
                            else
                            {
                                return false; // ошибка парсинга числа
                            }
                        }

                        // Иначе просто переходим к следующему числу
                        // continue;
                    }

                    if (char.IsWhiteSpace(c))
                    {
                        if (currentNumber.Length > 0)
                        {
                            if (int.TryParse(currentNumber, out result))
                            {

                                if (reader.Peek() == '\n')
                                {
                                    isEndOfLine = true;
                                }
                                return true;
                            }
                            else
                            {
                                return false; // ошибка парсинга числа
                            }
                        }
                        // пробелы до начала числа — игнорируем
                    }
                    else
                    {
                        currentNumber += c;
                    }
                }

                // Дошли до конца файла — возможно, есть ещё одно число
                if (currentNumber.Length > 0)
                {
                    if (int.TryParse(currentNumber, out result))
                    {
                        isEndOfLine = true; // условно считаем, что строка закончилась
                        return true;
                    }
                }

                return false; // действительно конец файла
            }


        }
    }
}
