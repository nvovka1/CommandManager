using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Nvovka.CommandManager.Contract.Models;

namespace Nvovka.CommandManager.Data.Repository;
public interface ICommandDupperRepository
{
    Task<int> InsertOrderAsync(CommandItem command);
    Task<CommandItem?> GetCommandItemAsync(int id);
}

public class CommandDupperRepository: ICommandDupperRepository
{
    private readonly string _connectionString;

    public CommandDupperRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

    // Insert Order with Items
    public async Task<int> InsertOrderAsync(CommandItem command)
    {
        const string insertCommandSql = @"
            INSERT INTO CommandItems (Name, Description, CreatedDate, ModifiedDate, Status)
            VALUES (@Name, @Description, @CreatedDate, @ModifiedDate, @Status);
            SELECT CAST(SCOPE_IDENTITY() as int);";

        const string insertOrderItemSql = @"
            INSERT INTO CommandReferenceItems (CommandItemId, Description, CreatedDate, ModifiedDate)
            VALUES (@CommandItemId, @Description, @CreatedDate, @ModifiedDate);";

        using var connection = CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            // Insert Order
            command.Id = await connection.QuerySingleAsync<int>(
                insertCommandSql,
                command,
                transaction);
            ////if(command.CommandReferenceItems?.Count == 0)
            ////    return command.Id;

            foreach (var referenceItem in command?.CommandReferenceItems)
            {
                referenceItem.CommandItemId = command.Id;
                await connection.ExecuteAsync(insertOrderItemSql,
                    referenceItem,
                    transaction);
            }

            transaction.Commit();
            return command.Id;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    // Retrieve Order with Items
    public async Task<CommandItem?> GetCommandItemAsync(int id)
    {
        const string commandSql = "SELECT * FROM CommandItems WHERE Id = @Id;";
        const string comandItemsSql = "SELECT * FROM CommandReferenceItems WHERE CommandItemId = @CommandItemId;";

        using var connection = CreateConnection();
        var command = await connection.QuerySingleOrDefaultAsync<CommandItem>(commandSql, new { Id = id });

        if (command != null)
        {
            var items = await connection.QueryAsync<CommandReferenceItem>(comandItemsSql, new { CommandItemId = id });
            command.CommandReferenceItems = items.ToList();
        }

        return command;
    }

    ////// Update Order
    ////public async Task UpdateOrderAsync(Order order)
    ////{
    ////    const string updateOrderSql = @"
    ////        UPDATE Orders SET CustomerName = @CustomerName, OrderDate = @OrderDate
    ////        WHERE OrderId = @OrderId;";

    ////    const string deleteItemsSql = "DELETE FROM OrderItems WHERE OrderId = @OrderId;";
    ////    const string insertOrderItemSql = @"
    ////        INSERT INTO OrderItems (OrderId, ProductName, Quantity)
    ////        VALUES (@OrderId, @ProductName, @Quantity);";

    ////    using var connection = CreateConnection();
    ////    connection.Open();
    ////    using var transaction = connection.BeginTransaction();

    ////    try
    ////    {
    ////        // Update Order
    ////        await connection.ExecuteAsync(updateOrderSql, order, transaction);

    ////        // Delete existing items and insert updated items
    ////        await connection.ExecuteAsync(deleteItemsSql, new { OrderId = order.OrderId }, transaction);

    ////        foreach (var item in order.Items)
    ////        {
    ////            await connection.ExecuteAsync(insertOrderItemSql,
    ////                new { OrderId = order.OrderId, item.ProductName, item.Quantity },
    ////                transaction);
    ////        }

    ////        transaction.Commit();
    ////    }
    ////    catch
    ////    {
    ////        transaction.Rollback();
    ////        throw;
    ////    }
    ////}

    ////// Delete Order with Items
    ////public async Task DeleteOrderAsync(int orderId)
    ////{
    ////    const string deleteItemsSql = "DELETE FROM OrderItems WHERE OrderId = @OrderId;";
    ////    const string deleteOrderSql = "DELETE FROM Orders WHERE OrderId = @OrderId;";

    ////    using var connection = CreateConnection();
    ////    connection.Open();
    ////    using var transaction = connection.BeginTransaction();

    ////    try
    ////    {
    ////        await connection.ExecuteAsync(deleteItemsSql, new { OrderId = orderId }, transaction);
    ////        await connection.ExecuteAsync(deleteOrderSql, new { OrderId = orderId }, transaction);
    ////        transaction.Commit();
    ////    }
    ////    catch
    ////    {
    ////        transaction.Rollback();
    ////        throw;
    ////    }
    ////}
}
