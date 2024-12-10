using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq.EntityFrameworkCore;
using SchoolWeb.DataLayer.Data;
using SchoolWeb.DataLayer.Models;
using SchoolWeb.ViewModels.SubjectsViewModel;
using SchoolWeb.ViewModels.SortStates;
using SchoolWeb.ViewModels.EmployeesViewModel;

namespace SchoolWeb.Tests
{
    public class SubjectsControllerTests
    {
        [Fact]
        public async Task GetSubjectsList()
        {
            // Arrange
            var subjects = TestDataHelper.GetFakeSubjectsList();
            var schoolContextMock = new Mock<SchoolContext>();
            schoolContextMock.Setup(x => x.Subjects).ReturnsDbSet(subjects);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация SubjectsController
            var subjectsController = new SubjectsController(
                schoolContextMock.Object,
                configurationMock.Object
            );

            // Act
            var result = await subjectsController.Index(new FilterSubjectViewModel(), SubjectSortState.No, 1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult);
            var model = Assert.IsAssignableFrom<SubjectViewModel>(viewResult.ViewData.Model);
            Assert.Equal(4, model.Subjects.Count());
        }

        [Fact]
        public async Task GetSubject()
        {
            // Arrange
            var subjects = TestDataHelper.GetFakeSubjectsList();
            var schoolContextMock = new Mock<SchoolContext>();
            schoolContextMock.Setup(x => x.Subjects).ReturnsDbSet(subjects);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация SubjectsController
            var subjectsController = new SubjectsController(
                schoolContextMock.Object,
                configurationMock.Object
            );

            // Act
            var notFoundResult = await subjectsController.Details(6);
            var foundResult = await subjectsController.Details(1);

            // Assert
            Assert.IsType<NotFoundResult>(notFoundResult);
            Assert.IsType<ViewResult>(foundResult);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_GivenInvalidModel()
        {
            // Arrange
            var subjects = TestDataHelper.GetFakeSubjectsList();
            var schoolContextMock = new Mock<SchoolContext>();
            schoolContextMock.Setup(x => x.Subjects).ReturnsDbSet(subjects);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация SubjectsController
            var subjectsController = new SubjectsController(
                schoolContextMock.Object,
                configurationMock.Object
            );

            subjectsController.ModelState.AddModelError("error", "some error");

            // Act
            var result = await subjectsController.Create(subject: null);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Create_ReturnsARedirectAndCreate_WhenModelStateIsValid()
        {
            // Arrange
            var subjects = TestDataHelper.GetFakeSubjectsList();
            var employees = TestDataHelper.GetFakeEmployeesList();
            var schoolContextMock = new Mock<SchoolContext>();
            schoolContextMock.Setup(x => x.Subjects).ReturnsDbSet(subjects);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация SubjectsController
            var subjectsController = new SubjectsController(
                schoolContextMock.Object,
                configurationMock.Object
            );

            // Arrange
            var subject = new Subject
            {
                SubjectId = 1,
                Name = "Математика",
                Description = "Изучение чисел, функций и уравнений.",
                EmployeeId = 1,
                Employee = employees.SingleOrDefault(e => e.EmployeeId == 1)
            };

            // Act
            var result = await subjectsController.Create(subject);

            // Assert: проверка перенаправления на действие Index
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            schoolContextMock.Verify();
        }

        [Fact]
        public async Task Edit_ReturnsNotFound()
        {
            // Arrange
            var subjects = TestDataHelper.GetFakeSubjectsList();
            var employees = TestDataHelper.GetFakeEmployeesList();
            var schoolContextMock = new Mock<SchoolContext>();
            schoolContextMock.Setup(x => x.Subjects).ReturnsDbSet(subjects);
            schoolContextMock.Setup(x => x.Employees).ReturnsDbSet(employees);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация SubjectsController
            var subjectsController = new SubjectsController(
                schoolContextMock.Object,
                configurationMock.Object
            );

            // Act
            var notFoundResult = await subjectsController.Edit(100);
            var foundResult = await subjectsController.Edit(3);

            // Assert
            Assert.IsType<NotFoundResult>(notFoundResult);
            Assert.IsType<ViewResult>(foundResult);
        }

        [Fact]
        public async Task Edit_ReturnsBadRequest_GivenInvalidModel()
        {
            // Arrange
            var subjects = TestDataHelper.GetFakeSubjectsList();
            var employees = TestDataHelper.GetFakeEmployeesList();
            var schoolContextMock = new Mock<SchoolContext>();
            schoolContextMock.Setup(x => x.Subjects).ReturnsDbSet(subjects);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация SubjectsController
            var subjectsController = new SubjectsController(
                schoolContextMock.Object,
                configurationMock.Object
            );

            // Arrange
            var subject = new Subject
            {
                SubjectId = 1,
                Name = "Математика",
                Description = "Изучение чисел, функций и уравнений.",
                EmployeeId = 1,
                Employee = employees.SingleOrDefault(e => e.EmployeeId == 1)
            };

            var result = await subjectsController.Edit(4, subject);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_ReturnsARedirectAndCreate_WhenModelStateIsValid()
        {
            // Arrange
            var subjects = TestDataHelper.GetFakeSubjectsList();
            var employees = TestDataHelper.GetFakeEmployeesList();
            var schoolContextMock = new Mock<SchoolContext>();
            schoolContextMock.Setup(x => x.Subjects).ReturnsDbSet(subjects);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация SubjectsController
            var subjectsController = new SubjectsController(
                schoolContextMock.Object,
                configurationMock.Object
            );

            // Arrange
            var subject = new Subject
            {
                SubjectId = 6,
                Name = "Математика",
                Description = "Изучение чисел, функций и уравнений.",
                EmployeeId = 1,
                Employee = employees.SingleOrDefault(e => e.EmployeeId == 1)
            };

            var result = await subjectsController.Edit(6, subject);

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
            var subjects = TestDataHelper.GetFakeSubjectsList();
            var schoolContextMock = new Mock<SchoolContext>();
            schoolContextMock.Setup(x => x.Subjects).ReturnsDbSet(subjects);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация SubjectsController
            var subjectsController = new SubjectsController(
                schoolContextMock.Object,
                configurationMock.Object
            );

            // Act
            var notFoundResult = await subjectsController.Delete(6);
            var foundResult = await subjectsController.Delete(3);

            // Assert
            Assert.IsType<NotFoundResult>(notFoundResult);
            Assert.IsType<ViewResult>(foundResult);
        }

        [Fact]
        public async Task Delete_ReturnsARedirectAndDelete()
        {
            // Arrange
            var subjects = TestDataHelper.GetFakeSubjectsList();
            var schoolContextMock = new Mock<SchoolContext>();
            schoolContextMock.Setup(x => x.Subjects).ReturnsDbSet(subjects);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация SubjectsController
            var subjectsController = new SubjectsController(
                schoolContextMock.Object,
                configurationMock.Object
            );

            // Act
            var result = await subjectsController.DeleteConfirmed(3);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            schoolContextMock.Verify();
        }
    }
}
