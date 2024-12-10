using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq.EntityFrameworkCore;
using SchoolWeb.DataLayer.Data;
using SchoolWeb.DataLayer.Models;
using SchoolWeb.ViewModels.SchedulesViewModel;
using SchoolWeb.ViewModels.SortStates;

namespace SchoolWeb.Tests
{
    public class SchedulesControllerTests
    {
        [Fact]
        public async Task GetSchedulesList()
        {
            // Arrange
            var schedules = TestDataHelper.GetFakeSchedulesList();
            var classes = TestDataHelper.GetFakeClassesList();
            var subjects = TestDataHelper.GetFakeSubjectsList();
            var schoolContextMock = new Mock<SchoolContext>();
            schoolContextMock.Setup(x => x.Schedules).ReturnsDbSet(schedules);
            schoolContextMock.Setup(x => x.Classes).ReturnsDbSet(classes);
            schoolContextMock.Setup(x => x.Subjects).ReturnsDbSet(subjects);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("20");

            // Инициализация SchedulesController
            var schedulesController = new SchedulesController(
                schoolContextMock.Object,
                configurationMock.Object
            );

            // Act
            var result = await schedulesController.Index(new FilterScheduleViewModel(), ScheduleSortState.No, 1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult);
            var model = Assert.IsAssignableFrom<ScheduleViewModel>(viewResult.ViewData.Model);
            Assert.Equal(TestDataHelper.GetFakeSchedulesList().Count, model.Schedules.Count());
            Assert.NotNull(model.Schedules);
        }

        [Fact]
        public async Task GetSchedule()
        {
            // Arrange
            var schedules = TestDataHelper.GetFakeSchedulesList();
            var schoolContextMock = new Mock<SchoolContext>();
            schoolContextMock.Setup(x => x.Schedules).ReturnsDbSet(schedules);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация SchedulesController
            var schedulesController = new SchedulesController(
                schoolContextMock.Object,
                configurationMock.Object
            );

            // Act
            var notFoundResult = await schedulesController.Details(100);
            var foundResult = await schedulesController.Details(1);

            // Assert
            Assert.IsType<NotFoundResult>(notFoundResult);
            Assert.IsType<ViewResult>(foundResult);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_GivenInvalidModel()
        {
            // Arrange
            var schedules = TestDataHelper.GetFakeSchedulesList();
            var schoolContextMock = new Mock<SchoolContext>();
            schoolContextMock.Setup(x => x.Schedules).ReturnsDbSet(schedules);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация SchedulesController
            var schedulesController = new SchedulesController(
                schoolContextMock.Object,
                configurationMock.Object
            );

            schedulesController.ModelState.AddModelError("error", "some error");

            // Act
            var result = await schedulesController.Create(schedule: null);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Create_ReturnsARedirectAndCreate_WhenModelStateIsValid()
        {
            // Arrange
            var schedules = TestDataHelper.GetFakeSchedulesList();
            var schoolContextMock = new Mock<SchoolContext>();
            schoolContextMock.Setup(x => x.Schedules).ReturnsDbSet(schedules);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация SchedulesController
            var schedulesController = new SchedulesController(
                schoolContextMock.Object,
                configurationMock.Object
            );

            // Arrange
            var schedule = new Schedule
            {
                ScheduleId = 1,
                Date = new DateOnly(2024, 12, 6), // Пример даты
                DayOfWeek = "Friday",
                ClassId = 1,
                SubjectId = 2,
                StartTime = new TimeOnly(9, 0), // Начало урока в 9:00
                EndTime = new TimeOnly(10, 0), // Конец урока в 10:00
                Class = TestDataHelper.GetFakeClassesList().SingleOrDefault(c => c.ClassId == 1),
                Subject = TestDataHelper.GetFakeSubjectsList().SingleOrDefault(c => c.SubjectId == 1)
            };

            // Act
            var result = await schedulesController.Create(schedule);

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
            var schedules = TestDataHelper.GetFakeSchedulesList();
            var classes = TestDataHelper.GetFakeClassesList();
            var subjects = TestDataHelper.GetFakeSubjectsList();
            var schoolContextMock = new Mock<SchoolContext>();
            schoolContextMock.Setup(x => x.Schedules).ReturnsDbSet(schedules);
            schoolContextMock.Setup(x => x.Classes).ReturnsDbSet(classes);
            schoolContextMock.Setup(x => x.Subjects).ReturnsDbSet(subjects);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация SchedulesController
            var schedulesController = new SchedulesController(
                schoolContextMock.Object,
                configurationMock.Object
            );

            // Act
            var notFoundResult = await schedulesController.Edit(400);
            var foundResult = await schedulesController.Edit(5);

            // Assert
            Assert.IsType<NotFoundResult>(notFoundResult);
            Assert.IsType<ViewResult>(foundResult);
        }

        [Fact]
        public async Task Edit_ReturnsBadRequest_GivenInvalidModel()
        {
            // Arrange
            var schedules = TestDataHelper.GetFakeSchedulesList();
            var schoolContextMock = new Mock<SchoolContext>();
            schoolContextMock.Setup(x => x.Schedules).ReturnsDbSet(schedules);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация SchedulesController
            var schedulesController = new SchedulesController(
                schoolContextMock.Object,
                configurationMock.Object
            );

            // Arrange
            var schedule = new Schedule
            {
                ScheduleId = 4,
                Date = new DateOnly(2024, 12, 6), // Пример даты
                DayOfWeek = "Friday",
                ClassId = 1,
                SubjectId = 2,
                StartTime = new TimeOnly(9, 0), // Начало урока в 9:00
                EndTime = new TimeOnly(10, 0), // Конец урока в 10:00
                Class = TestDataHelper.GetFakeClassesList().SingleOrDefault(c => c.ClassId == 1),
                Subject = TestDataHelper.GetFakeSubjectsList().SingleOrDefault(c => c.SubjectId == 1)
            };

            var result = await schedulesController.Edit(1, schedule);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_ReturnsARedirectAndCreate_WhenModelStateIsValid()
        {
            // Arrange
            var schedules = TestDataHelper.GetFakeSchedulesList();
            var schoolContextMock = new Mock<SchoolContext>();
            schoolContextMock.Setup(x => x.Schedules).ReturnsDbSet(schedules);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация SchedulesController
            var schedulesController = new SchedulesController(
                schoolContextMock.Object,
                configurationMock.Object
            );

            // Arrange
            var schedule = new Schedule
            {
                ScheduleId = 4,
                Date = new DateOnly(2024, 12, 6), // Пример даты
                DayOfWeek = "Friday",
                ClassId = 1,
                SubjectId = 2,
                StartTime = new TimeOnly(9, 0), // Начало урока в 9:00
                EndTime = new TimeOnly(10, 0), // Конец урока в 10:00
                Class = TestDataHelper.GetFakeClassesList().SingleOrDefault(c => c.ClassId == 1),
                Subject = TestDataHelper.GetFakeSubjectsList().SingleOrDefault(c => c.SubjectId == 1)
            };

            var result = await schedulesController.Edit(4, schedule);

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
            var schedules = TestDataHelper.GetFakeSchedulesList();
            var schoolContextMock = new Mock<SchoolContext>();
            schoolContextMock.Setup(x => x.Schedules).ReturnsDbSet(schedules);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация SchedulesController
            var schedulesController = new SchedulesController(
                schoolContextMock.Object,
                configurationMock.Object
            );

            // Act
            var notFoundResult = await schedulesController.Delete(100);
            var foundResult = await schedulesController.Delete(3);

            // Assert
            Assert.IsType<NotFoundResult>(notFoundResult);
            Assert.IsType<ViewResult>(foundResult);
        }

        [Fact]
        public async Task Delete_ReturnsARedirectAndDelete()
        {
            // Arrange
            var schedules = TestDataHelper.GetFakeSchedulesList();
            var schoolContextMock = new Mock<SchoolContext>();
            schoolContextMock.Setup(x => x.Schedules).ReturnsDbSet(schedules);

            // Мокирование IConfiguration и установка значения PageSize
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(config => config["Parameters:PageSize"]).Returns("10");

            // Инициализация SchedulesController
            var schedulesController = new SchedulesController(
                schoolContextMock.Object,
                configurationMock.Object
            );

            // Act
            var result = await schedulesController.DeleteConfirmed(3);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            schoolContextMock.Verify();
        }
    }
}
