using System.Linq.Expressions;
using Domain.Common;
using Domain.Specs.Interfaces;
using Domain.Specs.Models;

namespace Domain.Specs;

public static class SpecificationBuilderExtension
{
    public static ISpecificationBuilder<T> Where<T>(this ISpecificationBuilder<T> builder, Expression<Func<T, bool>> criteria)
        where T : BaseEntity
    {
        builder.Spec!.Criteria = criteria;

        return builder;
    }

    public static ISpecificationBuilder<T> Combine<T>(this ISpecificationBuilder<T> builder, Expression<Func<T, bool>> criteria, BinaryExpressionType type)
        where T : BaseEntity
    {
        builder.Spec!.CombineExpression(criteria, type);

        return builder;
    }

    public static ISpecificationBuilder<T> AsSplitQuery<T>(this ISpecificationBuilder<T> builder)
        where T : BaseEntity
    {
        builder.Spec!.IsSplitQuery = true;

        return builder;
    }

    public static ISpecificationBuilder<T> AsNoTracking<T>(this ISpecificationBuilder<T> builder)
        where T : BaseEntity
    {
        builder.Spec!.IsNoTracking = true;

        return builder;
    }

    public static IIncludableSpecificationBuilder<T, TProperty> Include<T, TProperty>(this ISpecificationBuilder<T> builder, Expression<Func<T, TProperty>> includeExpression)
        where T : BaseEntity
    {
        IncludeInfo includeInfo = new()
        {
            EntityType = typeof(T),
            InCludeType = InCludeType.Include,
            LamdaExpression = includeExpression,
            PropertyType = typeof(TProperty),
        };

        builder.Spec!.Includes.Add(includeInfo);

        return new IncludableSpecificationBuilder<T, TProperty>(builder.Spec);
    }

    public static IIncludableSpecificationBuilder<T, TProperty> ThenInclude<T, TPreviousProperty, TProperty>(
        this IIncludableSpecificationBuilder<T, TPreviousProperty> builder,
        Expression<Func<TPreviousProperty, TProperty>> thenIncludeExpression)
       where T : BaseEntity
    {
        return ThenIncludeBase(thenIncludeExpression, builder);
    }

    public static IIncludableSpecificationBuilder<T, TProperty> ThenInclude<T, TPreviousProperty, TProperty>(
        this IIncludableSpecificationBuilder<T, ICollection<TPreviousProperty>> builder,
        Expression<Func<TPreviousProperty, TProperty>> thenIncludeExpression)
       where T : BaseEntity
    {
        return ThenIncludeBase(thenIncludeExpression, Collectionbuilder: builder);
    }

    public static IIncludableSpecificationBuilder<T, TProperty> ThenIncludeBase<T, TPreviousProperty, TProperty>(
        Expression<Func<TPreviousProperty, TProperty>> thenIncludeExpression,
        IIncludableSpecificationBuilder<T, TPreviousProperty> builder = null!,
        IIncludableSpecificationBuilder<T, ICollection<TPreviousProperty>> Collectionbuilder = null!)
       where T : BaseEntity
    {
        IncludeInfo includeInfo = new()
        {
            EntityType = typeof(T),
            InCludeType = InCludeType.ThenInclude,
            LamdaExpression = thenIncludeExpression,
            PreviousPropertyType = typeof(TPreviousProperty),
            PropertyType = typeof(TProperty),
        };

        Specification<T>? Spec = builder != null ? builder.Spec : Collectionbuilder.Spec;

        Spec!.Includes.Add(includeInfo);

        return new IncludableSpecificationBuilder<T, TProperty>(Spec);
    }
}