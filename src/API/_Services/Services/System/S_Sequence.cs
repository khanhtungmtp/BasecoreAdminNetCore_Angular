
using System.Data.Common;
using API._Repositories;
using API._Services.Interfaces.System;
using API.Data;
using Microsoft.EntityFrameworkCore;

namespace API._Services.Services.System;
public class S_Sequence(IRepositoryAccessor repoStore, DataContext context) : BaseServices(repoStore), I_Sequence
{
    private readonly DataContext _context = context;

    public async Task<int> GetNextSequenceValueAsync()
    {
        DbConnection? connection = _context.Database.GetDbConnection();
        try
        {
            await connection.OpenAsync();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT NEXT VALUE FOR Forumsequence;";

            // Assuming the sequence returns INT type. Adjust the type according to your sequence's type.
            object? result = await command.ExecuteScalarAsync();
            return result != null ? Convert.ToInt32(result) : 0;
        }
        finally
        {
            await connection.CloseAsync();
        }
    }
}
