﻿namespace Projector.Specs
{
    using System;
    using Projector.ObjectModel;

    public interface ITypeCut
    {
        ITypeCut OfKind(TypeKind kind);
        ITypeCut OfKind(params TypeKind[] kinds);

        ITypeCut Matching(Func<Type, bool> predicate);

        void Spec(Action<ITypeScope> spec);
    }
}