﻿namespace Projector.Specs
{
    using System;
    using Projector.ObjectModel;

    public interface ITypeCut : ITypeScopeBlock
    {
        ITypeCut OfKind(TypeKind kind);
        ITypeCut OfKind(params TypeKind[] kinds);

        ITypeCut Matching(Func<ProjectionType, bool> predicate);
    }
}
