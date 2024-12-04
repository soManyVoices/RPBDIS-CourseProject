using SchoolWeb.DataLayer.Models;

namespace SchoolWeb.DataLayer.Data
{
    public static class DbInitializer
    {
        public static void Initialize(SchoolContext db)
        {
            db.Database.EnsureCreated();

            // Проверка, есть ли данные в базе
            if (db.Students.Any())
            {
                return; // База данных уже инициализирована
            }

            Random rand = new(1);

            // Списки имен, фамилий и отчеств
            string[] firstNames = { "Иван", "Дмитрий", "Алексей", "Сергей", "Анна", "Мария", "Екатерина", "Ольга", "Наталья", "Татьяна" };
            string[] lastNames = { "Иванов", "Петров", "Сидоров", "Кузнецов", "Смирнов", "Морозов", "Попов", "Васильев", "Михайлов", "Новиков" };
            string[] middleNames = { "Александрович", "Викторович", "Сергеевич", "Михайлович", "Алексеевич", "Иванович", "Петровна", "Сергеевна", "Владимировна", "Дмитриевна" };

            // Вставка данных для типов классов
            var classTypes = new List<ClassType>
            {
                new ClassType { Name = "Начальный", Description = "Описание типа Начальный" },
                new ClassType { Name = "Средний", Description = "Описание типа Средний" },
                new ClassType { Name = "Старший", Description = "Описание типа Старший" },
                new ClassType { Name = "Подготовительный", Description = "Описание типа Подготовительный" },
                new ClassType { Name = "Младший", Description = "Описание типа Младший" },
                new ClassType { Name = "Технический", Description = "Описание типа Технический" },
                new ClassType { Name = "Гуманитарный", Description = "Описание типа Гуманитарный" },
                new ClassType { Name = "Естественный", Description = "Описание типа Естественный" },
                new ClassType { Name = "Математический", Description = "Описание типа Математический" },
                new ClassType { Name = "Художественный", Description = "Описание типа Художественный" }
            };

            // Вставка данных в базу
            db.ClassTypes.AddRange(classTypes);
            db.SaveChanges();


            // Вставка данных для сотрудников
            string[] employeeFirstNames = { "Андрей", "Виктор", "Олег", "Татьяна", "Наталья" };
            string[] employeeLastNames = { "Ковалев", "Романов", "Морозов", "Иванова", "Смирнова" };
            string[] employeeMiddleNames = { "Андреевич", "Викторович", "Олегович", "Татьяновна", "Натальевна" };

            // Создаем позиции, чтобы связать их с сотрудниками
            string[] positionNames = { "Учитель математики", "Учитель русского языка", "Учитель физики", "Учитель истории", "Учитель химии" };
            for (int i = 0; i < 500; i++) // Генерируем 500 позиций для сотрудников
            {
                db.Positions.Add(new Position
                {
                    Name = positionNames[rand.Next(positionNames.Length)],
                    Description = $"Описание должности {positionNames[rand.Next(positionNames.Length)]}",
                    Salary = 50000 + rand.Next(10000) // Зарплата от 50,000 до 60,000
                });
            }
            db.SaveChanges();

            var positionIds = db.Positions.Select(p => p.PositionId).ToList();

            // Генерация сотрудников
            for (int i = 0; i < 500; i++) // Генерируем 500 сотрудников
            {
                db.Employees.Add(new Employee
                {
                    FirstName = employeeFirstNames[rand.Next(employeeFirstNames.Length)],
                    LastName = employeeLastNames[rand.Next(employeeLastNames.Length)],
                    MiddleName = employeeMiddleNames[rand.Next(employeeMiddleNames.Length)],
                    PositionId = positionIds[rand.Next(positionIds.Count)]
                });
            }
            db.SaveChanges();

            // Вставка данных для предметов
            string[] subjects = { "Математика", "Русский язык", "История", "Физика", "Химия" };
            for (int i = 0; i < 500; i++) // Генерируем 500 предметов
            {
                db.Subjects.Add(new Subject
                {
                    Name = subjects[rand.Next(subjects.Length)],
                    Description = $"Описание предмета {subjects[rand.Next(subjects.Length)]}",
                    EmployeeId = rand.Next(1, 501) // Привязываем к сотрудникам
                });
            }
            db.SaveChanges();

            var classTypeIds = db.ClassTypes.Select(ct => ct.ClassTypeId).ToList();

            // Генерация данных для классов
            string[] classNames = { "1А", "2Б", "3В", "4Г", "5Д" };
            string[] teacherNames = { "Иванова Т.В.", "Петров А.В.", "Сидорова Е.Г.", "Кузнецов Н.Д.", "Морозова Л.В." };

            for (int i = 0; i < 500; i++) // Генерируем 500 классов
            {
                db.Classes.Add(new Class
                {
                    Name = classNames[rand.Next(classNames.Length)],
                    ClassTeacher = teacherNames[rand.Next(teacherNames.Length)],
                    ClassTypeId = classTypeIds[rand.Next(classTypeIds.Count)],
                    StudentCount = rand.Next(15, 35),
                    YearCreated = DateTime.Now.Year - rand.Next(1, 10)
                });
            }
            db.SaveChanges();

            // Вставка данных для студентов
            string[] fatherNames = { "Александр", "Михаил", "Павел", "Сергей", "Владимир" };
            string[] motherNames = { "Елена", "Ольга", "Татьяна", "Ирина", "Наталья" };
            string[] genders = { "Мужской", "Женский" };

            for (int i = 0; i < 20000; i++) // Генерируем 20000 студентов
            {
                db.Students.Add(new Student
                {
                    FirstName = firstNames[rand.Next(firstNames.Length)],
                    LastName = lastNames[rand.Next(lastNames.Length)],
                    MiddleName = middleNames[rand.Next(middleNames.Length)],
                    Gender = genders[rand.Next(genders.Length)],
                    DateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-rand.Next(6, 18))),
                    Address = $"ул. Примерная, д. {rand.Next(1, 100)}, кв. {rand.Next(1, 50)}",
                    FatherFirstName = fatherNames[rand.Next(fatherNames.Length)],
                    FatherLastName = lastNames[rand.Next(lastNames.Length)],
                    FatherMiddleName = middleNames[rand.Next(middleNames.Length)],
                    MotherFirstName = motherNames[rand.Next(motherNames.Length)],
                    MotherLastName = lastNames[rand.Next(lastNames.Length)],
                    MotherMiddleName = middleNames[rand.Next(middleNames.Length)],
                    AdditionalInfo = $"Интересы: Спорт, Музыка, Чтение. Примечание {i}.",
                    ClassId = rand.Next(1, 6)
                });
            }
            db.SaveChanges();

            // Массив с русскими днями недели
            string[] DaysOfWeek = new string[]
            {
                "Понедельник",
                "Вторник",
                "Среда",
                "Четверг",
                "Пятница",
                "Суббота",
                "Воскресенье"
            };

            // Вставка данных для расписания
            for (int i = 0; i < 20000; i++) // Генерируем 20000 расписаний
            {
                db.Schedules.Add(new Schedule
                {
                    ClassId = rand.Next(1, 6),
                    SubjectId = rand.Next(1, subjects.Length + 1),
                    Date = DateOnly.FromDateTime(DateTime.Today.AddDays(rand.Next(-30, 30))),
                    DayOfWeek = DaysOfWeek[rand.Next(7)], // Выбор случайного дня недели на русском
                    StartTime = TimeOnly.FromDateTime(DateTime.Today.AddHours(rand.Next(8, 15))),
                    EndTime = TimeOnly.FromDateTime(DateTime.Today.AddHours(rand.Next(16, 18)))
                });
            }
            db.SaveChanges();

        }

    }
}
