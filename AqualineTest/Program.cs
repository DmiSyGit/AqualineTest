using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;

namespace AqualineTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //Задание 1
            Console.WriteLine("Введите название организации:");
            string nameOrg = Console.ReadLine();
            Console.WriteLine("Введите ОПФ организации:");
            string formOrg = Console.ReadLine();
            Console.WriteLine("Результат работы метода:");
            Console.WriteLine(GetString(nameOrg, formOrg));

            // Задание 2
            Console.WriteLine("Введите дату события в формате: dd.mm.yyyy \nПример: 27.12.2011");
            DateTime[] dateTimes = GenerationTask(Console.ReadLine());
            if (dateTimes.Length == 2)
            {
                Console.WriteLine($"Дата начала задачи: {dateTimes[0].ToString("d")}({dateTimes[0].DayOfWeek.ToString()})");
                Console.WriteLine($"Дата окончания задачи: {dateTimes[1].ToString("d")}({dateTimes[1].DayOfWeek.ToString()})");
            }

        }

        static List<string> formsOrg = new List<string> { "АДОК", "А-ка", "АНО", "АО", "АОЗТ", "АООТ", "АП", "АПАОЗТ", "АПАООТ", "АППТ", "АПСТ", "АПТОО", "Арт", "АС", "АСКФХ", "АТП", "Б-ка", "БСП", "Б-ца", "ГКООП", "ГП", "ГСК", "ГУП", "ГУЧ", "Д(С)У", "Д/С", "ДОУч", "ДП", "ДУП", "ЖД", "ЖСК", "ЗАО", "З-д", "Ин-т", "ИЧП", "КБ", "КЛХ", "Комбанк", "Комп", "Кондоминиум", "КООП", "Корп", "КС", "КФ", "КФХ", "КХ", "КЦ", "ЛПХ", "Миссия", "Монастырь", "МП", "МСЧ", "МУП", "МУУП", "МУУЧ", "МУЧ", "МФПГ", "МХП", "НОТК", "НОТП", "НП", "НПО", "НПП", "НПФ", "НТЦ", "ОАО", "Община", "ОД", "ОДО", "ОО", "ООБ", "ООО", "ООС", "ОП", "ОПАОЗТ", "ОПАООТ", "ОППТ", "Опроф", "ОПСТ", "ОПТОО", "ОТД", "ОУЧ", "ОФ", "Партия", "ПАТП", "ПИФ", "ПК", "П-ка", "ПКК", "ПКП", "ПКФ", "ПО", "ПОБ", "ПОО", "ППКООП", "ППО", "ПРЕД", "Приход", "ПРОФКОМ", "ПрТ", "ПС", "ПТ", "ПТК", "РедСМИ", "РО", "РОБ", "РСУ", "РЭУ", "СВХ", "СЕМ", "СКБ", "СМТ", "СМУ", "Союз", "СОЮЗКФХ", "СОЮЗПОБ", "СП", "СТ", "СХК", "ТВ", "ТД", "ТОО", "ТОПроф", "ТСЖ", "ТФ", "ТФПГ", "УОО", "УПТК", "Уч", "УЧПТК", "ФИК", "ФИНОТДЕЛ", "Фирма", "ФКП", "ФЛ", "Фонд", "ФПГ", "ФФ", "ХОЗУ", "ЦБУХ", "ЦДН", "Церковь", "ЦРБ", "ЦРБУХ", "ЧИФ", "ЧОП", "Школа", "Я/С" };
        private static string GetString(string nameOrg, string formOrg = "")
        {
            // Приведение наименования к соответствию условиям
            nameOrg = Regex.Replace(nameOrg, "\'", "", RegexOptions.IgnoreCase).Trim();
            nameOrg = Regex.Replace(nameOrg, "\"", "", RegexOptions.IgnoreCase).Trim();
            foreach (string substring in formsOrg)
            {
                // Удаление ОПФ из названия
                nameOrg = Regex.Replace(" " + nameOrg + " ", " " + substring + " ", "", RegexOptions.IgnoreCase).Trim();
            }
            string resultString = nameOrg + " " + formOrg.Trim();
            return resultString.Trim();
        }

        private static DateTime[] GenerationTask(string date)
        {
            try
            {
                DateTime[] result = new DateTime[2];
                DateTime dateOfEvent = DateTime.Parse(date);
                // Вычисление даты начала задачи
                DateTime dateOfStartTask = dateOfEvent;
                dateOfStartTask = dateOfStartTask.AddDays(-1);
                while (dateOfStartTask.DayOfWeek == DayOfWeek.Sunday || dateOfStartTask.DayOfWeek == DayOfWeek.Saturday)
                {
                    dateOfStartTask = dateOfStartTask.AddDays(-1);
                }
                result[0] = dateOfStartTask;
                // Вычисление даты окончания задачи
                DateTime dateOfEndTask = dateOfEvent;
                dateOfEndTask = dateOfEndTask.AddDays(1);
                while (dateOfEndTask.DayOfWeek == DayOfWeek.Sunday || dateOfEndTask.DayOfWeek == DayOfWeek.Saturday)
                {
                    dateOfEndTask = dateOfEndTask.AddDays(1);
                }
                result[1] = dateOfEndTask;
                return result;
            }
            catch
            {
                Console.WriteLine("Произошла непредвиденная ошибка!\nВозможно произведен некорректный ввод.");
                DateTime[] result = new DateTime[0];
                return result;
            }
        }


        // Задание 3

        public virtual void ObnovlenieInformaciiOKontragentahIzDadata(Context context)
        {
            // Генерация строки фильтров для запроса
            string str_filter = "";
            if (context.ToljkoPustyeStatusy)
                str_filter = "StatusOrganizaciiDadata is NULL";
            else
                str_filter = "NOT (INN LIKE '%-%' OR INN LIKE '%*%' OR INN LIKE '%нет%')";
            // Получение контрагентов соответствующих фильтрации при помощи обращения через ELMA PublicAPI
            var contractors = PublicAPI.CRM.Contractor.ContractorLegal.Find(str_filter);
            context.VsegoKontragentov = contractors.Count;
            //
            double half_cl = (double)context.VsegoKontragentov / 2;// Половина от количества контр агентов
            if (context.RezhimZapuskaObnovleniya == "Автоматический")
                switch ((int)DateTime.Now.DayOfWeek)// В зависимости от текущего дня недели обновляется определенное количество
                {
                    case 6:
                        context.NachaljnyyNomerVyborki = 1;
                        context.MaksimaljnoeKolichestvoKontragentov = (int)Math.Ceiling(half_cl);
                        break;
                    case 0:
                        context.NachaljnyyNomerVyborki = (int)Math.Ceiling(half_cl);
                        context.MaksimaljnoeKolichestvoKontragentov = (int)Math.Floor(half_cl);
                        break;
                    default:
                        context.NachaljnyyNomerVyborki = 1;
                        context.MaksimaljnoeKolichestvoKontragentov = 0;
                        break;
                }
            // Строка о выполнении
            context.Otchetovypolnenii = string.Concat(context.Otchetovypolnenii, "Режим запуска обновления - ", context.RezhimZapuskaObnovleniya, "\n");
            context.Otchetovypolnenii = string.Concat(context.Otchetovypolnenii, "Всего контрагентов найдено - ", context.VsegoKontragentov.ToString(), "\n");
            context.Otchetovypolnenii = string.Concat(context.Otchetovypolnenii, "Начальный номер выборки - ", context.NachaljnyyNomerVyborki.ToString(), "\n");
            context.Otchetovypolnenii = string.Concat(context.Otchetovypolnenii, "Размер выборки - ", context.MaksimaljnoeKolichestvoKontragentov.ToString(), "\n");
            // Обнуление
            context.StrokObrabotano = 0;
            context.KolichestvoKontragentovSIzm = 0;
            // проходимся по массиву контрагентов
            foreach (var cl in contractors)
            {
                try
                {
                    if (context.StrokObrabotano >= (context.NachaljnyyNomerVyborki + context.MaksimaljnoeKolichestvoKontragentov) - 1)
                        break;
                    else
                    {
                        context.StrokObrabotano++;
                        if (context.StrokObrabotano >= context.NachaljnyyNomerVyborki)
                        {
                            // Запомнили данные до обновления, добавили к информации о выполнении, строку о проделанной работе
                            cl.StatusOrganizaciiPredyduschiy = cl.StatusOrganizaciiDadata;
                            context.Otchetovypolnenii = string.Concat(context.Otchetovypolnenii, cl.INN, ";", cl.StatusOrganizaciiPredyduschiy, ";");
                            // Приостановили поток
                            Thread.Sleep(50);
                            // Формирование запроса к api сайта, который предоставляет информацию  об организациях
                            string query = string.Format("https://suggestions.dadata.ru/suggestions/api/4_1/rs/findById/party?query={0}", cl.INN);
                            HttpWebRequest request_dadata = (HttpWebRequest)WebRequest.Create(query);
                            // Необходимые параметры для подключения
                            request_dadata.Headers.Add(HttpRequestHeader.Authorization, "Token 426g432df0g504g762286q5o8e7045knetf7w955");
                            request_dadata.Method = "GET";
                            request_dadata.Accept = "application/json";
                            // Делаем запрос, получаем поток байт, читаем
                            WebResponse response_dadata = request_dadata.GetResponse();
                            using (StreamReader sr_dadata = new StreamReader(response_dadata.GetResponseStream()))
                            {
                                string resultQuery = sr_dadata.ReadToEnd();
                                // Получение данных из JSON и обновление их в контрагентах, сохранение 
                                JsonDataOrganization jsonDataOrganization = JsonConvert.DeserializeObject<JsonDataOrganization>(resultQuery);
                                var data = jsonDataOrganization.suggestions[0];
                                cl.StatusOrganizaciiDadata = data.data.state.status;
                                cl.OPFOrganizaciiDadata = data.data.opf.full;
                                cl.Save();
                            }
                            response_dadata.Close();
                            // Добавили к информации о выполнении, строку о проделанной работе
                            context.Otchetovypolnenii = string.Concat(context.Otchetovypolnenii, cl.StatusOrganizaciiDadata, ";", cl.OPFOrganizaciiDadata, "\n");
                            // Если контрагент изменился прибавляем счетчик
                            if (cl.StatusOrganizaciiPredyduschiy != cl.StatusOrganizaciiDadata)
                                context.KolichestvoKontragentovSIzm++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Если ошибка фиксируем ее в отчет
                    string st_temp = ex.Message;
                    st_temp = Regex.Replace(st_temp, @"[ \r\n\t\s]", " ");
                    context.Otchetovypolnenii = string.Concat(context.Otchetovypolnenii, ";", st_temp, "\n");
                }
            }
            // Генерация названия для файла логов
            string log_file_path = @"D:\ElmaLogs_dadata\";
            string log_file_name = string.Concat(log_file_path, "log_dadata_", DateTime.Now.ToString("yy-MM-dd_HH-mm-ss"), ".txt");
            try
            {
                // Записываем в файл .TXT информацию из отчета
                StreamWriter sw = new StreamWriter(log_file_name);
                sw.Write(context.Otchetovypolnenii);
                sw.Close();                                                                                                                                                                                                                                                                                                                                                                                     
            }
            catch (Exception ex)
            {
                // Если ошибка фиксируем в отчет
                string st_temp = ex.Message;
                st_temp = Regex.Replace(st_temp, @"[ \r\n\t\s]", " ");
                context.Otchetovypolnenii = string.Concat(context.Otchetovypolnenii, st_temp);
            }
        }
        public class JsonDataOrganization
        {
            public List<Suggestion> suggestions;
        }
        public class Suggestion
        {
            public DataOrganization data;
        }
        public class DataOrganization
        {
            public DataState state;
            public DataOpf opf;
        }
        public class DataState
        {
            public string status;
        }
        public class DataOpf
        {
            public string full;
        }
        */
    }
}