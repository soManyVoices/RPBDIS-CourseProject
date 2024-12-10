using SchoolWeb.DataLayer.Models;

namespace SchoolWeb.Tests
{
    internal class TestDataHelper
    {
        public static List<ClassType> GetFakeClassTypesList()
        {
            return new List<ClassType>
            {
                new ClassType
                {
                    ClassTypeId = 1,
                    Name = "Начальная школа",
                    Description = "Классы с 1 по 4. Основной упор на базовые предметы."
                },
                new ClassType
                {
                    ClassTypeId = 2,
                    Name = "Средняя школа",
                    Description = "Классы с 5 по 9. Изучение предметов расширенного уровня."
                },
                new ClassType
                {
                    ClassTypeId = 3,
                    Name = "Старшая школа",
                    Description = "Классы с 10 по 11. Профильная подготовка для экзаменов."
                }
            };
        }

        public static List<Class> GetFakeClassesList()
        {
            var classTypes = GetFakeClassTypesList();
            return new List<Class>
            {
                new Class
                {
                    ClassId = 1,
                    Name = "1А",
                    ClassTeacher = "Иван Иванович Петров",
                    ClassTypeId = 1,
                    StudentCount = 25,
                    YearCreated = 2005,
                    ClassType = classTypes.SingleOrDefault(m => m.ClassTypeId == 1),
                },
                new Class
                {
                    ClassId = 2,
                    Name = "2Б",
                    ClassTeacher = "Мария Сергеевна Смирнова",
                    ClassTypeId = 2,
                    StudentCount = 30,
                    YearCreated = 2010,
                    ClassType = classTypes.SingleOrDefault(m => m.ClassTypeId == 2),
                },
                new Class
                {
                    ClassId = 3,
                    Name = "5В",
                    ClassTeacher = "Алексей Павлович Кузьмин",
                    ClassTypeId = 2,
                    StudentCount = 28,
                    YearCreated = 2010,
                    ClassType = classTypes.SingleOrDefault(m => m.ClassTypeId == 3)
                },
            };
        }
        public static List<Subject> GetFakeSubjectsList()
        {
            // Получаем список сотрудников
            var employees = GetFakeEmployeesList();

            return new List<Subject>
            {
                    new Subject
                    {
                        SubjectId = 1,
                        Name = "Математика",
                        Description = "Изучение чисел, функций и уравнений.",
                        EmployeeId = 1,
                        Employee = employees.SingleOrDefault(e => e.EmployeeId == 1)
                    },
                    new Subject
                    {
                        SubjectId = 2,
                        Name = "Русский язык",
                        Description = "Изучение грамматики и орфографии русского языка.",
                        EmployeeId = 2,
                        Employee = employees.SingleOrDefault(e => e.EmployeeId == 2)
                    },
                    new Subject
                    {
                        SubjectId = 3,
                        Name = "Физика",
                        Description = "Основы механики, электричества и других физических явлений.",
                        EmployeeId = 3,
                        Employee = employees.SingleOrDefault(e => e.EmployeeId == 3)
                    },
                    new Subject
                    {
                        SubjectId = 4,
                        Name = "Химия",
                        Description = "Изучение свойств веществ и химических реакций.",
                        EmployeeId = 4,
                        Employee = employees.SingleOrDefault(e => e.EmployeeId == 4)
                    }
            };
        }

        public static List<Employee> GetFakeEmployeesList()
        {
            // Получаем список должностей
            var positions = GetFakePositionsList();

            return new List<Employee>
                {
                    new Employee
                    {
                        EmployeeId = 1,
                        FirstName = "Иван",
                        LastName = "Иванов",
                        MiddleName = "Петрович",
                        PositionId = 1,
                        Position = positions.SingleOrDefault(p => p.PositionId == 1)
                    },
                    new Employee
                    {
                        EmployeeId = 2,
                        FirstName = "Мария",
                        LastName = "Петрова",
                        MiddleName = "Александровна",
                        PositionId = 2,
                        Position = positions.SingleOrDefault(p => p.PositionId == 2)
                    },
                    new Employee
                    {
                        EmployeeId = 3,
                        FirstName = "Алексей",
                        LastName = "Сидоров",
                        MiddleName = "Иванович",
                        PositionId = 3,
                        Position = positions.SingleOrDefault(p => p.PositionId == 3)
                    },
                    new Employee
                    {
                        EmployeeId = 4,
                        FirstName = "Ольга",
                        LastName = "Кузнецова",
                        MiddleName = "Сергеевна",
                        PositionId = 4,
                        Position = positions.SingleOrDefault(p => p.PositionId == 4)
                    }
                };
        }

        public static List<Position> GetFakePositionsList()
        {
            return new List<Position>
            {
                new Position
                {
                    PositionId = 1,
                    Name = "Учитель математики",
                    Description = "Отвечает за преподавание математики и проведение контрольных работ.",
                    Salary = 50000
                },
                new Position
                {
                    PositionId = 2,
                    Name = "Учитель русского языка",
                    Description = "Проводит занятия по русскому языку, включая грамматику и литературу.",
                    Salary = 48000
                },
                new Position
                {
                    PositionId = 3,
                    Name = "Учитель физики",
                    Description = "Преподает основы механики, электричества и других физических явлений.",
                    Salary = 55000
                },
                new Position
                {
                    PositionId = 4,
                    Name = "Учитель химии",
                    Description = "Изучение химических реакций, свойств веществ и лабораторных экспериментов.",
                    Salary = 53000
                }
            };
        }

        public static List<Schedule> GetFakeSchedulesList()
        {
            int schedulesNumber = 20; // Количество записей в расписании
            var classes = GetFakeClassesList(); // Список классов
            var subjects = GetFakeSubjectsList(); // Список предметов
            Random rand = new Random(); // Генератор случайных чисел

            List<Schedule> schedules = new List<Schedule>(); // Список для хранения расписания

            for (int scheduleId = 1; scheduleId <= schedulesNumber; scheduleId++)
            {
                int classId = rand.Next(1, classes.Count + 1); // Случайный выбор класса
                int subjectId = rand.Next(1, subjects.Count + 1); // Случайный выбор предмета

                // Генерация случайной даты в пределах ближайших недель
                DateTime today = DateTime.Now.Date;
                DateOnly date = DateOnly.FromDateTime(today.AddDays(rand.Next(-10, 10)));

                // Случайное время начала и окончания занятий
                TimeOnly startTime = new TimeOnly(rand.Next(8, 16), 0); // Время с 08:00 до 15:59
                TimeOnly endTime = startTime.AddMinutes(45); // Продолжительность урока 45 минут

                schedules.Add(new Schedule
                {
                    ScheduleId = scheduleId,
                    Date = date,
                    DayOfWeek = date.DayOfWeek.ToString(),
                    ClassId = classId,
                    SubjectId = subjectId,
                    StartTime = startTime,
                    EndTime = endTime,
                    Class = classes.SingleOrDefault(c => c.ClassId == classId),
                    Subject = subjects.SingleOrDefault(s => s.SubjectId == subjectId)
                });
            }

            return schedules;
        }

        public static List<Student> GetFakeStudentsList()
        {
            var classes = GetFakeClassesList(); // Получение списка классов
            Random rand = new Random(); // Генератор случайных чисел
            List<Student> students = new List<Student>(); // Список для хранения студентов

            // Пример списка имен и фамилий
            string[] maleFirstNames = { "Иван", "Алексей", "Дмитрий", "Николай", "Сергей" };
            string[] femaleFirstNames = { "Анна", "Мария", "Екатерина", "Ольга", "Елена" };
            string[] lastNames = { "Иванов", "Петров", "Сидоров", "Кузнецов", "Смирнов" };

            for (int studentId = 1; studentId <= 30; studentId++)
            {
                // Случайное определение пола
                bool isMale = rand.Next(0, 2) == 0;

                // Генерация данных
                string firstName = isMale
                    ? maleFirstNames[rand.Next(maleFirstNames.Length)]
                    : femaleFirstNames[rand.Next(femaleFirstNames.Length)];
                string lastName = lastNames[rand.Next(lastNames.Length)];
                string middleName = isMale ? "Иванович" : "Ивановна";

                string fatherFirstName = maleFirstNames[rand.Next(maleFirstNames.Length)];
                string fatherLastName = lastName;
                string fatherMiddleName = "Алексеевич";

                string motherFirstName = femaleFirstNames[rand.Next(femaleFirstNames.Length)];
                string motherLastName = lastName;
                string motherMiddleName = "Петровна";

                int? classId = rand.Next(0, 2) == 0 ? (int?)null : rand.Next(1, classes.Count + 1);

                students.Add(new Student
                {
                    StudentId = studentId,
                    FirstName = firstName,
                    LastName = lastName,
                    MiddleName = middleName,
                    DateOfBirth = DateOnly.FromDateTime(DateTime.Now.AddYears(-rand.Next(7, 18))),
                    Gender = isMale ? "Мужской" : "Женский",
                    Address = $"ул. Школьная, д. {rand.Next(1, 100)}",
                    FatherFirstName = fatherFirstName,
                    FatherLastName = fatherLastName,
                    FatherMiddleName = fatherMiddleName,
                    MotherFirstName = motherFirstName,
                    MotherLastName = motherLastName,
                    MotherMiddleName = motherMiddleName,
                    ClassId = classId,
                    AdditionalInfo = rand.Next(0, 2) == 0 ? null : "Отличник",
                    Class = classes.SingleOrDefault(c => c.ClassId == classId)
                });
            }

            return students;
        }
    }
}