using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq.EntityFrameworkCore;
using SchoolWeb.DataLayer.Data;
using SchoolWeb.DataLayer.Models;
using SchoolWeb.ViewModels.StudentViewModel;
using SchoolWeb.ViewModels.SortStates;
using SchoolWeb.ViewModels.ClassesViewModel;

namespace SchoolWeb.Tests
{
    public class StudentsControllerTests
    {
        [Fact]
        public async Task GetStudentsList()
        {
            // Arrange
            var students = TestDataHelper.GetFakeStudentsList();
            var classes = TestDataHelper.GetFakeClassesList();
            var subjects = TestDataHelper.GetFakeSubjectsList();
            var schoolContextMock = new Mock<SchoolContext>();
            schoolContextMock.Setup(x => x.Students).ReturnsDbSet(students);
            schoolContextMock.Setup(x => x.Classes).ReturnsDbSet(classes);
            schoolContextMock.Setup(x => x.Subjects).ReturnsDbSet(subjects);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("20");

            // Инициализация StudentsController
            var studentsController = new StudentsController(
                schoolContextMock.Object,
                configurationMock.Object
            );

            // Act
            var result = await studentsController.Index(new FilterStudentViewModel(), StudentSortState.No, 1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult);
            var model = Assert.IsAssignableFrom<StudentViewModel>(viewResult.ViewData.Model);
            Assert.Equal(TestDataHelper.GetFakeSchedulesList().Count, model.Students.Count());
            Assert.NotNull(model.Students);
        }

        [Fact]
        public async Task GetStudent()
        {
            // Arrange
            var students = TestDataHelper.GetFakeStudentsList();
            var schoolContextMock = new Mock<SchoolContext>();
            schoolContextMock.Setup(x => x.Students).ReturnsDbSet(students);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("20");

            // Инициализация StudentsController
            var studentsController = new StudentsController(
                schoolContextMock.Object,
                configurationMock.Object
            );

            // Act
            var notFoundResult = await studentsController.Details(100);
            var foundResult = await studentsController.Details(1);

            // Assert
            Assert.IsType<NotFoundResult>(notFoundResult);
            Assert.IsType<ViewResult>(foundResult);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_GivenInvalidModel()
        {
            // Arrange
            var students = TestDataHelper.GetFakeStudentsList();
            var schoolContextMock = new Mock<SchoolContext>();
            schoolContextMock.Setup(x => x.Students).ReturnsDbSet(students);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("20");

            // Инициализация StudentsController
            var studentsController = new StudentsController(
                schoolContextMock.Object,
                configurationMock.Object
            );

            studentsController.ModelState.AddModelError("error", "some error");

            // Act
            var result = await studentsController.Create(student: null);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Create_ReturnsARedirectAndCreate_WhenModelStateIsValid()
        {
            // Arrange
            var students = TestDataHelper.GetFakeStudentsList();
            var classes = TestDataHelper.GetFakeClassesList();
            var schoolContextMock = new Mock<SchoolContext>();
            schoolContextMock.Setup(x => x.Students).ReturnsDbSet(students);
            schoolContextMock.Setup(x => x.Classes).ReturnsDbSet(classes);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("20");

            // Инициализация StudentsController
            var studentsController = new StudentsController(
                schoolContextMock.Object,
                configurationMock.Object
            );

            // Arrange
            var student = new Student
            {
                StudentId = 1,
                FirstName = "Иван",
                LastName = "Иванов",
                MiddleName = "Иванович",
                DateOfBirth = new DateOnly(2010, 5, 15),
                Gender = "Мужской",
                Address = "ул. Ленина, д. 10",
                FatherFirstName = "Сергей",
                FatherLastName = "Иванов",
                FatherMiddleName = "Александрович",
                MotherFirstName = "Мария",
                MotherLastName = "Иванова",
                MotherMiddleName = "Викторовна",
                ClassId = 2,
                AdditionalInfo = "Отличник",
                Class = classes.SingleOrDefault(m => m.ClassId == 1),
            };

            // Act
            var result = await studentsController.Create(student);

            // Assert: проверка перенаправления на действие Index
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            schoolContextMock.Verify();
        }

        [Fact]
        public async Task Edit_ReturnsNotFound()
        { // Arrange
            var students = TestDataHelper.GetFakeStudentsList();
            var classes = TestDataHelper.GetFakeClassesList();
            var schoolContextMock = new Mock<SchoolContext>();
            schoolContextMock.Setup(x => x.Students).ReturnsDbSet(students);
            schoolContextMock.Setup(x => x.Classes).ReturnsDbSet(classes);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("20");

            // Инициализация StudentsController
            var studentsController = new StudentsController(
                schoolContextMock.Object,
                configurationMock.Object
            );

            // Act
            var notFoundResult = await studentsController.Edit(100);
            var foundResult = await studentsController.Edit(3);

            // Assert
            Assert.IsType<NotFoundResult>(notFoundResult);
            Assert.IsType<ViewResult>(foundResult);
        }

        [Fact]
        public async Task Edit_ReturnsBadRequest_GivenInvalidModel()
        {
            // Arrange
            var students = TestDataHelper.GetFakeStudentsList();
            var classes = TestDataHelper.GetFakeClassesList();
            var schoolContextMock = new Mock<SchoolContext>();
            schoolContextMock.Setup(x => x.Students).ReturnsDbSet(students);
            schoolContextMock.Setup(x => x.Classes).ReturnsDbSet(classes);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("20");

            // Инициализация StudentsController
            var studentsController = new StudentsController(
                schoolContextMock.Object,
                configurationMock.Object
            );

            // Arrange
            var student = new Student
            {
                StudentId = 1,
                FirstName = "Иван",
                LastName = "Иванов",
                MiddleName = "Иванович",
                DateOfBirth = new DateOnly(2010, 5, 15),
                Gender = "Мужской",
                Address = "ул. Ленина, д. 10",
                FatherFirstName = "Сергей",
                FatherLastName = "Иванов",
                FatherMiddleName = "Александрович",
                MotherFirstName = "Мария",
                MotherLastName = "Иванова",
                MotherMiddleName = "Викторовна",
                ClassId = 2,
                AdditionalInfo = "Отличник",
                Class = classes.SingleOrDefault(m => m.ClassId == 1),
            };

            var result = await studentsController.Edit(4, student);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_ReturnsARedirectAndCreate_WhenModelStateIsValid()
        {
            // Arrange
            var students = TestDataHelper.GetFakeStudentsList();
            var classes = TestDataHelper.GetFakeClassesList();
            var schoolContextMock = new Mock<SchoolContext>();
            schoolContextMock.Setup(x => x.Students).ReturnsDbSet(students);
            schoolContextMock.Setup(x => x.Classes).ReturnsDbSet(classes);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("20");

            // Инициализация StudentsController
            var studentsController = new StudentsController(
                schoolContextMock.Object,
                configurationMock.Object
            );

            // Arrange
            var student = new Student
            {
                StudentId = 6,
                FirstName = "Иван",
                LastName = "Иванов",
                MiddleName = "Иванович",
                DateOfBirth = new DateOnly(2010, 5, 15),
                Gender = "Мужской",
                Address = "ул. Ленина, д. 10",
                FatherFirstName = "Сергей",
                FatherLastName = "Иванов",
                FatherMiddleName = "Александрович",
                MotherFirstName = "Мария",
                MotherLastName = "Иванова",
                MotherMiddleName = "Викторовна",
                ClassId = 2,
                AdditionalInfo = "Отличник",
                Class = classes.SingleOrDefault(m => m.ClassId == 1),
            };

            var result = await studentsController.Edit(6, student);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            schoolContextMock.Verify();
        }

        [Fact]
        public async Task Delete_ReturnsNotFound()
        {
            // Arrange
            var students = TestDataHelper.GetFakeStudentsList();
            var schoolContextMock = new Mock<SchoolContext>();
            schoolContextMock.Setup(x => x.Students).ReturnsDbSet(students);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("20");

            // Инициализация StudentsController
            var studentsController = new StudentsController(
                schoolContextMock.Object,
                configurationMock.Object
            );

            // Act
            var notFoundResult = await studentsController.Delete(100);
            var foundResult = await studentsController.Delete(3);

            // Assert
            Assert.IsType<NotFoundResult>(notFoundResult);
            Assert.IsType<ViewResult>(foundResult);
        }

        [Fact]
        public async Task Delete_ReturnsARedirectAndDelete()
        {
            // Arrange
            var students = TestDataHelper.GetFakeStudentsList();
            var schoolContextMock = new Mock<SchoolContext>();
            schoolContextMock.Setup(x => x.Students).ReturnsDbSet(students);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("20");

            // Инициализация StudentsController
            var studentsController = new StudentsController(
                schoolContextMock.Object,
                configurationMock.Object
            );

            // Act
            var result = await studentsController.DeleteConfirmed(3);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            schoolContextMock.Verify();
        }
    }
}
