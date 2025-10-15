// Строка подключения к базе данных
string connectionString = "Server=192.168.9.203\\sqlexpress;Database=AirportManagementDB;User Id=student1;Password=123456;TrustServerCertificate=true;";

// Красивая анимация загрузки
await ShowLoadingAnimation();

Console.WriteLine("🏗️ Система управления аэропортом");
Console.WriteLine("=====================================");

// Создаем сервис базы данных
var databaseService = new DatabaseService(connectionString);

// Создаем репозитории
var airportRepository = new AirportRepository(databaseService);
var statusRepository = new StatusRepository(databaseService);
var airplaneRepository = new AirplaneRepository(databaseService);
var gateRepository = new GateRepository(databaseService);

try
{
    // Проверка подключения
    Console.WriteLine("\n🔍 Проверка подключения к базе данных...");
    var (success, errorMessage) = await databaseService.TestConnectionAsync();

    if (!success)
    {
        Console.WriteLine($"\n❌ Ошибка подключения: {errorMessage}");
        Console.WriteLine("Проверьте настройки подключения и повторите попытку.");
        Console.ReadKey();
        return;
    }

    Console.WriteLine("✅ Подключение к базе данных успешно!");

    // Основной цикл программы
    bool running = true;
    while (running)
    {
        Console.Clear();
        Console.WriteLine("🏗️ Система управления аэропортом");
        Console.WriteLine("=====================================");
        Console.WriteLine("Выберите действие:");
        Console.WriteLine("1. Просмотреть аэропорты");
        Console.WriteLine("2. Просмотреть статусы самолетов");
        Console.WriteLine("3. Просмотреть самолеты");
        Console.WriteLine("4. Добавить новый самолет");
        Console.WriteLine("5. Редактировать самолет");
        Console.WriteLine("6. Изменить статус самолета");
        Console.WriteLine("7. Назначить самолет к воротам");
        Console.WriteLine("8. Удалить самолет");
        Console.WriteLine("9. Статистика");
        Console.WriteLine("0. Выход");
        Console.WriteLine("=====================================");

        Console.Write("Ваш выбор: ");
        string choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                await ShowAirports();
                break;
            case "2":
                await ShowStatuses();
                break;
            case "3":
                await ShowAirplanes();
                break;
            case "4":
                await AddAirplane();
                break;
            case "5":
                await EditAirplane();
                break;
            case "6":
                await UpdateAirplaneStatus();
                break;
            case "7":
                await AssignAirplaneToGate();
                break;
            case "8":
                await DeleteAirplane();
                break;
            case "9":
                await ShowStatistics();
                break;
            case "0":
                running = false;
                break;
            default:
                Console.WriteLine("Неверный выбор. Нажмите любую клавишу...");
                Console.ReadKey();
                break;
        }
    }

    Console.WriteLine("Программа завершена.");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Критическая ошибка: {ex.Message}");
    Console.WriteLine($"Подробности: {ex.InnerException?.Message}");
}
finally
{
    Console.WriteLine("Нажмите любую клавишу для выхода...");
    Console.ReadKey();
}

// Вспомогательные методы


// Методы для работы с данными

async Task ShowAirports()
{
    Console.Clear();
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("╔════════════════════════════════════════════════════════════════════════════╗");
    Console.WriteLine("║                        🏢 СПИСОК АЭРОПОРТОВ                                ║");
    Console.WriteLine("╚════════════════════════════════════════════════════════════════════════════╝");
    Console.ResetColor();

    var airports = await airportRepository.GetAllAirportsAsync();
    if (airports.Any())
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("┌────┬──────┬─────────────────────────┬──────────────────┬────────────────┐");
        Console.WriteLine("│ ID │ Код  │ Название                │ Город            │ Страна         │");
        Console.WriteLine("├────┼──────┼─────────────────────────┼──────────────────┼────────────────┤");
        Console.ResetColor();

        foreach (var airport in airports)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"│ {airport.AirportID,-2} │ {TruncateString(airport.AirportCode, 4),-4} │ {TruncateString(airport.AirportName, 23),-23} │ {TruncateString(airport.City, 16),-16} │ {TruncateString(airport.Country, 14),-14} │");
            Console.ResetColor();
        }

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("└────┴──────┴─────────────────────────┴──────────────────┴────────────────┘");
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\n📊 Всего аэропортов: {airports.Count()}");
        Console.ResetColor();
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("\n❌ Аэропорты не найдены.");
        Console.ResetColor();
    }

    Console.WriteLine("\nНажмите любую клавишу для продолжения...");
    Console.ReadKey();
}


async Task ShowStatuses()
{
    Console.Clear();
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("╔════════════════════════════════════════════════════════════════════════════╗");
    Console.WriteLine("║                        📋 СТАТУСЫ САМОЛЕТОВ                                ║");
    Console.WriteLine("╚════════════════════════════════════════════════════════════════════════════╝");
    Console.ResetColor();

    var statuses = await statusRepository.GetAllStatusesAsync();
    if (statuses.Any())
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("┌────┬──────────────────┬────────────────────────────────────────────────────┐");
        Console.WriteLine("│ ID │ Название         │ Описание                                           │");
        Console.WriteLine("├────┼──────────────────┼────────────────────────────────────────────────────┤");
        Console.ResetColor();

        foreach (var status in statuses)
        {
            // Цвет в зависимости от статуса
            ConsoleColor statusColor = status.StatusName.ToLower() switch
            {
                "active" => ConsoleColor.Green,
                "inactive" => ConsoleColor.Gray,
                "repair" => ConsoleColor.Red,
                "in flight" => ConsoleColor.Cyan,
                "boarding" => ConsoleColor.Yellow,
                _ => ConsoleColor.White
            };

            Console.ForegroundColor = statusColor;
            Console.Write($"│ {status.StatusID,-2} │ ");
            Console.Write($"{TruncateString(status.StatusName, 16),-16} │ ");
            Console.Write($"{TruncateString(status.StatusDescription, 50),-50} │");
            Console.WriteLine();
            Console.ResetColor();
        }

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("└────┴──────────────────┴────────────────────────────────────────────────────┘");
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\n📊 Всего статусов: {statuses.Count()}");
        Console.ResetColor();
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("\n❌ Статусы не найдены.");
        Console.ResetColor();
    }

    Console.WriteLine("\nНажмите любую клавишу для продолжения...");
    Console.ReadKey();
}

async Task ShowAirplanes()
{
    Console.Clear();
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("╔════════════════════════════════════════════════════════════════════════════════════════════════════════════════════════════╗");
    Console.WriteLine("║                                                    ✈️ СПИСОК САМОЛЕТОВ                                                      ║");
    Console.WriteLine("╚════════════════════════════════════════════════════════════════════════════════════════════════════════════════════════════╝");
    Console.ResetColor();

    var airplanes = await airplaneRepository.GetAllAirplanesAsync();
    if (airplanes.Any())
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("┌────┬──────────────────┬──────────────────┬──────────────┬──────┬──────────────┬────────────┬──────────────────────────┐");
        Console.WriteLine("│ ID │ Модель           │ Авиакомпания     │ Рег. номер   │ Вмест│ Статус       │ Ворота     │ Аэропорт                 │");
        Console.WriteLine("├────┼──────────────────┼──────────────────┼──────────────┼──────┼──────────────┼────────────┼──────────────────────────┤");
        Console.ResetColor();

        foreach (var airplane in airplanes)
        {
            // Цвет строки в зависимости от статуса
            ConsoleColor rowColor = airplane.Status?.StatusName.ToLower() switch
            {
                "active" => ConsoleColor.Green,
                "inactive" => ConsoleColor.DarkGray,
                "repair" => ConsoleColor.Red,
                "in flight" => ConsoleColor.Cyan,
                "boarding" => ConsoleColor.Yellow,
                _ => ConsoleColor.White
            };

            Console.ForegroundColor = rowColor;
            
            string statusName = airplane.Status?.StatusName ?? "Неизвестно";
            string gateName = airplane.Gate?.GateName ?? "Не назнач.";
            string airportInfo = airplane.Gate?.Airport != null 
                ? $"{airplane.Gate.Airport.AirportCode}" 
                : "-";

            Console.WriteLine($"│ {airplane.AirplaneID,-2} │ {TruncateString(airplane.Model, 16),-16} │ {TruncateString(airplane.Airline, 16),-16} │ {TruncateString(airplane.RegistrationNumber, 12),-12} │ {airplane.Capacity,4} │ {TruncateString(statusName, 12),-12} │ {TruncateString(gateName, 10),-10} │ {TruncateString(airportInfo, 24),-24} │");
            Console.ResetColor();
        }

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("└────┴──────────────────┴──────────────────┴──────────────┴──────┴──────────────┴────────────┴──────────────────────────┘");
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\n📊 Всего самолетов: {airplanes.Count()}");
        Console.ResetColor();

        // Показываем детали по запросу
        Console.WriteLine("\n💡 Для просмотра подробной информации о самолёте введите его ID (или нажмите Enter для выхода):");
        Console.Write("ID: ");
        string input = Console.ReadLine();
        
        if (!string.IsNullOrWhiteSpace(input) && int.TryParse(input, out int selectedId))
        {
            var selectedPlane = airplanes.FirstOrDefault(a => a.AirplaneID == selectedId);
            if (selectedPlane != null)
            {
                ShowAirplaneDetails(selectedPlane);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("❌ Самолёт с таким ID не найден.");
                Console.ResetColor();
                Console.ReadKey();
            }
        }
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("\n❌ Самолеты не найдены.");
        Console.ResetColor();
    }

    Console.WriteLine("\nНажмите любую клавишу для продолжения...");
    Console.ReadKey();
}

async Task AddAirplane()
{
    Console.Clear();
    Console.WriteLine("➕ ДОБАВЛЕНИЕ НОВОГО САМОЛЕТА");
    Console.WriteLine("================================");

    try
    {
        // Показываем доступные статусы и ворота для справки
        Console.WriteLine("\n📋 ДОСТУПНЫЕ СТАТУСЫ:");
        var statuses = await statusRepository.GetAllStatusesAsync();
        foreach (var status in statuses)
        {
            Console.WriteLine($"   {status.StatusID}. {status.StatusName} - {status.StatusDescription}");
        }

        Console.WriteLine("\n🚪 ДОСТУПНЫЕ ВОРОТА:");
        var gates = await GetAllGatesWithAirports();
        if (gates.Any())
        {
            foreach (var gate in gates)
            {
                Console.WriteLine($"   {gate.GateID}. {gate.GateName} ({gate.GateType}) - {gate.Airport?.AirportName ?? "Неизвестный аэропорт"}");
            }
        }
        else
        {
            Console.WriteLine("   Ворота не найдены");
        }

        Console.WriteLine("\n" + "=".PadRight(50));

        var airplane = new Airplane();

        // Ввод модели самолета
        Console.Write("Введите модель самолета: ");
        airplane.Model = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(airplane.Model))
        {
            Console.WriteLine("❌ Модель самолета не может быть пустой!");
            Console.ReadKey();
            return;
        }

        // Ввод авиакомпании
        Console.Write("Введите авиакомпанию: ");
        airplane.Airline = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(airplane.Airline))
        {
            Console.WriteLine("❌ Авиакомпания не может быть пустой!");
            Console.ReadKey();
            return;
        }

        // Ввод регистрационного номера
        Console.Write("Введите регистрационный номер: ");
        airplane.RegistrationNumber = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(airplane.RegistrationNumber))
        {
            Console.WriteLine("❌ Регистрационный номер не может быть пустым!");
            Console.ReadKey();
            return;
        }

        // Ввод вместимости
        Console.Write("Введите вместимость (пассажиров): ");
        if (int.TryParse(Console.ReadLine(), out int capacity) && capacity > 0)
        {
            airplane.Capacity = capacity;
        }
        else
        {
            Console.WriteLine("❌ Введите корректное число для вместимости!");
            Console.ReadKey();
            return;
        }

        // Выбор статуса
        Console.WriteLine("Доступные статусы:");
        var availableStatuses = await statusRepository.GetAllStatusesAsync();
        foreach (var status in availableStatuses)
        {
            Console.WriteLine($"   {status.StatusID}. {status.StatusName}");
        }
        Console.Write("Введите ID статуса (1-5): ");
        string statusInput = Console.ReadLine();
        if (int.TryParse(statusInput, out int statusId) && statusId >= 1 && statusId <= 5)
        {
            airplane.StatusID = statusId;
        }
        else
        {
            airplane.StatusID = 1; // По умолчанию Active
        }

        // Выбор ворот (опционально)
        Console.Write("Введите ID ворот (или нажмите Enter для пропуска): ");
        string gateInput = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(gateInput))
        {
            if (int.TryParse(gateInput, out int gateId) && gateId > 0)
            {
                airplane.GateID = gateId;
            }
            else
            {
                Console.WriteLine("❌ Введите корректный ID ворот!");
                Console.ReadKey();
                return;
            }
        }

        // Даты производства и обслуживания (опционально)
        Console.Write("Введите дату производства (гггг-мм-дд) или нажмите Enter: ");
        string manufactureDateInput = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(manufactureDateInput))
        {
            if (DateTime.TryParse(manufactureDateInput, out DateTime manufactureDate))
            {
                airplane.ManufactureDate = manufactureDate;
            }
            else
            {
                Console.WriteLine("❌ Неверный формат даты!");
                Console.ReadKey();
                return;
            }
        }

        // Дата последнего обслуживания
        Console.Write("Введите дату последнего обслуживания (гггг-мм-дд) или нажмите Enter: ");
        string lastMaintenanceInput = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(lastMaintenanceInput))
        {
            if (DateTime.TryParse(lastMaintenanceInput, out DateTime lastMaintenance))
            {
                airplane.LastMaintenanceDate = lastMaintenance;
            }
            else
            {
                Console.WriteLine("❌ Неверный формат даты!");
                Console.ReadKey();
                return;
            }
        }

        // Дата следующего обслуживания
        Console.Write("Введите дату следующего обслуживания (гггг-мм-дд) или нажмите Enter: ");
        string nextMaintenanceInput = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(nextMaintenanceInput))
        {
            if (DateTime.TryParse(nextMaintenanceInput, out DateTime nextMaintenance))
            {
                airplane.NextMaintenanceDate = nextMaintenance;
            }
            else
            {
                Console.WriteLine("❌ Неверный формат даты!");
                Console.ReadKey();
                return;
            }
        }

        airplane.CreatedDate = DateTime.Now;

        Console.WriteLine("\n📋 ПОДТВЕРЖДЕНИЕ ДАННЫХ:");
        Console.WriteLine($"Модель: {airplane.Model}");
        Console.WriteLine($"Авиакомпания: {airplane.Airline}");
        Console.WriteLine($"Регистрация: {airplane.RegistrationNumber}");
        Console.WriteLine($"Вместимость: {airplane.Capacity}");
        var statusName = availableStatuses.FirstOrDefault(s => s.StatusID == airplane.StatusID)?.StatusName ?? "Неизвестно";
        Console.WriteLine($"Статус: {statusName} (ID: {airplane.StatusID})");
        if (airplane.GateID.HasValue)
            Console.WriteLine($"Ворота ID: {airplane.GateID}");
        if (airplane.ManufactureDate.HasValue)
            Console.WriteLine($"Дата производства: {airplane.ManufactureDate.Value:dd.MM.yyyy}");
        if (airplane.LastMaintenanceDate.HasValue)
            Console.WriteLine($"Последнее обслуживание: {airplane.LastMaintenanceDate.Value:dd.MM.yyyy}");
        if (airplane.NextMaintenanceDate.HasValue)
            Console.WriteLine($"Следующее обслуживание: {airplane.NextMaintenanceDate.Value:dd.MM.yyyy}");
        Console.WriteLine($"Дата создания: {airplane.CreatedDate:dd.MM.yyyy HH:mm:ss}");

        Console.Write("\nСохранить самолет? (y/n): ");
        string confirmation = Console.ReadLine();

        if (confirmation.ToLower() == "y" || confirmation.ToLower() == "да")
        {
            var newId = await airplaneRepository.CreateAirplaneAsync(airplane);
            Console.WriteLine($"\n✅ Самолет успешно добавлен с ID: {newId}");

            // Показываем добавленный самолет
            var addedAirplane = await airplaneRepository.GetAirplaneWithDetailsAsync(newId);
            if (addedAirplane != null)
            {
                Console.WriteLine("\n📋 ДОБАВЛЕННЫЙ САМОЛЕТ:");
                Console.WriteLine($"ID: {addedAirplane.AirplaneID}");
                Console.WriteLine($"Модель: {addedAirplane.Model}");
                Console.WriteLine($"Авиакомпания: {addedAirplane.Airline}");
                Console.WriteLine($"Регистрация: {addedAirplane.RegistrationNumber}");
                Console.WriteLine($"Вместимость: {addedAirplane.Capacity}");
                Console.WriteLine($"Статус: {addedAirplane.Status}");
                if (addedAirplane.Gate != null)
                    Console.WriteLine($"Ворота: {addedAirplane.Gate.GateName}");
            }
        }
        else
        {
            Console.WriteLine("\n❌ Добавление отменено.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Ошибка при добавлении: {ex.Message}");
    }

    Console.WriteLine("\nНажмите любую клавишу для продолжения...");
    Console.ReadKey();
}


async Task UpdateAirplaneStatus()
{
    Console.Clear();
    Console.WriteLine("🔄 ИЗМЕНЕНИЕ СТАТУСА САМОЛЕТА");
    Console.WriteLine("================================");

    try
    {
        // Показываем текущие самолеты
        var airplanes = await airplaneRepository.GetAllAirplanesAsync();
        if (!airplanes.Any())
        {
            Console.WriteLine("Самолеты не найдены.");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("Текущие самолеты:");
        foreach (var airplane in airplanes)
        {
            Console.WriteLine($"ID: {airplane.AirplaneID} - {airplane.Model} ({airplane.Status?.StatusName ?? "Без статуса"})");
        }

        Console.Write("\nВведите ID самолета: ");
        if (int.TryParse(Console.ReadLine(), out int airplaneId))
        {
            Console.WriteLine("Доступные статусы:");
            var statuses = await statusRepository.GetAllStatusesAsync();
            foreach (var status in statuses)
            {
                Console.WriteLine($"   {status.StatusID}. {status.StatusName}");
            }
            Console.Write("Введите ID нового статуса: ");
            string newStatusInput = Console.ReadLine();
            if (int.TryParse(newStatusInput, out int newStatusId) && newStatusId >= 1 && newStatusId <= 5)
            {
                var success = await airplaneRepository.UpdateAirplaneStatusAsync(airplaneId, newStatusId);
                if (success)
                {
                    Console.WriteLine("✅ Статус самолета успешно изменен!");
                }
                else
                {
                    Console.WriteLine("❌ Не удалось изменить статус самолета.");
                }
            }
            else
            {
                Console.WriteLine("Неверный формат статуса!");
            }
        }
        else
        {
            Console.WriteLine("Неверный формат ID самолета!");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Ошибка: {ex.Message}");
    }

    Console.WriteLine("\nНажмите любую клавишу для продолжения...");
    Console.ReadKey();
}


async Task EditAirplane()
{
    Console.Clear();
    Console.WriteLine("✏️ РЕДАКТИРОВАНИЕ САМОЛЕТА");
    Console.WriteLine("================================");

    try
    {
        // Показываем текущие самолеты
        var airplanes = await airplaneRepository.GetAllAirplanesAsync();
        if (!airplanes.Any())
        {
            Console.WriteLine("Самолеты не найдены.");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("Текущие самолеты:");
        foreach (var airplane in airplanes)
        {
            Console.WriteLine($"ID: {airplane.AirplaneID} - {airplane.Model} ({airplane.Status?.StatusName ?? "Без статуса"})");
        }

        Console.Write("\nВведите ID самолета для редактирования: ");
        if (int.TryParse(Console.ReadLine(), out int airplaneId))
        {
            var airplane = await airplaneRepository.GetAirplaneWithDetailsAsync(airplaneId);
            if (airplane == null)
            {
                Console.WriteLine("❌ Самолет не найден!");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"\n📋 ТЕКУЩИЕ ДАННЫЕ САМОЛЕТА ID {airplaneId}:");
            Console.WriteLine($"Модель: {airplane.Model}");
            Console.WriteLine($"Авиакомпания: {airplane.Airline}");
            Console.WriteLine($"Регистрация: {airplane.RegistrationNumber}");
            Console.WriteLine($"Вместимость: {airplane.Capacity}");
            Console.WriteLine($"Статус: {airplane.Status?.StatusName ?? "Не установлен"} (ID: {airplane.StatusID})");
            Console.WriteLine($"Ворота: {airplane.Gate?.GateName ?? "Не назначены"} (ID: {airplane.GateID})");
            if (airplane.ManufactureDate.HasValue)
                Console.WriteLine($"Дата производства: {airplane.ManufactureDate.Value:dd.MM.yyyy}");

            Console.WriteLine("\n" + "=".PadRight(50));
            Console.WriteLine("Что вы хотите изменить?");
            Console.WriteLine("1. Модель самолета");
            Console.WriteLine("2. Авиакомпанию");
            Console.WriteLine("3. Регистрационный номер");
            Console.WriteLine("4. Вместимость");
            Console.WriteLine("5. Статус");
            Console.WriteLine("6. Назначить к воротам");
            Console.WriteLine("7. Дату производства");
            Console.WriteLine("8. Все поля сразу");
            Console.WriteLine("0. Отмена");

            Console.Write("Ваш выбор: ");
            string editChoice = Console.ReadLine();

            bool changesMade = false;
            var updatedAirplane = new Airplane
            {
                AirplaneID = airplaneId,
                Model = airplane.Model,
                Airline = airplane.Airline,
                RegistrationNumber = airplane.RegistrationNumber,
                Capacity = airplane.Capacity,
                Status = airplane.Status,
                GateID = airplane.GateID,
                ManufactureDate = airplane.ManufactureDate,
                LastMaintenanceDate = airplane.LastMaintenanceDate,
                NextMaintenanceDate = airplane.NextMaintenanceDate,
                CreatedDate = airplane.CreatedDate
            };

            switch (editChoice)
            {
                case "1":
                    Console.Write("Введите новую модель: ");
                    string newModel = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(newModel))
                    {
                        updatedAirplane.Model = newModel;
                        changesMade = true;
                    }
                    break;

                case "2":
                    Console.Write("Введите новую авиакомпанию: ");
                    string newAirline = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(newAirline))
                    {
                        updatedAirplane.Airline = newAirline;
                        changesMade = true;
                    }
                    break;

                case "3":
                    Console.Write("Введите новый регистрационный номер: ");
                    string newRegistration = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(newRegistration))
                    {
                        updatedAirplane.RegistrationNumber = newRegistration;
                        changesMade = true;
                    }
                    break;

                case "4":
                    Console.Write("Введите новую вместимость: ");
                    if (int.TryParse(Console.ReadLine(), out int newCapacity) && newCapacity > 0)
                    {
                        updatedAirplane.Capacity = newCapacity;
                        changesMade = true;
                    }
                    else
                    {
                        Console.WriteLine("❌ Неверное значение вместимости!");
                    }
                    break;

                case "5":
                    Console.WriteLine("Доступные статусы:");
                    var statuses = await statusRepository.GetAllStatusesAsync();
                    foreach (var status in statuses)
                    {
                        Console.WriteLine($"   {status.StatusID}. {status.StatusName}");
                    }
                    Console.Write("Введите ID нового статуса: ");
                    if (int.TryParse(Console.ReadLine(), out int newStatusId) && newStatusId >= 1 && newStatusId <= 5)
                    {
                        updatedAirplane.StatusID = newStatusId;
                        changesMade = true;
                    }
                    else
                    {
                        Console.WriteLine("❌ Неверный ID статуса!");
                    }
                    break;

                case "6":
                    Console.WriteLine("Доступные ворота:");
                    var gates = await GetAllGatesWithAirports();
                    foreach (var gate in gates)
                    {
                        Console.WriteLine($"   {gate.GateID}. {gate.GateName} ({gate.GateType}) - {gate.Airport?.AirportName ?? "Неизвестный аэропорт"}");
                    }
                    Console.Write("Введите ID ворот (или 0 для снятия с ворот): ");
                    if (int.TryParse(Console.ReadLine(), out int newGateId))
                    {
                        updatedAirplane.GateID = newGateId > 0 ? newGateId : null;
                        changesMade = true;
                    }
                    else
                    {
                        Console.WriteLine("❌ Неверный ID ворот!");
                    }
                    break;

                case "7":
                    Console.Write("Введите новую дату производства (гггг-мм-дд): ");
                    string dateInput = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(dateInput))
                    {
                        if (DateTime.TryParse(dateInput, out DateTime newDate))
                        {
                            updatedAirplane.ManufactureDate = newDate;
                            changesMade = true;
                        }
                        else
                        {
                            Console.WriteLine("❌ Неверный формат даты!");
                        }
                    }
                    break;

                case "8":
                    // Редактирование всех полей
                    Console.Write($"Модель ({airplane.Model}): ");
                    string fullModel = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(fullModel))
                        updatedAirplane.Model = fullModel;

                    Console.Write($"Авиакомпания ({airplane.Airline}): ");
                    string fullAirline = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(fullAirline))
                        updatedAirplane.Airline = fullAirline;

                    Console.Write($"Регистрационный номер ({airplane.RegistrationNumber}): ");
                    string fullRegistration = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(fullRegistration))
                        updatedAirplane.RegistrationNumber = fullRegistration;

                    Console.Write($"Вместимость ({airplane.Capacity}): ");
                    if (int.TryParse(Console.ReadLine(), out int fullCapacity) && fullCapacity > 0)
                        updatedAirplane.Capacity = fullCapacity;

                    Console.Write($"Статус ID ({airplane.StatusID}): ");
                    string fullStatusInput = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(fullStatusInput))
                    {
                        if (int.TryParse(fullStatusInput, out int fullStatusId) && fullStatusId >= 1 && fullStatusId <= 5)
                            updatedAirplane.StatusID = fullStatusId;
                    }

                    Console.Write($"ID ворот ({airplane.GateID}): ");
                    string fullGateInput = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(fullGateInput))
                    {
                        if (int.TryParse(fullGateInput, out int fullGateId) && fullGateId > 0)
                            updatedAirplane.GateID = fullGateId;
                        else if (fullGateInput == "0")
                            updatedAirplane.GateID = null;
                    }

                    Console.Write($"Дата производства ({airplane.ManufactureDate?.ToString("dd.MM.yyyy") ?? "не указана"}): ");
                    string fullManufactureInput = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(fullManufactureInput))
                    {
                        if (DateTime.TryParse(fullManufactureInput, out DateTime fullManufacture))
                            updatedAirplane.ManufactureDate = fullManufacture;
                    }

                    Console.Write($"Последнее обслуживание ({airplane.LastMaintenanceDate?.ToString("dd.MM.yyyy") ?? "не указана"}): ");
                    string fullLastMaintenanceInput = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(fullLastMaintenanceInput))
                    {
                        if (DateTime.TryParse(fullLastMaintenanceInput, out DateTime fullLastMaintenance))
                            updatedAirplane.LastMaintenanceDate = fullLastMaintenance;
                    }

                    Console.Write($"Следующее обслуживание ({airplane.NextMaintenanceDate?.ToString("dd.MM.yyyy") ?? "не указана"}): ");
                    string fullNextMaintenanceInput = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(fullNextMaintenanceInput))
                    {
                        if (DateTime.TryParse(fullNextMaintenanceInput, out DateTime fullNextMaintenance))
                            updatedAirplane.NextMaintenanceDate = fullNextMaintenance;
                    }

                    changesMade = true;
                    break;

                case "0":
                    Console.WriteLine("Редактирование отменено.");
                    Console.ReadKey();
                    return;

                default:
                    Console.WriteLine("Неверный выбор!");
                    Console.ReadKey();
                    return;
            }

            if (changesMade)
            {
                Console.WriteLine("\n📋 НОВЫЕ ДАННЫЕ:");
                Console.WriteLine($"Модель: {updatedAirplane.Model}");
                Console.WriteLine($"Авиакомпания: {updatedAirplane.Airline}");
                Console.WriteLine($"Регистрация: {updatedAirplane.RegistrationNumber}");
                Console.WriteLine($"Вместимость: {updatedAirplane.Capacity}");
                Console.WriteLine($"Статус: {updatedAirplane.Status}");
                if (updatedAirplane.GateID.HasValue)
                    Console.WriteLine($"Ворота ID: {updatedAirplane.GateID}");
                if (updatedAirplane.ManufactureDate.HasValue)
                    Console.WriteLine($"Дата производства: {updatedAirplane.ManufactureDate.Value:dd.MM.yyyy}");
                if (updatedAirplane.LastMaintenanceDate.HasValue)
                    Console.WriteLine($"Последнее обслуживание: {updatedAirplane.LastMaintenanceDate.Value:dd.MM.yyyy}");
                if (updatedAirplane.NextMaintenanceDate.HasValue)
                    Console.WriteLine($"Следующее обслуживание: {updatedAirplane.NextMaintenanceDate.Value:dd.MM.yyyy}");

                Console.Write("\nСохранить изменения? (y/n): ");
                string saveConfirmation = Console.ReadLine();

                if (saveConfirmation.ToLower() == "y" || saveConfirmation.ToLower() == "да")
                {
                    var success = await airplaneRepository.UpdateAirplaneAsync(updatedAirplane);
                    if (success)
                    {
                        Console.WriteLine("✅ Самолет успешно обновлен!");

                        // Показываем обновленные данные
                        var updatedAirplaneInfo = await airplaneRepository.GetAirplaneWithDetailsAsync(airplaneId);
                        if (updatedAirplaneInfo != null)
                        {
                            Console.WriteLine("\n📋 ОБНОВЛЕННЫЕ ДАННЫЕ:");
                            Console.WriteLine($"ID: {updatedAirplaneInfo.AirplaneID}");
                            Console.WriteLine($"Модель: {updatedAirplaneInfo.Model}");
                            Console.WriteLine($"Авиакомпания: {updatedAirplaneInfo.Airline}");
                            Console.WriteLine($"Регистрация: {updatedAirplaneInfo.RegistrationNumber}");
                            Console.WriteLine($"Вместимость: {updatedAirplaneInfo.Capacity}");
                            if (updatedAirplaneInfo.Status != null)
                                Console.WriteLine($"Статус: {updatedAirplaneInfo.Status.StatusName}");
                            if (updatedAirplaneInfo.Gate != null)
                                Console.WriteLine($"Ворота: {updatedAirplaneInfo.Gate.GateName}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("❌ Не удалось обновить самолет.");
                    }
                }
                else
                {
                    Console.WriteLine("❌ Изменения отменены.");
                }
            }
            else
            {
                Console.WriteLine("❌ Изменения не были внесены.");
            }
        }
        else
        {
            Console.WriteLine("❌ Неверный формат ID самолета!");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Ошибка при редактировании: {ex.Message}");
    }

    Console.WriteLine("\nНажмите любую клавишу для продолжения...");
    Console.ReadKey();
}




async Task AssignAirplaneToGate()
{
    Console.Clear();
    Console.WriteLine("🚪 НАЗНАЧЕНИЕ САМОЛЕТА К ВОРОТАМ");
    Console.WriteLine("====================================");

    try
    {
        // Показываем текущие самолеты
        var airplanes = await airplaneRepository.GetAllAirplanesAsync();
        if (!airplanes.Any())
        {
            Console.WriteLine("Самолеты не найдены.");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("Текущие самолеты:");
        foreach (var airplane in airplanes)
        {
            Console.WriteLine($"ID: {airplane.AirplaneID} - {airplane.Model} (Ворота: {airplane.Gate?.GateName ?? "Не назначены"})");
        }

        Console.Write("\nВведите ID самолета: ");
        if (int.TryParse(Console.ReadLine(), out int airplaneId))
        {
            // Показываем доступные ворота
            Console.WriteLine("\n🚪 ДОСТУПНЫЕ ВОРОТА:");
            var gates = await gateRepository.GetAllGatesWithAirportsAsync();
            if (gates.Any())
            {
                foreach (var gate in gates)
                {
                    Console.WriteLine($"   {gate.GateID}. {gate.GateName} ({gate.GateType}) - {gate.Airport?.AirportName ?? "Неизвестный аэропорт"}");
                }
            }
            else
            {
                Console.WriteLine("   Ворота не найдены");
            }
            
            Console.Write("\nВведите ID ворот (или 0 для снятия с ворот): ");
            if (int.TryParse(Console.ReadLine(), out int gateId))
            {
                int? gateIdParam = gateId > 0 ? gateId : null;
                var success = await airplaneRepository.AssignAirplaneToGateAsync(airplaneId, gateIdParam);
                if (success)
                {
                    if (gateIdParam.HasValue)
                    {
                        Console.WriteLine($"✅ Самолет успешно назначен к воротам (ID: {gateIdParam.Value})!");
                    }
                    else
                    {
                        Console.WriteLine("✅ Самолет успешно снят с ворот!");
                    }
                }
                else
                {
                    if (gateIdParam.HasValue)
                    {
                        Console.WriteLine("❌ Не удалось назначить самолет к воротам.");
                    }
                    else
                    {
                        Console.WriteLine("❌ Не удалось снять самолет с ворот.");
                    }
                }
            }
            else
            {
                Console.WriteLine("Неверный формат ID ворот!");
            }
        }
        else
        {
            Console.WriteLine("Неверный формат ID самолета!");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Ошибка: {ex.Message}");
    }

    Console.WriteLine("\nНажмите любую клавишу для продолжения...");
    Console.ReadKey();
}


async Task DeleteAirplane()
{
    Console.Clear();
    Console.WriteLine("🗑️ УДАЛЕНИЕ САМОЛЕТА");
    Console.WriteLine("========================");

    try
    {
        // Показываем текущие самолеты
        var airplanes = await airplaneRepository.GetAllAirplanesAsync();
        if (!airplanes.Any())
        {
            Console.WriteLine("Самолеты не найдены.");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("Текущие самолеты:");
        foreach (var airplane in airplanes)
        {
            Console.WriteLine($"ID: {airplane.AirplaneID} - {airplane.Model} ({airplane.Status?.StatusName ?? "Без статуса"})");
        }

        Console.Write("\nВведите ID самолета для удаления: ");
        if (int.TryParse(Console.ReadLine(), out int airplaneId))
        {
            Console.Write($"Вы уверены, что хотите удалить самолет с ID {airplaneId}? (y/n): ");
            string confirmation = Console.ReadLine();

            if (confirmation.ToLower() == "y" || confirmation.ToLower() == "да")
            {
                var success = await airplaneRepository.DeleteAirplaneAsync(airplaneId);
                if (success)
                {
                    Console.WriteLine("✅ Самолет успешно удален!");
                }
                else
                {
                    Console.WriteLine("❌ Не удалось удалить самолет.");
                }
            }
            else
            {
                Console.WriteLine("Операция отменена.");
            }
        }
        else
        {
            Console.WriteLine("Неверный формат ID самолета!");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Ошибка: {ex.Message}");
    }

    Console.WriteLine("\nНажмите любую клавишу для продолжения...");
    Console.ReadKey();
}


async Task ShowStatistics()
{
    Console.Clear();
    Console.WriteLine("📈 СТАТИСТИКА АЭРОПОРТА");
    Console.WriteLine("========================");

    try
    {
        var airports = await airportRepository.GetAllAirportsAsync();
        var statuses = await statusRepository.GetAllStatusesAsync();
        var airplanes = await airplaneRepository.GetAllAirplanesAsync();

        Console.WriteLine($"Всего аэропортов: {airports.Count()}");
        Console.WriteLine($"Всего статусов: {statuses.Count()}");
        Console.WriteLine($"Всего самолетов: {airplanes.Count()}");

        // Статистика по авиакомпаниям
        var airlines = airplanes.GroupBy(a => a.Airline).OrderByDescending(g => g.Count());
        if (airlines.Any())
        {
            Console.WriteLine("\nПо авиакомпаниям:");
            foreach (var airline in airlines)
            {
                Console.WriteLine($"   {airline.Key}: {airline.Count()} самолетов");
            }
        }

        // Статистика по статусам
        var statusGroups = airplanes.GroupBy(a => a.Status?.StatusName ?? "Неизвестно").OrderByDescending(g => g.Count());
        if (statusGroups.Any())
        {
            Console.WriteLine("\nПо статусам:");
            foreach (var statusGroup in statusGroups)
            {
                Console.WriteLine($"   {statusGroup.Key}: {statusGroup.Count()} самолетов");
            }
        }

        // Статистика по аэропортам
        var airportGroups = airplanes.Where(a => a.Gate?.Airport != null)
                                   .GroupBy(a => a.Gate.Airport.AirportName)
                                   .OrderByDescending(g => g.Count());
        if (airportGroups.Any())
        {
            Console.WriteLine("\nПо аэропортам:");
            foreach (var airportGroup in airportGroups)
            {
                Console.WriteLine($"   {airportGroup.Key}: {airportGroup.Count()} самолетов");
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Ошибка при получении статистики: {ex.Message}");
    }

    Console.WriteLine("\nНажмите любую клавишу для продолжения...");
    Console.ReadKey();
}

async Task<IEnumerable<Gate>> GetAllGatesWithAirports()
{
    return await gateRepository.GetAllGatesWithAirportsAsync();
}

// Вспомогательный метод для отображения детальной информации о самолёте
void ShowAirplaneDetails(Airplane airplane)
{
    Console.Clear();
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("╔════════════════════════════════════════════════════════════════════════════╗");
    Console.WriteLine("║                      📋 ПОДРОБНАЯ ИНФОРМАЦИЯ О САМОЛЁТЕ                    ║");
    Console.WriteLine("╚════════════════════════════════════════════════════════════════════════════╝");
    Console.ResetColor();
    Console.WriteLine();

    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("┌─────────────────────────────┬──────────────────────────────────────────────┐");
    Console.WriteLine("│ Параметр                    │ Значение                                     │");
    Console.WriteLine("├─────────────────────────────┼──────────────────────────────────────────────┤");
    Console.ResetColor();

    PrintDetailRow("ID самолёта", airplane.AirplaneID.ToString());
    PrintDetailRow("Модель", airplane.Model);
    PrintDetailRow("Авиакомпания", airplane.Airline);
    PrintDetailRow("Регистрационный номер", airplane.RegistrationNumber);
    PrintDetailRow("Вместимость", $"{airplane.Capacity} пассажиров");
    
    if (airplane.Status != null)
    {
        PrintDetailRow("Статус", $"{airplane.Status.StatusName} - {airplane.Status.StatusDescription}");
    }
    
    if (airplane.Gate != null)
    {
        PrintDetailRow("Ворота", airplane.Gate.GateName);
        PrintDetailRow("Тип ворот", airplane.Gate.GateType);
        if (airplane.Gate.Airport != null)
        {
            PrintDetailRow("Аэропорт", $"{airplane.Gate.Airport.AirportCode} - {airplane.Gate.Airport.AirportName}");
            PrintDetailRow("Город", $"{airplane.Gate.Airport.City}, {airplane.Gate.Airport.Country}");
        }
    }
    else
    {
        PrintDetailRow("Ворота", "Не назначены");
    }

    if (airplane.ManufactureDate.HasValue)
        PrintDetailRow("Дата производства", airplane.ManufactureDate.Value.ToString("dd.MM.yyyy"));
    
    if (airplane.LastMaintenanceDate.HasValue)
        PrintDetailRow("Последнее обслуживание", airplane.LastMaintenanceDate.Value.ToString("dd.MM.yyyy"));
    
    if (airplane.NextMaintenanceDate.HasValue)
        PrintDetailRow("Следующее обслуживание", airplane.NextMaintenanceDate.Value.ToString("dd.MM.yyyy"));
    
    PrintDetailRow("Дата создания записи", airplane.CreatedDate.ToString("dd.MM.yyyy HH:mm:ss"));

    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("└─────────────────────────────┴──────────────────────────────────────────────┘");
    Console.ResetColor();

    Console.WriteLine("\nНажмите любую клавишу для возврата...");
    Console.ReadKey();
}

// Вспомогательный метод для печати строки деталей
void PrintDetailRow(string parameter, string value)
{
    Console.ForegroundColor = ConsoleColor.White;
    Console.Write($"│ {TruncateString(parameter, 27),-27} │ ");
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine($"{TruncateString(value, 44),-44} │");
    Console.ResetColor();
}

// Вспомогательный метод для обрезки строк
string TruncateString(string text, int maxLength)
{
    if (string.IsNullOrEmpty(text))
        return string.Empty;
    
    if (text.Length <= maxLength)
        return text;
    
    return text.Substring(0, maxLength - 3) + "...";
}

async Task ShowLoadingAnimation()
{
    Console.Clear();
    Console.ForegroundColor = ConsoleColor.Cyan;
    
    // Заголовок
    Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
    Console.WriteLine("║                    🚀 AIRPORT MANAGEMENT SYSTEM 🚀          ║");
    Console.WriteLine("║                        Загрузка системы...                   ║");
    Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
    Console.WriteLine();
    
    // Список забавных сообщений
    string[] loadingMessages = {
        "🔄 Инициализация системы управления аэропортом...",
        "☕ Сергей 1 наливает чай...",
        "💬 Сергей 2 пишет девушке...",
        "💻 Егор пишет код...",
        "📊 Подключение к базе данных...",
        "🛫 Загрузка информации о самолетах...",
        "🚪 Проверка состояния ворот...",
        "🏢 Анализ аэропортов...",
        "⚡ Оптимизация производительности...",
        "🎯 Готово! Система запущена!"
    };
    
    // Анимация прогресс-бара
    int totalSteps = loadingMessages.Length;
    
    for (int i = 0; i < totalSteps; i++)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write($"\r[{new string('█', i)}{new string('░', totalSteps - i)}] {100 * i / totalSteps}% ");
        
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\n{loadingMessages[i]}");
        
        // Анимация точек
        for (int j = 0; j < 3; j++)
        {
            Console.Write(".");
            await Task.Delay(200);
        }
        
        Console.WriteLine();
        await Task.Delay(800);
        
        // Очищаем строку для следующего шага
        if (i < totalSteps - 1)
        {
            Console.SetCursorPosition(0, Console.CursorTop - 2);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, Console.CursorTop - 1);
        }
    }
    
    // Финальная анимация
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("\n🎉 Система успешно загружена!");
    
    // Мигающий эффект
    for (int i = 0; i < 3; i++)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("✨ ГОТОВО К РАБОТЕ! ✨");
        await Task.Delay(300);
        
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine("✨ ГОТОВО К РАБОТЕ! ✨");
        await Task.Delay(300);
    }
    
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine("\nНажмите любую клавишу для продолжения...");
    Console.ReadKey();
    Console.Clear();
}

