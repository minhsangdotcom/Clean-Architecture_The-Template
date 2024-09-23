using Domain.Aggregates.AuditLogs;
using Domain.Common.ElasticConfigurations;
using Elastic.Clients.Elasticsearch.Analysis;

namespace Infrastructure.Data.Configurations;

public class AuditLogConfiguration : IElasticsearchDocumentConfigure<AuditLog>
{
    public void Configure(ref ElasticsearchConfigBuilder<AuditLog> buider)
    {
        buider.ToIndex();
        buider.HasKey(key => key.Id);

        buider.Settings(setting =>
            setting.Analysis(x =>
                x.Analyzers(an =>
                        an.Custom(
                            "myTokenizer",
                            ca => ca.Filter(["lowercase"]).Tokenizer("myTokenizer")
                        )
                    )
                    .Tokenizers(tz =>
                        tz.NGram(
                            "myTokenizer",
                            config =>
                                config
                                    .MinGram(2)
                                    .MaxGram(3)
                                    .TokenChars([TokenChar.Digit, TokenChar.Letter])
                        )
                    )
            )
        );

        buider.Properties(config =>
            config
                .Text(
                    t => t.Id,
                    config =>
                        config
                            .Fields(f =>
                                f.Keyword(
                                    ElsIndexExtension.GetKeywordName<AuditLog>(n =>
                                        n.ActionPerformBy!
                                    )
                                )
                            )
                            .Analyzer("myTokenizer")
                )
                .Text(
                    txt => txt.Entity,
                    config =>
                        config
                            .Fields(f =>
                                f.Keyword(
                                    ElsIndexExtension.GetKeywordName<AuditLog>(n => n.Entity!)
                                )
                            )
                            .Analyzer("myTokenizer")
                )
                .ByteNumber(b => b.Type)
                .Object(o => o.OldValue!)
                .Object(o => o.NewValue!)
                .Text(
                    txt => txt.ActionPerformBy!,
                    config =>
                        config
                            .Fields(field =>
                                field.Keyword(
                                    ElsIndexExtension.GetKeywordName<AuditLog>(name =>
                                        name.ActionPerformBy!
                                    )
                                )
                            )
                            .Analyzer("myTokenizer")
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
                                        config
                                            .Fields(f =>
                                                f.Keyword(
                                                    ElsIndexExtension.GetKeywordName<Agent>(name =>
                                                        name.Id
                                                    )
                                                )
                                            )
                                            .Analyzer("myTokenizer")
                                )
                                .Text(
                                    t => t.Agent!.FirstName!,
                                    config =>
                                        config
                                            .Fields(f =>
                                                f.Keyword(
                                                    ElsIndexExtension.GetKeywordName<Agent>(n =>
                                                        n.FirstName!
                                                    )
                                                )
                                            )
                                            .Analyzer("myTokenizer")
                                )
                                .Text(
                                    t => t.Agent!.LastName!,
                                    config =>
                                        config
                                            .Fields(f =>
                                                f.Keyword(
                                                    ElsIndexExtension.GetKeywordName<Agent>(n =>
                                                        n.LastName!
                                                    )
                                                )
                                            )
                                            .Analyzer("myTokenizer")
                                )
                                .Text(
                                    t => t.Agent!.Email!,
                                    config =>
                                        config
                                            .Fields(f =>
                                                f.Keyword(
                                                    ElsIndexExtension.GetKeywordName<Agent>(n =>
                                                        n.Email!
                                                    )
                                                )
                                            )
                                            .Analyzer("myTokenizer")
                                )
                                .Date(d => d.Agent!.DayOfBirth!)
                                .ByteNumber(b => b.Agent!.Gender!)
                                .Date(x => x.Agent!.CreatedAt)
                                .Nested(
                                    n => n.Agent!.Role!,
                                    config =>
                                        config.Properties(p =>
                                            p.Text(
                                                    t => t.Agent!.Role!.Name!,
                                                    config =>
                                                        config
                                                            .Fields(f =>
                                                                f.Keyword(
                                                                    ElsIndexExtension.GetKeywordName<RoleTest>(
                                                                        n => n.Name!
                                                                    )
                                                                )
                                                            )
                                                            .Analyzer("myTokenizer")
                                                )
                                                .Text(
                                                    t => t.Agent!.Role!.Guard!,
                                                    config =>
                                                        config
                                                            .Fields(f =>
                                                                f.Keyword(
                                                                    ElsIndexExtension.GetKeywordName<RoleTest>(
                                                                        n => n.Guard!
                                                                    )
                                                                )
                                                            )
                                                            .Analyzer("myTokenizer")
                                                )
                                                .Nested(
                                                    n => n.Agent!.Role!.Permissions!,
                                                    config =>
                                                        config.Properties(p =>
                                                            p.Text(
                                                                t =>
                                                                    t.Agent!.Role!.Permissions!.First().Name!,
                                                                config =>
                                                                    config
                                                                        .Fields(f =>
                                                                            f.Keyword(
                                                                                ElsIndexExtension.GetKeywordName<PermissionTest>(
                                                                                    n => n.Name!
                                                                                )
                                                                            )
                                                                        )
                                                                        .Analyzer("myTokenizer")
                                                            )
                                                        )
                                                )
                                        )
                                )
                        )
                )
        );
    }
}
