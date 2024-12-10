using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq.EntityFrameworkCore;
using SchoolWeb.DataLayer.Data;
using SchoolWeb.DataLayer.Models;
using SchoolWeb.ViewModels.EmployeesViewModel;
using SchoolWeb.ViewModels.SortStates;

namespace SchoolWeb.Tests
{
    public class EmployeesControllerTests
    {
        [Fact]
        public async Task GetEmployeesList()
        {
            // Arrange
            var employees = TestDataHelper.GetFakeEmployeesList();
            var positions = TestDataHelper.GetFakePositionsList();
            var schoolContextMock = new Mock<SchoolContext>();
            schoolContextMock.Setup(x => x.Employees).ReturnsDbSet(employees);
            schoolContextMock.Setup(x => x.Positions).ReturnsDbSet(positions);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация EmployeesController
            var employeesController = new EmployeesController(
                schoolContextMock.Object,
                configurationMock.Object
            );

            // Act
            var result = await employeesController.Index(new FilterEmployeeViewModel(), EmployeeSortState.No, 1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult);
            var model = Assert.IsAssignableFrom<EmployeeViewModel>(viewResult.ViewData.Model);
            Assert.Equal(4, model.Employees.Count());
        }

        [Fact]
        public async Task GetEmployee()
        {
            // Arrange
            var employees = TestDataHelper.GetFakeEmployeesList();
            var schoolContextMock = new Mock<SchoolContext>();
            schoolContextMock.Setup(x => x.Employees).ReturnsDbSet(employees);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация EmployeesController
            var employeesController = new EmployeesController(
                schoolContextMock.Object,
                configurationMock.Object
            );

            // Act
            var notFoundResult = await employeesController.Details(6);
            var foundResult = await employeesController.Details(1);

            // Assert
            Assert.IsType<NotFoundResult>(notFoundResult);
            Assert.IsType<ViewResult>(foundResult);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_GivenInvalidModel()
        {
            // Arrange
            var employees = TestDataHelper.GetFakeEmployeesList();
            var schoolContextMock = new Mock<SchoolContext>();
            schoolContextMock.Setup(x => x.Employees).ReturnsDbSet(employees);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация EmployeesController
            var employeesController = new EmployeesController(
                schoolContextMock.Object,
                configurationMock.Object
            );
            employeesController.ModelState.AddModelError("error", "some error");

            // Act
            var result = await employeesController.Create(employee: null);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Create_ReturnsARedirectAndCreate_WhenModelStateIsValid()
        {
            // Arrange
            var employees = TestDataHelper.GetFakeEmployeesList();
            var positions = TestDataHelper.GetFakePositionsList();
            var schoolContextMock = new Mock<SchoolContext>();
            schoolContextMock.Setup(x => x.Employees).ReturnsDbSet(employees);
            schoolContextMock.Setup(x => x.Positions).ReturnsDbSet(positions);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация EmployeesController
            var employeesController = new EmployeesController(
                schoolContextMock.Object,
                configurationMock.Object
            );

            // Arrange
            var employee = new Employee
            {
                EmployeeId = 1,
                FirstName = "Иван",
                LastName = "Иванов",
                MiddleName = "Петрович",
                PositionId = 1,
                Position = positions.SingleOrDefault(p => p.PositionId == 1)
            };

            // Act
            var result = await employeesController.Create(employee);

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
            var employees = TestDataHelper.GetFakeEmployeesList();
            var positions = TestDataHelper.GetFakePositionsList();
            var schoolContextMock = new Mock<SchoolContext>();
            schoolContextMock.Setup(x => x.Employees).ReturnsDbSet(employees);
            schoolContextMock.Setup(x => x.Positions).ReturnsDbSet(positions);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация EmployeesController
            var employeesController = new EmployeesController(
                schoolContextMock.Object,
                configurationMock.Object
            );

            // Act
            var notFoundResult = await employeesController.Edit(100);
            var foundResult = await employeesController.Edit(4);

            // Assert
            Assert.IsType<NotFoundResult>(notFoundResult);
            Assert.IsType<ViewResult>(foundResult);
        }

        [Fact]
        public async Task Edit_ReturnsBadRequest_GivenInvalidModel()
        {
            // Arrange
            var employees = TestDataHelper.GetFakeEmployeesList();
            var positions = TestDataHelper.GetFakePositionsList();
            var schoolContextMock = new Mock<SchoolContext>();
            schoolContextMock.Setup(x => x.Employees).ReturnsDbSet(employees);
            schoolContextMock.Setup(x => x.Positions).ReturnsDbSet(positions);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация EmployeesController
            var employeesController = new EmployeesController(
                schoolContextMock.Object,
                configurationMock.Object
            );

            // Arrange
            var employee = new Employee
            {
                EmployeeId = 1,
                FirstName = "Иван",
                LastName = "Иванов",
                MiddleName = "Петрович",
                PositionId = 1,
                Position = positions.SingleOrDefault(p => p.PositionId == 1)
            };

            var result = await employeesController.Edit(4, employee);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_ReturnsARedirectAndCreate_WhenModelStateIsValid()
        {
            // Arrange
            var employees = TestDataHelper.GetFakeEmployeesList();
            var positions = TestDataHelper.GetFakePositionsList();
            var schoolContextMock = new Mock<SchoolContext>();
            schoolContextMock.Setup(x => x.Employees).ReturnsDbSet(employees);
            schoolContextMock.Setup(x => x.Positions).ReturnsDbSet(positions);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация EmployeesController
            var employeesController = new EmployeesController(
                schoolContextMock.Object,
                configurationMock.Object
            );

            // Arrange
            var employee = new Employee
            {
                EmployeeId = 6,
                FirstName = "Иван",
                LastName = "Иванов",
                MiddleName = "Петрович",
                PositionId = 1,
                Position = positions.SingleOrDefault(p => p.PositionId == 1)
            };
            var result = await employeesController.Edit(6, employee);

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
            var employees = TestDataHelper.GetFakeEmployeesList();
            var schoolContextMock = new Mock<SchoolContext>();
            schoolContextMock.Setup(x => x.Employees).ReturnsDbSet(employees);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация EmployeesController
            var employeesController = new EmployeesController(
                schoolContextMock.Object,
                configurationMock.Object
            );

            // Act
            var notFoundResult = await employeesController.Delete(6);
            var foundResult = await employeesController.Delete(3);

            // Assert
            Assert.IsType<NotFoundResult>(notFoundResult);
            Assert.IsType<ViewResult>(foundResult);
        }

        [Fact]
        public async Task Delete_ReturnsARedirectAndDelete()
        {
            // Arrange
            var employees = TestDataHelper.GetFakeEmployeesList();
            var schoolContextMock = new Mock<SchoolContext>();
            schoolContextMock.Setup(x => x.Employees).ReturnsDbSet(employees);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация EmployeesController
            var employeesController = new EmployeesController(
                schoolContextMock.Object,
                configurationMock.Object
            );

            // Act
            var result = await employeesController.DeleteConfirmed(3);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            schoolContextMock.Verify();
        }
    }
}
