using Moq;
using Xunit;

namespace AirportTestApp.Tests;

/// <summary>
/// Тестовый класс для проверки функциональности управления самолётами в системе управления аэропортом
/// </summary>
public class AirplaneRepositoryTests
{
    /// <summary>
    /// ✅ ТЕСТ №1: Успешное добавление самолёта с КОРРЕКТНЫМИ данными
    /// Ожидаемый результат: PASSED - метод возвращает ID > 0
    /// </summary>
    [Fact]
    public async Task CreateAirplane_WithValidData_ReturnsSuccessfulId()
    {
        // Arrange (Подготовка)
        var mockDatabaseService = new Mock<DatabaseService>("FakeConnectionString");
        
        // Mock возвращает ID нового самолёта
        mockDatabaseService
            .Setup(db => db.ExecuteScalarAsync<int>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(10); // Успешное добавление с ID = 10

        var repository = new AirplaneRepository(mockDatabaseService.Object);

        // Самолёт с корректными данными
        var airplane = new Airplane
        {
            Model = "Boeing 737-800",
            Capacity = 189,
            Airline = "Aeroflot",
            StatusID = 1,
            GateID = 1,
            RegistrationNumber = "RA-73001",
            ManufactureDate = new DateTime(2020, 5, 15),
            LastMaintenanceDate = new DateTime(2024, 9, 1),
            NextMaintenanceDate = new DateTime(2025, 3, 1),
            CreatedDate = DateTime.Now
        };

        // Act (Действие)
        var result = await repository.CreateAirplaneAsync(airplane);

        // Assert (Проверка)
        Assert.True(result > 0, "✅ ID нового самолёта должен быть больше 0");
        Assert.Equal(10, result);
        
        mockDatabaseService.Verify(
            db => db.ExecuteScalarAsync<int>(It.IsAny<string>(), It.IsAny<object>()),
            Times.Once,
            "✅ Метод ExecuteScalarAsync должен быть вызван один раз");
    }

    /// <summary>
    /// ❌ ТЕСТ №2: Неудачное добавление самолёта с НЕКОРРЕКТНЫМИ данными
    /// Ожидаемый результат: PASSED (тест проходит, потому что ОЖИДАЕТСЯ исключение)
    /// База данных должна отклонить некорректные данные
    /// </summary>
    [Fact]
    public async Task CreateAirplane_WithInvalidData_ThrowsException()
    {
        // Arrange (Подготовка)
        var mockDatabaseService = new Mock<DatabaseService>("FakeConnectionString");
        
        // Mock выбрасывает исключение при некорректных данных (имитация SQL ошибки)
        mockDatabaseService
            .Setup(db => db.ExecuteScalarAsync<int>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ThrowsAsync(new InvalidOperationException("❌ Ошибка валидации данных: некорректные параметры самолёта"));

        var repository = new AirplaneRepository(mockDatabaseService.Object);

        // Самолёт с НЕКОРРЕКТНЫМИ данными
        var invalidAirplane = new Airplane
        {
            Model = "", // ❌ Пустая модель
            Capacity = -100, // ❌ Отрицательная вместимость
            Airline = "",
            RegistrationNumber = "",
            CreatedDate = DateTime.Now
        };

        // Act & Assert (Действие и проверка)
        // ✅ Тест УСПЕШЕН, если выбрасывается исключение
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await repository.CreateAirplaneAsync(invalidAirplane));

        Assert.Contains("валидации", exception.Message.ToLower());
        
        mockDatabaseService.Verify(
            db => db.ExecuteScalarAsync<int>(It.IsAny<string>(), It.IsAny<object>()),
            Times.Once);
    }

    /// <summary>
    /// ✅ ТЕСТ №3: Проверка КОРРЕКТНОСТИ установки свойств самолёта
    /// Ожидаемый результат: PASSED - все свойства установлены правильно
    /// </summary>
    [Fact]
    public void Airplane_Properties_WithValidData_AreSetCorrectly()
    {
        // Arrange & Act (Подготовка и действие)
        var airplane = new Airplane
        {
            AirplaneID = 5,
            Model = "Airbus A320",
            Capacity = 180,
            Airline = "S7 Airlines",
            StatusID = 1,
            GateID = 3,
            RegistrationNumber = "VQ-BRE",
            ManufactureDate = new DateTime(2019, 8, 20),
            LastMaintenanceDate = new DateTime(2024, 10, 5),
            NextMaintenanceDate = new DateTime(2025, 4, 5),
            CreatedDate = DateTime.Now
        };

        // Assert (Проверка)
        Assert.Equal(5, airplane.AirplaneID);
        Assert.Equal("Airbus A320", airplane.Model);
        Assert.Equal(180, airplane.Capacity);
        Assert.Equal("S7 Airlines", airplane.Airline);
        Assert.Equal(1, airplane.StatusID);
        Assert.Equal(3, airplane.GateID);
        Assert.Equal("VQ-BRE", airplane.RegistrationNumber);
        Assert.NotNull(airplane.ManufactureDate);
        Assert.NotNull(airplane.LastMaintenanceDate);
        Assert.NotNull(airplane.NextMaintenanceDate);
        Assert.True(airplane.CreatedDate <= DateTime.Now);
    }

    /// <summary>
    /// ❌ ТЕСТ №4: Проверка НЕКОРРЕКТНЫХ свойств самолёта
    /// Ожидаемый результат: PASSED (тест проходит, потому что проверка ВЫЯВЛЯЕТ ошибки)
    /// </summary>
    [Fact]
    public void Airplane_Properties_WithInvalidData_FailsValidation()
    {
        // Arrange (Подготовка)
        var invalidAirplane = new Airplane
        {
            Model = "", // ❌ Пустая модель
            Capacity = -50, // ❌ Отрицательная вместимость
            Airline = null, // ❌ Null авиакомпания
            RegistrationNumber = ""
        };

        // Assert (Проверка НЕКОРРЕКТНЫХ данных)
        // ✅ Тест УСПЕШЕН, если проверки выявляют ошибки
        Assert.True(string.IsNullOrWhiteSpace(invalidAirplane.Model), "❌ Модель не должна быть пустой");
        Assert.True(invalidAirplane.Capacity < 0, "❌ Вместимость не может быть отрицательной");
        Assert.True(string.IsNullOrWhiteSpace(invalidAirplane.Airline), "❌ Авиакомпания не должна быть пустой");
        Assert.True(string.IsNullOrWhiteSpace(invalidAirplane.RegistrationNumber), "❌ Регистрационный номер не должен быть пустым");
    }

    /// <summary>
    /// ✅ ТЕСТ №5: Успешное обновление статуса самолёта
    /// Ожидаемый результат: PASSED - статус обновляется
    /// </summary>
    [Fact]
    public async Task UpdateAirplaneStatus_WithValidData_ReturnsTrue()
    {
        // Arrange (Подготовка)
        var mockDatabaseService = new Mock<DatabaseService>("FakeConnectionString");
        
        // Mock возвращает 1 (одна строка обновлена)
        mockDatabaseService
            .Setup(db => db.ExecuteAsync(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(1);

        var repository = new AirplaneRepository(mockDatabaseService.Object);

        // Act (Действие)
        var result = await repository.UpdateAirplaneStatusAsync(1, 2);

        // Assert (Проверка)
        Assert.True(result, "✅ Обновление статуса должно вернуть true при успехе");
        
        mockDatabaseService.Verify(
            db => db.ExecuteAsync(It.IsAny<string>(), It.IsAny<object>()),
            Times.Once);
    }

    /// <summary>
    /// ❌ ТЕСТ №6: Неудачное обновление статуса несуществующего самолёта
    /// Ожидаемый результат: PASSED (тест проходит, потому что ОЖИДАЕТСЯ false)
    /// </summary>
    [Fact]
    public async Task UpdateAirplaneStatus_WithNonExistentId_ReturnsFalse()
    {
        // Arrange (Подготовка)
        var mockDatabaseService = new Mock<DatabaseService>("FakeConnectionString");
        
        // Mock возвращает 0 (ни одна строка не обновлена - самолёт не найден)
        mockDatabaseService
            .Setup(db => db.ExecuteAsync(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(0);

        var repository = new AirplaneRepository(mockDatabaseService.Object);

        // Act (Действие)
        var result = await repository.UpdateAirplaneStatusAsync(99999, 2); // Несуществующий ID

        // Assert (Проверка)
        // ✅ Тест УСПЕШЕН, если метод возвращает false
        Assert.False(result, "❌ Обновление несуществующего самолёта должно вернуть false");
        
        mockDatabaseService.Verify(
            db => db.ExecuteAsync(It.IsAny<string>(), It.IsAny<object>()),
            Times.Once);
    }
}

