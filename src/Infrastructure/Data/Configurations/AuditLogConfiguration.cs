using Domain.Aggregates.AuditLogs;
using Domain.Common.ElasticConfigurations;

namespace Infrastructure.Data.Configurations;

public class AuditLogConfiguration : IElasticsearchDocumentConfigure<AuditLog>
{
    public void Configure(ref ElasticsearchConfigBuilder<AuditLog> buider)
    {
        buider.ToIndex();
        buider.HasKey(key => key.Id);

        buider.Properties(config =>
            config
                .Text(
                    t => t.Id,
                    config =>
                        config.Fields(f =>
                            f.Keyword(
                                ElsIndexExtension.GetKeywordName<AuditLog>(n => n.ActionPerformBy!)
                            )
                        )
                )
                .Text(
                    txt => txt.Entity,
                    config =>
                        config.Fields(f =>
                            f.Keyword(ElsIndexExtension.GetKeywordName<AuditLog>(n => n.Entity!))
                        )
                )
                .ByteNumber(b => b.Type)
                .Object(o => o.OldValue!)
                .Object(o => o.NewValue!)
                .Text(
                    txt => txt.ActionPerformBy!,
                    config =>
                        config.Fields(field =>
                            field.Keyword(
                                ElsIndexExtension.GetKeywordName<AuditLog>(name =>
                                    name.ActionPerformBy!
                                )
                            )
                        )
                )
                .Date(d => d.CreatedAt)
                .Nested(
                    n => n.Agent!,
                    nest =>
                        nest.Properties(nestProp =>
                            nestProp
                                .Text(
                                    t => t.Id,
                                    config =>
                                        config.Fields(f =>
                                            f.Keyword(
                                                ElsIndexExtension.GetKeywordName<Agent>(name =>
                                                    name.Id
                                                )
                                            )
                                        )
                                )
                                .Text(
                                    t => t.Agent!.FirstName!,
                                    config =>
                                        config.Fields(f =>
                                            f.Keyword(
                                                ElsIndexExtension.GetKeywordName<Agent>(n =>
                                                    n.FirstName!
                                                )
                                            )
                                        )
                                )
                                .Text(
                                    t => t.Agent!.LastName!,
                                    config =>
                                        config.Fields(f =>
                                            f.Keyword(
                                                ElsIndexExtension.GetKeywordName<Agent>(n =>
                                                    n.LastName!
                                                )
                                            )
                                        )
                                )
                                .Text(t => t.Agent!.Email!)
                                .Date(d => d.Agent!.DayOfBirth!)
                                .ByteNumber(b => b.Agent!.Gender!)
                                .Date(x => x.Agent!.CreatedAt)
                        )
                )
        );
    }
}
