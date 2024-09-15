using System.Data.Common;

namespace Contracts.Common;

public record SharedTransaction(DbTransaction Transaction, DbConnection Connection);
