using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Data.Common;

namespace Ordering.API.Helpers;

public class SqlConnectionHealthCheck : IHealthCheck 
{ 
    private static readonly string DefaultTestQuery = "Select 1"; 

    public string ConnectionString { get; } 
    public string TestQuery { get; } 

    public SqlConnectionHealthCheck(string connectionString) 
        : this(connectionString, testQuery: DefaultTestQuery) 
    { 
    } 
    
    public SqlConnectionHealthCheck(string connectionString, string testQuery) 
    { 
        ConnectionString = connectionString; 
        TestQuery = testQuery; } 
    
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default(CancellationToken)) 
    { 
        using (var connection = new SqlConnection(ConnectionString)) 
        { 
            try { await connection.OpenAsync(cancellationToken); 
                if (TestQuery != null) 
                { 
                    var command = connection.CreateCommand(); 
                    command.CommandText = TestQuery; 
                    await command.ExecuteNonQueryAsync(cancellationToken); 
                } 
            } catch (DbException ex) 
            { 
                return new HealthCheckResult(status: context.Registration.FailureStatus, exception: ex); 
            } 
        } 
        
        return HealthCheckResult.Healthy(); 
    } 
}
