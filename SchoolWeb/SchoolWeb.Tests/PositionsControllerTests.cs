using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq.EntityFrameworkCore;
using SchoolWeb.DataLayer.Data;
using SchoolWeb.DataLayer.Models;
using SchoolWeb.ViewModels.PositionsViewModel;
using SchoolWeb.ViewModels.SortStates;

namespace SchoolWeb.Tests
{
    public class PositionsControllerTests
    {
        [Fact]
        public async Task GetPositionsList()
        {
            // Arrange
            var positions = TestDataHelper.GetFakePositionsList();
            var schoolContextMock = new Mock<SchoolContext>();
            schoolContextMock.Setup(x => x.Positions).ReturnsDbSet(positions);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация PositionsController
            var positionsController = new PositionsController(
                schoolContextMock.Object,
                configurationMock.Object
            );

            // Act
            var result = await positionsController.Index(new FilterPositionViewModel(), PositionSortState.No, 1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult);
            var model = Assert.IsAssignableFrom<PositionViewModel>(viewResult.ViewData.Model);
            Assert.Equal(4, model.Positions.Count());
        }

        [Fact]
        public async Task GetPosition()
        {
            // Arrange
            var positions = TestDataHelper.GetFakePositionsList();
            var schoolContextMock = new Mock<SchoolContext>();
            schoolContextMock.Setup(x => x.Positions).ReturnsDbSet(positions);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация PositionsController
            var positionsController = new PositionsController(
                schoolContextMock.Object,
                configurationMock.Object
            );

            // Act
            var notFoundResult = await positionsController.Details(6);
            var foundResult = await positionsController.Details(1);

            // Assert
            Assert.IsType<NotFoundResult>(notFoundResult);
            Assert.IsType<ViewResult>(foundResult);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_GivenInvalidModel()
        {
            // Arrange
            var positions = TestDataHelper.GetFakePositionsList();
            var schoolContextMock = new Mock<SchoolContext>();
            schoolContextMock.Setup(x => x.Positions).ReturnsDbSet(positions);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация PositionsController
            var positionsController = new PositionsController(
                schoolContextMock.Object,
                configurationMock.Object
            );
            positionsController.ModelState.AddModelError("error", "some error");

            // Act
            var result = await positionsController.Create(position: null);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Create_ReturnsARedirectAndCreate_WhenModelStateIsValid()
        {
            // Arrange
            var positions = TestDataHelper.GetFakePositionsList();
            var schoolContextMock = new Mock<SchoolContext>();
            schoolContextMock.Setup(x => x.Positions).ReturnsDbSet(positions);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация PositionsController
            var positionsController = new PositionsController(
                schoolContextMock.Object,
                configurationMock.Object
            );

            // Arrange
            var position = new Position
            {
                PositionId = 1,
                Name = "Учитель математики",
                Description = "Отвечает за преподавание математики и проведение контрольных работ.",
                Salary = 50000
            };

            // Act
            var result = await positionsController.Create(position);

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
            var positions = TestDataHelper.GetFakePositionsList();
            var schoolContextMock = new Mock<SchoolContext>();
            schoolContextMock.Setup(x => x.Positions).ReturnsDbSet(positions);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация PositionsController
            var positionsController = new PositionsController(
                schoolContextMock.Object,
                configurationMock.Object
            );

            // Act
            var notFoundResult = await positionsController.Edit(100);
            var foundResult = await positionsController.Edit(3);

            // Assert
            Assert.IsType<NotFoundResult>(notFoundResult);
            Assert.IsType<ViewResult>(foundResult);
        }

        [Fact]
        public async Task Edit_ReturnsBadRequest_GivenInvalidModel()
        {
            // Arrange
            var positions = TestDataHelper.GetFakePositionsList();
            var schoolContextMock = new Mock<SchoolContext>();
            schoolContextMock.Setup(x => x.Positions).ReturnsDbSet(positions);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация PositionsController
            var positionsController = new PositionsController(
                schoolContextMock.Object,
                configurationMock.Object
            );

            // Arrange
            var position = new Position
            {
                PositionId = 1,
                Name = "Учитель математики",
                Description = "Отвечает за преподавание математики и проведение контрольных работ.",
                Salary = 50000
            };
            var result = await positionsController.Edit(4, position);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_ReturnsARedirectAndCreate_WhenModelStateIsValid()
        {
            // Arrange
            var positions = TestDataHelper.GetFakePositionsList();
            var schoolContextMock = new Mock<SchoolContext>();
            schoolContextMock.Setup(x => x.Positions).ReturnsDbSet(positions);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация PositionsController
            var positionsController = new PositionsController(
                schoolContextMock.Object,
                configurationMock.Object
            );

            // Arrange
            var position = new Position
            {
                PositionId = 4,
                Name = "Учитель математики",
                Description = "Отвечает за преподавание математики и проведение контрольных работ.",
                Salary = 50000
            };

            var result = await positionsController.Edit(4, position);

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
            var positions = TestDataHelper.GetFakePositionsList();
            var schoolContextMock = new Mock<SchoolContext>();
            schoolContextMock.Setup(x => x.Positions).ReturnsDbSet(positions);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация PositionsController
            var positionsController = new PositionsController(
                schoolContextMock.Object,
                configurationMock.Object
            );

            // Act
            var notFoundResult = await positionsController.Delete(6);
            var foundResult = await positionsController.Delete(3);

            // Assert
            Assert.IsType<NotFoundResult>(notFoundResult);
            Assert.IsType<ViewResult>(foundResult);
        }

        [Fact]
        public async Task Delete_ReturnsARedirectAndDelete()
        {
            // Arrange
            var positions = TestDataHelper.GetFakePositionsList();
            var schoolContextMock = new Mock<SchoolContext>();
            schoolContextMock.Setup(x => x.Positions).ReturnsDbSet(positions);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация PositionsController
            var positionsController = new PositionsController(
                schoolContextMock.Object,
                configurationMock.Object
            );

            // Act
            var result = await positionsController.DeleteConfirmed(3);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            schoolContextMock.Verify();
        }
    }
}
