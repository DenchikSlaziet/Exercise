using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Work.Models;

namespace Work
{
    // Подключение осуществляется к MsSQL, что бы поменять строку подключения, скопируйте нужную вам строку в App.config
    // При первом обращении программы к БД она подвисает, потому что происходит подключение
    // При создании 1000000 строк, автоматически наберется 100 строк в которых пол мужской и ФИО начинается с "F"
    // ФИО берутся с файла, котрый хранится рядом с exe
    // В задании 6 я не понял что нужно изменить в БД что бы программа работала быстрее
     
    internal class Program
    {
        private static object locker = new object();
        private static MyDbContext context = new MyDbContext();
        private static Stopwatch timer = new Stopwatch();
        private static List<string> names = GetRandomFio();

        static void Main(string[] args)
        {
                while (true)
                {
                    Console.WriteLine("Выбирите нужное вам действие:\n1 - Добавить запись\n2 - Вывод всех строк\n" +
                        "3 - Вывод уникальных людей(ФИО и Дата)\n4 - Вывод мужчин на букву F\n5 - Добавление строк в БД");

                    switch (GetParseInt())
                    {
                        // Добавление записи
                        case 1:
                        lock (locker)
                        {
                            context.Peoples.Add(GetPeople());
                            context.SaveChanges();
                            Console.WriteLine("Запись добавлена!");
                        }
                        break;

                        //Вывести всю таблицу
                        case 2:
                        lock (locker)
                        {
                            PrintTable(context.Peoples.ToList());
                        }
                        break;

                        //Вывести уникальные значения
                        case 3:
                        lock (locker)
                        {
                            var list = context.Peoples.ToList();

                            PrintTable(list.Where(x => list.Count(y => x.FIO == y.FIO &&
                            x.DateOfBirth == y.DateOfBirth) == 1).OrderBy(x => x.FIO).ToList());
                        }
                        break;

                        //Вывести людей у в которых пол мужской и ФИО начинается с "F"
                        case 4:
                        lock (locker)
                        {
                            timer.Start();
                            PrintTable(context.Peoples.Where(x => x.FIO.StartsWith("F") &&
                            x.Gender == Gender.Male.ToString()).ToList());
                            timer.Stop();

                            Console.WriteLine($"Потраченное время: {timer.Elapsed.TotalMilliseconds} мс");
                        }
                        break;

                        //Создание 1000000 строк
                        case 5:
                        for (int i = 0; i < 100; i++)
                        {
                            SaveAll(10000);
                        }
                        break;

                    default:
                        Console.WriteLine("Нет такой функции!");
                            break;
                    }
                }
        }

        private static void PrintTable(List<People> peoples)
        {
            Console.WriteLine("\n{0,-35} {1,-15} {2,-10} {3}","ФИО","Дата рождения","Пол","Возраст");
            foreach (var item in peoples)
            {
                Console.WriteLine("{0,-35} {1,-15} {2,-10} {3}", item.FIO , item.DateOfBirth , item.Gender , item.Age);
            }
            Console.WriteLine("\n");
        }

        private static int GetParseInt()
        {
            while(true)
            {
                if(int.TryParse(Console.ReadLine(), out int result) && result > 0)
                {
                    return result;
                }
                Console.Write("Неврное значение, введите еще раз: ");
            }
        }

        private static string GetParseString(string print)
        {
            while (true)
            {
                Console.Write($"Введите {print}: ");
                var str = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(str))
                {
                    return str;
                }
                Console.WriteLine("Неврное значение!");
            }
        }

        private static DateTime GetParseDate(string print)
        {
            while (true)
            {
                Console.Write($"Введите {print}: ");
                if (DateTime.TryParse(Console.ReadLine(),out DateTime date) && date < DateTime.Now)
                {
                    return date;
                }
                Console.WriteLine("Неврное значение!");
            }
        }

        private static Gender GetParseGender()
        {
            while (true)
            {
                Console.Write($"Введите пол(M/F): ");
                var str = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(str) && str.Length == 1)
                {
                    switch (str.ToLower())
                    {
                        case "m":
                            return Gender.Male;

                        case "f":
                            return Gender.Female;

                        default:
                            Console.WriteLine("Такого пола нет!");
                            break;
                    }
                }
                Console.WriteLine("Пол обозначается одной буквой!");
            }
        }

        private static People GetPeople()
        {
            var fio= GetParseString("ФИО");
            var date = GetParseDate("дату рождения");
            var gender = GetParseGender();
            return new People(fio, date, gender);
        }

        /// <summary>
        /// Поток добавления данных в бд. Создаю поток что бы поделить добавление 1000000 строк на разные потоки.
        /// </summary>
        /// <param name="count">Кол-во строк</param>
        private static async void SaveAll(int count)
        {
            var rnd = new Random();
            var list = new List<People>();

            await Task.Run(() =>
             {
                 for (int i = 0; i < count; i++)
                 {
                     list.Add(new People(names[rnd.Next(0,names.Count-1)],
                     DateTime.Now, (Gender)rnd.Next(0, 2)));
                 }
             });

            lock (locker)
            {
                context.Peoples.AddRange(list);
                context.SaveChanges();
            }
        }

        private static List<string> GetRandomFio()
        {
            using (var sr = new StreamReader("FIO.txt"))
            {
                var list = new List<string>();

                for (int i = 0; i < 500; i++)
                {
                    list.Add(sr.ReadLine());
                }

                return list;
            }
        }
    }
}
