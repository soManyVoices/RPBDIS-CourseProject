using SchoolWeb.DataLayer.Data;
using SchoolWeb.DataLayer.Models;
using SchoolWeb.ViewModels.ClassesViewModel;
using SchoolWeb.ViewModels.SortStates;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq.EntityFrameworkCore;
using System.Security.Claims;

namespace SchoolWeb.Tests
{
    public class ClassesControllerTests
    {
        [Fact]
        public async Task GetClassesList()
        {
            // Arrange
            var classes = TestDataHelper.GetFakeClassesList();
            var classTypes = TestDataHelper.GetFakeClassTypesList();
            var schoolContextMock = new Mock<SchoolContext>();
            schoolContextMock.Setup(x => x.Classes).ReturnsDbSet(classes);
            schoolContextMock.Setup(x => x.ClassTypes).ReturnsDbSet(classTypes);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация ClassesController
            var classesController = new ClassesController(
                schoolContextMock.Object,
                configurationMock.Object
            );

            // Act
            var result = await classesController.Index(new FilterClassViewModel(), ClassSortState.No, 1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult);
            var model = Assert.IsAssignableFrom<ClassViewModel>(viewResult.ViewData.Model);
            Assert.Equal(3, model.Classes.Count());
        }

        [Fact]
        public async Task GetClass()
        {
            // Arrange
            var classes = TestDataHelper.GetFakeClassesList();
            var schoolContextMock = new Mock<SchoolContext>();
            schoolContextMock.Setup(x => x.Classes).ReturnsDbSet(classes);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация ClassesController
            var classesController = new ClassesController(
                schoolContextMock.Object,
                configurationMock.Object
            );

            // Act
            var notFoundResult = await classesController.Details(6);
            var foundResult = await classesController.Details(1);

            // Assert
            Assert.IsType<NotFoundResult>(notFoundResult);
            Assert.IsType<ViewResult>(foundResult);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_GivenInvalidModel()
        {
            // Arrange
            var classes = TestDataHelper.GetFakeClassesList();
            var schoolContextMock = new Mock<SchoolContext>();
            schoolContextMock.Setup(x => x.Classes).ReturnsDbSet(classes);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация ClassesController
            var classesController = new ClassesController(
                schoolContextMock.Object,
                configurationMock.Object
            );
            classesController.ModelState.AddModelError("error", "some error");

            // Act
            var result = await classesController.Create(classItem: null);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Create_ReturnsARedirectAndCreate_WhenModelStateIsValid()
        {
            // Arrange
            var classes = TestDataHelper.GetFakeClassesList();
            var classTypes = TestDataHelper.GetFakeClassTypesList();
            var schoolContextMock = new Mock<SchoolContext>();
            schoolContextMock.Setup(x => x.Classes).ReturnsDbSet(classes);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация ClassesController
            var classesController = new ClassesController(
                schoolContextMock.Object,
                configurationMock.Object
            );

            // Arrange
            var classItem = new Class
            {
                ClassId = 1,
                Name = "1А",
                ClassTeacher = "Иван Иванович Петров",
                ClassTypeId = 1,
                StudentCount = 25,
                YearCreated = 2005,
                ClassType = classTypes.SingleOrDefault(m => m.ClassTypeId == 1),
            };

            // Act
            var result = await classesController.Create(classItem);

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
            var classes = TestDataHelper.GetFakeClassesList();
            var classTypes = TestDataHelper.GetFakeClassTypesList();
            var schoolContextMock = new Mock<SchoolContext>();
            schoolContextMock.Setup(x => x.Classes).ReturnsDbSet(classes);
            schoolContextMock.Setup(x => x.ClassTypes).ReturnsDbSet(classTypes);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация ClassesController
            var classesController = new ClassesController(
                schoolContextMock.Object,
                configurationMock.Object
            );

            // Act
            var notFoundResult = await classesController.Edit(100);
            var foundResult = await classesController.Edit(3);

            // Assert
            Assert.IsType<NotFoundResult>(notFoundResult);
            Assert.IsType<ViewResult>(foundResult);
        }

        [Fact]
        public async Task Edit_ReturnsBadRequest_GivenInvalidModel()
        {
            // Arrange
            var classes = TestDataHelper.GetFakeClassesList();
            var classTypes = TestDataHelper.GetFakeClassTypesList();
            var schoolContextMock = new Mock<SchoolContext>();
            schoolContextMock.Setup(x => x.Classes).ReturnsDbSet(classes);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация ClassesController
            var classesController = new ClassesController(
                schoolContextMock.Object,
                configurationMock.Object
            );

            // Arrange
            var classItem = new Class
            {
                ClassId = 4,
                Name = "1А",
                ClassTeacher = "Иван Иванович Петров",
                ClassTypeId = 1,
                StudentCount = 25,
                YearCreated = 2005,
                ClassType = classTypes.SingleOrDefault(m => m.ClassTypeId == 1),
            };
            var result = await classesController.Edit(1, classItem);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_ReturnsARedirectAndCreate_WhenModelStateIsValid()
        {
            // Arrange
            var classes = TestDataHelper.GetFakeClassesList();
            var classTypes = TestDataHelper.GetFakeClassTypesList();
            var schoolContextMock = new Mock<SchoolContext>();
            schoolContextMock.Setup(x => x.Classes).ReturnsDbSet(classes);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация ClassesController
            var classesController = new ClassesController(
                schoolContextMock.Object,
                configurationMock.Object
            );

            // Arrange
            var classItem = new Class
            {
                ClassId = 6,
                Name = "1А",
                ClassTeacher = "Иван Иванович Петров",
                ClassTypeId = 1,
                StudentCount = 25,
                YearCreated = 2005,
                ClassType = classTypes.SingleOrDefault(m => m.ClassTypeId == 1),
            };
            var result = await classesController.Edit(6, classItem);

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
            var classes = TestDataHelper.GetFakeClassesList();
            var schoolContextMock = new Mock<SchoolContext>();
            schoolContextMock.Setup(x => x.Classes).ReturnsDbSet(classes);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация ClassesController
            var classesController = new ClassesController(
                schoolContextMock.Object,
                configurationMock.Object
            );

            // Act
            var notFoundResult = await classesController.Delete(6);
            var foundResult = await classesController.Delete(3);

            // Assert
            Assert.IsType<NotFoundResult>(notFoundResult);
            Assert.IsType<ViewResult>(foundResult);
        }

        [Fact]
        public async Task Delete_ReturnsARedirectAndDelete()
        {
            // Arrange
            var classes = TestDataHelper.GetFakeClassesList();
            var schoolContextMock = new Mock<SchoolContext>();
            schoolContextMock.Setup(x => x.Classes).ReturnsDbSet(classes);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация ClassesController
            var classesController = new ClassesController(
                schoolContextMock.Object,
                configurationMock.Object
            );

            // Act
            var result = await classesController.DeleteConfirmed(3);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            schoolContextMock.Verify();
        }
    }
}
