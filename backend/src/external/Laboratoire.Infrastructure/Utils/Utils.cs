using System.Data;
using System.Text;
using Dapper;

namespace Laboratoire.Infrastructure.Utils;

public class Utils
{
    public static (string sql, DynamicParameters parameters) BulkSqlStatement<T>(IEnumerable<T> entities, string tableName, string? schema)
    {

        var allowedSchemas = new HashSet<string> { "inventory", "document", };
        var allowedTables = new HashSet<string> { "chemical_hazard", "crop_protocol", "protocol" };

        if (!string.IsNullOrEmpty(schema) && !allowedSchemas.Contains(schema))
            throw new ArgumentException($"Invalid schema : {schema}");

        if (!allowedTables.Contains(tableName))
            throw new ArgumentException($"Invalid table: {tableName}");

        DynamicParameters parameters = new DynamicParameters();
        StringBuilder sqlBuilder = new StringBuilder();

        var arr = entities.ToArray();
        var properties = typeof(T).GetProperties();

        string fullTableName = !string.IsNullOrEmpty(schema) ? $"{schema}.{tableName}" : $"{tableName}";
        sqlBuilder.Append($"INSERT INTO {fullTableName} (");

        sqlBuilder.Append(string.Join(", ", properties.Select(p => $"{ToSnakeCase(p.Name)}")));
        sqlBuilder.Append(") VALUES ");

        // Criando parâmetros seguros
        for (var i = 0; i < arr.Length; i++)
        {
            sqlBuilder.Append("(");
            for (var j = 0; j < properties.Length; j++)
            {
                var property = properties[j];
                string paramName = $"@{property.Name}{i}";
                sqlBuilder.Append(paramName);
                if (j < properties.Length - 1) sqlBuilder.Append(", ");

                object? value = property.GetValue(arr[i]);
                DbType dbType = GetDbType(property.PropertyType);

                if (value is DateTime dtVal)
                {
                    // Verifica se o valor de DateTime não é UTC e o converte
                    if (dtVal.Kind != DateTimeKind.Utc)
                    {
                        value = dtVal.ToUniversalTime();
                    }
                }

                parameters.Add(paramName, value, dbType);
            }
            sqlBuilder.Append(i < arr.Length - 1 ? "), " : ");");
        }

        return (sqlBuilder.ToString(), parameters);
    }

    private static DbType GetDbType(Type type)
    {
        if (type == typeof(int) || type == typeof(int?)) return DbType.Int32;
        if (type == typeof(long) || type == typeof(long?)) return DbType.Int64;
        if (type == typeof(string)) return DbType.String;
        if (type == typeof(decimal) || type == typeof(decimal?)) return DbType.Decimal;
        if (type == typeof(double) || type == typeof(double?)) return DbType.Double;
        if (type == typeof(DateTime) || type == typeof(DateTime?)) return DbType.DateTime;
        if (type == typeof(DateOnly) || type == typeof(DateOnly?)) return DbType.Date;
        if (type == typeof(bool) || type == typeof(bool?)) return DbType.Boolean;
        return DbType.Object;
    }

    private static string ToSnakeCase(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return input;

        var stringBuilder = new StringBuilder();
        for (int i = 0; i < input.Length; i++)
        {
            if (char.IsUpper(input[i]) && i > 0)
            {
                stringBuilder.Append('_');
                stringBuilder.Append(char.ToLowerInvariant(input[i]));
                continue;
            }
            
            if (char.IsUpper(input[i]))
            {
                stringBuilder.Append(char.ToLowerInvariant(input[i]));
                continue;
            }

            stringBuilder.Append(input[i]);

        }

        return stringBuilder.ToString();
    }
}
