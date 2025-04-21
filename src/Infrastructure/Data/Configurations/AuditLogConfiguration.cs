using Domain.Aggregates.AuditLogs;
using Elastic.Clients.Elasticsearch.Analysis;
using FluentConfiguration.Configurations;

namespace Infrastructure.Data.Configurations;

public class AuditLogConfiguration : IElasticsearchDocumentConfigure<AuditLog>
{
    public void Configure(ref ElasticsearchConfigBuilder<AuditLog> buider, string? prefix = null)
    {
        buider.ToIndex(prefix);
        buider.HasKey(key => key.Id);

        buider.Settings(setting =>
            setting.Analysis(x =>
                x.Analyzers(an =>
                        an.Custom(
                                "myTokenizer",
                                ca => ca.Filter(["lowercase"]).Tokenizer("myTokenizer")
                            )
                            .Custom(
                                "standardAnalyzer",
                                ca => ca.Filter(["lowercase"]).Tokenizer("standard")
                            )
                    )
                    .Tokenizers(tz =>
                        tz.NGram(
                            "myTokenizer",
                            config =>
                                config
                                    .MinGram(3)
                                    .MaxGram(4)
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
                            .SearchAnalyzer("standardAnalyzer")
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
                            .SearchAnalyzer("standardAnalyzer")
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
                            .SearchAnalyzer("standardAnalyzer")
                )
                .Keyword(d => d.CreatedAt)
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
                                            .SearchAnalyzer("standardAnalyzer")
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
                                            .SearchAnalyzer("standardAnalyzer")
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
                                            .SearchAnalyzer("standardAnalyzer")
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
                                            .SearchAnalyzer("standardAnalyzer")
                                )
                                .Date(d => d.Agent!.DayOfBirth!)
                                .ByteNumber(b => b.Agent!.Gender!)
                                .Keyword(x => x.Agent!.CreatedAt)
                        )
                )
        );
    }
}
