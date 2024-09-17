using System.Data.Common;

namespace Contracts.Dtos.Models;

public record SharedTransaction(DbTransaction Transaction, DbConnection Connection);
