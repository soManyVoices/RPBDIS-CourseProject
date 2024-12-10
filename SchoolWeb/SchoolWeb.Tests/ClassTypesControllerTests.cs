using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq.EntityFrameworkCore;
using SchoolWeb.DataLayer.Data;
using SchoolWeb.DataLayer.Models;
using SchoolWeb.ViewModels.ClassTypesViewModel;
using SchoolWeb.ViewModels.PositionsViewModel;
using SchoolWeb.ViewModels.SortStates;

namespace SchoolWeb.Tests
{
    public class ClassTypesControllerTests
    {
        [Fact]
        public async Task GetClassTypesList()
        {
            // Arrange
            var classTypes = TestDataHelper.GetFakeClassTypesList();
            var schoolContextMock = new Mock<SchoolContext>();
            schoolContextMock.Setup(x => x.ClassTypes).ReturnsDbSet(classTypes);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация ClassTypesController
            var classTypesController = new ClassTypesController(
                schoolContextMock.Object,
                configurationMock.Object
            );

            // Act
            var result = await classTypesController.Index(new FilterClassTypeViewModel(), ClassTypeSortState.No, 1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult);
            var model = Assert.IsAssignableFrom<ClassTypeViewModel>(viewResult.ViewData.Model);
            Assert.Equal(3, model.ClassTypes.Count());
        }

        [Fact]
        public async Task GetClassType()
        {
            // Arrange
            var classTypes = TestDataHelper.GetFakeClassTypesList();
            var schoolContextMock = new Mock<SchoolContext>();
            schoolContextMock.Setup(x => x.ClassTypes).ReturnsDbSet(classTypes);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация ClassTypesController
            var classTypesController = new ClassTypesController(
                schoolContextMock.Object,
                configurationMock.Object
            );

            // Act
            var notFoundResult = await classTypesController.Details(100);
            var foundResult = await classTypesController.Details(1);

            // Assert
            Assert.IsType<NotFoundResult>(notFoundResult);
            Assert.IsType<ViewResult>(foundResult);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_GivenInvalidModel()
        {
            // Arrange
            var classTypes = TestDataHelper.GetFakeClassTypesList();
            var schoolContextMock = new Mock<SchoolContext>();
            schoolContextMock.Setup(x => x.ClassTypes).ReturnsDbSet(classTypes);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация ClassTypesController
            var classTypesController = new ClassTypesController(
                schoolContextMock.Object,
                configurationMock.Object
            );

            classTypesController.ModelState.AddModelError("error", "some error");

            // Act
            var result = await classTypesController.Create(classType: null);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Create_ReturnsARedirectAndCreate_WhenModelStateIsValid()
        {
            // Arrange
            var classTypes = TestDataHelper.GetFakeClassTypesList();
            var schoolContextMock = new Mock<SchoolContext>();
            schoolContextMock.Setup(x => x.ClassTypes).ReturnsDbSet(classTypes);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация ClassTypesController
            var classTypesController = new ClassTypesController(
                schoolContextMock.Object,
                configurationMock.Object
            );

            // Arrange
            var classType = new ClassType
            {
                ClassTypeId = 1,
                Name = "Начальная школа",
                Description = "Классы с 1 по 4. Основной упор на базовые предметы."
            };

            // Act
            var result = await classTypesController.Create(classType);

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
            var classTypes = TestDataHelper.GetFakeClassTypesList();
            var schoolContextMock = new Mock<SchoolContext>();
            schoolContextMock.Setup(x => x.ClassTypes).ReturnsDbSet(classTypes);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация ClassTypesController
            var classTypesController = new ClassTypesController(
                schoolContextMock.Object,
                configurationMock.Object
            );

            // Act
            var notFoundResult = await classTypesController.Edit(100);
            var foundResult = await classTypesController.Edit(3);

            // Assert
            Assert.IsType<NotFoundResult>(notFoundResult);
            Assert.IsType<ViewResult>(foundResult);
        }

        [Fact]
        public async Task Edit_ReturnsBadRequest_GivenInvalidModel()
        {
            // Arrange
            var classTypes = TestDataHelper.GetFakeClassTypesList();
            var schoolContextMock = new Mock<SchoolContext>();
            schoolContextMock.Setup(x => x.ClassTypes).ReturnsDbSet(classTypes);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация ClassTypesController
            var classTypesController = new ClassTypesController(
                schoolContextMock.Object,
                configurationMock.Object
            );

            // Arrange
            var classType = new ClassType
            {
                ClassTypeId = 1,
                Name = "Начальная школа",
                Description = "Классы с 1 по 4. Основной упор на базовые предметы."
            };

            var result = await classTypesController.Edit(4, classType);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_ReturnsARedirectAndCreate_WhenModelStateIsValid()
        {
            // Arrange
            var classTypes = TestDataHelper.GetFakeClassTypesList();
            var schoolContextMock = new Mock<SchoolContext>();
            schoolContextMock.Setup(x => x.ClassTypes).ReturnsDbSet(classTypes);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация ClassTypesController
            var classTypesController = new ClassTypesController(
                schoolContextMock.Object,
                configurationMock.Object
            );

            // Arrange
            var classType = new ClassType
            {
                ClassTypeId = 4,
                Name = "Начальная школа",
                Description = "Классы с 1 по 4. Основной упор на базовые предметы."
            };

            var result = await classTypesController.Edit(4, classType);

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
            var classTypes = TestDataHelper.GetFakeClassTypesList();
            var schoolContextMock = new Mock<SchoolContext>();
            schoolContextMock.Setup(x => x.ClassTypes).ReturnsDbSet(classTypes);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация ClassTypesController
            var classTypesController = new ClassTypesController(
                schoolContextMock.Object,
                configurationMock.Object
            );

            // Act
            var notFoundResult = await classTypesController.Delete(6);
            var foundResult = await classTypesController.Delete(3);

            // Assert
            Assert.IsType<NotFoundResult>(notFoundResult);
            Assert.IsType<ViewResult>(foundResult);
        }

        [Fact]
        public async Task Delete_ReturnsARedirectAndDelete()
        {
            // Arrange
            var classTypes = TestDataHelper.GetFakeClassTypesList();
            var schoolContextMock = new Mock<SchoolContext>();
            schoolContextMock.Setup(x => x.ClassTypes).ReturnsDbSet(classTypes);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация ClassTypesController
            var classTypesController = new ClassTypesController(
                schoolContextMock.Object,
                configurationMock.Object
            );

            // Act
            var result = await classTypesController.DeleteConfirmed(3);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            schoolContextMock.Verify();
        }
    }
}
