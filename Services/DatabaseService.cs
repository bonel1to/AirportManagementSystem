using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

public class DatabaseService
{
    private readonly string _connectionString;

    public DatabaseService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public virtual async Task<IEnumerable<T>> QueryAsync<T>(string sql, object parameters = null)
    {
        using var connection = new SqlConnection(_connectionString);
        return await connection.QueryAsync<T>(sql, parameters);
    }

    public virtual async Task<T> QueryFirstOrDefaultAsync<T>(string sql, object parameters = null)
    {
        using var connection = new SqlConnection(_connectionString);
        return await connection.QueryFirstOrDefaultAsync<T>(sql, parameters);
    }

    public virtual async Task<int> ExecuteAsync(string sql, object parameters = null)
    {
        using var connection = new SqlConnection(_connectionString);
        return await connection.ExecuteAsync(sql, parameters);
    }

    public virtual async Task<T> ExecuteScalarAsync<T>(string sql, object parameters = null)
    {
        using var connection = new SqlConnection(_connectionString);
        return await connection.ExecuteScalarAsync<T>(sql, parameters);
    }

    // Метод для запросов с несколькими типами
    public async Task<IEnumerable<TReturn>> QueryAsync<T1, T2, TReturn>(
        string sql,
        Func<T1, T2, TReturn> map,
        object parameters = null,
        string splitOn = "Id")
    {
        using var connection = new SqlConnection(_connectionString);
        return await connection.QueryAsync(sql, map, parameters, splitOn: splitOn);
    }

    // Метод для запросов с тремя типами
    public async Task<IEnumerable<TReturn>> QueryAsync<T1, T2, T3, TReturn>(
        string sql,
        Func<T1, T2, T3, TReturn> map,
        object parameters = null,
        string splitOn = "Id")
    {
        using var connection = new SqlConnection(_connectionString);
        return await connection.QueryAsync(sql, map, parameters, splitOn: splitOn);
    }

    // Метод для запросов с четырьмя типами
    public async Task<IEnumerable<TReturn>> QueryAsync<T1, T2, T3, T4, TReturn>(
        string sql,
        Func<T1, T2, T3, T4, TReturn> map,
        object parameters = null,
        string splitOn = "Id")
    {
        using var connection = new SqlConnection(_connectionString);
        return await connection.QueryAsync(sql, map, parameters, splitOn: splitOn);
    }

    // Метод для проверки подключения с подробной диагностикой
    public async Task<(bool Success, string ErrorMessage)> TestConnectionAsync()
    {
        try
        {
            Console.WriteLine($"Попытка подключения с строкой: {_connectionString}");

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            // Проверяем, что можем выполнить простой запрос
            var result = await connection.QueryFirstOrDefaultAsync<int>("SELECT 1");

            Console.WriteLine("✅ Подключение к базе данных успешно!");
            return (true, null);
        }
        catch (SqlException ex)
        {
            string errorMessage = $"Ошибка SQL Server: {ex.Message}";
            Console.WriteLine($"❌ {errorMessage}");
            Console.WriteLine($"Номер ошибки: {ex.Number}");
            Console.WriteLine($"Сервер: {ex.Server}");
            return (false, errorMessage);
        }
        catch (Exception ex)
        {
            string errorMessage = $"Общая ошибка подключения: {ex.Message}";
            Console.WriteLine($"❌ {errorMessage}");
            return (false, errorMessage);
        }
    }
}
